#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using SevenUpdate.WCF;

#endregion

namespace SevenUpdate
{
    /// <summary>
    ///   Contains callback methods for WCF
    /// </summary>
    internal sealed class ServiceCallBack : IServiceCallback
    {
        #region IServiceCallback Members

        /// <summary>
        ///   Occurs when a error occurs when downloading or installing updates
        /// </summary>
        /// <param name = "exception">the exception that occurred</param>
        /// <param name = "type">the type of error that occurred</param>
        public void OnErrorOccurred(string exception, ErrorType type)
        {
            ErrorOccurred(this, new ErrorOccurredEventArgs(exception, type));
        }

        /// <summary>
        ///   Occurs when the installation of updates has completed
        /// </summary>
        /// <param name = "installedUpdates">the number of updates installed</param>
        /// <param name = "failedUpdates">the number of failed updates</param>
        public void OnInstallCompleted(int installedUpdates, int failedUpdates)
        {
            InstallDone(this, new InstallCompletedEventArgs(installedUpdates, failedUpdates));
        }

        /// <summary>
        ///   Occurs when the download of updates has completed
        /// </summary>
        /// <param name = "errorOccurred"><c>true</c> if an error occurred, otherwise <c>false</c></param>
        public void OnDownloadCompleted(bool errorOccurred)
        {
            DownloadDone(this, new DownloadCompletedEventArgs(errorOccurred));
        }

        /// <summary>
        ///   Occurs when the install progress has changed
        /// </summary>
        /// <param name = "updateName">the name of the update being installed</param>
        /// <param name = "progress">the progress percentage completion</param>
        /// <param name = "updatesComplete">the number of updates that have already been installed</param>
        /// <param name = "totalUpdates">the total number of updates being installed</param>
        public void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            InstallProgressChanged(this, new InstallProgressChangedEventArgs(updateName, progress, updatesComplete, totalUpdates));
        }

