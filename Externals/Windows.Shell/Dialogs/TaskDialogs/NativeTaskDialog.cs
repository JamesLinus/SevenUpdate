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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Encapsulates the native logic required to create, 
    ///   configure, and show a TaskDialog, 
    ///   via the TaskDialogIndirect() Win32 function.
    /// </summary>
    /// <remarks>
    /// A new instance of this class should 
    ///   be created for each messagebox show, as
    ///   the HWNDs for TaskDialogs do not remain constant 
    ///   across calls to TaskDialogIndirect.
    /// </remarks>
    internal class NativeTaskDialog : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly TaskDialogNativeMethods.TaskDialogConfig nativeDialogConfig;

        /// <summary>
        /// </summary>
        private readonly TaskDialog outerDialog;

        /// <summary>
        /// </summary>
        private readonly NativeTaskDialogSettings settings;

        /// <summary>
        /// </summary>
        private readonly IntPtr[] updatedStrings = new IntPtr[Enum.GetNames(typeof(TaskDialogNativeMethods.TaskDialogElement)).Length];

        /// <summary>
        /// </summary>
        private IntPtr buttonArray;

        /// <summary>
        /// </summary>
        private bool checkBoxChecked;

        /// <summary>
        /// </summary>
        private bool disposed;

        /// <summary>
        /// </summary>
        private bool firstRadioButtonClicked = true;

        /// <summary>
        /// </summary>
        private IntPtr hWndDialog;

        /// <summary>
        /// </summary>
        private IntPtr radioButtonArray;

        // Configuration is applied at dialog creation time.

        /// <summary>
        /// </summary>
        private int selectedButtonID;

        /// <summary>
        /// </summary>
        private int selectedRadioButtonID;

        /// <summary>
        /// </summary>
        private DialogShowState showState = DialogShowState.PreShow;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="settings">
        /// </param>
        /// <param name="outerDialog">
        /// </param>
        internal NativeTaskDialog(NativeTaskDialogSettings settings, TaskDialog outerDialog)
        {
            this.nativeDialogConfig = settings.NativeConfiguration;
            this.settings = settings;

            // Wireup dialog proc message loop for this instance.
            this.nativeDialogConfig.pfCallback = new TaskDialogNativeMethods.TaskDialogCallBack(this.DialogProc);

            // Keep a reference to the outer shell, so we can notify.
            this.outerDialog = outerDialog;
        }

        /// <summary>
        /// </summary>
        ~NativeTaskDialog()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public DialogShowState ShowState
        {
            get
            {
                return this.showState;
            }
        }

        /// <summary>
        /// </summary>
        internal bool CheckBoxChecked
        {
            get
            {
                return this.checkBoxChecked;
            }
        }

        /// <summary>
        /// </summary>
        internal int SelectedButtonID
        {
            get
            {
                return this.selectedButtonID;
            }
        }

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
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

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        internal void AssertCurrentlyShowing()
        {
            Debug.Assert(this.showState == DialogShowState.Showing, "Update*() methods should only be called while native dialog is showing");
        }

        // The new task dialog does not support the existing 
        // Win32 functions for closing (e.g. EndDialog()); instead,
        // a "click button" message is sent. In this case, we're 
        // abstracting out to say that the TaskDialog consumer can
        // simply call "Close" and we'll "click" the cancel button. 
        // Note that the cancel button doesn't actually
        // have to exist for this to work.
        /// <summary>
        /// </summary>
        /// <param name="result">
        /// </param>
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
                    id = DialogsDefaults.MinimumDialogControlId; // custom buttons
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

        /// <summary>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="Win32Exception">
        /// </exception>
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
                var hresult = TaskDialogNativeMethods.TaskDialogIndirect(
                    this.nativeDialogConfig, out this.selectedButtonID, out this.selectedRadioButtonID, out this.checkBoxChecked);

                if (CoreErrorHelper.Failed(hresult))
                {
                    string msg;
                    switch (hresult)
                    {
                        case HRESULT.InvalidArg:
                            msg = "Invalid arguments to Win32 call.";
                            break;
                        case HRESULT.OutofMemory:
                            msg = "Dialog contents too complex.";
                            break;
                        default:
                            msg = String.Format(CultureInfo.CurrentCulture, "An unexpected internal error occurred in the Win32 call:{0:x}", hresult);
                            break;
                    }

                    var e = Marshal.GetExceptionForHR((int)hresult);
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

        /// <summary>
        /// Updates the button enabled.
        /// </summary>
        /// <param name="buttonID">
        /// The button ID.
        /// </param>
        /// <param name="enabled">
        /// if set to <see langword="true"/> [enabled].
        /// </param>
        internal void UpdateButtonEnabled(int buttonID, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.EnableButton, buttonID, enabled ? 1 : 0);
        }

        /// <summary>
        /// Updates the check box checked.
        /// </summary>
        /// <param name="cbc">
        /// if set to <see langword="true"/> [CBC].
        /// </param>
        internal void UpdateCheckBoxChecked(bool cbc)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.ClickVerification, cbc ? 1 : 0, 1);
        }

        /// <summary>
        /// Updates the elevation icon.
        /// </summary>
        /// <param name="buttonId">
        /// The button id.
        /// </param>
        /// <param name="showIcon">
        /// if set to <see langword="true"/> [show icon].
        /// </param>
        internal void UpdateElevationIcon(int buttonId, bool showIcon)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetButtonElevationRequiredState, buttonId, Convert.ToInt32(showIcon));
        }

        /// <summary>
        /// Updates the expanded text.
        /// </summary>
        /// <param name="expandedText">
        /// The expanded text.
        /// </param>
        internal void UpdateExpandedText(string expandedText)
        {
            this.UpdateTextCore(expandedText, TaskDialogNativeMethods.TaskDialogElement.ExpandedInformation);
        }

        /// <summary>
        /// Updates the footer icon.
        /// </summary>
        /// <param name="footerIcon">
        /// The footer icon.
        /// </param>
        internal void UpdateFooterIcon(TaskDialogStandardIcon footerIcon)
        {
            this.UpdateIconCore(footerIcon, TaskDialogNativeMethods.TaskDialogIconElement.IconFooter);
        }

        /// <summary>
        /// Updates the footer text.
        /// </summary>
        /// <param name="footerText">
        /// The footer text.
        /// </param>
        internal void UpdateFooterText(string footerText)
        {
            this.UpdateTextCore(footerText, TaskDialogNativeMethods.TaskDialogElement.Footer);
        }

        /// <summary>
        /// Updates the instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        internal void UpdateInstruction(string instruction)
        {
            this.UpdateTextCore(instruction, TaskDialogNativeMethods.TaskDialogElement.MainInstruction);
        }

        /// <summary>
        /// Updates the main icon.
        /// </summary>
        /// <param name="mainIcon">
        /// The main icon.
        /// </param>
        internal void UpdateMainIcon(TaskDialogStandardIcon mainIcon)
        {
            this.UpdateIconCore(mainIcon, TaskDialogNativeMethods.TaskDialogIconElement.IconMain);
        }

        /// <summary>
        /// Updates the progress bar range.
        /// </summary>
        internal void UpdateProgressBarRange()
        {
            this.AssertCurrentlyShowing();

            // Build range LPARAM - note it is in REVERSE intuitive order.
            var range = MakeLongLParam(this.settings.ProgressBarMaximum, this.settings.ProgressBarMinimum);

            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetProgressBarRange, 0, range);
        }

        /// <summary>
        /// Updates the state of the progress bar.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        internal void UpdateProgressBarState(TaskDialogProgressBarState state)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetProgressBarState, (int)state, 0);
        }

        /// <summary>
        /// Updates the progress bar value.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        internal void UpdateProgressBarValue(int i)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetProgressBarPos, i, 0);
        }

        /// <summary>
        /// Updates the radio button enabled.
        /// </summary>
        /// <param name="buttonID">
        /// The button ID.
        /// </param>
        /// <param name="enabled">
        /// if set to <see langword="true"/> [enabled].
        /// </param>
        internal void UpdateRadioButtonEnabled(int buttonID, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.EnableRadioButton, buttonID, enabled ? 1 : 0);
        }

        /// <summary>
        /// Updates the text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        internal void UpdateText(string text)
        {
            this.UpdateTextCore(text, TaskDialogNativeMethods.TaskDialogElement.Content);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
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
            // be cleaned up automagically by the default 
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

            if (disposing)
            {
                // Clean up managed resources - currently there are none
                // that are interesting.
                if (this.outerDialog != null)
                {
                    this.outerDialog.Dispose();
                }

                if (this.nativeDialogConfig != null)
                {
                    this.nativeDialogConfig.Dispose();
                }
            }
        }

        /// <summary>
        /// Allocates the and marshal buttons.
        /// </summary>
        /// <param name="structs">
        /// The structs.
        /// </param>
        /// <returns>
        /// </returns>
        private static IntPtr AllocateAndMarshalButtons(ICollection<TaskDialogNativeMethods.TaskDialogButton> structs)
        {
            var initialPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TaskDialogNativeMethods.TaskDialogButton)) * structs.Count);

            var currentPtr = initialPtr;
            foreach (var button in structs)
            {
                Marshal.StructureToPtr(button, currentPtr, false);
                currentPtr = (IntPtr)((int)currentPtr + Marshal.SizeOf(button));
            }

            return initialPtr;
        }

        /// <summary>
        /// Makes the long L param.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private static long MakeLongLParam(int a, int b)
        {
            return (a << 16) + b;
        }

        /// <summary>
        /// Dialogs the proc.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="lpRefData">The lp ref data.</param>
        /// <returns></returns>
        private int DialogProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData)
        {
            // Fetch the HWND - it may be the first time we're getting it.
            this.hWndDialog = hwnd;

            // Big switch on the various notifications the 
            // dialog proc can get.
            switch ((TaskDialogNativeMethods.TaskDialogNotification)msg)
            {
                case TaskDialogNativeMethods.TaskDialogNotification.Created:
                    var result = this.PerformDialogInitialization();
                    this.outerDialog.RaiseOpenedEvent();
                    return result;
                case TaskDialogNativeMethods.TaskDialogNotification.ButtonClicked:
                    return this.HandleButtonClick((int)wParam);
                case TaskDialogNativeMethods.TaskDialogNotification.RadioButtonClicked:
                    return this.HandleRadioButtonClick((int)wParam);
                case TaskDialogNativeMethods.TaskDialogNotification.HyperlinkClicked:
                    return this.HandleHyperlinkClick(lParam);
                case TaskDialogNativeMethods.TaskDialogNotification.Help:
                    return this.HandleHelpInvocation();
                case TaskDialogNativeMethods.TaskDialogNotification.Timer:
                    return this.HandleTick((int)wParam);
                case TaskDialogNativeMethods.TaskDialogNotification.Destroyed:
                    return this.PerformDialogCleanup();
                default:
                    break;
            }

            return (int)HRESULT.OK;
        }

        /// <summary>
        /// Frees the old string.
        /// </summary>
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

        /// <summary>
        /// Handles the button click.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
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

        /// <summary>
        /// Handles the help invocation.
        /// </summary>
        /// <returns>
        /// </returns>
        private int HandleHelpInvocation()
        {
            this.outerDialog.RaiseHelpInvokedEvent();
            return CoreErrorHelper.Ignored;
        }

        /// <summary>
        /// Handles the hyperlink click.
        /// </summary>
        /// <param name="pszHref">
        /// The PSZ href.
        /// </param>
        /// <returns>
        /// </returns>
        private int HandleHyperlinkClick(IntPtr pszHref)
        {
            var link = Marshal.PtrToStringUni(pszHref);
            this.outerDialog.RaiseHyperlinkClickEvent(link);

            return CoreErrorHelper.Ignored;
        }

        /// <summary>
        /// Handles the radio button click.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
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
            return CoreErrorHelper.Ignored;
        }

        /// <summary>
        /// Handles the tick.
        /// </summary>
        /// <param name="ticks">
        /// The ticks.
        /// </param>
        /// <returns>
        /// </returns>
        private int HandleTick(int ticks)
        {
            this.outerDialog.RaiseTickEvent(ticks);
            return CoreErrorHelper.Ignored;
        }

        /// <summary>
        /// Determines whether [is option set] [the specified flag].
        /// </summary>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if [is option set] [the specified flag]; otherwise, <see langword="false"/>.
        /// </returns>
        private bool IsOptionSet(TaskDialogNativeMethods.TaskDialogFlags flag)
        {
            return (this.nativeDialogConfig.dwFlags & flag) == flag;
        }

        // Allocates a new string on the unmanaged heap, 
        // and stores the pointer so we can free it later.

        /// <summary>
        /// Makes the new string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Marshals the dialog control structs.
        /// </summary>
        private void MarshalDialogControlStructs()
        {
            if (this.settings.GetButtons() != null && this.settings.GetButtons().Length > 0)
            {
                this.buttonArray = AllocateAndMarshalButtons(this.settings.GetButtons());
                this.settings.NativeConfiguration.pButtons = this.buttonArray;
                this.settings.NativeConfiguration.cButtons = (uint)this.settings.GetButtons().Length;
            }

            if (this.settings.GetRadioButtons() == null || this.settings.GetRadioButtons().Length <= 0)
            {
                return;
            }

            this.radioButtonArray = AllocateAndMarshalButtons(this.settings.GetRadioButtons());
            this.settings.NativeConfiguration.pRadioButtons = this.radioButtonArray;
            this.settings.NativeConfiguration.cRadioButtons = (uint)this.settings.GetRadioButtons().Length;
        }

        /// <summary>
        /// Performs the dialog cleanup.
        /// </summary>
        /// <returns>
        /// </returns>
        private int PerformDialogCleanup()
        {
            this.firstRadioButtonClicked = true;

            return CoreErrorHelper.Ignored;
        }

        /// <summary>
        /// Performs the dialog initialization.
        /// </summary>
        /// <returns>
        /// </returns>
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

            return CoreErrorHelper.Ignored;
        }

        /// <summary>
        /// Sends the message helper.
        /// </summary>
        /// <param name="msg">
        /// The MSG.
        /// </param>
        /// <param name="wParam">
        /// The w param.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// </returns>
        private int SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage msg, int wParam, long lParam)
        {
            // Be sure to at least assert here - 
            // messages to invalid handles often just disappear silently
            Debug.Assert(this.hWndDialog != null, "HWND for dialog is null during SendMessage");

            return (int)CoreNativeMethods.SendMessage(this.hWndDialog, (uint)msg, (IntPtr)wParam, new IntPtr(lParam));
        }

        /// <summary>
        /// Updates the icon core.
        /// </summary>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>
        private void UpdateIconCore(TaskDialogStandardIcon icon, TaskDialogNativeMethods.TaskDialogIconElement element)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.UpdateIcon, (int)element, (long)icon);
        }

        /// <summary>
        /// Updates the text core.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>
        private void UpdateTextCore(string s, TaskDialogNativeMethods.TaskDialogElement element)
        {
            this.AssertCurrentlyShowing();

            this.FreeOldString(element);
            this.SendMessageHelper(TaskDialogNativeMethods.TaskDialogMessage.SetElementText, (int)element, (long)this.MakeNewString(s, element));
        }

        #endregion
    }
}