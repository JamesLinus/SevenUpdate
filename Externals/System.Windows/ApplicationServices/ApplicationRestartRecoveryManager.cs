// ***********************************************************************
// <copyright file="ApplicationRestartRecoveryManager.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    using System.Runtime.InteropServices;
    using System.Windows.Internal;
    using System.Windows.Properties;

    /// <summary>
    ///   Provides access to the Application Restart and Recovery features available in Windows Vista or higher.
    ///   Application Restart and Recovery lets an application do some recovery work to save data before the process
    ///   exits.
    /// </summary>
    public static class ApplicationRestartRecoveryManager
    {
        #region Public Methods

        /// <summary>
        ///   Called by an application's <see cref = "RecoveryCallback" /> method to indicate that the recovery work is
        ///   complete.
        /// </summary>
        /// <remarks>
        ///   This should be the last call made by the <see cref = "RecoveryCallback" /> method because Windows Error
        ///   Reporting will terminate the application after this method is invoked.
        /// </remarks>
        /// <param name = "success"><b>true</b> to indicate the the program was able to complete its recovery workbefore terminating; otherwise <b>false</b>.</param>
        public static void ApplicationRecoveryFinished(bool success)
        {
            AppRestartRecoveryNativeMethods.ApplicationRecoveryFinished(success);
        }

        /// <summary>
        ///   Called by an application's <see cref = "RecoveryCallback" /> method to indicate that it is still
        ///   performing recovery work.
        /// </summary>
        /// <returns>A <see cref = "System.Boolean" /> value indicating whether the user canceled the recovery.</returns>
        /// <exception cref = "System.Windows.ApplicationServices.ApplicationRecoveryException">This method must be called from a registered callback method.</exception>
        public static bool ApplicationRecoveryInProgress()
        {
            bool canceled;
            var hr = AppRestartRecoveryNativeMethods.ApplicationRecoveryInProgress(out canceled);

            if (!ErrorHelper.Succeeded(hr))
            {
                throw new InvalidOperationException(Resources.ApplicationRecoveryMustBeCalledFromCallback);
            }

            return canceled;
        }

        /// <summary>Registers an application for recovery by Application Restart and Recovery.</summary>
        /// <param name = "settings">An object that specifies the callback method, an optional parameter to pass to the callback method and a time interval.</param>
        /// <exception cref = "System.ArgumentException">The registration failed due to an invalid parameter.</exception>
        /// <exception cref = "System.ComponentModel.Win32Exception">
        ///   The registration failed.</exception>
        /// <remarks>
        ///   The time interval is the period of time within which the recovery callback method calls the <see
        ///    cref = "ApplicationRecoveryInProgress" /> method to indicate that it is still performing recovery work.
        /// </remarks>
        public static void RegisterForApplicationRecovery(RecoverySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var handle = GCHandle.Alloc(settings.RecoveryData);

            var hr =
                AppRestartRecoveryNativeMethods.RegisterApplicationRecoveryCallback(
                    AppRestartRecoveryNativeMethods.InternalCallback, (IntPtr)handle, settings.PingInterval, 0);

            if (!ErrorHelper.Succeeded(hr))
            {
                if (hr == Result.InvalidArguments)
                {
                    throw new ArgumentException(Resources.ApplicationRecoveryBadParameters, "settings");
                }

                throw new ApplicationRecoveryException(Resources.ApplicationRecoveryFailedToRegister);
            }
        }

        /// <summary>
        ///   Registers an application for automatic restart if the application is terminated by Windows Error
        ///   Reporting.
        /// </summary>
        /// <param name = "settings">An object that specifies
        ///   the command line arguments used to restart the application, and the conditions under which the application
        ///   should not be restarted.</param>
        /// <exception cref = "System.ArgumentException">Registration failed due to an invalid parameter.</exception>
        /// <exception cref = "System.InvalidOperationException">The attempt to register failed.</exception>
        /// <remarks>A registered application will not be restarted if it executed for less than 60 seconds before terminating.</remarks>
        public static void RegisterForApplicationRestart(RestartSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var hr = AppRestartRecoveryNativeMethods.RegisterApplicationRestart(settings.Command, settings.Restrictions);

            if (hr == Result.Fail)
            {
                throw new InvalidOperationException(Resources.ApplicationRecoveryFailedToRegisterForRestart);
            }

            if (hr == Result.InvalidArguments)
            {
                throw new ArgumentException(Resources.ApplicationRecoverFailedToRegisterForRestartBadParameters);
            }
        }

        /// <summary>Removes an application's recovery registration.</summary>
        /// <exception cref = "System.Windows.ApplicationServices.ApplicationRecoveryException">
        /// The attempt to unregister for recovery failed.</exception>
        public static void UnregisterApplicationRecovery()
        {
            var hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRecoveryCallback();

            if (!ErrorHelper.Succeeded(hr))
            {
                throw new ApplicationRecoveryException(Resources.ApplicationRecoveryFailedToUnregister);
            }
        }

        /// <summary>Removes an application's restart registration.</summary>
        /// <exception cref = "System.Windows.ApplicationServices.ApplicationRecoveryException">The attempt to unregister
        /// for restart failed.</exception>
        public static void UnregisterApplicationRestart()
        {
            var hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRestart();

            if (!ErrorHelper.Succeeded(hr))
            {
                throw new ApplicationRecoveryException(Resources.ApplicationRecoveryFailedToUnregisterForRestart);
            }
        }

        #endregion
    }
}
