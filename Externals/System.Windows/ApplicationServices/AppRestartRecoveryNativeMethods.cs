// ***********************************************************************
// <copyright file="AppRestartRecoveryNativeMethods.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ***********************************************************************
namespace System.Windows.ApplicationServices
{
    using System.Runtime.InteropServices;
    using System.Windows.Internal;

    /// <summary>
    /// Provides native Win32 Methods to assist with Application recovery
    /// </summary>
    internal static class AppRestartRecoveryNativeMethods
    {
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
            bool canceled;
            ApplicationRecoveryInProgress(out canceled);

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
        internal static extern Result ApplicationRecoveryInProgress([Out] [MarshalAs(UnmanagedType.Bool)] out bool canceled);

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
        internal static extern Result GetApplicationRecoveryCallback(
            IntPtr processHandle, out RecoveryCallback recoveryCallback, out object state, out uint pingInterval, out uint flags);

        /// <summary>
        /// </summary>
        /// <param name="callback">
        /// </param>
        /// <param name="parameter">
        /// </param>
        /// <param name="pingInterval">
        /// </param>
        /// <param name="flags">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [PreserveSig]
        internal static extern Result RegisterApplicationRecoveryCallback(InternalRecoveryCallback callback, IntPtr parameter, uint pingInterval, uint flags);

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
        internal static extern Result RegisterApplicationRestart([MarshalAs(UnmanagedType.BStr)] string commandLineArgs, RestartRestrictions flags);

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
        internal static extern Result GetApplicationRestartSettings(IntPtr process, IntPtr commandLine, ref uint size, out RestartRestrictions flags);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result UnregisterApplicationRecoveryCallback();

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result UnregisterApplicationRestart();

        /// <summary>
        /// </summary>
        internal delegate uint InternalRecoveryCallback(IntPtr state);
    }
}