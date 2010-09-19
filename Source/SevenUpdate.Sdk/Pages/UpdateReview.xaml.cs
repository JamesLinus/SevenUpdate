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
using System.Windows;
using System.Windows.Media;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateRegistry.xaml
    /// </summary>
    public sealed partial class UpdateReview
    {
        #region Constructors

        /// <summary>
        ///   The constructor for the UpdateRegistry page
        /// </summary>
        public UpdateReview()
        {
            InitializeComponent();
            DataContext = Core.UpdateInfo;
            MouseLeftButtonDown += Core.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
            tbTitle.Foreground = AeroGlass.IsEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 51, 153));
        }

        #endregion

        private static void SaveProject(bool export = false)
        {
            var appUpdates = new Collection<Update>();

            var appName = Base.GetLocaleString(Core.AppInfo.Name);
            if (Core.AppInfo.Is64Bit)
            {
                if (!appName.Contains("x64") && !appName.Contains("X64"))
                    appName += " (x64)";
            }

            var updateNames = new ObservableCollection<string>();

            // If SUA exists lets remove the old info
            if (Core.AppIndex > -1)
            {
                appUpdates = Base.Deserialize<Collection<Update>>(Core.UserStore + Core.Projects[Core.AppIndex].ApplicationName + ".sui");
                updateNames = Core.Projects[Core.AppIndex].UpdateNames;
                Core.Projects.RemoveAt(Core.AppIndex);
            }

            // If we are just updating the SUA, lets add it
            if (appUpdates.Count == 0 || Core.UpdateIndex == -1)
            {
                updateNames.Add(Base.GetLocaleString(Core.UpdateInfo.Name));
                appUpdates.Add(Core.UpdateInfo);
            }
                // If we are updating the update, lets remove the old info and add the new.
            else
            {
                updateNames.RemoveAt(Core.UpdateIndex);
                appUpdates.RemoveAt(Core.UpdateIndex);
                appUpdates.Add(Core.UpdateInfo);
                updateNames.Add(Base.GetLocaleString(Core.UpdateInfo.Name));
            }


            // Save the SUI File
            Base.Serialize(appUpdates, Core.UserStore + appName + ".sui");

            // Save project file
            var project = new Project {ApplicationName = appName, UpdateNames = updateNames};
            Core.Projects.Add(project);
            Base.Serialize(Core.Projects, Core.ProjectsFile);

            if (Core.IsNewProject)
                // Save the SUA file
                Base.Serialize(Core.AppInfo, Core.UserStore + appName + ".sua");

            if (!export)
            {
                Core.IsNewProject = false;
                MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
                return;
            }

            var fileName = Core.SaveFileDialog(null, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), appName, "sui");

            if (fileName == null)
                return;

            File.Copy(Core.UserStore + appName + ".sui", fileName, true);

            if (Core.IsNewProject)
            {
                fileName = Core.SaveFileDialog(null, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), appName, "sua");

                if (fileName == null)
                    return;
                File.Copy(Core.UserStore + appName + ".sua", fileName, true);
            }
            Core.IsNewProject = false;
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        private void SaveExport_Click(object sender, RoutedEventArgs e)
        {
            SaveProject(true);
        }

        #region UI Events

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            tbTitle.Foreground = e.IsGlassEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 51, 153));
        }

        #endregion

        private void CommandLink_Click(object sender, RoutedEventArgs e)
        {
            SaveProject();
        }

        #endregion
    }
}