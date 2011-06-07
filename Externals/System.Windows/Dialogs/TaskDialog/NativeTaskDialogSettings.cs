// ***********************************************************************
// <copyright file="NativeTaskDialogSettings.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ************************************************************************

namespace System.Windows.Dialogs
{
    using Collections.Generic;

    using Runtime.InteropServices;

    /// <summary>
    ///   Encapsulates additional configuration needed by <c>NativeTaskDialog</c> that it can't get from the <see
    ///    cref="TaskDialogConfig" /> struct.
    /// </summary>
    internal sealed class NativeTaskDialogSettings
    {
        #region Constants and Fields

        /// <summary>
        ///   A collection of <c>TaskDialogButton</c>.
        /// </summary>
        private TaskDialogButtonData[] buttons;

        /// <summary>
        ///   A collection of <c>TaskDialogRadioButton</c>.
        /// </summary>
        private TaskDialogButtonData[] radioButtons;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the NativeTaskDialogSettings class.
        /// </summary>
        internal NativeTaskDialogSettings()
        {
            this.NativeConfiguration = new TaskDialogConfig();

            // Apply standard settings.
            this.NativeConfiguration.Size = (uint)Marshal.SizeOf(this.NativeConfiguration);
            this.NativeConfiguration.HandleParent = IntPtr.Zero;
            this.NativeConfiguration.Instance = IntPtr.Zero;
            this.NativeConfiguration.Flags = TaskDialogFlags.AllowDialogCancellation;
            this.NativeConfiguration.CommonButtons = TaskDialogCommonButtonFlags.OkButton;
            this.NativeConfiguration.MainIcon = new TaskDialogConfigIconUnion(0);
            this.NativeConfiguration.FooterIcon = new TaskDialogConfigIconUnion(0);
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
        ///   Gets or sets the elevated buttons.
        /// </summary>
        /// <value>The elevated buttons.</value>
        public List<int> ElevatedButtons { get; set; }

        /// <summary>
        ///   Gets the native configuration.
        /// </summary>
        /// <value>The native configuration.</value>
        public TaskDialogConfig NativeConfiguration { get; private set; }

        /// <summary>
        ///   Gets or sets the progress bar maximum.
        /// </summary>
        /// <value>The progress bar maximum.</value>
        public int ProgressBarMaximum { get; set; }

        /// <summary>
        ///   Gets or sets the progress bar minimum.
        /// </summary>
        /// <value>The progress bar minimum.</value>
        public int ProgressBarMinimum { get; set; }

        /// <summary>
        ///   Gets or sets the state of the progress bar.
        /// </summary>
        /// <value>The state of the progress bar.</value>
        public TaskDialogProgressBarState ProgressBarState { get; set; }

        /// <summary>
        ///   Gets or sets the progress bar value.
        /// </summary>
        /// <value>The progress bar value.</value>
        public int ProgressBarValue { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Gets the buttons.
        /// </summary>
        /// <returns>
        ///   The collection of <c>TaskDialogButton</c>'s of the dialog dialog.
        /// </returns>
        public TaskDialogButtonData[] GetButtons()
        {
            return this.buttons;
        }

        /// <summary>
        ///   Gets the radio buttons.
        /// </summary>
        /// <returns>
        ///   The collection of <c>TaskDialogRadioButton</c>'s of the dialog dialog.
        /// </returns>
        public TaskDialogButtonData[] GetRadioButtons()
        {
            return this.radioButtons;
        }

        /// <summary>
        ///   Sets the buttons.
        /// </summary>
        /// <param name="value">
        ///   The value.
        /// </param>
        public void SetButtons(TaskDialogButtonData[] value)
        {
            this.buttons = value;
        }

        /// <summary>
        ///   Sets the radio buttons.
        /// </summary>
        /// <param name="value">
        ///   The value.
        /// </param>
        public void SetRadioButtons(TaskDialogButtonData[] value)
        {
            this.radioButtons = value;
        }

        #endregion
    }
}