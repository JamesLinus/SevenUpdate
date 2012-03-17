// ***********************************************************************
// <copyright file="AppRestartRecoveryNativeMethods.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.ApplicationServices
{
    using System;
    using System.Runtime.InteropServices;

    using SevenSoftware.Windows.Internal;

    /// <summary>Provides native Win32 Methods to assist with Application recovery.</summary>
    internal static class AppRestartRecoveryNativeMethods
    {
        /// <summary>The internal callback.</summary>
        private static InternalRecoveryCallback internalCallback = InternalRecoveryHandler;

        /// <summary>The application recovery callback.</summary>
        /// <param name="state">The state of the application.</param>
        /// <returns>The error result.</returns>
        internal delegate uint InternalRecoveryCallback(IntPtr state);

        /// <summary>Gets the internal recovery callback.</summary>
        internal static InternalRecoveryCallback InternalCallback
        {
            get
            {
                return internalCallback;
            }
        }

        [DllImport("kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished([MarshalAs(UnmanagedType.Bool)] bool success);

        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result ApplicationRecoveryInProgress(
                [Out] [MarshalAs(UnmanagedType.Bool)] out bool canceled);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [PreserveSig]
        internal static extern Result RegisterApplicationRecoveryCallback(InternalRecoveryCallback callback, 
                                                                          IntPtr param, 
                                                                          uint pingInterval, 
                                                                          uint flags);

        // Unused.
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result RegisterApplicationRestart([MarshalAs(UnmanagedType.BStr)] string commandLineArgs, 
                                                                 RestartRestrictions flags);

        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result UnregisterApplicationRecoveryCallback();

        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result UnregisterApplicationRestart();

        /// <summary>Handles and invokes the internal recovery callback.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Returns an error free result.</returns>
        private static uint InternalRecoveryHandler(IntPtr parameter)
        {
            bool cancelled;
            ApplicationRecoveryInProgress(out cancelled);

            GCHandle handle = GCHandle.FromIntPtr(parameter);
            var data = handle.Target as RecoveryData;
            if (data != null)
            {
                data.Invoke();
            }

            handle.Free();

            return 0;
        }
    }
}