// ***********************************************************************
// <copyright file="WcfService.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// <summary>
//   Contains methods and events that run a WCF service
// .</summary> ***********************************************************************

namespace SevenUpdate
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using SevenUpdate.Properties;
    using SevenUpdate.Service;

    /// <summary>Contains methods and events that run a WCF service.</summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.PerSession)]
    internal class WcfService : IElevatedProcessCallback
    {
        #region Constants and Fields

        /// <summary>The service callback context.</summary>
        private static IElevatedProcess context;

        #endregion

        #region Public Events

        /// <summary>Occurs when the download completed.</summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadDone;

        /// <summary>Occurs when the download progress changed.</summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>Occurs when an error has occurred when downloading or installing updates.</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>Occurs when the installation completed.</summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallDone;

        /// <summary>Occurs when the installation progress changed.</summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;

        /// <summary>Occurs when the <c>SevenUpdate</c>.Admin service faults or encounters a serious error.</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ServiceError;

        /// <summary>Occurs when one or more hidden updates have been restored.</summary>
        public static event EventHandler<EventArgs> SettingsChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether Seven Update is connected to the admin process.</summary>
        private static bool IsConnected { get; set; }

        #endregion

        #region Public Methods

        /// <summary>Occurs when the process starts.</summary>
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

        /// <summary>Occurs when the process as exited.</summary>
        public void ElevatedProcessStopped()
        {
            Core.Instance.IsAdmin = false;
            IsConnected = false;
            context = null;

            // OperationContext.Current.GetCallbackChannel<IElevatedProcess>(); Signal Seven Update it can do elevated
            // actions now
        }

        /// <summary>Occurs when the download of updates has completed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            DownloadDone(this, e);
        }

        /// <summary>Occurs when the download progress has changed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged(this, e);
        }

        /// <summary>Occurs when a error occurs when downloading or installing updates.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            ErrorOccurred(this, e);
            App.LogError(sender, e);
        }

        /// <summary>Occurs when the installation of updates has completed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnInstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            InstallDone(this, e);
        }

        /// <summary>Occurs when the install progress has changed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            InstallProgressChanged(this, e);
        }

        #endregion

        #region Methods

        /// <summary>Aborts the installation of updates.</summary>
        /// <returns><c>True</c> if the install was aborted, otherwise <c>False</c>.</returns>
        internal static bool AbortInstall()
        {
            var abort = Utilities.StartProcess(Path.Combine(Utilities.AppDir, @"SevenUpdate.Admin.exe"), "Abort");

            return abort;
        }

        /// <summary>Adds an application to Seven Update.</summary>
        /// <param name="application">The application to add to Seven Update.</param>
        internal static void AddSua(Sua application)
        {
            if (!Connect())
            {
                return;
            }

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
                        IsConnected = false;
                    }
                    catch (Exception e)
                    {
                        ErrorOccurred(
                            null,
                            new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                        throw;
                    }
                });
        }

        /// <summary>Reports an error with the admin process.</summary>
        /// <param name="e">The exception data that caused the error.</param>
        internal static void AdminError(Exception e)
        {
            Core.Instance.IsAdmin = false;
            Utilities.ReportError(e, ErrorType.FatalError);
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
                catch (Exception ex)
                {
                    if (!(ex is UnauthorizedAccessException || ex is AccessViolationException))
                    {
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(ex), ErrorType.FatalError));
                        throw;
                    }
                }
            }
        }

        /// <summary>Connects to the <c>SevenUpdate</c>.Admin sub program.</summary>
        /// <returns><c>True</c> if the connection to <c>WcfService</c> was successful.</returns>
        internal static bool Connect()
        {
            MyServiceHost.StartService();

#if (!DEBUG)
            if (Process.GetProcessesByName("SevenUpdate.Admin").Length < 1)
            {
                IsConnected = false;
                context = null;
                Core.Instance.IsAdmin = false;
                var success = Utilities.StartProcess(Path.Combine(Utilities.AppDir, @"SevenUpdate.Admin.exe"));
                if (!success)
                {
                    return false;
                }
            }

#else
            if (Process.GetProcessesByName("SevenUpdate.Admin.vshost").Length < 1
                && Process.GetProcessesByName("SevenUpdate.Admin").Length < 1)
            {
                IsConnected = false;
                context = null;
                Core.Instance.IsAdmin = false;
                var success = Utilities.StartProcess(Path.Combine(Utilities.AppDir, @"SevenUpdate.Admin.exe"));
                if (!success)
                {
                    return false;
                }
            }

#endif
            return true;
        }

        /// <summary>Disconnects from <c>SevenUpdate</c>.Admin.</summary>
        internal static void Disconnect()
        {
            Core.Instance.IsAdmin = false;
            IsConnected = false;
            try
            {
                if (context != null)
                {
                    context.Shutdown();
                }
            }
            catch (Exception ex)
            {
                if (
                    !(ex is CommunicationObjectAbortedException || ex is CommunicationObjectFaultedException
                      || ex is ObjectDisposedException || ex is FaultException))
                {
                    ErrorOccurred(
                        null, new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(ex), ErrorType.FatalError));
                    throw;
                }
            }

            MyServiceHost.StopService();
        }

        /// <summary>Hides an update.</summary>
        /// <param name="hiddenUpdate">The update to hide.</param>
        /// <returns><c>True</c> if the admin process was executed.</returns>
        internal static bool HideUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
            {
                return false;
            }

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
                        IsConnected = false;
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (TimeoutException)
                    {
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (Exception e)
                    {
                        ErrorOccurred(
                            null,
                            new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                        throw;
                    }
                });

            return true;
        }

        /// <summary>Hides multiple updates.</summary>
        /// <param name="hiddenUpdates">The list of updates to hide.</param>
        /// <returns><c>True</c> if the admin process was executed, otherwise <c>False</c>.</returns>
        internal static bool HideUpdates(Collection<Suh> hiddenUpdates)
        {
            if (!Connect())
            {
                ErrorOccurred(null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                return false;
            }

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
                        IsConnected = false;
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (Exception e)
                    {
                        ErrorOccurred(
                            null,
                            new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                        throw;
                    }
                });
            return true;
        }

        /// <summary>Installs selected updates.</summary>
        /// <returns><c>True</c> if the admin process was executed, otherwise <c>False</c>.</returns>
        internal static bool Install()
        {
            if (!Connect())
            {
                return false;
            }
            Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                delegate
                {
                    try
                    {
                        context.InstallUpdates(Core.Applications);
                        IsConnected = true;
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                        context = null;
                        IsConnected = false;
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (TimeoutException)
                    {
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                        IsConnected = false;
                    }
                    catch (Exception e)
                    {
                        context = null;
                        IsConnected = false;
                        ErrorOccurred(
                            null,
                            new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                    }
                });

            return IsConnected;
        }

        /// <summary>Save the settings and call <c>SevenUpdate</c>.Admin to commit them.</summary>
        /// <param name="autoOn"><c>True</c> if auto updates are enabled, otherwise <c>False</c>.</param>
        /// <param name="options">The options to save.</param>
        /// <param name="sul">The list of application to update to save.</param>
        /// <returns><c>True</c> if the admin process was executed, otherwise <c>False</c>.</returns>
        internal static bool SaveSettings(bool autoOn, Config options, Collection<Sua> sul)
        {
            if (!Connect())
            {
                return false;
            }

            Task.Factory.StartNew(WaitForAdmin).ContinueWith(
                delegate
                {
                    try
                    {
                        context.ChangeSettings(sul, options, autoOn);
                        if (SettingsChanged != null)
                        {
                            SettingsChanged(null, new EventArgs());
                        }
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                        context = null;
                        IsConnected = false;
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (TimeoutException)
                    {
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (Exception e)
                    {
                        ErrorOccurred(
                            null,
                            new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                        throw;
                    }
                });

            return true;
        }

        /// <summary>Removes an update from the hidden list.</summary>
        /// <param name="hiddenUpdate">The hidden update to show.</param>
        /// <returns><c>True</c> if the admin process was executed, otherwise <c>False</c>.</returns>
        internal static bool ShowUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
            {
                return false;
            }

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
                        IsConnected = false;
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (TimeoutException)
                    {
                        ErrorOccurred(
                            null, new ErrorOccurredEventArgs(Resources.CouldNotConnectService, ErrorType.FatalError));
                    }
                    catch (Exception e)
                    {
                        ErrorOccurred(
                            null,
                            new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                        throw;
                    }
                });

            return true;
        }

        /// <summary>Waits for the admin process to connect.</summary>
        private static void WaitForAdmin()
        {
            var task = Task.Factory.StartNew(
                () =>
                {
                    while (!IsConnected && context == null)
                    {
                    }
                });
            task.Wait(15000);
        }

        #endregion
    }
}
