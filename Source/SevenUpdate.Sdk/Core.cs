// ***********************************************************************
// <copyright file="Core.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.Sdk
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Dialogs.TaskDialogs;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Shell;

    using SevenUpdate.Sdk.Properties;
    using SevenUpdate.Sdk.Windows;

    using Application = System.Windows.Application;
    using IWin32Window = System.Windows.Forms.IWin32Window;
    using MessageBox = System.Windows.MessageBox;
    using Shortcut = SevenUpdate.Shortcut;

    /// <summary>
    /// Contains methods that are essential for the program
    /// </summary>
    internal static class Core
    {
        #region Constants and Fields

        /// <summary>
        ///   The location of the file that contains the collection of Projects for the SDK
        /// </summary>
        public static readonly string ProjectsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                     @"\Seven Software\Seven Update SDK\Projects.sul";

        /// <summary>
        ///   The user application data location
        /// </summary>
        public static readonly string UserStore = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Seven Software\Seven Update SDK\";

        /// <summary>
        ///   The application directory of Seven Update
        /// </summary>
        private static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the application information of the project
        /// </summary>
        /// <value>The application info.</value>
        public static Sua AppInfo { get; private set; }

        /// <summary>
        ///   Gets or sets the index for the selected project
        /// </summary>
        /// <value>The index of the application</value>
        internal static int AppIndex { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the current project being edited is new
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if this instance is new project; otherwise, <see langword = "false" />.
        /// </value>
        internal static bool IsNewProject { get; set; }

        /// <summary>
        ///   Gets the collection of Project
        /// </summary>
        /// <value>The projects.</value>
        internal static Collection<Project> Projects
        {
            get
            {
                return Utilities.Deserialize<Collection<Project>>(ProjectsFile) ?? new ObservableCollection<Project>();
            }
        }

        /// <summary>
        ///   Gets or sets the index for the current shortcut being edited
        /// </summary>
        /// <value>The selected shortcut.</value>
        internal static int SelectedShortcut { get; set; }

        /// <summary>
        ///   Gets or sets the index for the selected update in the selected project
        /// </summary>
        /// <value>The index of the update.</value>
        internal static int UpdateIndex { get; set; }

        /// <summary>
        ///   Gets the current update being edited
        /// </summary>
        /// <value>The update info.</value>
        internal static Update UpdateInfo { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Edit the selected project or update
        /// </summary>
        internal static void EditItem()
        {
            IsNewProject = false;
            AppInfo = Utilities.Deserialize<Sua>(UserStore + Projects[AppIndex].ApplicationName + @".sua");
            if (UpdateIndex < 0)
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/AppInfo.xaml", UriKind.Relative));
            }
            else
            {
                UpdateInfo = Utilities.Deserialize<Collection<Update>>(UserStore + Projects[AppIndex].ApplicationName + @".sui")[UpdateIndex];
                if (UpdateInfo.Files == null)
                {
                    UpdateInfo.Files = new ObservableCollection<UpdateFile>();
                }

                if (UpdateInfo.Shortcuts == null)
                {
                    UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();
                }

                if (UpdateInfo.RegistryItems == null)
                {
                    UpdateInfo.RegistryItems = new ObservableCollection<RegistryItem>();
                }

                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateInfo.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// The rectangle_ mouse left button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The arguments generated by the event
        /// </param>
        internal static void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Gets the IWin32 window.
        /// </summary>
        /// <param name="visual">
        /// The visual object
        /// </param>
        /// <returns>
        /// The Win32 Window
        /// </returns>
        internal static IWin32Window GetIWin32Window(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual) as HwndSource;
            if (source != null)
            {
                IWin32Window win = new OldWindow(source.Handle);
                return win;
            }

            return null;
        }

        /// <summary>
        /// Creates a new project
        /// </summary>
        internal static void NewProject()
        {
            IsNewProject = true;
            AppIndex = -1;
            UpdateIndex = -1;
            AppInfo = new Sua();
            UpdateInfo = new Update();
            AppInfo.Description = new ObservableCollection<LocaleString>();
            AppInfo.Name = new ObservableCollection<LocaleString>();
            AppInfo.Publisher = new ObservableCollection<LocaleString>();
            UpdateInfo.Name = new ObservableCollection<LocaleString>();
            UpdateInfo.Description = new ObservableCollection<LocaleString>();
            UpdateInfo.ReleaseDate = DateTime.Now.ToShortDateString();
            UpdateInfo.Files = new ObservableCollection<UpdateFile>();
            UpdateInfo.RegistryItems = new ObservableCollection<RegistryItem>();
            UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/AppInfo.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Creates a new update for the selected project
        /// </summary>
        internal static void NewUpdate()
        {
            IsNewProject = false;
            AppInfo = Utilities.Deserialize<Sua>(UserStore + Projects[AppIndex].ApplicationName + @".sua");
            UpdateInfo = new Update
                {
                    Files = new ObservableCollection<UpdateFile>(), 
                    RegistryItems = new ObservableCollection<RegistryItem>(), 
                    Shortcuts = new ObservableCollection<Shortcut>(), 
                    Description = new ObservableCollection<LocaleString>(), 
                    Name = new ObservableCollection<LocaleString>()
                };
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateInfo.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Opens a OpenFileDialog
        /// </summary>
        /// <param name="initialDirectory">
        /// Gets or sets the initial directory displayed when the dialog is shown. A <see langword="null"/> or empty string indicates that the dialog is using the default directory
        /// </param>
        /// <param name="multiSelect">
        /// Gets or sets a value that determines whether the user can select more than one file
        /// </param>
        /// <param name="defaultExtension">
        /// Gets or sets the default file extension to be added to the file names. If the value is <see langword="null"/> or empty, the extension is not added to the file names
        /// </param>
        /// <param name="navigateToShortcut">
        /// Gets or sets a value that controls whether shortcuts should be treated as their target items, allowing an application to open a .lnk file
        /// </param>
        /// <returns>
        /// A collection of the selected files
        /// </returns>
        internal static string[] OpenFileDialog(string initialDirectory = null, bool multiSelect = false, string defaultExtension = null, bool navigateToShortcut = false)
        {
            var openFileDialog = new OpenFileDialog
                {
                    AutoUpgradeEnabled = true, 
                    Multiselect = multiSelect, 
                    InitialDirectory = initialDirectory, 
                    DereferenceLinks = navigateToShortcut, 
                    CheckFileExists = true, 
                    DefaultExt = defaultExtension, 
                    ValidateNames = true
                };
            switch (defaultExtension)
            {
                case @"sua":
                    openFileDialog.Filter = Resources.Sua + @" (*.sua)|*.sua";
                    break;
                case @"sui":
                    openFileDialog.Filter = Resources.Sui + @" (*.sui)|*.sui";
                    break;
                case "reg":
                    openFileDialog.Filter = Resources.RegFile + @" (*.reg)|*.reg";
                    break;
                case "lnk":
                    openFileDialog.Filter = Resources.Shortcut + @" (*.lnk)|*.lnk";
                    break;
                default:
                    openFileDialog.AddExtension = false;
                    openFileDialog.Filter = Resources.AllFiles + @"|*.*";
                    break;
            }

            return openFileDialog.ShowDialog(GetIWin32Window(Application.Current.MainWindow)) != DialogResult.OK ? null : openFileDialog.FileNames;
        }

        /// <summary>
        /// Opens a SaveFileDialog
        /// </summary>
        /// <param name="initialDirectory">
        /// Gets or sets the initial directory displayed when the dialog is shown. A <see langword="null"/> or empty string indicates that the dialog is using the default directory
        /// </param>
        /// <param name="defaultFileName">
        /// Sets the default file name
        /// </param>
        /// <param name="defaultExtension">
        /// Gets or sets the default file extension to be added to the file names. If the value is <see langword="null"/> or empty, the extension is not added to the file names
        /// </param>
        /// <returns>
        /// Gets the selected filename
        /// </returns>
        internal static string SaveFileDialog(string initialDirectory, string defaultFileName, string defaultExtension = null)
        {
            var saveFileDialog = new SaveFileDialog
                {
                    FileName = defaultFileName, 
                    CheckFileExists = false, 
                    DefaultExt = defaultExtension, 
                    AddExtension = true, 
                    InitialDirectory = initialDirectory, 
                    ValidateNames = true
                };

            switch (defaultExtension)
            {
                case @"sua":
                    saveFileDialog.Filter = Resources.Sua + @" (*.sua)|*.sua";
                    break;
                case @"sui":
                    saveFileDialog.Filter = Resources.Sui + @" (*.sui)|*.sui";
                    break;
                case @"reg":
                    saveFileDialog.Filter = Resources.RegFile + @" (*.reg)|*.reg";
                    break;
                case @"lnk":
                    saveFileDialog.Filter = Resources.Shortcut + @" (*.lnk)|*.lnk";
                    break;
                default:
                    saveFileDialog.AddExtension = false;
                    saveFileDialog.Filter = Resources.AllFiles + @"|*.*";
                    break;
            }

            return saveFileDialog.ShowDialog(GetIWin32Window(Application.Current.MainWindow)) != DialogResult.OK ? null : saveFileDialog.FileName;
        }

        /// <summary>
        /// The base_ serialization error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The arguments generated by the event
        /// </param>
        internal static void SerializationError(object sender, SerializationErrorEventArgs e)
        {
            ShowMessage(Resources.ProjectLoadError, TaskDialogStandardIcon.Error, e.Exception.Message);
        }

        /// <summary>
        /// Sets the Windows 7 <see cref="JumpList"/>
        /// </summary>
        internal static void SetJumpList()
        {
            // Create JumpTask
            var jumpList = new JumpList();

            var startIndex = Projects.Count - 2;
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            JumpTask jumpTask;
            for (var x = startIndex; x < Projects.Count; x++)
            {
                jumpTask = new JumpTask
                    {
                        ApplicationPath = AppDir + @"SevenUpdate.Sdk.exe", 
                        IconResourcePath = AppDir + @"SevenUpdate.Base.dll", 
                        IconResourceIndex = 7, 
                        Title = Resources.CreateUpdate, 
                        CustomCategory = Projects[x].ApplicationName, 
                        Arguments = @"-newupdate " + x, 
                    };
                jumpList.JumpItems.Add(jumpTask);
                for (var y = 0; y < Projects[x].UpdateNames.Count; y++)
                {
                    jumpTask = new JumpTask
                        {
                            ApplicationPath = AppDir + @"SevenUpdate.Sdk.exe", 
                            IconResourcePath = AppDir + @"SevenUpdate.Base.dll", 
                            IconResourceIndex = 8, 
                            Title = String.Format(CultureInfo.CurrentCulture, Resources.Edit, Projects[x].UpdateNames[y]), 
                            CustomCategory = Projects[x].ApplicationName, 
                            Arguments = @"-edit " + x + " " + y
                        };

                    jumpList.JumpItems.Add(jumpTask);
                }
            }

            // Configure a new JumpTask
            jumpTask = new JumpTask
                {
                    ApplicationPath = AppDir + @"SevenUpdate.Sdk.exe", 
                    IconResourcePath = AppDir + @"SevenUpdate.Base.dll", 
                    IconResourceIndex = 6, 
                    Title = Resources.CreateProject, 
                    CustomCategory = Resources.Tasks, 
                    Arguments = @"-newproject"
                };
            jumpList.JumpItems.Add(jumpTask);
            JumpList.SetJumpList(Application.Current, jumpList);
        }

        /// <summary>
        /// Shows either a <see cref="TaskDialog"/> or a <see cref="System.Windows.MessageBox"/> if running legacy windows.
        /// </summary>
        /// <param name="instructionText">
        /// The main text to display (Blue 14pt for <see cref="TaskDialog"/>)
        /// </param>
        /// <param name="icon">
        /// The <see cref="TaskDialogStandardIcon"/> to display
        /// </param>
        /// <param name="description">
        /// A description of the message, supplements the instruction text
        /// </param>
        /// <returns>
        /// Returns the result of the message
        /// </returns>
        internal static TaskDialogResult ShowMessage(string instructionText, TaskDialogStandardIcon icon, string description = null)
        {
            return ShowMessage(instructionText, icon, TaskDialogStandardButtons.Ok, description);
        }

        /// <summary>
        /// Shows either a <see cref="TaskDialog"/> or a <see cref="System.Windows.MessageBox"/> if running legacy windows.
        /// </summary>
        /// <param name="instructionText">
        /// The main text to display (Blue 14pt for <see cref="TaskDialog"/>)
        /// </param>
        /// <param name="icon">
        /// The icon to use
        /// </param>
        /// <param name="standardButtons">
        /// The standard buttons to use (with or without the custom default button text)
        /// </param>
        /// <param name="description">
        /// A description of the message, supplements the instruction text
        /// </param>
        /// <param name="footerText">
        /// Text to display as a footer message
        /// </param>
        /// <param name="defaultButtonText">
        /// Text to display on the button
        /// </param>
        /// <param name="displayShieldOnButton">
        /// Indicates if a UAC shield is to be displayed on the defaultButton
        /// </param>
        /// <returns>
        /// Returns the result of the message
        /// </returns>
        private static TaskDialogResult ShowMessage(
            string instructionText, 
            TaskDialogStandardIcon icon, 
            TaskDialogStandardButtons standardButtons, 
            string description = null, 
            string footerText = null, 
            string defaultButtonText = null, 
            bool displayShieldOnButton = false)
        {
            if (TaskDialog.IsPlatformSupported)
            {
                var td = new TaskDialog
                    {
                        Caption = Resources.SevenUpdateSDK, 
                        InstructionText = instructionText, 
                        Text = description, 
                        Icon = icon, 
                        FooterText = footerText, 
                        FooterIcon = TaskDialogStandardIcon.Information, 
                        CanCancel = true, 
                    };
                if (defaultButtonText != null)
                {
                    var button = new TaskDialogButton(@"btnCustom", defaultButtonText)
                        {
                            Default = true, 
                            ShowElevationIcon = displayShieldOnButton
                        };
                    td.Controls.Add(button);

                    switch (standardButtons)
                    {
                        case TaskDialogStandardButtons.Ok:
                            button = new TaskDialogButton(@"btnOK", "OK")
                                {
                                    Default = false
                                };
                            td.Controls.Add(button);
                            break;
                        case TaskDialogStandardButtons.Cancel:
                            button = new TaskDialogButton(@"btnCancel", "Cancel")
                                {
                                    Default = false
                                };
                            td.Controls.Add(button);
                            break;
                        case TaskDialogStandardButtons.Retry:
                            button = new TaskDialogButton(@"btnRetry", "Retry")
                                {
                                    Default = false
                                };
                            td.Controls.Add(button);
                            break;
                        case TaskDialogStandardButtons.Close:
                            button = new TaskDialogButton(@"btnClose", "Close")
                                {
                                    Default = false
                                };
                            td.Controls.Add(button);
                            break;
                    }
                }
                else
                {
                    td.StandardButtons = standardButtons;
                }

                return td.ShowDialog(Application.Current.MainWindow);
            }

            var message = instructionText;
            var msgIcon = MessageBoxImage.None;

            if (description != null)
            {
                message += Environment.NewLine + description;
            }

            if (footerText != null)
            {
                message += Environment.NewLine + footerText;
            }

            switch (icon)
            {
                case TaskDialogStandardIcon.Error:
                    msgIcon = MessageBoxImage.Error;
                    break;
                case TaskDialogStandardIcon.Information:
                    msgIcon = MessageBoxImage.Information;
                    break;
                case TaskDialogStandardIcon.Warning:
                    msgIcon = MessageBoxImage.Warning;
                    break;
            }

            MessageBoxResult result;

            if (standardButtons == TaskDialogStandardButtons.Cancel || defaultButtonText != null)
            {
                result = MessageBox.Show(message, Resources.SevenUpdateSDK, MessageBoxButton.OKCancel, msgIcon);
            }
            else
            {
                result = MessageBox.Show(message, Resources.SevenUpdateSDK, MessageBoxButton.OK, msgIcon);
            }

            switch (result)
            {
                case MessageBoxResult.No:
                    return TaskDialogResult.No;
                case MessageBoxResult.OK:
                    return TaskDialogResult.Ok;
                case MessageBoxResult.Yes:
                    return TaskDialogResult.Yes;
                default:
                    return TaskDialogResult.Cancel;
            }
        }

        #endregion

        /// <summary>
        /// The old window
        /// </summary>
        private sealed class OldWindow : IWin32Window, IDisposable
        {
            #region Constants and Fields

            /// <summary>
            ///   The pointer to the window
            /// </summary>
            private readonly IntPtr windowHandle;

            /// <summary>
            ///   <see langword = "true" /> if the window is disposed
            /// </summary>
            private bool disposed;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="OldWindow"/> class.
            /// </summary>
            /// <param name="handle">
            /// The handle.
            /// </param>
            public OldWindow(IntPtr handle)
            {
                this.windowHandle = handle;
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="OldWindow"/> class.
            /// </summary>
            ~OldWindow()
            {
                this.Dispose(false);
            }

            #endregion

            #region Properties

            /// <summary>
            ///   Gets the handle to the window represented by the implementer.
            /// </summary>
            /// <value></value>
            /// <returns>A handle to the window represented by the implementer.</returns>
            IntPtr IWin32Window.Handle
            {
                get
                {
                    return this.windowHandle;
                }
            }

            #endregion

            #region Implemented Interfaces

            #region IDisposable

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(false);

                // Unregister object for finalization.
                GC.SuppressFinalize(this);
            }

            #endregion

            #endregion

            #region Methods

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources
            /// </summary>
            /// <param name="disposing">
            /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.
            /// </param>
            private void Dispose(bool disposing)
            {
                lock (this)
                {
                    // Do nothing if the object has already been disposed of.
                    if (this.disposed)
                    {
                        return;
                    }

                    if (disposing)
                    {
                        // Release disposable objects used by this instance here.
                    }

                    // Release unmanaged resources here. Don't access reference type fields.

                    // Remember that the object has been disposed of.
                    this.disposed = true;
                }
            }

            #endregion
        }
    }
}