//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using Microsoft.Windows.Dialogs.Controls;
using Microsoft.Windows.Internal;
using Microsoft.Windows.Shell;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Defines the abstract base class for the common file dialogs.
    /// </summary>
    [ContentProperty("Controls")]
    public abstract class CommonFileDialog : IDialogControlHost, IDisposable
    {
        /// <summary>
        ///   Contains a common error message string shared by classes that 
        ///   inherit from this class.
        /// </summary>
        protected const string IllegalPropertyChangeString = " cannot be changed while dialog is showing";

        #region Constructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialog()
        {
            if (!CoreHelpers.RunningOnVista)
                throw new PlatformNotSupportedException("Common File Dialog requires Windows Vista or later.");

            fileNames = new Collection<string>();
            Filters = new CommonFileDialogFilterCollection();
            items = new Collection<IShellItem>();
            Controls = new CommonFileDialogControlCollection<CommonFileDialogControl>(this);
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified title.
        /// </summary>
        /// <param name = "title">The title to display in the dialog.</param>
        protected CommonFileDialog(string title) : this()
        {
            this.title = title;
        }

        #endregion

        /// <summary>
        ///   The collection of names selected by the user.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes"),
         SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields",
             Justification = "This is an internal field used by the CommonOpenFileDialog and possibly other dialogs deriving from this base class.")] protected readonly Collection<string> fileNames;

        internal readonly Collection<IShellItem> items;
        private bool? canceled;
        private IFileDialogCustomize customize;
        private bool filterSet; // filters can be set only once
        private IFileDialog nativeDialog;
        private NativeDialogEventSink nativeEventSink;
        private IntPtr parentWindow = IntPtr.Zero;
        private bool resetSelections;
        internal DialogShowState showState = DialogShowState.PreShow;

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

        #region IDialogControlHost Members

        bool IDialogControlHost.IsCollectionChangeAllowed()
        {
            return true;
        }

        void IDialogControlHost.ApplyCollectionChanged()
        {
            // Query IFileDialogCustomize interface before adding controls
            GetCustomizedFileDialog();
            // Populate all the custom controls and add them to the dialog
            foreach (CommonFileDialogControl control in Controls)
            {
                if (!control.IsAdded)
                {
                    control.HostingDialog = this;
                    control.Attach(customize);
                    control.IsAdded = true;
                }
            }
        }

        bool IDialogControlHost.IsControlPropertyChangeAllowed(string propertyName, DialogControl control)
        {
            GenerateNotImplementedException();
            return false;
        }

        void IDialogControlHost.ApplyControlPropertyChange(string propertyName, DialogControl control)
        {
            if (propertyName == "Text")
            {
                if (control is CommonFileDialogTextBox)
                    customize.SetEditBoxText(control.Id, ((CommonFileDialogControl) control).Text);
                else
                    customize.SetControlLabel(control.Id, ((CommonFileDialogControl) control).Text);
            }
            else if (propertyName == "Visible")
            {
                var dialogControl = control as CommonFileDialogControl;
                ShellNativeMethods.CDCONTROLSTATE state;

                customize.GetControlState(control.Id, out state);

                if (dialogControl.Visible)
                    state |= ShellNativeMethods.CDCONTROLSTATE.CDCS_VISIBLE;
                else if (dialogControl.Visible == false)
                    state &= ~ShellNativeMethods.CDCONTROLSTATE.CDCS_VISIBLE;

                customize.SetControlState(control.Id, state);
            }
            else if (propertyName == "Enabled")
            {
                var dialogControl = control as CommonFileDialogControl;
                ShellNativeMethods.CDCONTROLSTATE state;

                customize.GetControlState(control.Id, out state);

                if (dialogControl.Enabled)
                    state |= ShellNativeMethods.CDCONTROLSTATE.CDCS_ENABLED;
                else if (dialogControl.Enabled == false)
                    state &= ~ShellNativeMethods.CDCONTROLSTATE.CDCS_ENABLED;

                customize.SetControlState(control.Id, state);
            }
            else if (propertyName == "SelectedIndex")
            {
                if (control is CommonFileDialogRadioButtonList)
                {
                    var list = control as CommonFileDialogRadioButtonList;
                    customize.SetSelectedControlItem(control.Id, list.SelectedIndex);
                }
                else if (control is CommonFileDialogComboBox)
                {
                    var box = control as CommonFileDialogComboBox;
                    customize.SetSelectedControlItem(control.Id, box.SelectedIndex);
                }
            }
            else if (propertyName == "IsChecked")
            {
                if (control is CommonFileDialogCheckBox)
                {
                    var checkBox = control as CommonFileDialogCheckBox;
                    customize.SetCheckButtonState(control.Id, checkBox.IsChecked);
                }
            }
        }

        #endregion

        #region Helpers

        private bool NativeDialogShowing { get { return (nativeDialog != null) && (showState == DialogShowState.Showing || showState == DialogShowState.Closing); } }

        /// <summary>
        ///   Ensures that the user has selected one or more files.
        /// </summary>
        /// <permission cref = "System.InvalidOperationException">
        ///   The dialog has not been dismissed yet or the dialog was cancelled.
        /// </permission>
        protected void CheckFileNamesAvailable()
        {
            if (showState != DialogShowState.Closed)
                throw new InvalidOperationException("Filename not available - dialog has not closed yet.");

            if (canceled.GetValueOrDefault())
                throw new InvalidOperationException("Filename not available - dialog was canceled.");

            Debug.Assert(fileNames.Count != 0, "FileNames empty - shouldn't happen unless dialog canceled or not yet shown.");
        }

        /// <summary>
        ///   Ensures that the user has selected one or more files.
        /// </summary>
        /// <permission cref = "System.InvalidOperationException">
        ///   The dialog has not been dismissed yet or the dialog was cancelled.
        /// </permission>
        protected void CheckFileItemsAvailable()
        {
            if (showState != DialogShowState.Closed)
                throw new InvalidOperationException("Filename not available - dialog has not closed yet.");

            if (canceled.GetValueOrDefault())
                throw new InvalidOperationException("Filename not available - dialog was canceled.");

            Debug.Assert(items.Count != 0, "Items list empty - shouldn't happen unless dialog canceled or not yet shown.");
        }

        internal static string GetFileNameFromShellItem(IShellItem item)
        {
            string filename = null;
            IntPtr pszString;
            HRESULT hr = item.GetDisplayName(ShellNativeMethods.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out pszString);
            if (hr == HRESULT.S_OK && pszString != IntPtr.Zero)
            {
                filename = Marshal.PtrToStringAuto(pszString);
                Marshal.FreeCoTaskMem(pszString);
            }
            return filename;
        }

        internal static IShellItem GetShellItemAt(IShellItemArray array, int i)
        {
            IShellItem result;
            var index = (uint) i;
            array.GetItemAt(index, out result);
            return result;
        }

        /// <summary>
        ///   Throws an exception when the dialog is showing preventing
        ///   a requested change to a property or the visible set of controls.
        /// </summary>
        /// <param name = "message">The message to include in the exception.</param>
        /// <permission cref = "System.InvalidOperationException"> The dialog is in an
        ///   invalid state to perform the requested operation.</permission>
        protected void ThrowIfDialogShowing(string message)
        {
            if (NativeDialogShowing)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        ///   Get the IFileDialogCustomize interface, preparing to add controls.
        /// </summary>
        private void GetCustomizedFileDialog()
        {
            if (customize == null)
            {
                if (nativeDialog == null)
                {
                    InitializeNativeFileDialog();
                    nativeDialog = GetNativeFileDialog();
                }
                customize = (IFileDialogCustomize) nativeDialog;
            }
        }

        #endregion

        #region CheckChanged handling members

        /// <summary>
        ///   Raises the <see cref = "CommonFileDialog.FileOk" /> event just before the dialog is about to return with a result.
        /// </summary>
        /// <param name = "e">The event data.</param>
        protected virtual void OnFileOk(CancelEventArgs e)
        {
            CancelEventHandler handler = FileOk;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///   Raises the <see cref = "FolderChanging" /> to stop navigation to a particular location.
        /// </summary>
        /// <param name = "e">Cancelable event arguments.</param>
        protected virtual void OnFolderChanging(CommonFileDialogFolderChangeEventArgs e)
        {
            EventHandler<CommonFileDialogFolderChangeEventArgs> handler = FolderChanging;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///   Raises the <see cref = "CommonFileDialog.FolderChanged" /> event when the user navigates to a new folder.
        /// </summary>
        /// <param name = "e">The event data.</param>
        protected virtual void OnFolderChanged(EventArgs e)
        {
            EventHandler handler = FolderChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///   Raises the <see cref = "CommonFileDialog.SelectionChanged" /> event when the user changes the selection in the dialog's view.
        /// </summary>
        /// <param name = "e">The event data.</param>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            EventHandler handler = SelectionChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///   Raises the <see cref = "CommonFileDialog.FileTypeChanged" /> event when the dialog is opened to notify the 
        ///   application of the initial chosen filetype.
        /// </summary>
        /// <param name = "e">The event data.</param>
        protected virtual void OnFileTypeChanged(EventArgs e)
        {
            EventHandler handler = FileTypeChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///   Raises the <see cref = "CommonFileDialog.DialogOpening" /> event when the dialog is opened.
        /// </summary>
        /// <param name = "e">The event data.</param>
        protected virtual void OnOpening(EventArgs e)
        {
            EventHandler handler = DialogOpening;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        #region NativeDialogEventSink Nested Class

        private class NativeDialogEventSink : IFileDialogEvents, IFileDialogControlEvents
        {
            private bool firstFolderChanged = true;
            private CommonFileDialog parent;

            public NativeDialogEventSink(CommonFileDialog commonDialog)
            {
                parent = commonDialog;
            }

            #region IFileDialogControlEvents Members

            public void OnItemSelected(IFileDialogCustomize pfdc, int dwIDCtl, int dwIDItem)
            {
                // Find control
                DialogControl control = parent.Controls.GetControlbyId(dwIDCtl);

                // Process ComboBox and/or RadioButtonList
                if (control is ICommonFileDialogIndexedControls)
                {
                    // Update selected item and raise SelectedIndexChanged event
                    var controlInterface = control as ICommonFileDialogIndexedControls;
                    controlInterface.SelectedIndex = dwIDItem;
                    controlInterface.RaiseSelectedIndexChangedEvent();
                }
                    // Process Menu
                else if (control is CommonFileDialogMenu)
                {
                    var menu = control as CommonFileDialogMenu;

                    // Find the menu item that was clicked and invoke it's click event
                    foreach (CommonFileDialogMenuItem item in menu.Items)
                    {
                        if (item.Id == dwIDItem)
                        {
                            item.RaiseClickEvent();
                            break;
                        }
                    }
                }
            }

            public void OnButtonClicked(IFileDialogCustomize pfdc, int dwIDCtl)
            {
                // Find control
                DialogControl control = parent.Controls.GetControlbyId(dwIDCtl);

                // Call corresponding event
                if (control is CommonFileDialogButton)
                    ((CommonFileDialogButton) control).RaiseClickEvent();
            }

            public void OnCheckButtonToggled(IFileDialogCustomize pfdc, int dwIDCtl, bool bChecked)
            {
                // Find control
                DialogControl control = parent.Controls.GetControlbyId(dwIDCtl);

                // Update control and call corresponding event
                if (control is CommonFileDialogCheckBox)
                {
                    var box = control as CommonFileDialogCheckBox;
                    box.IsChecked = bChecked;
                    box.RaiseCheckedChangedEvent();
                }
            }

            public void OnControlActivating(IFileDialogCustomize pfdc, int dwIDCtl)
            {
            }

            #endregion

            #region IFileDialogEvents Members

            public HRESULT OnFileOk(IFileDialog pfd)
            {
                var args = new CancelEventArgs();
                parent.OnFileOk(args);

                if (!args.Cancel)
                {
                    // Make sure all custom properties are sync'ed
                    if (parent.Controls != null)
                    {
                        foreach (CommonFileDialogControl control in parent.Controls)
                        {
                            if (control is CommonFileDialogTextBox)
                            {
                                (control as CommonFileDialogTextBox).SyncValue();
                                (control as CommonFileDialogTextBox).Closed = true;
                            }
                                // Also check subcontrols
                            else if (control is CommonFileDialogGroupBox)
                            {
                                var groupbox = control as CommonFileDialogGroupBox;
                                foreach (CommonFileDialogControl subcontrol in groupbox.Items)
                                {
                                    if (subcontrol is CommonFileDialogTextBox)
                                    {
                                        (subcontrol as CommonFileDialogTextBox).SyncValue();
                                        (subcontrol as CommonFileDialogTextBox).Closed = true;
                                    }
                                }
                            }
                        }
                    }
                }

                return (args.Cancel ? HRESULT.S_FALSE : HRESULT.S_OK);
            }

            public HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
            {
                var args = new CommonFileDialogFolderChangeEventArgs(GetFileNameFromShellItem(psiFolder));
                if (!firstFolderChanged)
                    parent.OnFolderChanging(args);
                return (args.Cancel ? HRESULT.S_FALSE : HRESULT.S_OK);
            }

            public void OnFolderChange(IFileDialog pfd)
            {
                if (firstFolderChanged)
                {
                    firstFolderChanged = false;
                    parent.OnOpening(EventArgs.Empty);
                }
                else
                    parent.OnFolderChanged(EventArgs.Empty);
            }

            public void OnSelectionChange(IFileDialog pfd)
            {
                parent.OnSelectionChanged(EventArgs.Empty);
            }

            public void OnShareViolation(IFileDialog pfd, IShellItem psi, out ShellNativeMethods.FDE_SHAREVIOLATION_RESPONSE pResponse)
            {
                // Do nothing: we will ignore share violations, 
                // and don't register
                // for them, so this method should never be called.
                pResponse = ShellNativeMethods.FDE_SHAREVIOLATION_RESPONSE.FDESVR_ACCEPT;
            }

            public void OnTypeChange(IFileDialog pfd)
            {
                parent.OnFileTypeChanged(EventArgs.Empty);
            }

            public void OnOverwrite(IFileDialog pfd, IShellItem psi, out ShellNativeMethods.FDE_OVERWRITE_RESPONSE pResponse)
            {
                // Don't accept or reject the dialog, keep default settings
                pResponse = ShellNativeMethods.FDE_OVERWRITE_RESPONSE.FDEOR_DEFAULT;
            }

            #endregion
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///   Releases the resources used by the current instance of the CommonFileDialog class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        // Template method to allow derived dialog to create actual
        // specific COM coclass (e.g. FileOpenDialog or FileSaveDialog).
        internal abstract void InitializeNativeFileDialog();
        internal abstract IFileDialog GetNativeFileDialog();
        internal abstract void PopulateWithFileNames(Collection<string> names);
        internal abstract void PopulateWithIShellItems(Collection<IShellItem> shellItems);
        internal abstract void CleanUpNativeFileDialog();
        internal abstract ShellNativeMethods.FOS GetDerivedOptionFlags(ShellNativeMethods.FOS flags);

        private static void GenerateNotImplementedException()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        ///   Releases the unmanaged resources used by the CommonFileDialog class and optionally 
        ///   releases the managed resources.
        /// </summary>
        /// <param name = "disposing"><b>true</b> to release both managed and unmanaged resources; 
        ///   <b>false</b> to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
            if (disposing)
                CleanUpNativeFileDialog();
        }

        #region Public API

        // Events.

        private bool addToMruList = true;
        private Guid cookieIdentifier;
        private string defaultDirectory;
        private ShellContainer defaultDirectoryShellContainer;
        private bool ensureFileExists;
        private bool ensurePathExists;
        private bool ensureReadOnly;
        private bool ensureValidNames;

        private string initialDirectory;
        private ShellContainer initialDirectoryShellContainer;
        private bool navigateToShortcut = true;
        private bool restoreDirectory;
        private bool showHiddenItems;
        private bool showPlacesList = true;

        private string title;

        /// <summary>
        ///   Gets the collection of controls for the dialog.
        /// </summary>
        public CommonFileDialogControlCollection<CommonFileDialogControl> Controls { get; private set; }

        /// <summary>
        ///   Gets the filters used by the dialog.
        /// </summary>
        public CommonFileDialogFilterCollection Filters { get; private set; }

        /// <summary>
        ///   Gets or sets the dialog title.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                if (NativeDialogShowing)
                    nativeDialog.SetTitle(value);
            }
        }

        // This is the first of many properties that are backed by the FOS_*
        // bitflag options set with IFileDialog.SetOptions(). 
        // SetOptions() fails
        // if called while dialog is showing (e.g. from a callback).

        /// <summary>
        ///   Gets or sets a value that determines whether the file must exist beforehand.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value. <b>true</b> if the file must exist.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool EnsureFileExists
        {
            get { return ensureFileExists; }
            set
            {
                ThrowIfDialogShowing("EnsureFileExists" + IllegalPropertyChangeString);
                ensureFileExists = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that specifies whether the returned file must be in an existing folder.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value. <b>true</b> if the file must exist.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool EnsurePathExists
        {
            get { return ensurePathExists; }
            set
            {
                ThrowIfDialogShowing("EnsurePathExists" + IllegalPropertyChangeString);
                ensurePathExists = value;
            }
        }

        ///<summary>
        ///  Gets or sets a value that determines whether to validate file names.
        ///</summary>
        ///<value>A <see cref = "System.Boolean" /> value. <b>true </b>to check for situations that would prevent an application from opening the selected file, such as sharing violations or access denied errors.</value>
        ///<exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool EnsureValidNames
        {
            get { return ensureValidNames; }
            set
            {
                ThrowIfDialogShowing("EnsureValidNames" + IllegalPropertyChangeString);
                ensureValidNames = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines whether read-only items are returned.
        ///   Default value for CommonOpenFileDialog is true (allow read-only files) and 
        ///   CommonSaveFileDialog is false (don't allow read-only files).
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value. <b>true</b> includes read-only items.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool EnsureReadOnly
        {
            get { return ensureReadOnly; }
            set
            {
                ThrowIfDialogShowing("EnsureReadOnly" + IllegalPropertyChangeString);
                ensureReadOnly = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines the restore directory.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool RestoreDirectory
        {
            get { return restoreDirectory; }
            set
            {
                ThrowIfDialogShowing("RestoreDirectory" + IllegalPropertyChangeString);
                restoreDirectory = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that controls whether 
        ///   to show or hide the list of pinned places that
        ///   the user can choose.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value. <b>true</b> if the list is visible; otherwise <b>false</b>.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ShowPlaces",
            Justification = "The property is for showing or hiding the _Places_ section in Vista")]
        public bool ShowPlacesList
        {
            get { return showPlacesList; }
            set
            {
                ThrowIfDialogShowing("ShowPlacesList" + IllegalPropertyChangeString);
                showPlacesList = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that controls whether to show or hide the list of places where the user has recently opened or saved items.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool AddToMostRecentlyUsedList
        {
            get { return addToMruList; }
            set
            {
                ThrowIfDialogShowing("AddToMostRecentlyUsedList" + IllegalPropertyChangeString);
                addToMruList = value;
            }
        }

        ///<summary>
        ///  Gets or sets a value that controls whether to show hidden items.
        ///</summary>
        ///<value>A <see cref = "System.Boolean" /> value.<b>true</b> to show the items; otherwise <b>false</b>.</value>
        ///<exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool ShowHiddenItems
        {
            get { return showHiddenItems; }
            set
            {
                ThrowIfDialogShowing("ShowHiddenItems" + IllegalPropertyChangeString);
                showHiddenItems = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that controls whether 
        ///   properties can be edited.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value. </value>
        public bool AllowPropertyEditing { get; set; }

        ///<summary>
        ///  Gets or sets a value that controls whether shortcuts should be treated as their target items, allowing an application to open a .lnk file.
        ///</summary>
        ///<value>A <see cref = "System.Boolean" /> value. <b>true</b> indicates that shortcuts should be treated as their targets. </value>
        ///<exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool NavigateToShortcut
        {
            get { return navigateToShortcut; }
            set
            {
                ThrowIfDialogShowing("NavigateToShortcut" + IllegalPropertyChangeString);
                navigateToShortcut = value;
            }
        }

        /// <summary>
        ///   Gets or sets the default file extension to be added to file names. If the value is null
        ///   or String.Empty, the extension is not added to the file names.
        /// </summary>
        public string DefaultExtension { get; set; }

        /// <summary>
        ///   Gets the index for the currently selected file type.
        /// </summary>
        public int SelectedFileTypeIndex
        {
            get
            {
                if (nativeDialog != null)
                {
                    uint fileType;
                    nativeDialog.GetFileTypeIndex(out fileType);
                    return (int) fileType;
                }

                return -1;
            }
        }

        /// <summary>
        ///   Gets the selected filename.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be used when multiple files are selected.</exception>
        public string FileName
        {
            get
            {
                CheckFileNamesAvailable();

                if (fileNames.Count > 1)
                    throw new InvalidOperationException("Multiple files selected - the FileNames property should be used instead.");

                string returnFilename = fileNames[0];

                // "If extension is a null reference (Nothing in Visual 
                // Basic), the returned string contains the specified 
                // path with its extension removed."  Since we do not want 
                // to remove any existing extension, make sure the 
                // DefaultExtension property is NOT null.

                // if we should, and there is one to set...
                if (!string.IsNullOrEmpty(DefaultExtension))
                    returnFilename = Path.ChangeExtension(returnFilename, DefaultExtension);

                return returnFilename;
            }
        }

        /// <summary>
        ///   Gets the selected item as a ShellObject.
        /// </summary>
        /// <value>A <see cref = "Microsoft.Windows.Shell.ShellObject"></see> object.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be used when multiple files
        ///   are selected.</exception>
        public ShellObject FileAsShellObject
        {
            get
            {
                CheckFileItemsAvailable();

                if (items.Count > 1)
                    throw new InvalidOperationException("Multiple files selected - the Items property should be used instead.");

                return items.Count == 1 ? ShellObjectFactory.Create(items[0]) : null;
            }
        }

        /// <summary>
        ///   Gets or sets the initial directory displayed when the dialog is shown. 
        ///   A null or empty string indicates that the dialog is using the default directory.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string InitialDirectory { get { return initialDirectory; } set { initialDirectory = value; } }

        /// <summary>
        ///   Gets or sets a location that is always selected when the dialog is opened, 
        ///   regardless of previous user action. A null value implies that the dialog is using 
        ///   the default location.
        /// </summary>
        public ShellContainer InitialDirectoryShellContainer { get { return initialDirectoryShellContainer; } set { initialDirectoryShellContainer = value; } }

        /// <summary>
        ///   Sets the folder and path used as a default if there is not a recently used folder value available.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "This is following the native API")]
        public string DefaultDirectory { set { defaultDirectory = value; } }

        /// <summary>
        ///   Sets the location (<see cref = "Microsoft.Windows.Shell.ShellContainer">ShellContainer</see> 
        ///   used as a default if there is not a recently used folder value available.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "This is following the native API")]
        public ShellContainer DefaultDirectoryShellContainer { set { defaultDirectoryShellContainer = value; } }

        // Null = use default identifier.

        /// <summary>
        ///   Gets or sets a value that enables a calling application 
        ///   to associate a GUID with a dialog's persisted state.
        /// </summary>
        public Guid CookieIdentifier { get { return cookieIdentifier; } set { cookieIdentifier = value; } }

        /// <summary>
        ///   Default file name.
        /// </summary>
        public string DefaultFileName { get; set; }

        /// <summary>
        ///   Raised just before the dialog is about to return with a result. Occurs when the user clicks on the Open 
        ///   or Save button on a file dialog box.
        /// </summary>
        public event CancelEventHandler FileOk;

        /// <summary>
        ///   Raised just before the user navigates to a new folder.
        /// </summary>
        public event EventHandler<CommonFileDialogFolderChangeEventArgs> FolderChanging;

        /// <summary>
        ///   Raised when the user navigates to a new folder.
        /// </summary>
        public event EventHandler FolderChanged;

        /// <summary>
        ///   Raised when the user changes the selection in the dialog's view.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        ///   Raised when the dialog is opened to notify the application of the initial chosen filetype.
        /// </summary>
        public event EventHandler FileTypeChanged;

        /// <summary>
        ///   Raised when the dialog is opening.
        /// </summary>
        public event EventHandler DialogOpening;

        /// <summary>
        ///   Tries to set the File(s) Type Combo to match the value in 
        ///   'DefaultExtension'.  Only doing this if 'this' is a Save dialog 
        ///   as it makes no sense to do this if only Opening a file.
        /// </summary>
        /// <param name = "dialog">The native/IFileDialog instance.</param>
        private void SyncFileTypeComboToDefaultExtension(IFileDialog dialog)
        {
            // make sure it's a Save dialog and that there is a default 
            // extension to sync to.
            if (!(this is CommonSaveFileDialog) || DefaultExtension == null || Filters.Count <= 0)
                return;

            // The native version of SetFileTypeIndex() requires an
            // unsigned integer as its parameter. This (having it be defined
            // as a uint right up front) avoids a cast, and the potential 
            // problems of casting a signed value to an unsigned one.
            uint filtersCounter;

            CommonFileDialogFilter filter;

            for (filtersCounter = 0; filtersCounter < Filters.Count; filtersCounter++)
            {
                filter = Filters[(int) filtersCounter];

                if (filter.Extensions.Contains(DefaultExtension))
                {
                    // set the docType combo to match this 
                    // extension. property is a 1-based index.
                    dialog.SetFileTypeIndex(filtersCounter + 1);

                    // we're done, exit for
                    break;
                }
            }
        }

        /// <summary>
        ///   Adds a location, such as a folder, library, search connector, or known folder, to the list of
        ///   places available for a user to open or save items. This method actually adds an item
        ///   to the <b>Favorite Links</b> or <b>Places</b> section of the Open/Save dialog.
        /// </summary>
        /// <param name = "place">The item to add to the places list.</param>
        /// <param name = "location">One of the enumeration values that indicates placement of the item in the list.</param>
        public void AddPlace(ShellContainer place, FileDialogAddPlaceLocation location)
        {
            // Get our native dialog
            if (nativeDialog == null)
            {
                InitializeNativeFileDialog();
                nativeDialog = GetNativeFileDialog();
            }

            // Add the shellitem to the places list
            if (nativeDialog != null)
                nativeDialog.AddPlace(place.NativeShellItem, (ShellNativeMethods.FDAP) location);
        }

        /// <summary>
        ///   Adds a location (folder, library, search connector, known folder) to the list of
        ///   places available for the user to open or save items. This method actually adds an item
        ///   to the <b>Favorite Links</b> or <b>Places</b> section of the Open/Save dialog. Overload method
        ///   takes in a string for the path.
        /// </summary>
        /// <param name = "path">The item to add to the places list.</param>
        /// <param name = "location">One of the enumeration values that indicates placement of the item in the list.</param>
        public void AddPlace(string path, FileDialogAddPlaceLocation location)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            // Get our native dialog
            if (nativeDialog == null)
            {
                InitializeNativeFileDialog();
                nativeDialog = GetNativeFileDialog();
            }

            // Create a native shellitem from our path
            IShellItem2 nativeShellItem;
            var guid = new Guid(ShellIIDGuid.IShellItem2);
            int retCode = ShellNativeMethods.SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out nativeShellItem);

            if (!CoreErrorHelper.Succeeded(retCode))
                throw new ExternalException("Shell item could not be created.", Marshal.GetExceptionForHR(retCode));

            // Add the shellitem to the places list
            if (nativeDialog != null)
                nativeDialog.AddPlace(nativeShellItem, (ShellNativeMethods.FDAP) location);
        }

        /// <summary>
        ///   Displays the dialog.
        /// </summary>
        /// <param name = "ownerWindowHandle">Window handle of any top-level window that will own the modal dialog box.</param>
        /// <returns>A <see cref = "CommonFileDialogResult" /> object.</returns>
        public CommonFileDialogResult ShowDialog(IntPtr ownerWindowHandle)
        {
            if (ownerWindowHandle == IntPtr.Zero)
                throw new ArgumentException("ownerWindowHandle");

            // Set the parent / owner window
            parentWindow = ownerWindowHandle;

            // Show the modal dialog
            return ShowDialog();
        }

        /// <summary>
        ///   Displays the dialog.
        /// </summary>
        /// <param name = "window">Top-level WPF window that will own the modal dialog box.</param>
        /// <returns>A <see cref = "CommonFileDialogResult" /> object.</returns>
        public CommonFileDialogResult ShowDialog(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            // Set the parent / owner window
            parentWindow = (new WindowInteropHelper(window)).Handle;

            // Show the modal dialog
            return ShowDialog();
        }

        /// <summary>
        ///   Displays the dialog.
        /// </summary>
        /// <returns>A <see cref = "CommonFileDialogResult" /> object.</returns>
        public CommonFileDialogResult ShowDialog()
        {
            CommonFileDialogResult result;

            // Fetch derived native dialog (i.e. Save or Open).
            InitializeNativeFileDialog();
            nativeDialog = GetNativeFileDialog();

            // Apply outer properties to native dialog instance.
            ApplyNativeSettings(nativeDialog);
            InitializeEventSink(nativeDialog);

            // Clear user data if Reset has been called 
            // since the last show.
            if (resetSelections)
                resetSelections = false;

            // Show dialog.
            showState = DialogShowState.Showing;
            int hresult = nativeDialog.Show(parentWindow);
            showState = DialogShowState.Closed;

            // Create return information.
            if (CoreErrorHelper.Matches(hresult, (int) HRESULT.ERROR_CANCELLED))
            {
                canceled = true;
                result = CommonFileDialogResult.Cancel;
                fileNames.Clear();
            }
            else
            {
                canceled = false;
                result = CommonFileDialogResult.OK;

                // Populate filenames if user didn't cancel.
                PopulateWithFileNames(fileNames);

                // Populate the actual IShellItems
                PopulateWithIShellItems(items);
            }

            return result;
        }

        /// <summary>
        ///   Removes the current selection.
        /// </summary>
        public void ResetUserSelections()
        {
            resetSelections = true;
        }

        #endregion

        #region Configuration

        private void InitializeEventSink(IFileDialog nativeDlg)
        {
            // Check if we even need to have a sink.
            if (FileOk != null || FolderChanging != null || FolderChanged != null || SelectionChanged != null || FileTypeChanged != null || DialogOpening != null ||
                (Controls != null && Controls.Count > 0))
            {
                uint cookie;
                nativeEventSink = new NativeDialogEventSink(this);
                nativeDlg.Advise(nativeEventSink, out cookie);
            }
        }

        private void ApplyNativeSettings(IFileDialog dialog)
        {
            Debug.Assert(dialog != null, "No dialog instance to configure");

            if (parentWindow == IntPtr.Zero)
            {
                if (Application.Current != null && Application.Current.MainWindow != null)
                    parentWindow = (new WindowInteropHelper(Application.Current.MainWindow)).Handle;
            }

            var guid = new Guid(ShellIIDGuid.IShellItem2);

            // Apply option bitflags.
            dialog.SetOptions(CalculateNativeDialogOptionFlags());

            // Other property sets.
            if (title != null)
                dialog.SetTitle(title);
            if (initialDirectoryShellContainer != null)
                dialog.SetFolder(initialDirectoryShellContainer.NativeShellItem);
            if (defaultDirectoryShellContainer != null)
                dialog.SetDefaultFolder(defaultDirectoryShellContainer.NativeShellItem);
            if (!String.IsNullOrEmpty(initialDirectory))
            {
                // Create a native shellitem from our path
                IShellItem2 initialDirectoryShellItem;
                ShellNativeMethods.SHCreateItemFromParsingName(initialDirectory, IntPtr.Zero, ref guid, out initialDirectoryShellItem);

                // If we get a real shell item back, 
                // then use that as the initial folder - otherwise,
                // we'll allow the dialog to revert to the default folder. 
                // (OR should we fail loudly?)
                if (initialDirectoryShellItem != null)
                    dialog.SetFolder(initialDirectoryShellItem);
            }
            if (!string.IsNullOrEmpty(defaultDirectory))
            {
                // Create a native shellitem from our path
                IShellItem2 defaultDirectoryShellItem;
                ShellNativeMethods.SHCreateItemFromParsingName(defaultDirectory, IntPtr.Zero, ref guid, out defaultDirectoryShellItem);

                // If we get a real shell item back, 
                // then use that as the initial folder - otherwise,
                // we'll allow the dialog to revert to the default folder. 
                // (OR should we fail loudly?)
                if (defaultDirectoryShellItem != null)
                    dialog.SetDefaultFolder(defaultDirectoryShellItem);
            }

            // Apply file type filters, if available.
            if (Filters.Count > 0 && !filterSet)
            {
                dialog.SetFileTypes((uint) Filters.Count, Filters.GetAllFilterSpecs());

                filterSet = true;

                SyncFileTypeComboToDefaultExtension(dialog);
            }

            if (cookieIdentifier != Guid.Empty)
                dialog.SetClientGuid(ref cookieIdentifier);

            // Set the default extension
            if (!string.IsNullOrEmpty(DefaultExtension))
                dialog.SetDefaultExtension(DefaultExtension);

            // Set the default filename
            dialog.SetFileName(DefaultFileName);
        }

        private ShellNativeMethods.FOS CalculateNativeDialogOptionFlags()
        {
            // We start with only a few flags set by default, 
            // then go from there based on the current state
            // of the managed dialog's property values.
            ShellNativeMethods.FOS flags = ShellNativeMethods.FOS.FOS_NOTESTFILECREATE;

            // Call to derived (concrete) dialog to 
            // set dialog-specific flags.
            flags = GetDerivedOptionFlags(flags);

            // Apply other optional flags.
            if (ensureFileExists)
                flags |= ShellNativeMethods.FOS.FOS_FILEMUSTEXIST;
            if (ensurePathExists)
                flags |= ShellNativeMethods.FOS.FOS_PATHMUSTEXIST;
            if (!ensureValidNames)
                flags |= ShellNativeMethods.FOS.FOS_NOVALIDATE;
            if (!EnsureReadOnly)
                flags |= ShellNativeMethods.FOS.FOS_NOREADONLYRETURN;
            if (restoreDirectory)
                flags |= ShellNativeMethods.FOS.FOS_NOCHANGEDIR;
            if (!showPlacesList)
                flags |= ShellNativeMethods.FOS.FOS_HIDEPINNEDPLACES;
            if (!addToMruList)
                flags |= ShellNativeMethods.FOS.FOS_DONTADDTORECENT;
            if (showHiddenItems)
                flags |= ShellNativeMethods.FOS.FOS_FORCESHOWHIDDEN;
            if (!navigateToShortcut)
                flags |= ShellNativeMethods.FOS.FOS_NODEREFERENCELINKS;
            return flags;
        }

        #endregion
    }
}