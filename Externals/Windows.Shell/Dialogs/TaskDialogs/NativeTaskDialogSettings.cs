// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Dialogs.TaskDialogs
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
        private TaskDialogNativeMethods.TaskDialogButton[] buttons;

        /// <summary>
        /// </summary>
        private TaskDialogNativeMethods.TaskDialogButton[] radioButtons;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeTaskDialogSettings"/> class.
        /// </summary>
        internal NativeTaskDialogSettings()
        {
            this.NativeConfiguration = new TaskDialogNativeMethods.TaskDialogConfig();

            // Apply standard settings.
            this.NativeConfiguration.cbSize = (uint)Marshal.SizeOf(this.NativeConfiguration);
            this.NativeConfiguration.hwndParent = IntPtr.Zero;
            this.NativeConfiguration.hInstance = IntPtr.Zero;
            this.NativeConfiguration.dwFlags = TaskDialogNativeMethods.TaskDialogFlags.AllowDialogCancellation;
            this.NativeConfiguration.dwCommonButtons = TaskDialogNativeMethods.TaskDialogCommonButtonFlags.OkButton;
            this.NativeConfiguration.MainIcon = new TaskDialogNativeMethods.TaskDialogConfigIconUnion(0);
            this.NativeConfiguration.FooterIcon = new TaskDialogNativeMethods.TaskDialogConfigIconUnion(0);
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
        public TaskDialogNativeMethods.TaskDialogConfig NativeConfiguration { get; private set; }

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
        public TaskDialogNativeMethods.TaskDialogButton[] GetButtons()
        {
            return this.buttons;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public TaskDialogNativeMethods.TaskDialogButton[] GetRadioButtons()
        {
            return this.radioButtons;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void SetButtons(TaskDialogNativeMethods.TaskDialogButton[] value)
        {
            this.buttons = value;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void SetRadioButtons(TaskDialogNativeMethods.TaskDialogButton[] value)
        {
            this.radioButtons = value;
        }

        #endregion
    }
}