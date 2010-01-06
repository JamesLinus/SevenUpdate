#region GNU Public License v3

// Copyright 2007-2010 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
//  
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//  
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using SevenUpdate.Admin.WCF;
using SevenUpdate.Base;
using File = System.IO.File;

#endregion

namespace SevenUpdate.Admin
{
    /// <summary>
    /// Class containing methods to install updates
    /// </summary>
    internal static class Install
    {
        #region Global Vars

        /// <summary>
        /// Gets an int that indicates to move a file on reboot
        /// </summary>
        private const int MOVEONREBOOT = 5;

        /// <summary>
        /// The localized name of the current update being installed
        /// </summary>
        private static string currentUpdateName;

        /// <summary>
        /// The index position of the current update being installed
        /// </summary>
        private static int updateIndex;

        /// <summary>
        /// The total number of updates being installed
        /// </summary>
        private static int updateCount;

        /// <summary>
        /// Moves or deletes a file on reboot
        /// </summary>
        /// <param name="lpExistingFileName">The source filename</param>
        /// <param name="lpNewFileName">The destination filename</param>
        /// <param name="dwFlags">A int indicating the move operation to perform</param>
        /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
        [DllImport("kernel32.dll")]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

        #endregion

        private static void ReportProgress(int installProgress)
        {
            if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                EventService.InstallProgressChanged(currentUpdateName, installProgress, updateIndex, updateCount);
            if (App.NotifyIcon != null)
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));
        }

        #region Update Installation

        /// <summary>Installs updates</summary>
        internal static void InstallUpdates(Collection<SUI> applications)
        {
            if (applications == null)
            {
                if (EventService.ErrorOccurred != null && App.IsClientConnected)
                    EventService.ErrorOccurred(new Exception("Applications file could not be deserialized"), ErrorType.FatalError);
                Base.Base.ReportError("Applications file could not be deserialized", Base.Base.AllUserStore);
                App.ShutdownApp();
                return;
            }
            if (applications.Count < 1)
            {
                if (EventService.ErrorOccurred != null && App.IsClientConnected)
                    EventService.ErrorOccurred(new Exception("Applications file could not be deserialized"), ErrorType.DownloadError);
                Base.Base.ReportError("Applications file could not be deserialized", Base.Base.AllUserStore);
                App.ShutdownApp();
            }

            #region variables

            updateCount = applications.Sum(t => t.Updates.Count);
            int completedUpdates = 0, failedUpdates = 0;
            var errorOccurred = false;

            #endregion

            ReportProgress(0);

            for (var x = 0; x < applications.Count; x++)
            {
                for (var y = 0; y < applications[x].Updates.Count; y++)
                {
                    if (File.Exists(Base.Base.AllUserStore + "abort.lock"))
                    {
                        File.Delete(Base.Base.AllUserStore + "abort.lock");
                        App.ShutdownApp();
                    }

                    currentUpdateName = Base.Base.GetLocaleString(applications[x].Updates[y].Name);
                    if (applications[x].Directory == Base.Base.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, true))
                    {
                        try
                        {
                            Process.GetProcessesByName("Seven Update.Helper")[0].Kill();
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            Process.GetProcessesByName("SevenUpdate.Helper")[0].Kill();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    ReportProgress(5);

                    #region Registry

                    // ReSharper disable RedundantAssignment
                    errorOccurred = SetRegistryItems(applications[x].Updates[y].RegistryItems);
                    // ReSharper restore RedundantAssignment

                    #endregion

                    ReportProgress(25);

                    #region Files

                    errorOccurred = UpdateFiles(applications[x].Updates[y].Files, Base.Base.AllUserStore + @"downloads\" + currentUpdateName + @"\");

                    #endregion

                    ReportProgress(75);

                    #region Shortcuts

                    SetShortcuts(applications[x].Updates[y].Shortcuts, applications[x].Directory, applications[x].Is64Bit);

                    #endregion

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

                    #region If Seven Update

                    if (applications[x].Directory == Base.Base.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, true) && Base.Base.RebootNeeded)
                    {
                        for (var z = 0; z < applications[x].Updates[y].Files.Count; z++)
                        {
                            switch (applications[x].Updates[y].Files[z].Action)
                            {
                                case FileAction.Delete:
                                case FileAction.UnregisterThenDelete:
                                    try
                                    {
                                        File.Delete(applications[x].Updates[y].Files[z].Destination);
                                    }
                                    catch (Exception)
                                    {
                                        MoveFileEx(applications[x].Updates[y].Files[z].Destination, null, MOVEONREBOOT);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        Base.Base.StartProcess(Base.Base.AppDir + "SevenUpdate.Helper.exe", "\"" + currentUpdateName + "\"");

                        App.ShutdownApp();
                    }

                    #endregion

                    ReportProgress(100);

                    updateIndex++;
                }
            }

            ReportProgress(100);

            if (Base.Base.RebootNeeded)
            {
                MoveFileEx(Base.Base.AllUserStore + "reboot.lock", null, MOVEONREBOOT);

                if (Directory.Exists(Base.Base.AllUserStore + "downloads"))
                    MoveFileEx(Base.Base.AllUserStore + "downloads", null, MOVEONREBOOT);
            }
            else
            {
                // Delete the downloads directory if no errors were found and no reboot is needed
                if (!errorOccurred)
                {
                    if (Directory.Exists(Base.Base.AllUserStore + "downloads"))
                    {
                        try
                        {
                            Directory.Delete(Base.Base.AllUserStore + "downloads", true);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (EventService.InstallCompleted != null)
                EventService.InstallCompleted(completedUpdates, failedUpdates);

            App.ShutdownApp();
        }

        /// <summary>
        /// Sets the registry items of an update
        /// </summary>
        /// <param name="regItems">The registry changes to install on the system</param>
        /// <returns>a bool value indicating if an error occurred</returns>
        private static bool SetRegistryItems(IList<RegistryItem> regItems)
        {
            RegistryKey key;
            var error = false;
            if (regItems == null)
                return false;

            for (int x = 0; x < regItems.Count; x++)
            {
                switch (regItems[x].Hive)
                {
                    case RegistryHive.ClassesRoot:
                        key = Registry.ClassesRoot;
                        break;
                    case RegistryHive.CurrentUser:
                        key = Registry.CurrentUser;
                        break;
                    case RegistryHive.LocalMachine:
                        key = Registry.LocalMachine;
                        break;
                    case RegistryHive.Users:
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
                            Registry.SetValue(regItems[x].Key, regItems[x].KeyValue, regItems[x].Data, regItems[x].ValueKind);
                        }
                        catch (Exception e)
                        {
                            Base.Base.ReportError(e, Base.Base.AllUserStore);
                            EventService.ErrorOccurred(e, ErrorType.InstallationError);
                            error = true;
                        }
                        break;
                    case RegistryAction.DeleteKey:
                        try
                        {
                            key.OpenSubKey(regItems[x].Key, true).DeleteSubKeyTree(regItems[x].Key);
                        }
                        catch (Exception e)
                        {
                            Base.Base.ReportError(e, Base.Base.AllUserStore);
                            EventService.ErrorOccurred(e, ErrorType.InstallationError);
                        }
                        break;
                    case RegistryAction.DeleteValue:
                        try
                        {
                            key.OpenSubKey(regItems[x].Key, true).DeleteValue(regItems[x].KeyValue, false);
                        }
                        catch (Exception e)
                        {
                            Base.Base.ReportError(e, Base.Base.AllUserStore);
                            EventService.ErrorOccurred(e, ErrorType.InstallationError);
                        }
                        break;
                }

                #region Report Progress

                int installProgress = (x * 100) / regItems.Count;
                if (installProgress > 30)
                    installProgress -= 10;

                ReportProgress(installProgress);

                #endregion
            }

            return error;
        }

        /// <summary>
        /// Installs the shortcuts of an update
        /// </summary>
        /// <param name="shortcuts">the shortcuts to install on the system</param>
        /// <param name="appDirectory">the directory where the application is installed</param>
        /// <param name="is64Bit"><c>true</c> if the application is 64 bit, otherwise <c>false</c></param>
        private static void SetShortcuts(IList<Shortcut> shortcuts, string appDirectory, bool is64Bit)
        {
            if (shortcuts == null)
                return;

            var ws = new WshShell();
            // Choose the path for the shortcut
            for (int x = 0; x < shortcuts.Count; x++)
            {
                try
                {
                    var linkLocation = Base.Base.ConvertPath(shortcuts[x].Location, appDirectory, is64Bit);
                    if (shortcuts[x].Action == ShortcutAction.Add || (shortcuts[x].Action == ShortcutAction.Update && File.Exists(linkLocation)))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(linkLocation)))
                            Directory.CreateDirectory(Path.GetDirectoryName(linkLocation));
                        File.Delete(linkLocation);
                        var shortcut = (IWshShortcut) ws.CreateShortcut(linkLocation);
                        // Where the shortcut should point to
                        shortcut.TargetPath = Base.Base.ConvertPath(shortcuts[x].Target, appDirectory, is64Bit);
                        // Description for the shortcut
                        shortcut.Description = Base.Base.GetLocaleString(shortcuts[x].Description);
                        // Location for the shortcut's icon
                        shortcut.IconLocation = Base.Base.ConvertPath(shortcuts[x].Icon, appDirectory, is64Bit);
                        // The arguments to be used for the shortcut
                        shortcut.Arguments = shortcuts[x].Arguments;
                        // The working directory to be used for the shortcut
                        shortcut.WorkingDirectory = appDirectory;
                        // Create the shortcut at the given path
                        shortcut.Save();
                    }

                    if (shortcuts[x].Action == ShortcutAction.Delete)
                    {
                        File.Delete(linkLocation);
                    }
                }
                catch (Exception e)
                {
                    Base.Base.ReportError(e, Base.Base.AllUserStore);
                    EventService.ErrorOccurred(e, ErrorType.InstallationError);
                }

                #region Report Progress

                int installProgress = (x * 100) / shortcuts.Count;
                if (installProgress > 90)
                    installProgress -= 15;

                ReportProgress(installProgress);

                #endregion
            }
        }

        /// <summary>
        /// Installs the files in the update
        /// </summary>
        /// <param name="files">the collection of files to update</param>
        /// <param name="downloadDirectory">the path to the download folder where the update files are located</param>
        /// <returns><c>true</c> if updated all files without errors, otherwise <c>false</c></returns>
        private static bool UpdateFiles(IList<UpdateFile> files, string downloadDirectory)
        {
            var error = false;
            for (var x = 0; x < files.Count; x++)
            {
                var destinationFile = files[x].Destination;
                var sourceFile = downloadDirectory + Path.GetFileName(destinationFile);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                }
                catch (Exception e)
                {
                    Base.Base.ReportError(e, Base.Base.AllUserStore);
                    EventService.ErrorOccurred(e, ErrorType.InstallationError);
                    return false;
                }

                switch (files[x].Action)
                {
                        #region Delete file

                    case FileAction.ExecuteThenDelete:
                    case FileAction.UnregisterThenDelete:
                    case FileAction.Delete:
                        if (files[x].Action == FileAction.ExecuteThenDelete)
                        {
                            if (File.Exists(sourceFile))
                            {
                                Base.Base.StartProcess(sourceFile, files[x].Args, true);
                            }
                        }

                        if (files[x].Action == FileAction.UnregisterThenDelete)
                            Base.Base.StartProcess("regsvr32", "/u /s" + destinationFile);

                        try
                        {
                            File.Delete(destinationFile);
                        }
                        catch (Exception)
                        {
                            MoveFileEx(destinationFile, null, MOVEONREBOOT);
                        }

                        break;

                        #endregion

                    case FileAction.Execute:
                        try
                        {
                            Base.Base.StartProcess(destinationFile, files[x].Args);
                        }
                        catch (Exception e)
                        {
                            Base.Base.ReportError(e + sourceFile, Base.Base.AllUserStore);
                            EventService.ErrorOccurred(e, ErrorType.InstallationError);
                        }
                        break;

                        #region Update file

                    case FileAction.Update:
                    case FileAction.UpdateIfExist:
                    case FileAction.UpdateThenExecute:
                    case FileAction.UpdateThenRegister:
                        if (File.Exists(sourceFile))
                        {
                            try
                            {
                                if (File.Exists(destinationFile))
                                {
                                    File.Copy(destinationFile, destinationFile + ".bak", true);
                                    File.Delete(destinationFile);
                                }
                                File.Move(sourceFile, destinationFile);

                                if (File.Exists(destinationFile + ".bak"))
                                    File.Delete(destinationFile + ".bak");
                            }
                            catch (Exception)
                            {
                                if (!File.Exists(Base.Base.AllUserStore + "reboot.lock"))
                                {
                                    File.Create(Base.Base.AllUserStore + "reboot.lock").WriteByte(0);
                                }

                                MoveFileEx(sourceFile, destinationFile, MOVEONREBOOT);
                                File.Delete(destinationFile + ".bak");
                            }
                        }
                        else
                        {
                            Base.Base.ReportError("FileNotFound: " + sourceFile, Base.Base.AllUserStore);
                            EventService.ErrorOccurred(new Exception("FileNotFound: " + sourceFile), ErrorType.InstallationError);
                            error = true;
                        }

                        if (files[x].Action == FileAction.UpdateThenExecute)
                        {
                            try
                            {
                                Base.Base.StartProcess(destinationFile, files[x].Args);
                            }
                            catch (Exception e)
                            {
                                Base.Base.ReportError(e + sourceFile, Base.Base.AllUserStore);
                                EventService.ErrorOccurred(e, ErrorType.InstallationError);
                            }
                        }

                        if (files[x].Action == FileAction.UpdateThenRegister)
                            Base.Base.StartProcess("regsvr32", "/s" + destinationFile);
                        break;

                        #endregion
                }

                #region Report Progress

                int installProgress = (x * 100) / files.Count;
                if (installProgress > 70)
                    installProgress -= 15;

                ReportProgress(installProgress);

                #endregion
            }
            return error;
        }

        #endregion

        #region Update History

        /// <summary>
        /// Adds an update to the update history
        /// </summary>
        /// <param name="updateInfo">the update information</param>
        /// <param name="failed"><c>true</c> if the update failed, otherwise <c>false</c></param>
        /// <param name="appInfo"> the application information</param>
        private static void AddHistory(SUI appInfo, Update updateInfo, bool failed = false)
        {
            var history = Base.Base.Deserialize<Collection<SUH>>(Base.Base.HistoryFile) ?? new Collection<SUH>();
            var hist = new SUH
                           {
                               HelpUrl = appInfo.HelpUrl,
                               Publisher = appInfo.Publisher,
                               PublisherUrl = appInfo.PublisherUrl,
                               Description = updateInfo.Description,
                               Status = failed == false ? UpdateStatus.Successful : UpdateStatus.Failed,
                               InfoUrl = updateInfo.InfoUrl,
                               InstallDate = DateTime.Now.ToShortDateString(),
                               ReleaseDate = updateInfo.ReleaseDate,
                               Importance = updateInfo.Importance,
                               Name = updateInfo.Name
                           };


            history.Add(hist);

            Base.Base.Serialize(history, Base.Base.HistoryFile);
        }

        #endregion
    }
}