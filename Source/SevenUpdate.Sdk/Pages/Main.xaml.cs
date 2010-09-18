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
using System.Windows.Navigation;
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
            if (AeroGlass.IsEnabled)
            {
                tbTitle.Foreground = Brushes.Black;
                tbHelp.Foreground = Brushes.Black;
                tbAbout.Foreground = Brushes.Black;
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                tbHelp.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
                tbAbout.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
            }
        }

        #endregion

        private void LoadProjects()
        {
            treeView.Items.Clear();
            if (Core.Projects.Count <= 0)
                return;

            treeView.Visibility = Visibility.Visible;

            for (var x = 0; x < Core.Projects.Count; x++)
            {
                var app = new TreeViewItem {Header = Core.Projects[x].ApplicationName, Tag = x};

                for (var y = 0; y < Core.Projects[x].UpdateNames.Count; y++)
                {
                    var index = new[] {x, y};
                    app.Items.Add(new TreeViewItem {Header = Core.Projects[x].UpdateNames[y], Tag = index});
                }

                if (x == 0)
                    app.IsSelected = true;

                treeView.Items.Add(app);
            }
        }

        #region UI Events

        #region Commandlink - Click

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            Core.NewProject();
        }

        private void clEdit_Click(object sender, RoutedEventArgs e)
        {
            Core.EditItem();
        }

        private void NewUpdate_Click(object sender, RoutedEventArgs e)
        {
            Core.NewUpdate();
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
                e.Handled = true;
            }
            catch
            {
            }
        }

        #region MenuItem - Click

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = treeView.SelectedItem as TreeViewItem;
            if (item == null)
                return;

            if (item.HasItems)
            {
                var index = item.Tag is int ? (int) item.Tag : 0;
                File.Delete(Core.UserStore + Core.Projects[index].ApplicationName + ".sui");
                File.Delete(Core.UserStore + Core.Projects[index].ApplicationName + ".sua");
                Core.Projects.RemoveAt(index);
                Base.Serialize(Core.Projects, Core.ProjectsFile);
            }
            else
            {
                var index = item.Tag as int[];
                if (index != null)
                {
                    var updates = Base.Deserialize<Collection<Update>>(Core.UserStore + Core.Projects[index[0]].ApplicationName + ".sui");
                    Core.Projects[index[0]].UpdateNames.RemoveAt(index[1]);
                    Base.Serialize(Core.Projects, Core.ProjectsFile);
                    updates.RemoveAt(index[1]);
                    Base.Serialize(updates, Core.UserStore + Core.Projects[index[0]].ApplicationName + ".sui");
                }
            }

            LoadProjects();
        }

        private void ReleaseApp_Click(object sender, RoutedEventArgs e)
        {
            var appName = Core.Projects[Core.AppIndex].ApplicationName;
            var fileName = Core.SaveFileDialog(null, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), appName, "sua");

            if (fileName == null)
                return;
            File.Copy(Core.UserStore + appName + ".sua", fileName, true);
        }

        private void ReleaseUpdates_Click(object sender, RoutedEventArgs e)
        {
            var appName = Core.Projects[Core.AppIndex].ApplicationName;

            var fileName = Core.SaveFileDialog(null, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), appName, "sui");

            if (fileName == null)
                return;

            File.Copy(Core.UserStore + appName + ".sui", fileName, true);
        }

        #endregion

        #region TreeView - SelectedItemChanged

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem == null)
            {
                // clTest.Visibility = Visibility.Collapsed;
                clNewUpdate.Visibility = Visibility.Collapsed;
                clEdit.Visibility = Visibility.Collapsed;
            }
            else
            {
                // clTest.Visibility = Visibility.Visible;
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
                // clTest.Visibility = Visibility.Visible;

                clNewUpdate.Note = Properties.Resources.AddUpdate + " " + item.Header;
                // clTest.Content = Properties.Resources.Test + " " + item.Header;
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
                // clTest.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                tbTitle.Foreground = Brushes.Black;
                tbHelp.Foreground = Brushes.Black;
                //tbTitle.TextEffects = new TextEffectCollection();
                //tbTitle.TextEffects.Add(Core.dropShadowEffect);
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                tbHelp.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
            }
        }

        #endregion

        #endregion
    }
}