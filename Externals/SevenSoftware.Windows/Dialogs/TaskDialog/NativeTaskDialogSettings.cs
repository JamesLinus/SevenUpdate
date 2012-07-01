// <copyright file="NativeTaskDialogSettings.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>
    ///   Encapsulates additional configuration needed by NativeTaskDialog that it can't get from the TASKDIALOGCONFIG
    ///   struct.
    /// </summary>
    internal class NativeTaskDialogSettings
    {
        /// <summary>Initializes a new instance of the <see cref="NativeTaskDialogSettings" /> class.</summary>
        internal NativeTaskDialogSettings()
        {
            NativeConfiguration = new TaskDialogConfiguration();

            // Apply standard settings.
            NativeConfiguration.Size = (uint)Marshal.SizeOf(NativeConfiguration);
            NativeConfiguration.ParentHandle = IntPtr.Zero;
            NativeConfiguration.Instance = IntPtr.Zero;
            NativeConfiguration.TaskDialogFlags = TaskDialogOptions.AllowCancel;
            NativeConfiguration.CommonButtons = TaskDialogCommonButtons.Ok;
            NativeConfiguration.MainIcon = new IconUnion(0);
            NativeConfiguration.FooterIcon = new IconUnion(0);
            NativeConfiguration.Width = TaskDialogDefaults.IdealWidth;

            // Zero out all the custom button fields.
            NativeConfiguration.ButtonCount = 0;
            NativeConfiguration.RadioButtonCount = 0;
            NativeConfiguration.Buttons = IntPtr.Zero;
            NativeConfiguration.RadioButtons = IntPtr.Zero;
            NativeConfiguration.DefaultButtonIndex = 0;
            NativeConfiguration.DefaultRadioButtonIndex = 0;

            // Various text defaults.
            NativeConfiguration.WindowTitle = TaskDialogDefaults.Caption;
            NativeConfiguration.MainInstruction = TaskDialogDefaults.MainInstruction;
            NativeConfiguration.Content = TaskDialogDefaults.Content;
            NativeConfiguration.VerificationText = null;
            NativeConfiguration.ExpandedInformation = null;
            NativeConfiguration.ExpandedControlText = null;
            NativeConfiguration.CollapsedControlText = null;
            NativeConfiguration.FooterText = null;
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