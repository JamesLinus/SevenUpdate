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
        ///   The internal callback
        /// </summary>
        internal static InternalRecoveryCallback InternalCallback;

        /// <summary>
        ///   Initializes static members of the <see cref = "AppRestartRecoveryNativeMethods" /> class.
        /// </summary>
        static AppRestartRecoveryNativeMethods()
        {
            InternalCallback = new InternalRecoveryCallback(InternalRecoveryHandler);
        }

        /// <summary>
        /// The application recovery callback
        /// </summary>
        /// <param name="state">
        /// The state of the application
        /// </param>
        /// <returns>
        /// The error result
        /// </returns>
        internal delegate uint InternalRecoveryCallback(IntPtr state);

        /// <summary>
        /// Indicates if the recovery has finished
        /// </summary>
        /// <param name="success">
        /// if set to <see langword="true"/> recovery was finished
        /// </param>
        [DllImport(@"kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished([MarshalAs(UnmanagedType.Bool)] bool success);

        /// <summary>
        /// Indicates if the recovery is still in progress
        /// </summary>
        /// <param name="canceled">
        /// if set to <see langword="true"/> recovery is in progress
        /// </param>
        /// <returns>
        /// The error result
        /// </returns>
        [DllImport(@"kernel32.dll")]
        [PreserveSig]
        internal static extern Result ApplicationRecoveryInProgress([Out] [MarshalAs(UnmanagedType.Bool)] out bool canceled);

        /// <summary>
        /// Gets the application recovery callback.
        /// </summary>
        /// <param name="processHandle">
        /// The process handle.
        /// </param>
        /// <param name="recoveryCallback">
        /// The recovery callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="pingInterval">
        /// The ping interval.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <returns>
        /// The error result
        /// </returns>
        [DllImport(@"kernel32.dll")]
        [PreserveSig]
        internal static extern Result GetApplicationRecoveryCallback(
            IntPtr processHandle, out RecoveryCallback recoveryCallback, out object state, out uint pingInterval, out uint flags);

        /// <summary>
        /// Registers the application recovery callback.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="pingInterval">
        /// The ping interval.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <returns>
        /// The error result
        /// </returns>
        [DllImport(@"kernel32.dll", CharSet = CharSet.Unicode)]
        [PreserveSig]
        internal static extern Result RegisterApplicationRecoveryCallback(InternalRecoveryCallback callback, IntPtr parameter, uint pingInterval, uint flags);

        // Unused.

        /// <summary>
        /// Registers the application restart event
        /// </summary>
        /// <param name="commandLineArgs">
        /// The command line args.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <returns>
        /// The error result
        /// </returns>
        [DllImport(@"kernel32.dll")]
        [PreserveSig]
        internal static extern Result RegisterApplicationRestart([MarshalAs(UnmanagedType.BStr)] string commandLineArgs, RestartRestrictions flags);

        /// <summary>
        /// Gets the application restart settings.
        /// </summary>
        /// <param name="process">
        /// The pointer to the process
        /// </param>
        /// <param name="commandLine">
        /// The pointer to the command line args
        /// </param>
        /// <param name="size">
        /// The size of the settings
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <returns>
        /// The error result
        /// </returns>
        [DllImport(@"kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [PreserveSig]
        internal static extern Result GetApplicationRestartSettings(IntPtr process, IntPtr commandLine, ref uint size, out RestartRestrictions flags);

        /// <summary>
        /// Unregisters the application recovery callback.
        /// </summary>
        /// <returns>
        /// The error result
        /// </returns>
        [DllImport(@"kernel32.dll")]
        [PreserveSig]
        internal static extern Result UnregisterApplicationRecoveryCallback();

        /// <summary>
        /// Unregisters the application restart method
        /// </summary>
        /// <returns>
        /// The error result
        /// </returns>
        [DllImport(@"kernel32.dll")]
        [PreserveSig]
        internal static extern Result UnregisterApplicationRestart();

        /// <summary>
        /// Handles and invokes the internal recovery callback
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// Returns an error free result
        /// </returns>
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