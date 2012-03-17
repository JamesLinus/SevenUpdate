// ***********************************************************************
// <copyright file="Main.xaml.cs" project="SevenUpdate.Sdk" assembly="SevenUpdate.Sdk" solution="SevenUpdate.Sdk" company="Seven Software">
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

namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Controls;

    using SevenUpdate.Sdk.Windows;

    /// <summary>Interaction logic for Main.xaml.</summary>
    public sealed partial class Main
    {
        /// <summary>Initializes a new instance of the <see cref="Main" /> class.</summary>
        public Main()
        {
            this.InitializeComponent();
            this.MouseLeftButtonDown -= Core.EnableDragOnGlass;
            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged -= this.UpdateUI;
            AeroGlass.CompositionChanged += this.UpdateUI;
            if (AeroGlass.IsGlassEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.tbHelp.Foreground = Brushes.Black;
                this.tbAbout.Foreground = Brushes.Black;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.tbHelp.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
                this.tbAbout.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
            }
        }

        /// <summary>Updates the UI based on the <c>TreeViewItem</c> selected.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>EventArgs</c> instance containing the event data.</param>
        private void ChangeUI(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem parent = this.treeView.FindTreeViewItem(t => t.Items.Contains(e.NewValue));

            if (this.treeView.SelectedItem == null)
            {
                // clTest.Visibility = Visibility.Collapsed;
                this.clNewUpdate.Visibility = Visibility.Collapsed;
                this.clEdit.Visibility = Visibility.Collapsed;
            }
            else
            {
                // clTest.Visibility = Visibility.Visible;
                this.clNewUpdate.Visibility = Visibility.Visible;
                this.clEdit.Visibility = Visibility.Visible;
            }

            var item = this.treeView.SelectedItem as TreeViewItem;

            if (item == null)
            {
                return;
            }

            this.clEdit.Content = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Edit, item.Header);
            this.clNewUpdate.Note = string.Format(CultureInfo.CurrentCulture, 
                    Properties.Resources.AddUpdate, 
                    item.Header);
            if (parent == null)
            {
                Core.AppIndex = item.Tag is int ? (int)item.Tag : -1;
                Core.UpdateIndex = -1;
                this.clNewUpdate.Visibility = Visibility.Visible;

                // clTest.Visibility = Visibility.Visible;

                // clTest.Content = string.Format(Properties.Resources.Test, item.Header);
            }
            else
            {
                var index = item.Tag as int[];
                if (index != null)
                {
                    Core.AppIndex = index[0];
                    Core.UpdateIndex = index[1];
                }

                this.clNewUpdate.Visibility = Visibility.Collapsed;

                // clTest.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>Deletes an item from the <c>TreeView</c> and Project collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            var item = this.treeView.SelectedItem as TreeViewItem;
            if (item == null)
            {
                return;
            }

            if (item.HasItems)
            {
                int index = item.Tag is int ? (int)item.Tag : 0;
                File.Delete(Path.Combine(App.UserStore, Core.Projects[index].ApplicationName + ".sui"));
                File.Delete(Path.Combine(App.UserStore, Core.Projects[index].ApplicationName + ".sua"));
                Core.Projects.RemoveAt(index);
                Utilities.Serialize(Core.Projects, Core.ProjectsFile);
            }
            else
            {
                var index = item.Tag as int[];
                if (index != null)
                {
                    if (File.Exists(Path.Combine(App.UserStore, Core.Projects[index[0]].ApplicationName + ".sui")))
                    {
                        var updates =
                                Utilities.Deserialize<Collection<Update>>(Path.Combine(App.UserStore, 
                                        Core.Projects[index[0]].ApplicationName + ".sui"));
                        updates.RemoveAt(index[1]);
                        Utilities.Serialize(updates, 
                                Path.Combine(App.UserStore, Core.Projects[index[0]].ApplicationName + ".sui"));
                    }

                    Core.Projects[index[0]].UpdateNames.RemoveAt(index[1]);
                    Utilities.Serialize(Core.Projects, Core.ProjectsFile);
                }
            }

            this.LoadProjects();
        }

        /// <summary>Edits the selected project item.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void EditSelectedItem(object sender, RoutedEventArgs e)
        {
            Core.EditItem();
        }

        /// <summary>Loads the projects.</summary>
        private void LoadProjects()
        {
            Core.Projects = File.Exists(Core.ProjectsFile)
                                    ? Utilities.Deserialize<Collection<Project>>(Core.ProjectsFile)
                                    : null;

            this.treeView.Items.Clear();
            if (Core.Projects == null)
            {
                this.treeView.Visibility = Visibility.Collapsed;
                return;
            }

            if (Core.Projects.Count <= 0)
            {
                this.treeView.Visibility = Visibility.Collapsed;
                return;
            }

            this.treeView.Visibility = Visibility.Visible;

            for (int x = 0; x < Core.Projects.Count; x++)
            {
                var app = new TreeViewItem { Header = Core.Projects[x].ApplicationName, Tag = x };
                for (int y = 0; y < Core.Projects[x].UpdateNames.Count; y++)
                {
                    var index = new[] { x, y };
                    app.Items.Add(new TreeViewItem { Header = Core.Projects[x].UpdateNames[y], Tag = index });
                }

                if (x == 0)
                {
                    app.IsSelected = true;
                }

                this.treeView.Items.Add(app);
            }
        }

        /// <summary>Loads the collection of <c>Project</c>'s into the UI.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void LoadUI(object sender, RoutedEventArgs e)
        {
            this.LoadProjects();
        }

        /// <summary>Opens a browser and opens the support page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void NavigateToSupport(object sender, MouseButtonEventArgs e)
        {
            Utilities.StartProcess(@"http://sevenupdate.com/support");
        }

        /// <summary>Creates a new project.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void NewProject(object sender, RoutedEventArgs e)
        {
            Core.NewProject();
        }

        /// <summary>Creates a new update for the selected <c>Project</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void NewUpdate(object sender, RoutedEventArgs e)
        {
            Core.NewUpdate();
        }

        /// <summary>Opens a dialog and saves the <c>Sua</c> for the selected project.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void ReleaseSua(object sender, RoutedEventArgs e)
        {
            string fileName = Core.Projects[Core.AppIndex].ExportedSuaFileName
                              ?? Core.Projects[Core.AppIndex].ApplicationName;
            fileName = Core.SaveFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), 
                    fileName, 
                    @"sua");

            if (fileName == null)
            {
                return;
            }

            Core.Projects[Core.AppIndex].ExportedSuaFileName = Path.GetFileNameWithoutExtension(fileName);

            Utilities.Serialize(Core.Projects, Core.ProjectsFile);

            string appName = Core.Projects[Core.AppIndex].ApplicationName;

            File.Copy(Path.Combine(App.UserStore, appName + ".sua"), fileName, true);
        }

        /// <summary>Opens a dialog and saves the Sui for the selected project.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void ReleaseSui(object sender, RoutedEventArgs e)
        {
            string fileName = Core.Projects[Core.AppIndex].ExportedSuiFileName
                              ?? Core.Projects[Core.AppIndex].ApplicationName;

            fileName = Core.SaveFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), 
                    fileName, 
                    @"sui");

            if (fileName == null)
            {
                return;
            }

            Core.Projects[Core.AppIndex].ExportedSuiFileName = Path.GetFileNameWithoutExtension(fileName);

            Utilities.Serialize(Core.Projects, Core.ProjectsFile);

            string appName = Core.Projects[Core.AppIndex].ApplicationName;

            File.Copy(Path.Combine(App.UserStore, appName + ".sui"), fileName, true);
        }

        /// <summary>Selects the <c>TreeViewItem</c> when right clicking on the <c>TreeView</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void SelectedItemOnRightClick(object sender, MouseButtonEventArgs e)
        {
            var tv = (TreeView)sender;
            IInputElement element = tv.InputHitTest(e.GetPosition(tv));
            while (!((element is TreeView) || element == null))
            {
                if (element is TreeViewItem)
                {
                    break;
                }

                if (!(element is FrameworkElement))
                {
                    break;
                }

                var fe = (FrameworkElement)element;
                element = (IInputElement)(fe.Parent ?? fe.TemplatedParent);
            }

            if (!(element is TreeViewItem))
            {
                return;
            }

            element.Focus();
            e.Handled = true;
        }

        /// <summary>Shows the About Dialog window.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void ShowAboutDialog(object sender, MouseButtonEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.tbHelp.Foreground = Brushes.Black;
                this.tbAbout.Foreground = Brushes.Black;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.tbHelp.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
                this.tbAbout.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
            }
        }
    }
}