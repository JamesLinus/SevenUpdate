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
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for Main.xaml
    /// </summary>
    public sealed partial class Main
    {
        #region Constructors

        /// <summary>
        ///   The constructor for the Main page
        /// </summary>
        public Main()
        {
            InitializeComponent();
            MouseLeftButtonDown += Core.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
            tbTitle.Foreground = AeroGlass.IsEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 102, 204));
            tbHelp.Foreground = AeroGlass.IsEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 102, 204));
            tbAbout.Foreground = AeroGlass.IsEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 102, 204));
        }

        #endregion

        #region Fields

        public static ObservableCollection<Project> Projects;

        #endregion

        private void LoadProjects()
        {
            treeView.Items.Clear();
            Projects = Base.Deserialize<ObservableCollection<Project>>(Core.ProjectsFile) ?? new ObservableCollection<Project>();
            if (Projects.Count <= 0)
                return;

            treeView.Visibility = Visibility.Visible;

            for (int x = 0; x < Projects.Count; x++)
            {
                var app = new TreeViewItem {Header = Projects[x].ApplicationName, Tag = x};

                for (int y = 0; y < Projects[x].UpdateNames.Count; y++)
                {
                    var index = new[] {x, y};
                    app.Items.Add(new TreeViewItem {Header = Projects[x].UpdateNames[y], Tag = index});
                }

                treeView.Items.Add(app);
            }
        }

        private void NewUpdate_Click(object sender, RoutedEventArgs e)
        {
            Core.AppInfo = Base.Deserialize<Sua>(Core.UserStore + Projects[Core.AppIndex].ApplicationName + ".sua");
            Core.UpdateInfo = new Update
                                  {
                                      Files = new ObservableCollection<UpdateFile>(),
                                      RegistryItems = new ObservableCollection<RegistryItem>(),
                                      Shortcuts = new ObservableCollection<Shortcut>(),
                                      Description = new ObservableCollection<LocaleString>(),
                                      Name = new ObservableCollection<LocaleString>()
                                  };
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem == null)
            {
                clTest.Visibility = Visibility.Collapsed;
                clDeploy.Visibility = Visibility.Collapsed;
                clNewUpdate.Visibility = Visibility.Collapsed;
                clEdit.Visibility = Visibility.Collapsed;
            }
            else
            {
                clTest.Visibility = Visibility.Visible;
                clDeploy.Visibility = Visibility.Visible;
                clNewUpdate.Visibility = Visibility.Visible;
                clEdit.Visibility = Visibility.Visible;
            }

            var item = treeView.SelectedItem as TreeViewItem;

            if (item == null)
                return;

            clEdit.Content = Properties.Resources.Edit + " " + item.Header;

            if (item.HasItems)
            {
                Core.AppIndex = item.Tag is int ? (int) item.Tag : -1;
                Core.UpdateIndex = -1;
                clNewUpdate.Visibility = Visibility.Visible;
                clTest.Visibility = Visibility.Visible;
                clDeploy.Visibility = Visibility.Visible;


                clNewUpdate.Note = Properties.Resources.AddUpdate + " " + item.Header;
                clTest.Content = Properties.Resources.Test + " " + item.Header;
            }
            else
            {
                var index = item.Tag as int[];
                if (index != null)
                {
                    Core.AppIndex = index[0];
                    Core.UpdateIndex = index[1];
                }

                clNewUpdate.Visibility = Visibility.Collapsed;
                clTest.Visibility = Visibility.Collapsed;
                clDeploy.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = treeView.SelectedItem as TreeViewItem;
            if (item == null)
                return;

            if (item.HasItems)
            {
                var index = item.Tag is int ? (int) item.Tag : 0;
                File.Delete(Core.UserStore + Projects[index].ApplicationName + ".sui");
                File.Delete(Core.UserStore + Projects[index].ApplicationName + ".sua");
                Projects.RemoveAt(index);
                Base.Serialize(Projects, Core.ProjectsFile);
            }
            else
            {
                var index = item.Tag as int[];
                if (index != null)
                {
                    var updates = Base.Deserialize<Collection<Update>>(Core.UserStore + Projects[index[0]].ApplicationName + ".sui");
                    Projects[index[0]].UpdateNames.RemoveAt(index[1]);
                    Base.Serialize(Projects, Core.ProjectsFile);
                    updates.RemoveAt(index[1]);
                    Base.Serialize(updates, Core.UserStore + Projects[index[0]].ApplicationName + ".sui");
                }
            }

            LoadProjects();
        }

        private void clEdit_Click(object sender, RoutedEventArgs e)
        {
            Core.AppInfo = Base.Deserialize<Sua>(Core.UserStore + Projects[Core.AppIndex].ApplicationName + ".sua");
            if (Core.UpdateIndex < 0)
                MainWindow.NavService.Navigate(new Uri(@"Pages\AppInfo.xaml", UriKind.Relative));
            else
            {
                Core.UpdateInfo = Base.Deserialize<Collection<Update>>(Core.UserStore + Projects[Core.AppIndex].ApplicationName + ".sui")[Core.UpdateIndex];
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
            }
        }

        private void clDeploy_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),};

            if (cfd.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.OK)
                return;
            var appName = Projects[Core.AppIndex].ApplicationName;
            File.Copy(Core.UserStore + appName + ".sua", cfd.FileName + @"\" + appName + ".sua", true);
            File.Copy(Core.UserStore + appName + ".sui", cfd.FileName + @"\" + appName + ".sui", true);
        }

        #region UI Events

        #region TextBox - Text Changed Events

        #endregion

        #region TextBox - Lost Keyboard Focus

        #endregion

        #region RadioButton - Checked

        #endregion

        #region Button - Click

        #endregion

        #region Commandlink - Cli

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            Core.AppInfo = new Sua();
            Core.UpdateInfo = new Update();
            Core.AppInfo.Description = new ObservableCollection<LocaleString>();
            Core.AppInfo.Name = new ObservableCollection<LocaleString>();
            Core.AppInfo.Publisher = new ObservableCollection<LocaleString>();
            Core.UpdateInfo.Name = new ObservableCollection<LocaleString>();
            Core.UpdateInfo.Description = new ObservableCollection<LocaleString>();
            Core.UpdateInfo.ReleaseDate = DateTime.Now.ToShortDateString();
            Core.UpdateInfo.Files = new ObservableCollection<UpdateFile>();
            Core.UpdateInfo.RegistryItems = new ObservableCollection<RegistryItem>();
            Core.UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();
            MainWindow.NavService.Navigate(new Uri(@"Pages\AppInfo.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        private void Help_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(@"http://sevenupdate.com/support");
        }

        #endregion

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProjects();
        }

        private void treeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tv = (TreeView) sender;
            var element = tv.InputHitTest(e.GetPosition(tv));
            while (!((element is TreeView) || element == null))
            {
                if (element is TreeViewItem)
                    break;

                if (!(element is FrameworkElement))
                    break;
                var fe = (FrameworkElement) element;
                element = (IInputElement) (fe.Parent ?? fe.TemplatedParent);
            }
            if (!(element is TreeViewItem))
                return;
            element.Focus();
            e.Handled = true;
        }

        #region MenuItem - Click

        #endregion

        #region ComboBox - Selection Changed

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                tbTitle.Foreground = Brushes.Black;
                tbHelp.Foreground = Brushes.Black;
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
                tbHelp.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
            }
        }

        #endregion

        #endregion
    }
}