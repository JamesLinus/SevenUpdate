// ***********************************************************************
// Assembly         : SevenUpdate
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Shell;

    using Microsoft.Windows.Dialogs;
    using Microsoft.Windows.Dialogs.TaskDialogs;
    using Microsoft.Windows.Internal;

    using SevenUpdate.Pages;
    using SevenUpdate.Properties;

    /// <summary>
    /// </summary>
    internal sealed class Core : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal static NavigationService NavService;

        /// <summary>
        /// </summary>
        internal static TaskbarItemInfo TaskBar;

        /// <summary>
        /// </summary>
        private static Core instance;

        /// <summary>
        /// </summary>
        private static bool isAdmin;

        /// <summary>
        /// </summary>
        private static UpdateAction updateAction;

        #endregion

        #region Events

        /// <summary>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   Occurs when the user cancels their update selection
        /// </summary>
        internal static event EventHandler UpdateActionChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the update configuration settings
        /// </summary>
        public static Config Settings
        {
            get
            {
                var t = Base.Deserialize<Config>(Base.ConfigFile);
                return t ?? new Config { AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false };
            }
        }

        /// <summary>
        ///   Gets or Sets a value indicating if the current user enabled admin access
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                return CoreNativeMethods.IsUserAnAdmin() || isAdmin;
            }

            set
            {
                isAdmin = value;
                this.OnPropertyChanged("IsAdmin");
            }
        }

        /// <summary>
        /// </summary>
        public UpdateAction UpdateAction
        {
            get
            {
                return updateAction;
            }

            set
            {
                updateAction = value;
                this.OnPropertyChanged("UpdateAction");
                if (UpdateActionChanged != null)
                {
                    UpdateActionChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        ///   Gets or Sets a collection of applications to update
        /// </summary>
        internal static Collection<Sui> Applications { get; set; }

        /// <summary>
        /// </summary>
        internal static Core Instance
        {
            get
            {
                return instance ?? (instance = new Core());
            }
        }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress
        /// </summary>
        internal static bool IsInstallInProgress { private get; set; }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress and Seven Update was started after an auto check
        /// </summary>
        internal static bool IsReconnect { get; set; }

        /// <summary>
        ///   Gets a collection of software that Seven Update can check for updates
        /// </summary>
        private static IEnumerable<Sua> AppsToUpdate
        {
            get
            {
                return Base.Deserialize<Collection<Sua>>(Base.AppsFile);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks for updates
        /// </summary>
        /// <param name="auto">
        /// <c>true</c> if it's called because of an auto update check, otherwise <c>false</c>
        /// </param>
        internal static void CheckForUpdates(bool auto)
        {
            if (auto)
            {
                if (!IsInstallInProgress && !Base.RebootNeeded)
                {
                    CheckForUpdates();
                }
            }
            else
            {
                CheckForUpdates();
            }
        }

        /// <summary>
        /// Checks for updates
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
                    Search.SearchForUpdatesAsync(AppsToUpdate);
                }
                else
                {
                    Instance.UpdateAction = UpdateAction.RebootNeeded;
                    if (
                        ShowMessage(
                            Resources.RebootComputer, 
                            TaskDialogStandardIcon.Information, 
                            TaskDialogStandardButtons.Cancel, 
                            Resources.RebootNeededFirst, 
                            null, 
                            Resources.RestartNow) != TaskDialogResult.Cancel)
                    {
                        Base.StartProcess("shutdown.exe", "-r -t 00");
                    }
                }
            }
            else
            {
                ShowMessage(null, TaskDialogStandardIcon.Information, Resources.AlreadyUpdating);
            }
        }

        /// <summary>
        /// Gets the total size of a single update
        /// </summary>
        /// <param name="files">
        /// the collection of files of an update
        /// </param>
        /// <returns>
        /// a ulong value of the size of the update
        /// </returns>
        internal static ulong GetUpdateSize(IEnumerable<UpdateFile> files)
        {
            return files.Aggregate<UpdateFile, ulong>(0, (current, t) => current + t.FileSize);
        }

        /// <summary>
        /// </summary>
        internal static void SetJumpList()
        {
            var jumpList = new JumpList();

            var jumpTask = new JumpTask
                {
                    ApplicationPath = Base.AppDir + @"SevenUpdate.exe", 
                    IconResourcePath = Base.AppDir + @"SevenUpdate.Base.dll", 
                    IconResourceIndex = 2, 
                    Title = Resources.CheckForUpdates, 
                    CustomCategory = Resources.Tasks, 
                    Arguments = "-check", 
                };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                {
                    ApplicationPath = Base.AppDir + @"SevenUpdate.exe", 
                    IconResourcePath = Base.AppDir + @"SevenUpdate.Base.dll", 
                    IconResourceIndex = 5, 
                    Title = Resources.RestoreHiddenUpdates, 
                    CustomCategory = Resources.Tasks, 
                    Arguments = "-hidden", 
                };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                {
                    ApplicationPath = Base.AppDir + @"SevenUpdate.eye", 
                    IconResourcePath = Base.AppDir + @"SevenUpdate.Base.dll", 
                    IconResourceIndex = 4, 
                    Title = Resources.ViewUpdateHistory, 
                    CustomCategory = Resources.Tasks, 
                    Arguments = "-history", 
                };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                {
                    ApplicationPath = Base.AppDir + @"SevenUpdate.exe", 
                    IconResourcePath = Base.AppDir + @"SevenUpdate.Base.dll", 
                    IconResourceIndex = 3, 
                    Title = Resources.ChangeSettings, 
                    CustomCategory = Resources.Tasks, 
                    Arguments = "-settings", 
                };

            jumpList.JumpItems.Add(jumpTask);

            JumpList.SetJumpList(Application.Current, jumpList);
        }

        /// <summary>
        /// Shows either a <see cref="TaskDialog"/> or a <see cref="MessageBox"/> if running legacy windows.
        /// </summary>
        /// <param name="instructionText">
        /// The main text to display (Blue 14pt for <see cref="TaskDialog"/>)
        /// </param>
        /// <param name="icon">
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
        /// Shows either a <see cref="TaskDialog"/> or a <see cref="MessageBox"/> if running legacy windows.
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
        internal static TaskDialogResult ShowMessage(
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
                    var button = new TaskDialogButton("btnCustom", defaultButtonText) { Default = true, ShowElevationIcon = displayShieldOnButton };
                    td.Controls.Add(button);
                }

                return Application.Current == null ? td.Show() : td.ShowDialog(Application.Current.MainWindow);
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
                result = MessageBox.Show(message, Resources.SevenUpdate, MessageBoxButton.OKCancel, msgIcon);
            }
            else
            {
                result = MessageBox.Show(message, Resources.SevenUpdate, MessageBoxButton.OK, msgIcon);
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

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="name">
        /// </param>
        private void OnPropertyChanged(string name)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}