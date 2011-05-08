// ***********************************************************************
// <copyright file="AppRestartRecoveryNativeMethods.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    using System.Runtime.InteropServices;
    using System.Windows.Internal;

    /// <summary>Provides native Win32 Methods to assist with Application recovery.</summary>
    internal static class AppRestartRecoveryNativeMethods
    {
        /// <summary>The internal callback.</summary>
        internal static readonly InternalRecoveryCallback InternalCallback = InternalRecoveryHandler;

        /// <summary>The application recovery callback.</summary>
        /// <param name="state">The state of the application.</param>
        /// <returns>The error result.</returns>
        internal delegate uint InternalRecoveryCallback(IntPtr state);

        /// <summary>Indicates that the calling application has completed its data recovery.</summary>
        /// <param name="success">If set to <see langword="true" /> the data was successfully recovered; otherwise, <see langword="false" />.</param>
        [DllImport(@"kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished([MarshalAs(UnmanagedType.Bool)] bool success);

        /// <summary>Indicates that the calling application is continuing to recover data.</summary>
        /// <param name="canceled">Indicates whether the user has canceled the recovery process. Set by WER if the user clicks the Cancel button.</param>
        /// <returns>S_OK if function succeeded, otherwise the error result.</returns>
        [DllImport(@"kernel32.dll"), PreserveSig]
        internal static extern Result ApplicationRecoveryInProgress([Out, MarshalAs(UnmanagedType.Bool)] out bool canceled);

        /// <summary>Retrieves a pointer to the callback routine registered for the specified process. The address returned is in the virtual address space of the process.</summary>
        /// <param name="callback">A pointer to the recovery callback function.</param>
        /// <param name="parameter">A pointer to the callback parameter.</param>
        /// <param name="pingInterval">The recovery ping interval, in 100-nanosecond intervals.</param>
        /// <param name="flags">Reserved for future use.</param>
        /// <returns>S_OK if function succeeded, otherwise the error result.</returns>
        [DllImport(@"kernel32.dll", CharSet = CharSet.Unicode), PreserveSig]
        internal static extern Result RegisterApplicationRecoveryCallback(InternalRecoveryCallback callback, IntPtr parameter, uint pingInterval, uint flags);

        /// <summary>Registers the active instance of an application for restart.</summary>
        /// <param name="commandLineArgs">A pointer to a Unicode string that specifies the command-line arguments for the application when it is restarted.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>S_OK if function succeeded, otherwise the error result.</returns>
        [DllImport(@"kernel32.dll"), PreserveSig]
        internal static extern Result RegisterApplicationRestart([MarshalAs(UnmanagedType.BStr)] string commandLineArgs, RestartRestrictions flags);

        /// <summary>Removes the active instance of an application from the recovery list.</summary>
        /// <returns>S_OK if function succeeded, otherwise the error result.</returns>
        [DllImport(@"kernel32.dll"), PreserveSig]
        internal static extern Result UnregisterApplicationRecoveryCallback();

        /// <summary>Removes the active instance of an application from the restart list.</summary>
        /// <returns>S_OK if function succeeded, otherwise the error result.</returns>
        [DllImport(@"kernel32.dll"), PreserveSig]
        internal static extern Result UnregisterApplicationRestart();

        /// <summary>Handles and invokes the internal recovery callback.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Returns an error free result.</returns>
        private static uint InternalRecoveryHandler(IntPtr parameter)
        {
            bool canceled;
            ApplicationRecoveryInProgress(out canceled);

            var handle = GCHandle.FromIntPtr(parameter);
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