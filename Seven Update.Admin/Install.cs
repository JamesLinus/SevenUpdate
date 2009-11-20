#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using SevenUpdate.WCF;
using File = System.IO.File;

#endregion

namespace SevenUpdate
{
    /// <summary>
    /// Class containing methods to install updates
    /// </summary>
    internal class Install
    {
        #region Global Vars

        /// <summary>
        /// Gets an int that indicates to move a file on reboot
        /// </summary>
        internal const int MOVEONREBOOT = 5;

        /// <summary>
        /// Gets or Sets a value indicating an installation is in progress
        /// </summary>
        private static int installProgress;

        /// <summary>
        /// Moves or deletes a file on reboot
        /// </summary>
        /// <param name="lpExistingFileName">The source filename</param>
        /// <param name="lpNewFileName">The destination filename</param>
        /// <param name="dwFlags">A int indicating the move operation to perform</param>
        /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
        [DllImport("kernel32.dll")]
        internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

        #endregion

        #region Update Installation

        /// <summary>Installs updates</summary>
        internal static void InstallUpdates(Collection<SUI> applications)
        {
            if (applications == null)
            {
                if (EventService.ErrorOccurred != null && App.IsClientConnected)
                    EventService.ErrorOccurred(new Exception("Applications file could not be found"), ErrorType.FatalError);
                Shared.ReportError("Applications file could not be found", Shared.AllUserStore);
                Environment.Exit(0);
            }
            else
            {
                if (applications.Count < 1)
                {
                    if (EventService.ErrorOccurred != null && App.IsClientConnected)
                        EventService.ErrorOccurred(new Exception("Applications file could not be found"), ErrorType.DownloadError);
                    Shared.ReportError("Applications file could not be found", Shared.AllUserStore);
                    Environment.Exit(0);
                }
            }

            #region variables

            int currentUpdate = 0, completedUpdates = 0, failedUpdates = 0;
            var errorOccurred = false;
            string currentUpdateTitle = null;

            #endregion

            int totalUpdates = applications.Sum(t => t.Updates.Count);

            #region Report Progress

            installProgress = GetProgress(5, currentUpdate, totalUpdates);

            if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                EventService.InstallProgressChanged(currentUpdateTitle, -1, 0, totalUpdates);
            if (App.NotifyIcon != null)
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " 0% " + App.RM.GetString("Complete"));

            #endregion

            for (var x = 0; x < applications.Count; x++)
            {
                for (var y = 0; y < applications[x].Updates.Count; y++)
                {
                    if (App.Abort)
                        Environment.Exit(0);

                    currentUpdateTitle = Shared.GetLocaleString(applications[x].Updates[y].Name);

                    #region Report Progress

                    installProgress = GetProgress(5, currentUpdate, totalUpdates);

                    if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                        EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                    if (App.NotifyIcon != null)
                        Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                                   App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                    #endregion

                    #region Registry

                    // ReSharper disable RedundantAssignment
                    errorOccurred = SetRegistryItems(applications[x].Updates[y]);
                    // ReSharper restore RedundantAssignment

                    #endregion

                    #region Report Progress

                    installProgress = GetProgress(15, currentUpdate, totalUpdates);

                    if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                        EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                    if (App.NotifyIcon != null)
                        Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                                   App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                    #endregion

                    #region Files

                    errorOccurred = UpdateFiles(applications[x].Updates[y].Files, Shared.AllUserStore + @"downloads\" + currentUpdateTitle + @"\", applications[x].Directory,
                                                applications[x].Is64Bit, currentUpdateTitle, currentUpdate, totalUpdates);

                    #endregion

                    #region Shortcuts

                    SetShortcuts(applications[x].Updates[y], applications[x].Directory, applications[x].Is64Bit);

                    #endregion

                    #region Report Progress

                    installProgress = GetProgress(installProgress + 10, currentUpdate, totalUpdates);

                    if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                        EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                    if (App.NotifyIcon != null)
                        Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                                   App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                    #endregion

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

                    if (applications[x].Directory == Shared.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, true) && Shared.RebootNeeded)
                    {
                        for (var z = 0; z < applications[x].Updates[y].Files.Count; z++)
                        {
                            if (applications[x].Updates[y].Files[z].Action != FileAction.UnregisterAndDelete && applications[x].Updates[y].Files[z].Action != FileAction.Delete)
                            {
                            }
                            else
                            {
                                try
                                {
                                    File.Delete(Shared.ConvertPath(applications[x].Updates[y].Files[z].Destination, @"%PROGRAMFILES%\Seven Software\Seven Update", true));
                                }
                                catch (Exception)
                                {
                                    MoveFileEx(Shared.ConvertPath(applications[x].Updates[y].Files[z].Destination, @"%PROGRAMFILES%\Seven Software\Seven Update", true), null,
                                               MOVEONREBOOT);
                                }
                            }
                        }

                        var proc = new Process
                                       {
                                           StartInfo =
                                               {
                                                   FileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Seven Update.Helper.exe",
                                                   UseShellExecute = true,
                                                   Arguments = "\"" + currentUpdateTitle + "\"",
                                                   CreateNoWindow = true,
                                                   WindowStyle = ProcessWindowStyle.Hidden
                                               }
                                       };

                        proc.Start();

                        Environment.Exit(0);
                    }

                    #endregion

                    #region Report Progress

                    installProgress = GetProgress(100, currentUpdate, totalUpdates);

                    if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                        EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                    if (App.NotifyIcon != null)
                        Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                                   App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                    #endregion

                    currentUpdate++;
                }
            }

