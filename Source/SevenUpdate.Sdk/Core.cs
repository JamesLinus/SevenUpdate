// ***********************************************************************
// <copyright file="Core.cs" project="SevenUpdate.Sdk" assembly="SevenUpdate.Sdk" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate.Sdk
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Dialogs;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Shell;

    using Pages;

    using Properties;

    using Windows;

    using Application = System.Windows.Application;
    using IWin32Window = System.Windows.Forms.IWin32Window;
    using MessageBox = System.Windows.MessageBox;

    /// <summary>Contains methods that are essential for the program.</summary>
    internal static class Core
    {
        #region Constants and Fields

        /// <summary>The location of the file that contains the collection of Projects for the SDK.</summary>
        public static readonly string ProjectsFile =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seven Update SDK", "Projects.sul");

        /// <summary>The application information page.</summary>
        private static AppInfo appInfoPage;

        /// <summary>The main page.</summary>
        private static Main mainPage;

        /// <summary>The update files page.</summary>
        private static UpdateFiles updateFilesPage;

        /// <summary>The update information page.</summary>
        private static UpdateInfo updateInfoPage;

        /// <summary>The update registry page.</summary>
        private static UpdateRegistry updateRegistryPage;

        /// <summary>The update review page.</summary>
        private static UpdateReview updateReviewPage;

        /// <summary>The update shortcuts page.</summary>
        private static UpdateShortcuts updateShortcutsPage;

        #endregion

        #region Properties

        /// <summary>Gets the application information of the project.</summary>
        /// <value>The application info.</value>
        public static Sua AppInfo { get; private set; }

        /// <summary>Gets or sets the index for the selected project.</summary>
        /// <value>The index of the application.</value>
        internal static int AppIndex { get; set; }

        /// <summary>Gets the Main page.</summary>
        internal static Main MainPage
        {
            get
            {
                return mainPage ?? (mainPage = new Main());
            }
        }

        /// <summary>Gets or sets a collection of Projects.</summary>
        /// <value>The projects.</value>
        internal static Collection<Project> Projects { get; set; }

        /// <summary>Gets or sets the index for the current shortcut being edited.</summary>
        /// <value>The selected shortcut.</value>
        internal static int SelectedShortcut { get; set; }

        /// <summary>Gets the UpdateFiles page.</summary>
        internal static UpdateFiles UpdateFilesPage
        {
            get
            {
                return updateFilesPage ?? (updateFilesPage = new UpdateFiles());
            }

            private set
            {
                updateFilesPage = value;
            }
        }

        /// <summary>Gets or sets the index for the selected update in the selected project.</summary>
        /// <value>The index of the update.</value>
        internal static int UpdateIndex { private get; set; }

        /// <summary>Gets the current update being edited.</summary>
        /// <value>The update info.</value>
        internal static Update UpdateInfo { get; private set; }

        /// <summary>Gets the UpdateInfo page.</summary>
        internal static UpdateInfo UpdateInfoPage
        {
            get
            {
                return updateInfoPage ?? (updateInfoPage = new UpdateInfo());
            }

            private set
            {
                updateInfoPage = value;
            }
        }

        /// <summary>Gets the UpdateRegistry page.</summary>
        internal static UpdateRegistry UpdateRegistryPage
        {
            get
            {
                return updateRegistryPage ?? (updateRegistryPage = new UpdateRegistry());
            }

            private set
            {
                updateRegistryPage = value;
            }
        }

        /// <summary>Gets the UpdateReview page.</summary>
        internal static UpdateReview UpdateReviewPage
        {
            get
            {
                return updateReviewPage ?? (updateReviewPage = new UpdateReview());
            }

            private set
            {
                updateReviewPage = value;
            }
        }

        /// <summary>Gets the UpdateShortcuts page.</summary>
        internal static UpdateShortcuts UpdateShortcutsPage
        {
            get
            {
                return updateShortcutsPage ?? (updateShortcutsPage = new UpdateShortcuts());
            }

            private set
            {
                updateShortcutsPage = value;
            }
        }

        /// <summary>Gets or sets the AppInfo page.</summary>
        private static AppInfo AppInfoPage
        {
            get
            {
                return appInfoPage ?? (appInfoPage = new AppInfo());
            }

            set
            {
                appInfoPage = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether the current project being edited is new.</summary>
        /// <value><c>True</c> if this instance is new project; otherwise, <c>False</c>.</value>
        private static bool IsNewProject { get; set; }

        #endregion

        #region Methods

        /// <summary>Edit the selected project or update.</summary>
        internal static void EditItem()
        {
            ResetPages();
            IsNewProject = false;

            if (File.Exists(Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sua")))
            {
                AppInfo =
                    Utilities.Deserialize<Sua>(Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sua"));
            }
            else
            {
                AppInfo = null;
                UpdateInfo = null;
                ShowMessage(
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        Resources.FileLoadError,
                        Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sua")),
                    TaskDialogStandardIcon.Error);
                return;
            }

            if (UpdateIndex < 0)
            {
                MainWindow.NavService.Navigate(AppInfoPage);
                return;
            }

            if (File.Exists(Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + @".sui")))
            {
                UpdateInfo =
                    Utilities.Deserialize<Collection<Update>>(
                        Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sui"))[UpdateIndex];
            }
            else
            {
                AppInfo = null;
                UpdateInfo = null;
                ShowMessage(
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        Resources.FileLoadError,
                        Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sui")),
                    TaskDialogStandardIcon.Error);
                return;
            }

            var jumpTask = new JumpTask
                {
                    IconResourcePath =
                        Path.Combine(Directory.GetParent(Utilities.AppDir).FullName, "Shared", @"SevenUpdate.Base.dll"),
                    IconResourceIndex = 8,
                    Title = Utilities.GetLocaleString(UpdateInfo.Name),
                    Arguments = @"-edit " + AppIndex + " " + UpdateIndex
                };

            JumpList.AddToRecentCategory(jumpTask);

            MainWindow.NavService.Navigate(UpdateInfoPage);
        }

        /// <summary>The rectangle_ mouse left button down.</summary>
        /// <param name="sender">  The object that called the event.</param>
        /// <param name="e">  The arguments generated by the event.</param>
        internal static void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>Gets the IWin32 window.</summary>
        /// <param name="visual">  The visual object.</param>
        /// <returns>The Win32 Window.</returns>
        internal static IWin32Window GetIWin32Window(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual) as HwndSource;
            if (source != null)
            {
                var win = new Win32Window(source.Handle);
                var window = win;
                win.Dispose();
                return window;
            }

            return null;
        }

        /// <summary>Creates a new project.</summary>
        internal static void NewProject()
        {
            ResetPages();
            IsNewProject = true;
            AppIndex = -1;
            UpdateIndex = -1;
            AppInfo = new Sua();
            UpdateInfo = new Update { ReleaseDate = DateTime.Now.ToShortDateString() };
            MainWindow.NavService.Navigate(AppInfoPage);
        }

        /// <summary>Creates a new update for the selected project.</summary>
        internal static void NewUpdate()
        {
            ResetPages();
            IsNewProject = false;
            AppInfo =
                Utilities.Deserialize<Sua>(Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sua"));
            UpdateInfo = new Update();

            MainWindow.NavService.Navigate(UpdateInfoPage);
        }

        /// <summary>Opens a OpenFileDialog.</summary>
        /// <param name="initialDirectory">
        ///   Gets or sets the initial directory displayed when the dialog is shown. A <c>null</c> or empty string
        ///   indicates that the dialog is using the default directory.
        /// </param>
        /// <param name="initialFileName">  Gets or sets the initial filename displayed when the dialog is shown.</param>
        /// <param name="multiSelect">  Gets or sets a value that determines whether the user can select more than one file.</param>
        /// <param name="defaultExtension">
        ///   Gets or sets the default file extension to be added to the file names. If the value is <c>null</c> or
        ///   empty, the extension is not added to the file names.
        /// </param>
        /// <param name="navigateToShortcut">
        ///   Gets or sets a value that controls whether shortcuts should be treated as their target items, allowing an
        ///   application to open a .lnk file.
        /// </param>
        /// <returns>A collection of the selected files.</returns>
        internal static string[] OpenFileDialog(
            string initialDirectory = null,
            string initialFileName = null,
            bool multiSelect = false,
            string defaultExtension = null,
            bool navigateToShortcut = false)
        {
            string[] result;
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.AutoUpgradeEnabled = true;
                openFileDialog.Multiselect = multiSelect;
                openFileDialog.InitialDirectory = initialDirectory;
                openFileDialog.DereferenceLinks = navigateToShortcut;
                openFileDialog.CheckFileExists = true;
                openFileDialog.DefaultExt = defaultExtension;
                openFileDialog.FileName = initialFileName;
                openFileDialog.ValidateNames = true;

                switch (defaultExtension)
                {
                    case @"sua":
                        openFileDialog.Filter = Resources.Sua + @" (*.sua)|*.sua";
                        break;
                    case @"sui":
                        openFileDialog.Filter = Resources.Sui + @" (*.sui)|*.sui";
                        break;
                    case @"reg":
                        openFileDialog.Filter = Resources.RegFile + @" (*.reg)|*.reg";
                        break;
                    case @"lnk":
                        openFileDialog.Filter = Resources.Shortcut + @" (*.lnk)|*.lnk";
                        break;
                    default:
                        openFileDialog.AddExtension = false;
                        openFileDialog.Filter = Resources.AllFiles + @"|*.*";
                        break;
                }

                result = openFileDialog.ShowDialog(GetIWin32Window(Application.Current.MainWindow)) != DialogResult.OK
                             ? null
                             : openFileDialog.FileNames;
            }

            return result;
        }

        /// <summary>Opens a SaveFileDialog.</summary>
        /// <param name="initialDirectory">
        ///   Gets or sets the initial directory displayed when the dialog is shown. A <c>null</c> or empty string
        ///   indicates that the dialog is using the default directory.
        /// </param>
        /// <param name="defaultFileName">  Sets the default file name.</param>
        /// <param name="defaultExtension">
        ///   Gets or sets the default file extension to be added to the file names. If the value is <c>null</c> or
        ///   empty, the extension is not added to the file names.
        /// </param>
        /// <returns>Gets the selected filename.</returns>
        internal static string SaveFileDialog(
            string initialDirectory, string defaultFileName, string defaultExtension = null)
        {
            string result;
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.DefaultExt = defaultExtension;
                saveFileDialog.AddExtension = true;
                saveFileDialog.InitialDirectory = initialDirectory;
                saveFileDialog.ValidateNames = true;
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

                result = saveFileDialog.ShowDialog(GetIWin32Window(Application.Current.MainWindow)) != DialogResult.OK
                             ? null
                             : saveFileDialog.FileName;
            }

            return result;
        }

        /// <summary>Saves the project.</summary>
        /// <param name="export">  Export the sui/sua files, <c>False</c> otherwise.</param>
        internal static void SaveProject(bool export = false)
        {
            var appUpdates = new Collection<Update>();
            var appName = Utilities.GetLocaleString(AppInfo.Name);

            if (Projects == null)
            {
                Projects = new Collection<Project>();
            }

            if (AppInfo.Platform == Platform.X64)
            {
                if (!appName.Contains("x64") && !appName.Contains("X64"))
                {
                    appName += " (x64)";
                }
            }

            var suiFile = Path.Combine(App.UserStore, appName + ".sui");
            var suaFile = Path.Combine(App.UserStore, appName + ".sua");

            var updateNames = new ObservableCollection<string>();

            var suiFileName = appName;
            var suaFileName = appName;

            // If SUA exists lets remove the old info
            if (AppIndex > -1)
            {
                if (File.Exists(Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sui")))
                {
                    appUpdates =
                        Utilities.Deserialize<Collection<Update>>(
                            Path.Combine(App.UserStore, Projects[AppIndex].ApplicationName + ".sui"));
                }

                suiFileName = Projects[AppIndex].ExportedSuiFileName;
                suaFileName = Projects[AppIndex].ExportedSuaFileName;

                updateNames = Projects[AppIndex].UpdateNames;
                Projects.RemoveAt(AppIndex);
            }

            // If we are just updating the SUA, lets add it
            if (appUpdates.Count == 0 || UpdateIndex == -1)
            {
                updateNames.Add(Utilities.GetLocaleString(UpdateInfo.Name));
                appUpdates.Add(UpdateInfo);
            }
            else
            {
                // If we are updating the update, lets remove the old info and add the new.
                updateNames.RemoveAt(UpdateIndex);
                appUpdates.RemoveAt(UpdateIndex);
                appUpdates.Add(UpdateInfo);
                updateNames.Add(Utilities.GetLocaleString(UpdateInfo.Name));
            }

            // Save the SUI File
            Utilities.Serialize(appUpdates, suiFile);

            // Save project file
            var project = new Project { ApplicationName = appName, };

            foreach (var t in updateNames)
            {
                project.UpdateNames.Add(t);
            }

            if (IsNewProject)
            {
                // Save the SUA file
                Utilities.Serialize(AppInfo, suaFile);
            }

            if (!export)
            {
                Projects.Insert(0, project);
                Utilities.Serialize(Projects, ProjectsFile);

                IsNewProject = false;
                MainWindow.NavService.Navigate(MainPage);
                return;
            }

            project.ExportedSuiFileName = suiFileName ?? appName;
            var fileName = SaveFileDialog(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                project.ExportedSuiFileName,
                @"sui");

            if (fileName == null)
            {
                Projects.Insert(0, project);
                Utilities.Serialize(Projects, ProjectsFile);
                return;
            }

            project.ExportedSuiFileName = Path.GetFileNameWithoutExtension(fileName);
            File.Copy(suiFile, fileName, true);
            if (IsNewProject)
            {
                project.ExportedSuaFileName = suaFileName ?? appName;
                fileName = SaveFileDialog(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    project.ExportedSuaFileName,
                    @"sua");

                if (fileName == null)
                {
                    Projects.Insert(0, project);
                    Utilities.Serialize(Projects, ProjectsFile);
                    return;
                }

                project.ExportedSuaFileName = Path.GetFileNameWithoutExtension(fileName);
                File.Copy(suaFile, fileName, true);
            }

            Projects.Insert(0, project);
            Utilities.Serialize(Projects, ProjectsFile);

            IsNewProject = false;
            MainWindow.NavService.Navigate(MainPage);
        }

        /// <summary>Shows either a <c>TaskDialog</c> or a <c>System.Windows.MessageBox</c> if running legacy windows.</summary>
        /// <param name="instructionText">  The main text to display (Blue 14pt for <c>TaskDialog</c>).</param>
        /// <param name="icon">  The <c>TaskDialogStandardIcon</c> to display.</param>
        /// <param name="description">  A description of the message, supplements the instruction text.</param>
        internal static void ShowMessage(string instructionText, TaskDialogStandardIcon icon, string description = null)
        {
            ShowMessage(instructionText, icon, TaskDialogStandardButtons.Ok, description);
        }

        /// <summary>Updates a LocaleString collection a new value.</summary>
        /// <param name="value">  The value to update the collection with.</param>
        /// <param name="localeStrings">  The collection for the value.</param>
        internal static void UpdateLocaleStrings(string value, ObservableCollection<LocaleString> localeStrings)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var found = false;

                foreach (var t in localeStrings.Where(t => t.Lang == Utilities.Locale))
                {
                    t.Value = value;
                    found = true;
                }

                if (!found)
                {
                    var ls = new LocaleString { Lang = Utilities.Locale, Value = value };
                    localeStrings.Add(ls);
                }
            }
            else
            {
                for (var x = 0; x < localeStrings.Count; x++)
                {
                    if (localeStrings[x].Lang == Utilities.Locale)
                    {
                        localeStrings.RemoveAt(x);
                    }
                }
            }
        }

        /// <summary>Resets the pages to default status.</summary>
        private static void ResetPages()
        {
            AppInfoPage = null; // new AppInfo();
            UpdateInfoPage = null; // new UpdateInfo();
            UpdateFilesPage = null; // new UpdateFiles();
            UpdateRegistryPage = null; // new UpdateRegistry();
            UpdateShortcutsPage = null; // new UpdateShortcuts();
            UpdateReviewPage = null;
        }

        /// <summary>Shows either a <c>TaskDialog</c> or a <c>System.Windows.MessageBox</c> if running legacy windows.</summary>
        /// <param name="instructionText">  The main text to display (Blue 14pt for <c>TaskDialog</c>).</param>
        /// <param name="icon">  The icon to use.</param>
        /// <param name="standardButtons">  The standard buttons to use (with or without the custom default button text).</param>
        /// <param name="description">  A description of the message, supplements the instruction text.</param>
        /// <param name="footerText">  Text to display as a footer message.</param>
        /// <param name="defaultButtonText">  Text to display on the button.</param>
        /// <param name="displayShieldOnButton">  Indicates if a UAC shield is to be displayed on the defaultButton.</param>
        private static void ShowMessage(
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
                using (var td = new TaskDialog())
                {
                    td.Caption = Resources.SevenUpdateSDK;
                    td.InstructionText = instructionText;
                    td.Text = description;
                    td.Icon = icon;
                    td.FooterText = footerText;
                    td.FooterIcon = TaskDialogStandardIcon.Information;
                    td.CanCancel = true;

                    if (defaultButtonText != null)
                    {
                        var button = new TaskDialogButton(@"btnCustom", defaultButtonText)
                            { Default = true, ShowElevationIcon = displayShieldOnButton };
                        td.Controls.Add(button);

                        switch (standardButtons)
                        {
                            case TaskDialogStandardButtons.Ok:
                                button = new TaskDialogButton(@"btnOK", Resources.OK) { Default = false };
                                td.Controls.Add(button);
                                break;
                            case TaskDialogStandardButtons.Cancel:
                                button = new TaskDialogButton(@"btnCancel", Resources.Cancel) { Default = false };
                                td.Controls.Add(button);
                                break;
                            case TaskDialogStandardButtons.Retry:
                                button = new TaskDialogButton(@"btnRetry", Resources.Retry) { Default = false };
                                td.Controls.Add(button);
                                break;
                            case TaskDialogStandardButtons.Close:
                                button = new TaskDialogButton(@"btnClose", Resources.Close) { Default = false };
                                td.Controls.Add(button);
                                break;
                        }
                    }
                    else
                    {
                        td.StandardButtons = standardButtons;
                    }

                    td.ShowDialog(Application.Current.MainWindow);
                    return;
                }
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
                    return;
                case MessageBoxResult.OK:
                    return;
                case MessageBoxResult.Yes:
                    return;
                default:
                    return;
            }
        }

        #endregion
    }
}