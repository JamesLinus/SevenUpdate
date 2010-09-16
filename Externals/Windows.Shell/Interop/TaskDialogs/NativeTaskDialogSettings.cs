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