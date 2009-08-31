/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using SevenUpdate.WCF;

namespace SevenUpdate
{
    class Install
    {
        #region Global Vars

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
        internal const int MoveOnReboot = 5;

        internal static bool Abort { get; set; }

        static int installProgress;
        #endregion

        #region Update Installation

        /// <summary>
        /// Checks if the file is in use
        /// </summary>
        /// <param name="files">The list of files of an update</param>
        /// <param name="appDir">The application directory</param>
        /// <returns></returns>
        static bool CheckFileInUse(Collection<UpdateFile> files, string appDir, bool Is64Bit)
        {
            string fileDest;

            foreach (UpdateFile file in files)
            {
                fileDest = Shared.ConvertPath(file.Destination, appDir, Is64Bit);

                if (File.Exists(fileDest))
                {
                    try
                    {
                        FileStream fileStream = new FileStream(fileDest, FileMode.Open, FileAccess.ReadWrite);

                        fileStream.Close();
                    }
                    catch (Exception)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Installs updates
        /// </summary>
        internal static void InstallUpdates(Collection<Application> applications)
        {
            #region variables
            int currentUpdate = 0, totalUpdates = 0;
            bool errorOccurred = false;
            string currentUpdateTitle = null;
            #endregion

            for (int x = 0; x < applications.Count; x++)
            {
                totalUpdates += applications[x].Updates.Count;
            }
            #region Report Progress

            installProgress = GetProgress(5, currentUpdate, totalUpdates);

            if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                EventService.InstallProgressChanged(currentUpdateTitle, -1, 0, totalUpdates);
            if (App.NotifyIcon != null)
                DispatcherObjectDelegates.BeginInvoke<string>(System.Windows.Application.Current.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " 0% " + App.RM.GetString("Complete"));

            #endregion

            foreach (Application app in applications)
            {

                try
                {
                    for (int x = 0; x < app.Updates.Count; x++)
                    {

                        if (Abort)
                            Environment.Exit(0);

                        currentUpdateTitle = Shared.GetLocaleString(app.Updates[x].Name);

                        #region Report Progress

                        installProgress = GetProgress(5, currentUpdate, totalUpdates);

                        if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                            EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                        if (App.NotifyIcon != null)
                            DispatcherObjectDelegates.BeginInvoke<string>(System.Windows.Application.Current.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                        #endregion

                        #region Registry

                        SetRegistryItems(app.Updates[x]);

                        #endregion

                        #region Report Progress

                        installProgress = GetProgress(15, currentUpdate, totalUpdates);

                        if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                            EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                        if (App.NotifyIcon != null)
                            DispatcherObjectDelegates.BeginInvoke<string>(System.Windows.Application.Current.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                        #endregion

                        UpdateFiles(app.Updates[x].Files, Shared.appStore + @"downloads\" + currentUpdateTitle + @"\", app.Directory, app.Is64Bit, currentUpdateTitle, currentUpdate, totalUpdates);

                        #region Shortcuts

                        SetShortcuts(app.Updates[x], app.Directory, app.Is64Bit);

                        #endregion

                        #region Report Progress

                        installProgress = GetProgress(installProgress + 10, currentUpdate, totalUpdates);

                        if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                            EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                        if (App.NotifyIcon != null)
                            DispatcherObjectDelegates.BeginInvoke<string>(System.Windows.Application.Current.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                        #endregion

                        AddHistory(app, app.Updates[x]);

                        #region If Seven Update

                        if (app.Directory == Shared.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, true) && Shared.RebootNeeded)
                        {
                            for (int y = 0; y < app.Updates[x].Files.Count; y++)
                            {
                                if (app.Updates[x].Files[y].Action == FileAction.UnregisterAndDelete || app.Updates[x].Files[y].Action == FileAction.Delete)
                                {
                                    try
                                    {
                                        File.Delete(Shared.ConvertPath(app.Updates[x].Files[y].Destination, @"%PROGRAMFILES%\Seven Software\Seven Update", true));

                                    }
                                    catch (Exception)
                                    {
                                        MoveFileEx(Shared.ConvertPath(app.Updates[x].Files[y].Destination, @"%PROGRAMFILES%\Seven Software\Seven Update", true), null, MoveOnReboot);
                                    }
                                }
                            }

                            Process proc = new Process();

                            proc.StartInfo.FileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Seven Update.Helper.exe";

                            proc.StartInfo.UseShellExecute = true;

                            proc.StartInfo.Arguments = "\"" + currentUpdateTitle + "\"";

                            proc.StartInfo.CreateNoWindow = true;

                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                            proc.Start();

                            Environment.Exit(0);
                        }

                        #endregion

                        #region Report Progress

                        installProgress = GetProgress(100, currentUpdate, totalUpdates);

                        if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                            EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                        if (App.NotifyIcon != null)
                            DispatcherObjectDelegates.BeginInvoke<string>(System.Windows.Application.Current.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                        #endregion

                        currentUpdate++;

                        app.Updates.RemoveAt(x);
                    }
                }
                catch (Exception f)
                {
                    EventService.ErrorOccurred(f.Message);
                    Shared.ReportError(f.Message, Shared.appStore);
                }
            }
            #region Report Progress

            installProgress = 100;

            if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
            if (App.NotifyIcon != null)
                DispatcherObjectDelegates.BeginInvoke<string>(System.Windows.Application.Current.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

            #endregion

            if (Shared.RebootNeeded)
            {
                MoveFileEx(Shared.appStore + "reboot.lock", null, MoveOnReboot);

                if (Directory.Exists(Shared.appStore + "downloads"))
                {
                    MoveFileEx(Shared.appStore + "downloads", null, MoveOnReboot);
                }
            }
            else
            {
                // Delete the downloads directory if no errors were found and no reboot is needed
                if (!errorOccurred)
                {
                    if (Directory.Exists(Shared.appStore + "downloads"))
                    {
                        try
                        {
                            Directory.Delete(Shared.appStore + "downloads", true);
                        }
                        catch (Exception) { }
                    }
                }
            }
            EventService.InstallDone(false);

            Environment.Exit(0);
        }

        /// <summary>
        /// The current progress of the installation
        /// </summary>
        /// <param name="curProgress">Current progress of the installation</param>
        /// <param name="outOf">The total number of updates</param>
        /// <param name="currentUpdate">The current index of the update</param>
        /// <returns>Returns the progress percentage of the update</returns>
        static int GetProgress(int currentProgress, int currentUpdate, int totalUpdates)
        {
            try
            {
                int percent = (currentProgress * 100) / totalUpdates;

                int updProgress = percent / currentUpdate;

                return updProgress / totalUpdates;
            }
            catch (Exception) { return -1; }
        }

        /// <summary>
        /// Sets the registry items of an update
        /// </summary>
        /// <param name="update">The update to use to set the registry items</param>
        /// <param name="appDir">The application directory</param>
        static void SetRegistryItems(Update update)
        {
            RegistryKey key;

            if (update.RegistryItems != null)
                foreach (RegistryItem reg in update.RegistryItems)
                {
                    switch (reg.Hive)
                    {
                        case RegistryHive.ClassesRoot:
                            key = Registry.ClassesRoot; break;
                        case RegistryHive.CurrentUser: key = Registry.CurrentUser; break;
                        case RegistryHive.LocalMachine: key = Registry.LocalMachine; break;
                        case RegistryHive.Users: key = Registry.Users; break;
                        default: key = Registry.CurrentUser; break;
                    }
                    try
                    {
                        switch (reg.Action)
                        {
                            case RegistryAction.Add:
                                Registry.SetValue(reg.Key, reg.KeyValue, reg.Data, reg.ValueKind);
                                break;
                            case RegistryAction.DeleteKey:
                                key.OpenSubKey(reg.Key, true).DeleteSubKeyTree(reg.Key);
                                break;
                            case RegistryAction.DeleteValue:
                                key.OpenSubKey(reg.Key, true).DeleteValue(reg.KeyValue, false);
                                break;
                        }
                    }
                    catch (Exception) { }
                }
        }

        /// <summary>
        /// Installs the shortcuts of an update
        /// </summary>
        /// <param name="update">The update to use to install shortcuts</param>
        /// <param name="appDir">The application directory</param>
        static void SetShortcuts(Update update, string applicationDirectory, bool Is64Bit)
        {
            if (update.Shortcuts != null)
            {
                IWshRuntimeLibrary.WshShell ws = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut;
                // Choose the path for the shortcut
                foreach (Shortcut link in update.Shortcuts)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Shared.ConvertPath(link.Location, true, Is64Bit)));
                    File.Delete(Shared.ConvertPath(link.Location, true, Is64Bit));
                    shortcut = (IWshRuntimeLibrary.IWshShortcut)ws.CreateShortcut(Shared.ConvertPath(link.Location, true, Is64Bit));
                    // Where the shortcut should point to
                    shortcut.TargetPath = Shared.ConvertPath(link.Target, applicationDirectory, Is64Bit);
                    // Description for the shortcut
                    shortcut.Description = Shared.GetLocaleString(link.Description);
                    // Location for the shortcut's icon
                    shortcut.IconLocation = Shared.ConvertPath(link.Icon, applicationDirectory, Is64Bit); ;
                    // The arguments to be used for the shortcut
                    shortcut.Arguments = link.Arguments;
                    // The working directory to be used for the shortcut
                    shortcut.WorkingDirectory = Shared.ConvertPath(applicationDirectory, true, Is64Bit);
                    // Create the shortcut at the given path
                    shortcut.Save();
                }
            }

        }

        /// <summary>
        /// Installs the files in the update
        /// </summary>
        /// <param name="files">The collection of files to update</param>
        /// <param name="downloadDirectory">The path to the download folder where the update files are located</param>
        /// <param name="appDirectory">The application directory</param>
        /// <param name="is64Bit">Secifies if the application is 64Bit</param>
        /// <returns></returns>
        static void UpdateFiles(ObservableCollection<UpdateFile> files, string downloadDirectory, string appDirectory, bool is64Bit, string currentUpdateTitle, int currentUpdate, int totalUpdates)
        {

            string destinationFile;
            string sourceFile;
            for (int x = 0; x < files.Count; x++)
            {
                destinationFile = Shared.ConvertPath(files[x].Destination, appDirectory, is64Bit);
                sourceFile = downloadDirectory + Path.GetFileName(destinationFile);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                }
                catch (Exception e) { Shared.ReportError(e.Message, Shared.appStore); EventService.ErrorOccurred(e.Message); }

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
                                Process proc = new Process();
                                proc.StartInfo.FileName = sourceFile;
                                proc.StartInfo.Arguments = files[x].Arguments;
                                proc.Start();
                                proc.WaitForExit();
                            }
                        }

                        if (files[x].Action == FileAction.UnregisterAndDelete)
                        {
                            Process.Start("regsvr32", "/u " + destinationFile);
                        }

                        try
                        {
                            System.IO.File.Delete(destinationFile);
                        }
                        catch (Exception)
                        {
                            MoveFileEx(destinationFile, null, MoveOnReboot);
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
                            catch (Exception e)
                            {
                                if (!File.Exists(Shared.appStore + "reboot.lock"))
                                {
                                    File.Create(Shared.appStore + "reboot.lock").WriteByte(0);

                                    App.SetFileSecurity(Shared.appStore + "reboot.lock");
                                }

                                MoveFileEx(sourceFile, destinationFile, MoveOnReboot);
                                File.Delete(destinationFile + ".bak");
                                EventService.ErrorOccurred(e.Message);
                                Shared.ReportError(e.Message, Shared.appStore);
                            }
                        }
                        else
                        {
                            Shared.ReportError("FileNotFound: " + sourceFile, Shared.appStore);
                            EventService.ErrorOccurred("FileNotFound: " + sourceFile);
                        }

                        if (files[x].Action == FileAction.UpdateAndExecute)
                        {
                            Process.Start(destinationFile, files[x].Arguments);
                        }

                        if (files[x].Action == FileAction.UpdateAndRegister)
                        {
                            Process.Start("regsvc32", "/s " + destinationFile);
                        }
                        break;
                    #endregion
                }

                #region Report Progress

                installProgress = (x * 100) / files.Count;
                if (installProgress > 90)
                    installProgress -= 15;

                if (EventService.InstallProgressChanged != null && App.IsClientConnected)
                    EventService.InstallProgressChanged(currentUpdateTitle, installProgress, currentUpdate, totalUpdates);
                if (App.NotifyIcon != null)
                    DispatcherObjectDelegates.BeginInvoke<string>(System.Windows.Application.Current.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("InstallingUpdates") + " " + installProgress + " " + App.RM.GetString("Complete"));

                #endregion

            }
        }

        #endregion

        #region Update History

        /// <summary>
        /// Adds an update to the update history
        /// </summary>
        /// <param name="app">The application info</param>
        /// <param name="info">The update to add</param>
        /// <param name="error">The error message when trying to install updates</param>
        static void AddHistory(Application appInfo, Update updateInfo, string error)
        {
            var history = Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Update History.xml");

            UpdateInformation hist = new UpdateInformation();

            hist.HelpUrl = appInfo.HelpUrl;

            hist.Publisher = appInfo.Publisher;

            hist.PublisherUrl = appInfo.PublisherUrl;

            if (error == null)
            {
                hist.Status = UpdateStatus.Successful;

                hist.Description = updateInfo.Description;
            }
            else
            {
                hist.Status = UpdateStatus.Failed;
                LocaleString ls = new LocaleString();
                ls.Value = error;
                ls.lang = "en";
                ObservableCollection<LocaleString> desc = new ObservableCollection<LocaleString>();
                desc.Add(ls);
                hist.Description = desc;
            }

            hist.InfoUrl = updateInfo.InfoUrl;

            hist.InstallDate = DateTime.Now.ToShortDateString();

            hist.ReleaseDate = updateInfo.ReleaseDate;

            hist.Importance = updateInfo.Importance;

            hist.Name = updateInfo.Name;

            history.Add(hist);

            Shared.SerializeCollection<UpdateInformation>(history, Shared.appStore + "Update History.xml");

        }

        /// <summary>
        /// Adds an update to the update history
        /// </summary>
        /// <param name="app">The application info</param>
        /// <param name="info">The update to add</param>
        static void AddHistory(Application app, Update info)
        {
            AddHistory(app, info, null);
        }

        #endregion
    }
}