            #region Report Progress

            installProgress = 100;

            if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
            if (App.NotifyIcon != null)
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

            #endregion

            if (Shared.RebootNeeded)
            {
                MoveFileEx(Shared.AllUserStore + "reboot.lock", null, MOVEONREBOOT);

                if (Directory.Exists(Shared.AllUserStore + "downloads"))
                    MoveFileEx(Shared.AllUserStore + "downloads", null, MOVEONREBOOT);
            }
            else
            {
                // Delete the downloads directory if no errors were found and no reboot is needed
                if (!errorOccurred)
                {
                    if (Directory.Exists(Shared.AllUserStore + "downloads"))
                    {
                        try
                        {
                            Directory.Delete(Shared.AllUserStore + "downloads", true);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (EventService.InstallCompleted != null)
                EventService.InstallCompleted(completedUpdates, failedUpdates);

            Environment.Exit(0);
        }

        /// <summary>
        /// The current progress of the installation
        /// </summary>
        /// <param name="currentProgress">the progress of the current update</param>
        /// <param name="currentUpdate">the current index of the update</param>
        /// <param name="totalUpdates">the total number of updates being installed</param>
        /// <returns>returns the progress percentage of the update</returns>
        private static int GetProgress(int currentProgress, int currentUpdate, int totalUpdates)
        {
            try
            {
                var percent = (currentProgress*100)/totalUpdates;

                var updProgress = percent/(currentUpdate + 1);

                return updProgress/totalUpdates;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Sets the registry items of an update
        /// </summary>
        /// <param name="update">the update to use to set the registry items</param>
        private static bool SetRegistryItems(Update update)
        {
            RegistryKey key;
            var error = false;
            if (update.RegistryItems != null)
            {
                foreach (var reg in update.RegistryItems)
                {
                    switch (reg.Hive)
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
                    switch (reg.Action)
                    {
                        case RegistryAction.Add:
                            try
                            {
                                Registry.SetValue(reg.Key, reg.KeyValue, reg.Data, reg.ValueKind);
                            }
                            catch (Exception e)
                            {
                                Shared.ReportError(e.Message, Shared.AllUserStore);
                                EventService.ErrorOccurred(e, ErrorType.InstallationError);
                                error = true;
                            }
                            break;
                        case RegistryAction.DeleteKey:
                            try
                            {
                                key.OpenSubKey(reg.Key, true).DeleteSubKeyTree(reg.Key);
                            }
                            catch (Exception e)
                            {
                                Shared.ReportError(e.Message, Shared.AllUserStore);
                                EventService.ErrorOccurred(e, ErrorType.InstallationError);
                            }
                            break;
                        case RegistryAction.DeleteValue:
                            try
                            {
                                key.OpenSubKey(reg.Key, true).DeleteValue(reg.KeyValue, false);
                            }
                            catch (Exception e)
                            {
                                Shared.ReportError(e.Message, Shared.AllUserStore);
                                EventService.ErrorOccurred(e, ErrorType.InstallationError);
                            }
                            break;
                    }
                }
            }
            return error;
        }

        /// <summary>
        /// Installs the shortcuts of an update
        /// </summary>
        /// <param name="update">the update to use to install shortcuts</param>
        /// <param name="applicationDirectory">the directory where the application is installed</param>
        /// <param name="is64Bit"><c>true</c> if the application is 64 bit, otherwise <c>false</c></param>
        private static void SetShortcuts(Update update, string applicationDirectory, bool is64Bit)
        {
            if (update.Shortcuts == null)
                return;
            var ws = new WshShell();
            // Choose the path for the shortcut
            foreach (var link in update.Shortcuts)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Shared.ConvertPath(link.Location, true, is64Bit)));
                    File.Delete(Shared.ConvertPath(link.Location, true, is64Bit));
                    var shortcut = (IWshShortcut) ws.CreateShortcut(Shared.ConvertPath(link.Location, true, is64Bit));
                    // Where the shortcut should point to
                    shortcut.TargetPath = Shared.ConvertPath(link.Target, applicationDirectory, is64Bit);
                    // Description for the shortcut
                    shortcut.Description = Shared.GetLocaleString(link.Description);
                    // Location for the shortcut's icon
                    shortcut.IconLocation = Shared.ConvertPath(link.Icon, applicationDirectory, is64Bit);
                    // The arguments to be used for the shortcut
                    shortcut.Arguments = link.Arguments;
                    // The working directory to be used for the shortcut
                    shortcut.WorkingDirectory = Shared.ConvertPath(applicationDirectory, true, is64Bit);
                    // Create the shortcut at the given path
                    shortcut.Save();
                }
                catch (Exception e)
                {
                    Shared.ReportError(e.Message, Shared.AllUserStore);
                    EventService.ErrorOccurred(e, ErrorType.InstallationError);
                }
            }
        }

        /// <summary>
        /// Installs the files in the update
        /// </summary>
        /// <param name="files">the collection of files to update</param>
        /// <param name="downloadDirectory">the path to the download folder where the update files are located</param>
        /// <param name="appDirectory">the application directory</param>
        /// <param name="is64Bit">Indicates if an application is 64-bit, <c>true</c> if 64-bit, otherwise <c>false</c></param>
        /// <param name="currentUpdateTitle">the name of the current update </param>
        /// <param name="currentUpdate">the index of the current update in relation to the total number of updates</param>
        /// <param name="totalUpdates">the total number of updates to install</param>
        /// <returns><c>true</c> if updated all files without errors, otherwise <c>false</c></returns>
        private static bool UpdateFiles(IList<UpdateFile> files, string downloadDirectory, string appDirectory, bool is64Bit, string currentUpdateTitle, int currentUpdate,
                                        int totalUpdates)
        {
            var error = false;
            for (var x = 0; x < files.Count; x++)
            {
                var destinationFile = Shared.ConvertPath(files[x].Destination, appDirectory, is64Bit);
                var sourceFile = downloadDirectory + Path.GetFileName(destinationFile);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                }
                catch (Exception e)
                {
                    Shared.ReportError(e.Message, Shared.AllUserStore);
                    EventService.ErrorOccurred(e, ErrorType.InstallationError);
                    return false;
                }

                switch (files[x].Action)
                {
                        #region Delete file

                    case FileAction.ExecuteAndDelete:
                    case FileAction.UnregisterAndDelete:
                    case FileAction.Delete:
                        if (files[x].Action == FileAction.ExecuteAndDelete)
                        {
                            if (File.Exists(sourceFile))
                            {
                                var proc = new Process {StartInfo = {FileName = sourceFile, Arguments = files[x].Args}};
                                proc.Start();
                                proc.WaitForExit();
                            }
                        }

                        if (files[x].Action == FileAction.UnregisterAndDelete)
                            Process.Start("regsvr32", "/u " + destinationFile);

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

                        #region Update file

                    case FileAction.Update:
                    case FileAction.UpdateAndExecute:
                    case FileAction.UpdateAndRegister:
                        if (File.Exists(sourceFile))
                        {
                            try
                            {
                                App.SetFileSecurity(sourceFile);

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
                                if (!File.Exists(Shared.AllUserStore + "reboot.lock"))
                                {
                                    File.Create(Shared.AllUserStore + "reboot.lock").WriteByte(0);

                                    App.SetFileSecurity(Shared.AllUserStore + "reboot.lock");
                                }

                                MoveFileEx(sourceFile, destinationFile, MOVEONREBOOT);
                                File.Delete(destinationFile + ".bak");
                            }
                        }
                        else
                        {
                            Shared.ReportError("FileNotFound: " + sourceFile, Shared.AllUserStore);
                            EventService.ErrorOccurred(new Exception("FileNotFound: " + sourceFile), ErrorType.InstallationError);
                            error = true;
                        }

                        if (files[x].Action == FileAction.UpdateAndExecute)
                        {
                            try
                            {
                                Process.Start(destinationFile, files[x].Args);
                            }
                            catch (Exception e)
                            {
                                Shared.ReportError(e.Message + sourceFile, Shared.AllUserStore);
                                EventService.ErrorOccurred(e, ErrorType.InstallationError);
                            }
                        }

                        if (files[x].Action == FileAction.UpdateAndRegister)
                            Process.Start("regsvc32", "/s " + destinationFile);
                        break;

                        #endregion
                }

                #region Report Progress

                installProgress = (x*100)/files.Count;
                if (installProgress > 90)
                    installProgress -= 15;

                if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                    EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                if (App.NotifyIcon != null)
                    Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                               App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

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
            var history = Shared.Deserialize<Collection<SUH>>(Shared.HistoryFile) ?? new Collection<SUH>();
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

            Shared.Serialize(history, Shared.HistoryFile);
        }

        #endregion
    }
}