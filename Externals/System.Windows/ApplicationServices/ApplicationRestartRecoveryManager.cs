﻿// ***********************************************************************
// <copyright file="ApplicationRestartRecoveryManager.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    using System.Runtime.InteropServices;
    using System.Windows.Internal;

    /// <summary>Provides access to the Application Restart and Recoveryfeatures available in Windows Vista or higher. Application Restart and Recovery lets anapplication do some recovery work to save data before the process exits.</summary>
    public static class ApplicationRestartRecoveryManager
    {
        #region Public Methods

        /// <summary>Called by an application's <see cref="RecoveryCallback" /> method to indicate that the recovery work is complete.</summary>
        /// <remarks>This shouldbe the last call made by the <see cref="RecoveryCallback" /> method becauseWindows Error Reporting will terminate the applicationafter this method is invoked.</remarks>
        /// <param name="success"><see langword="true" /> to indicate the the program was able to complete its recoverywork before terminating; otherwise <see langword="false" />.</param>
        public static void ApplicationRecoveryFinished(bool success)
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new PlatformNotSupportedException("Only supported on Windows Vista or newer.");
            }

            AppRestartRecoveryNativeMethods.ApplicationRecoveryFinished(success);
        }

        /// <summary>Called by an application's <see cref="RecoveryCallback" /> method to indicate that it is still performing recovery work.</summary>
        /// <returns>A <see cref="System.Boolean" /> value indicating whether the usercanceled the recovery.</returns>
        public static bool ApplicationRecoveryInProgress()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new PlatformNotSupportedException("Only supported on Windows Vista or newer.");
            }

            bool canceled;

            var hr = AppRestartRecoveryNativeMethods.ApplicationRecoveryInProgress(out canceled);

            if (hr == Result.Fail)
            {
                throw new InvalidOperationException("This method must be called from the registered callback method.");
            }

            return canceled;
        }

        /// <summary>Registers an application for recovery by Application Restart and Recovery.</summary>
        /// <param name="settings">An object that specifiesthe callback method, an optional parameter to pass to the callbackmethod and a time interval.</param>
        /// <remarks>The time interval is the period of time within which the recovery callback method calls the <see cref="ApplicationRecoveryInProgress" /> method to indicatethat it is still performing recovery work.</remarks>
        public static void RegisterForApplicationRecovery(RecoverySettings settings)
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new PlatformNotSupportedException("Only supported on Windows Vista or newer.");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var handle = GCHandle.Alloc(settings.RecoveryData);

            var hr = AppRestartRecoveryNativeMethods.RegisterApplicationRecoveryCallback(
                AppRestartRecoveryNativeMethods.InternalCallback, (IntPtr)handle, settings.PingInterval, 0);

            if (!ErrorHelper.Succeeded((int)hr))
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Registers an application for automatic restart if the application is terminated by Windows Error Reporting.</summary>
        /// <param name="settings">An object that specifies the command line arguments used to restart the application, and the conditions under which the application should not be restarted.</param>
        /// <remarks>A registered application will not be restarted if it executed for less than 60 seconds before terminating.</remarks>
        public static void RegisterForApplicationRestart(RestartSettings settings)
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new PlatformNotSupportedException("Only supported on Windows Vista or newer.");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var hr = AppRestartRecoveryNativeMethods.RegisterApplicationRestart(settings.Command, settings.Restrictions);

            if (hr == Result.Fail)
            {
                throw new InvalidOperationException("Application failed to registered for restart.");
            }

            if (hr == Result.InvalidArg)
            {
                throw new ArgumentException("Failed to register application for restart due to bad parameters.");
            }
        }

        /// <summary>Removes an application's recovery registration.</summary>
        public static void UnregisterApplicationRecovery()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new PlatformNotSupportedException("Only supported on Windows Vista or newer.");
            }

            var hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRecoveryCallback();

            if (hr == Result.Fail)
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Removes an application's restart registration.</summary>
        public static void UnregisterApplicationRestart()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new PlatformNotSupportedException("Only supported on Windows Vista or newer.");
            }

            var hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRestart();

            if (hr == Result.Fail)
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}