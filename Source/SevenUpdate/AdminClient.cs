// ***********************************************************************
// <copyright file="AdminClient.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;

    using SevenUpdate.Service;

    /// <summary>
    /// Provides static methods that control <see cref="SevenUpdate"/>.Admin for operations that require administrator access
    /// </summary>
    internal static class AdminClient
    {
        #region Constants and Fields

        /// <summary>
        ///   The client of the WCF service
        /// </summary>
        private static ServiceClient wcfClient;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the <see cref = "SevenUpdate" />.Admin service faults or encounters a serious error
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ServiceError;

        /// <summary>
        ///   Occurs when one or more hidden updates have been restored
        /// </summary>
        public static event EventHandler<EventArgs> SettingsChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Aborts the installation of updates
        /// </summary>
        /// <returns>
        /// <c>true</c> if the install was aborted, otherwise <c>false</c>
        /// </returns>
        internal static bool AbortInstall()
        {
            var abort = false;
            try
            {
                abort = Utilities.StartProcess(Utilities.AppDir + @"SevenUpdate.Admin.exe", "Abort", true);
                if (abort && wcfClient != null)
                {
                    if (wcfClient.State == CommunicationState.Opened)
                    {
                        wcfClient.Unsubscribe();
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.ReportError(e, Utilities.UserStore);
            }

            return abort;
        }

        /// <summary>
        /// Adds an application to Seven Update
        /// </summary>
        /// <param name="application">
        /// the application to add to Seven Update
        /// </param>
        internal static void AddSua(Sua application)
        {
            if (!Connect())
            {
                return;
            }

            wcfClient.AddApp(application);
        }

        /// <summary>
        /// Reports an error with the admin process
        /// </summary>
        /// <param name="e">
        /// The exception data that caused the error
        /// </param>
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
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Connects to the <see cref="SevenUpdate"/>.Admin sub program
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the connection to <see cref="WcfService"/> was successful
        /// </returns>
        internal static bool Connect()
        {
            var task = Task.Factory.StartNew(() => WaitForAdmin());
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Disconnects from <see cref="SevenUpdate"/>.Admin
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
                    wcfClient.Unsubscribe();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Hides an update
        /// </summary>
        /// <param name="hiddenUpdate">
        /// the update to hide
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the admin process was executed
        /// </returns>
        internal static bool HideUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
            {
                return false;
            }

            wcfClient.HideUpdate(hiddenUpdate);
            return true;
        }

        /// <summary>
        /// Hides multiple updates
        /// </summary>
        /// <param name="hiddenUpdates">
        /// the list of updates to hide
        /// </param>
        /// <returns>
        /// <c>true</c> if the admin process was executed, otherwise <c>false</c>
        /// </returns>
        internal static bool HideUpdates(Collection<Suh> hiddenUpdates)
        {
            if (!Connect())
            {
                return false;
            }

            wcfClient.HideUpdates(hiddenUpdates);
            return true;
        }

        /// <summary>
        /// Installs selected updates
        /// </summary>
        /// <returns>
        /// <c>true</c> if the admin process was executed, otherwise <c>false</c>
        /// </returns>
        internal static bool Install()
        {
            if (!Connect())
            {
                return false;
            }

            wcfClient.InstallUpdates(Core.Applications);
            return true;
        }

        /// <summary>
        /// Save the settings and call <see cref="SevenUpdate"/>.Admin to commit them.
        /// </summary>
        /// <param name="autoOn">
        /// <c>true</c> if auto updates are enabled, otherwise <c>false</c>
        /// </param>
        /// <param name="options">
        /// the options to save
        /// </param>
        /// <param name="sul">
        /// the list of application to update to save
        /// </param>
        internal static void SaveSettings(bool autoOn, Config options, Collection<Sua> sul)
        {
            if (!Connect())
            {
                return;
            }

            wcfClient.ChangeSettings(sul, options, autoOn);

            if (SettingsChanged != null)
            {
                SettingsChanged(null, new EventArgs());
            }
        }

        /// <summary>
        /// Removes an update from the hidden list
        /// </summary>
        /// <param name="hiddenUpdate">
        /// the hidden update to show
        /// </param>
        /// <returns>
        /// <c>true</c> if the admin process was executed, otherwise <c>false</c>
        /// </returns>
        internal static bool ShowUpdate(Suh hiddenUpdate)
        {
            if (!Connect())
            {
                return false;
            }

            wcfClient.ShowUpdate(hiddenUpdate);
            return true;
        }

        /// <summary>
        /// Waits for the admin process
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the admin process was successfully started
        /// </returns>
        private static bool WaitForAdmin()
        {
            if (Process.GetProcessesByName("SevenUpdate.Admin").Length < 1)
            {
                var success = Utilities.StartProcess(Utilities.AppDir + @"SevenUpdate.Admin.exe");
                if (!success)
                {
                    return false;
                }

                Thread.Sleep(1000);
                wcfClient = new ServiceClient(new InstanceContext(new ServiceCallBack()));
            }

            if (wcfClient == null)
            {
                wcfClient = new ServiceClient(new InstanceContext(new ServiceCallBack()));
            }

            while (wcfClient.State != CommunicationState.Opened && wcfClient.State != CommunicationState.Created)
            {
                if (wcfClient.State == CommunicationState.Faulted)
                {
                    break;
                }

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

        #endregion
    }
}