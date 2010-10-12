// ***********************************************************************
// <copyright file="Install.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Win32;

    using File = System.IO.File;

    /// <summary>Class containing methods to install updates</summary>
    public static class Install
    {
        #region Constants and Fields

        /// <summary>Gets an int that indicates to move a file on reboot</summary>
        private const int MoveOnReboot = 5;

        /// <summary>The localized name of the current update being installed</summary>
        private static string currentUpdateName;

        /// <summary>Indicates if an error has occurred</summary>
        private static bool errorOccurred;

        /// <summary>The total number of updates being installed</summary>
        private static int updateCount;

        /// <summary>The index position of the current update being installed</summary>
        private static int updateIndex;

        #endregion

        #region Events

        /// <summary>Occurs when the installation completed.</summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallCompleted;

        /// <summary>Occurs when the installation progress changed</summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether Seven Update is installing updates</summary>
        public static bool IsInstalling { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>Installs updates</summary>
        /// <param name="applications">The collection of applications to install updates</param>
        public static void InstallUpdates(Collection<Sui> applications)
        {
            if (applications == null)
            {
                return;
            }

            if (applications.Count < 1)
            {
                return;
            }

            IsInstalling = true;
            updateCount = applications.Sum(t => t.Updates.Count);
            int completedUpdates = 0, failedUpdates = 0;

            ReportProgress(0);

            for (var x = 0; x < applications.Count; x++)
            {
                for (var y = 0; y < applications[x].Updates.Count; y++)
                {
                    errorOccurred = false;
                    if (File.Exists(Utilities.AllUserStore + @"abort.lock"))
                    {
                        File.Delete(Utilities.AllUserStore + @"abort.lock");
                        return;
                    }

                    currentUpdateName = Utilities.GetLocaleString(applications[x].Updates[y].Name);
                    if (applications[x].AppInfo.Directory == Utilities.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, applications[x].AppInfo.Is64Bit))
                    {
                        try
                        {
                            Process.GetProcessesByName(@"SevenUpdate.Helper")[0].Kill();
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }

                    ReportProgress(5);

                    SetRegistryItems(applications[x].Updates[y].RegistryItems, applications[x].AppInfo.Is64Bit);

                    ReportProgress(25);

                    UpdateFiles(applications[x].Updates[y].Files, Utilities.AllUserStore + @"downloads\" + currentUpdateName + @"\");

                    ReportProgress(75);

                    SetShortcuts(applications[x].Updates[y].Shortcuts, applications[x].AppInfo);

                    ReportProgress(95);

                    if (errorOccurred)
                    {
                        failedUpdates++;
                        AddHistory(applications[x], applications[x].Updates[y]);
                    }
                    else
                    {
                        completedUpdates++;
                        AddHistory(applications[x], applications[x].Updates[y]);
                    }

                    if (applications[x].AppInfo.Directory == Utilities.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, true) && Utilities.RebootNeeded)
                    {
                        foreach (var t in applications[x].Updates[y].Files)
                        {
                            switch (t.Action)
                            {
                                case FileAction.Delete:
                                case FileAction.UnregisterThenDelete:
                                    try
                                    {
                                        File.Delete(t.Destination.PathAndQuery);
                                    }
                                    catch (IOException)
                                    {
                                        NativeMethods.MoveFileExW(t.Destination.PathAndQuery, null, MoveOnReboot);
                                    }
                                    catch (UnauthorizedAccessException)
                                    {
                                        NativeMethods.MoveFileExW(t.Destination.PathAndQuery, null, MoveOnReboot);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        Utilities.StartProcess(Utilities.AppDir + @"SevenUpdate.Helper.exe", "\"" + currentUpdateName + "\"");
                        IsInstalling = false;
                        return;
                    }

                    ReportProgress(100);

                    updateIndex++;
                }
            }

            ReportProgress(100);

            if (Utilities.RebootNeeded)
            {
                NativeMethods.MoveFileExW(Utilities.AllUserStore + "reboot.lock", null, MoveOnReboot);

                if (Directory.Exists(Utilities.AllUserStore + "downloads"))
                {
                    NativeMethods.MoveFileExW(Utilities.AllUserStore + "downloads", null, MoveOnReboot);
                }
            }
            else
            {
                // Delete the downloads directory if no errors were found and no reboot is needed
                if (!errorOccurred)
                {
                    if (Directory.Exists(Utilities.AllUserStore + "downloads"))
                    {
                        try
                        {
                            Directory.Delete(Utilities.AllUserStore + "downloads", true);
                        }
                        catch (IOException)
                        {
                        }
                    }
                }
            }

            IsInstalling = false;
            if (InstallCompleted != null)
            {
                InstallCompleted(null, new InstallCompletedEventArgs(completedUpdates, failedUpdates));
            }

            return;
        }

        #endregion

        #region Methods

        /// <summary>Adds an update to the update history</summary>
        /// <param name="appInfo">the application information</param>
        /// <param name="updateInfo">the update information</param>
        /// <param name="failed"><see langword = "true" /> if the update failed, otherwise <see langword = "false" /></param>
        private static void AddHistory(Sui appInfo, Update updateInfo, bool failed = false)
        {
            var history = Utilities.Deserialize<Collection<Suh>>(Utilities.HistoryFile) ?? new Collection<Suh>();
            var hist = new Suh
                {
                    HelpUrl = appInfo.AppInfo.HelpUrl, 
                    Publisher = appInfo.AppInfo.Publisher, 
                    AppUrl = appInfo.AppInfo.AppUrl, 
                    Description = updateInfo.Description, 
                    Status = failed == false ? UpdateStatus.Successful : UpdateStatus.Failed, 
                    InfoUrl = updateInfo.InfoUrl, 
                    InstallDate = DateTime.Now.ToShortDateString(), 
                    ReleaseDate = updateInfo.ReleaseDate, 
                    Importance = updateInfo.Importance, 
                    Name = updateInfo.Name
                };

            history.Add(hist);

            Utilities.Serialize(history, new Uri(Utilities.HistoryFile));
        }

        /// <summary>Reports the installation progress</summary>
        /// <param name="installProgress">The current install progress percentage</param>
        private static void ReportProgress(int installProgress)
        {
            if (InstallProgressChanged != null)
            {
                InstallProgressChanged(null, new InstallProgressChangedEventArgs(currentUpdateName, installProgress, updateIndex, updateCount));
            }
        }

        /// <summary>Sets the registry items of an update</summary>
        /// <param name="regItems">The registry changes to install on the system</param>
        /// <param name="is64Bit">Indicates if the application is 64 bit</param>
        private static void SetRegistryItems(IList<RegistryItem> regItems, bool is64Bit)
        {
            RegistryKey key;
            if (regItems == null)
            {
                return;
            }

            for (var x = 0; x < regItems.Count; x++)
            {
                var keyPath = Utilities.GetRegistryKey(regItems[x].Key, is64Bit);
                switch (keyPath)
                {
                    case "HKEY_CLASSES_ROOT":
                        key = Registry.ClassesRoot;
                        break;
                    case "HKEY_CURRENT_USER":
                        key = Registry.CurrentUser;
                        break;
                    case "HKEY_LOCAL_MACHINE":
                        key = Registry.LocalMachine;
                        break;
                    case "HKEY_USERS":
                        key = Registry.Users;
                        break;
                    default:
                        key = Registry.CurrentUser;
                        break;
                }

                switch (regItems[x].Action)
                {
                    case RegistryAction.Add:
                        try
                        {
                            Registry.SetValue(keyPath, regItems[x].KeyValue, regItems[x].Data, regItems[x].ValueKind);
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Utilities.ReportError(e, Utilities.AllUserStore);
                            errorOccurred = true;
                        }

                        break;
                    case RegistryAction.DeleteKey:
                        try
                        {
                            if (key != null)
                            {
                                // ReSharper disable PossibleNullReferenceException
                                key.OpenSubKey(regItems[x].Key, true).DeleteSubKeyTree(regItems[x].Key);
                            }

                            // ReSharper restore PossibleNullReferenceException
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Utilities.ReportError(e, Utilities.AllUserStore);
                        }

                        break;
                    case RegistryAction.DeleteValue:
                        try
                        {
                            // ReSharper disable PossibleNullReferenceException
                            key.OpenSubKey(regItems[x].Key, true).DeleteValue(regItems[x].KeyValue, false);

                            // ReSharper restore PossibleNullReferenceException
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Utilities.ReportError(e, Utilities.AllUserStore);
                        }

                        break;
                }

                var installProgress = (x * 100) / regItems.Count;
                if (installProgress > 30)
                {
                    installProgress -= 10;
                }

                ReportProgress(installProgress);
            }
        }

        /// <summary>Installs the shortcuts of an update</summary>
        /// <param name="shortcuts">the shortcuts to install on the system</param>
        /// <param name="appInfo">the application information</param>
        private static void SetShortcuts(IList<Shortcut> shortcuts, Sua appInfo)
        {
            if (shortcuts == null)
            {
                return;
            }

            // Choose the path for the shortcut
            for (var x = 0; x < shortcuts.Count; x++)
            {
                shortcuts[x].Location = Utilities.ConvertPath(shortcuts[x].Location, appInfo.Directory, appInfo.ValueName, appInfo.Is64Bit);
                var linkName = Utilities.GetLocaleString(shortcuts[x].Name);

                if (!shortcuts[x].Location.EndsWith(@"\", StringComparison.CurrentCulture))
                {
                    shortcuts[x].Location = shortcuts[x].Location + @"\";
                }

                if (shortcuts[x].Action == ShortcutAction.Add ||
                    (shortcuts[x].Action == ShortcutAction.Update && File.Exists(shortcuts[x].Location + linkName + @".lnk")))
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    if (!Directory.Exists(shortcuts[x].Location))
                    {
                        Directory.CreateDirectory(shortcuts[x].Location);
                    }

                    File.Delete(shortcuts[x].Location + linkName + @".lnk");

                    //// ReSharper restore AssignNullToNotNullAttribute

                    ////var shortcut = (IWshShortcut)ws.CreateShortcut(shortcuts[x].Location + linkName + @".lnk");

                    ////// Where the shortcut should point to
                    ////shortcut.TargetPath = Utilities.ConvertPath(shortcuts[x].Target, appInfo.Directory, appInfo.ValueName, appInfo.Is64Bit);

                    ////// Description for the shortcut
                    ////shortcut.Description = Utilities.GetLocaleString(shortcuts[x].Description);

                    ////// Location for the shortcut's icon
                    ////shortcut.IconLocation = Utilities.ConvertPath(shortcuts[x].Icon, appInfo.Directory, appInfo.ValueName, appInfo.Is64Bit);

                    ////// The arguments to be used for the shortcut
                    ////shortcut.Arguments = shortcuts[x].Arguments;

                    ////// The working directory to be used for the shortcut
                    ////shortcut.WorkingDirectory = appInfo.Directory;

                    ////// Create the shortcut at the given path
                    ////shortcut.Save();
                }

                if (shortcuts[x].Action == ShortcutAction.Delete)
                {
                    File.Delete(shortcuts[x].Location);
                }

                var installProgress = (x * 100) / shortcuts.Count;
                if (installProgress > 90)
                {
                    installProgress -= 15;
                }

                ReportProgress(installProgress);
            }
        }

        /// <summary>Updates the file on the system</summary>
        /// <param name="file">The file to install or update</param>
        private static void UpdateFile(UpdateFile file)
        {
            switch (file.Action)
            {
                case FileAction.ExecuteThenDelete:
                case FileAction.UnregisterThenDelete:
                case FileAction.Delete:
                    if (file.Action == FileAction.ExecuteThenDelete)
                    {
                        if (File.Exists(file.Source.PathAndQuery))
                        {
                            Utilities.StartProcess(file.Source.PathAndQuery, file.Args, true);
                        }
                    }

                    if (file.Action == FileAction.UnregisterThenDelete)
                    {
                        Utilities.StartProcess("regsvr32", "/u /s" + file.Destination);
                    }

                    try
                    {
                        File.Delete(file.Destination.PathAndQuery);
                    }
                    catch (IOException)
                    {
                        NativeMethods.MoveFileExW(file.Destination.PathAndQuery, null, MoveOnReboot);
                    }

                    break;

                case FileAction.Execute:
                    try
                    {
                        Utilities.StartProcess(file.Destination.PathAndQuery, file.Args);
                    }
                    catch (FileNotFoundException e)
                    {
                        Utilities.ReportError(e + file.Source.PathAndQuery, Utilities.AllUserStore);
                        errorOccurred = true;
                    }

                    break;

                case FileAction.Update:
                case FileAction.UpdateIfExist:
                case FileAction.UpdateThenExecute:
                case FileAction.UpdateThenRegister:
                    if (File.Exists(file.Source.PathAndQuery))
                    {
                        try
                        {
                            if (File.Exists(file.Destination.PathAndQuery))
                            {
                                File.Copy(file.Destination.PathAndQuery, file.Destination + @".bak", true);
                                File.Delete(file.Destination.PathAndQuery);
                            }

                            File.Move(file.Source.PathAndQuery, file.Destination.PathAndQuery);

                            if (File.Exists(file.Destination + @".bak"))
                            {
                                File.Delete(file.Destination + @".bak");
                            }
                        }
                        catch (IOException)
                        {
                            if (!File.Exists(Utilities.AllUserStore + @"reboot.lock"))
                            {
                                using (var rebootFile = File.Create(Utilities.AllUserStore + @"reboot.lock"))
                                {
                                    rebootFile.WriteByte(0);
                                }
                            }

                            NativeMethods.MoveFileExW(file.Source.PathAndQuery, file.Destination.PathAndQuery, MoveOnReboot);
                            File.Delete(file.Destination + @".bak");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            if (!File.Exists(Utilities.AllUserStore + @"reboot.lock"))
                            {
                                using (var rebootFile = File.Create(Utilities.AllUserStore + @"reboot.lock"))
                                {
                                    rebootFile.WriteByte(0);
                                }
                            }

                            NativeMethods.MoveFileExW(file.Source.PathAndQuery, file.Destination.PathAndQuery, MoveOnReboot);
                            File.Delete(file.Destination + @".bak");
                        }
                    }
                    else
                    {
                        Utilities.ReportError(@"FileNotFound: " + file.Source, Utilities.AllUserStore);
                        errorOccurred = true;
                    }

                    if (file.Action == FileAction.UpdateThenExecute)
                    {
                        try
                        {
                            Utilities.StartProcess(file.Destination.PathAndQuery, file.Args);
                        }
                        catch (FileNotFoundException e)
                        {
                            Utilities.ReportError(e + file.Source.PathAndQuery, Utilities.AllUserStore);
                            errorOccurred = true;
                        }
                    }

                    if (file.Action == FileAction.UpdateThenRegister)
                    {
                        Utilities.StartProcess(@"regsvr32", @"/s" + file.Destination);
                    }

                    break;
            }
        }

        /// <summary>Installs the files in the update</summary>
        /// <param name="files">the collection of files to update</param>
        /// <param name="downloadDirectory">the path to the download folder where the update files are located</param>
        private static void UpdateFiles(IList<UpdateFile> files, string downloadDirectory)
        {
            for (var x = 0; x < files.Count; x++)
            {
                files[x].Source = new Uri (downloadDirectory + Path.GetFileName(files[x].Destination.PathAndQuery));
                try
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    Directory.CreateDirectory(Path.GetDirectoryName(files[x].Destination.PathAndQuery));

                    // ReSharper restore AssignNullToNotNullAttribute
                }
                catch (IOException e)
                {
                    Utilities.ReportError(e, Utilities.AllUserStore);
                    errorOccurred = true;
                }

                var x1 = x;
                var x2 = x;
                Task.Factory.StartNew(() => UpdateFile(files[x1])).ContinueWith(
                    delegate
                        {
                            var installProgress = (x2 * 100) / files.Count;
                            if (installProgress > 70)
                            {
                                installProgress -= 15;
                            }

                            ReportProgress(installProgress);
                        });
            }
        }

        #endregion
    }
}