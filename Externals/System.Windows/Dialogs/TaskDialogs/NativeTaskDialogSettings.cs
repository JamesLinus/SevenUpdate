// ***********************************************************************
// <copyright file=NativeTaskDialogSettings.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Encapsulates additional configuration needed by NativeTaskDialog
    ///   that it can't get from the TASKDIALOGCONFIG struct.
    /// </summary>
    internal sealed class NativeTaskDialogSettings
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private TaskDialogNativeMethods.TaskDialogButtonData[] buttons;

        /// <summary>
        /// </summary>
        private TaskDialogNativeMethods.TaskDialogButtonData[] radioButtons;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NativeTaskDialogSettings" /> class.
        /// </summary>
        internal NativeTaskDialogSettings()
        {
            this.NativeConfiguration = new TaskDialogNativeMethods.TaskDialogConfig();

            // Apply standard settings.
            this.NativeConfiguration.Size = (uint)Marshal.SizeOf(this.NativeConfiguration);
            this.NativeConfiguration.handleParent = IntPtr.Zero;
            this.NativeConfiguration.Instance = IntPtr.Zero;
            this.NativeConfiguration.flags = TaskDialogNativeMethods.TaskDialogFlags.AllowDialogCancellation;
            this.NativeConfiguration.CommonButtons = TaskDialogNativeMethods.TaskDialogCommonButtonFlags.OkButton;
            this.NativeConfiguration.MainIcon = new TaskDialogNativeMethods.TaskDialogConfigIconUnion(0);
            this.NativeConfiguration.FooterIcon = new TaskDialogNativeMethods.TaskDialogConfigIconUnion(0);
            this.NativeConfiguration.Width = TaskDialogDefaults.IdealWidth;

            // Zero out all the custom button fields.
            this.NativeConfiguration.ButtonLength = 0;
            this.NativeConfiguration.RadioButtonsLength = 0;
            this.NativeConfiguration.ButtonCollection = IntPtr.Zero;
            this.NativeConfiguration.RadioButtonCollection = IntPtr.Zero;
            this.NativeConfiguration.DefaultButton = 0;
            this.NativeConfiguration.DefaultRadioButton = 0;

            // Various text defaults.
            this.NativeConfiguration.WindowTitle = TaskDialogDefaults.Caption;
            this.NativeConfiguration.MainInstruction = TaskDialogDefaults.MainInstruction;
            this.NativeConfiguration.Content = TaskDialogDefaults.Content;
            this.NativeConfiguration.VerificationText = null;
            this.NativeConfiguration.ExpandedInformation = null;
            this.NativeConfiguration.ExpandedControlText = null;
            this.NativeConfiguration.CollapsedControlText = null;
            this.NativeConfiguration.Footer = null;
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
        public TaskDialogNativeMethods.TaskDialogButtonData[] GetButtons()
        {
            return this.buttons;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public TaskDialogNativeMethods.TaskDialogButtonData[] GetRadioButtons()
        {
            return this.radioButtons;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void SetButtons(TaskDialogNativeMethods.TaskDialogButtonData[] value)
        {
            this.buttons = value;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void SetRadioButtons(TaskDialogNativeMethods.TaskDialogButtonData[] value)
        {
            this.radioButtons = value;
        }

        #endregion
    }
}