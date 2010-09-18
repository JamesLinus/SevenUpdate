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
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using Microsoft.Win32;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Internal;
using SevenUpdate.Sdk.Properties;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk
{
    internal static class Core
    {
        #region Properties

        /// <summary>
        ///   The application information of the project
        /// </summary>
        public static Sua AppInfo { get; set; }

        /// <summary>
        ///   Gets or sets the current update being edited
        /// </summary>
        internal static Update UpdateInfo { get; set; }

        /// <summary>
        ///   Gets or sets the current shortcut being edited
        /// </summary>
        internal static int SelectedShortcut { get; set; }

        /// <summary>
        ///   Gets or sets if aero glass is enabled
        /// </summary>
        internal static bool IsGlassEnabled { get; set; }

        /// <summary>
        ///   Gets of sets if the current project being edited is new
        /// </summary>
        internal static bool IsNewProject { get; set; }

        /// <summary>
        ///   Checks to see if a Url is valid and on the internet
        /// </summary>
        /// <param name = "url">A url to check</param>
        /// <returns>True if url is valid, otherwise false</returns>
        internal static bool CheckUrl(string url)
        {
            try
            {
                new Uri(url);
                var request = WebRequest.Create(url);
                request.Timeout = 15000;
                request.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Fields

        /// <summary>
        ///   The application directory of Seven Update
        /// </summary>
        public static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";

        /// <summary>
        ///   The user application data location
        /// </summary>
        public static readonly string UserStore = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Seven Software\Seven Update SDK\";

        /// <summary>
        ///   The location of the list of projects for the SDK
        /// </summary>
        public static readonly string ProjectsFile = UserStore + @"Projects.sul";

        public static ObservableCollection<Project> Projects = Base.Deserialize<ObservableCollection<Project>>(ProjectsFile) ?? new ObservableCollection<Project>();

        internal static int UpdateIndex = -1;
        internal static int AppIndex = -1;

        #endregion

        #region Methods

        /// <summary>
        ///   Sets the Windows 7 JumpList
        /// </summary>
        internal static void SetJumpLists()
        {
            // Create JumpTask
            var jumpList = new JumpList();

            JumpTask jumpTask;
            for (var x = 0; x < Projects.Count; x++)
            {
                jumpTask = new JumpTask
                               {
                                   ApplicationPath = Base.AppDir + "SevenUpdate.Sdk.exe",
                                   IconResourcePath = Base.AppDir + "SevenUpdate.Sdk.exe",
                                   Title = Resources.CreateUpdate,
                                   CustomCategory = Projects[x].ApplicationName,
                                   Arguments = "-newupdate " + x
                               };
                jumpList.JumpItems.Add(jumpTask);
                for (var y = 0; y < Projects[x].UpdateNames.Count; y++)
                {
                    jumpTask = new JumpTask
                                   {
                                       ApplicationPath = Base.AppDir + "SevenUpdate.Sdk.exe",
                                       IconResourcePath = Base.AppDir + "SevenUpdate.Sdk.exe",
                                       Title = Resources.Edit + " " + Projects[x].UpdateNames[y],
                                       CustomCategory = Projects[x].ApplicationName,
                                       Arguments = "-edit " + x + " " + y
                                   };
                    jumpList.JumpItems.Add(jumpTask);
                }
            }

            //Configure a new JumpTask
            jumpTask = new JumpTask
                           {
                               ApplicationPath = Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = Base.AppDir + "SevenUpdate.Sdk.exe",
                               Title = Resources.CreateProject,
                               CustomCategory = "Tasks",
                               Arguments = "-newproject"
                           };
            jumpList.JumpItems.Add(jumpTask);

            JumpList.SetJumpList(Application.Current, jumpList);
        }

        internal static void EditItem()
        {
            IsNewProject = false;
            AppInfo = Base.Deserialize<Sua>(UserStore + Projects[AppIndex].ApplicationName + ".sua");
            if (UpdateIndex < 0)
                MainWindow.NavService.Navigate(new Uri(@"Pages\AppInfo.xaml", UriKind.Relative));
            else
            {
                UpdateInfo = Base.Deserialize<Collection<Update>>(UserStore + Projects[AppIndex].ApplicationName + ".sui")[UpdateIndex];
                if (UpdateInfo.Files == null)
                    UpdateInfo.Files = new ObservableCollection<UpdateFile>();
                if (UpdateInfo.Shortcuts == null)
                    UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();
                if (UpdateInfo.RegistryItems == null)
                    UpdateInfo.RegistryItems = new ObservableCollection<RegistryItem>();
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
            }
        }

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
            MainWindow.NavService.Navigate(new Uri(@"Pages\AppInfo.xaml", UriKind.Relative));
        }

        internal static void NewUpdate()
        {
            IsNewProject = false;
            AppInfo = Base.Deserialize<Sua>(UserStore + Projects[AppIndex].ApplicationName + ".sua");
            UpdateInfo = new Update
                             {
                                 Files = new ObservableCollection<UpdateFile>(),
                                 RegistryItems = new ObservableCollection<RegistryItem>(),
                                 Shortcuts = new ObservableCollection<Shortcut>(),
                                 Description = new ObservableCollection<LocaleString>(),
                                 Name = new ObservableCollection<LocaleString>()
                             };
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        /// <summary>
        ///   Checks if a file or UNC is valid
        /// </summary>
        /// <param name = "path">The path we want to check</param>
        /// <param name = "is64Bit">Specifies if the application is 64 bit</param>
        public static bool IsValidFilePath(string path, bool is64Bit)
        {
            path = Base.ConvertPath(path, true, is64Bit);
            const string pattern = @"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$";
            var reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(path);
        }

        #endregion

        #region CommonFileDialog

        /// <summary>
        ///   Opens a OpenFileDialog
        /// </summary>
        /// <param name = "initialDirectory">Gets or sets the inital directory displayed when the dialog is shown. A null or empty string indicates that the dialog is using the default directory</param>
        /// <param name = "multiSelect">Gets or sets a value that determines whether the user can select more than one file</param>
        /// <param name = "defaultDirectory">Sets the folder or path used as a default if there is not a recently used folder value available</param>
        /// <param name = "defaultFileName">Sets the default file name</param>
        /// <param name = "defaultExtension">Gets or sets the default file extension to be added to the file names. If the value is null or empty, the extension is not added to the file names</param>
        /// <param name = "navigateToShortcut">Gets or sets a value that controls whether shortcuts should be treated as their target items, allowing an application to open a .lnk file</param>
        /// <returns>A collection of the selected files</returns>
        internal static string[] OpenFileDialog(string initialDirectory = null, bool multiSelect = false, string defaultDirectory = null, string defaultFileName = null, string defaultExtension = null,
                                                bool navigateToShortcut = false)
        {
            if (CoreHelpers.RunningOnXP)
            {
                var ofd = new OpenFileDialog
                              {
                                  Multiselect = multiSelect,
                                  FileName = defaultFileName,
                                  CheckFileExists = true,
                                  DefaultExt = defaultExtension,
                                  InitialDirectory = initialDirectory,
                                  DereferenceLinks = navigateToShortcut
                              };

                switch (defaultExtension)
                {
                    case "sua":
                        ofd.Filter = Resources.Sua + " (*.sua)|*.sua";
                        break;
                    case "sui":
                        ofd.Filter = Resources.Sui + " (*.sui)|*.sui";
                        break;
                    case "reg":
                        ofd.Filter = Resources.RegFile + " (*.reg)|*.reg";
                        break;
                    case "lnk":
                        ofd.Filter = Resources.Shortcut + " (*.lnk)|*.lnk";
                        break;
                    default:
                        ofd.AddExtension = false;
                        ofd.Filter = Resources.AllFiles + "|*.*";
                        break;
                }
                return ofd.ShowDialog(Application.Current.MainWindow).GetValueOrDefault() ? ofd.FileNames : null;
            }

            var cfd = new CommonOpenFileDialog
                          {
                              InitialDirectory = initialDirectory,
                              EnsureFileExists = true,
                              EnsureValidNames = true,
                              Multiselect = true,
                              DefaultDirectory = defaultDirectory,
                              NavigateToShortcut = navigateToShortcut,
                              DefaultExtension = defaultExtension,
                              DefaultFileName = defaultFileName
                          };
            switch (defaultExtension)
            {
                case "sua":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.Sua, "*.sua"));
                    break;
                case "sui":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.Sui, "*.sui"));
                    break;
                case "reg":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.RegFile, "*.reg"));
                    break;
                case "lnk":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.Shortcut, "*.lnk"));
                    break;
            }
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
            {
                var fileNames = new string[cfd.FileNames.Count];
                cfd.FileNames.CopyTo(fileNames, 0);
                return fileNames;
            }
            return null;
        }

        /// <summary>
        ///   Opens a SaveFileDialog
        /// </summary>
        /// <param name = "initialDirectory">Gets or sets the inital directory displayed when the dialog is shown. A null or empty string indicates that the dialog is using the default directory</param>
        /// <param name = "defaultDirectory">Sets the folder or path used as a default if there is not a recently used folder value available</param>
        /// <param name = "defaultFileName">Sets the default file name</param>
        /// <param name = "defaultExtension">Gets or sets the default file extension to be added to the file names. If the value is null or empty, the extension is not added to the file names</param>
        /// <returns>Gets the selected filename</returns>
        internal static string SaveFileDialog(string initialDirectory, string defaultDirectory, string defaultFileName, string defaultExtension = null)
        {
            if (CoreHelpers.RunningOnXP)
            {
                var ofd = new SaveFileDialog
                              {FileName = defaultFileName, CheckFileExists = true, DefaultExt = defaultExtension, AddExtension = true, InitialDirectory = initialDirectory, ValidateNames = true};

                switch (defaultExtension)
                {
                    case "sua":
                        ofd.Filter = Resources.Sua + " (*.sua)|*.sua";
                        break;
                    case "sui":
                        ofd.Filter = Resources.Sui + " (*.sui)|*.sui";
                        break;
                    case "reg":
                        ofd.Filter = Resources.RegFile + " (*.reg)|*.reg";
                        break;
                    case "lnk":
                        ofd.Filter = Resources.Shortcut + " (*.lnk)|*.lnk";
                        break;
                    default:
                        ofd.AddExtension = false;
                        ofd.Filter = Resources.AllFiles + "|*.*";
                        break;
                }
                return ofd.ShowDialog(Application.Current.MainWindow).GetValueOrDefault() ? ofd.FileName : null;
            }

            var cfd = new CommonSaveFileDialog
                          {
                              InitialDirectory = initialDirectory,
                              EnsureValidNames = true,
                              EnsureFileExists = true,
                              DefaultDirectory = defaultDirectory,
                              DefaultExtension = defaultExtension,
                              DefaultFileName = defaultFileName,
                              AlwaysAppendDefaultExtension = true,
                              AddToMostRecentlyUsedList = true
                          };
            switch (defaultExtension)
            {
                case "sua":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.Sua, "*.sua"));
                    break;
                case "sui":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.Sui, "*.sui"));
                    break;
                case "reg":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.RegFile, "*.reg"));
                    break;
                case "lnk":
                    cfd.Filters.Add(new CommonFileDialogFilter(Resources.Shortcut, "*.lnk"));
                    break;
                default:
                    cfd.AlwaysAppendDefaultExtension = false;
                    break;
            }
            return cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK ? cfd.FileName : null;
        }

        #endregion

        #region TaskDialog Methods

        /// <summary>
        ///   Shows either a TaskDialog or a MessageBox if running legacy windows.
        /// </summary>
        /// <param name = "instructionText">The main text to display (Blue 14pt for TaskDialog)</param>
        /// <param name = "description">A description of the message, supplements the instruction text</param>
        /// <returns>Returns the result of the message</returns>
        internal static TaskDialogResult ShowMessage(string instructionText, string description)
        {
            return ShowMessage(instructionText, TaskDialogStandardIcon.None, TaskDialogStandardButtons.Ok, description);
        }

        /// <summary>
        ///   Shows either a TaskDialog or a MessageBox if running legacy windows.
        /// </summary>
        /// <param name = "instructionText">The main text to display (Blue 14pt for TaskDialog)</param>
        /// <param name = "description">A description of the message, supplements the instruction text</param>
        /// <param name = "icon"></param>
        /// <returns>Returns the result of the message</returns>
        internal static TaskDialogResult ShowMessage(string instructionText, TaskDialogStandardIcon icon, string description = null)
        {
            return ShowMessage(instructionText, icon, TaskDialogStandardButtons.Ok, description);
        }

        /// <summary>
        ///   Shows either a TaskDialog or a MessageBox if running legacy windows.
        /// </summary>
        /// <param name = "instructionText">The main text to display (Blue 14pt for TaskDialog)</param>
        /// <param name = "description">A description of the message, supplements the instruction text</param>
        /// <param name = "defaultButtonText">Text to display on the button</param>
        /// <param name = "displayShieldOnButton">Indicates if a UAC shield is to be displayed on the defaultButton</param>
        /// <returns>Returns the result of the message</returns>
        internal static TaskDialogResult ShowMessage(string instructionText, string description, string defaultButtonText, bool displayShieldOnButton)
        {
            return ShowMessage(instructionText, TaskDialogStandardIcon.None, TaskDialogStandardButtons.Cancel, description, null, defaultButtonText, displayShieldOnButton);
        }

        /// <summary>
        ///   Shows either a TaskDialog or a MessageBox if running legacy windows.
        /// </summary>
        /// <param name = "instructionText">The main text to display (Blue 14pt for TaskDialog)</param>
        /// <param name = "icon">The icon to use</param>
        /// <param name = "standardButtons">The standard buttons to use (with or without the custom default button text)</param>
        /// <param name = "description">A description of the message, supplements the instruction text</param>
        /// <param name = "footerText">Text to display as a footer message</param>
        /// <param name = "defaultButtonText">Text to display on the button</param>
        /// <param name = "displayShieldOnButton">Indicates if a UAC shield is to be displayed on the defaultButton</param>
        /// <returns>Returns the result of the message</returns>
        internal static TaskDialogResult ShowMessage(string instructionText, TaskDialogStandardIcon icon, TaskDialogStandardButtons standardButtons, string description = null, string footerText = null,
                                                     string defaultButtonText = null, bool displayShieldOnButton = false)
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
                                 Cancelable = true,
                             };
                if (defaultButtonText != null)
                {
                    var button = new TaskDialogButton("btnCustom", defaultButtonText) {Default = true, ShowElevationIcon = displayShieldOnButton};
                    td.Controls.Add(button);

                    switch (standardButtons)
                    {
                        case TaskDialogStandardButtons.Ok:
                            button = new TaskDialogButton("btnOK", "OK") {Default = false};
                            td.Controls.Add(button);
                            break;
                        case TaskDialogStandardButtons.Cancel:
                            button = new TaskDialogButton("btnCancel", "Cancel") {Default = false};
                            td.Controls.Add(button);
                            break;
                        case TaskDialogStandardButtons.Retry:
                            button = new TaskDialogButton("btnRetry", "Retry") {Default = false};
                            td.Controls.Add(button);
                            break;
                        case TaskDialogStandardButtons.Close:
                            button = new TaskDialogButton("btnClose", "Close") {Default = false};
                            td.Controls.Add(button);
                            break;
                    }
                }
                else
                    td.StandardButtons = standardButtons;

                return td.ShowDialog(Application.Current.MainWindow);
            }
            var message = instructionText;
            var msgIcon = MessageBoxImage.None;

            if (description != null)
                message += Environment.NewLine + description;

            if (footerText != null)
                message += Environment.NewLine + footerText;

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

            var result = MessageBox.Show(message, Resources.SevenUpdateSDK, MessageBoxButton.OK, msgIcon);

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

        #region Event methods

        internal static void Base_SerializationError(object sender, SerializationErrorEventArgs e)
        {
            ShowMessage(Resources.ProjectLoadError, TaskDialogStandardIcon.Error, e.Exception.Message);
        }

        internal static void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
    }
}