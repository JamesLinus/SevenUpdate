// <copyright file="NativeTaskDialog.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using SevenSoftware.Windows.Internal;
    using SevenSoftware.Windows.Properties;

    /// <summary>
    ///   Encapsulates the native logic required to create, configure, and show a TaskDialog, via the
    ///   TaskDialogIndirect() Win32 function.
    /// </summary>
    /// <remarks>
    ///   A new instance of this class should be created for each messagebox show, as the HWNDs for TaskDialogs do not
    ///   remain constant across calls to TaskDialogIndirect.
    /// </remarks>
    internal class NativeTaskDialog : IDisposable
    {
        /// <summary>The collection of buttons.</summary>
        private IntPtr buttonArray;

        /// <summary>The handle for the dialog.</summary>
        private IntPtr dialogHandle;

        /// <summary>Indicates if the dialog is disposed.</summary>
        private bool disposed;

        /// <summary>Indicates if the first radio button is clicked.</summary>
        private bool firstRadioButtonClicked = true;

        /// <summary>Indicates if the first radio button is clicked.</summary>
        private TaskDialogConfiguration nativeDialogConfig;

        /// <summary>The outer dialog.</summary>
        private TaskDialog outerDialog;

        /// <summary>The collection of radio buttons.</summary>
        private IntPtr radioButtonArray;

        /// <summary>The dialog settings.</summary>
        private NativeTaskDialogSettings settings;

        /// <summary>The strings for the dialog.</summary>
        private IntPtr[] updatedStrings = new IntPtr[Enum.GetNames(typeof(TaskDialogElements)).Length];

        /// <summary>Initializes a new instance of the <see cref="NativeTaskDialog" /> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="outerDialog">The outer dialog.</param>
        internal NativeTaskDialog(NativeTaskDialogSettings settings, TaskDialog outerDialog)
        {
            this.nativeDialogConfig = settings.NativeConfiguration;
            this.settings = settings;

            // Wireup dialog proc message loop for this instance.
            this.nativeDialogConfig.Callback = this.DialogProc;

            this.ShowState = DialogShowState.PreShow;

            // Keep a reference to the outer shell, so we can notify.
            this.outerDialog = outerDialog;
        }

        /// <summary>Finalizes an instance of the <see cref="NativeTaskDialog" /> class.</summary>
        ~NativeTaskDialog()
        {
            this.Dispose(false);
        }

        /// <summary>Gets a value indicating whether the <c>CheckBox</c> is checked.</summary>
        public bool CheckBoxChecked { get; private set; }

        /// <summary>Gets the selected button ID.</summary>
        public int SelectedButtonId { get; private set; }

        /// <summary>Gets the selected radio button ID.</summary>
        public int SelectedRadioButtonId { get; private set; }

        /// <summary>Gets the state of the dialog.</summary>
        public DialogShowState ShowState { get; private set; }

        /// <summary>Finalizes an instance of the NativeTaskDialog class.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Show debug message when the native dialog is showing.</summary>
        internal void AssertCurrentlyShowing()
        {
            Debug.Assert(
                this.ShowState == DialogShowState.Showing, 
                "Update*() methods should only be called while native dialog is showing");
        }

        /// <summary>
        ///   The new task dialog does not support the existing Win32 functions for closing (e.g. EndDialog()); instead,
        ///   a "click button" message is sent. In this case, we're abstracting out to say that the <c>TaskDialog</c>
        ///   consumer can simply call "Close" and we'll "click" the cancel button. .
        /// </summary>
        /// <param name="result">The result to give when closing the dialog.</param>
        internal void NativeClose(TaskDialogResult result)
        {
            this.ShowState = DialogShowState.Closing;

            int id;
            switch (result)
            {
                case TaskDialogResult.Close:
                    id = (int)TaskDialogCommonButtonReturnIds.Close;
                    break;
                case TaskDialogResult.CustomButtonClicked:
                    id = TaskDialogDefaults.MinimumDialogControlId; // custom buttons
                    break;
                case TaskDialogResult.No:
                    id = (int)TaskDialogCommonButtonReturnIds.No;
                    break;
                case TaskDialogResult.Ok:
                    id = (int)TaskDialogCommonButtonReturnIds.Ok;
                    break;
                case TaskDialogResult.Retry:
                    id = (int)TaskDialogCommonButtonReturnIds.Retry;
                    break;
                case TaskDialogResult.Yes:
                    id = (int)TaskDialogCommonButtonReturnIds.Yes;
                    break;
                default:
                    id = (int)TaskDialogCommonButtonReturnIds.Cancel;
                    break;
            }

            this.SendMessageHelper(TaskDialogMessages.ClickButton, id, 0);
        }

        /// <summary>Shows the native dialog.</summary>
        internal void NativeShow()
        {
            // Applies config struct and other settings, then calls main Win32 function.
            if (this.settings == null)
            {
                throw new InvalidOperationException(Resources.NativeTaskDialogConfigurationError);
            }

            // Do a last-minute parse of the various dialog control lists, and only allocate the memory at the last
            // minute.
            this.MarshalDialogControlStructs();

            // Make the call and show the dialog. NOTE: this call is BLOCKING, though the thread WILL re-enter via the
            // DialogProc.
            try
            {
                this.ShowState = DialogShowState.Showing;

                int selectedButtonId;
                int selectedRadioButtonId;
                bool checkBoxChecked;

                // Here is the way we use "vanilla" P/Invoke to call TaskDialogIndirect().  
                Result result = NativeMethods.TaskDialogIndirect(
                    this.nativeDialogConfig, out selectedButtonId, out selectedRadioButtonId, out checkBoxChecked);

                if (ErrorHelper.Failed(result))
                {
                    string msg;
                    switch (result)
                    {
                        case Result.InvalidArguments:
                            msg = Resources.NativeTaskDialogInternalErrorArgs;
                            break;
                        case Result.OutOfMemory:
                            msg = Resources.NativeTaskDialogInternalErrorComplex;
                            break;
                        default:
                            msg = string.Format(
                                CultureInfo.InvariantCulture, Resources.NativeTaskDialogInternalErrorUnexpected, result);
                            break;
                    }

                    Exception e = Marshal.GetExceptionForHR((int)result);
                    throw new Win32Exception(msg, e);
                }

                this.SelectedButtonId = selectedButtonId;
                this.SelectedRadioButtonId = selectedRadioButtonId;
                this.CheckBoxChecked = checkBoxChecked;
            }
            catch (EntryPointNotFoundException exc)
            {
                throw new NotSupportedException(Resources.NativeTaskDialogVersionError, exc);
            }
            finally
            {
                this.ShowState = DialogShowState.Closed;
            }
        }

        /// <summary>Updates the button enabled.</summary>
        /// <param name="buttonId">The button ID.</param>
        /// <param name="enabled">If set to <c>True</c> enabled.</param>
        internal void UpdateButtonEnabled(int buttonId, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogMessages.EnableButton, buttonId, enabled ? 1 : 0);
        }

        /// <summary>Updates the check box checked.</summary>
        /// <param name="isChecked">If set to <c>True</c> the checkbox is checked.</param>
        internal void UpdateCheckBoxChecked(bool isChecked)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogMessages.ClickVerification, isChecked ? 1 : 0, 1);
        }

        /// <summary>Updates the elevation icon.</summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="showIcon">If set to <c>True</c> show the uac icon</param>
        internal void UpdateElevationIcon(int buttonId, bool showIcon)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(
                TaskDialogMessages.SetButtonElevationRequiredState, buttonId, Convert.ToInt32(showIcon));
        }

        /// <summary>Updates the expanded text.</summary>
        /// <param name="expandedText">The expanded text.</param>
        internal void UpdateExpandedText(string expandedText)
        {
            this.UpdateTextCore(expandedText, TaskDialogElements.ExpandedInformation);
        }

        /// <summary>Updates the footer icon.</summary>
        /// <param name="footerIcon">The footer icon.</param>
        internal void UpdateFooterIcon(TaskDialogStandardIcon footerIcon)
        {
            this.UpdateIconCore(footerIcon, TaskDialogIconElement.Footer);
        }

        /// <summary>Updates the footer text.</summary>
        /// <param name="footerText">The footer text.</param>
        internal void UpdateFooterText(string footerText)
        {
            this.UpdateTextCore(footerText, TaskDialogElements.Footer);
        }

        /// <summary>Updates the instruction.</summary>
        /// <param name="instruction">The instruction.</param>
        internal void UpdateInstruction(string instruction)
        {
            this.UpdateTextCore(instruction, TaskDialogElements.MainInstruction);
        }

        /// <summary>Updates the main icon.</summary>
        /// <param name="mainIcon">The main icon.</param>
        internal void UpdateMainIcon(TaskDialogStandardIcon mainIcon)
        {
            this.UpdateIconCore(mainIcon, TaskDialogIconElement.Main);
        }

        /// <summary>Updates the progress bar range.</summary>
        internal void UpdateProgressBarRange()
        {
            this.AssertCurrentlyShowing();

            // Build range LPARAM - note it is in REVERSE intuitive order.
            long range = MakeLongLongParam(this.settings.ProgressBarMaximum, this.settings.ProgressBarMinimum);

            this.SendMessageHelper(TaskDialogMessages.SetProgressBarRange, 0, range);
        }

        /// <summary>Updates the state of the progress bar.</summary>
        /// <param name="state">The progress bar state.</param>
        internal void UpdateProgressBarState(TaskDialogProgressBarState state)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogMessages.SetProgressBarState, (int)state, 0);
        }

        /// <summary>Updates the progress bar value.</summary>
        /// <param name="i">The progress value.</param>
        internal void UpdateProgressBarValue(int i)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogMessages.SetProgressBarPosition, i, 0);
        }

        /// <summary>Update the radio button when enabled has changed.</summary>
        /// <param name="buttonId">The button ID.</param>
        /// <param name="enabled">If set to <c>True</c> the radio button is enabled.</param>
        internal void UpdateRadioButtonEnabled(int buttonId, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogMessages.EnableRadioButton, buttonId, enabled ? 1 : 0);
        }

        /// <summary>Updates the content text.</summary>
        /// <param name="text">The text to update.</param>
        internal void UpdateText(string text)
        {
            this.UpdateTextCore(text, TaskDialogElements.Content);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing">Release both managed and unmanaged resources; <c>False</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                // Single biggest resource - make sure the dialog itself has been instructed to close.
                if (this.ShowState == DialogShowState.Showing)
                {
                    this.NativeClose(TaskDialogResult.Cancel);
                }

                // Clean up custom allocated strings that were updated while the dialog was showing. Note that the
                // strings passed in the initial TaskDialogIndirect call will be cleaned up automagically by the default
                // marshalling logic.
                if (this.updatedStrings != null)
                {
                    for (int i = 0; i < this.updatedStrings.Length; i++)
                    {
                        if (this.updatedStrings[i] != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(this.updatedStrings[i]);
                            this.updatedStrings[i] = IntPtr.Zero;
                        }
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

                if (disposing)
                {
                    // Clean up managed resources - currently there are none that are interesting.
                }
            }
        }

        /// <summary>Allocates and marshals buttons.</summary>
        /// <param name="buttons">The collection of buttons.</param>
        /// <returns>The result.</returns>
        private static IntPtr AllocateAndMarshalButtons(ICollection<TaskDialogButtonData> buttons)
        {
            IntPtr initialPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TaskDialogButtonData)) * buttons.Count);

            IntPtr currentPtr = initialPtr;
            foreach (var button in buttons)
            {
                Marshal.StructureToPtr(button, currentPtr, false);
                currentPtr = (IntPtr)((int)currentPtr + Marshal.SizeOf(button));
            }

            return initialPtr;
        }

        /// <summary>Makes a Long parameter.</summary>
        /// <param name="a">The first parameter.</param>
        /// <param name="b">The second parameter.</param>
        /// <returns>The parameter as a long.</returns>
        private static long MakeLongLongParam(int a, int b)
        {
            return (a << 16) + b;
        }

        /// <summary>Processes dialog messages.</summary>
        /// <param name="windowHandle">The handle for the dialog.</param>
        /// <param name="message">The message code to process.</param>
        /// <param name="parameter">The button id.</param>
        /// <param name="parameterLength">The hyperlink id.</param>
        /// <param name="referenceData">The reference data</param>
        /// <returns>The result for the dialog.</returns>
        private int DialogProc(
            IntPtr windowHandle, uint message, IntPtr parameter, IntPtr parameterLength, IntPtr referenceData)
        {
            // Fetch the HWND - it may be the first time we're getting it.
            this.dialogHandle = windowHandle;

            // Big switch on the various notifications the dialog proc can get.
            switch ((TaskDialogNotifications)message)
            {
                case TaskDialogNotifications.Created:
                    int result = this.PerformDialogInitialization();
                    this.outerDialog.RaiseOpenedEvent();
                    return result;
                case TaskDialogNotifications.ButtonClicked:
                    return this.HandleButtonClick((int)parameter);
                case TaskDialogNotifications.RadioButtonClicked:
                    return this.HandleRadioButtonClick((int)parameter);
                case TaskDialogNotifications.HyperlinkClicked:
                    return this.HandleHyperlinkClick(parameterLength);
                case TaskDialogNotifications.Help:
                    return this.HandleHelpInvocation();
                case TaskDialogNotifications.Timer:
                    return this.HandleTick((int)parameter);
                case TaskDialogNotifications.Destroyed:
                    return this.PerformDialogCleanup();
            }

            return (int)Result.Ok;
        }

        /// <summary>Frees the old string.</summary>
        /// <param name="element">The element.</param>
        private void FreeOldString(TaskDialogElements element)
        {
            var elementIndex = (int)element;
            if (this.updatedStrings[elementIndex] != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.updatedStrings[elementIndex]);
                this.updatedStrings[elementIndex] = IntPtr.Zero;
            }
        }

        /// <summary>Once the task dialog HWND is open, we need to send additional messages to configure it.</summary>
        /// <param name="id">The id button id.</param>
        /// <returns>The result.</returns>
        private int HandleButtonClick(int id)
        {
            // First we raise a Click event, if there is a custom button However, we implement Close() by sending a
            // cancel button, so we don't want to raise a click event in response to that.
            if (this.ShowState != DialogShowState.Closing)
            {
                this.outerDialog.RaiseButtonClickEvent(id);
            }

            // Once that returns, we raise a Closing event for the dialog The Win32 API handles button
            // clicking-and-closing as an atomic action, but it is more .NET friendly to split them up. Unfortunately,
            // we do NOT have the return values at this stage.
            if (id < TaskDialogDefaults.MinimumDialogControlId)
            {
                return this.outerDialog.RaiseClosingEvent(id);
            }

            return (int)Result.False;
        }

        /// <summary>Handles the help invocation.</summary>
        /// <returns>The result.</returns>
        private int HandleHelpInvocation()
        {
            this.outerDialog.RaiseHelpInvokedEvent();
            return ErrorHelper.Ignored;
        }

        /// <summary>Handles the hyperlink click.</summary>
        /// <param name="reference">The hyperlink reference.</param>
        /// <returns>The result.</returns>
        private int HandleHyperlinkClick(IntPtr reference)
        {
            string link = Marshal.PtrToStringUni(reference);
            this.outerDialog.RaiseHyperlinkClickEvent(link);

            return ErrorHelper.Ignored;
        }

        /// <summary>Handles the radio button click.</summary>
        /// <param name="id">The id of the radio button.</param>
        /// <returns>The result.</returns>
        private int HandleRadioButtonClick(int id)
        {
            // When the dialog sets the radio button to default, it (somewhat confusingly)issues a radio button clicked
            // event
            // - we mask that out - though ONLY if
            // we do have a default radio button
            if (this.firstRadioButtonClicked && !this.IsOptionSet(TaskDialogOptions.NoDefaultRadioButton))
            {
                this.firstRadioButtonClicked = false;
            }
            else
            {
                this.outerDialog.RaiseButtonClickEvent(id);
            }

            // Note: we don't raise Closing, as radio buttons are non-committing buttons
            return ErrorHelper.Ignored;
        }

        /// <summary>Handles timer ticks.</summary>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>The result.</returns>
        private int HandleTick(int ticks)
        {
            this.outerDialog.RaiseTickEvent(ticks);
            return ErrorHelper.Ignored;
        }

        /// <summary>Determines whether the options are set for the dialog</summary>
        /// <param name="flag">The option flags.</param>
        /// <returns><c>True</c> if options are set; otherwise, <c>False</c>.</returns>
        private bool IsOptionSet(TaskDialogOptions flag)
        {
            return (this.nativeDialogConfig.TaskDialogFlags & flag) == flag;
        }

        /// <summary>Allocates a new string on the unmanaged heap, and stores the pointer so we can free it later.</summary>
        /// <param name="text">The string to create.</param>
        /// <param name="element">The element.</param>
        /// <returns>The pointer for the new string.</returns>
        private IntPtr MakeNewString(string text, TaskDialogElements element)
        {
            IntPtr newStringPtr = Marshal.StringToHGlobalUni(text);
            this.updatedStrings[(int)element] = newStringPtr;
            return newStringPtr;
        }

        /// <summary>
        ///   Checks to see if the given element already has an updated string, and if so, frees it. This is done in
        ///   preparation for a call to MakeNewString(), to prevent leaks from multiple updates calls on the same
        ///   element within a single native dialog lifetime.
        /// </summary>
        /// <remarks>
        ///   Builds the actual configuration that the NativeTaskDialog (and underlying Win32 API) expects, by parsing
        ///   the various control lists, marshaling to the unmanaged heap, etc.
        /// </remarks>
        private void MarshalDialogControlStructs()
        {
            if (this.settings.Buttons != null && this.settings.Buttons.Length > 0)
            {
                this.buttonArray = AllocateAndMarshalButtons(this.settings.Buttons);
                this.settings.NativeConfiguration.Buttons = this.buttonArray;
                this.settings.NativeConfiguration.ButtonCount = (uint)this.settings.Buttons.Length;
            }

            if (this.settings.RadioButtons != null && this.settings.RadioButtons.Length > 0)
            {
                this.radioButtonArray = AllocateAndMarshalButtons(this.settings.RadioButtons);
                this.settings.NativeConfiguration.RadioButtons = this.radioButtonArray;
                this.settings.NativeConfiguration.RadioButtonCount = (uint)this.settings.RadioButtons.Length;
            }
        }

        /// <summary>Performs the dialog cleanup.</summary>
        /// <returns>The result code.</returns>
        private int PerformDialogCleanup()
        {
            this.firstRadioButtonClicked = true;

            return ErrorHelper.Ignored;
        }

        /// <summary>Performs the dialog initialization.</summary>
        /// <returns>The result code.</returns>
        private int PerformDialogInitialization()
        {
            // Initialize Progress or Marquee Bar.
            if (this.IsOptionSet(TaskDialogOptions.ShowProgressBar))
            {
                this.UpdateProgressBarRange();

                // The order of the following is important - state is more important than value, and non-normal states
                // turn off the bar value change animation, which is likely the intended and preferable behavior.
                this.UpdateProgressBarState(this.settings.ProgressBarState);
                this.UpdateProgressBarValue(this.settings.ProgressBarValue);

                // Due to a bug that wasn't fixed in time for RTM of Vista, second SendMessage is required if the state
                // is non-Normal.
                this.UpdateProgressBarValue(this.settings.ProgressBarValue);
            }
            else if (this.IsOptionSet(TaskDialogOptions.ShowMarqueeProgressBar))
            {
                // TDM_SET_PROGRESS_BAR_MARQUEE is necessary to cause the marquee to start animating. Note that this
                // internal task dialog setting is round-tripped when the marquee is is set to different states, so it
                // never has to be touched/sent again.
                this.SendMessageHelper(TaskDialogMessages.SetProgressBarMarquee, 1, 0);
                this.UpdateProgressBarState(this.settings.ProgressBarState);
            }

            if (this.settings.ElevatedButtons != null && this.settings.ElevatedButtons.Count > 0)
            {
                foreach (int id in this.settings.ElevatedButtons)
                {
                    this.UpdateElevationIcon(id, true);
                }
            }

            return ErrorHelper.Ignored;
        }

        /// <summary>Sends a message to the dialog.</summary>
        /// <param name="message">The message to send.</param>
        /// <param name="parameter">The button id to send.</param>
        /// <param name="parameterLength">The hyperlink.</param>
        /// <returns>The result.</returns>
        private int SendMessageHelper(TaskDialogMessages message, int parameter, long parameterLength)
        {
            // Be sure to at least assert here - messages to invalid handles often just disappear silently
            Debug.Assert(this.dialogHandle != null, "HWND for dialog is null during SendMessage");

            return
                (int)
                NativeMethods.SendMessage(
                    this.dialogHandle, (uint)message, (IntPtr)parameter, new IntPtr(parameterLength));
        }

        /// <summary>Updates the icon.</summary>
        /// <param name="icon">The icon to display.</param>
        /// <param name="element">The element where the icon is displayed.</param>
        private void UpdateIconCore(TaskDialogStandardIcon icon, TaskDialogIconElement element)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogMessages.UpdateIcon, (int)element, (long)icon);
        }

        /// <summary>Updates the text.</summary>
        /// <param name="text">The text to display.</param>
        /// <param name="element">The element where the text is displayed on.</param>
        private void UpdateTextCore(string text, TaskDialogElements element)
        {
            this.AssertCurrentlyShowing();

            this.FreeOldString(element);
            this.SendMessageHelper(
                TaskDialogMessages.SetElementText, (int)element, (long)this.MakeNewString(text, element));
        }
    }
}