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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Markup;

    using Microsoft.Windows.Dialogs.Controls;
    using Microsoft.Windows.Internal;
    using Microsoft.Windows.Shell;

    /// <summary>
    /// Defines the abstract base class for the common file dialogs.
    /// </summary>
    [ContentProperty("Controls")]
    public abstract class CommonFileDialog : IDialogControlHost, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal readonly Collection<IShellItem> items;

        /// <summary>
        /// </summary>
        internal DialogShowState showState = DialogShowState.PreShow;

        /// <summary>
        ///   Contains a common error message string shared by classes that 
        ///   inherit from this class.
        /// </summary>
        protected const string IllegalPropertyChangeString = " cannot be changed while dialog is showing";

        /// <summary>
        ///   The collection of names selected by the user.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", 
            Justification = "This is an internal field used by the CommonOpenFileDialog and possibly other dialogs deriving from this base class.")]
        protected readonly Collection<string> fileNames;

        // Events.

        /// <summary>
        /// </summary>
        private bool addToMruList = true;

        /// <summary>
        /// </summary>
        private bool? canceled;

        /// <summary>
        /// </summary>
        private Guid cookieIdentifier;

        /// <summary>
        /// </summary>
        private IFileDialogCustomize customize;

        /// <summary>
        /// </summary>
        private string defaultDirectory;

        /// <summary>
        /// </summary>
        private ShellContainer defaultDirectoryShellContainer;

        /// <summary>
        /// </summary>
        private bool ensureFileExists;

        /// <summary>
        /// </summary>
        private bool ensurePathExists;

        /// <summary>
        /// </summary>
        private bool ensureReadOnly;

        /// <summary>
        /// </summary>
        private bool ensureValidNames;

        /// <summary>
        /// </summary>
        private bool filterSet; // filters can be set only once

        /// <summary>
        /// </summary>
        private string initialDirectory;

        /// <summary>
        /// </summary>
        private ShellContainer initialDirectoryShellContainer;

        /// <summary>
        /// </summary>
        private IFileDialog nativeDialog;

        /// <summary>
        /// </summary>
        private NativeDialogEventSink nativeEventSink;

        /// <summary>
        /// </summary>
        private bool navigateToShortcut = true;

        /// <summary>
        /// </summary>
        private IntPtr parentWindow = IntPtr.Zero;

        /// <summary>
        /// </summary>
        private bool resetSelections;

        /// <summary>
        /// </summary>
        private bool restoreDirectory;

        /// <summary>
        /// </summary>
        private bool showHiddenItems;

        /// <summary>
        /// </summary>
        private bool showPlacesList = true;

        /// <summary>
        /// </summary>
        private string title;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialog()
        {
            if (!CoreHelpers.RunningOnVista)
            {
                throw new PlatformNotSupportedException("Common File Dialog requires Windows Vista or later.");
            }

            this.fileNames = new Collection<string>();
            this.Filters = new CommonFileDialogFilterCollection();
            this.items = new Collection<IShellItem>();
            this.Controls = new CommonFileDialogControlCollection<CommonFileDialogControl>(this);
        }

        /// <summary>
        /// Creates a new instance of this class with the specified title.
        /// </summary>
        /// <param name="title">
        /// The title to display in the dialog.
        /// </param>
        protected CommonFileDialog(string title)
            : this()
        {
            this.title = title;
        }

        /// <summary>
        /// </summary>
        ~CommonFileDialog()
        {
            this.Dispose(false);
        }

        #endregion

        #region Events

        /// <summary>
        ///   Raised when the dialog is opening.
        /// </summary>
        public event EventHandler DialogOpening;

        /// <summary>
        ///   Raised just before the dialog is about to return with a result. Occurs when the user clicks on the Open 
        ///   or Save button on a file dialog box.
        /// </summary>
        public event CancelEventHandler FileOk;

        /// <summary>
        ///   Raised when the dialog is opened to notify the application of the initial chosen filetype.
        /// </summary>
        public event EventHandler FileTypeChanged;

        /// <summary>
        ///   Raised when the user navigates to a new folder.
        /// </summary>
        public event EventHandler FolderChanged;

        /// <summary>
        ///   Raised just before the user navigates to a new folder.
        /// </summary>
        public event EventHandler<CommonFileDialogFolderChangeEventArgs> FolderChanging;

        /// <summary>
        ///   Raised when the user changes the selection in the dialog's view.
        /// </summary>
        public event EventHandler SelectionChanged;

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
        ///   Gets or sets a value that controls whether to show or hide the list of places where the user has recently opened or saved items.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool AddToMostRecentlyUsedList
        {
            get
            {
                return this.addToMruList;
            }

            set
            {
                this.ThrowIfDialogShowing("AddToMostRecentlyUsedList" + IllegalPropertyChangeString);
                this.addToMruList = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that controls whether 
        ///   properties can be edited.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value. </value>
        public bool AllowPropertyEditing { get; set; }

        /// <summary>
        ///   Gets the collection of controls for the dialog.
        /// </summary>
        public CommonFileDialogControlCollection<CommonFileDialogControl> Controls { get; private set; }

        /// <summary>
        ///   Gets or sets a value that enables a calling application 
        ///   to associate a GUID with a dialog's persisted state.
        /// </summary>
        public Guid CookieIdentifier
        {
            get
            {
                return this.cookieIdentifier;
            }

            set
            {
                this.cookieIdentifier = value;
            }
        }

        /// <summary>
        ///   Sets the folder and path used as a default if there is not a recently used folder value available.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "This is following the native API")]
        public string DefaultDirectory
        {
            set
            {
                this.defaultDirectory = value;
            }
        }

        /// <summary>
        ///   Sets the location (<see cref = "Microsoft.Windows.Shell.ShellContainer">ShellContainer</see> 
        ///   used as a default if there is not a recently used folder value available.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "This is following the native API")]
        public ShellContainer DefaultDirectoryShellContainer
        {
            set
            {
                this.defaultDirectoryShellContainer = value;
            }
        }

        /// <summary>
        ///   Gets or sets the default file extension to be added to file names. If the value is null
        ///   or String.Empty, the extension is not added to the file names.
        /// </summary>
        public string DefaultExtension { get; set; }

        /// <summary>
        ///   Default file name.
        /// </summary>
        public string DefaultFileName { get; set; }

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
            get
            {
                return this.ensureFileExists;
            }

            set
            {
                this.ThrowIfDialogShowing("EnsureFileExists" + IllegalPropertyChangeString);
                this.ensureFileExists = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that specifies whether the returned file must be in an existing folder.
        /// </summary>
        /// <value>A <see cref = "System.Boolean" /> value. <b>true</b> if the file must exist.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool EnsurePathExists
        {
            get
            {
                return this.ensurePathExists;
            }

            set
            {
                this.ThrowIfDialogShowing("EnsurePathExists" + IllegalPropertyChangeString);
                this.ensurePathExists = value;
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
            get
            {
                return this.ensureReadOnly;
            }

            set
            {
                this.ThrowIfDialogShowing("EnsureReadOnly" + IllegalPropertyChangeString);
                this.ensureReadOnly = value;
            }
        }

        ///<summary>
        ///  Gets or sets a value that determines whether to validate file names.
        ///</summary>
        ///<value>A <see cref = "System.Boolean" /> value. <b>true </b>to check for situations that would prevent an application from opening the selected file, such as sharing violations or access denied errors.</value>
        ///<exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool EnsureValidNames
        {
            get
            {
                return this.ensureValidNames;
            }

            set
            {
                this.ThrowIfDialogShowing("EnsureValidNames" + IllegalPropertyChangeString);
                this.ensureValidNames = value;
            }
        }

        /// <summary>
        ///   Gets the selected item as a ShellObject.
        /// </summary>
        /// <value>A <see cref = "Microsoft.Windows.Shell.ShellObject" /> object.</value>
        /// <exception cref = "System.InvalidOperationException">This property cannot be used when multiple files
        ///   are selected.</exception>
        public ShellObject FileAsShellObject
        {
            get
            {
                this.CheckFileItemsAvailable();

                if (this.items.Count > 1)
                {
                    throw new InvalidOperationException("Multiple files selected - the Items property should be used instead.");
                }

                return this.items.Count == 1 ? ShellObjectFactory.Create(this.items[0]) : null;
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
                this.CheckFileNamesAvailable();

                if (this.fileNames.Count > 1)
                {
                    throw new InvalidOperationException("Multiple files selected - the FileNames property should be used instead.");
                }

                var returnFilename = this.fileNames[0];

                // "If extension is a null reference (Nothing in Visual 
                // Basic), the returned string contains the specified 
                // path with its extension removed."  Since we do not want 
                // to remove any existing extension, make sure the 
                // DefaultExtension property is NOT null.

                // if we should, and there is one to set...
                if (!string.IsNullOrEmpty(this.DefaultExtension))
                {
                    returnFilename = Path.ChangeExtension(returnFilename, this.DefaultExtension);
                }

                return returnFilename;
            }
        }

        /// <summary>
        ///   Gets the filters used by the dialog.
        /// </summary>
        public CommonFileDialogFilterCollection Filters { get; private set; }

        /// <summary>
        ///   Gets or sets the initial directory displayed when the dialog is shown. 
        ///   A null or empty string indicates that the dialog is using the default directory.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string InitialDirectory
        {
            get
            {
                return this.initialDirectory;
            }

            set
            {
                this.initialDirectory = value;
            }
        }

        /// <summary>
        ///   Gets or sets a location that is always selected when the dialog is opened, 
        ///   regardless of previous user action. A null value implies that the dialog is using 
        ///   the default location.
        /// </summary>
        public ShellContainer InitialDirectoryShellContainer
        {
            get
            {
                return this.initialDirectoryShellContainer;
            }

            set
            {
                this.initialDirectoryShellContainer = value;
            }
        }

        ///<summary>
        ///  Gets or sets a value that controls whether shortcuts should be treated as their target items, allowing an application to open a .lnk file.
        ///</summary>
        ///<value>A <see cref = "System.Boolean" /> value. <b>true</b> indicates that shortcuts should be treated as their targets. </value>
        ///<exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool NavigateToShortcut
        {
            get
            {
                return this.navigateToShortcut;
            }

            set
            {
                this.ThrowIfDialogShowing("NavigateToShortcut" + IllegalPropertyChangeString);
                this.navigateToShortcut = value;
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
            get
            {
                return this.restoreDirectory;
            }

            set
            {
                this.ThrowIfDialogShowing("RestoreDirectory" + IllegalPropertyChangeString);
                this.restoreDirectory = value;
            }
        }

        /// <summary>
        ///   Gets the index for the currently selected file type.
        /// </summary>
        public int SelectedFileTypeIndex
        {
            get
            {
                if (this.nativeDialog != null)
                {
                    uint fileType;
                    this.nativeDialog.GetFileTypeIndex(out fileType);
                    return (int)fileType;
                }

                return -1;
            }
        }

        ///<summary>
        ///  Gets or sets a value that controls whether to show hidden items.
        ///</summary>
        ///<value>A <see cref = "System.Boolean" /> value.<b>true</b> to show the items; otherwise <b>false</b>.</value>
        ///<exception cref = "System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool ShowHiddenItems
        {
            get
            {
                return this.showHiddenItems;
            }

            set
            {
                this.ThrowIfDialogShowing("ShowHiddenItems" + IllegalPropertyChangeString);
                this.showHiddenItems = value;
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
            get
            {
                return this.showPlacesList;
            }

            set
            {
                this.ThrowIfDialogShowing("ShowPlacesList" + IllegalPropertyChangeString);
                this.showPlacesList = value;
            }
        }

        /// <summary>
        ///   Gets or sets the dialog title.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.SetTitle(value);
                }
            }
        }

        /// <summary>
        /// </summary>
        private bool NativeDialogShowing
        {
            get
            {
                return (this.nativeDialog != null) && (this.showState == DialogShowState.Showing || this.showState == DialogShowState.Closing);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a location, such as a folder, library, search connector, or known folder, to the list of
        ///   places available for a user to open or save items. This method actually adds an item
        ///   to the <b>Favorite Links</b> or <b>Places</b> section of the Open/Save dialog.
        /// </summary>
        /// <param name="place">
        /// The item to add to the places list.
        /// </param>
        /// <param name="location">
        /// One of the enumeration values that indicates placement of the item in the list.
        /// </param>
        public void AddPlace(ShellContainer place, FileDialogAddPlaceLocation location)
        {
            // Get our native dialog
            if (this.nativeDialog == null)
            {
                this.InitializeNativeFileDialog();
                this.nativeDialog = this.GetNativeFileDialog();
            }

            // Add the shellitem to the places list
            if (this.nativeDialog != null)
            {
                this.nativeDialog.AddPlace(place.NativeShellItem, (ShellNativeMethods.FDAP)location);
            }
        }

        /// <summary>
        /// Adds a location (folder, library, search connector, known folder) to the list of
        ///   places available for the user to open or save items. This method actually adds an item
        ///   to the <b>Favorite Links</b> or <b>Places</b> section of the Open/Save dialog. Overload method
        ///   takes in a string for the path.
        /// </summary>
        /// <param name="path">
        /// The item to add to the places list.
        /// </param>
        /// <param name="location">
        /// One of the enumeration values that indicates placement of the item in the list.
        /// </param>
        public void AddPlace(string path, FileDialogAddPlaceLocation location)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            // Get our native dialog
            if (this.nativeDialog == null)
            {
                this.InitializeNativeFileDialog();
                this.nativeDialog = this.GetNativeFileDialog();
            }

            // Create a native shellitem from our path
            IShellItem2 nativeShellItem;
            var guid = new Guid(ShellIidGuid.IShellItem2);
            var retCode = ShellNativeMethods.SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out nativeShellItem);

            if (!CoreErrorHelper.Succeeded(retCode))
            {
                throw new ExternalException("Shell item could not be created.", Marshal.GetExceptionForHR(retCode));
            }

            // Add the shellitem to the places list
            if (this.nativeDialog != null)
            {
                this.nativeDialog.AddPlace(nativeShellItem, (ShellNativeMethods.FDAP)location);
            }
        }

        /// <summary>
        /// Removes the current selection.
        /// </summary>
        public void ResetUserSelections()
        {
            this.resetSelections = true;
        }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        /// <param name="ownerWindowHandle">
        /// Window handle of any top-level window that will own the modal dialog box.
        /// </param>
        /// <returns>
        /// A <see cref="CommonFileDialogResult"/> object.
        /// </returns>
        public CommonFileDialogResult ShowDialog(IntPtr ownerWindowHandle)
        {
            if (ownerWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException("ownerWindowHandle");
            }

            // Set the parent / owner window
            this.parentWindow = ownerWindowHandle;

            // Show the modal dialog
            return this.ShowDialog();
        }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        /// <param name="window">
        /// Top-level WPF window that will own the modal dialog box.
        /// </param>
        /// <returns>
        /// A <see cref="CommonFileDialogResult"/> object.
        /// </returns>
        public CommonFileDialogResult ShowDialog(Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            // Set the parent / owner window
            this.parentWindow = (new WindowInteropHelper(window)).Handle;

            // Show the modal dialog
            return this.ShowDialog();
        }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        /// <returns>
        /// A <see cref="CommonFileDialogResult"/> object.
        /// </returns>
        public CommonFileDialogResult ShowDialog()
        {
            CommonFileDialogResult result;

            // Fetch derived native dialog (i.e. Save or Open).
            this.InitializeNativeFileDialog();
            this.nativeDialog = this.GetNativeFileDialog();

            // Apply outer properties to native dialog instance.
            this.ApplyNativeSettings(this.nativeDialog);
            this.InitializeEventSink(this.nativeDialog);

            // Clear user data if Reset has been called 
            // since the last show.
            if (this.resetSelections)
            {
                this.resetSelections = false;
            }

            // Show dialog.
            this.showState = DialogShowState.Showing;
            var hresult = this.nativeDialog.Show(this.parentWindow);
            this.showState = DialogShowState.Closed;

            // Create return information.
            if (CoreErrorHelper.Matches(hresult, (int)HRESULT.ErrorCancelled))
            {
                this.canceled = true;
                result = CommonFileDialogResult.Cancel;
                this.fileNames.Clear();
            }
            else
            {
                this.canceled = false;
                result = CommonFileDialogResult.OK;

                // Populate filenames if user didn't cancel.
                this.PopulateWithFileNames(this.fileNames);

                // Populate the actual IShellItems
                this.PopulateWithIShellItems(this.items);
            }

            return result;
        }

        #endregion

        #region Implemented Interfaces

        #region IDialogControlHost

        /// <summary>
        /// </summary>
        void IDialogControlHost.ApplyCollectionChanged()
        {
            // Query IFileDialogCustomize interface before adding controls
            this.GetCustomizedFileDialog();

            // Populate all the custom controls and add them to the dialog
            foreach (var control in this.Controls.Where(control => !control.IsAdded))
            {
                control.HostingDialog = this;
                control.Attach(this.customize);
                control.IsAdded = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        /// <param name="control">
        /// </param>
        void IDialogControlHost.ApplyControlPropertyChange(string propertyName, DialogControl control)
        {
            switch (propertyName)
            {
                case "Text":
                    if (control is CommonFileDialogTextBox)
                    {
                        this.customize.SetEditBoxText(control.Id, ((CommonFileDialogControl)control).Text);
                    }
                    else
                    {
                        this.customize.SetControlLabel(control.Id, ((CommonFileDialogControl)control).Text);
                    }

                    break;
                case "Visible":
                    {
                        var dialogControl = control as CommonFileDialogControl;
                        ShellNativeMethods.DCONTROLSTATE state;

                        this.customize.GetControlState(control.Id, out state);

                        if (dialogControl.Visible)
                        {
                            state |= ShellNativeMethods.DCONTROLSTATE.CdcsVisible;
                        }
                        else if (dialogControl.Visible == false)
                        {
                            state &= ~ShellNativeMethods.DCONTROLSTATE.CdcsVisible;
                        }

                        this.customize.SetControlState(control.Id, state);
                    }

                    break;
                case "Enabled":
                    {
                        var dialogControl = control as CommonFileDialogControl;
                        ShellNativeMethods.DCONTROLSTATE state;

                        this.customize.GetControlState(control.Id, out state);

                        if (dialogControl.Enabled)
                        {
                            state |= ShellNativeMethods.DCONTROLSTATE.CdcsEnabled;
                        }
                        else if (dialogControl.Enabled == false)
                        {
                            state &= ~ShellNativeMethods.DCONTROLSTATE.CdcsEnabled;
                        }

                        this.customize.SetControlState(control.Id, state);
                    }

                    break;
                case "SelectedIndex":
                    if (control is CommonFileDialogRadioButtonList)
                    {
                        var list = control as CommonFileDialogRadioButtonList;
                        this.customize.SetSelectedControlItem(control.Id, list.SelectedIndex);
                    }
                    else if (control is CommonFileDialogComboBox)
                    {
                        var box = control as CommonFileDialogComboBox;
                        this.customize.SetSelectedControlItem(control.Id, box.SelectedIndex);
                    }

                    break;
                case "IsChecked":
                    if (control is CommonFileDialogCheckBox)
                    {
                        var checkBox = control as CommonFileDialogCheckBox;
                        this.customize.SetCheckButtonState(control.Id, checkBox.IsChecked);
                    }

                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        bool IDialogControlHost.IsCollectionChangeAllowed()
        {
            return true;
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
            GenerateNotImplementedException();
            return false;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases the resources used by the current instance of the CommonFileDialog class.
        /// </summary>
        public void Dispose()
        {
            // COMMENTED BY CODEIT.RIGHT
            // this.Dispose(true);
            this.Dispose(true);

            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        internal static string GetFileNameFromShellItem(IShellItem item)
        {
            string filename = null;
            IntPtr pszString;
            var hr = item.GetDisplayName(ShellNativeMethods.SIGDN.Desktopabsoluteparsing, out pszString);
            if (hr == HRESULT.S_OK && pszString != IntPtr.Zero)
            {
                filename = Marshal.PtrToStringAuto(pszString);
                Marshal.FreeCoTaskMem(pszString);
            }

            return filename;
        }

        /// <summary>
        /// </summary>
        /// <param name="array">
        /// </param>
        /// <param name="i">
        /// </param>
        /// <returns>
        /// </returns>
        internal static IShellItem GetShellItemAt(IShellItemArray array, int i)
        {
            IShellItem result;
            var index = (uint)i;
            array.GetItemAt(index, out result);
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="flags">
        /// </param>
        /// <returns>
        /// </returns>
        internal abstract ShellNativeMethods.FOSs GetDerivedOptionFlags(ShellNativeMethods.FOSs flags);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        internal abstract IFileDialog GetNativeFileDialog();

        /// <summary>
        /// </summary>
        /// <param name="shellItems">
        /// </param>
        internal abstract void PopulateWithIShellItems(Collection<IShellItem> shellItems);

        /// <summary>
        /// </summary>
        protected virtual void ApplyCollectionChanged()
        {
            ((IDialogControlHost)this).ApplyCollectionChanged();
        }

        /// <summary>
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        /// <param name="control">
        /// </param>
        protected virtual void ApplyControlPropertyChange(string propertyName, DialogControl control)
        {
            ((IDialogControlHost)this).ApplyControlPropertyChange(propertyName, control);
        }

        /// <summary>
        /// Ensures that the user has selected one or more files.
        /// </summary>
        /// <permission cref="System.InvalidOperationException">
        /// The dialog has not been dismissed yet or the dialog was cancelled.
        /// </permission>
        protected void CheckFileItemsAvailable()
        {
            if (this.showState != DialogShowState.Closed)
            {
                throw new InvalidOperationException("Filename not available - dialog has not closed yet.");
            }

            if (this.canceled.GetValueOrDefault())
            {
                throw new InvalidOperationException("Filename not available - dialog was canceled.");
            }

            Debug.Assert(this.items.Count != 0, "Items list empty - shouldn't happen unless dialog canceled or not yet shown.");
        }

        /// <summary>
        /// Ensures that the user has selected one or more files.
        /// </summary>
        /// <permission cref="System.InvalidOperationException">
        /// The dialog has not been dismissed yet or the dialog was cancelled.
        /// </permission>
        protected void CheckFileNamesAvailable()
        {
            if (this.showState != DialogShowState.Closed)
            {
                throw new InvalidOperationException("Filename not available - dialog has not closed yet.");
            }

            if (this.canceled.GetValueOrDefault())
            {
                throw new InvalidOperationException("Filename not available - dialog was canceled.");
            }

            Debug.Assert(this.fileNames.Count != 0, "FileNames empty - shouldn't happen unless dialog canceled or not yet shown.");
        }

        /// <summary>
        /// </summary>
        protected abstract void CleanUpNativeFileDialog();

        /// <summary>
        /// Releases the unmanaged resources used by the CommonFileDialog class and optionally 
        ///   releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <b>true</b> to release both managed and unmanaged resources; 
        ///   <b>false</b> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.CleanUpNativeFileDialog();
            }
        }

        /// <summary>
        /// </summary>
        protected abstract void InitializeNativeFileDialog();

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        protected virtual bool IsCollectionChangeAllowed()
        {
            return ((IDialogControlHost)this).IsCollectionChangeAllowed();
        }

        /// <summary>
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        /// <param name="control">
        /// </param>
        /// <returns>
        /// </returns>
        protected virtual bool IsControlPropertyChangeAllowed(string propertyName, DialogControl control)
        {
            return ((IDialogControlHost)this).IsControlPropertyChangeAllowed(propertyName, control);
        }

        /// <summary>
        /// Raises the <see cref="CommonFileDialog.FileOk"/> event just before the dialog is about to return with a result.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        protected void OnFileOk(CancelEventArgs e)
        {
            var handler = this.FileOk;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="names">
        /// </param>
        protected abstract void PopulateWithFileNames(Collection<string> names);

        /// <summary>
        /// Throws an exception when the dialog is showing preventing
        ///   a requested change to a property or the visible set of controls.
        /// </summary>
        /// <param name="message">
        /// The message to include in the exception.
        /// </param>
        /// <permission cref="System.InvalidOperationException">
        /// The dialog is in an
        ///   invalid state to perform the requested operation.
        /// </permission>
        protected void ThrowIfDialogShowing(string message)
        {
            if (this.NativeDialogShowing)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private static void GenerateNotImplementedException()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// </summary>
        /// <param name="dialog">
        /// </param>
        private void ApplyNativeSettings(IFileDialog dialog)
        {
            Debug.Assert(dialog != null, "No dialog instance to configure");

            if (this.parentWindow == IntPtr.Zero)
            {
                if (Application.Current != null && Application.Current.MainWindow != null)
                {
                    this.parentWindow = (new WindowInteropHelper(Application.Current.MainWindow)).Handle;
                }
            }

            var guid = new Guid(ShellIidGuid.IShellItem2);

            // Apply option bitflags.
            dialog.SetOptions(this.CalculateNativeDialogOptionFlags());

            // Other property sets.
            if (this.title != null)
            {
                dialog.SetTitle(this.title);
            }

            if (this.initialDirectoryShellContainer != null)
            {
                dialog.SetFolder(this.initialDirectoryShellContainer.NativeShellItem);
            }

            if (this.defaultDirectoryShellContainer != null)
            {
                dialog.SetDefaultFolder(this.defaultDirectoryShellContainer.NativeShellItem);
            }

            if (!String.IsNullOrEmpty(this.initialDirectory))
            {
                // Create a native shellitem from our path
                IShellItem2 initialDirectoryShellItem;
                ShellNativeMethods.SHCreateItemFromParsingName(this.initialDirectory, IntPtr.Zero, ref guid, out initialDirectoryShellItem);

                // If we get a real shell item back, 
                // then use that as the initial folder - otherwise,
                // we'll allow the dialog to revert to the default folder. 
                // (OR should we fail loudly?)
                if (initialDirectoryShellItem != null)
                {
                    dialog.SetFolder(initialDirectoryShellItem);
                }
            }

            if (!string.IsNullOrEmpty(this.defaultDirectory))
            {
                // Create a native shellitem from our path
                IShellItem2 defaultDirectoryShellItem;
                ShellNativeMethods.SHCreateItemFromParsingName(this.defaultDirectory, IntPtr.Zero, ref guid, out defaultDirectoryShellItem);

                // If we get a real shell item back, 
                // then use that as the initial folder - otherwise,
                // we'll allow the dialog to revert to the default folder. 
                // (OR should we fail loudly?)
                if (defaultDirectoryShellItem != null)
                {
                    dialog.SetDefaultFolder(defaultDirectoryShellItem);
                }
            }

            // Apply file type filters, if available.
            if (this.Filters.Count > 0 && !this.filterSet)
            {
                dialog.SetFileTypes((uint)this.Filters.Count, this.Filters.GetAllFilterSpecs());

                this.filterSet = true;

                this.SyncFileTypeComboToDefaultExtension(dialog);
            }

            if (this.cookieIdentifier != Guid.Empty)
            {
                dialog.SetClientGuid(ref this.cookieIdentifier);
            }

            // Set the default extension
            if (!string.IsNullOrEmpty(this.DefaultExtension))
            {
                dialog.SetDefaultExtension(this.DefaultExtension);
            }

            // Set the default filename
            dialog.SetFileName(this.DefaultFileName);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private ShellNativeMethods.FOSs CalculateNativeDialogOptionFlags()
        {
            // We start with only a few flags set by default, 
            // then go from there based on the current state
            // of the managed dialog's property values.
            var flags = ShellNativeMethods.FOSs.FosNotestfilecreate;

            // Call to derived (concrete) dialog to 
            // set dialog-specific flags.
            flags = this.GetDerivedOptionFlags(flags);

            // Apply other optional flags.
            if (this.ensureFileExists)
            {
                flags |= ShellNativeMethods.FOSs.FosFilemustexist;
            }

            if (this.ensurePathExists)
            {
                flags |= ShellNativeMethods.FOSs.FosPathmustexist;
            }

            if (!this.ensureValidNames)
            {
                flags |= ShellNativeMethods.FOSs.FosNovalidate;
            }

            if (!this.EnsureReadOnly)
            {
                flags |= ShellNativeMethods.FOSs.FosNoreadonlyreturn;
            }

            if (this.restoreDirectory)
            {
                flags |= ShellNativeMethods.FOSs.FosNochangedir;
            }

            if (!this.showPlacesList)
            {
                flags |= ShellNativeMethods.FOSs.FosHidepinnedplaces;
            }

            if (!this.addToMruList)
            {
                flags |= ShellNativeMethods.FOSs.FosDontaddtorecent;
            }

            if (this.showHiddenItems)
            {
                flags |= ShellNativeMethods.FOSs.FosForceshowhidden;
            }

            if (!this.navigateToShortcut)
            {
                flags |= ShellNativeMethods.FOSs.FosNodereferencelinks;
            }

            return flags;
        }

        /// <summary>
        /// Get the IFileDialogCustomize interface, preparing to add controls.
        /// </summary>
        private void GetCustomizedFileDialog()
        {
            if (this.customize != null)
            {
                return;
            }

            if (this.nativeDialog == null)
            {
                this.InitializeNativeFileDialog();
                this.nativeDialog = this.GetNativeFileDialog();
            }

            this.customize = (IFileDialogCustomize)this.nativeDialog;
        }

        /// <summary>
        /// </summary>
        /// <param name="nativeDlg">
        /// </param>
        private void InitializeEventSink(IFileDialog nativeDlg)
        {
            // Check if we even need to have a sink.
            if ((((((this.FileOk == null && this.FolderChanging == null) && this.FolderChanged == null) && this.SelectionChanged == null) && this.FileTypeChanged == null) &&
                 this.DialogOpening == null) && (this.Controls == null || this.Controls.Count <= 0))
            {
                return;
            }

            uint cookie;
            this.nativeEventSink = new NativeDialogEventSink(this);
            nativeDlg.Advise(this.nativeEventSink, out cookie);
        }

        /// <summary>
        /// Raises the <see cref="CommonFileDialog.FileTypeChanged"/> event when the dialog is opened to notify the 
        ///   application of the initial chosen filetype.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void OnFileTypeChanged(EventArgs e)
        {
            var handler = this.FileTypeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CommonFileDialog.FolderChanged"/> event when the user navigates to a new folder.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void OnFolderChanged(EventArgs e)
        {
            var handler = this.FolderChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="FolderChanging"/> to stop navigation to a particular location.
        /// </summary>
        /// <param name="e">
        /// Cancelable event arguments.
        /// </param>
        private void OnFolderChanging(CommonFileDialogFolderChangeEventArgs e)
        {
            var handler = this.FolderChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CommonFileDialog.DialogOpening"/> event when the dialog is opened.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void OnOpening(EventArgs e)
        {
            var handler = this.DialogOpening;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CommonFileDialog.SelectionChanged"/> event when the user changes the selection in the dialog's view.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void OnSelectionChanged(EventArgs e)
        {
            var handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Tries to set the File(s) Type Combo to match the value in 
        ///   'DefaultExtension'.  Only doing this if 'this' is a Save dialog 
        ///   as it makes no sense to do this if only Opening a file.
        /// </summary>
        /// <param name="dialog">
        /// The native/IFileDialog instance.
        /// </param>
        private void SyncFileTypeComboToDefaultExtension(IFileDialog dialog)
        {
            // make sure it's a Save dialog and that there is a default 
            // extension to sync to.
            if (!(this is CommonSaveFileDialog) || this.DefaultExtension == null || this.Filters.Count <= 0)
            {
                return;
            }

            // The native version of SetFileTypeIndex() requires an
            // unsigned integer as its parameter. This (having it be defined
            // as a uint right up front) avoids a cast, and the potential 
            // problems of casting a signed value to an unsigned one.
            uint filtersCounter;

            CommonFileDialogFilter filter;

            for (filtersCounter = 0; filtersCounter < this.Filters.Count; filtersCounter++)
            {
                filter = this.Filters[(int)filtersCounter];

                if (!filter.Extensions.Contains(this.DefaultExtension))
                {
                    continue;
                }

                // set the docType combo to match this 
                // extension. property is a 1-based index.
                dialog.SetFileTypeIndex(filtersCounter + 1);

                // we're done, exit for
                break;
            }
        }

        #endregion

        /// <summary>
        /// </summary>
        private class NativeDialogEventSink : IFileDialogEvents, IFileDialogControlEvents
        {
            #region Constants and Fields

            /// <summary>
            /// </summary>
            private readonly CommonFileDialog parent;

            /// <summary>
            /// </summary>
            private bool firstFolderChanged = true;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// </summary>
            /// <param name="commonDialog">
            /// </param>
            public NativeDialogEventSink(CommonFileDialog commonDialog)
            {
                this.parent = commonDialog;
            }

            #endregion

            #region Implemented Interfaces

            #region IFileDialogControlEvents

            /// <summary>
            /// </summary>
            /// <param name="pfdc">
            /// </param>
            /// <param name="dwIDCtl">
            /// </param>
            public void OnButtonClicked(IFileDialogCustomize pfdc, int dwIDCtl)
            {
                // Find control
                var control = this.parent.Controls.GetControlbyId(dwIDCtl);

                // Call corresponding event
                if (control is CommonFileDialogButton)
                {
                    ((CommonFileDialogButton)control).RaiseClickEvent();
                }
            }

            /// <summary>
            /// </summary>
            /// <param name="pfdc">
            /// </param>
            /// <param name="dwIDCtl">
            /// </param>
            /// <param name="bChecked">
            /// </param>
            public void OnCheckButtonToggled(IFileDialogCustomize pfdc, int dwIDCtl, bool bChecked)
            {
                // Find control
                var control = this.parent.Controls.GetControlbyId(dwIDCtl);

                // Update control and call corresponding event
                if (!(control is CommonFileDialogCheckBox))
                {
                    return;
                }

                var box = control as CommonFileDialogCheckBox;
                box.IsChecked = bChecked;
                box.RaiseCheckedChangedEvent();
            }

            /// <summary>
            /// </summary>
            /// <param name="pfdc">
            /// </param>
            /// <param name="dwIDCtl">
            /// </param>
            public void OnControlActivating(IFileDialogCustomize pfdc, int dwIDCtl)
            {
            }

            /// <summary>
            /// </summary>
            /// <param name="pfdc">
            /// </param>
            /// <param name="dwIDCtl">
            /// </param>
            /// <param name="dwIDItem">
            /// </param>
            public void OnItemSelected(IFileDialogCustomize pfdc, int dwIDCtl, int dwIDItem)
            {
                // Find control
                var control = this.parent.Controls.GetControlbyId(dwIDCtl);

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
                    foreach (var item in menu.Items.Where(item => item.Id == dwIDItem))
                    {
                        item.RaiseClickEvent();
                        break;
                    }
                }
            }

            #endregion

            #region IFileDialogEvents

            /// <summary>
            /// </summary>
            /// <param name="pfd">
            /// </param>
            /// <returns>
            /// </returns>
            public HRESULT OnFileOk(IFileDialog pfd)
            {
                var args = new CancelEventArgs();
                this.parent.OnFileOk(args);

                if (!args.Cancel)
                {
                    // Make sure all custom properties are sync'ed
                    if (this.parent.Controls != null)
                    {
                        foreach (var control in this.parent.Controls)
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
                                foreach (var subcontrol in groupbox.Items.OfType<CommonFileDialogTextBox>())
                                {
                                    subcontrol.SyncValue();
                                    subcontrol.Closed = true;
                                }
                            }
                        }
                    }
                }

                return args.Cancel ? HRESULT.SFalse : HRESULT.S_OK;
            }

            /// <summary>
            /// </summary>
            /// <param name="pfd">
            /// </param>
            public void OnFolderChange(IFileDialog pfd)
            {
                if (this.firstFolderChanged)
                {
                    this.firstFolderChanged = false;
                    this.parent.OnOpening(EventArgs.Empty);
                }
                else
                {
                    this.parent.OnFolderChanged(EventArgs.Empty);
                }
            }

            /// <summary>
            /// </summary>
            /// <param name="pfd">
            /// </param>
            /// <param name="psiFolder">
            /// </param>
            /// <returns>
            /// </returns>
            public HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
            {
                var args = new CommonFileDialogFolderChangeEventArgs(GetFileNameFromShellItem(psiFolder));
                if (!this.firstFolderChanged)
                {
                    this.parent.OnFolderChanging(args);
                }

                return args.Cancel ? HRESULT.SFalse : HRESULT.S_OK;
            }

            /// <summary>
            /// </summary>
            /// <param name="pfd">
            /// </param>
            /// <param name="psi">
            /// </param>
            /// <param name="pResponse">
            /// </param>
            public void OnOverwrite(IFileDialog pfd, IShellItem psi, out ShellNativeMethods.FdeOverwriteResponse pResponse)
            {
                // Don't accept or reject the dialog, keep default settings
                pResponse = ShellNativeMethods.FdeOverwriteResponse.FdeorDefault;
            }

            /// <summary>
            /// </summary>
            /// <param name="pfd">
            /// </param>
            public void OnSelectionChange(IFileDialog pfd)
            {
                this.parent.OnSelectionChanged(EventArgs.Empty);
            }

            /// <summary>
            /// </summary>
            /// <param name="pfd">
            /// </param>
            /// <param name="psi">
            /// </param>
            /// <param name="pResponse">
            /// </param>
            public void OnShareViolation(IFileDialog pfd, IShellItem psi, out ShellNativeMethods.FdeShareviolationResponse pResponse)
            {
                // Do nothing: we will ignore share violations, 
                // and don't register
                // for them, so this method should never be called.
                pResponse = ShellNativeMethods.FdeShareviolationResponse.FdesvrAccept;
            }

            /// <summary>
            /// </summary>
            /// <param name="pfd">
            /// </param>
            public void OnTypeChange(IFileDialog pfd)
            {
                this.parent.OnFileTypeChanged(EventArgs.Empty);
            }

            #endregion

            #endregion
        }
    }
}