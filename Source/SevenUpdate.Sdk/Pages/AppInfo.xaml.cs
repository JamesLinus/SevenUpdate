// ***********************************************************************
// Assembly         : SevenUpdate.Sdk
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;

    using Microsoft.Windows.Dialogs;
    using Microsoft.Windows.Dialogs.TaskDialogs;
    using Microsoft.Windows.Dwm;

    using SevenUpdate.Sdk.Helpers;
    using SevenUpdate.Sdk.Windows;

    using Application = System.Windows.Application;
    using TextBox = System.Windows.Controls.TextBox;

    /// <summary>
    /// Interaction logic for AppInfo.xaml
    /// </summary>
    public sealed partial class AppInfo
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AppInfo" /> class.
        /// </summary>
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
            AeroGlass.DwmCompositionChanged += this.UpdateUI;
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

        /// <summary>
        /// Opens a <see cref="OpenFileDialog"/> to browse for the application install location
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.
        /// </param>
        private void BrowseForAppLocation(object sender, MouseButtonEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog(Application.Current.MainWindow.GetIWin32Window()) == DialogResult.OK)
            {
                this.tbxAppLocation.Text = Base.ConvertPath(folderBrowserDialog.SelectedPath, false, Core.AppInfo.Is64Bit);
            }
        }

        /// <summary>
        /// Changes the UI to show the file system application location
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void ChangeToFileSystemLocation(object sender, RoutedEventArgs e)
        {
            if (this.tbxAppLocation == null)
            {
                return;
            }

            this.tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule { IsRegistryPath = false };

            // ReSharper disable PossibleNullReferenceException
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);

            // ReSharper restore PossibleNullReferenceException
            Core.AppInfo.ValueName = null;
        }

        /// <summary>
        /// Changes the UI to show the registry application location
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void ChangeToRegistryLocation(object sender, RoutedEventArgs e)
        {
            if (this.tbxAppLocation == null)
            {
                return;
            }

            this.tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule { IsRegistryPath = true };

            // ReSharper disable PossibleNullReferenceException
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Converts the application location path to system variables
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void ConvertPath(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.rbtnFileSystem.IsChecked.GetValueOrDefault())
            {
                this.tbxAppLocation.Text = Base.ConvertPath(this.tbxAppLocation.Text, false, Core.AppInfo.Is64Bit);
            }
        }

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this instance has errors; otherwise, <see langword="false"/>.
        /// </returns>
        private bool HasErrors()
        {
            // ReSharper disable PossibleNullReferenceException
            if (this.rbtnRegistry.IsChecked.GetValueOrDefault() && this.tbxValueName.GetBindingExpression(TextBox.TextProperty).HasError)
            {
                return true;
            }

            return this.tbxAppName.GetBindingExpression(TextBox.TextProperty).HasError || this.tbxPublisher.GetBindingExpression(TextBox.TextProperty).HasError ||
                   this.tbxAppUrl.GetBindingExpression(TextBox.TextProperty).HasError || this.tbxHelpUrl.GetBindingExpression(TextBox.TextProperty).HasError ||
                   this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).HasError || this.tbxAppDescription.GetBindingExpression(TextBox.TextProperty).HasError ||
                   this.tbxSuiUrl.GetBindingExpression(TextBox.TextProperty).HasError;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Loads the application info into the UI.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void LoadAppInfo(object sender, RoutedEventArgs e)
        {
            // ReSharper disable PossibleNullReferenceException
            this.tbxPublisher.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxAppDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxAppName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxAppLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxValueName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxAppUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxHelpUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.tbxSuiUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // ReSharper restore PossibleNullReferenceException
            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Base.Locale))
            {
                this.tbxAppName.Text = t.Value;
            }

            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Base.Locale))
            {
                this.tbxAppDescription.Text = t.Value;
            }

            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Base.Locale))
            {
                this.tbxPublisher.Text = t.Value;
            }
        }

        /// <summary>
        /// Loads the <see cref="LocaleString"/>'s for the <see cref="Sua"/> into the UI
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void LoadLocaleStrings(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxAppName == null || this.cbxLocale.SelectedIndex < 0)
            {
                return;
            }

            Base.Locale = ((ComboBoxItem)this.cbxLocale.SelectedItem).Tag.ToString();

            var found = false;

            // Load Values
            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Base.Locale))
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
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Base.Locale))
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
            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Base.Locale))
            {
                this.tbxPublisher.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxPublisher.Text = null;
            }
        }

        /// <summary>
        /// Moves on to the next pages if no errors are present
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
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

        /// <summary>
        /// Saves the project and goes back to the main page
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
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
                    var appName = Base.GetLocaleString(Core.AppInfo.Name);
                    if (Core.AppInfo.Is64Bit)
                    {
                        if (!appName.Contains("x64") && !appName.Contains("X64"))
                        {
                            appName += " (x64)";
                        }
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
                    var project = new Project { ApplicationName = appName, UpdateNames = updateNames };

                    Core.Projects.Add(project);
                    Base.Serialize(Core.Projects, Core.ProjectsFile);
                    MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
                }
            }
            else
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// Changes the UI depending on whether Aero Glass is enabled.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="Microsoft.Windows.Dwm.AeroGlass.DwmCompositionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void UpdateUI(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
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

        #endregion
    }
}