// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.ApplicationServices
{
    using System.Runtime.InteropServices;
    using System.Windows.Internal;

    /// <summary>
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
        /// <parameter name="parameter">
        /// </parameter>
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
        /// <parameter name="success">
        /// </parameter>
        [DllImport("kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished([MarshalAs(UnmanagedType.Bool)] bool success);

        /// <summary>
        /// </summary>
        /// <parameter name="canceled">
        /// </parameter>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result ApplicationRecoveryInProgress([Out] [MarshalAs(UnmanagedType.Bool)] out bool canceled);

        /// <summary>
        /// </summary>
        /// <parameter name="processHandle">
        /// </parameter>
        /// <parameter name="recoveryCallback">
        /// </parameter>
        /// <parameter name="state">
        /// </parameter>
        /// <parameter name="pingInterval">
        /// </parameter>
        /// <parameter name="flags">
        /// </parameter>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result GetApplicationRecoveryCallback(
            IntPtr processHandle, out RecoveryCallback recoveryCallback, out object state, out uint pingInterval, out uint flags);

        /// <summary>
        /// </summary>
        /// <parameter name="callback">
        /// </parameter>
        /// <parameter name="parameter">
        /// </parameter>
        /// <parameter name="pingInterval">
        /// </parameter>
        /// <parameter name="flags">
        /// </parameter>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [PreserveSig]
        internal static extern Result RegisterApplicationRecoveryCallback(InternalRecoveryCallback callback, IntPtr parameter, uint pingInterval, uint flags);

        // Unused.

        /// <summary>
        /// </summary>
        /// <parameter name="commandLineArgs">
        /// </parameter>
        /// <parameter name="flags">
        /// </parameter>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern Result RegisterApplicationRestart([MarshalAs(UnmanagedType.BStr)] string commandLineArgs, RestartRestrictions flags);

        /// <summary>
        /// </summary>
        /// <parameter name="process">
        /// </parameter>
        /// <parameter name="commandLine">
        /// </parameter>
        /// <parameter name="size">
        /// </parameter>
        /// <parameter name="flags">
        /// </parameter>
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
        /// <parameter name="state">
        /// </parameter>
        internal delegate UInt32 InternalRecoveryCallback(IntPtr state);
    }
}