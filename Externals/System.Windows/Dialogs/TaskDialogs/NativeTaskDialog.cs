// ***********************************************************************
// <copyright file="NativeTaskDialog.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows.Controls;
    using System.Windows.Internal;

    /// <summary>Encapsulates the native logic required to create,  configure, and show a <see cref="TaskDialog"/>, via the TaskDialogIndirect() Win32 function.</summary>
    /// <remarks>A new instance of this class should  be created for each Message Box show, as the handles for <see cref="TaskDialogs"/> do not remain constant across calls to TaskDialogIndirect.</remarks>
    internal sealed class NativeTaskDialog : IDisposable
    {
        #region Constants and Fields

        /// <summary>The native dialog configuration</summary>
        private TaskDialogNativeMethods.TaskDialogConfig nativeDialogConfig;

        /// <summary>The outer dialog</summary>
        private TaskDialog outerDialog;

        /// <summary>The dialog settings</summary>
        private NativeTaskDialogSettings settings;

        /// <summary>The strings for the dialog</summary>
        private IntPtr[] updatedStrings = new IntPtr[Enum.GetNames(typeof(TaskDialogNativeMethods.TaskDialogElement)).Length];

        /// <summary>The collection of buttons</summary>
        private IntPtr buttonArray;

        /// <summary>Indicates if the <see cref = "CheckBox" /> is checked</summary>
        private bool checkBoxChecked;

        /// <summary>The pointer for the dialog</summary>
        private IntPtr dialogPointer;

        /// <summary>Indicates if the dialog is disposed</summary>
        private bool disposed;

        /// <summary>Indicates if the first radio button is clicked</summary>
        private bool firstRadioButtonClicked = true;

        /// <summary>The collection of radio buttons</summary>
        private IntPtr radioButtonArray;

        // Configuration is applied at dialog creation time.

        /// <summary>The selected button id</summary>
        private int selectedButtonID;

        /// <summary>The selected radio button id</summary>
        private int selectedRadioButtonID;

        /// <summary>The state of the dialog</summary>
        private DialogShowState showState = DialogShowState.PreShow;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NativeTaskDialog"/> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="outerDialog">The outer dialog.</param>
        internal NativeTaskDialog(NativeTaskDialogSettings settings, TaskDialog outerDialog)
        {
            this.nativeDialogConfig = settings.NativeConfiguration;
            this.settings = settings;

            // Wire up dialog proc message loop for this instance.
            this.nativeDialogConfig.Callback = new TaskDialogNativeMethods.TaskDialogCallBack(this.DialogProc);

            // Keep a reference to the outer shell, so we can notify.
            this.outerDialog = outerDialog;
        }

        /// <summary>Finalizes an instance of the <see cref="NativeTaskDialog"/> class.</summary>
        ~NativeTaskDialog()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>Gets the state of the dialog</summary>
        public DialogShowState ShowState
        {
            get
            {
                return this.showState;
            }
        }

        /// <summary>Gets a value indicating whether the <see cref = "CheckBox" /> is checked</summary>
        internal bool CheckBoxChecked
        {
            get
            {
                return this.checkBoxChecked;
            }
        }

        /// <summary>Gets the selected button ID</summary>
        internal int SelectedButtonID
        {
            get
            {
                return this.selectedButtonID;
            }
        }

        /// <summary>Gets the selected radio button ID</summary>
        internal int SelectedRadioButtonID
        {
            get
            {
                return this.selectedRadioButtonID;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>Finalizes an instance of the <see cref="NativeTaskDialog"/> class.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>Show debug message when the native dialog is showing</summary>
        internal void AssertCurrentlyShowing()
        {
            Debug.Assert(this.showState == DialogShowState.Showing, "Update*() methods should only be called while native dialog is showing");
        }

        /// <summary>
        /// The new task dialog does not support the existing
        ///   Win32 functions for closing (e.g. EndDialog()); instead,
        ///   a "click button" message is sent. In this case, we're
        ///   abstracting out to say that the <see cref="TaskDialog"/> consumer can
        ///   simply call "Close" and we'll "click" the cancel button.
        /// </summary>
        /// <param name="result">The result to give when closing the dialog</param>
        internal void NativeClose(TaskDialogResult result)
        {
            this.showState = DialogShowState.Closing;

            var id = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.Cancel;

            switch (result)
            {
                case TaskDialogResult.Close:
                    id = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.Close;
                    break;
                case TaskDialogResult.CustomButtonClicked:
                    id = 9; // custom buttons
                    break;
                case TaskDialogResult.No:
                    id = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.No;
                    break;
                case TaskDialogResult.Ok:
                    id = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.OK;
                    break;
                case TaskDialogResult.Retry:
                    id = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.Retry;
                    break;
                case TaskDialogResult.Yes:
                    id = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.Yes;
                    break;
            }

            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.ClickButton, id, 0);
        }

        /// <summary>Shows the native dialog</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        internal void NativeShow()
        {
            // Applies config struct and other settings, then
            // calls main Win32 function.
            if (this.settings == null)
            {
                throw new InvalidOperationException("An error has occurred in dialog configuration.");
            }

            // Do a last-minute parse of the various dialog control lists,  
            // and only allocate the memory at the last minute.
            this.MarshalDialogControlStructs();

            // Make the call and show the dialog.
            // NOTE: this call is BLOCKING, though the thread 
            // WILL re-enter via the DialogProc.
            try
            {
                this.showState = DialogShowState.Showing;

                // Here is the way we use "vanilla" P/Invoke to call 
                // TaskDialogIndirect().  
                var result = TaskDialogNativeMethods.TaskDialogIndirect(this.nativeDialogConfig, out this.selectedButtonID, out this.selectedRadioButtonID, out this.checkBoxChecked);

                if (ErrorHelper.Failed(result))
                {
                    string msg;
                    switch (result)
                    {
                        case Result.InvalidArg:
                            msg = "Invalid arguments to Win32 call.";
                            break;
                        case Result.OutOfMemory:
                            msg = "Dialog contents too complex.";
                            break;
                        default:
                            msg = String.Format(CultureInfo.CurrentCulture, "An unexpected internal error occurred in the Win32 call:{0:x}", result);
                            break;
                    }

                    var e = Marshal.GetExceptionForHR((int)result);
                    throw new Win32Exception(msg, e);
                }
            }
            catch (EntryPointNotFoundException)
            {
            }
            finally
            {
                this.showState = DialogShowState.Closed;
            }
        }

        /// <summary>Updates the button enabled.</summary>
        /// <param name="buttonID">The button ID.</param>
        /// <param name="enabled">if set to <see langword="true"/> [enabled].</param>
        internal void UpdateButtonEnabled(int buttonID, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.EnableButton, buttonID, enabled ? 1 : 0);
        }

        /// <summary>Updates the check box checked.</summary>
        /// <param name="isChecked">if set to <see langword="true"/> the checkbox is checked.</param>
        internal void UpdateCheckBoxChecked(bool isChecked)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.ClickVerification, isChecked ? 1 : 0, 1);
        }

        /// <summary>Updates the elevation icon.</summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="showIcon">if set to <see langword="true"/> [show icon].</param>
        internal void UpdateElevationIcon(int buttonId, bool showIcon)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetButtonElevationRequiredState, buttonId, Convert.ToInt32(showIcon));
        }

        /// <summary>Updates the expanded text.</summary>
        /// <param name="expandedText">The expanded text.</param>
        internal void UpdateExpandedText(string expandedText)
        {
            this.UpdateTextCore(expandedText, TaskDialogNativeMethods.TaskDialogElement.ExpandedInformation);
        }

        /// <summary>Updates the footer icon.</summary>
        /// <param name="footerIcon">The footer icon.</param>
        internal void UpdateFooterIcon(TaskDialogStandardIcon footerIcon)
        {
            this.UpdateIconCore(footerIcon, TaskDialogNativeMethods.TaskDialogIconElement.IconFooter);
        }

        /// <summary>Updates the footer text.</summary>
        /// <param name="footerText">The footer text.</param>
        internal void UpdateFooterText(string footerText)
        {
            this.UpdateTextCore(footerText, TaskDialogNativeMethods.TaskDialogElement.Footer);
        }

        /// <summary>Updates the instruction.</summary>
        /// <param name="instruction">The instruction.</param>
        internal void UpdateInstruction(string instruction)
        {
            this.UpdateTextCore(instruction, TaskDialogNativeMethods.TaskDialogElement.MainInstruction);
        }

        /// <summary>Updates the main icon.</summary>
        /// <param name="mainIcon">The main icon.</param>
        internal void UpdateMainIcon(TaskDialogStandardIcon mainIcon)
        {
            this.UpdateIconCore(mainIcon, TaskDialogNativeMethods.TaskDialogIconElement.IconMain);
        }

        /// <summary>Updates the progress bar range.</summary>
        internal void UpdateProgressBarRange()
        {
            this.AssertCurrentlyShowing();

            // Build range LPARAM - note it is in REVERSE intuitive order.
            var range = MakeLongParameter(this.settings.ProgressBarMaximum, this.settings.ProgressBarMinimum);

            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetProgressBarRange, 0, range);
        }

        /// <summary>Updates the state of the progress bar.</summary>
        /// <param name="state">The progress bar state</param>
        internal void UpdateProgressBarState(TaskDialogProgressBarState state)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetProgressBarState, (int)state, 0);
        }

        /// <summary>Updates the progress bar value.</summary>
        /// <param name="i">The progress value</param>
        internal void UpdateProgressBarValue(int i)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetProgressBarPos, i, 0);
        }

        /// <summary>Update the radio button when enabled has changed</summary>
        /// <param name="buttonID">The button ID.</param>
        /// <param name="enabled">if set to <see langword="true"/> the radio button is enabled.</param>
        internal void UpdateRadioButtonEnabled(int buttonID, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.EnableRadioButton, buttonID, enabled ? 1 : 0);
        }

        /// <summary>Updates the content text</summary>
        /// <param name="text">The text to update</param>
        internal void UpdateText(string text)
        {
            this.UpdateTextCore(text, TaskDialogNativeMethods.TaskDialogElement.Content);
        }

        /// <summary>Allocates and marshals buttons.</summary>
        /// <param name="buttons">The collection of buttons</param>
        /// <returns>The result</returns>
        private static IntPtr AllocateAndMarshalButtons(ICollection<TaskDialogNativeMethods.TaskDialogButtonData> buttons)
        {
            var initialPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TaskDialogNativeMethods.TaskDialogButtonData)) * buttons.Count);

            var currentPtr = initialPtr;
            foreach (var button in buttons)
            {
                Marshal.StructureToPtr(button, currentPtr, false);
                currentPtr = (IntPtr)((int)currentPtr + Marshal.SizeOf(button));
            }

            return initialPtr;
        }

        /// <summary>Makes a Long parameter</summary>
        /// <param name="a">The first parameter</param>
        /// <param name="b">The second parameter</param>
        /// <returns>The parameter as a long</returns>
        private static long MakeLongParameter(int a, int b)
        {
            return (a << 16) + b;
        }

        /// <summary>Processes dialog messages</summary>
        /// <param name="pointer">The handle for the dialog</param>
        /// <param name="msg">The message code to process</param>
        /// <param name="parameter">The button id</param>
        /// <param name="parameterLength">The hyperlink id.</param>
        /// <param name="data">The data to process</param>
        /// <returns>The result for the dialog</returns>
        private int DialogProc(IntPtr pointer, uint msg, IntPtr parameter, IntPtr parameterLength, IntPtr data)
        {
            // Fetch the HWND - it may be the first time we're getting it.
            this.dialogPointer = pointer;

            // Big switch on the various notifications the 
            // dialog proc can get.
            switch ((TaskDialogNativeMethods.TaskDialogNotification)msg)
            {
                case TaskDialogNativeMethods.TaskDialogNotification.Created:
                    var result = this.PerformDialogInitialization();
                    this.outerDialog.RaiseOpenedEvent();
                    return result;
                case TaskDialogNativeMethods.TaskDialogNotification.ButtonClicked:
                    return this.HandleButtonClick((int)parameter);
                case TaskDialogNativeMethods.TaskDialogNotification.RadioButtonClicked:
                    return this.HandleRadioButtonClick((int)parameter);
                case TaskDialogNativeMethods.TaskDialogNotification.HyperlinkClicked:
                    return this.HandleHyperlinkClick(parameterLength);
                case TaskDialogNativeMethods.TaskDialogNotification.Help:
                    return this.HandleHelpInvocation();
                case TaskDialogNativeMethods.TaskDialogNotification.Timer:
                    return this.HandleTick((int)parameter);
                case TaskDialogNativeMethods.TaskDialogNotification.Destroyed:
                    return this.PerformDialogCleanup();
                default:
                    break;
            }

            return (int)Result.OK;
        }

        /// <summary>Releases unmanaged and - optionally - managed resources</summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;

            // Single biggest resource - make sure the dialog 
            // itself has been instructed to close.
            if (this.showState == DialogShowState.Showing)
            {
                this.NativeClose(TaskDialogResult.Cancel);
            }

            // Clean up custom allocated strings that were updated
            // while the dialog was showing. Note that the strings
            // passed in the initial TaskDialogIndirect call will
            // be cleaned up automatically by the default 

            // marshalling logic.
            if (this.updatedStrings != null)
            {
                for (var i = 0; i < this.updatedStrings.Length; i++)
                {
                    if (this.updatedStrings[i] == IntPtr.Zero)
                    {
                        continue;
                    }

                    Marshal.FreeHGlobal(this.updatedStrings[i]);
                    this.updatedStrings[i] = IntPtr.Zero;
                }
            }

            // Clean up the button and radio button arrays, if any.
            if (this.buttonArray != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.buttonArray);
                this.buttonArray = IntPtr.Zero;
            }

            if (this.radioButtonArray != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.radioButtonArray);
                this.radioButtonArray = IntPtr.Zero;
            }

            if (!disposing)
            {
                return;
            }

            // Clean up managed resources - currently there are none that are interesting.
            if (this.outerDialog != null)
            {
                this.outerDialog.Dispose();
            }
        }

        /// <summary>Frees the old string.</summary>
        /// <param name="element">The element.</param>
        private void FreeOldString(TaskDialogNativeMethods.TaskDialogElement element)
        {
            var elementIndex = (int)element;
            if (this.updatedStrings[elementIndex] == IntPtr.Zero)
            {
                return;
            }

            Marshal.FreeHGlobal(this.updatedStrings[elementIndex]);
            this.updatedStrings[elementIndex] = IntPtr.Zero;
        }

        // Once the task dialog HWND is open, we need to send 
        // additional messages to configure it.

        /// <summary>Handles the button click.</summary>
        /// <param name="id">The id button id</param>
        /// <returns>The result</returns>
        private int HandleButtonClick(int id)
        {
            // First we raise a Click event, if there is a custom button
            // However, we implement Close() by sending a cancel button, so 
            // we don't want to raise a click event in response to that.
            if (this.showState != DialogShowState.Closing)
            {
                this.outerDialog.RaiseButtonClickEvent(id);
            }

            // Once that returns, we raise a Closing event for the dialog
            // The Win32 API handles button clicking-and-closing 
            // as an atomic action,
            // but it is more .NET friendly to split them up.
            // Unfortunately, we do NOT have the return values at this stage.
            return id <= 9 ? this.outerDialog.RaiseClosingEvent(id) : 1;
        }

        /// <summary>Handles the help invocation.</summary>
        /// <returns>The result</returns>
        private int HandleHelpInvocation()
        {
            this.outerDialog.RaiseHelpInvokedEvent();
            return ErrorHelper.Ignored;
        }

        /// <summary>Handles the hyperlink click.</summary>
        /// <param name="reference">The hyperlink reference</param>
        /// <returns>The result</returns>
        private int HandleHyperlinkClick(IntPtr reference)
        {
            var link = Marshal.PtrToStringUni(reference);
            this.outerDialog.RaiseHyperlinkClickEvent(link);

            return ErrorHelper.Ignored;
        }

        /// <summary>Handles the radio button click.</summary>
        /// <param name="id">The id of the radio button</param>
        /// <returns>The result</returns>
        private int HandleRadioButtonClick(int id)
        {
            // When the dialog sets the radio button to default, 
            // it (somewhat confusingly)issues a radio button clicked event
            // - we mask that out - though ONLY if
            // we do have a default radio button
            if (this.firstRadioButtonClicked && !this.IsOptionSet(TaskDialogNativeMethods.TaskDialogFlags.NoDefaultRadioButton))
            {
                this.firstRadioButtonClicked = false;
            }
            else
            {
                this.outerDialog.RaiseButtonClickEvent(id);
            }

            // Note: we don't raise Closing, as radio 
            // buttons are non-committing buttons
            return ErrorHelper.Ignored;
        }

        /// <summary>Handles timer ticks</summary>
        /// <param name="ticks">The number of ticks</param>
        /// <returns>The result</returns>
        private int HandleTick(int ticks)
        {
            this.outerDialog.RaiseTickEvent(ticks);
            return ErrorHelper.Ignored;
        }

        /// <summary>Determines whether [is option set] [the specified flag].</summary>
        /// <param name="flag">The option flags</param>
        /// <returns><see langword="true"/> if [is option set] [the specified flag]; otherwise, <see langword="false"/>.</returns>
        private bool IsOptionSet(TaskDialogNativeMethods.TaskDialogFlags flag)
        {
            return (this.nativeDialogConfig.flags & flag) == flag;
        }

        // Allocates a new string on the unmanaged heap, 
        // and stores the pointer so we can free it later.

        /// <summary>Makes a new string.</summary>
        /// <param name="s">The string to create</param>
        /// <param name="element">The element.</param>
        /// <returns>The pointer for the new string</returns>
        private IntPtr MakeNewString(string s, TaskDialogNativeMethods.TaskDialogElement element)
        {
            var newStringPtr = Marshal.StringToHGlobalUni(s);
            this.updatedStrings[(int)element] = newStringPtr;
            return newStringPtr;
        }

        // Checks to see if the given element already has an 
        // updated string, and if so, 
        // frees it. This is done in preparation for a call to 
        // MakeNewString(), to prevent
        // leaks from multiple updates calls on the same element 
        // within a single native dialog lifetime.

        // Builds the actual configuration that the 
        // NativeTaskDialog (and underlying Win32 API)
        // expects, by parsing the various control lists, 
        // marshaling to the unmanaged heap, etc.

        /// <summary>Marshals the dialog control structs.</summary>
        private void MarshalDialogControlStructs()
        {
            if (this.settings.GetButtons() != null && this.settings.GetButtons().Length > 0)
            {
                this.buttonArray = AllocateAndMarshalButtons(this.settings.GetButtons());
                this.settings.NativeConfiguration.ButtonCollection = this.buttonArray;
                this.settings.NativeConfiguration.ButtonLength = (uint)this.settings.GetButtons().Length;
            }

            if (this.settings.GetRadioButtons() == null || this.settings.GetRadioButtons().Length <= 0)
            {
                return;
            }

            this.radioButtonArray = AllocateAndMarshalButtons(this.settings.GetRadioButtons());
            this.settings.NativeConfiguration.RadioButtonCollection = this.radioButtonArray;
            this.settings.NativeConfiguration.RadioButtonsLength = (uint)this.settings.GetRadioButtons().Length;
        }

        /// <summary>Performs the dialog cleanup.</summary>
        /// <returns>The result code</returns>
        private int PerformDialogCleanup()
        {
            this.firstRadioButtonClicked = true;

            return ErrorHelper.Ignored;
        }

        /// <summary>Performs the dialog initialization.</summary>
        /// <returns>the result code</returns>
        private int PerformDialogInitialization()
        {
            // Initialize Progress or Marquee Bar.
            if (this.IsOptionSet(TaskDialogNativeMethods.TaskDialogFlags.ShowProgressBar))
            {
                this.UpdateProgressBarRange();

                // The order of the following is important - 
                // state is more important than value, 
                // and non-normal states turn off the bar value change 
                // animation, which is likely the intended
                // and preferable behavior.
                this.UpdateProgressBarState(this.settings.ProgressBarState);
                this.UpdateProgressBarValue(this.settings.ProgressBarValue);

                // Due to a bug that wasn't fixed in time for RTM of Vista,
                // second SendMessage is required if the state is non-Normal.
                this.UpdateProgressBarValue(this.settings.ProgressBarValue);
            }
            else if (this.IsOptionSet(TaskDialogNativeMethods.TaskDialogFlags.ShowMarqueeProgressBar))
            {
                // TDM_SET_PROGRESS_BAR_MARQUEE is necessary 
                // to cause the marquee to start animating.
                // Note that this internal task dialog setting is 
                // round-tripped when the marquee is
                // is set to different states, so it never has to 
                // be touched/sent again.
                this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetProgressBarMarquee, 1, 0);
                this.UpdateProgressBarState(this.settings.ProgressBarState);
            }

            if (this.settings.ElevatedButtons != null && this.settings.ElevatedButtons.Count > 0)
            {
                foreach (var id in this.settings.ElevatedButtons)
                {
                    this.UpdateElevationIcon(id, true);
                }
            }

            return ErrorHelper.Ignored;
        }

        /// <summary>Sends a message to the dialog</summary>
        /// <param name="msg">The message to send</param>
        /// <param name="parameter">The button id to send</param>
        /// <param name="parameterLength">The hyperlink</param>
        /// <returns>The result</returns>
        private int SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage msg, int parameter, long parameterLength)
        {
            // Be sure to at least assert here - 
            // messages to invalid handles often just disappear silently
            Debug.Assert(true, "HWND for dialog is null during SendMessage");

            return (int)NativeMethods.SendMessage(this.dialogPointer, (uint)msg, (IntPtr)parameter, new IntPtr(parameterLength));
        }

        /// <summary>Updates the icon</summary>
        /// <param name="icon">The icon to display</param>
        /// <param name="element">The element where the icon is displayed</param>
        private void UpdateIconCore(TaskDialogStandardIcon icon, TaskDialogNativeMethods.TaskDialogIconElement element)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.UpdateIcon, (int)element, (long)icon);
        }

        /// <summary>Updates the text</summary>
        /// <param name="text">The text to display</param>
        /// <param name="element">The element where the text is displayed on</param>
        private void UpdateTextCore(string text, TaskDialogNativeMethods.TaskDialogElement element)
        {
            this.AssertCurrentlyShowing();

            this.FreeOldString(element);
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetElementText, (int)element, (long)this.MakeNewString(text, element));
        }

        #endregion
    }
}