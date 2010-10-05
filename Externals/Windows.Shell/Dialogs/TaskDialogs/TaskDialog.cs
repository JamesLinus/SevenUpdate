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
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Interop;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Encapsulates a new-to-Vista Win32 TaskDialog window 
    ///   - a powerful successor to the MessageBox available
    ///   in previous versions of Windows.
    /// </summary>
    [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public sealed class TaskDialog : IDialogControlHost, IDisposable
    {
        // Global instance of TaskDialog, to be used by static Show() method.
        // As most parameters of a dialog created via static Show() will have
        // identical parameters, we'll create one TaskDialog and treat it
        // as a NativeTaskDialog generator for all static Show() calls.
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private static TaskDialog staticDialog;

        // Main current native dialog.

        /// <summary>
        /// </summary>
        private List<TaskDialogButtonBase> buttons;

        // Main content (maps to MessageBox's "message"). 
        /// <summary>
        /// </summary>
        private bool cancelable;

        /// <summary>
        /// </summary>
        private string caption;

        /// <summary>
        /// </summary>
        private string checkBoxText;

        /// <summary>
        /// </summary>
        private List<TaskDialogButtonBase> commandLinks;

        /// <summary>
        /// </summary>
        private string detailsCollapsedLabel;

        /// <summary>
        /// </summary>
        private bool detailsExpanded;

        /// <summary>
        /// </summary>
        private string detailsExpandedLabel;

        /// <summary>
        /// </summary>
        private string detailsExpandedText;

        /// <summary>
        /// </summary>
        private bool disposed;

        /// <summary>
        /// </summary>
        private TaskDialogExpandedDetailsLocation expansionMode;

        /// <summary>
        /// </summary>
        private bool? footerCheckBoxChecked;

        /// <summary>
        /// </summary>
        private TaskDialogStandardIcon footerIcon;

        /// <summary>
        /// </summary>
        private string footerText;

        /// <summary>
        /// </summary>
        private bool hyperlinksEnabled;

        /// <summary>
        /// </summary>
        private TaskDialogStandardIcon icon;

        /// <summary>
        /// </summary>
        private string instructionText;

        /// <summary>
        /// </summary>
        private NativeTaskDialog nativeDialog;

        /// <summary>
        /// </summary>
        private IntPtr ownerWindow;

        /// <summary>
        /// </summary>
        private TaskDialogProgressBar progressBar;

        /// <summary>
        /// </summary>
        private List<TaskDialogButtonBase> radioButtons;

        /// <summary>
        /// </summary>
        private TaskDialogStandardButtons standardButtons = TaskDialogStandardButtons.None;

        /// <summary>
        /// </summary>
        private TaskDialogStartupLocation startupLocation;

        /// <summary>
        /// </summary>
        private string text;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a basic TaskDialog window
        /// </summary>
        public TaskDialog()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            // Initialize various data structs.
            this.Controls = new DialogControlCollection<TaskDialogControl>(this);
            this.buttons = new List<TaskDialogButtonBase>();
            this.radioButtons = new List<TaskDialogButtonBase>();
            this.commandLinks = new List<TaskDialogButtonBase>();
        }

        /// <summary>
        /// 
        ///   TaskDialog Finalizer
        /// </summary>
        ~TaskDialog()
        {
            this.Dispose(false);
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the TaskDialog is closing.
        /// </summary>
        public event EventHandler<TaskDialogClosingEventArgs> Closing;

        /// <summary>
        ///   Occurs when a user clicks on Help.
        /// </summary>
        public event EventHandler HelpInvoked;

        /// <summary>
        ///   Occurs when a user clicks a hyperlink.
        /// </summary>
        public event EventHandler<TaskDialogHyperlinkClickedEventArgs> HyperlinkClick;

        /// <summary>
        ///   Occurs when the TaskDialog is opened.
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        ///   Occurs when a progress bar changes.
        /// </summary>
        public event EventHandler<TaskDialogTickEventArgs> Tick;

        #endregion

        #region Properties

        /// <summary>
        ///   Indicates whether this feature is supported on the current platform.
        /// </summary>
        public static bool IsPlatformSupported
        {
            get
            {
                // We need Windows Vista onwards ...
                return CoreHelpers.RunningOnVista;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if Cancelable is set.
        /// </summary>
        public bool Cancelable
        {
            get
            {
                return this.cancelable;
            }

            set
            {
                this.ThrowIfDialogShowing("Cancelable can't be set while dialog is showing.");
                this.cancelable = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the caption text.
        /// </summary>
        public string Caption
        {
            get
            {
                return this.caption;
            }

            set
            {
                this.ThrowIfDialogShowing("Dialog caption can't be set while dialog is showing.");
                this.caption = value;
            }
        }

        /// <summary>
        ///   Gets a value that contains the TaskDialog controls.
        /// </summary>
        public DialogControlCollection<TaskDialogControl> Controls { get; private set; }

        /// <summary>
        ///   Gets or sets a value that contains the collapsed control text.
        /// </summary>
        public string DetailsCollapsedLabel
        {
            get
            {
                return this.detailsCollapsedLabel;
            }

            set
            {
                this.ThrowIfDialogShowing("Collapsed control text can't be set while dialog is showing.");
                this.detailsCollapsedLabel = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if the details section is expanded.
        /// </summary>
        public bool DetailsExpanded
        {
            get
            {
                return this.detailsExpanded;
            }

            set
            {
                this.ThrowIfDialogShowing("Expanded state of the dialog can't be modified while dialog is showing.");
                this.detailsExpanded = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the expanded control text.
        /// </summary>
        public string DetailsExpandedLabel
        {
            get
            {
                return this.detailsExpandedLabel;
            }

            set
            {
                this.ThrowIfDialogShowing("Expanded control label can't be set while dialog is showing.");
                this.detailsExpandedLabel = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the expanded text in the details section.
        /// </summary>
        public string DetailsExpandedText
        {
            get
            {
                return this.detailsExpandedText;
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.detailsExpandedText = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.UpdateExpandedText(this.detailsExpandedText);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the expansion mode for this dialog.
        /// </summary>
        public TaskDialogExpandedDetailsLocation ExpansionMode
        {
            get
            {
                return this.expansionMode;
            }

            set
            {
                this.ThrowIfDialogShowing("Expanded information mode can't be set while dialog is showing.");
                this.expansionMode = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates if the footer checkbox is checked.
        /// </summary>
        public bool? FooterCheckBoxChecked
        {
            get
            {
                return !this.footerCheckBoxChecked.HasValue ? false : this.footerCheckBoxChecked;
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.footerCheckBoxChecked = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.UpdateCheckBoxChecked(this.footerCheckBoxChecked.Value);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the footer check box text.
        /// </summary>
        public string FooterCheckBoxText
        {
            get
            {
                return this.checkBoxText;
            }

            set
            {
                this.ThrowIfDialogShowing("Checkbox text can't be set while dialog is showing.");
                this.checkBoxText = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the footer icon.
        /// </summary>
        public TaskDialogStandardIcon FooterIcon
        {
            get
            {
                return this.footerIcon;
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.footerIcon = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.UpdateFooterIcon(this.footerIcon);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the footer text.
        /// </summary>
        public string FooterText
        {
            get
            {
                return this.footerText;
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.footerText = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.UpdateFooterText(this.footerText);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if hyperlinks are enabled.
        /// </summary>
        public bool HyperlinksEnabled
        {
            get
            {
                return this.hyperlinksEnabled;
            }

            set
            {
                this.ThrowIfDialogShowing("Hyperlinks can't be enabled/disabled while dialog is showing.");
                this.hyperlinksEnabled = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the TaskDialog main icon.
        /// </summary>
        public TaskDialogStandardIcon Icon
        {
            get
            {
                return this.icon;
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.icon = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.UpdateMainIcon(this.icon);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the instruction text.
        /// </summary>
        public string InstructionText
        {
            get
            {
                return this.instructionText;
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.instructionText = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.UpdateInstruction(this.instructionText);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the owner window's handle.
        /// </summary>
        public IntPtr OwnerWindowHandle
        {
            get
            {
                return this.ownerWindow;
            }

            set
            {
                this.ThrowIfDialogShowing("Dialog owner cannot be modified while dialog is showing.");
                this.ownerWindow = value;
            }
        }

        /// <summary>
        ///   Gets or sets the progress bar on the taskdialog. ProgressBar a visual representation 
        ///   of the progress of a long running operation.
        /// </summary>
        public TaskDialogProgressBar ProgressBar
        {
            get
            {
                return this.progressBar;
            }

            set
            {
                this.ThrowIfDialogShowing("Progress bar can't be changed while dialog is showing");
                if (value != null)
                {
                    if (value.HostingDialog != null)
                    {
                        throw new InvalidOperationException("Progress bar cannot be hosted in multiple dialogs.");
                    }

                    value.HostingDialog = this;
                }

                this.progressBar = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the standard buttons.
        /// </summary>
        public TaskDialogStandardButtons StandardButtons
        {
            get
            {
                return this.standardButtons;
            }

            set
            {
                this.ThrowIfDialogShowing("Standard buttons can't be set while dialog is showing.");
                this.standardButtons = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the startup location.
        /// </summary>
        public TaskDialogStartupLocation StartupLocation
        {
            get
            {
                return this.startupLocation;
            }

            set
            {
                this.ThrowIfDialogShowing("Startup location can't be changed while dialog is showing.");
                this.startupLocation = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the message text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.text = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.UpdateText(this.text);
                }
            }
        }

        /// <summary>
        /// </summary>
        private bool NativeDialogShowing
        {
            get
            {
                return (this.nativeDialog != null) && (this.nativeDialog.ShowState == DialogShowState.Showing || this.nativeDialog.ShowState == DialogShowState.Closing);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and shows a task dialog with the specified message text.
        /// </summary>
        /// <param name="text">
        /// The text to display.
        /// </param>
        /// <returns>
        /// The dialog result.
        /// </returns>
        public static TaskDialogResult Show(string text)
        {
            return ShowCoreStatic(text, TaskDialogDefaults.MainInstruction, TaskDialogDefaults.Caption);
        }

        /// <summary>
        /// Creates and shows a task dialog with the specified supporting text and main instruction.
        /// </summary>
        /// <param name="text">
        /// The supporting text to display.
        /// </param>
        /// <param name="instructionText">
        /// The main instruction text to display.
        /// </param>
        /// <returns>
        /// The dialog result.
        /// </returns>
        public static TaskDialogResult Show(string text, string instructionText)
        {
            return ShowCoreStatic(text, instructionText, TaskDialogDefaults.Caption);
        }

        /// <summary>
        /// Creates and shows a task dialog with the specified supporting text, main instruction, and dialog caption.
        /// </summary>
        /// <param name="text">
        /// The supporting text to display.
        /// </param>
        /// <param name="instructionText">
        /// The main instruction text to display.
        /// </param>
        /// <param name="caption">
        /// The caption for the dialog.
        /// </param>
        /// <returns>
        /// The dialog result.
        /// </returns>
        public static TaskDialogResult Show(string text, string instructionText, string caption)
        {
            return ShowCoreStatic(text, instructionText, caption);
        }

        /// <summary>
        /// Close TaskDialog
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// if TaskDialog is not showing.
        /// </exception>
        public void Close()
        {
            if (!this.NativeDialogShowing)
            {
                throw new InvalidOperationException("Attempting to close a non-showing dialog.");
            }

            this.nativeDialog.NativeClose(TaskDialogResult.Cancel);

            // TaskDialog's own cleanup code - 
            // which runs post show - will handle disposal of native dialog.
        }

        /// <summary>
        /// Close TaskDialog with a given TaskDialogResult
        /// </summary>
        /// <param name="closingResult">
        /// TaskDialogResult to return from the TaskDialog.Show() method
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// if TaskDialog is not showing.
        /// </exception>
        public void Close(TaskDialogResult closingResult)
        {
            if (!this.NativeDialogShowing)
            {
                throw new InvalidOperationException("Attempting to close a non-showing dialog.");
            }

            this.nativeDialog.NativeClose(closingResult);

            // TaskDialog's own cleanup code - 
            // which runs post show - will handle disposal of native dialog.
        }

        /// <summary>
        /// Dispose TaskDialog Resources
        /// </summary>
        /// <param name="disposing">
        /// If true, indicates that this is being called via Dispose rather than via the finalizer.
        /// </param>
        public void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;

            if (disposing)
            {
                // Clean up managed resources.
                if (this.nativeDialog != null && this.nativeDialog.ShowState == DialogShowState.Showing)
                {
                    this.nativeDialog.NativeClose(TaskDialogResult.Cancel);
                }

                this.buttons = null;
                this.radioButtons = null;
                this.commandLinks = null;
            }

            // Clean up unmanaged resources SECOND, NTD counts on 
            // being closed before being disposed.
            if (this.nativeDialog != null)
            {
                this.nativeDialog.Dispose();
                this.nativeDialog = null;
            }

            if (staticDialog == null)
            {
                return;
            }

            staticDialog.Dispose();
            staticDialog = null;
        }

        /// <summary>
        /// Creates and shows a task dialog.
        /// </summary>
        /// <returns>
        /// The dialog result.
        /// </returns>
        public TaskDialogResult Show()
        {
            return this.ShowCore();
        }

        /// <summary>
        /// Creates and shows a modal task dialog.
        /// </summary>
        /// <param name="window">
        /// </param>
        /// <returns>
        /// The dialog result.
        /// </returns>
        public TaskDialogResult ShowDialog(Window window)
        {
            this.OwnerWindowHandle = new WindowInteropHelper(window).Handle;
            return this.ShowCore();
        }

        #endregion

        // Called whenever controls have been added or removed.
        #region Implemented Interfaces

        #region IDialogControlHost

        /// <summary>
        /// </summary>
        void IDialogControlHost.ApplyCollectionChanged()
        {
            // If we're showing, we should never get here - 
            // the changing notification would have thrown and the 
            // property would not have been changed.
            Debug.Assert(!this.NativeDialogShowing, "Collection changed notification received despite show state of dialog");
        }

        // Called when a control currently in the collection 
        // has a property changing - this is 
        // basically to screen out property changes that 
        // cannot occur while the dialog is showing
        // because the Win32 API has no way for us to 
        // propagate the changes until we re-invoke the Win32 call.

        // Called when a control currently in the collection 
        // has a property changed - this handles propagating
        // the new property values to the Win32 API. 
        // If there isn't a way to change the Win32 value, then we
        // should have already screened out the property set 
        // in NotifyControlPropertyChanging.
        /// <summary>
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        /// <param name="control">
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        void IDialogControlHost.ApplyControlPropertyChange(string propertyName, DialogControl control)
        {
            // We only need to apply changes to the 
            // native dialog when it actually exists.
            if (this.NativeDialogShowing)
            {
                if (control is TaskDialogProgressBar)
                {
                    if (!this.progressBar.HasValidValues)
                    {
                        throw new ArgumentException("Progress bar must have a value between Minimum and Maximum.");
                    }

                    switch (propertyName)
                    {
                        case "State":
                            this.nativeDialog.UpdateProgressBarState(this.progressBar.State);
                            break;
                        case "Value":
                            this.nativeDialog.UpdateProgressBarValue(this.progressBar.Value);
                            break;
                        case "Minimum":
                        case "Maximum":
                            this.nativeDialog.UpdateProgressBarRange();
                            break;
                        default:
                            Debug.Assert(true, "Unknown property being set");
                            break;
                    }
                }
                else if (control is TaskDialogButton)
                {
                    var button = (TaskDialogButton)control;
                    switch (propertyName)
                    {
                        case "ShowElevationIcon":
                            this.nativeDialog.UpdateElevationIcon(button.Id, button.ShowElevationIcon);
                            break;
                        case "Enabled":
                            this.nativeDialog.UpdateButtonEnabled(button.Id, button.Enabled);
                            break;
                        default:
                            Debug.Assert(true, "Unknown property being set");
                            break;
                    }
                }
                else if (control is TaskDialogRadioButton)
                {
                    var button = (TaskDialogRadioButton)control;
                    switch (propertyName)
                    {
                        case "Enabled":
                            this.nativeDialog.UpdateRadioButtonEnabled(button.Id, button.Enabled);
                            break;
                        default:
                            Debug.Assert(true, "Unknown property being set");
                            break;
                    }
                }
                else
                {
                    // Do nothing with property change - 
                    // note that this shouldn't ever happen, we should have
                    // either thrown on the changing event, or we handle above.
                    Debug.Assert(true, "Control property changed notification not handled properly - being ignored");
                }
            }

            return;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        bool IDialogControlHost.IsCollectionChangeAllowed()
        {
            // Only allow additions to collection if dialog is NOT showing.
            return !this.NativeDialogShowing;
        }

        /// <summary>
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        /// <param name="control">
        /// </param>
        /// <returns>
        /// </returns>
        bool IDialogControlHost.IsControlPropertyChangeAllowed(string propertyName, DialogControl control)
        {
            Debug.Assert(control is TaskDialogControl, "Property changing for a control that is not a TaskDialogControl-derived type");
            Debug.Assert(propertyName != "Name", "Name changes at any time are not supported - public API should have blocked this");

            var canChange = false;

            if (!this.NativeDialogShowing)
            {
                // Certain properties can't be changed if the dialog is not showing
                // we need a handle created before we can set these...
                switch (propertyName)
                {
                    case "Enabled":
                        break;
                    default:
                        canChange = true;
                        break;
                }
            }
            else
            {
                // If the dialog is showing, we can only 
                // allow some properties to change.
                switch (propertyName)
                {
                        // Properties that CAN'T be changed while dialog is showing.
                    case "Text":
                    case "Default":
                        break;

                        // Properties that CAN be changed while dialog is showing.
                    case "ShowElevationIcon":
                    case "Enabled":
                        canChange = true;
                        break;
                    default:
                        Debug.Assert(true, "Unknown property name coming through property changing handler");
                        break;
                }
            }

            return canChange;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Dispose TaskDialog Resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        // All Raise*() methods are called by the 
        // NativeTaskDialog when various pseudo-controls
        // are triggered.
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        internal void RaiseButtonClickEvent(int id)
        {
            // First check to see if the ID matches a custom button.
            var button = this.GetButtonForId(id);

            // If a custom button was found, 
            // raise the event - if not, it's a standard button, and
            // we don't support custom event handling for the standard buttons
            if (button != null)
            {
                button.RaiseClickEvent();
            }
        }

        // Gives event subscriber a chance to prevent 
        // the dialog from closing, based on 
        // the current state of the app and the button 
        // used to commit. Note that we don't 
        // have full access at this stage to 
        // the full dialog state.
        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        internal int RaiseClosingEvent(int id)
        {
            var handler = this.Closing;
            if (handler != null)
            {
                TaskDialogButtonBase customButton;
                var e = new TaskDialogClosingEventArgs();

                // Try to identify the button - is it a standard one?
                var buttonClicked = MapButtonIdToStandardButton(id);

                // If not, it had better be a custom button...
                if (buttonClicked == TaskDialogStandardButtons.None)
                {
                    customButton = this.GetButtonForId(id);

                    // ... or we have a problem.
                    if (customButton == null)
                    {
                        throw new InvalidOperationException("Bad button ID in closing event.");
                    }

                    e.CustomButton = customButton.Name;

                    e.TaskDialogResult = TaskDialogResult.CustomButtonClicked;
                }
                else
                {
                    e.TaskDialogResult = (TaskDialogResult)buttonClicked;
                }

                // Raise the event and determine how to proceed.
                handler(this, e);
                if (e.Cancel)
                {
                    return (int)HRESULT.SFalse;
                }
            }

            // It's okay to let the dialog close.
            return (int)HRESULT.S_OK;
        }

        /// <summary>
        /// </summary>
        internal void RaiseHelpInvokedEvent()
        {
            var handler = this.HelpInvoked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="link">
        /// </param>
        internal void RaiseHyperlinkClickEvent(string link)
        {
            var handler = this.HyperlinkClick;
            if (handler != null)
            {
                handler(this, new TaskDialogHyperlinkClickedEventArgs(link));
            }
        }

        /// <summary>
        /// </summary>
        internal void RaiseOpenedEvent()
        {
            var handler = this.Opened;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ticks">
        /// </param>
        internal void RaiseTickEvent(int ticks)
        {
            var handler = this.Tick;
            if (handler != null)
            {
                handler(this, new TaskDialogTickEventArgs(ticks));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="settings">
        /// </param>
        /// <param name="controls">
        /// </param>
        private static void ApplyElevatedIcons(NativeTaskDialogSettings settings, IEnumerable<TaskDialogButtonBase> controls)
        {
            foreach (var control in controls.Cast<TaskDialogButton>().Where(control => control.ShowElevationIcon))
            {
                if (settings.ElevatedButtons == null)
                {
                    settings.ElevatedButtons = new List<int>();
                }

                settings.ElevatedButtons.Add(control.Id);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="controls">
        /// </param>
        /// <returns>
        /// </returns>
        private static TaskDialogNativeMethods.TaskdialogButton[] BuildButtonStructArray(IList<TaskDialogButtonBase> controls)
        {
            TaskDialogButtonBase button;

            var totalButtons = controls.Count;
            var buttonStructs = new TaskDialogNativeMethods.TaskdialogButton[totalButtons];
            for (var i = 0; i < totalButtons; i++)
            {
                button = controls[i];
                buttonStructs[i] = new TaskDialogNativeMethods.TaskdialogButton(button.Id, button.ToString());
            }

            return buttonStructs;
        }

        /// <summary>
        /// </summary>
        /// <param name="native">
        /// </param>
        /// <returns>
        /// </returns>
        private static TaskDialogResult ConstructDialogResult(NativeTaskDialog native)
        {
            Debug.Assert(native.ShowState == DialogShowState.Closed, "dialog result being constructed for unshown dialog.");

            TaskDialogResult result;

            var standardButton = MapButtonIdToStandardButton(native.SelectedButtonID);

            // If returned ID isn't a standard button, let's fetch 
            if (standardButton == TaskDialogStandardButtons.None)
            {
                result = TaskDialogResult.CustomButtonClicked;
            }
            else
            {
                result = (TaskDialogResult)standardButton;
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="controls">
        /// </param>
        /// <returns>
        /// </returns>
        private static int FindDefaultButtonId(IEnumerable<TaskDialogButtonBase> controls)
        {
            return controls.Where(control => control.Default).Select(control => control.Id).FirstOrDefault();
        }

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <returns>
        /// </returns>
        private static TaskDialogStandardButtons MapButtonIdToStandardButton(int id)
        {
            switch ((TaskDialogNativeMethods.TaskdialogCommonButtonReturnID)id)
            {
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDOK:
                    return TaskDialogStandardButtons.Ok;
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDCANCEL:
                    return TaskDialogStandardButtons.Cancel;
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDABORT:

                    // Included for completeness in API - 
                    // we can't pass in an Abort standard button.
                    return TaskDialogStandardButtons.None;
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDRETRY:
                    return TaskDialogStandardButtons.Retry;
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDIGNORE:

                    // Included for completeness in API - 
                    // we can't pass in an Ignore standard button.
                    return TaskDialogStandardButtons.None;
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDYES:
                    return TaskDialogStandardButtons.Yes;
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDNO:
                    return TaskDialogStandardButtons.No;
                case TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDCLOSE:
                    return TaskDialogStandardButtons.Close;
                default:
                    return TaskDialogStandardButtons.None;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <param name="instructionText">
        /// </param>
        /// <param name="caption">
        /// </param>
        /// <returns>
        /// </returns>
        private static TaskDialogResult ShowCoreStatic(string text, string instructionText, string caption)
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            // If no instance cached yet, create it.
            if (staticDialog == null)
            {
                // New TaskDialog will automatically pick up defaults when 
                // a new config structure is created as part of ShowCore().
                staticDialog = new TaskDialog();
            }

            // Set the few relevant properties, 
            // and go with the defaults for the others.
            staticDialog.text = text;
            staticDialog.instructionText = instructionText;
            staticDialog.caption = caption;

            return staticDialog.Show();
        }

        /// <summary>
        /// </summary>
        /// <param name="settings">
        /// </param>
        private void ApplyControlConfiguration(NativeTaskDialogSettings settings)
        {
            // Deal with progress bars/marquees.
            if (this.progressBar != null)
            {
                if (this.progressBar.State == TaskDialogProgressBarState.Marquee)
                {
                    settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TaskdialogFlagss.TdfShowMarqueeProgressBar;
                }
                else
                {
                    settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TaskdialogFlagss.TdfShowProgressBar;
                }
            }

            // Build the native struct arrays that NativeTaskDialog 
            // needs - though NTD will handle
            // the heavy lifting marshalling to make sure 
            // all the cleanup is centralized there.
            if (this.buttons.Count > 0 || this.commandLinks.Count > 0)
            {
                // These are the actual arrays/lists of 
                // the structs that we'll copy to the 
                // unmanaged heap.
                var sourceList = this.buttons.Count > 0 ? this.buttons : this.commandLinks;
                settings.SetButtons(BuildButtonStructArray(sourceList));

                // Apply option flag that forces all 
                // custom buttons to render as command links.
                if (this.commandLinks.Count > 0)
                {
                    settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TaskdialogFlagss.TdfUseCommandLinks;
                }

                // Set default button and add elevation icons 
                // to appropriate buttons.
                settings.NativeConfiguration.nDefaultButton = FindDefaultButtonId(sourceList);

                ApplyElevatedIcons(settings, sourceList);
            }

            if (this.radioButtons.Count <= 0)
            {
                return;
            }

            settings.SetRadioButtons(BuildButtonStructArray(this.radioButtons));

            // Set default radio button - radio buttons don't support.
            var defaultRadioButton = FindDefaultButtonId(this.radioButtons);
            settings.NativeConfiguration.nDefaultRadioButton = defaultRadioButton;

            if (defaultRadioButton == TaskDialogNativeMethods.NODefaultButtonSpecified)
            {
                settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TaskdialogFlagss.TdfnoDefaultRadioButton;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="settings">
        /// </param>
        private void ApplyCoreSettings(NativeTaskDialogSettings settings)
        {
            this.ApplyGeneralNativeConfiguration(settings.NativeConfiguration);
            this.ApplyTextConfiguration(settings.NativeConfiguration);
            this.ApplyOptionConfiguration(settings.NativeConfiguration);
            this.ApplyControlConfiguration(settings);
        }

        /// <summary>
        /// </summary>
        /// <param name="dialogConfig">
        /// </param>
        private void ApplyGeneralNativeConfiguration(TaskDialogNativeMethods.TASKDIALOGCONFIG dialogConfig)
        {
            // If an owner wasn't specifically specified, 
            // we'll use the app's main window.
            if (this.ownerWindow != IntPtr.Zero)
            {
                dialogConfig.hwndParent = this.ownerWindow;
            }

            // Other miscellaneous sets.
            dialogConfig.MainIcon = new TaskDialogNativeMethods.TaskdialogconfigIconUnion((int)this.icon);
            dialogConfig.FooterIcon = new TaskDialogNativeMethods.TaskdialogconfigIconUnion((int)this.footerIcon);
            dialogConfig.dwCommonButtons = (TaskDialogNativeMethods.TaskdialogCommonButtonFlagss)this.standardButtons;
        }

        /// <summary>
        /// </summary>
        /// <param name="dialogConfig">
        /// </param>
        private void ApplyOptionConfiguration(TaskDialogNativeMethods.TASKDIALOGCONFIG dialogConfig)
        {
            // Handle options - start with no options set.
            var options = TaskDialogNativeMethods.TaskdialogFlagss.NONE;
            if (this.cancelable)
            {
                options |= TaskDialogNativeMethods.TaskdialogFlagss.TdfAllowDialogCancellation;
            }

            if (this.footerCheckBoxChecked.HasValue && this.footerCheckBoxChecked.Value)
            {
                options |= TaskDialogNativeMethods.TaskdialogFlagss.TdfVerificationFlagChecked;
            }

            if (this.hyperlinksEnabled)
            {
                options |= TaskDialogNativeMethods.TaskdialogFlagss.TdfEnableHyperlinks;
            }

            if (this.detailsExpanded)
            {
                options |= TaskDialogNativeMethods.TaskdialogFlagss.TdfExpandedBYDefault;
            }

            if (this.Tick != null)
            {
                options |= TaskDialogNativeMethods.TaskdialogFlagss.TdfCallbackTimer;
            }

            if (this.startupLocation == TaskDialogStartupLocation.CenterOwner)
            {
                options |= TaskDialogNativeMethods.TaskdialogFlagss.TdfPositionRelativeTOWindow;
            }

            // Note: no validation required, as we allow this to 
            // be set even if there is no expanded information 
            // text because that could be added later.
            // Default for Win32 API is to expand into (and after) 
            // the content area.
            if (this.expansionMode == TaskDialogExpandedDetailsLocation.ExpandFooter)
            {
                options |= TaskDialogNativeMethods.TaskdialogFlagss.TdfExpandFooterArea;
            }

            // Finally, apply options to config.
            dialogConfig.dwFlags = options;
        }

        /// <summary>
        /// </summary>
        /// <param name="settings">
        /// </param>
        private void ApplySupplementalSettings(NativeTaskDialogSettings settings)
        {
            if (this.progressBar != null)
            {
                if (this.progressBar.State != TaskDialogProgressBarState.Marquee)
                {
                    settings.ProgressBarMinimum = this.progressBar.Minimum;
                    settings.ProgressBarMaximum = this.progressBar.Maximum;
                    settings.ProgressBarValue = this.progressBar.Value;
                    settings.ProgressBarState = this.progressBar.State;
                }
            }

            if (this.HelpInvoked != null)
            {
                settings.InvokeHelp = true;
            }
        }

        /// <summary>
        /// Sets important text properties.
        /// </summary>
        /// <param name="dialogConfig">
        /// An instance of a <see cref="TaskDialogNativeMethods.TASKDIALOGCONFIG"/> object.
        /// </param>
        private void ApplyTextConfiguration(TaskDialogNativeMethods.TASKDIALOGCONFIG dialogConfig)
        {
            // note that nulls or empty strings are fine here.
            dialogConfig.pszContent = this.text;
            dialogConfig.pszWindowTitle = this.caption;
            dialogConfig.pszMainInstruction = this.instructionText;
            dialogConfig.pszExpandedInformation = this.detailsExpandedText;
            dialogConfig.pszExpandedControlText = this.detailsExpandedLabel;
            dialogConfig.pszCollapsedControlText = this.detailsCollapsedLabel;
            dialogConfig.pszFooter = this.footerText;
            dialogConfig.pszVerificationText = this.checkBoxText;
        }

        // Cleans up data and structs from a single 
        // native dialog Show() invocation.

        // Dispose pattern - cleans up data and structs for 
        // a) any native dialog currently showing, and
        // b) anything else that the outer TaskDialog has.

        /// <summary>
        /// </summary>
        private void CleanUp()
        {
            // Reset values that would be considered 
            // 'volatile' in a given instance.
            if (this.progressBar != null)
            {
                this.progressBar.Reset();
            }

            // Clean out sorted control lists - 
            // though we don't of course clear the main controls collection,
            // so the controls are still around; we'll 
            // resort on next show, since the collection may have changed.
            if (this.buttons != null)
            {
                this.buttons.Clear();
            }

            if (this.commandLinks != null)
            {
                this.commandLinks.Clear();
            }

            if (this.radioButtons != null)
            {
                this.radioButtons.Clear();
            }

            this.progressBar = null;

            // Have the native dialog clean up the rest.
            if (this.nativeDialog != null)
            {
                this.nativeDialog.Dispose();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <returns>
        /// </returns>
        private TaskDialogButtonBase GetButtonForId(int id)
        {
            return (TaskDialogButtonBase)this.Controls.GetControlbyId(id);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private TaskDialogResult ShowCore()
        {
            TaskDialogResult result;

            try
            {
                // Populate control lists, based on current 
                // contents - note we are somewhat late-bound 
                // on our control lists, to support XAML scenarios.
                this.SortDialogControls();

                // First, let's make sure it even makes 
                // sense to try a show.
                this.ValidateCurrentDialogSettings();

                // Create settings object for new dialog, 
                // based on current state.
                var settings = new NativeTaskDialogSettings();
                this.ApplyCoreSettings(settings);
                this.ApplySupplementalSettings(settings);

                // Show the dialog.
                // NOTE: this is a BLOCKING call; the dialog proc callbacks
                // will be executed by the same thread as the 
                // Show() call before the thread of execution 
                // contines to the end of this method.
                this.nativeDialog = new NativeTaskDialog(settings, this);
                this.nativeDialog.NativeShow();

                // Build and return dialog result to public API - leaving it
                // null after an exception is thrown is fine in this case
                result = ConstructDialogResult(this.nativeDialog);
                this.footerCheckBoxChecked = this.nativeDialog.CheckBoxChecked;
            }
            finally
            {
                this.CleanUp();
                this.nativeDialog = null;
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        private void SortDialogControls()
        {
            foreach (var control in this.Controls)
            {
                if (control is TaskDialogButtonBase && String.IsNullOrEmpty(((TaskDialogButtonBase)control).Text))
                {
                    if (control is TaskDialogCommandLink && String.IsNullOrEmpty(((TaskDialogCommandLink)control).Instruction))
                    {
                        throw new InvalidOperationException("Button text must be non-empty");
                    }
                }

                // Loop through child controls 
                // and sort the controls based on type.
                if (control is TaskDialogCommandLink)
                {
                    this.commandLinks.Add((TaskDialogCommandLink)control);
                }
                else if (control is TaskDialogRadioButton)
                {
                    if (this.radioButtons == null)
                    {
                        this.radioButtons = new List<TaskDialogButtonBase>();
                    }

                    this.radioButtons.Add((TaskDialogRadioButton)control);
                }
                else if (control is TaskDialogButtonBase)
                {
                    if (this.buttons == null)
                    {
                        this.buttons = new List<TaskDialogButtonBase>();
                    }

                    this.buttons.Add((TaskDialogButtonBase)control);
                }
                else if (control is TaskDialogProgressBar)
                {
                    this.progressBar = (TaskDialogProgressBar)control;
                }
                else
                {
                    throw new ArgumentException("Unknown dialog control type.");
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private void ThrowIfDialogShowing(string message)
        {
            if (this.NativeDialogShowing)
            {
                throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private void ValidateCurrentDialogSettings()
        {
            if (this.footerCheckBoxChecked.HasValue && this.footerCheckBoxChecked.Value && String.IsNullOrEmpty(this.checkBoxText))
            {
                throw new InvalidOperationException("Checkbox text must be provided to enable the dialog checkbox.");
            }

            // Progress bar validation.
            // Make sure the progress bar values are valid.
            // the Win32 API will valiantly try to rationalize 
            // bizarre min/max/value combinations, but we'll save
            // it the trouble by validating.
            if (this.progressBar != null)
            {
                if (!this.progressBar.HasValidValues)
                {
                    throw new ArgumentException("Progress bar must have a value between the minimum and maxium values.");
                }
            }

            // Validate Buttons collection.
            // Make sure we don't have buttons AND 
            // command-links - the Win32 API treats them as different
            // flavors of a single button struct.
            if (this.buttons.Count > 0 && this.commandLinks.Count > 0)
            {
                throw new NotSupportedException("Dialog cannot display both non-standard buttons and command links.");
            }

            // if (buttons.Count > 0 && standardButtons != TaskDialogStandardButtons.None)
            // throw new NotSupportedException("Dialog cannot display both non-standard buttons and standard buttons.");
        }

        #endregion
    }
}