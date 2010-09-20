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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Internal;
using SevenUpdate.Pages;
using SevenUpdate.Properties;

#endregion

namespace SevenUpdate
{
    internal sealed class Core : INotifyPropertyChanged
    {
        #region Fields

        private static Core instance;

        private static UpdateAction updateAction;

        private static bool isAdmin;
        internal static Core Instance { get { return instance ?? (instance = new Core()); } }

        #endregion

        #region Properties

        public UpdateAction UpdateAction
        {
            get { return updateAction; }
            set
            {
                updateAction = value;
                OnPropertyChanged("UpdateAction");
                if (UpdateActionChanged != null)
                    UpdateActionChanged(this, new EventArgs());
            }
        }

        /// <summary>
        ///   Gets or Sets a value indicating if the current user enabled admin access
        /// </summary>
        public bool IsAdmin
        {
            get { return CoreNativeMethods.IsUserAnAdmin() || isAdmin; }

            set
            {
                isAdmin = value;
                OnPropertyChanged("IsAdmin");
            }
        }

        /// <summary>
        ///   Gets a collection of software that Seven Update can check for updates
        /// </summary>
        private static IEnumerable<Sua> AppsToUpdate { get { return Base.Deserialize<Collection<Sua>>(Base.AppsFile); } }

        /// <summary>
        ///   Gets the update configuration settings
        /// </summary>
        public static Config Settings
        {
            get
            {
                var t = Base.Deserialize<Config>(Base.ConfigFile);
                return t ?? new Config {AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false};
            }
        }

        /// <summary>
        ///   Gets or Sets a collection of applications to update
        /// </summary>
        internal static Collection<Sui> Applications { get; set; }

        /// <summary>
        ///   Gets a value indicating if an auto check is being performed
        /// </summary>
        internal static bool IsAutoCheck { get; set; }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress
        /// </summary>
        internal static bool IsInstallInProgress { private get; set; }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress and Seven Update was started after an autocheck
        /// </summary>
        internal static bool IsReconnect { get; set; }

        #endregion

        #region Recount Methods

        /// <summary>
        ///   Gets the total size of a single update
        /// </summary>
        /// <param name = "files">the collection of files of an update</param>
        /// <returns>a ulong value of the size of the update</returns>
        internal static ulong GetUpdateSize(IEnumerable<UpdateFile> files)
        {
            return files.Aggregate<UpdateFile, ulong>(0, (current, t) => current + t.FileSize);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///   Checks for updates
        /// </summary>
        /// <param name = "auto"><c>true</c> if it's called because of an auto update check, otherwise <c>false</c></param>
        internal static void CheckForUpdates(bool auto)
        {
            if (auto)
            {
                if (!IsInstallInProgress && !Base.RebootNeeded)
                    CheckForUpdates();
            }
            else
                CheckForUpdates();
        }

        /// <summary>
        ///   Checks for updates
        /// </summary>
        internal static void CheckForUpdates()
        {
            if (!IsInstallInProgress)
            {
                if (Base.RebootNeeded == false)
                {
                    Instance.UpdateAction = UpdateAction.CheckingForUpdates;
                    Properties.Settings.Default.lastUpdateCheck = DateTime.Now;
                    IsInstallInProgress = true;
                    Search.SearchForUpdatesAync(AppsToUpdate);
                }
                else
                {
                    Instance.UpdateAction = UpdateAction.RebootNeeded;
                    if (ShowMessage(Resources.RebootComputer, TaskDialogStandardIcon.Information, TaskDialogStandardButtons.Cancel, Resources.RebootNeededFirst, null, Resources.RestartNow) !=
                        TaskDialogResult.Cancel)
                        Base.StartProcess("shutdown.exe", "-r -t 00");
                }
            }
            else
                ShowMessage(null, TaskDialogStandardIcon.Information, Resources.AlreadyUpdating);
        }

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #region TaskDialog Methods

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
                                 Caption = Resources.SevenUpdate,
                                 InstructionText = instructionText,
                                 Text = description,
                                 Icon = icon,
                                 FooterText = footerText,
                                 FooterIcon = TaskDialogStandardIcon.Information,
                                 Cancelable = true,
                                 StandardButtons = standardButtons
                             };
                if (defaultButtonText != null)
                {
                    var button = new TaskDialogButton("btnCustom", defaultButtonText) {Default = true, ShowElevationIcon = displayShieldOnButton};
                    td.Controls.Add(button);
                }

                return Application.Current == null ? td.Show() : td.ShowDialog(Application.Current.MainWindow);
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

            var result = MessageBox.Show(message, Resources.SevenUpdate, MessageBoxButton.OK, msgIcon);

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

        #region Event Declarations

        /// <summary>
        ///   Occurs when the user cancels their update selection
        /// </summary>
        internal static event EventHandler UpdateActionChanged;

        #endregion
    }
}