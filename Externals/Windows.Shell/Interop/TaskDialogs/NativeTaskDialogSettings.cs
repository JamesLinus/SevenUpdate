//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Encapsulates additional configuration needed by NativeTaskDialog
    ///   that it can't get from the TASKDIALOGCONFIG struct.
    /// </summary>
    internal class NativeTaskDialogSettings
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private TaskDialogNativeMethods.TaskdialogButton[] __buttons;

        /// <summary>
        /// </summary>
        private TaskDialogNativeMethods.TaskdialogButton[] __radioButtons;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        internal NativeTaskDialogSettings()
        {
            this.NativeConfiguration = new TaskDialogNativeMethods.TASKDIALOGCONFIG();

            // Apply standard settings.
            this.NativeConfiguration.cbSize = (uint)Marshal.SizeOf(this.NativeConfiguration);
            this.NativeConfiguration.hwndParent = IntPtr.Zero;
            this.NativeConfiguration.hInstance = IntPtr.Zero;
            this.NativeConfiguration.dwFlags = TaskDialogNativeMethods.TaskdialogFlagss.TdfAllowDialogCancellation;
            this.NativeConfiguration.dwCommonButtons = TaskDialogNativeMethods.TaskdialogCommonButtonFlagss.TdcbfokButton;
            this.NativeConfiguration.MainIcon = new TaskDialogNativeMethods.TaskdialogconfigIconUnion(0);
            this.NativeConfiguration.FooterIcon = new TaskDialogNativeMethods.TaskdialogconfigIconUnion(0);
            this.NativeConfiguration.cxWidth = TaskDialogDefaults.IdealWidth;

            // Zero out all the custom button fields.
            this.NativeConfiguration.cButtons = 0;
            this.NativeConfiguration.cRadioButtons = 0;
            this.NativeConfiguration.pButtons = IntPtr.Zero;
            this.NativeConfiguration.pRadioButtons = IntPtr.Zero;
            this.NativeConfiguration.nDefaultButton = 0;
            this.NativeConfiguration.nDefaultRadioButton = 0;

            // Various text defaults.
            this.NativeConfiguration.pszWindowTitle = TaskDialogDefaults.Caption;
            this.NativeConfiguration.pszMainInstruction = TaskDialogDefaults.MainInstruction;
            this.NativeConfiguration.pszContent = TaskDialogDefaults.Content;
            this.NativeConfiguration.pszVerificationText = null;
            this.NativeConfiguration.pszExpandedInformation = null;
            this.NativeConfiguration.pszExpandedControlText = null;
            this.NativeConfiguration.pszCollapsedControlText = null;
            this.NativeConfiguration.pszFooter = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public List<int> ElevatedButtons { get; set; }

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool InvokeHelp { get; set; }

        /// <summary>
        /// </summary>
        public TaskDialogNativeMethods.TASKDIALOGCONFIG NativeConfiguration { get; private set; }

        /// <summary>
        /// </summary>
        public int ProgressBarMaximum { get; set; }

        /// <summary>
        /// </summary>
        public int ProgressBarMinimum { get; set; }

        /// <summary>
        /// </summary>
        public TaskDialogProgressBarState ProgressBarState { get; set; }

        /// <summary>
        /// </summary>
        public int ProgressBarValue { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public TaskDialogNativeMethods.TaskdialogButton[] GetButtons()
        {
            return this.__buttons;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public TaskDialogNativeMethods.TaskdialogButton[] GetRadioButtons()
        {
            return this.__radioButtons;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void SetButtons(TaskDialogNativeMethods.TaskdialogButton[] value)
        {
            this.__buttons = value;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void SetRadioButtons(TaskDialogNativeMethods.TaskdialogButton[] value)
        {
            this.__radioButtons = value;
        }

        #endregion
    }
}