#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Runtime.InteropServices;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.ApplicationServices
{
    internal static class AppRestartRecoveryNativeMethods
    {
        #region Application Restart and Recovery Definitions

        internal static InternalRecoveryCallback internalCallback;

        static AppRestartRecoveryNativeMethods()
        {
            internalCallback = new InternalRecoveryCallback(InternalRecoveryHandler);
        }

        private static UInt32 InternalRecoveryHandler(IntPtr parameter)
        {
            bool cancelled;
            ApplicationRecoveryInProgress(out cancelled);

            var handle = GCHandle.FromIntPtr(parameter);
            var data = handle.Target as RecoveryData;
            data.Invoke();
            handle.Free();

            return (0);
        }

        [DllImport("kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished([MarshalAs(UnmanagedType.Bool)] bool success);

        [DllImport("kernel32.dll"), PreserveSig]
        internal static extern HRESULT ApplicationRecoveryInProgress([Out, MarshalAs(UnmanagedType.Bool)] out bool canceled);

        [DllImport("kernel32.dll"), PreserveSig]
        internal static extern HRESULT GetApplicationRecoveryCallback(IntPtr processHandle, out RecoveryCallback recoveryCallback, out object state, out uint pingInterval, out uint flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode), PreserveSig]
        internal static extern HRESULT RegisterApplicationRecoveryCallback(InternalRecoveryCallback callback, IntPtr param, uint pingInterval, uint flags);

        // Unused.

        [DllImport("kernel32.dll"), PreserveSig]
        internal static extern HRESULT RegisterApplicationRestart([MarshalAs(UnmanagedType.BStr)] string commandLineArgs, RestartRestrictions flags);

        [DllImport("KERNEL32.dll", CharSet = CharSet.Unicode, SetLastError = true), PreserveSig]
        internal static extern HRESULT GetApplicationRestartSettings(IntPtr process, IntPtr commandLine, ref uint size, out RestartRestrictions flags);

        [DllImport("kernel32.dll"), PreserveSig]
        internal static extern HRESULT UnregisterApplicationRecoveryCallback();

        [DllImport("kernel32.dll"), PreserveSig]
        internal static extern HRESULT UnregisterApplicationRestart();

        internal delegate UInt32 InternalRecoveryCallback(IntPtr state);

        #endregion
    }
}