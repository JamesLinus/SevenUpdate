// ***********************************************************************
// <copyright file="TaskDialog.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Interop;

    using SevenSoftware.Windows.Internal;
    using SevenSoftware.Windows.Properties;

    /// <summary>
    ///   Encapsulates a new-to-Vista Win32 TaskDialog window - a powerful successor to the MessageBox available in
    ///   previous versions of Windows.
    /// </summary>
    public sealed class TaskDialog : IDialogControlHost, IDisposable
    {
        /// <summary>
        ///   Global instance of TaskDialog, to be used by static Show() method. As most parameters of a dialog created
        ///   via static Show() will have identical parameters, we'll create one TaskDialog and treat it as a
        ///   NativeTaskDialog generator for all static Show() calls.
        /// </summary>
        private static TaskDialog staticDialog;

        /// <summary>The collection of buttons displayed on the TaskDialog</summary>
        private List<TaskDialogButtonBase> buttons = new List<TaskDialogButtonBase>();

        /// <summary>Indicates whether the dialog can be canceled</summary>
        private bool canCancel;

        /// <summary>The caption for the dialog</summary>
        private string caption;

        /// <summary>The text to display for the checkbox</summary>
        private string checkBoxText;

        /// <summary>The collection of commandlinks to show on the dialog</summary>
        private List<TaskDialogButtonBase> commandLinks = new List<TaskDialogButtonBase>();

        /// <summary>The collection of controls to show on the dialog</summary>
        private DialogControlCollection<TaskDialogControl> controls;

        /// <summary>The text for the collapsed details</summary>
        private string detailsCollapsedLabel;

        /// <summary>The text when the details are expanded.</summary>
        private bool detailsExpanded;

        /// <summary>The label for the details expander control.</summary>
        private string detailsExpandedLabel;

        /// <summary>The text to show when the details section is expanded.</summary>
        private string detailsExpandedText;

        /// <summary>Indicates if the object is disposed.</summary>
        private bool disposed;

        /// <summary>The expansion mode for the details</summary>
        private TaskDialogExpandedDetailsLocation expansionMode;

        /// <summary>Indicates of the footer checkbox is checked</summary>
        private bool? footerCheckBoxChecked;

        /// <summary>The icon to display on the footer</summary>
        private TaskDialogStandardIcon footerIcon;

        /// <summary>The text to display on the footer</summary>
        private string footerText;

        /// <summary>Indicates a value indicating whether hyperlinks are enabled for urls</summary>
        private bool hyperlinksEnabled;

        /// <summary>The icon to display on the dialog</summary>
        private TaskDialogStandardIcon icon;

        /// <summary>The instruction text to display</summary>
        private string instructionText;

        /// <summary>The native dialog object</summary>
        private NativeTaskDialog nativeDialog;

        /// <summary>The window that owns this item.</summary>
        private IntPtr ownerWindow;

        /// <summary>The progress bar object for the dialog</summary>
        private TaskDialogProgressBar progressBar;

        /// <summary>The collection of radio buttons displayed on the dialog</summary>
        private List<TaskDialogButtonBase> radioButtons = new List<TaskDialogButtonBase>();

        /// <summary>The collection of standard buttons displayed on the task dialog</summary>
        private TaskDialogStandardButtons standardButtons = TaskDialogStandardButtons.None;

        /// <summary>The location where the task dialog should be shown.</summary>
        private TaskDialogStartupLocation startupLocation;

        /// <summary>The main text to display on the dialog.</summary>
        private string text;

        /// <summary>Initializes a new instance of the <see cref="TaskDialog" /> class. Creates a basic TaskDialog window</summary>
        public TaskDialog()
        {
            // Initialize various data structs.
            this.controls = new DialogControlCollection<TaskDialogControl>(this);
        }

        /// <summary>Finalizes an instance of the <see cref="TaskDialog" /> class. TaskDialog Finalizer</summary>
        ~TaskDialog()
        {
            this.Dispose(false);
        }

        /// <summary>Occurs when the TaskDialog is closing.</summary>
        public event EventHandler<TaskDialogClosingEventArgs> Closing;

        /// <summary>Occurs when a user clicks on Help.</summary>
        public event EventHandler HelpInvoked;

        /// <summary>Occurs when a user clicks a hyperlink.</summary>
        public event EventHandler<TaskDialogHyperlinkClickedEventArgs> HyperlinkClick;

        /// <summary>Occurs when the TaskDialog is opened.</summary>
        public event EventHandler Opened;

        /// <summary>Occurs when a progress bar changes.</summary>
        public event EventHandler<TaskDialogTickEventArgs> Tick;

        /// <summary>Gets a value indicating whether this feature is supported on the current platform.</summary>
        public static bool IsPlatformSupported
        {
            get
            {
                // We need Windows Vista onwards ...
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        /// <summary>Gets or sets a value indicating whether CanCancel is set.</summary>
        public bool CanCancel
        {
            get
            {
                return this.canCancel;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.CancelableCannotBeChanged);
                this.canCancel = value;
            }
        }

        /// <summary>Gets or sets a value that contains the caption text.</summary>
        public string Caption
        {
            get
            {
                return this.caption;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.CaptionCannotBeChanged);
                this.caption = value;
            }
        }

        /// <summary>Gets a value that contains the TaskDialog controls.</summary>
        public DialogControlCollection<TaskDialogControl> Controls
        {
            // "Show protection" provided by collection itself, as well as individual controls.
            get
            {
                return this.controls;
            }
        }

        /// <summary>Gets or sets a value that contains the collapsed control text.</summary>
        public string DetailsCollapsedLabel
        {
            get
            {
                return this.detailsCollapsedLabel;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.CollapsedTextCannotBeChanged);
                this.detailsCollapsedLabel = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether the details section is expanded.</summary>
        public bool DetailsExpanded
        {
            get
            {
                return this.detailsExpanded;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.ExpandingStateCannotBeChanged);
                this.detailsExpanded = value;
            }
        }

        /// <summary>Gets or sets a value that contains the expanded control text.</summary>
        public string DetailsExpandedLabel
        {
            get
            {
                return this.detailsExpandedLabel;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.ExpandedLabelCannotBeChanged);
                this.detailsExpandedLabel = value;
            }
        }

        /// <summary>Gets or sets a value that contains the expanded text in the details section.</summary>
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

        /// <summary>Gets or sets a value that contains the expansion mode for this dialog.</summary>
        public TaskDialogExpandedDetailsLocation ExpansionMode
        {
            get
            {
                return this.expansionMode;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.ExpandedDetailsCannotBeChanged);
                this.expansionMode = value;
            }
        }

        /// <summary>Gets or sets a value that indicates if the footer checkbox is checked.</summary>
        public bool? FooterCheckBoxChecked
        {
            get
            {
                return this.footerCheckBoxChecked.GetValueOrDefault(false);
            }

            set
            {
                // Set local value, then update native dialog if showing.
                this.footerCheckBoxChecked = value;
                if (!this.NativeDialogShowing)
                {
                    return;
                }

                bool? checkBoxChecked = this.footerCheckBoxChecked;
                this.nativeDialog.UpdateCheckBoxChecked(checkBoxChecked != null && checkBoxChecked.Value);
            }
        }

        /// <summary>Gets or sets a value that contains the footer check box text.</summary>
        public string FooterCheckBoxText
        {
            get
            {
                return this.checkBoxText;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.CheckBoxCannotBeChanged);
                this.checkBoxText = value;
            }
        }

        /// <summary>Gets or sets a value that contains the footer icon.</summary>
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

        /// <summary>Gets or sets a value that contains the footer text.</summary>
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

        /// <summary>Gets or sets a value indicating whether hyperlinks are enabled.</summary>
        public bool HyperlinksEnabled
        {
            get
            {
                return this.hyperlinksEnabled;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.HyperlinksCannotBetSet);
                this.hyperlinksEnabled = value;
            }
        }

        /// <summary>Gets or sets a value that contains the TaskDialog main icon.</summary>
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

        /// <summary>Gets or sets a value that contains the instruction text.</summary>
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

        /// <summary>Gets or sets a value that contains the owner window's handle.</summary>
        public IntPtr OwnerWindowHandle
        {
            get
            {
                return this.ownerWindow;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.OwnerCannotBeChanged);
                this.ownerWindow = value;
            }
        }

        /// <summary>
        ///   Gets or sets the progress bar on the taskdialog. ProgressBar a visual representation of the progress of a
        ///   long running operation.
        /// </summary>
        public TaskDialogProgressBar ProgressBar
        {
            get
            {
                return this.progressBar;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.ProgressBarCannotBeChanged);
                if (value != null)
                {
                    if (value.HostingDialog != null)
                    {
                        throw new InvalidOperationException(Resources.ProgressBarCannotBeHostedInMultipleDialogs);
                    }

                    value.HostingDialog = this;
                }

                this.progressBar = value;
            }
        }

        /// <summary>Gets or sets a value that contains the standard buttons.</summary>
        public TaskDialogStandardButtons StandardButtons
        {
            get
            {
                return this.standardButtons;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.StandardButtonsCannotBeChanged);
                this.standardButtons = value;
            }
        }

        /// <summary>Gets or sets a value that contains the startup location.</summary>
        public TaskDialogStartupLocation StartupLocation
        {
            get
            {
                return this.startupLocation;
            }

            set
            {
                this.ThrowIfDialogShowing(Resources.StartupLocationCannotBeChanged);
                this.startupLocation = value;
            }
        }

        /// <summary>Gets or sets a value that contains the message text.</summary>
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

        /// <summary>Gets a value indicating whether a native dialog is showing.</summary>
        /// <value><c>True</c> if a native dialog is showing; otherwise, <c>False</c>.</value>
        private bool NativeDialogShowing
        {
            get
            {
                return (this.nativeDialog != null)
                       &&
                       (this.nativeDialog.ShowState == DialogShowState.Showing
                        || this.nativeDialog.ShowState == DialogShowState.Closing);
            }
        }

        /// <summary>Creates and shows a task dialog with the specified message text.</summary>
        /// <param name="text">The text to display.</param>
        /// <returns>The dialog result.</returns>
        public static TaskDialogResult Show(string text)
        {
            return ShowCoreStatic(text, TaskDialogDefaults.MainInstruction, TaskDialogDefaults.Caption);
        }

        /// <summary>Creates and shows a task dialog with the specified supporting text and main instruction.</summary>
        /// <param name="text">The supporting text to display.</param>
        /// <param name="instructionText">The main instruction text to display.</param>
        /// <returns>The dialog result.</returns>
        public static TaskDialogResult Show(string text, string instructionText)
        {
            return ShowCoreStatic(text, instructionText, TaskDialogDefaults.Caption);
        }

        /// <summary>Creates and shows a task dialog with the specified supporting text, main instruction, and dialog caption.</summary>
        /// <param name="text">The supporting text to display.</param>
        /// <param name="instructionText">The main instruction text to display.</param>
        /// <param name="caption">The caption for the dialog.</param>
        /// <returns>The dialog result.</returns>
        public static TaskDialogResult Show(string text, string instructionText, string caption)
        {
            return ShowCoreStatic(text, instructionText, caption);
        }

        /// <summary>Close TaskDialog</summary>
        /// <exception cref="InvalidOperationException">if TaskDialog is not showing.</exception>
        public void Close()
        {
            if (!this.NativeDialogShowing)
            {
                throw new InvalidOperationException(Resources.TaskDialogCloseNonShowing);
            }

            this.nativeDialog.NativeClose(TaskDialogResult.Cancel);

            // TaskDialog's own cleanup code - which runs post show - will handle disposal of native dialog.
        }

        /// <summary>Close TaskDialog with a given TaskDialogResult</summary>
        /// <param name="closingResult">TaskDialogResult to return from the TaskDialog.Show() method</param>
        /// <exception cref="InvalidOperationException">if TaskDialog is not showing.</exception>
        public void Close(TaskDialogResult closingResult)
        {
            if (!this.NativeDialogShowing)
            {
                throw new InvalidOperationException(Resources.TaskDialogCloseNonShowing);
            }

            this.nativeDialog.NativeClose(closingResult);

            // TaskDialog's own cleanup code - which runs post show - will handle disposal of native dialog.
        }

        /// <summary>Dispose TaskDialog Resources</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Dispose TaskDialog Resources</summary>
        /// <param name="disposing">If true, indicates that this is being called via Dispose rather than via the finalizer.</param>
        public void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
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

                // Clean up unmanaged resources SECOND, NTD counts on being closed before being disposed.
                if (this.nativeDialog != null)
                {
                    this.nativeDialog.Dispose();
                    this.nativeDialog = null;
                }

                if (staticDialog != null)
                {
                    staticDialog.Dispose();
                    staticDialog = null;
                }
            }
        }

        /// <summary>Creates and shows a task dialog.</summary>
        /// <returns>The dialog result.</returns>
        public TaskDialogResult Show()
        {
            return this.ShowCore();
        }

        /// <summary>Creates and shows a modal task dialog.</summary>
        /// <param name="window">The window.</param>
        /// <returns>The dialog result.</returns>
        public TaskDialogResult ShowDialog(Window window)
        {
            this.OwnerWindowHandle = new WindowInteropHelper(window).Handle;
            return this.ShowCore();
        }

        /// <summary>Called whenever controls have been added or removed</summary>
        void IDialogControlHost.ApplyCollectionChanged()
        {
            // If we're showing, we should never get here - the changing notification would have thrown and the property
            // would not have been changed.
            Debug.Assert(
                    !this.NativeDialogShowing, "Collection changed notification received despite show state of dialog");
        }

        /// <summary>
        ///   Called when a control currently in the collection has a property changing - this is basically to screen
        ///   out property changes that cannot occur while the dialog is showing because the Win32 API has no way for us
        ///   to propagate the changes until we re-invoke the Win32 call.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="control">The dialog control that changed.</param>
        void IDialogControlHost.ApplyControlPropertyChange(string propertyName, DialogControl control)
        {
            // We only need to apply changes to the native dialog when it actually exists.
            if (this.NativeDialogShowing)
            {
                TaskDialogButton button;
                TaskDialogRadioButton radioButton;
                if (control is TaskDialogProgressBar)
                {
                    if (!this.progressBar.HasValidValues)
                    {
                        throw new ArgumentException(Resources.TaskDialogProgressBarValueInRange);
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
                else if ((button = control as TaskDialogButton) != null)
                {
                    switch (propertyName)
                    {
                        case "ShowElevationIcon":
                            this.nativeDialog.UpdateElevationIcon(button.Id, button.UseElevationIcon);
                            break;
                        case "Enabled":
                            this.nativeDialog.UpdateButtonEnabled(button.Id, button.Enabled);
                            break;
                        default:
                            Debug.Assert(true, "Unknown property being set");
                            break;
                    }
                }
                else if ((radioButton = control as TaskDialogRadioButton) != null)
                {
                    switch (propertyName)
                    {
                        case "Enabled":
                            this.nativeDialog.UpdateRadioButtonEnabled(radioButton.Id, radioButton.Enabled);
                            break;
                        default:
                            Debug.Assert(true, "Unknown property being set");
                            break;
                    }
                }
                else
                {
                    // Do nothing with property change - note that this shouldn't ever happen, we should have either
                    // thrown on the changing event, or we handle above.
                    Debug.Assert(true, "Control property changed notification not handled properly - being ignored");
                }
            }
        }

        /// <summary>
        ///   Handle notifications of pseudo-controls being added or removed from the collection. PreFilter should throw
        ///   if a control cannot be added/removed in the dialog's current state. PostProcess should pass on changes to
        ///   native control, if appropriate.
        /// </summary>
        /// <returns><c>True</c> if collection change is allowed.</returns>
        bool IDialogControlHost.IsCollectionChangeAllowed()
        {
            // Only allow additions to collection if dialog is NOT showing.
            return !this.NativeDialogShowing;
        }

        /// <summary>
        ///   Handle notifications of individual child pseudo-controls' properties changing. Pre filter should throw if
        ///   the property cannot be set in the dialog's current state. PostProcess should pass on changes to native
        ///   control, if appropriate.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="control">The control <paramref name="propertyName" /> applies to.</param>
        /// <returns><c>True</c> if the property change is allowed.</returns>
        bool IDialogControlHost.IsControlPropertyChangeAllowed(string propertyName, DialogControl control)
        {
            Debug.Assert(
                    control is TaskDialogControl, 
                    "Property changing for a control that is not a TaskDialogControl-derived type");
            Debug.Assert(
                    propertyName != "Name", 
                    "Name changes at any time are not supported - public API should have blocked this");

            bool canChange = false;

            if (!this.NativeDialogShowing)
            {
                // Certain properties can't be changed if the dialog is not showing we need a handle created before we
                // can set these...
                if (propertyName != "Enabled")
                {
                    canChange = true;
                }
            }
            else
            {
                // If the dialog is showing, we can only allow some properties to change.
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

        /// <summary>Raises the button click event.</summary>
        /// <param name="id">The id for the button</param>
        internal void RaiseButtonClickEvent(int id)
        {
            // First check to see if the ID matches a custom button.
            TaskDialogButtonBase button = this.GetButtonForId(id);

            // If a custom button was found, raise the event - if not, it's a standard button, and we don't support
            // custom event handling for the standard buttons
            if (button != null)
            {
                button.RaiseClickEvent();
            }
        }

        /// <summary>
        ///   Raises the closing event. Gives event subscriber a chance to prevent the dialog from closing, based on the
        ///   current state of the application and the button used to commit. Note that we don't have full access at
        ///   this stage to the full dialog state.
        /// </summary>
        /// <param name="id">The id for the <c>TaskDialog</c>.</param>
        /// <returns>An integer.</returns>
        internal int RaiseClosingEvent(int id)
        {
            EventHandler<TaskDialogClosingEventArgs> handler = this.Closing;
            if (handler != null)
            {
                var e = new TaskDialogClosingEventArgs();

                // Try to identify the button - is it a standard one?
                TaskDialogStandardButtons buttonClicked = MapButtonIdToStandardButton(id);

                // If not, it had better be a custom button...
                if (buttonClicked == TaskDialogStandardButtons.None)
                {
                    TaskDialogButtonBase customButton = this.GetButtonForId(id);

                    // ... or we have a problem.
                    if (customButton == null)
                    {
                        throw new InvalidOperationException(Resources.TaskDialogBadButtonId);
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
                    return (int)Result.False;
                }
            }

            // It's okay to let the dialog close.
            return (int)Result.Ok;
        }

        /// <summary>Raises the help invoked event.</summary>
        internal void RaiseHelpInvokedEvent()
        {
            if (this.HelpInvoked != null)
            {
                this.HelpInvoked(this, EventArgs.Empty);
            }
        }

        /// <summary>Raises the hyperlink click event.</summary>
        /// <param name="link">The link from the hyperlink.</param>
        internal void RaiseHyperlinkClickEvent(string link)
        {
            EventHandler<TaskDialogHyperlinkClickedEventArgs> handler = this.HyperlinkClick;
            if (handler != null)
            {
                handler(this, new TaskDialogHyperlinkClickedEventArgs(link));
            }
        }

        /// <summary>Raises the opened event.</summary>
        internal void RaiseOpenedEvent()
        {
            if (this.Opened != null)
            {
                this.Opened(this, EventArgs.Empty);
            }
        }

        /// <summary>Raises the tick event.</summary>
        /// <param name="ticks">The ticks.</param>
        internal void RaiseTickEvent(int ticks)
        {
            if (this.Tick != null)
            {
                this.Tick(this, new TaskDialogTickEventArgs(ticks));
            }
        }

        /// <summary>Applies the elevated icons.</summary>
        /// <param name="settings">The dialog settings.</param>
        /// <param name="controls">The dialog controls.</param>
        private static void ApplyElevatedIcons(
                NativeTaskDialogSettings settings, IEnumerable<TaskDialogButtonBase> controls)
        {
            foreach (TaskDialogButton control in controls)
            {
                if (control.UseElevationIcon)
                {
                    if (settings.ElevatedButtons == null)
                    {
                        settings.ElevatedButtons = new List<int>();
                    }

                    settings.ElevatedButtons.Add(control.Id);
                }
            }
        }

        /// <summary>Builds the button struct array.</summary>
        /// <param name="controls">The dialog buttons</param>
        /// <returns>An array of TaskDialogButtons.</returns>
        private static TaskDialogButtonData[] BuildButtonStructArray(List<TaskDialogButtonBase> controls)
        {
            int totalButtons = controls.Count;
            var buttonStructs = new TaskDialogButtonData[totalButtons];
            for (int i = 0; i < totalButtons; i++)
            {
                TaskDialogButtonBase button = controls[i];
                buttonStructs[i] = new TaskDialogButtonData(button.Id, button.ToString());
            }

            return buttonStructs;
        }

        /// <summary>Constructs the dialog result.</summary>
        /// <param name="native">The native task dialog</param>
        /// <returns>The <c>TaskDialogResults</c>.</returns>
        private static TaskDialogResult ConstructDialogResult(NativeTaskDialog native)
        {
            Debug.Assert(
                    native.ShowState == DialogShowState.Closed, "dialog result being constructed for unshown dialog.");

            TaskDialogResult result;

            TaskDialogStandardButtons standardButton = MapButtonIdToStandardButton(native.SelectedButtonId);

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

        /// <summary>Finds the default button id.</summary>
        /// <param name="controls">The collection of button controls.</param>
        /// <returns>The button id.</returns>
        private static int FindDefaultButtonId(List<TaskDialogButtonBase> controls)
        {
            List<TaskDialogButtonBase> defaults = controls.FindAll(control => control.Default);

            if (defaults.Count == 1)
            {
                return defaults[0].Id;
            }

            if (defaults.Count > 1)
            {
                throw new InvalidOperationException(Resources.TaskDialogOnlyOneDefaultControl);
            }

            return 0;
        }

        /// <summary>Maps the button id to standard button.</summary>
        /// <param name="id">The button id.</param>
        /// <returns>The <c>TaskDialogButton</c>.</returns>
        private static TaskDialogStandardButtons MapButtonIdToStandardButton(int id)
        {
            switch ((TaskDialogCommonButtonReturnIds)id)
            {
                case TaskDialogCommonButtonReturnIds.Ok:
                    return TaskDialogStandardButtons.Ok;
                case TaskDialogCommonButtonReturnIds.Cancel:
                    return TaskDialogStandardButtons.Cancel;
                case TaskDialogCommonButtonReturnIds.Abort:

                    // Included for completeness in API - we can't pass in an Abort standard button.
                    return TaskDialogStandardButtons.None;
                case TaskDialogCommonButtonReturnIds.Retry:
                    return TaskDialogStandardButtons.Retry;
                case TaskDialogCommonButtonReturnIds.Ignore:

                    // Included for completeness in API - we can't pass in an Ignore standard button.
                    return TaskDialogStandardButtons.None;
                case TaskDialogCommonButtonReturnIds.Yes:
                    return TaskDialogStandardButtons.Yes;
                case TaskDialogCommonButtonReturnIds.No:
                    return TaskDialogStandardButtons.No;
                case TaskDialogCommonButtonReturnIds.Close:
                    return TaskDialogStandardButtons.Close;
                default:
                    return TaskDialogStandardButtons.None;
            }
        }

        /// <summary>Shows the <c>TaskDialog</c>.</summary>
        /// <param name="text">The text to show.</param>
        /// <param name="instructionText">The instruction text.</param>
        /// <param name="caption">The caption for the dialog</param>
        /// <returns>The <c>TaskDialogResults</c>.</returns>
        private static TaskDialogResult ShowCoreStatic(string text, string instructionText, string caption)
        {
            // If no instance cached yet, create it.
            if (staticDialog == null)
            {
                // New TaskDialog will automatically pick up defaults when a new config structure is created as part of
                // ShowCore().
                staticDialog = new TaskDialog();
            }

            // Set the few relevant properties, and go with the defaults for the others.
            staticDialog.text = text;
            staticDialog.instructionText = instructionText;
            staticDialog.caption = caption;

            return staticDialog.Show();
        }

        /// <summary>Applies the control configuration.</summary>
        /// <param name="settings">The task dialog settings.</param>
        private void ApplyControlConfiguration(NativeTaskDialogSettings settings)
        {
            // Deal with progress bars/marquees.
            if (this.progressBar != null)
            {
                if (this.progressBar.State == TaskDialogProgressBarState.Marquee)
                {
                    settings.NativeConfiguration.TaskDialogFlags |= TaskDialogOptions.ShowMarqueeProgressBar;
                }
                else
                {
                    settings.NativeConfiguration.TaskDialogFlags |= TaskDialogOptions.ShowProgressBar;
                }
            }

            // Build the native struct arrays that NativeTaskDialog needs - though NTD will handle the heavy lifting
            // marshalling to make sure all the cleanup is centralized there.
            if (this.buttons.Count > 0 || this.commandLinks.Count > 0)
            {
                // These are the actual arrays/lists of the structs that we'll copy to the unmanaged heap.
                List<TaskDialogButtonBase> sourceList = this.buttons.Count > 0 ? this.buttons : this.commandLinks;
                settings.Buttons = BuildButtonStructArray(sourceList);

                // Apply option flag that forces all custom buttons to render as command links.
                if (this.commandLinks.Count > 0)
                {
                    settings.NativeConfiguration.TaskDialogFlags |= TaskDialogOptions.UseCommandLinks;
                }

                // Set default button and add elevation icons to appropriate buttons.
                settings.NativeConfiguration.DefaultButtonIndex = FindDefaultButtonId(sourceList);

                ApplyElevatedIcons(settings, sourceList);
            }

            if (this.radioButtons.Count <= 0)
            {
                return;
            }

            settings.RadioButtons = BuildButtonStructArray(this.radioButtons);

            // Set default radio button - radio buttons don't support.
            int defaultRadioButton = FindDefaultButtonId(this.radioButtons);
            settings.NativeConfiguration.DefaultRadioButtonIndex = defaultRadioButton;

            if (defaultRadioButton == 0)
            {
                settings.NativeConfiguration.TaskDialogFlags |= TaskDialogOptions.NoDefaultRadioButton;
            }
        }

        /// <summary>Applies the core dialog settings.</summary>
        /// <param name="settings">The dialog settings.</param>
        private void ApplyCoreSettings(NativeTaskDialogSettings settings)
        {
            this.ApplyGeneralNativeConfiguration(settings.NativeConfiguration);
            this.ApplyTextConfiguration(settings.NativeConfiguration);
            this.ApplyOptionConfiguration(settings.NativeConfiguration);
            this.ApplyControlConfiguration(settings);
        }

        /// <summary>Applies the core settings.</summary>
        /// <param name="dialogConfig">The settings for the dialog</param>
        private void ApplyGeneralNativeConfiguration(TaskDialogConfiguration dialogConfig)
        {
            // If an owner wasn't specifically specified, we'll use the app's main window.
            if (this.ownerWindow != IntPtr.Zero)
            {
                dialogConfig.ParentHandle = this.ownerWindow;
            }

            // Other miscellaneous sets.
            dialogConfig.MainIcon = new IconUnion((int)this.icon);
            dialogConfig.FooterIcon = new IconUnion((int)this.footerIcon);
            dialogConfig.CommonButtons = (TaskDialogCommonButtons)this.standardButtons;
        }

        /// <summary>Applies the option configuration.</summary>
        /// <param name="dialogConfig">The dialog config.</param>
        private void ApplyOptionConfiguration(TaskDialogConfiguration dialogConfig)
        {
            // Handle options - start with no options set.
            TaskDialogOptions options = TaskDialogOptions.None;
            if (this.canCancel)
            {
                options |= TaskDialogOptions.AllowCancel;
            }

            if (this.footerCheckBoxChecked.HasValue && this.footerCheckBoxChecked.Value)
            {
                options |= TaskDialogOptions.CheckVerificationFlag;
            }

            if (this.hyperlinksEnabled)
            {
                options |= TaskDialogOptions.EnableHyperlinks;
            }

            if (this.detailsExpanded)
            {
                options |= TaskDialogOptions.ExpandedByDefault;
            }

            if (this.Tick != null)
            {
                options |= TaskDialogOptions.UseCallbackTimer;
            }

            if (this.startupLocation == TaskDialogStartupLocation.CenterOwner)
            {
                options |= TaskDialogOptions.PositionRelativeToWindow;
            }

            // Note: no validation required, as we allow this to be set even if there is no expanded information text
            // because that could be added later. Default for Win32 API is to expand into (and after) the content area.
            if (this.expansionMode == TaskDialogExpandedDetailsLocation.ExpandFooter)
            {
                options |= TaskDialogOptions.ExpandFooterArea;
            }

            // Finally, apply options to config.
            dialogConfig.TaskDialogFlags = options;
        }

        /// <summary>Applies the supplemental settings.</summary>
        /// <param name="settings">The dialog settings.</param>
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

        /// <summary>Sets important text properties.</summary>
        /// <param name="dialogConfig">An instance of a <see cref="TaskDialogConfiguration" /> object.</param>
        private void ApplyTextConfiguration(TaskDialogConfiguration dialogConfig)
        {
            // note that nulls or empty strings are fine here.
            dialogConfig.Content = this.text;
            dialogConfig.WindowTitle = this.caption;
            dialogConfig.MainInstruction = this.instructionText;
            dialogConfig.ExpandedInformation = this.detailsExpandedText;
            dialogConfig.ExpandedControlText = this.detailsExpandedLabel;
            dialogConfig.CollapsedControlText = this.detailsCollapsedLabel;
            dialogConfig.FooterText = this.footerText;
            dialogConfig.VerificationText = this.checkBoxText;
        }

        /// <summary>Cleans up data and structs from a single native dialog Show() invocation.</summary>
        private void CleanUp()
        {
            // Reset values that would be considered 'volatile' in a given instance.
            if (this.progressBar != null)
            {
                this.progressBar.Reset();
            }

            // Clean out sorted control lists - though we don't of course clear the main controls collection, so the
            // controls are still around; we'll resort on next show, since the collection may have changed.
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

        /// <summary>Gets the button for id.</summary>
        /// <param name="id">The button id.</param>
        /// <returns>The <c>TaskDialogButton</c>.</returns>
        private TaskDialogButtonBase GetButtonForId(int id)
        {
            return (TaskDialogButtonBase)this.controls.GetControlbyId(id);
        }

        /// <summary>Shows the core dialog</summary>
        /// <returns>Returns the result of the <c>TaskDialog</c>.</returns>
        private TaskDialogResult ShowCore()
        {
            TaskDialogResult result;

            try
            {
                // Populate control lists, based on current contents - note we are somewhat late-bound on our control
                // lists, to support XAML scenarios.
                this.SortDialogControls();

                // First, let's make sure it even makes sense to try a show.
                this.ValidateCurrentDialogSettings();

                // Create settings object for new dialog, based on current state.
                var settings = new NativeTaskDialogSettings();
                this.ApplyCoreSettings(settings);
                this.ApplySupplementalSettings(settings);

                // Show the dialog. NOTE: this is a BLOCKING call; the dialog proc callbacks will be executed by the
                // same thread as the Show() call before the thread of execution contines to the end of this method.
                this.nativeDialog = new NativeTaskDialog(settings, this);
                this.nativeDialog.NativeShow();

                // Build and return dialog result to public API - leaving it null after an exception is thrown is fine
                // in this case
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

        /// <summary>Sorts the dialog controls.</summary>
        private void SortDialogControls()
        {
            foreach (var control in this.controls)
            {
                var buttonBase = control as TaskDialogButtonBase;
                var commandLink = control as TaskDialogCommandLink;

                if (buttonBase != null && string.IsNullOrEmpty(buttonBase.Text) && commandLink != null
                    && string.IsNullOrEmpty(commandLink.Instruction))
                {
                    throw new InvalidOperationException(Resources.TaskDialogButtonTextEmpty);
                }

                TaskDialogRadioButton radButton;
                TaskDialogProgressBar progBar;

                // Loop through child controls and sort the controls based on type.
                if (commandLink != null)
                {
                    this.commandLinks.Add(commandLink);
                }
                else if ((radButton = control as TaskDialogRadioButton) != null)
                {
                    if (this.radioButtons == null)
                    {
                        this.radioButtons = new List<TaskDialogButtonBase>();
                    }

                    this.radioButtons.Add(radButton);
                }
                else if (buttonBase != null)
                {
                    if (this.buttons == null)
                    {
                        this.buttons = new List<TaskDialogButtonBase>();
                    }

                    this.buttons.Add(buttonBase);
                }
                else if ((progBar = control as TaskDialogProgressBar) != null)
                {
                    this.progressBar = progBar;
                }
                else
                {
                    throw new InvalidOperationException(Resources.TaskDialogUnkownControl);
                }
            }
        }

        /// <summary>Throws if dialog showing.</summary>
        /// <param name="message">The message to be shown in the exception.</param>
        private void ThrowIfDialogShowing(string message)
        {
            if (this.NativeDialogShowing)
            {
                throw new NotSupportedException(message);
            }
        }

        /// <summary>Validates the current dialog settings.</summary>
        private void ValidateCurrentDialogSettings()
        {
            if (this.footerCheckBoxChecked.HasValue && this.footerCheckBoxChecked.Value
                && string.IsNullOrEmpty(this.checkBoxText))
            {
                throw new InvalidOperationException(Resources.TaskDialogCheckBoxTextRequiredToEnableCheckBox);
            }

            // Progress bar validation. Make sure the progress bar values are valid. the Win32 API will valiantly try to
            // rationalize bizarre min/max/value combinations, but we'll save it the trouble by validating.
            if (this.progressBar != null && !this.progressBar.HasValidValues)
            {
                throw new InvalidOperationException(Resources.TaskDialogProgressBarValueInRange);
            }

            // Validate Buttons collection. Make sure we don't have buttons AND command-links - the Win32 API treats
            // them as different flavors of a single button struct.
            if (this.buttons.Count > 0 && this.commandLinks.Count > 0)
            {
                throw new NotSupportedException(Resources.TaskDialogSupportedButtonsAndLinks);
            }

            // Funny, it works just fine - even MSDN shows examples you can use standard and custom buttons together. So let's remove this artifical limitation.
            // if (buttons.Count > 0 && standardButtons != TaskDialogStandardButtons.None)
            // {
            // throw new
            // NotSupportedException(Properties.Resources.TaskDialogSupportedButtonsAndButtons);
            // }
        }

        // Dispose pattern - cleans up data and structs for a) any native dialog currently showing, and b) anything else
        // that the outer TaskDialog has.
    }
}