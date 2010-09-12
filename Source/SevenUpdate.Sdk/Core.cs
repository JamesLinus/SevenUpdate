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
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Windows.Dialogs;
using SevenUpdate.Sdk.Properties;

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
        ///   The current update being edited
        /// </summary>
        internal static Update UpdateInfo { get; set; }

        internal static int SelectedShortcut { get; set; }

        internal static bool IsGlassEnabled { get; set; }

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

        internal static int UpdateIndex = -1;
        internal static int AppIndex = -1;

        #endregion

        #region Methods

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