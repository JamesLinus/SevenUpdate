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
        private readonly TaskDialogNativeMethods.TASKDIALOGCONFIG nativeDialogConfig;

        /// <summary>
        /// </summary>
        private readonly TaskDialog outerDialog;

        /// <summary>
        /// </summary>
        private readonly NativeTaskDialogSettings settings;

        /// <summary>
        /// </summary>
        private readonly IntPtr[] updatedStrings = new IntPtr[Enum.GetNames(typeof(TaskDialogNativeMethods.TaskdialogElement)).Length];

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
            this.nativeDialogConfig.pfCallback = new TaskDialogNativeMethods.PFTASKDIALOGCALLBACK(this.DialogProc);

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

            var id = (int)TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDCANCEL;

            switch (result)
            {
                case TaskDialogResult.Close:
                    id = (int)TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDCLOSE;
                    break;
                case TaskDialogResult.CustomButtonClicked:
                    id = DialogsDefaults.MinimumDialogControlId; // custom buttons
                    break;
                case TaskDialogResult.No:
                    id = (int)TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDNO;
                    break;
                case TaskDialogResult.Ok:
                    id = (int)TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDOK;
                    break;
                case TaskDialogResult.Retry:
                    id = (int)TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDRETRY;
                    break;
                case TaskDialogResult.Yes:
                    id = (int)TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDYES;
                    break;
            }

            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmClickButton, id, 0);
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
                        case HRESULT.EInvalidarg:
                            msg = "Invalid arguments to Win32 call.";
                            break;
                        case HRESULT.EOutofmemory:
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
        /// </summary>
        /// <param name="buttonID">
        /// </param>
        /// <param name="enabled">
        /// </param>
        internal void UpdateButtonEnabled(int buttonID, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmEnableButton, buttonID, enabled ? 1 : 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="cbc">
        /// </param>
        internal void UpdateCheckBoxChecked(bool cbc)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmClickVerification, cbc ? 1 : 0, 1);
        }

        /// <summary>
        /// </summary>
        /// <param name="buttonId">
        /// </param>
        /// <param name="showIcon">
        /// </param>
        internal void UpdateElevationIcon(int buttonId, bool showIcon)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmSetButtonElevationRequiredState, buttonId, Convert.ToInt32(showIcon));
        }

        /// <summary>
        /// </summary>
        /// <param name="expandedText">
        /// </param>
        internal void UpdateExpandedText(string expandedText)
        {
            this.UpdateTextCore(expandedText, TaskDialogNativeMethods.TaskdialogElement.TdeExpandedInformation);
        }

        /// <summary>
        /// </summary>
        /// <param name="footerIcon">
        /// </param>
        internal void UpdateFooterIcon(TaskDialogStandardIcon footerIcon)
        {
            this.UpdateIconCore(footerIcon, TaskDialogNativeMethods.TaskdialogIconElement.TdieIconFooter);
        }

        /// <summary>
        /// </summary>
        /// <param name="footerText">
        /// </param>
        internal void UpdateFooterText(string footerText)
        {
            this.UpdateTextCore(footerText, TaskDialogNativeMethods.TaskdialogElement.TdeFooter);
        }

        /// <summary>
        /// </summary>
        /// <param name="instruction">
        /// </param>
        internal void UpdateInstruction(string instruction)
        {
            this.UpdateTextCore(instruction, TaskDialogNativeMethods.TaskdialogElement.TdeMainInstruction);
        }

        /// <summary>
        /// </summary>
        /// <param name="mainIcon">
        /// </param>
        internal void UpdateMainIcon(TaskDialogStandardIcon mainIcon)
        {
            this.UpdateIconCore(mainIcon, TaskDialogNativeMethods.TaskdialogIconElement.TdieIconMain);
        }

        /// <summary>
        /// </summary>
        internal void UpdateProgressBarRange()
        {
            this.AssertCurrentlyShowing();

            // Build range LPARAM - note it is in REVERSE intuitive order.
            var range = MakeLongLParam(this.settings.ProgressBarMaximum, this.settings.ProgressBarMinimum);

            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmSetProgressBarRange, 0, range);
        }

        /// <summary>
        /// </summary>
        /// <param name="state">
        /// </param>
        internal void UpdateProgressBarState(TaskDialogProgressBarState state)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmSetProgressBarState, (int)state, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="i">
        /// </param>
        internal void UpdateProgressBarValue(int i)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmSetProgressBarPos, i, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="buttonID">
        /// </param>
        /// <param name="enabled">
        /// </param>
        internal void UpdateRadioButtonEnabled(int buttonID, bool enabled)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmEnableRadioButton, buttonID, enabled ? 1 : 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="text">
        /// </param>
        internal void UpdateText(string text)
        {
            this.UpdateTextCore(text, TaskDialogNativeMethods.TaskdialogElement.TdeContent);
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
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
        /// </summary>
        /// <param name="structs">
        /// </param>
        /// <returns>
        /// </returns>
        private static IntPtr AllocateAndMarshalButtons(ICollection<TaskDialogNativeMethods.TaskdialogButton> structs)
        {
            var initialPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TaskDialogNativeMethods.TaskdialogButton)) * structs.Count);

            var currentPtr = initialPtr;
            foreach (var button in structs)
            {
                Marshal.StructureToPtr(button, currentPtr, false);
                currentPtr = (IntPtr)((int)currentPtr + Marshal.SizeOf(button));
            }

            return initialPtr;
        }

        /// <summary>
        /// </summary>
        /// <param name="a">
        /// </param>
        /// <param name="b">
        /// </param>
        /// <returns>
        /// </returns>
        private static long MakeLongLParam(int a, int b)
        {
            return (a << 16) + b;
        }

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="msg">
        /// </param>
        /// <param name="wParam">
        /// </param>
        /// <param name="lParam">
        /// </param>
        /// <param name="lpRefData">
        /// </param>
        /// <returns>
        /// </returns>
        private int DialogProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData)
        {
            // Fetch the HWND - it may be the first time we're getting it.
            this.hWndDialog = hwnd;

            // Big switch on the various notifications the 
            // dialog proc can get.
            switch ((TaskDialogNativeMethods.TaskdialogNotification)msg)
            {
                case TaskDialogNativeMethods.TaskdialogNotification.TdnCreated:
                    var result = this.PerformDialogInitialization();
                    this.outerDialog.RaiseOpenedEvent();
                    return result;
                case TaskDialogNativeMethods.TaskdialogNotification.TdnButtonClicked:
                    return this.HandleButtonClick((int)wParam);
                case TaskDialogNativeMethods.TaskdialogNotification.TdnRadioButtonClicked:
                    return this.HandleRadioButtonClick((int)wParam);
                case TaskDialogNativeMethods.TaskdialogNotification.TdnHyperlinkClicked:
                    return this.HandleHyperlinkClick(lParam);
                case TaskDialogNativeMethods.TaskdialogNotification.TdnHelp:
                    return this.HandleHelpInvocation();
                case TaskDialogNativeMethods.TaskdialogNotification.TdnTimer:
                    return this.HandleTick((int)wParam);
                case TaskDialogNativeMethods.TaskdialogNotification.TdnDestroyed:
                    return this.PerformDialogCleanup();
                default:
                    break;
            }

            return (int)HRESULT.S_OK;
        }

        /// <summary>
        /// </summary>
        /// <param name="element">
        /// </param>
        private void FreeOldString(TaskDialogNativeMethods.TaskdialogElement element)
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
        /// </summary>
        /// <param name="id">
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
        /// </summary>
        /// <returns>
        /// </returns>
        private int HandleHelpInvocation()
        {
            this.outerDialog.RaiseHelpInvokedEvent();
            return CoreErrorHelper.IGNORED;
        }

        /// <summary>
        /// </summary>
        /// <param name="pszHref">
        /// </param>
        /// <returns>
        /// </returns>
        private int HandleHyperlinkClick(IntPtr pszHref)
        {
            var link = Marshal.PtrToStringUni(pszHref);
            this.outerDialog.RaiseHyperlinkClickEvent(link);

            return CoreErrorHelper.IGNORED;
        }

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <returns>
        /// </returns>
        private int HandleRadioButtonClick(int id)
        {
            // When the dialog sets the radio button to default, 
            // it (somewhat confusingly)issues a radio button clicked event
            // - we mask that out - though ONLY if
            // we do have a default radio button
            if (this.firstRadioButtonClicked && !this.IsOptionSet(TaskDialogNativeMethods.TaskdialogFlagss.TdfnoDefaultRadioButton))
            {
                this.firstRadioButtonClicked = false;
            }
            else
            {
                this.outerDialog.RaiseButtonClickEvent(id);
            }

            // Note: we don't raise Closing, as radio 
            // buttons are non-committing buttons
            return CoreErrorHelper.IGNORED;
        }

        /// <summary>
        /// </summary>
        /// <param name="ticks">
        /// </param>
        /// <returns>
        /// </returns>
        private int HandleTick(int ticks)
        {
            this.outerDialog.RaiseTickEvent(ticks);
            return CoreErrorHelper.IGNORED;
        }

        /// <summary>
        /// </summary>
        /// <param name="flag">
        /// </param>
        /// <returns>
        /// </returns>
        private bool IsOptionSet(TaskDialogNativeMethods.TaskdialogFlagss flag)
        {
            return (this.nativeDialogConfig.dwFlags & flag) == flag;
        }

        // Allocates a new string on the unmanaged heap, 
        // and stores the pointer so we can free it later.

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        /// <param name="element">
        /// </param>
        /// <returns>
        /// </returns>
        private IntPtr MakeNewString(string s, TaskDialogNativeMethods.TaskdialogElement element)
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
        /// </summary>
        /// <returns>
        /// </returns>
        private int PerformDialogCleanup()
        {
            this.firstRadioButtonClicked = true;

            return CoreErrorHelper.IGNORED;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private int PerformDialogInitialization()
        {
            // Initialize Progress or Marquee Bar.
            if (this.IsOptionSet(TaskDialogNativeMethods.TaskdialogFlagss.TdfShowProgressBar))
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
            else if (this.IsOptionSet(TaskDialogNativeMethods.TaskdialogFlagss.TdfShowMarqueeProgressBar))
            {
                // TDM_SET_PROGRESS_BAR_MARQUEE is necessary 
                // to cause the marquee to start animating.
                // Note that this internal task dialog setting is 
                // round-tripped when the marquee is
                // is set to different states, so it never has to 
                // be touched/sent again.
                this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmSetProgressBarMarquee, 1, 0);
                this.UpdateProgressBarState(this.settings.ProgressBarState);
            }

            if (this.settings.ElevatedButtons != null && this.settings.ElevatedButtons.Count > 0)
            {
                foreach (var id in this.settings.ElevatedButtons)
                {
                    this.UpdateElevationIcon(id, true);
                }
            }

            return CoreErrorHelper.IGNORED;
        }

        /// <summary>
        /// </summary>
        /// <param name="msg">
        /// </param>
        /// <param name="wParam">
        /// </param>
        /// <param name="lParam">
        /// </param>
        /// <returns>
        /// </returns>
        private int SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage msg, int wParam, long lParam)
        {
            // Be sure to at least assert here - 
            // messages to invalid handles often just disappear silently
            Debug.Assert(this.hWndDialog != null, "HWND for dialog is null during SendMessage");

            return (int)CoreNativeMethods.SendMessage(this.hWndDialog, (uint)msg, (IntPtr)wParam, new IntPtr(lParam));
        }

        /// <summary>
        /// </summary>
        /// <param name="icon">
        /// </param>
        /// <param name="element">
        /// </param>
        private void UpdateIconCore(TaskDialogStandardIcon icon, TaskDialogNativeMethods.TaskdialogIconElement element)
        {
            this.AssertCurrentlyShowing();
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmUpdateIcon, (int)element, (long)icon);
        }

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        /// <param name="element">
        /// </param>
        private void UpdateTextCore(string s, TaskDialogNativeMethods.TaskdialogElement element)
        {
            this.AssertCurrentlyShowing();

            this.FreeOldString(element);
            this.SendMessageHelper(TaskDialogNativeMethods.TaskdialogMessage.TdmSetElementText, (int)element, (long)this.MakeNewString(s, element));
        }

        #endregion
    }
}