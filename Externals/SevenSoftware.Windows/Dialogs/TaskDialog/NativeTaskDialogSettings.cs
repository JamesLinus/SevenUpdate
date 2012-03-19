// <copyright file="NativeTaskDialogSettings.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    ///   Encapsulates additional configuration needed by NativeTaskDialog that it can't get from the TASKDIALOGCONFIG
    ///   struct.
    /// </summary>
    internal class NativeTaskDialogSettings
    {
        /// <summary>Initializes a new instance of the <see cref="NativeTaskDialogSettings" /> class.</summary>
        internal NativeTaskDialogSettings()
        {
            this.NativeConfiguration = new TaskDialogConfiguration();

            // Apply standard settings.
            this.NativeConfiguration.Size = (uint)Marshal.SizeOf(this.NativeConfiguration);
            this.NativeConfiguration.ParentHandle = IntPtr.Zero;
            this.NativeConfiguration.Instance = IntPtr.Zero;
            this.NativeConfiguration.TaskDialogFlags = TaskDialogOptions.AllowCancel;
            this.NativeConfiguration.CommonButtons = TaskDialogCommonButtons.Ok;
            this.NativeConfiguration.MainIcon = new IconUnion(0);
            this.NativeConfiguration.FooterIcon = new IconUnion(0);
            this.NativeConfiguration.Width = TaskDialogDefaults.IdealWidth;

            // Zero out all the custom button fields.
            this.NativeConfiguration.ButtonCount = 0;
            this.NativeConfiguration.RadioButtonCount = 0;
            this.NativeConfiguration.Buttons = IntPtr.Zero;
            this.NativeConfiguration.RadioButtons = IntPtr.Zero;
            this.NativeConfiguration.DefaultButtonIndex = 0;
            this.NativeConfiguration.DefaultRadioButtonIndex = 0;

            // Various text defaults.
            this.NativeConfiguration.WindowTitle = TaskDialogDefaults.Caption;
            this.NativeConfiguration.MainInstruction = TaskDialogDefaults.MainInstruction;
            this.NativeConfiguration.Content = TaskDialogDefaults.Content;
            this.NativeConfiguration.VerificationText = null;
            this.NativeConfiguration.ExpandedInformation = null;
            this.NativeConfiguration.ExpandedControlText = null;
            this.NativeConfiguration.CollapsedControlText = null;
            this.NativeConfiguration.FooterText = null;
        }

        /// <summary>Gets or sets a collection of <c>TaskDialogButton</c>.</summary>
        public TaskDialogButtonData[] Buttons { get; set; }

        /// <summary>Gets or sets the elevated buttons.</summary>
        /// <value>The elevated buttons.</value>
        public List<int> ElevatedButtons { get; set; }

        /// <summary>Gets or sets a value indicating whether to invoke help.</summary>
        public bool InvokeHelp { get; set; }

        /// <summary>Gets the native configuration.</summary>
        /// <value>The native configuration.</value>
        public TaskDialogConfiguration NativeConfiguration { get; private set; }

        /// <summary>Gets or sets the progress bar maximum.</summary>
        /// <value>The progress bar maximum.</value>
        public int ProgressBarMaximum { get; set; }

        /// <summary>Gets or sets the progress bar minimum.</summary>
        /// <value>The progress bar minimum.</value>
        public int ProgressBarMinimum { get; set; }

        /// <summary>Gets or sets the state of the progress bar.</summary>
        /// <value>The state of the progress bar.</value>
        public TaskDialogProgressBarState ProgressBarState { get; set; }

        /// <summary>Gets or sets the progress bar value.</summary>
        /// <value>The progress bar value.</value>
        public int ProgressBarValue { get; set; }

        /// <summary>Gets or sets a collection of <c>TaskDialogRadioButton</c>.</summary>
        public TaskDialogButtonData[] RadioButtons { get; set; }
    }
}