        /// <summary>
        ///   Occurs when the download progress has changed
        /// </summary>
        /// <param name = "bytesTransferred">the number of bytes downloaded</param>
        /// <param name = "bytesTotal">the total number of bytes to download</param>
        /// <param name = "filesTransferred">The number of files downloaded</param>
        /// <param name = "filesTotal">The total number of files to download</param>
        public void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal)
        {
            DownloadProgressChanged(this, new DownloadProgressChangedEventArgs(bytesTransferred, bytesTotal, filesTransferred, filesTotal));
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when an error has occurred when downloading or installing updates
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        ///   Occurs when the installation completed.
        /// </summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallDone;

        /// <summary>
        ///   Occurs when the installation progress changed
        /// </summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;

        /// <summary>
        ///   Occurs when the download completed.
        /// </summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadDone;

        /// <summary>
        ///   Occurs when the download progress changed
        /// </summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        #endregion
    }

    /// <summary>
    ///   Provides static methods that control SevenUpdate.Admin for operations that require administrator access
    /// </summary>
    internal static class AdminClient
    {
        /// <summary>
        ///   The client of the WCF service
        /// </summary>
        private static ServiceClient wcfClient;

        private static bool WaitForAdmin()
        {
            //if (Process.GetProcessesByName("SevenUpdate.Admin").Length < 1)
            //{
            //    var success = Base.StartProcess(Base.AppDir + "SevenUpdate.Admin.exe");
            //    if (!success)
            //        return false;
            //    Thread.Sleep(1000);
            //    wcfClient = new ServiceClient(new InstanceContext(new ServiceCallBack()));
            //}
            if (wcfClient == null)
                wcfClient = new ServiceClient(new InstanceContext(new ServiceCallBack()));

            while (wcfClient.State != CommunicationState.Opened && wcfClient.State != CommunicationState.Created)
            {
                if (wcfClient.State == CommunicationState.Faulted)
                    break;
                Thread.SpinWait(200);
                continue;
            }

            if (wcfClient.State == CommunicationState.Faulted)
            {
                AdminError(new Exception("Fault"));
                return WaitForAdmin();
            }


            try
            {
                wcfClient.Subscribe();
                Core.Instance.IsAdmin = true;
                return true;
            }
            catch (EndpointNotFoundException)
            {
                Thread.SpinWait(200);
                return WaitForAdmin();
            }
            catch (Exception e)
            {
                AdminError(e);
                return false;
            }
        }

        /// <summary>
        ///   Connects to the SevenUpdate.Admin sub program
        /// </summary>
        internal static bool Connect()
        {
            var task = Task.Factory.StartNew(() => WaitForAdmin());
            task.Wait();
            return task.Result;
        }

        internal static void AdminError(Exception e)
        {
            Core.Instance.IsAdmin = false;
            Base.ReportError(e, Base.UserStore);
            if (ServiceError != null)
                ServiceError(null, new ErrorOccurredEventArgs(e.Message, ErrorType.FatalError));
            var processes = Process.GetProcessesByName("SevenUpdate.Admin");
            foreach (var t in processes)
            {
                try
                {
                    t.Kill();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///   Disconnects from SevenUpdate.Admin
        /// </summary>
        internal static void Disconnect()
        {
            if (wcfClient == null)
            {
            }
            else
            {
                try
                {
                    wcfClient.UnSubscribe();
                }
                catch
                {
                }
            }
        }

        #region Install Methods

        /// <summary>
        ///   Aborts the installation of updates
        /// </summary>
        /// <returns><c>true</c> if the install was aborted, otherwise <c>false</c></returns>
        internal static bool AbortInstall()
        {
            var abort = false;
            try
            {
                abort = Base.StartProcess(Base.AppDir + "SevenUpdate.Admin.exe", "Abort", true);
                if (abort && wcfClient != null)
                {
                    if (wcfClient.State == CommunicationState.Opened)
                        wcfClient.UnSubscribe();
                }
            }
            catch (Exception e)
            {
                Base.ReportError(e, Base.UserStore);
            }
            return abort;
        }

        /// <summary>
        ///   Installs selected updates
        /// </summary>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool Install()
        {
            if (!Connect())
                return false;
            wcfClient.InstallUpdates(Core.Applications);
            return true;
        }

        #endregion

        #region Hide/Show Update Methods

        /// <summary>
        ///   Hides an update
        /// </summary>
        /// <param name = "hiddenUpdate">the update to hide</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise<c>false</c></returns>
        internal static bool HideUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
                return false;
            wcfClient.HideUpdate(hiddenUpdate);
            return true;
        }

        /// <summary>
        ///   Hides multiple updates
        /// </summary>
        /// <param name = "hiddenUpdates">the list of updates to hide</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool HideUpdates(Collection<Suh> hiddenUpdates)
        {
            if (!Connect())
                return false;
            wcfClient.HideUpdates(hiddenUpdates);
            return true;
        }

        /// <summary>
        ///   Unhides an update
        /// </summary>
        /// <param name = "hiddenUpdate">the hidden update to unhide</param>
        /// <returns><c>true</c> if the admin process was executed, otherwise <c>false</c></returns>
        internal static bool ShowUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
                return false;

            wcfClient.ShowUpdate(hiddenUpdate);
            return true;
        }

        #endregion

        #region Config Methods

        /// <summary>
        ///   Adds an application to Seven Update
        /// </summary>
        /// <param name = "app">the application to add to Seven Update</param>
        internal static void AddSua(Sua app)
        {
            if (!Connect())
                return;

            wcfClient.AddApp(app);
        }

        /// <summary>
        ///   Save the settings and call SevenUpdate.Admin to commit them.
        /// </summary>
        /// <param name = "autoOn"><c>true</c> if auto updates are enabled, otherwise <c>false</c></param>
        /// <param name = "options">the options to save</param>
        /// <param name = "sul">the list of application to update to save</param>
        internal static void SaveSettings(bool autoOn, Config options, Collection<Sua> sul)
        {
            if (!Connect())
                return;

            wcfClient.ChangeSettings(sul, options, autoOn);

            if (SettingsChanged != null)
                SettingsChanged(null, new EventArgs());
        }

        #endregion

        #region Event Declarations

        /// <summary>
        ///   Occurs when one or more hidden updates have been restored
        /// </summary>
        public static event EventHandler<EventArgs> SettingsChanged;

        /// <summary>
        ///   Occurs when the SevenUpdate.Admin serice faults or encounters a serious error
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ServiceError;

        #endregion
    }
}