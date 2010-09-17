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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Helpers;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for AppInfo.xaml
    /// </summary>
    public sealed partial class AppInfo
    {
        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the AppInfo page
        /// </summary>
        public AppInfo()
        {
            InitializeComponent();
            DataContext = Core.AppInfo;

            if (Environment.OSVersion.Version.Major < 6)
                return;
            if (Core.AppIndex > -1)
            {
                btnNext.Visibility = Visibility.Collapsed;
                btnCancel.Content = Properties.Resources.Save;
            }
            else
            {
                btnCancel.Content = Properties.Resources.Cancel;
                btnNext.Visibility = Visibility.Visible;
            }

            MouseLeftButtonDown += Core.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
            if (AeroGlass.IsEnabled)
            {
                tbTitle.Foreground = Brushes.Black;
                line.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                line.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;
                if (Environment.OSVersion.Version.Major < 6)
                {
                    tbTitle.TextEffects.Clear();
                }
            }
        }

        #endregion

        #region Methods

        private void LoadInfo()
        {
            // ReSharper disable PossibleNullReferenceException
            tbxPublisher.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxValueName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxHelpUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxSuiUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            // ReSharper restore PossibleNullReferenceException
            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Base.Locale))
                tbxAppName.Text = t.Value;

            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Base.Locale))
                tbxAppDescription.Text = t.Value;

            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Base.Locale))
                tbxPublisher.Text = t.Value;
        }

        private bool HasErrors()
        {
            if (rbtnRegistry.IsChecked.GetValueOrDefault() && tbxValueName.GetBindingExpression(TextBox.TextProperty).HasError)
                return true;

            return tbxAppName.GetBindingExpression(TextBox.TextProperty).HasError || tbxPublisher.GetBindingExpression(TextBox.TextProperty).HasError ||
                   tbxAppUrl.GetBindingExpression(TextBox.TextProperty).HasError || tbxHelpUrl.GetBindingExpression(TextBox.TextProperty).HasError ||
                   tbxAppLocation.GetBindingExpression(TextBox.TextProperty).HasError || tbxAppDescription.GetBindingExpression(TextBox.TextProperty).HasError ||
                   tbxSuiUrl.GetBindingExpression(TextBox.TextProperty).HasError;
        }

        #endregion

        #region UI Events

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!HasErrors())
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
            else
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (btnNext.Visibility != Visibility.Visible)
            {
                if (HasErrors())
                    Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
                else
                {
                    var appName = Base.GetLocaleString(Core.AppInfo.Name);
                    if (Core.AppInfo.Is64Bit)
                    {
                        if (!appName.Contains("x64") && !appName.Contains("X64"))
                            appName += " (x64)";
                    }

                    ObservableCollection<string> updateNames = null;

                    if (Core.AppIndex > -1)
                    {
                        updateNames = Core.Projects[Core.AppIndex].UpdateNames;
                        Core.Projects.RemoveAt(Core.AppIndex);
                    }

                    // Save the SUA file
                    Base.Serialize(Core.AppInfo, Core.UserStore + appName + ".sua");

                    // Save project file
                    var project = new Project {ApplicationName = appName, UpdateNames = updateNames};

                    Core.Projects.Add(project);
                    Base.Serialize(Core.Projects, Core.ProjectsFile);
                    MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
                }
            }
            else
                MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        private void Browse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                tbxAppLocation.Text = Base.ConvertPath(cfd.FileName, false, Core.AppInfo.Is64Bit);
        }

        #endregion

        #region TextBlock - Lost Focus

        private void tbxAppLocation_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (rbtnFileSystem.IsChecked.GetValueOrDefault())
                tbxAppLocation.Text = Base.ConvertPath(tbxAppLocation.Text, false, Core.AppInfo.Is64Bit);
        }

        #endregion

        #region ComboBox - Selection Changed

        private void Locale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxAppName == null || cbxLocale.SelectedIndex < 0)
                return;

            Base.Locale = ((ComboBoxItem) cbxLocale.SelectedItem).Tag.ToString();

            var found = false;
            // Load Values
            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Base.Locale))
            {
                tbxAppDescription.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxAppDescription.Text = null;

            found = false;
            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Base.Locale))
            {
                tbxAppName.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxAppName.Text = null;


            found = false;
            // Load Values
            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Base.Locale))
            {
                tbxPublisher.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxPublisher.Text = null;
        }

        #endregion

        #region Radio Button - Checked

        private void rbtnRegistry_Checked(object sender, RoutedEventArgs e)
        {
            if (tbxAppLocation == null)
                return;
            tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule {IsRegistryPath = true};
            // ReSharper disable PossibleNullReferenceException
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);
            // ReSharper restore PossibleNullReferenceException
        }

        private void rbtnFileSystem_Checked(object sender, RoutedEventArgs e)
        {
            if (tbxAppLocation == null)
                return;

            tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule {IsRegistryPath = false};
            // ReSharper disable PossibleNullReferenceException
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);
            // ReSharper restore PossibleNullReferenceException
            Core.AppInfo.ValueName = null;
        }

        #endregion

        #region Page

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                tbTitle.Foreground = Brushes.Black;
                line.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                line.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #endregion
    }
}