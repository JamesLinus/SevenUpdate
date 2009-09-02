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
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

#endregion

namespace SevenUpdate.WCF
{
    /// <summary>
    /// Contains callback methods for WCF
    /// </summary>
    internal class AdminCallBack : IEventSystemCallback
    {
        #region IEventSystemCallback Members

        /// <summary>
        /// An error occured when downloading or installing updates
        /// </summary>
        /// <param name="description">The description of the error</param>
        /// <param name="type">The type of error that occurred</param>
        public void OnErrorOccurred(string description, ErrorType type)
        {
            if (ErrorOccurredEventHandler == null) return;
            ErrorOccurredEventHandler(this, new ErrorOccurredEventArgs(description, type));
        }

        /// <summary>
        /// Occurs when the installation of updates has completed
        /// </summary>
        /// <param name="installedUpdates">The number of updates installed</param>
        /// <param name="failedUpdates">The number of failed updates</param>
        public void OnInstallDone(int installedUpdates, int failedUpdates)
        {
            if (InstallDoneEventHandler != null) InstallDoneEventHandler(this, new InstallDoneEventArgs(installedUpdates, failedUpdates));
        }

        /// <summary>
        /// Occurs when the download of updates has completed
        /// </summary>
        /// <param name="errorOccurred">True is an error occurred, otherwise false</param>
        public void OnDownloadDone(bool errorOccurred)
        {
            if (DownloadDoneEventHandler != null) DownloadDoneEventHandler(this, new DownloadDoneEventArgs(errorOccurred));
        }

        /// <summary>
        /// Occurs when the install progress has changed
        /// </summary>
        /// <param name="updateName">The name of the update being installed</param>
        /// <param name="progress">The progress percentage completion</param>
        /// <param name="updatesComplete">Number of updates that have already been installed</param>
        /// <param name="totalUpdates">The total number of updates being installed</param>
        public void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            if (InstallProgressChangedEventHandler != null) InstallProgressChangedEventHandler(this, new InstallProgressChangedEventArgs(updateName, progress, updatesComplete, totalUpdates));
        }

