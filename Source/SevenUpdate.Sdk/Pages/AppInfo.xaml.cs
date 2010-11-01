// ***********************************************************************
// <copyright file="AppInfo.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Dialogs;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenUpdate.Sdk.ValidationRules;
    using SevenUpdate.Sdk.Windows;

    using Application = System.Windows.Application;
    using TextBox = System.Windows.Controls.TextBox;

    /// <summary>Interaction logic for AppInfo.xaml</summary>
    public sealed partial class AppInfo
    {
        #region Fields

       /// <summary>Indicates if the application is 64 bit</summary>
        private static bool is64Bit;

        #endregion
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "AppInfo" /> class.</summary>
        public AppInfo()
        {
            this.InitializeComponent();
            this.DataContext = Core.AppInfo;

            if (Core.AppIndex > -1)
            {
                this.btnNext.Visibility = Visibility.Collapsed;
                this.btnCancel.Content = Properties.Resources.Save;
            }
            else
            {
                this.btnCancel.Content = Properties.Resources.Cancel;
                this.btnNext.Visibility = Visibility.Visible;
            }

            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged += this.UpdateUI;
            if (AeroGlass.IsEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        /// <summary>Opens a <see cref = "OpenFileDialog" /> to browse for the application install location</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void BrowseForAppLocation(object sender, MouseButtonEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog(Application.Current.MainWindow.GetIWin32Window()) == DialogResult.OK)
                {
                    this.tbxAppLocation.Text = Utilities.ConvertPath(folderBrowserDialog.SelectedPath, false, Core.AppInfo.Is64Bit);
                }
            }
        }

        /// <summary>Changes the UI to show the file system application location</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void ChangeToFileSystemLocation(object sender, RoutedEventArgs e)
        {
            if (this.tbxAppLocation == null)
            {
                return;
            }

            this.tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule
                {
                    IsRegistryPath = false
                };

            // ReSharper disable PossibleNullReferenceException
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);

            // ReSharper restore PossibleNullReferenceException
            Core.AppInfo.ValueName = null;
        }

        /// <summary>Changes the UI to show the registry application location</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void ChangeToRegistryLocation(object sender, RoutedEventArgs e)
        {
            if (this.tbxAppLocation == null)
            {
                return;
            }

            this.tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule
                {
                    IsRegistryPath = true
                };

            // ReSharper disable PossibleNullReferenceException
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>Converts the application location path to system variables</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.Input.KeyboardFocusChangedEventArgs" /> instance containing the event data.</param>
        private void ConvertPath(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.rbtnFileSystem.IsChecked.GetValueOrDefault())
            {
                this.tbxAppLocation.Text = Utilities.ConvertPath(this.tbxAppLocation.Text, false, Core.AppInfo.Is64Bit);
            }
        }

        /// <summary>Determines whether this instance has errors.</summary>
        /// <returns><see langword = "true" /> if this instance has errors; otherwise, <see langword = "false" />.</returns>
        private bool HasErrors()
        {
            if (this.rbtnRegistry.IsChecked.GetValueOrDefault() && tbxValueName.HasError)
            {
                return true;
            }

            return tbxAppName.HasError || tbxPublisher.HasError || tbxAppUrl.HasError || tbxHelpUrl.HasError || tbxAppLocation.HasError || tbxAppDescription.HasError || tbxSuiUrl.HasError;
        }

        /// <summary>Loads the application info into the UI.</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void LoadAppInfo(object sender, RoutedEventArgs e)
        {
            tbxAppName.HasError = String.IsNullOrWhiteSpace(tbxAppName.Text);
            tbxAppDescription.HasError = String.IsNullOrWhiteSpace(tbxAppDescription.Text);
            tbxPublisher.HasError = String.IsNullOrWhiteSpace(tbxPublisher.Text);
            
            // ReSharper disable PossibleNullReferenceException
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxValueName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxAppUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxHelpUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxSuiUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // ReSharper restore PossibleNullReferenceException
            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppName.Text = t.Value;
            }

            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppDescription.Text = t.Value;
            }

            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxPublisher.Text = t.Value;
            }

            is64Bit = Core.AppInfo.Is64Bit;
        }

        /// <summary>Loads the <see cref = "LocaleString" />'s for the <see cref = "Sua" /> into the UI</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void LoadLocaleStrings(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxAppName == null || this.cbxLocale.SelectedIndex < 0)
            {
                return;
            }

            Utilities.Locale = ((ComboBoxItem)this.cbxLocale.SelectedItem).Tag.ToString();

            var found = false;

            // Load Values
            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppDescription.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxAppDescription.Text = null;
            }

            found = false;

            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppName.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxAppName.Text = null;
            }

            found = false;

            // Load Values
            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxPublisher.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxPublisher.Text = null;
            }
        }

        /// <summary>Moves on to the next pages if no errors are present</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateInfo.xaml", UriKind.Relative));
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>Saves the project and goes back to the main page</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void SaveProject(object sender, RoutedEventArgs e)
        {
            if (this.btnNext.Visibility != Visibility.Visible)
            {
                if (this.HasErrors())
                {
                    Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
                }
                else
                {
                    var appName = Utilities.GetLocaleString(Core.AppInfo.Name);
                    File.Delete(App.UserStore + appName + ".sua");
                    if (Core.AppInfo.Is64Bit)
                    {
                        if (!appName.Contains("x64") && !appName.Contains("X64"))
                        {
                            appName += " (x64)";
                        }
                    }

                    File.Delete(App.UserStore + appName + ".sua");

                    ObservableCollection<string> updateNames = null;

                    if (Core.AppIndex > -1)
                    {
                        updateNames = Core.Projects[Core.AppIndex].UpdateNames;
                        Core.Projects.RemoveAt(Core.AppIndex);
                    }

                    // Save the SUA file
                    Utilities.Serialize(Core.AppInfo, App.UserStore + appName + ".sua");

                    // Save project file
                    var project = new Project
                        {
                            ApplicationName = appName,
                        };

                    if (updateNames != null)
                    {
                        foreach (var t in updateNames)
                        {
                            project.UpdateNames.Add(t);
                        }
                    }

                    if (Core.AppInfo.Is64Bit != is64Bit)
                    {
                        project.UpdateNames.Clear();
                        File.Delete(App.UserStore + appName + ".sui");
                    }

                    Core.Projects.Add(project);
                    Utilities.Serialize(Core.Projects, Core.ProjectsFile);
                    MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
                }
            }
            else
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
            }
        }

        /// <summary>Changes the UI depending on whether Aero Glass is enabled.</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "CompositionChangedEventArgs" /> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void ChangeDescription(object sender, RoutedEventArgs e)
        {
            var textBox = ((InfoTextBox)sender);
            textBox.HasError = String.IsNullOrWhiteSpace(textBox.Text);
            Core.UpdateLocaleStrings(textBox.Text, Core.AppInfo.Description);
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void ChangePublisher(object sender, RoutedEventArgs e)
        {
            var textBox = ((InfoTextBox)sender);
            textBox.HasError = String.IsNullOrWhiteSpace(textBox.Text);
            Core.UpdateLocaleStrings(textBox.Text, Core.AppInfo.Publisher);
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void ChangeName(object sender, RoutedEventArgs e)
        {
            var textBox = ((InfoTextBox)sender);
            textBox.HasError = String.IsNullOrWhiteSpace(textBox.Text);
            Core.UpdateLocaleStrings(textBox.Text, Core.AppInfo.Name);
        }

        #endregion
    }
}