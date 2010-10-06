// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.ApplicationServices
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// </summary>
    internal static class AppRestartRecoveryNativeMethods
    {
        #region Application Restart and Recovery Definitions

        /// <summary>
        /// </summary>
        internal static InternalRecoveryCallback internalCallback;

        /// <summary>
        /// </summary>
        static AppRestartRecoveryNativeMethods()
        {
            internalCallback = new InternalRecoveryCallback(InternalRecoveryHandler);
        }

        /// <summary>
        /// </summary>
        /// <param name="parameter">
        /// </param>
        /// <returns>
        /// </returns>
        private static uint InternalRecoveryHandler(IntPtr parameter)
        {
            bool cancelled;
            ApplicationRecoveryInProgress(out cancelled);

            var handle = GCHandle.FromIntPtr(parameter);
            var data = handle.Target as RecoveryData;
            data.Invoke();
            handle.Free();

            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="success">
        /// </param>
        [DllImport("kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished([MarshalAs(UnmanagedType.Bool)] bool success);

        /// <summary>
        /// </summary>
        /// <param name="canceled">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT ApplicationRecoveryInProgress([Out] [MarshalAs(UnmanagedType.Bool)] out bool canceled);

        /// <summary>
        /// </summary>
        /// <param name="processHandle">
        /// </param>
        /// <param name="recoveryCallback">
        /// </param>
        /// <param name="state">
        /// </param>
        /// <param name="pingInterval">
        /// </param>
        /// <param name="flags">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT GetApplicationRecoveryCallback(
            IntPtr processHandle, out RecoveryCallback recoveryCallback, out object state, out uint pingInterval, out uint flags);

        /// <summary>
        /// </summary>
        /// <param name="callback">
        /// </param>
        /// <param name="param">
        /// </param>
        /// <param name="pingInterval">
        /// </param>
        /// <param name="flags">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [PreserveSig]
        internal static extern HRESULT RegisterApplicationRecoveryCallback(InternalRecoveryCallback callback, IntPtr param, uint pingInterval, uint flags);

        // Unused.

        /// <summary>
        /// </summary>
        /// <param name="commandLineArgs">
        /// </param>
        /// <param name="flags">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT RegisterApplicationRestart([MarshalAs(UnmanagedType.BStr)] string commandLineArgs, RestartRestrictions flags);

        /// <summary>
        /// </summary>
        /// <param name="process">
        /// </param>
        /// <param name="commandLine">
        /// </param>
        /// <param name="size">
        /// </param>
        /// <param name="flags">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("KERNEL32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [PreserveSig]
        internal static extern HRESULT GetApplicationRestartSettings(IntPtr process, IntPtr commandLine, ref uint size, out RestartRestrictions flags);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT UnregisterApplicationRecoveryCallback();

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT UnregisterApplicationRestart();

        /// <summary>
        /// </summary>
        /// <param name="state">
        /// </param>
        internal delegate UInt32 InternalRecoveryCallback(IntPtr state);

        #endregion
    }
}