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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using SevenUpdate.Base;
using SevenUpdate.WCF;

#endregion

namespace SevenUpdate
{

    #region Event Args

    /// <summary>
    /// Provides event data for the DownloadCompleted event
    /// </summary>
    public class DownloadCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Contains event data associated with this event
        /// </summary>
        /// <param name="errorOccurred"><c>true</c> is an error occurred, otherwise <c>false</c></param>
        public DownloadCompletedEventArgs(bool errorOccurred)
        {
            ErrorOccurred = errorOccurred;
        }

        /// <summary>
        /// <c>true</c> if an error occurred, otherwise <c>false</c>
        /// </summary>
        internal bool ErrorOccurred { get; private set; }
    }

    /// <summary>
    /// Provides event data for the DownloadProgressChanged event
    /// </summary>
    public class DownloadProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Contains event data associated with this event
        /// </summary>
        /// <param name="bytesTransferred">the number of bytes transferred</param>
        /// <param name="bytesTotal">the total number of bytes to download</param>
        public DownloadProgressChangedEventArgs(ulong bytesTransferred, ulong bytesTotal)
        {
            BytesTotal = bytesTotal;
            BytesTransferred = bytesTransferred;
        }

        /// <summary>
        /// Gets the number of bytes transferred
        /// </summary>
        public ulong BytesTransferred { get; private set; }

        /// <summary>
        /// Gets the total number of bytes to download
        /// </summary>
        public ulong BytesTotal { get; private set; }
    }

    /// <summary>
    /// Provides event data for the InstallCompleted event
    /// </summary>
    public class InstallCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Contains event data associated with this event
        /// </summary>
        /// <param name="updatesInstalled">the number of updates installed</param>
        /// <param name="updatesFailed">the number of updates that failed</param>
        public InstallCompletedEventArgs(int updatesInstalled, int updatesFailed)
        {
            UpdatesInstalled = updatesInstalled;
            UpdatesFailed = updatesFailed;
        }

        /// <summary>
        /// Gets the number of updates that have been installed
        /// </summary>
        public int UpdatesInstalled { get; private set; }

        /// <summary>
        /// Gets the number of updates that failed.
        /// </summary>
        public int UpdatesFailed { get; private set; }
    }

    /// <summary>
    /// Provides event data for the InstallProgressChanged event
    /// </summary>
    public class InstallProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Contains event data associated with this event
        /// </summary>
        /// <param name="updateName">the name of the update currently being installed</param>
        /// <param name="progress">the progress percentage of the installation</param>
        /// <param name="updatesComplete">the number of updates that have been installed so far</param>
        /// <param name="totalUpdates">the total number of updates to install</param>
        public InstallProgressChangedEventArgs(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            CurrentProgress = progress;
            TotalUpdates = totalUpdates;
            UpdatesComplete = updatesComplete;
            UpdateName = updateName;
        }

        /// <summary>
        /// The progress percentage of the installation
        /// </summary>
        public int CurrentProgress { get; private set; }

        /// <summary>
        /// The total number of updates to install
        /// </summary>
        public int TotalUpdates { get; private set; }

        /// <summary>
        /// The number of updates that have been installed so far
        /// </summary>
        public int UpdatesComplete { get; private set; }

        /// <summary>
        /// The name of the current update being installed
        /// </summary>
        public string UpdateName { get; private set; }
    }

    #endregion

    /// <summary>
    /// Contains callback methods for WCF
    /// </summary>
    internal class AdminCallBack : IEventSystemCallback
    {
        #region IEventSystemCallback Members

        /// <summary>
        /// Occurs when a error occurs when downloading or installing updates
        /// </summary>
        /// <param name="exception">the exception that occurred</param>
        /// <param name="type">the type of error that occurred</param>
        public void OnErrorOccurred(Exception exception, ErrorType type)
        {
            if (ErrorOccurredEventHandler == null)
                return;
            ErrorOccurredEventHandler(this, new ErrorOccurredEventArgs(exception, type));
        }

        /// <summary>
        /// Occurs when the installation of updates has completed
        /// </summary>
        /// <param name="installedUpdates">the number of updates installed</param>
        /// <param name="failedUpdates">the number of failed updates</param>
        public void OnInstallCompleted(int installedUpdates, int failedUpdates)
        {
            if (InstallDoneEventHandler != null)
                InstallDoneEventHandler(this, new InstallCompletedEventArgs(installedUpdates, failedUpdates));
        }

        /// <summary>
        /// Occurs when the download of updates has completed
        /// </summary>
        /// <param name="errorOccurred"><c>true</c> if an error occurred, otherwise <c>false</c></param>
        public void OnDownloadCompleted(bool errorOccurred)
        {
            if (DownloadDoneEventHandler != null)
                DownloadDoneEventHandler(this, new DownloadCompletedEventArgs(errorOccurred));
        }

        /// <summary>
        /// Occurs when the install progress has changed
        /// </summary>
        /// <param name="updateName">the name of the update being installed</param>
        /// <param name="progress">the progress percentage completion</param>
        /// <param name="updatesComplete">the number of updates that have already been installed</param>
        /// <param name="totalUpdates">the total number of updates being installed</param>
        public void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            if (InstallProgressChangedEventHandler != null)
                InstallProgressChangedEventHandler(this, new InstallProgressChangedEventArgs(updateName, progress, updatesComplete, totalUpdates));
        }

        /// <summary>
        /// Occurs when the download progress has changed
        /// </summary>
        /// <param name="bytesTransferred">the number of bytes downloaded</param>
        /// <param name="bytesTotal">the total number of bytes to download</param>
        public void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal)
        {
            if (DownloadProgressChangedEventHandler != null)
                DownloadProgressChangedEventHandler(this, new DownloadProgressChangedEventArgs(bytesTransferred, bytesTotal));
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when an error has occurred when downloading or installing updates
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurredEventHandler;

        /// <summary>
        /// Occurs when the installation completed.
        /// </summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallDoneEventHandler;

        /// <summary>
        /// Occurs when the installation progress changed
        /// </summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChangedEventHandler;

        /// <summary>
        /// Occurs when the download completed.
        /// </summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadDoneEventHandler;

        /// <summary>
        /// Occurs when the download progress changed
        /// </summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChangedEventHandler;

        #endregion
    }

    /// <summary>
    /// Provides static methods that control Seven Update.Admin for operations that require administrator access
    /// </summary>
    internal class Admin : AdminCallBack
    {
        /// <summary>
        /// The client of the WCF service
        /// </summary>
        private static EventSystemClient wcf;

        /// <summary>
        /// Connects to the Seven Update.Admin sub program
        /// </summary>
        /// <returns>returns <c>true</c> if successful</returns>
        internal static void Connect()
        {
            wcf = new EventSystemClient(new InstanceContext(new AdminCallBack()));
            try
            {
                while (wcf.State != CommunicationState.Created)
                    Thread.CurrentThread.Join(500);
                wcf.Subscribe();
            }
            catch (EndpointNotFoundException)
            {
                Thread.CurrentThread.Join(500);
                Connect();
            }
            catch (Exception e)
            {
                Base.Base.ReportError(e.Message, Base.Base.UserStore);
            }
        }

        /// <summary>
        /// Disconnects from Seven Update.Admin
        /// </summary>
        internal static void Disconnect()
        {
            try
            {
                wcf.UnSubscribe();
            }
            catch (Exception e)
            {
                Base.Base.ReportError(e.Message, Base.Base.UserStore);
            }
        }

        #region Install & Config Methods

        /// <summary>
        /// Launches the Seven Update.Admin Module
        /// </summary>
        /// <param name="arguments">a string of arguments to be passed to admin module</param>
        /// <param name="wait">a boolean indicating if the current thread will wait for the admin process to exit</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        private static bool LaunchAdmin(string arguments, bool wait = false)
        {
            var proc = new Process {StartInfo = {FileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Seven Update.Admin.exe"}};

            if (!App.IsAdmin)
                proc.StartInfo.Verb = "runas";

            proc.StartInfo.UseShellExecute = true;

            proc.StartInfo.Arguments = arguments;

            proc.StartInfo.CreateNoWindow = true;

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                proc.Start();
                if (wait)
                    proc.WaitForExit();
                proc.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Base.Base.ReportError(e.Message, Base.Base.UserStore);
                proc.Dispose();
                return false;
            }
        }

        /// <summary>
        /// Aborts the installation of updates
        /// </summary>
        internal static bool AbortInstall()
        {
            bool abort = false;
            try
            {
                abort = LaunchAdmin("Abort");
                if (abort)
                    wcf.UnSubscribe();
            }
            catch (Exception e)
            {
                Base.Base.ReportError(e.Message, Base.Base.UserStore);
            }
            return abort;
        }

        /// <summary>
        /// Installs selected updates
        /// </summary>
        /// <returns> <c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool Install()
        {
            Base.Base.Serialize(App.Applications, Base.Base.UserStore + "Updates.sui");
            return LaunchAdmin("Install");
        }

        /// <summary>Hides an update</summary>
        /// <param name="hiddenUpdate">the update to hide</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool HideUpdate(SUH hiddenUpdate)
        {
            Base.Base.Serialize(hiddenUpdate, Base.Base.UserStore + "Update.suh");
            return LaunchAdmin("HideUpdate");
        }

        /// <summary>
        /// Hides multiple updates
        /// </summary>
        /// <param name="hiddenUpdates">the list of updates to hide</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool HideUpdates(Collection<SUH> hiddenUpdates)
        {
            Base.Base.Serialize(hiddenUpdates, Base.Base.UserStore + "Hidden.suh");
            if (LaunchAdmin("HideUpdates", true))
                return true;
            File.Delete(Base.Base.UserStore + "Hidden.suh");
            return false;
        }

        /// <summary>
        /// Unhides an update
        /// </summary>
        /// <param name="hiddenUpdate">the hidden update to unhide</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool ShowUpdate(SUH hiddenUpdate)
        {
            Base.Base.Serialize(hiddenUpdate, Base.Base.UserStore + "Update.suh");
            if (LaunchAdmin("ShowUpdate"))
                return true;
            File.Delete(Base.Base.UserStore + "Update.suh");
            return true;
        }

        /// <summary>
        /// Adds an application to Seven Update
        /// </summary>
        /// <param name="sul">the list of applications to update</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool AddSUA(Collection<SUA> sul)
        {
            Base.Base.Serialize(sul, Base.Base.UserStore + "Apps.sul");
            return LaunchAdmin("sua");
        }

        /// <summary>
        /// Save the settings and call Seven Update.Admin to commit them.
        /// </summary>
        /// <param name="autoOn"><c>true</c> if auto updates are enabled, otherwise <c>false</c></param>
        /// <param name="options">the options to save</param>
        /// <param name="sul">the list of application to update to save</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool SaveSettings(bool autoOn, Config options, Collection<SUA> sul)
        {
            // Save the application settings and applications to update in the user store
            Base.Base.SerializeStruct(options, Base.Base.UserStore + "App.config");
            Base.Base.Serialize(sul, Base.Base.UserStore + "Apps.sul");

            // Launch Seven Update.Admin to save the settings to the AppStore.
            return autoOn ? LaunchAdmin("Options-On", true) : LaunchAdmin("Options-Off", true);
        }

        #endregion
    }
}