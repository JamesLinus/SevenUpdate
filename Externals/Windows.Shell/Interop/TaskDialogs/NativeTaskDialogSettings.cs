//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#endregion

namespace Microsoft.Windows.Dialogs
{
    ///<summary>
    ///  Encapsulates additional configuration needed by NativeTaskDialog
    ///  that it can't get from the TASKDIALOGCONFIG struct.
    ///</summary>
    internal class NativeTaskDialogSettings
    {
        internal NativeTaskDialogSettings()
        {
            NativeConfiguration = new TaskDialogNativeMethods.TASKDIALOGCONFIG();

            // Apply standard settings.
            NativeConfiguration.cbSize = (uint) Marshal.SizeOf(NativeConfiguration);
            NativeConfiguration.hwndParent = IntPtr.Zero;
            NativeConfiguration.hInstance = IntPtr.Zero;
            NativeConfiguration.dwFlags = TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION;
            NativeConfiguration.dwCommonButtons = TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON;
            NativeConfiguration.MainIcon = new TaskDialogNativeMethods.TASKDIALOGCONFIG_ICON_UNION(0);
            NativeConfiguration.FooterIcon = new TaskDialogNativeMethods.TASKDIALOGCONFIG_ICON_UNION(0);
            NativeConfiguration.cxWidth = TaskDialogDefaults.IdealWidth;

            // Zero out all the custom button fields.
            NativeConfiguration.cButtons = 0;
            NativeConfiguration.cRadioButtons = 0;
            NativeConfiguration.pButtons = IntPtr.Zero;
            NativeConfiguration.pRadioButtons = IntPtr.Zero;
            NativeConfiguration.nDefaultButton = 0;
            NativeConfiguration.nDefaultRadioButton = 0;

            // Various text defaults.
            NativeConfiguration.pszWindowTitle = TaskDialogDefaults.Caption;
            NativeConfiguration.pszMainInstruction = TaskDialogDefaults.MainInstruction;
            NativeConfiguration.pszContent = TaskDialogDefaults.Content;
            NativeConfiguration.pszVerificationText = null;
            NativeConfiguration.pszExpandedInformation = null;
            NativeConfiguration.pszExpandedControlText = null;
            NativeConfiguration.pszCollapsedControlText = null;
            NativeConfiguration.pszFooter = null;
        }

        public int ProgressBarMinimum { get; set; }

        public int ProgressBarMaximum { get; set; }

        public int ProgressBarValue { get; set; }

        public TaskDialogProgressBarState ProgressBarState { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool InvokeHelp { get; set; }

        public TaskDialogNativeMethods.TASKDIALOGCONFIG NativeConfiguration { get; private set; }

        public TaskDialogNativeMethods.TASKDIALOG_BUTTON[] Buttons { get; set; }

        public TaskDialogNativeMethods.TASKDIALOG_BUTTON[] RadioButtons { get; set; }

        public List<int> ElevatedButtons { get; set; }
    }
}