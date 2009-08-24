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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace SevenUpdate.WCF
{
    class Client : ESB.IEventSystemCallback
    {
        /// <summary>
        /// The client of the ESB service
        /// </summary>
        static ESB.EventSystemClient wcf;

        static Client client;
        
        /// <summary>
        /// Connects to the Seven Update.Admin sub program
        /// </summary>
        /// <returns>Returns true if sucessful</returns>
        void Connect()
        {
            wcf = new SevenUpdate.ESB.EventSystemClient(new InstanceContext(this));
            try
            {
                while (wcf.State != CommunicationState.Created)
                { }
                wcf.Subscribe();
            }
            catch (Exception)
            {

            }
        }

        #region IEventSystemCallback Members

        public void OnErrorOccurred(string errorDescription)
        {
            OnEvent<ErrorOccurredEventArgs>(ErrorOccurredEventHandler, new ErrorOccurredEventArgs(errorDescription));
        }

        public void OnInstallDone(bool ErrorOccurred)
        {
            OnEvent<InstallDoneEventArgs>(InstallDoneEventHandler, new InstallDoneEventArgs(ErrorOccurred, Shared.RebootNeeded));
            try
            {
                wcf.Unsubscribe();
            }
            catch (Exception) { }
        }
        
        public void OnDownloadDone(bool ErrorOccurred)
        {
            OnEvent<DownloadDoneEventArgs>(DownloadDoneEventHandler, new DownloadDoneEventArgs(ErrorOccurred));
        }

        public void OnInstallProgressChanged(string updateTitle, int progress, int updatesComplete, int totalUpdates)
        {
            OnEvent<InstallProgressChangedEventArgs>(InstallProgressChangedEventHandler, new InstallProgressChangedEventArgs(updateTitle, progress, updatesComplete, totalUpdates));
        }

        public void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal)
        {
            OnEvent<DownloadProgressChangedEventArgs>(DownloadProgressChangedEventHandler, new DownloadProgressChangedEventArgs(bytesTransferred, bytesTotal));
        }

        static void OnEvent<T>(EventHandler<T> Event, T Args) where T : EventArgs
        {
            if (Event != null)
            {
                foreach (EventHandler<T> singleEvent in Event.GetInvocationList())
                {
                    if (singleEvent.Target != null && singleEvent.Target is ISynchronizeInvoke)
                    {
                        ISynchronizeInvoke target = (ISynchronizeInvoke)singleEvent.Target;
                        if (target.InvokeRequired)
                        {
                            target.BeginInvoke(singleEvent, new object[] { "OnEvent", Args });
                            continue;
                        }
                    }
                    singleEvent("OnEvent", Args);
                }
            }
        }

        #endregion

        #region Events

        #region Event Args

        public class ErrorOccurredEventArgs : EventArgs
        {
            public ErrorOccurredEventArgs(string errorDescription)
            {
                this.ErrorDescription = errorDescription;
            }

            public string ErrorDescription { get; set; }

        }

        public class InstallDoneEventArgs : EventArgs
        {
            public InstallDoneEventArgs(bool ErrorOccurred, bool rebootNeeded)
            {
                this.ErrorOccurred = ErrorOccurred;
                this.RebootNeeded = rebootNeeded;
            }

            /// <summary>
            /// Indicates if error occurred
            /// </summary>
            public bool ErrorOccurred { get; set; }

            /// <summary>
            /// Specifies if a reboot is needed
            /// </summary>
            public bool RebootNeeded { get; set; }
        }

        public class DownloadDoneEventArgs : EventArgs
        {
            public DownloadDoneEventArgs(bool ErrorOccurred)
            {
                this.ErrorOccurred = ErrorOccurred;
            }

            /// <summary>
            /// Indicates if error occurred
            /// </summary>
            internal bool ErrorOccurred { get; set; }
        }

        public class InstallProgressChangedEventArgs : EventArgs
        {
            public InstallProgressChangedEventArgs(string updateTitle, int progress, int updatesComplete, int totalUpdates)
            {
                this.CurrentProgress = progress;
                this.TotalUpdates = totalUpdates;
                this.UpdatesComplete = updatesComplete;
                this.UpdateTitle = updateTitle;
            }

            /// <summary>
            /// The progress percetage of the installation
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
            /// The title of the current update being installed
            /// </summary>
            public string UpdateTitle { get; set; }
        }

        public class DownloadProgressChangedEventArgs : EventArgs
        {
            public DownloadProgressChangedEventArgs(ulong bytesTransferred, ulong bytesTotal)
            {
                this.BytesTotal = bytesTotal;
                this.BytesTransferred = bytesTransferred;
            }

            public ulong BytesTransferred { get; set; }
            public ulong BytesTotal { get; set; }
        }

        #endregion

        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurredEventHandler;

        /// <summary>
        /// Raises an event when the installation completed.
        /// </summary>
        public static event EventHandler<InstallDoneEventArgs> InstallDoneEventHandler;

        /// <summary>
        /// Raises an event when the installation progress changed
        /// </summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChangedEventHandler;

        /// <summary>
        /// Raises an event when the download completed.
        /// </summary>
        public static event EventHandler<DownloadDoneEventArgs> DownloadDoneEventHandler;

        /// <summary>
        /// Raises an event when the download progress changed
        /// </summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChangedEventHandler;

        #endregion

        #region Install Methods

        /// <summary>
        /// Launches the Seven Update.Admin Module
        /// </summary>
        /// <param name="arguments">a string of arguments to be passed to admin module</param>
        /// <param name="wait">a boolen indicating if the current thread will wait for the admin process to exit</param>
        /// <returns>Returns a bool specifying if the admin module executed</returns>
        static bool LaunchAdmin(string arguments, bool wait)
        {
            Process proc = new Process();

            proc.StartInfo.FileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Seven Update.Admin.exe";

            if (!App.IsAdmin())
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Launches the Seven Update.Admin Module
        /// </summary>
        /// <param name="arguments">a string of arguments to be passed to admin module</param>
        /// <returns>Returns a bool specifying if the admin module executed</returns>
        static bool LaunchAdmin(string arguments)
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
            catch (Exception) { }
        }

        /// <summary>
        /// Installs selected updates
        /// </summary>
        /// <returns>returns true if sucessful</returns>
        internal static bool Install()
        {
            Shared.SerializeCollection<Application>(App.Applications, Shared.appStore + "Update List.xml");
            if (LaunchAdmin("Install") == false)
                    return false;
            client = new Client();
            client.Connect();
            return true;
        }

        /// <summary>
        /// Hides an update
        /// </summary>
        /// <param name="hiddenUpdate">The update to hide</param>
        /// <returns>Returns true if sucessful</returns>
        internal static bool HideUpdate(UpdateInformation hiddenUpdate)
        {
            Shared.Serialize<UpdateInformation>(hiddenUpdate, Shared.userStore + "HnH Update.xml");
            if (!Client.LaunchAdmin("HideUpdate"))
                return false;
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Hides multiple updates
        /// </summary>
        /// <param name="hiddenUpdates">The list of updates to hide</param>
        /// <returns>Returns true if sucessful</returns>
        internal static bool HideUpdates(ObservableCollection<UpdateInformation> hiddenUpdates)
        {
            Shared.SerializeCollection<UpdateInformation>(hiddenUpdates, Shared.userStore + "Hidden Updates.xml");
            if (!Client.LaunchAdmin("HideUpdates"))
            {
                System.IO.File.Delete(Shared.userStore + "Hidden Updates.xml");
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// UnHides an update
        /// </summary>
        /// <param name="hiddenUpdate">The Hidden Update to unhide</param>
        /// <returns>Retirms true if successful</returns>
        internal static bool ShowUpdate(UpdateInformation hiddenUpdate)
        {
            if (!Client.LaunchAdmin("ShowUpdate"))
                return false;
            else
            {
                Shared.Serialize<UpdateInformation>(hiddenUpdate, Shared.userStore + "HnH Update.xml");
                return true;
            }
        }

        internal static bool AddSUA(ObservableCollection<SUA> sul)
        {
            Shared.SerializeCollection<SUA>(sul, Shared.userStore + "SUApps.sul");
            return LaunchAdmin("sua");
        }

        internal static bool SaveSettings(bool AutoOn, UpdateOptions options, ObservableCollection<SUA> sul)
        {
            Shared.Serialize<UpdateOptions>(options, Shared.userStore + "Settings.xml");
            Shared.SerializeCollection<SUA>(sul, Shared.userStore + "SUApps.sul");
            if (AutoOn)
                return LaunchAdmin("Options-On", true);
            else
                return LaunchAdmin("Options-Off", true);
        }

        #endregion
    }
}