        /// <summary>
        /// Occurs when the download progress has changed
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes downloaded</param>
        /// <param name="bytesTotal">The total number of bytes to download</param>
        public void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal)
        {
            if (DownloadProgressChangedEventHandler != null) DownloadProgressChangedEventHandler(this, new DownloadProgressChangedEventArgs(bytesTransferred, bytesTotal));
        }

        #endregion

        #region Events

        #region Event Args

        #region Nested type: DownloadDoneEventArgs

        public class DownloadDoneEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="errorOccurred">True is an error occurred, otherwise false</param>
            public DownloadDoneEventArgs(bool errorOccurred)
            {
                ErrorOccurred = errorOccurred;
            }

            /// <summary>
            /// True is an error occurred, otherwise false
            /// </summary>
            internal bool ErrorOccurred { get; set; }
        }

        #endregion

        #region Nested type: DownloadProgressChangedEventArgs

        public class DownloadProgressChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="bytesTransferred">The number of bytes transferred</param>
            /// <param name="bytesTotal">The total number of bytes to download</param>
            public DownloadProgressChangedEventArgs(ulong bytesTransferred, ulong bytesTotal)
            {
                BytesTotal = bytesTotal;
                BytesTransferred = bytesTransferred;
            }

            /// <summary>
            /// The number of bytes transferred
            /// </summary>
            public ulong BytesTransferred { get; set; }

            /// <summary>
            /// The total number of bytes to download
            /// </summary>
            public ulong BytesTotal { get; set; }
        }

        #endregion

        #region Nested type: ErrorOccurredEventArgs

        public class ErrorOccurredEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="description">A description of the error that occurred</param>
            /// <param name="type">The type of error that occurred</param>
            public ErrorOccurredEventArgs(string description, ErrorType type)
            {
                Description = description;
                Type = type;
            }

            /// <summary>
            /// A description of the error that occurred
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// The type of error that occurred
            /// </summary>
            public ErrorType Type { get; set; }
        }

        #endregion

        #region Nested type: InstallDoneEventArgs

        public class InstallDoneEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="updatesInstalled"></param>
            /// <param name="updatesFailed"></param>
            public InstallDoneEventArgs(int updatesInstalled, int updatesFailed)
            {
                UpdatesInstalled = updatesInstalled;
                UpdatesFailed = updatesFailed;
            }

            /// <summary>
            /// The number of updates that have been installed
            /// </summary>
            public int UpdatesInstalled { get; set; }

            /// <summary>
            /// The number of updates that failed.
            /// </summary>
            public int UpdatesFailed { get; set; }
        }

        #endregion

        #region Nested type: InstallProgressChangedEventArgs

        public class InstallProgressChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="updateName">The name of the update currently being installed</param>
            /// <param name="progress">The progress percetage of the installation</param>
            /// <param name="updatesComplete">The number of updates that have been installed so far</param>
            /// <param name="totalUpdates">The total number of updates to install</param>
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
            public int CurrentProgress { get; set; }

            /// <summary>
            /// The total number of updates to install
            /// </summary>
            public int TotalUpdates { get; set; }

            /// <summary>
            /// The number of updates that have been installed so far
            /// </summary>
            public int UpdatesComplete { get; set; }

            /// <summary>
            /// The name of the current update being installed
            /// </summary>
            public string UpdateName { get; set; }
        }

        #endregion

        #endregion

        /// <summary>
        /// Occurs when an error has occurred when downloading or installing updates
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurredEventHandler;

        /// <summary>
        /// Occurs when the installation completed.
        /// </summary>
        public static event EventHandler<InstallDoneEventArgs> InstallDoneEventHandler;

        /// <summary>
        /// Occurs when the installation progress changed
        /// </summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChangedEventHandler;

        /// <summary>
        /// Occurs when the download completed.
        /// </summary>
        public static event EventHandler<DownloadDoneEventArgs> DownloadDoneEventHandler;

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
        /// <returns>Returns true if sucessful</returns>
        internal static void Connect()
        {
            wcf = new EventSystemClient(new InstanceContext(new AdminCallBack()));
            try
            {
                while (wcf.State != CommunicationState.Created) Thread.CurrentThread.Join(500);
                wcf.Subscribe();
            }
            catch (EndpointNotFoundException e)
            {
                Shared.ReportError(e.Message, Shared.UserStore);
                Thread.CurrentThread.Join(500);
                Connect();
            }
            catch (Exception e)
            {
                Shared.ReportError(e.Message, Shared.UserStore);
            }
        }

        /// <summary>
        /// Disconnects from Seven Update.Admin
        /// </summary>
        internal static void Disconnect()
        {
            wcf.Unsubscribe();
        }

        #region Install & Config Methods

        /// <summary>
        /// Launches the Seven Update.Admin Module
        /// </summary>
        /// <param name="arguments">a string of arguments to be passed to admin module</param>
        /// <param name="wait">a boolen indicating if the current thread will wait for the admin process to exit</param>
        /// <returns>Returns a bool specifying if the admin module executed</returns>
        private static bool LaunchAdmin(string arguments, bool wait)
        {
            var proc = new Process {StartInfo = {FileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Seven Update.Admin.exe"}};

            if (!App.IsAdmin) proc.StartInfo.Verb = "runas";

            proc.StartInfo.UseShellExecute = true;

            proc.StartInfo.Arguments = arguments;

            proc.StartInfo.CreateNoWindow = true;

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                proc.Start();
                if (wait) proc.WaitForExit();
                proc.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Shared.ReportError(e.Message, Shared.UserStore);
                proc.Dispose();
                return false;
            }
        }

        /// <summary>
        /// Launches the Seven Update.Admin Module
        /// </summary>
        /// <param name="arguments">a string of arguments to be passed to admin module</param>
        /// <returns>True if successful, false otherwise</returns>
        private static bool LaunchAdmin(string arguments)
        {
            return LaunchAdmin(arguments, false);
        }

        /// <summary>
        /// Aborts the installation of updates
        /// </summary>
        internal static void AbortInstall()
        {
            try
            {
                wcf.Unsubscribe();
            }
            catch (Exception e)
            {
                Shared.ReportError(e.Message, Shared.UserStore);
            }
        }

        /// <summary>
        /// Installs selected updates
        /// </summary>
        /// <returns>returns true if sucessful</returns>
        internal static bool Install()
        {
            return LaunchAdmin("Install");
        }

        /// <summary>
        /// Hides an update
        /// </summary>
        /// <param name="hiddenUpdate">The update to hide</param>
        /// <returns>True if sucessful, otherwise false</returns>
        internal static bool HideUpdate(SUH hiddenUpdate)
        {
            Shared.Serialize(hiddenUpdate, Shared.UserStore + "HnH Update.xml");
            return LaunchAdmin("HideUpdate");
        }

        /// <summary>
        /// Hides multiple updates
        /// </summary>
        /// <param name="hiddenUpdates">The list of updates to hide</param>
        /// <returns>Returns true if sucessful</returns>
        internal static bool HideUpdates(Collection<SUH> hiddenUpdates)
        {
            Shared.Serialize(hiddenUpdates, Shared.UserStore + "Hidden Updates.xml");
            if (LaunchAdmin("HideUpdates")) return true;
            File.Delete(Shared.UserStore + "Hidden Updates.xml");
            return false;
        }

        /// <summary>
        /// UnHides an update
        /// </summary>
        /// <param name="hiddenUpdate">The Hidden Update to unhide</param>
        /// <returns>Retirms true if successful</returns>
        internal static bool ShowUpdate(SUH hiddenUpdate)
        {
            if (!LaunchAdmin("ShowUpdate")) return false;
            Shared.Serialize(hiddenUpdate, Shared.UserStore + "HnH Update.xml");
            return true;
        }

        /// <summary>
        /// Adds an application to Seven Update
        /// </summary>
        /// <param name="sul">The list of applications to update</param>
        /// <returns>True if successful, otherwise false</returns>
        internal static bool AddSUA(Collection<SUA> sul)
        {
            Shared.Serialize(sul, Shared.UserStore + "Apps.sul");
            return LaunchAdmin("sua");
        }

        /// <summary>
        /// Save the settings and call Seven Update.Admin to commit them.
        /// </summary>
        /// <param name="autoOn">True if auto updates is enabled, otherwise false</param>
        /// <param name="options">The options to save</param>
        /// <param name="sul">The list of application to update to save</param>
        /// <returns>True if successful, otherwise false</returns>
        internal static bool SaveSettings(bool autoOn, Config options, Collection<SUA> sul)
        {
            /// Save the application settings and applications to update in the user store
            Shared.SerializeStruct(options, Shared.UserStore + "App.config");
            Shared.Serialize(sul, Shared.UserStore + "Apps.sul");

            /// Launch Seven Update.Admin to save the settings to the AppStore.
            return autoOn ? LaunchAdmin("Options-On", true) : LaunchAdmin("Options-Off", true);
        }

        #endregion
    }
}