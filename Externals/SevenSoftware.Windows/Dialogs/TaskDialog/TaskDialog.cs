// <copyright file="TaskDialog.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using SevenSoftware.Windows.Internal;
using SevenSoftware.Windows.Properties;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
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
        static TaskDialog staticDialog;

        /// <summary>The collection of buttons displayed on the TaskDialog</summary>
        List<TaskDialogButtonBase> buttons = new List<TaskDialogButtonBase>();

        /// <summary>Indicates whether the dialog can be canceled</summary>
        bool canCancel;

        /// <summary>The caption for the dialog</summary>
        string caption;

        /// <summary>The text to display for the checkbox</summary>
        string checkBoxText;

        /// <summary>The collection of commandlinks to show on the dialog</summary>
        List<TaskDialogButtonBase> commandLinks = new List<TaskDialogButtonBase>();

        /// <summary>The collection of controls to show on the dialog</summary>
        DialogControlCollection<TaskDialogControl> controls;

        /// <summary>The text for the collapsed details</summary>
        string detailsCollapsedLabel;

        /// <summary>The text when the details are expanded.</summary>
        bool detailsExpanded;

        /// <summary>The label for the details expander control.</summary>
        string detailsExpandedLabel;

        /// <summary>The text to show when the details section is expanded.</summary>
        string detailsExpandedText;

        /// <summary>Indicates if the object is disposed.</summary>
        bool disposed;

        /// <summary>The expansion mode for the details</summary>
        TaskDialogExpandedDetailsLocation expansionMode;

        /// <summary>Indicates of the footer checkbox is checked</summary>
        bool? footerCheckBoxChecked;

        /// <summary>The icon to display on the footer</summary>
        TaskDialogStandardIcon footerIcon;

        /// <summary>The text to display on the footer</summary>
        string footerText;

        /// <summary>Indicates a value indicating whether hyperlinks are enabled for urls</summary>
        bool hyperlinksEnabled;

        /// <summary>The icon to display on the dialog</summary>
        TaskDialogStandardIcon icon;

        /// <summary>The instruction text to display</summary>
        string instructionText;

        /// <summary>The native dialog object</summary>
        NativeTaskDialog nativeDialog;

        /// <summary>The window that owns this item.</summary>
        IntPtr ownerWindow;

        /// <summary>The progress bar object for the dialog</summary>
        TaskDialogProgressBar progressBar;

        /// <summary>The collection of radio buttons displayed on the dialog</summary>
        List<TaskDialogButtonBase> radioButtons = new List<TaskDialogButtonBase>();

        /// <summary>The collection of standard buttons displayed on the task dialog</summary>
        TaskDialogStandardButtons standardButtons = TaskDialogStandardButtons.None;

        /// <summary>The location where the task dialog should be shown.</summary>
        TaskDialogStartupLocation startupLocation;

        /// <summary>The main text to display on the dialog.</summary>
        string text;

        /// <summary>Initializes a new instance of the <see cref="TaskDialog" /> class. Creates a basic TaskDialog window</summary>
        public TaskDialog()
        {
            // Initialize various data structs.
            controls = new DialogControlCollection<TaskDialogControl>(this);
        }

        /// <summary>Finalizes an instance of the <see cref="TaskDialog" /> class. TaskDialog Finalizer</summary>
        ~TaskDialog()
        {
            Dispose(false);
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
            get { return canCancel; }

            set
            {
                ThrowIfDialogShowing(Resources.CancelableCannotBeChanged);
                canCancel = value;
            }
        }

        /// <summary>Gets or sets a value that contains the caption text.</summary>
        public string Caption
        {
            get { return caption; }

            set
            {
                ThrowIfDialogShowing(Resources.CaptionCannotBeChanged);
                caption = value;
            }
        }

        /// <summary>Gets a value that contains the TaskDialog controls.</summary>
        public DialogControlCollection<TaskDialogControl> Controls
        {
            // "Show protection" provided by collection itself, as well as individual controls.
            get { return controls; }
        }

        /// <summary>Gets or sets a value that contains the collapsed control text.</summary>
        public string DetailsCollapsedLabel
        {
            get { return detailsCollapsedLabel; }

            set
            {
                ThrowIfDialogShowing(Resources.CollapsedTextCannotBeChanged);
                detailsCollapsedLabel = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether the details section is expanded.</summary>
        public bool DetailsExpanded
        {
            get { return detailsExpanded; }

            set
            {
                ThrowIfDialogShowing(Resources.ExpandingStateCannotBeChanged);
                detailsExpanded = value;
            }
        }

        /// <summary>Gets or sets a value that contains the expanded control text.</summary>
        public string DetailsExpandedLabel
        {
            get { return detailsExpandedLabel; }

            set
            {
                ThrowIfDialogShowing(Resources.ExpandedLabelCannotBeChanged);
                detailsExpandedLabel = value;
            }
        }

        /// <summary>Gets or sets a value that contains the expanded text in the details section.</summary>
        public string DetailsExpandedText
        {
            get { return detailsExpandedText; }

            set
            {
                // Set local value, then update native dialog if showing.
                detailsExpandedText = value;
                if (NativeDialogShowing)
                {
                    nativeDialog.UpdateExpandedText(detailsExpandedText);
                }
            }
        }

        /// <summary>Gets or sets a value that contains the expansion mode for this dialog.</summary>
        public TaskDialogExpandedDetailsLocation ExpansionMode
        {
            get { return expansionMode; }

            set
            {
                ThrowIfDialogShowing(Resources.ExpandedDetailsCannotBeChanged);
                expansionMode = value;
            }
        }

        /// <summary>Gets or sets a value that indicates if the footer checkbox is checked.</summary>
        public bool? FooterCheckBoxChecked
        {
            get { return footerCheckBoxChecked.GetValueOrDefault(false); }

            set
            {
                // Set local value, then update native dialog if showing.
                footerCheckBoxChecked = value;
                if (!NativeDialogShowing)
                {
                    return;
                }

                bool? checkBoxChecked = footerCheckBoxChecked;
                nativeDialog.UpdateCheckBoxChecked(checkBoxChecked != null && checkBoxChecked.Value);
            }
        }

        /// <summary>Gets or sets a value that contains the footer check box text.</summary>
        public string FooterCheckBoxText
        {
            get { return checkBoxText; }

            set
            {
                ThrowIfDialogShowing(Resources.CheckBoxCannotBeChanged);
                checkBoxText = value;
            }
        }

        /// <summary>Gets or sets a value that contains the footer icon.</summary>
        public TaskDialogStandardIcon FooterIcon
        {
            get { return footerIcon; }

            set
            {
                // Set local value, then update native dialog if showing.
                footerIcon = value;
                if (NativeDialogShowing)
                {
                    nativeDialog.UpdateFooterIcon(footerIcon);
                }
            }
        }

        /// <summary>Gets or sets a value that contains the footer text.</summary>
        public string FooterText
        {
            get { return footerText; }

            set
            {
                // Set local value, then update native dialog if showing.
                footerText = value;
                if (NativeDialogShowing)
                {
                    nativeDialog.UpdateFooterText(footerText);
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether hyperlinks are enabled.</summary>
        public bool HyperlinksEnabled
        {
            get { return hyperlinksEnabled; }

            set
            {
                ThrowIfDialogShowing(Resources.HyperlinksCannotBetSet);
                hyperlinksEnabled = value;
            }
        }

        /// <summary>Gets or sets a value that contains the TaskDialog main icon.</summary>
        public TaskDialogStandardIcon Icon
        {
            get { return icon; }

            set
            {
                // Set local value, then update native dialog if showing.
                icon = value;
                if (NativeDialogShowing)
                {
                    nativeDialog.UpdateMainIcon(icon);
                }
            }
        }

        /// <summary>Gets or sets a value that contains the instruction text.</summary>
        public string InstructionText
        {
            get { return instructionText; }

            set
            {
                // Set local value, then update native dialog if showing.
                instructionText = value;
                if (NativeDialogShowing)
                {
                    nativeDialog.UpdateInstruction(instructionText);
                }
            }
        }

        /// <summary>Gets or sets a value that contains the owner window's handle.</summary>
        public IntPtr OwnerWindowHandle
        {
            get { return ownerWindow; }

            set
            {
                ThrowIfDialogShowing(Resources.OwnerCannotBeChanged);
                ownerWindow = value;
            }
        }

        /// <summary>
        ///   Gets or sets the progress bar on the taskdialog. ProgressBar a visual representation of the progress of a
        ///   long running operation.
        /// </summary>
        public TaskDialogProgressBar ProgressBar
        {
            get { return progressBar; }

            set
            {
                ThrowIfDialogShowing(Resources.ProgressBarCannotBeChanged);
                if (value != null)
                {
                    if (value.HostingDialog != null)
                    {
                        throw new InvalidOperationException(Resources.ProgressBarCannotBeHostedInMultipleDialogs);
                    }

                    value.HostingDialog = this;
                }

                progressBar = value;
            }
        }

        /// <summary>Gets or sets a value that contains the standard buttons.</summary>
        public TaskDialogStandardButtons StandardButtons
        {
            get { return standardButtons; }

            set
            {
                ThrowIfDialogShowing(Resources.StandardButtonsCannotBeChanged);
                standardButtons = value;
            }
        }

        /// <summary>Gets or sets a value that contains the startup location.</summary>
        public TaskDialogStartupLocation StartupLocation
        {
            get { return startupLocation; }

            set
            {
                ThrowIfDialogShowing(Resources.StartupLocationCannotBeChanged);
                startupLocation = value;
            }
        }

        /// <summary>Gets or sets a value that contains the message text.</summary>
        public string Text
        {
            get { return text; }

            set
            {
                // Set local value, then update native dialog if showing.
                text = value;
                if (NativeDialogShowing)
                {
                    nativeDialog.UpdateText(text);
                }
            }
        }

        /// <summary>Gets a value indicating whether a native dialog is showing.</summary>
        /// <value><c>True</c> if a native dialog is showing; otherwise, <c>False</c>.</value>
        bool NativeDialogShowing
        {
            get
            {
                return (nativeDialog != null)
                       &&
                       (nativeDialog.ShowState == DialogShowState.Showing
                        || nativeDialog.ShowState == DialogShowState.Closing);
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
            if (!NativeDialogShowing)
            {
                throw new InvalidOperationException(Resources.TaskDialogCloseNonShowing);
            }

            nativeDialog.NativeClose(TaskDialogResult.Cancel);

            // TaskDialog's own cleanup code - which runs post show - will handle disposal of native dialog.
        }

        /// <summary>Close TaskDialog with a given TaskDialogResult</summary>
        /// <param name="closingResult">TaskDialogResult to return from the TaskDialog.Show() method</param>
        /// <exception cref="InvalidOperationException">if TaskDialog is not showing.</exception>
        public void Close(TaskDialogResult closingResult)
        {
            if (!NativeDialogShowing)
            {
                throw new InvalidOperationException(Resources.TaskDialogCloseNonShowing);
            }

            nativeDialog.NativeClose(closingResult);

            // TaskDialog's own cleanup code - which runs post show - will handle disposal of native dialog.
        }

        /// <summary>Dispose TaskDialog Resources</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Dispose TaskDialog Resources</summary>
        /// <param name="disposing">If true, indicates that this is being called via Dispose rather than via the finalizer.</param>
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    // Clean up managed resources.
                    if (nativeDialog != null && nativeDialog.ShowState == DialogShowState.Showing)
                    {
                        nativeDialog.NativeClose(TaskDialogResult.Cancel);
                    }

                    buttons = null;
                    radioButtons = null;
                    commandLinks = null;
                }

                // Clean up unmanaged resources SECOND, NTD counts on being closed before being disposed.
                if (nativeDialog != null)
                {
                    nativeDialog.Dispose();
                    nativeDialog = null;
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
            return ShowCore();
        }

        /// <summary>Creates and shows a modal task dialog.</summary>
        /// <param name="window">The window.</param>
        /// <returns>The dialog result.</returns>
        public TaskDialogResult ShowDialog(Window window)
        {
            OwnerWindowHandle = new WindowInteropHelper(window).Handle;
            return ShowCore();
        }

        /// <summary>Called whenever controls have been added or removed</summary>
        void IDialogControlHost.ApplyCollectionChanged()
        {
            // If we're showing, we should never get here - the changing notification would have thrown and the property
            // would not have been changed.
            Debug.Assert(!NativeDialogShowing, "Collection changed notification received despite show state of dialog");
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
            if (NativeDialogShowing)
            {
                TaskDialogButton button;
                TaskDialogRadioButton radioButton;
                if (control is TaskDialogProgressBar)
                {
                    if (!progressBar.HasValidValues)
                    {
                        throw new ArgumentException(Resources.TaskDialogProgressBarValueInRange);
                    }

                    switch (propertyName)
                    {
                        case "State":
                            nativeDialog.UpdateProgressBarState(progressBar.State);
                            break;
                        case "Value":
                            nativeDialog.UpdateProgressBarValue(progressBar.Value);
                            break;
                        case "Minimum":
                        case "Maximum":
                            nativeDialog.UpdateProgressBarRange();
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
                            nativeDialog.UpdateElevationIcon(button.Id, button.UseElevationIcon);
                            break;
                        case "Enabled":
                            nativeDialog.UpdateButtonEnabled(button.Id, button.Enabled);
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
                            nativeDialog.UpdateRadioButtonEnabled(radioButton.Id, radioButton.Enabled);
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
            return !NativeDialogShowing;
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

            if (!NativeDialogShowing)
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
            TaskDialogButtonBase button = GetButtonForId(id);

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
            EventHandler<TaskDialogClosingEventArgs> handler = Closing;
            if (handler != null)
            {
                var e = new TaskDialogClosingEventArgs();

                // Try to identify the button - is it a standard one?
                TaskDialogStandardButtons buttonClicked = MapButtonIdToStandardButton(id);

                // If not, it had better be a custom button...
                if (buttonClicked == TaskDialogStandardButtons.None)
                {
                    TaskDialogButtonBase customButton = GetButtonForId(id);

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
            if (HelpInvoked != null)
            {
                HelpInvoked(this, EventArgs.Empty);
            }
        }

        /// <summary>Raises the hyperlink click event.</summary>
        /// <param name="link">The link from the hyperlink.</param>
        internal void RaiseHyperlinkClickEvent(string link)
        {
            EventHandler<TaskDialogHyperlinkClickedEventArgs> handler = HyperlinkClick;
            if (handler != null)
            {
                handler(this, new TaskDialogHyperlinkClickedEventArgs(link));
            }
        }

        /// <summary>Raises the opened event.</summary>
        internal void RaiseOpenedEvent()
        {
            if (Opened != null)
            {
                Opened(this, EventArgs.Empty);
            }
        }

        /// <summary>Raises the tick event.</summary>
        /// <param name="ticks">The ticks.</param>
        internal void RaiseTickEvent(int ticks)
        {
            if (Tick != null)
            {
                Tick(this, new TaskDialogTickEventArgs(ticks));
            }
        }

        /// <summary>Applies the elevated icons.</summary>
        /// <param name="settings">The dialog settings.</param>
        /// <param name="controls">The dialog controls.</param>
        static void ApplyElevatedIcons(NativeTaskDialogSettings settings, IEnumerable<TaskDialogButtonBase> controls)
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
        static TaskDialogButtonData[] BuildButtonStructArray(List<TaskDialogButtonBase> controls)
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
        static TaskDialogResult ConstructDialogResult(NativeTaskDialog native)
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
        static int FindDefaultButtonId(List<TaskDialogButtonBase> controls)
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
        static TaskDialogStandardButtons MapButtonIdToStandardButton(int id)
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
        static TaskDialogResult ShowCoreStatic(string text, string instructionText, string caption)
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
        void ApplyControlConfiguration(NativeTaskDialogSettings settings)
        {
            // Deal with progress bars/marquees.
            if (progressBar != null)
            {
                if (progressBar.State == TaskDialogProgressBarState.Marquee)
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
            if (buttons.Count > 0 || commandLinks.Count > 0)
            {
                // These are the actual arrays/lists of the structs that we'll copy to the unmanaged heap.
                List<TaskDialogButtonBase> sourceList = buttons.Count > 0 ? buttons : commandLinks;
                settings.Buttons = BuildButtonStructArray(sourceList);

                // Apply option flag that forces all custom buttons to render as command links.
                if (commandLinks.Count > 0)
                {
                    settings.NativeConfiguration.TaskDialogFlags |= TaskDialogOptions.UseCommandLinks;
                }

                // Set default button and add elevation icons to appropriate buttons.
                settings.NativeConfiguration.DefaultButtonIndex = FindDefaultButtonId(sourceList);

                ApplyElevatedIcons(settings, sourceList);
            }

            if (radioButtons.Count <= 0)
            {
                return;
            }

            settings.RadioButtons = BuildButtonStructArray(radioButtons);

            // Set default radio button - radio buttons don't support.
            int defaultRadioButton = FindDefaultButtonId(radioButtons);
            settings.NativeConfiguration.DefaultRadioButtonIndex = defaultRadioButton;

            if (defaultRadioButton == 0)
            {
                settings.NativeConfiguration.TaskDialogFlags |= TaskDialogOptions.NoDefaultRadioButton;
            }
        }

        /// <summary>Applies the core dialog settings.</summary>
        /// <param name="settings">The dialog settings.</param>
        void ApplyCoreSettings(NativeTaskDialogSettings settings)
        {
            ApplyGeneralNativeConfiguration(settings.NativeConfiguration);
            ApplyTextConfiguration(settings.NativeConfiguration);
            ApplyOptionConfiguration(settings.NativeConfiguration);
            ApplyControlConfiguration(settings);
        }

        /// <summary>Applies the core settings.</summary>
        /// <param name="dialogConfig">The settings for the dialog</param>
        void ApplyGeneralNativeConfiguration(TaskDialogConfiguration dialogConfig)
        {
            // If an owner wasn't specifically specified, we'll use the app's main window.
            if (ownerWindow != IntPtr.Zero)
            {
                dialogConfig.ParentHandle = ownerWindow;
            }

            // Other miscellaneous sets.
            dialogConfig.MainIcon = new IconUnion((int)icon);
            dialogConfig.FooterIcon = new IconUnion((int)footerIcon);
            dialogConfig.CommonButtons = (TaskDialogCommonButtons)standardButtons;
        }

        /// <summary>Applies the option configuration.</summary>
        /// <param name="dialogConfig">The dialog config.</param>
        void ApplyOptionConfiguration(TaskDialogConfiguration dialogConfig)
        {
            // Handle options - start with no options set.
            var options = TaskDialogOptions.None;
            if (canCancel)
            {
                options |= TaskDialogOptions.AllowCancel;
            }

            if (footerCheckBoxChecked.HasValue && footerCheckBoxChecked.Value)
            {
                options |= TaskDialogOptions.CheckVerificationFlag;
            }

            if (hyperlinksEnabled)
            {
                options |= TaskDialogOptions.EnableHyperlinks;
            }

            if (detailsExpanded)
            {
                options |= TaskDialogOptions.ExpandedByDefault;
            }

            if (Tick != null)
            {
                options |= TaskDialogOptions.UseCallbackTimer;
            }

            if (startupLocation == TaskDialogStartupLocation.CenterOwner)
            {
                options |= TaskDialogOptions.PositionRelativeToWindow;
            }

            // Note: no validation required, as we allow this to be set even if there is no expanded information text
            // because that could be added later. Default for Win32 API is to expand into (and after) the content area.
            if (expansionMode == TaskDialogExpandedDetailsLocation.ExpandFooter)
            {
                options |= TaskDialogOptions.ExpandFooterArea;
            }

            // Finally, apply options to config.
            dialogConfig.TaskDialogFlags = options;
        }

        /// <summary>Applies the supplemental settings.</summary>
        /// <param name="settings">The dialog settings.</param>
        void ApplySupplementalSettings(NativeTaskDialogSettings settings)
        {
            if (progressBar != null)
            {
                if (progressBar.State != TaskDialogProgressBarState.Marquee)
                {
                    settings.ProgressBarMinimum = progressBar.Minimum;
                    settings.ProgressBarMaximum = progressBar.Maximum;
                    settings.ProgressBarValue = progressBar.Value;
                    settings.ProgressBarState = progressBar.State;
                }
            }

            if (HelpInvoked != null)
            {
                settings.InvokeHelp = true;
            }
        }

        /// <summary>Sets important text properties.</summary>
        /// <param name="dialogConfig">An instance of a <see cref="TaskDialogConfiguration" /> object.</param>
        void ApplyTextConfiguration(TaskDialogConfiguration dialogConfig)
        {
            // note that nulls or empty strings are fine here.
            dialogConfig.Content = text;
            dialogConfig.WindowTitle = caption;
            dialogConfig.MainInstruction = instructionText;
            dialogConfig.ExpandedInformation = detailsExpandedText;
            dialogConfig.ExpandedControlText = detailsExpandedLabel;
            dialogConfig.CollapsedControlText = detailsCollapsedLabel;
            dialogConfig.FooterText = footerText;
            dialogConfig.VerificationText = checkBoxText;
        }

        /// <summary>Cleans up data and structs from a single native dialog Show() invocation.</summary>
        void CleanUp()
        {
            // Reset values that would be considered 'volatile' in a given instance.
            if (progressBar != null)
            {
                progressBar.Reset();
            }

            // Clean out sorted control lists - though we don't of course clear the main controls collection, so the
            // controls are still around; we'll resort on next show, since the collection may have changed.
            if (buttons != null)
            {
                buttons.Clear();
            }

            if (commandLinks != null)
            {
                commandLinks.Clear();
            }

            if (radioButtons != null)
            {
                radioButtons.Clear();
            }

            progressBar = null;

            // Have the native dialog clean up the rest.
            if (nativeDialog != null)
            {
                nativeDialog.Dispose();
            }
        }

        /// <summary>Gets the button for id.</summary>
        /// <param name="id">The button id.</param>
        /// <returns>The <c>TaskDialogButton</c>.</returns>
        TaskDialogButtonBase GetButtonForId(int id)
        {
            return (TaskDialogButtonBase)controls.GetControlbyId(id);
        }

        /// <summary>Shows the core dialog</summary>
        /// <returns>Returns the result of the <c>TaskDialog</c>.</returns>
        TaskDialogResult ShowCore()
        {
            TaskDialogResult result;

            try
            {
                // Populate control lists, based on current contents - note we are somewhat late-bound on our control
                // lists, to support XAML scenarios.
                SortDialogControls();

                // First, let's make sure it even makes sense to try a show.
                ValidateCurrentDialogSettings();

                // Create settings object for new dialog, based on current state.
                var settings = new NativeTaskDialogSettings();
                ApplyCoreSettings(settings);
                ApplySupplementalSettings(settings);

                // Show the dialog. NOTE: this is a BLOCKING call; the dialog proc callbacks will be executed by the
                // same thread as the Show() call before the thread of execution contines to the end of this method.
                nativeDialog = new NativeTaskDialog(settings, this);
                nativeDialog.NativeShow();

                // Build and return dialog result to public API - leaving it null after an exception is thrown is fine
                // in this case
                result = ConstructDialogResult(nativeDialog);
                footerCheckBoxChecked = nativeDialog.CheckBoxChecked;
            }
            finally
            {
                CleanUp();
                nativeDialog = null;
            }

            return result;
        }

        /// <summary>Sorts the dialog controls.</summary>
        void SortDialogControls()
        {
            foreach (var control in controls)
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
                    commandLinks.Add(commandLink);
                }
                else if ((radButton = control as TaskDialogRadioButton) != null)
                {
                    if (radioButtons == null)
                    {
                        radioButtons = new List<TaskDialogButtonBase>();
                    }

                    radioButtons.Add(radButton);
                }
                else if (buttonBase != null)
                {
                    if (buttons == null)
                    {
                        buttons = new List<TaskDialogButtonBase>();
                    }

                    buttons.Add(buttonBase);
                }
                else if ((progBar = control as TaskDialogProgressBar) != null)
                {
                    progressBar = progBar;
                }
                else
                {
                    throw new InvalidOperationException(Resources.TaskDialogUnkownControl);
                }
            }
        }

        /// <summary>Throws if dialog showing.</summary>
        /// <param name="message">The message to be shown in the exception.</param>
        void ThrowIfDialogShowing(string message)
        {
            if (NativeDialogShowing)
            {
                throw new NotSupportedException(message);
            }
        }

        /// <summary>Validates the current dialog settings.</summary>
        void ValidateCurrentDialogSettings()
        {
            if (footerCheckBoxChecked.HasValue && footerCheckBoxChecked.Value && string.IsNullOrEmpty(checkBoxText))
            {
                throw new InvalidOperationException(Resources.TaskDialogCheckBoxTextRequiredToEnableCheckBox);
            }

            // Progress bar validation. Make sure the progress bar values are valid. the Win32 API will valiantly try to
            // rationalize bizarre min/max/value combinations, but we'll save it the trouble by validating.
            if (progressBar != null && !progressBar.HasValidValues)
            {
                throw new InvalidOperationException(Resources.TaskDialogProgressBarValueInRange);
            }

            // Validate Buttons collection. Make sure we don't have buttons AND command-links - the Win32 API treats
            // them as different flavors of a single button struct.
            if (buttons.Count > 0 && commandLinks.Count > 0)
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