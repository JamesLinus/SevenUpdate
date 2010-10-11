// ***********************************************************************
// <copyright file="UpdateReview.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt">GNU General Public License Version 3</license>
// ***********************************************************************
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
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Dwm;
    using System.Windows.Media;

    using SevenUpdate.Sdk.Windows;

    /// <summary>
    /// Interaction logic for UpdateRegistry.xaml
    /// </summary>
    public sealed partial class UpdateReview
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UpdateReview" /> class.
        /// </summary>
        public UpdateReview()
        {
            this.InitializeComponent();
            this.DataContext = Core.UpdateInfo;
            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.DwmCompositionChanged += this.UpdateUI;
            this.tbTitle.Foreground = AeroGlass.IsEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 51, 153));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="export">
        /// <see langword="true"/> to export the sui/sua files, <see langword="false"/> otherwise
        /// </param>
        private static void SaveProject(bool export = false)
        {
            var appUpdates = new Collection<Update>();

            var appName = Utilities.GetLocaleString(Core.AppInfo.Name);
            if (Core.AppInfo.Is64Bit)
            {
                if (!appName.Contains("x64") && !appName.Contains("X64"))
                {
                    appName += " (x64)";
                }
            }

            var updateNames = new ObservableCollection<string>();

            // If SUA exists lets remove the old info
            if (Core.AppIndex > -1)
            {
                appUpdates = Utilities.Deserialize<Collection<Update>>(Core.UserStore + Core.Projects[Core.AppIndex].ApplicationName + ".sui");
                updateNames = Core.Projects[Core.AppIndex].UpdateNames;
                Core.Projects.RemoveAt(Core.AppIndex);
            }

            // If we are just updating the SUA, lets add it
            if (appUpdates.Count == 0 || Core.UpdateIndex == -1)
            {
                updateNames.Add(Utilities.GetLocaleString(Core.UpdateInfo.Name));
                appUpdates.Add(Core.UpdateInfo);
            }
            else
            {
                // If we are updating the update, lets remove the old info and add the new.
                updateNames.RemoveAt(Core.UpdateIndex);
                appUpdates.RemoveAt(Core.UpdateIndex);
                appUpdates.Add(Core.UpdateInfo);
                updateNames.Add(Utilities.GetLocaleString(Core.UpdateInfo.Name));
            }

            // Save the SUI File
            Utilities.Serialize(appUpdates, Core.UserStore + appName + ".sui");

            // Save project file
            var project = new Project
                {
                    ApplicationName = appName, 
                    UpdateNames = updateNames
                };
            Core.Projects.Add(project);
            Utilities.Serialize(Core.Projects, Core.ProjectsFile);

            if (Core.IsNewProject)
            {
                // Save the SUA file
                Utilities.Serialize(Core.AppInfo, Core.UserStore + appName + ".sua");
            }

            if (!export)
            {
                Core.IsNewProject = false;
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
                return;
            }

            var fileName = Core.SaveFileDialog(null, appName, "sui");

            if (fileName == null)
            {
                return;
            }

            File.Copy(Core.UserStore + appName + ".sui", fileName, true);

            if (Core.IsNewProject)
            {
                fileName = Core.SaveFileDialog(null, appName, "sua");

                if (fileName == null)
                {
                    return;
                }

                File.Copy(Core.UserStore + appName + ".sua", fileName, true);
            }

            Core.IsNewProject = false;
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Saves and exports the Project
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void SaveExportProject(object sender, RoutedEventArgs e)
        {
            SaveProject(true);
        }

        /// <summary>
        /// Saves the Project
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void SaveProject(object sender, RoutedEventArgs e)
        {
            SaveProject();
        }

        /// <summary>
        /// Updates the UI based on whether Aero Glass is enabled
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="AeroGlass.DwmCompositionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void UpdateUI(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            this.tbTitle.Foreground = e.IsGlassEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 51, 153));
        }

        #endregion
    }
}