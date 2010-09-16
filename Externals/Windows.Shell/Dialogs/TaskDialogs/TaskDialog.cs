#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Encapsulates a new-to-Vista Win32 TaskDialog window 
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
        private static TaskDialog staticDialog;

        // Main current native dialog.

        private List<TaskDialogButtonBase> buttons;
        private List<TaskDialogButtonBase> commandLinks;
        private NativeTaskDialog nativeDialog;
        private IntPtr ownerWindow;
        private List<TaskDialogButtonBase> radioButtons;

        #region Public Properties

        // Main content (maps to MessageBox's "message"). 
        private bool cancelable;
        private string caption;
        private string checkBoxText;
        private string detailsCollapsedLabel;
        private bool detailsExpanded;
        private string detailsExpandedLabel;
        private string detailsExpandedText;
        private TaskDialogExpandedDetailsLocation expansionMode;
        private bool? footerCheckBoxChecked;
        private TaskDialogStandardIcon footerIcon;
        private string footerText;
        private bool hyperlinksEnabled;
        private TaskDialogStandardIcon icon;
        private string instructionText;
        private TaskDialogProgressBar progressBar;
        private TaskDialogStandardButtons standardButtons = TaskDialogStandardButtons.None;
        private TaskDialogStartupLocation startupLocation;
        private string text;

        /// <summary>
        ///   Gets or sets a value that contains the owner window's handle.
        /// </summary>
        public IntPtr OwnerWindowHandle
        {
            get { return ownerWindow; }
            set
            {
                ThrowIfDialogShowing("Dialog owner cannot be modified while dialog is showing.");
                ownerWindow = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the message text.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                // Set local value, then update native dialog if showing.
                text = value;
                if (NativeDialogShowing)
                    nativeDialog.UpdateText(text);
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the instruction text.
        /// </summary>
        public string InstructionText
        {
            get { return instructionText; }
            set
            {
                // Set local value, then update native dialog if showing.
                instructionText = value;
                if (NativeDialogShowing)
                    nativeDialog.UpdateInstruction(instructionText);
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the caption text.
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set
            {
                ThrowIfDialogShowing("Dialog caption can't be set while dialog is showing.");
                caption = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the footer text.
        /// </summary>
        public string FooterText
        {
            get { return footerText; }
            set
            {
                // Set local value, then update native dialog if showing.
                footerText = value;
                if (NativeDialogShowing)
                    nativeDialog.UpdateFooterText(footerText);
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the footer check box text.
        /// </summary>
        public string FooterCheckBoxText
        {
            get { return checkBoxText; }
            set
            {
                ThrowIfDialogShowing("Checkbox text can't be set while dialog is showing.");
                checkBoxText = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the expanded text in the details section.
        /// </summary>
        public string DetailsExpandedText
        {
            get { return detailsExpandedText; }
            set
            {
                // Set local value, then update native dialog if showing.
                detailsExpandedText = value;
                if (NativeDialogShowing)
                    nativeDialog.UpdateExpandedText(detailsExpandedText);
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if the details section is expanded.
        /// </summary>
        public bool DetailsExpanded
        {
            get { return detailsExpanded; }
            set
            {
                ThrowIfDialogShowing("Expanded state of the dialog can't be modified while dialog is showing.");
                detailsExpanded = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the expanded control text.
        /// </summary>
        public string DetailsExpandedLabel
        {
            get { return detailsExpandedLabel; }
            set
            {
                ThrowIfDialogShowing("Expanded control label can't be set while dialog is showing.");
                detailsExpandedLabel = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the collapsed control text.
        /// </summary>
        public string DetailsCollapsedLabel
        {
            get { return detailsCollapsedLabel; }
            set
            {
                ThrowIfDialogShowing("Collapsed control text can't be set while dialog is showing.");
                detailsCollapsedLabel = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if Cancelable is set.
        /// </summary>
        public bool Cancelable
        {
            get { return cancelable; }
            set
            {
                ThrowIfDialogShowing("Cancelable can't be set while dialog is showing.");
                cancelable = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the TaskDialog main icon.
        /// </summary>
        public TaskDialogStandardIcon Icon
        {
            get { return icon; }
            set
            {
                // Set local value, then update native dialog if showing.
                icon = value;
                if (NativeDialogShowing)
                    nativeDialog.UpdateMainIcon(icon);
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the footer icon.
        /// </summary>
        public TaskDialogStandardIcon FooterIcon
        {
            get { return footerIcon; }
            set
            {
                // Set local value, then update native dialog if showing.
                footerIcon = value;
                if (NativeDialogShowing)
                    nativeDialog.UpdateFooterIcon(footerIcon);
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the standard buttons.
        /// </summary>
        public TaskDialogStandardButtons StandardButtons
        {
            get { return standardButtons; }
            set
            {
                ThrowIfDialogShowing("Standard buttons can't be set while dialog is showing.");
                standardButtons = value;
            }
        }

        /// <summary>
        ///   Gets a value that contains the TaskDialog controls.
        /// </summary>
        public DialogControlCollection<TaskDialogControl> Controls { get; private set; }

        /// <summary>
        ///   Gets or sets a value that determines if hyperlinks are enabled.
        /// </summary>
        public bool HyperlinksEnabled
        {
            get { return hyperlinksEnabled; }
            set
            {
                ThrowIfDialogShowing("Hyperlinks can't be enabled/disabled while dialog is showing.");
                hyperlinksEnabled = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates if the footer checkbox is checked.
        /// </summary>
        public bool? FooterCheckBoxChecked
        {
            get { return !footerCheckBoxChecked.HasValue ? false : footerCheckBoxChecked; }
            set
            {
                // Set local value, then update native dialog if showing.
                footerCheckBoxChecked = value;
                if (NativeDialogShowing)
                    nativeDialog.UpdateCheckBoxChecked(footerCheckBoxChecked.Value);
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the expansion mode for this dialog.
        /// </summary>
        public TaskDialogExpandedDetailsLocation ExpansionMode
        {
            get { return expansionMode; }
            set
            {
                ThrowIfDialogShowing("Expanded information mode can't be set while dialog is showing.");
                expansionMode = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that contains the startup location.
        /// </summary>
        public TaskDialogStartupLocation StartupLocation
        {
            get { return startupLocation; }
            set
            {
                ThrowIfDialogShowing("Startup location can't be changed while dialog is showing.");
                startupLocation = value;
            }
        }

        /// <summary>
        ///   Gets or sets the progress bar on the taskdialog. ProgressBar a visual representation 
        ///   of the progress of a long running operation.
        /// </summary>
        public TaskDialogProgressBar ProgressBar
        {
            get { return progressBar; }
            set
            {
                ThrowIfDialogShowing("Progress bar can't be changed while dialog is showing");
                if (value != null)
                {
                    if (value.HostingDialog != null)
                        throw new InvalidOperationException("Progress bar cannot be hosted in multiple dialogs.");

                    value.HostingDialog = this;
                }
                progressBar = value;
            }
        }

        /// <summary>
        ///   Occurs when a progress bar changes.
        /// </summary>
        public event EventHandler<TaskDialogTickEventArgs> Tick;

        /// <summary>
        ///   Occurs when a user clicks a hyperlink.
        /// </summary>
        public event EventHandler<TaskDialogHyperlinkClickedEventArgs> HyperlinkClick;

        /// <summary>
        ///   Occurs when the TaskDialog is closing.
        /// </summary>
        public event EventHandler<TaskDialogClosingEventArgs> Closing;

        /// <summary>
        ///   Occurs when a user clicks on Help.
        /// </summary>
        public event EventHandler HelpInvoked;

        /// <summary>
        ///   Occurs when the TaskDialog is opened.
        /// </summary>
        public event EventHandler Opened;

        #endregion

        #region Constructors

        // Constructors.

        /// <summary>
        ///   Creates a basic TaskDialog window
        /// </summary>
        public TaskDialog()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            // Initialize various data structs.
            Controls = new DialogControlCollection<TaskDialogControl>(this);
            buttons = new List<TaskDialogButtonBase>();
            radioButtons = new List<TaskDialogButtonBase>();
            commandLinks = new List<TaskDialogButtonBase>();
        }

        #endregion

        #region Static Show Methods

        /// <summary>
        ///   Creates and shows a task dialog with the specified message text.
        /// </summary>
        /// <param name = "text">The text to display.</param>
        /// <returns>The dialog result.</returns>
        public static TaskDialogResult Show(string text)
        {
            return ShowCoreStatic(text, TaskDialogDefaults.MainInstruction, TaskDialogDefaults.Caption);
        }

        /// <summary>
        ///   Creates and shows a task dialog with the specified supporting text and main instruction.
        /// </summary>
        /// <param name = "text">The supporting text to display.</param>
        /// <param name = "instructionText">The main instruction text to display.</param>
        /// <returns>The dialog result.</returns>
        public static TaskDialogResult Show(string text, string instructionText)
        {
            return ShowCoreStatic(text, instructionText, TaskDialogDefaults.Caption);
        }

        /// <summary>
        ///   Creates and shows a task dialog with the specified supporting text, main instruction, and dialog caption.
        /// </summary>
        /// <param name = "text">The supporting text to display.</param>
        /// <param name = "instructionText">The main instruction text to display.</param>
        /// <param name = "caption">The caption for the dialog.</param>
        /// <returns>The dialog result.</returns>
        public static TaskDialogResult Show(string text, string instructionText, string caption)
        {
            return ShowCoreStatic(text, instructionText, caption);
        }

        #endregion

        #region Instance Show Methods

        /// <summary>
        ///   Creates and shows a task dialog.
        /// </summary>
        /// <returns>The dialog result.</returns>
        public TaskDialogResult Show()
        {
            return ShowCore();
        }

        /// <summary>
        ///   Creates and shows a modal task dialog.
        /// </summary>
        /// <returns>The dialog result.</returns>
        public TaskDialogResult ShowDialog(Window window)
        {
            OwnerWindowHandle = new WindowInteropHelper(window).Handle;
            return ShowCore();
        }

        #endregion

        #region Core Show Logic

        // CORE SHOW METHODS:
        // All static Show() calls forward here - 
        // it is responsible for retrieving
        // or creating our cached TaskDialog instance, getting it configured,
        // and in turn calling the appropriate instance Show.

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

        private TaskDialogResult ShowCore()
        {
            TaskDialogResult result;

            try
            {
                // Populate control lists, based on current 
                // contents - note we are somewhat late-bound 
                // on our control lists, to support XAML scenarios.
                SortDialogControls();

                // First, let's make sure it even makes 
                // sense to try a show.
                ValidateCurrentDialogSettings();

                // Create settings object for new dialog, 
                // based on current state.
                var settings = new NativeTaskDialogSettings();
                ApplyCoreSettings(settings);
                ApplySupplementalSettings(settings);

                // Show the dialog.
                // NOTE: this is a BLOCKING call; the dialog proc callbacks
                // will be executed by the same thread as the 
                // Show() call before the thread of execution 
                // contines to the end of this method.
                nativeDialog = new NativeTaskDialog(settings, this);
                nativeDialog.NativeShow();

                // Build and return dialog result to public API - leaving it
                // null after an exception is thrown is fine in this case
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

        // Helper that looks at the current state of the TaskDialog and verifies
        // that there aren't any abberant combinations of properties.
        // NOTE that this method is designed to throw 
        // rather than return a bool.
        private void ValidateCurrentDialogSettings()
        {
            if (footerCheckBoxChecked.HasValue && footerCheckBoxChecked.Value && String.IsNullOrEmpty(checkBoxText))
                throw new InvalidOperationException("Checkbox text must be provided to enable the dialog checkbox.");

            // Progress bar validation.
            // Make sure the progress bar values are valid.
            // the Win32 API will valiantly try to rationalize 
            // bizarre min/max/value combinations, but we'll save
            // it the trouble by validating.
            if (progressBar != null)
            {
                if (!progressBar.HasValidValues)
                    throw new ArgumentException("Progress bar must have a value between the minimum and maxium values.");
            }

            // Validate Buttons collection.
            // Make sure we don't have buttons AND 
            // command-links - the Win32 API treats them as different
            // flavors of a single button struct.
            if (buttons.Count > 0 && commandLinks.Count > 0)
                throw new NotSupportedException("Dialog cannot display both non-standard buttons and command links.");
            //if (buttons.Count > 0 && standardButtons != TaskDialogStandardButtons.None)
            //    throw new NotSupportedException("Dialog cannot display both non-standard buttons and standard buttons.");
        }

        // Analyzes the final state of the NativeTaskDialog instance and creates the 
        // final TaskDialogResult that will be returned from the public API
        private static TaskDialogResult ConstructDialogResult(NativeTaskDialog native)
        {
            Debug.Assert(native.ShowState == DialogShowState.Closed, "dialog result being constructed for unshown dialog.");

            TaskDialogResult result;

            var standardButton = MapButtonIdToStandardButton(native.SelectedButtonID);

            // If returned ID isn't a standard button, let's fetch 
            if (standardButton == TaskDialogStandardButtons.None)
                result = TaskDialogResult.CustomButtonClicked;
            else
                result = (TaskDialogResult) standardButton;

            return result;
        }

        /// <summary>
        ///   Close TaskDialog
        /// </summary>
        /// <exception cref = "InvalidOperationException">if TaskDialog is not showing.</exception>
        public void Close()
        {
            if (!NativeDialogShowing)
                throw new InvalidOperationException("Attempting to close a non-showing dialog.");

            nativeDialog.NativeClose(TaskDialogResult.Cancel);
            // TaskDialog's own cleanup code - 
            // which runs post show - will handle disposal of native dialog.
        }

        /// <summary>
        ///   Close TaskDialog with a given TaskDialogResult
        /// </summary>
        /// <param name = "closingResult">TaskDialogResult to return from the TaskDialog.Show() method</param>
        /// <exception cref = "InvalidOperationException">if TaskDialog is not showing.</exception>
        public void Close(TaskDialogResult closingResult)
        {
            if (!NativeDialogShowing)
                throw new InvalidOperationException("Attempting to close a non-showing dialog.");

            nativeDialog.NativeClose(closingResult);
            // TaskDialog's own cleanup code - 
            // which runs post show - will handle disposal of native dialog.
        }

        #endregion

        #region Configuration Construction

        private void ApplyCoreSettings(NativeTaskDialogSettings settings)
        {
            ApplyGeneralNativeConfiguration(settings.NativeConfiguration);
            ApplyTextConfiguration(settings.NativeConfiguration);
            ApplyOptionConfiguration(settings.NativeConfiguration);
            ApplyControlConfiguration(settings);
        }

        private void ApplyGeneralNativeConfiguration(TaskDialogNativeMethods.TASKDIALOGCONFIG dialogConfig)
        {
            // If an owner wasn't specifically specified, 
            // we'll use the app's main window.
            if (ownerWindow != IntPtr.Zero)
                dialogConfig.hwndParent = ownerWindow;

            // Other miscellaneous sets.
            dialogConfig.MainIcon = new TaskDialogNativeMethods.TASKDIALOGCONFIG_ICON_UNION((int) icon);
            dialogConfig.FooterIcon = new TaskDialogNativeMethods.TASKDIALOGCONFIG_ICON_UNION((int) footerIcon);
            dialogConfig.dwCommonButtons = (TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS) standardButtons;
        }

        /// <summary>
        ///   Sets important text properties.
        /// </summary>
        /// <param name = "dialogConfig">An instance of a <see cref = "TaskDialogNativeMethods.TASKDIALOGCONFIG" /> object.</param>
        private void ApplyTextConfiguration(TaskDialogNativeMethods.TASKDIALOGCONFIG dialogConfig)
        {
            // note that nulls or empty strings are fine here.
            dialogConfig.pszContent = text;
            dialogConfig.pszWindowTitle = caption;
            dialogConfig.pszMainInstruction = instructionText;
            dialogConfig.pszExpandedInformation = detailsExpandedText;
            dialogConfig.pszExpandedControlText = detailsExpandedLabel;
            dialogConfig.pszCollapsedControlText = detailsCollapsedLabel;
            dialogConfig.pszFooter = footerText;
            dialogConfig.pszVerificationText = checkBoxText;
        }

        private void ApplyOptionConfiguration(TaskDialogNativeMethods.TASKDIALOGCONFIG dialogConfig)
        {
            // Handle options - start with no options set.
            var options = TaskDialogNativeMethods.TASKDIALOG_FLAGS.NONE;
            if (cancelable)
                options |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION;
            if (footerCheckBoxChecked.HasValue && footerCheckBoxChecked.Value)
                options |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED;
            if (hyperlinksEnabled)
                options |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_ENABLE_HYPERLINKS;
            if (detailsExpanded)
                options |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_EXPANDED_BY_DEFAULT;
            if (Tick != null)
                options |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_CALLBACK_TIMER;
            if (startupLocation == TaskDialogStartupLocation.CenterOwner)
                options |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_POSITION_RELATIVE_TO_WINDOW;

            // Note: no validation required, as we allow this to 
            // be set even if there is no expanded information 
            // text because that could be added later.
            // Default for Win32 API is to expand into (and after) 
            // the content area.
            if (expansionMode == TaskDialogExpandedDetailsLocation.ExpandFooter)
                options |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_EXPAND_FOOTER_AREA;

            // Finally, apply options to config.
            dialogConfig.dwFlags = options;
        }

        // Builds the actual configuration 
        // that the NativeTaskDialog (and underlying Win32 API)
        // expects, by parsing the various control 
        // lists, marshalling to the unmanaged heap, etc.

        private void ApplyControlConfiguration(NativeTaskDialogSettings settings)
        {
            // Deal with progress bars/marquees.
            if (progressBar != null)
            {
                if (progressBar.State == TaskDialogProgressBarState.Marquee)
                    settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_SHOW_MARQUEE_PROGRESS_BAR;
                else
                    settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_SHOW_PROGRESS_BAR;
            }

            // Build the native struct arrays that NativeTaskDialog 
            // needs - though NTD will handle
            // the heavy lifting marshalling to make sure 
            // all the cleanup is centralized there.
            if (buttons.Count > 0 || commandLinks.Count > 0)
            {
                // These are the actual arrays/lists of 
                // the structs that we'll copy to the 
                // unmanaged heap.
                var sourceList = (buttons.Count > 0 ? buttons : commandLinks);
                settings.Buttons = BuildButtonStructArray(sourceList);

                // Apply option flag that forces all 
                // custom buttons to render as command links.
                if (commandLinks.Count > 0)
                    settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS;

                // Set default button and add elevation icons 
                // to appropriate buttons.
                settings.NativeConfiguration.nDefaultButton = FindDefaultButtonId(sourceList);

                ApplyElevatedIcons(settings, sourceList);
            }
            if (radioButtons.Count <= 0)
                return;
            settings.RadioButtons = BuildButtonStructArray(radioButtons);

            // Set default radio button - radio buttons don't support.
            var defaultRadioButton = FindDefaultButtonId(radioButtons);
            settings.NativeConfiguration.nDefaultRadioButton = defaultRadioButton;

            if (defaultRadioButton == TaskDialogNativeMethods.NO_DEFAULT_BUTTON_SPECIFIED)
                settings.NativeConfiguration.dwFlags |= TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_NO_DEFAULT_RADIO_BUTTON;
        }

        private static TaskDialogNativeMethods.TASKDIALOG_BUTTON[] BuildButtonStructArray(IList<TaskDialogButtonBase> controls)
        {
            TaskDialogButtonBase button;

            var totalButtons = controls.Count;
            var buttonStructs = new TaskDialogNativeMethods.TASKDIALOG_BUTTON[totalButtons];
            for (var i = 0; i < totalButtons; i++)
            {
                button = controls[i];
                buttonStructs[i] = new TaskDialogNativeMethods.TASKDIALOG_BUTTON(button.Id, button.ToString());
            }
            return buttonStructs;
        }

        // Searches list of controls and returns the ID of 
        // the default control, or null if no default was specified.
        private static int FindDefaultButtonId(IEnumerable<TaskDialogButtonBase> controls)
        {
            const int found = TaskDialogNativeMethods.NO_DEFAULT_BUTTON_SPECIFIED;
            return controls.Where(control => control.Default).Select(control => control.Id).FirstOrDefault();
        }

        private static void ApplyElevatedIcons(NativeTaskDialogSettings settings, IEnumerable<TaskDialogButtonBase> controls)
        {
            foreach (var control in controls.Cast<TaskDialogButton>().Where(control => control.ShowElevationIcon))
            {
                if (settings.ElevatedButtons == null)
                    settings.ElevatedButtons = new List<int>();
                settings.ElevatedButtons.Add(control.Id);
            }
        }

        private void ApplySupplementalSettings(NativeTaskDialogSettings settings)
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
                settings.InvokeHelp = true;
        }

        // Here we walk our controls collection and 
        // sort the various controls by type. 
        private void SortDialogControls()
        {
            foreach (var control in Controls)
            {
                if (control is TaskDialogButtonBase && String.IsNullOrEmpty(((TaskDialogButtonBase) control).Text))
                {
                    if (control is TaskDialogCommandLink && String.IsNullOrEmpty(((TaskDialogCommandLink) control).Instruction))
                        throw new InvalidOperationException("Button text must be non-empty");
                }

                // Loop through child controls 
                // and sort the controls based on type.
                if (control is TaskDialogCommandLink)
                    commandLinks.Add((TaskDialogCommandLink) control);
                else if (control is TaskDialogRadioButton)
                {
                    if (radioButtons == null)
                        radioButtons = new List<TaskDialogButtonBase>();
                    radioButtons.Add((TaskDialogRadioButton) control);
                }
                else if (control is TaskDialogButtonBase)
                {
                    if (buttons == null)
                        buttons = new List<TaskDialogButtonBase>();
                    buttons.Add((TaskDialogButtonBase) control);
                }
                else if (control is TaskDialogProgressBar)
                    progressBar = (TaskDialogProgressBar) control;
                else
                    throw new ArgumentException("Unknown dialog control type.");
            }
        }

        #endregion

        #region Helpers

        // Helper to map the standard button IDs returned by 
        // TaskDialogIndirect to the standard button ID enum - 
        // note that we can't just cast, as the Win32
        // typedefs differ incoming and outgoing.

        private bool NativeDialogShowing { get { return (nativeDialog != null) && (nativeDialog.ShowState == DialogShowState.Showing || nativeDialog.ShowState == DialogShowState.Closing); } }

        private static TaskDialogStandardButtons MapButtonIdToStandardButton(int id)
        {
            switch ((TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID) id)
            {
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDOK:
                    return TaskDialogStandardButtons.Ok;
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDCANCEL:
                    return TaskDialogStandardButtons.Cancel;
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDABORT:
                    // Included for completeness in API - 
                    // we can't pass in an Abort standard button.
                    return TaskDialogStandardButtons.None;
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDRETRY:
                    return TaskDialogStandardButtons.Retry;
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDIGNORE:
                    // Included for completeness in API - 
                    // we can't pass in an Ignore standard button.
                    return TaskDialogStandardButtons.None;
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDYES:
                    return TaskDialogStandardButtons.Yes;
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDNO:
                    return TaskDialogStandardButtons.No;
                case TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDCLOSE:
                    return TaskDialogStandardButtons.Close;
                default:
                    return TaskDialogStandardButtons.None;
            }
        }

        private void ThrowIfDialogShowing(string message)
        {
            if (NativeDialogShowing)
                throw new NotSupportedException(message);
        }

        // NOTE: we are going to require names be unique 
        // across both buttons and radio buttons,
        // even though the Win32 API allows them to be separate.
        private TaskDialogButtonBase GetButtonForId(int id)
        {
            return (TaskDialogButtonBase) Controls.GetControlbyId(id);
        }

        #endregion

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

        // We're explicitly implementing this interface 
        // as the user will never need to know about it
        // or use it directly - it is only for the internal 
        // implementation of "pseudo controls" within 
        // the dialogs.

        // Called whenever controls are being added 
        // to or removed from the dialog control collection.

        #region IDialogControlHost Members

        bool IDialogControlHost.IsCollectionChangeAllowed()
        {
            // Only allow additions to collection if dialog is NOT showing.
            return !NativeDialogShowing;
        }

        // Called whenever controls have been added or removed.
        void IDialogControlHost.ApplyCollectionChanged()
        {
            // If we're showing, we should never get here - 
            // the changing notification would have thrown and the 
            // property would not have been changed.
            Debug.Assert(!NativeDialogShowing, "Collection changed notification received despite show state of dialog");
        }

        // Called when a control currently in the collection 
        // has a property changing - this is 
        // basically to screen out property changes that 
        // cannot occur while the dialog is showing
        // because the Win32 API has no way for us to 
        // propagate the changes until we re-invoke the Win32 call.
        bool IDialogControlHost.IsControlPropertyChangeAllowed(string propertyName, DialogControl control)
        {
            Debug.Assert(control is TaskDialogControl, "Property changing for a control that is not a TaskDialogControl-derived type");
            Debug.Assert(propertyName != "Name", "Name changes at any time are not supported - public API should have blocked this");

            var canChange = false;

            if (!NativeDialogShowing)
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

        // Called when a control currently in the collection 
        // has a property changed - this handles propagating
        // the new property values to the Win32 API. 
        // If there isn't a way to change the Win32 value, then we
        // should have already screened out the property set 
        // in NotifyControlPropertyChanging.
        void IDialogControlHost.ApplyControlPropertyChange(string propertyName, DialogControl control)
        {
            // We only need to apply changes to the 
            // native dialog when it actually exists.
            if (NativeDialogShowing)
            {
                if (control is TaskDialogProgressBar)
                {
                    if (!progressBar.HasValidValues)
                        throw new ArgumentException("Progress bar must have a value between Minimum and Maximum.");
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
                else if (control is TaskDialogButton)
                {
                    var button = (TaskDialogButton) control;
                    switch (propertyName)
                    {
                        case "ShowElevationIcon":
                            nativeDialog.UpdateElevationIcon(button.Id, button.ShowElevationIcon);
                            break;
                        case "Enabled":
                            nativeDialog.UpdateButtonEnabled(button.Id, button.Enabled);
                            break;
                        default:
                            Debug.Assert(true, "Unknown property being set");
                            break;
                    }
                }
                else if (control is TaskDialogRadioButton)
                {
                    var button = (TaskDialogRadioButton) control;
                    switch (propertyName)
                    {
                        case "Enabled":
                            nativeDialog.UpdateRadioButtonEnabled(button.Id, button.Enabled);
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

        #endregion

        #region Event Percolation Methods

        // All Raise*() methods are called by the 
        // NativeTaskDialog when various pseudo-controls
        // are triggered.
        internal void RaiseButtonClickEvent(int id)
        {
            // First check to see if the ID matches a custom button.
            var button = GetButtonForId(id);

            // If a custom button was found, 
            // raise the event - if not, it's a standard button, and
            // we don't support custom event handling for the standard buttons
            if (button != null)
                button.RaiseClickEvent();
        }

        internal void RaiseHyperlinkClickEvent(string link)
        {
            var handler = HyperlinkClick;
            if (handler != null)
                handler(this, new TaskDialogHyperlinkClickedEventArgs(link));
        }

        // Gives event subscriber a chance to prevent 
        // the dialog from closing, based on 
        // the current state of the app and the button 
        // used to commit. Note that we don't 
        // have full access at this stage to 
        // the full dialog state.
        internal int RaiseClosingEvent(int id)
        {
            var handler = Closing;
            if (handler != null)
            {
                TaskDialogButtonBase customButton;
                var e = new TaskDialogClosingEventArgs();

                // Try to identify the button - is it a standard one?
                var buttonClicked = MapButtonIdToStandardButton(id);

                // If not, it had better be a custom button...
                if (buttonClicked == TaskDialogStandardButtons.None)
                {
                    customButton = GetButtonForId(id);

                    // ... or we have a problem.
                    if (customButton == null)
                        throw new InvalidOperationException("Bad button ID in closing event.");

                    e.CustomButton = customButton.Name;

                    e.TaskDialogResult = TaskDialogResult.CustomButtonClicked;
                }
                else
                    e.TaskDialogResult = (TaskDialogResult) buttonClicked;

                // Raise the event and determine how to proceed.
                handler(this, e);
                if (e.Cancel)
                    return (int) HRESULT.S_FALSE;
            }

            // It's okay to let the dialog close.
            return (int) HRESULT.S_OK;
        }

        internal void RaiseHelpInvokedEvent()
        {
            var handler = HelpInvoked;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        internal void RaiseOpenedEvent()
        {
            var handler = Opened;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        internal void RaiseTickEvent(int ticks)
        {
            var handler = Tick;
            if (handler != null)
                handler(this, new TaskDialogTickEventArgs(ticks));
        }

        #endregion

        #region Cleanup Code

        // Cleans up data and structs from a single 
        // native dialog Show() invocation.

        // Dispose pattern - cleans up data and structs for 
        // a) any native dialog currently showing, and
        // b) anything else that the outer TaskDialog has.
        private bool disposed;

        /// <summary>
        ///   Dispose TaskDialog Resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp()
        {
            // Reset values that would be considered 
            // 'volatile' in a given instance.
            if (progressBar != null)
                progressBar.Reset();

            // Clean out sorted control lists - 
            // though we don't of course clear the main controls collection,
            // so the controls are still around; we'll 
            // resort on next show, since the collection may have changed.
            if (buttons != null)
                buttons.Clear();
            if (commandLinks != null)
                commandLinks.Clear();
            if (radioButtons != null)
                radioButtons.Clear();
            progressBar = null;

            // Have the native dialog clean up the rest.
            if (nativeDialog != null)
                nativeDialog.Dispose();
        }

        /// <summary>
        ///   TaskDialog Finalizer
        /// </summary>
        ~TaskDialog()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Dispose TaskDialog Resources
        /// </summary>
        /// <param name = "disposing">If true, indicates that this is being called via Dispose rather than via the finalizer.</param>
        public void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;

            if (disposing)
            {
                // Clean up managed resources.
                if (nativeDialog != null && nativeDialog.ShowState == DialogShowState.Showing)
                    nativeDialog.NativeClose(TaskDialogResult.Cancel);

                buttons = null;
                radioButtons = null;
                commandLinks = null;
            }

            // Clean up unmanaged resources SECOND, NTD counts on 
            // being closed before being disposed.
            if (nativeDialog != null)
            {
                nativeDialog.Dispose();
                nativeDialog = null;
            }

            if (staticDialog == null)
                return;
            staticDialog.Dispose();
            staticDialog = null;
        }

        #endregion
    }
}