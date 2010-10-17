// ***********************************************************************
// <copyright file="WcfService.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using SevenUpdate.Service;

    /// <summary>
    /// Contains methods and events that run a WCF service
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.PerSession)]
    public class WcfService : IElevatedProcessCallback
    {
        #region Fields

        /// <summary>
        /// The service callback context
        /// </summary>
        private static IElevatedProcess context;

        #endregion

        #region Events

        /// <summary>Occurs when the download completed.</summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadDone;

        /// <summary>Occurs when the download progress changed</summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>Occurs when an error has occurred when downloading or installing updates</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>Occurs when the installation completed.</summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallDone;

        /// <summary>Occurs when the installation progress changed</summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;

        /// <summary>Occurs when the <see cref = "SevenUpdate" />.Admin service faults or encounters a serious error</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ServiceError;

        /// <summary>Occurs when one or more hidden updates have been restored</summary>
        public static event EventHandler<EventArgs> SettingsChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether Seven Update is connected to the admin process
        /// </summary>
        internal static bool IsConnected { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Occurs when the process starts
        /// </summary>
        public void ElevatedProcessStarted()
        {
            context = OperationContext.Current.GetCallbackChannel<IElevatedProcess>();
            if (context == null)
            {
                IsConnected = false;
                Core.Instance.IsAdmin = false;
                return;
            }

            IsConnected = true;
            Core.Instance.IsAdmin = true;

            // Signal Seven Update it can do elevated actions now
        }

        /// <summary>
        /// Occurs when the process as exited
        /// </summary>
        public void ElevatedProcessStopped()
        {
            Core.Instance.IsAdmin = false;
            IsConnected = false;
            context = null;

            // OperationContext.Current.GetCallbackChannel<IElevatedProcess>();
            // Signal Seven Update it can do elevated actions now
        }

        /// <summary>Occurs when the download of updates has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            DownloadDone(this, e);
        }

        /// <summary>Occurs when the download progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged(this, e);
        }

        /// <summary>Occurs when a error occurs when downloading or installing updates</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            ErrorOccurred(this, e);
        }

        /// <summary>Occurs when the installation of updates has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnInstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            InstallDone(this, e);
        }

        /// <summary>Occurs when the install progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            InstallProgressChanged(this, e);
        }

        #endregion

        #region Methods

        /// <summary>Aborts the installation of updates</summary>
        /// <returns><see langword = "true" /> if the install was aborted, otherwise <see langword = "false" /></returns>
        internal static bool AbortInstall()
        {
            var abort = false;
            try
            {
                abort = Utilities.StartProcess(Utilities.AppDir + @"SevenUpdate.Admin.exe", "Abort", true);
            }
            catch (Exception e)
            {
                Utilities.ReportError(e, Utilities.UserStore);
            }

            return abort;
        }

        /// <summary>Adds an application to Seven Update</summary>
        /// <param name="application">the application to add to Seven Update</param>
        internal static void AddSua(Sua application)
        {
            if (!Connect())
            {
                return;
            }

            if (!IsConnected || context == null)
            {
                Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                    delegate
                    {
                        try
                        {
                            context.AddApp(application);
                        }
                        catch (CommunicationObjectAbortedException)
                        {
                            context = null;
                        }
                    });
            }
            else
            {
                try
                {
                    context.AddApp(application);
                }
                catch (CommunicationObjectAbortedException)
                {
                    context = null;
                }
            }
        }

        /// <summary>Reports an error with the admin process</summary>
        /// <param name="e">The exception data that caused the error</param>
        internal static void AdminError(Exception e)
        {
            Core.Instance.IsAdmin = false;
            Utilities.ReportError(e, Utilities.UserStore);
            if (ServiceError != null)
            {
                ServiceError(null, new ErrorOccurredEventArgs(e.Message, ErrorType.FatalError));
            }

            var processes = Process.GetProcessesByName("SevenUpdate.Admin");
            foreach (var t in processes)
            {
                try
                {
                    t.Kill();
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (AccessViolationException)
                {
                }
            }
        }

        /// <summary>Connects to the <see cref="SevenUpdate"/>.Admin sub program</summary>
        /// <returns><see langword="true"/> if the connection to <see cref="WcfService"/> was successful</returns>
        internal static bool Connect()
        {
            if (MyServiceHost.Instance == null)
            {
                MyServiceHost.StartService();
            }

            #if (DEBUG == FALSE)
            if (Process.GetProcessesByName("SevenUpdate.Admin").Length < 1 && Process.GetProcessesByName("SevenUpdate.Admin.vshost").Length < 1)
            {
                var success = Utilities.StartProcess(Utilities.AppDir + @"SevenUpdate.Admin.exe");
                if (!success)
                {
                    IsConnected = false;
                    Core.Instance.IsAdmin = false;
                    return false;
                }
            }
            #endif

            return true;
        }

        /// <summary>Disconnects from <see cref="SevenUpdate"/>.Admin</summary>
        internal static void Disconnect()
        {
            Core.Instance.IsAdmin = false;
            IsConnected = false;
            if (context != null)
                context.Shutdown();
            MyServiceHost.StopService();
        }

        /// <summary>Hides an update</summary>
        /// <param name="hiddenUpdate">the update to hide</param>
        /// <returns><see langword="true"/> if the admin process was executed</returns>
        internal static bool HideUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
            {
                return false;
            }

            if (!IsConnected || context == null)
            {
                Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                    delegate
                        {
                            try
                            {
                                context.HideUpdate(hiddenUpdate);
                            }
                            catch (CommunicationObjectAbortedException)
                            {
                                context = null;
                            }
                        });
            }
            else
            {
                try
                {
                    context.HideUpdate(hiddenUpdate);
                }
                catch (CommunicationObjectAbortedException)
                {
                    context = null;
                }
            }

            return context != null;
        }

        /// <summary>Hides multiple updates</summary>
        /// <param name="hiddenUpdates">the list of updates to hide</param>
        /// <returns><see langword = "true" /> if the admin process was executed, otherwise <see langword = "false" /></returns>
        internal static bool HideUpdates(Collection<Suh> hiddenUpdates)
        {
            if (!Connect())
            {
                return false;
            }

            if (!IsConnected || context == null)
            {
                Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                    delegate
                    {
                        try
                        {
                            context.HideUpdates(hiddenUpdates);
                        }
                        catch (CommunicationObjectAbortedException)
                        {
                            context = null;
                        }
                    });
            }
            else
            {
                try
                {
                    context.HideUpdates(hiddenUpdates);
                }
                catch (CommunicationObjectAbortedException)
                {
                    context = null;
                }
            }

            return context != null;
        }

        /// <summary>Installs selected updates</summary>
        /// <returns><see langword = "true" /> if the admin process was executed, otherwise <see langword = "false" /></returns>
        internal static bool Install()
        {
            if (!Connect())
            {
                return false;
            }

            if (!IsConnected || context == null)
            {
                Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                    delegate
                    {
                        try
                        {
                            context.InstallUpdates(Core.Applications);
                        }
                        catch (CommunicationObjectAbortedException)
                        {
                            context = null;
                        }
                    });
            }
            else
            {
                try
                {
                    context.InstallUpdates(Core.Applications);
                }
                catch (CommunicationObjectAbortedException)
                {
                    context = null;
                }
            }

            return context != null;
        }

        /// <summary>Save the settings and call <see cref="SevenUpdate"/>.Admin to commit them.</summary>
        /// <param name="autoOn"><see langword = "true" /> if auto updates are enabled, otherwise <see langword = "false" /></param>
        /// <param name="options">the options to save</param>
        /// <param name="sul">the list of application to update to save</param>
        internal static void SaveSettings(bool autoOn, Config options, Collection<Sua> sul)
        {
            if (!Connect())
            {
                return;
            }

            if (!IsConnected || context == null)
            {
                Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                    delegate
                    {
                        try
                        {
                            context.ChangeSettings(sul, options, autoOn);
                        }
                        catch (CommunicationObjectAbortedException)
                        {
                            context = null;
                        }
                    });
            }
            else
            {
                try
                {
                    context.ChangeSettings(sul, options, autoOn);
                }
                catch (CommunicationObjectAbortedException)
                {
                    context = null;
                }
            }

            if (SettingsChanged != null)
            {
                SettingsChanged(null, new EventArgs());
            }
        }

        /// <summary>Removes an update from the hidden list</summary>
        /// <param name="hiddenUpdate">the hidden update to show</param>
        /// <returns><see langword = "true" /> if the admin process was executed, otherwise <see langword = "false" /></returns>
        internal static bool ShowUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
            {
                return false;
            }

            if (!IsConnected || context == null)
            {
                Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                    delegate
                    {
                        try
                        {
                            context.ShowUpdate(hiddenUpdate);
                        }
                        catch (CommunicationObjectAbortedException)
                        {
                            context = null;
                        }
                    });
            }

            return context != null;
        }

        /// <summary>
        /// Waits for the admin process to connect
        /// </summary>
        private static void WaitForAdmin()
        {
            while (!IsConnected)
            {
            }
        }

        #endregion
    }
}