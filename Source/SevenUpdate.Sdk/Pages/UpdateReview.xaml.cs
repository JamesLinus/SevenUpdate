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
using System.Windows;
using System.Windows.Media;
using Microsoft.Windows.Dialogs;
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

            if (Environment.OSVersion.Version.Major < 6)
                return;

            MouseLeftButtonDown += Core.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
        }

        #endregion

        private static void SaveProject(int updateIndex = -1)
        {
            var projects = Base.Deserialize<Collection<Project>>(Core.ProjectsFile) ?? new Collection<Project>();

            var appUpdates = new Collection<Update>();

            var appName = Base.GetLocaleString(Core.AppInfo.Name);
            for (int x = 0; x < projects.Count; x++)
            {
                if (projects[x].ApplicationName != Base.GetLocaleString(Core.AppInfo.Name))
                    continue;
                appUpdates = Base.Deserialize<Collection<Update>>(projects[x].Sui);
                projects.RemoveAt(x);
                break;
            }

            if (appUpdates.Count == 0 || updateIndex == -1)
                appUpdates.Add(Core.UpdateInfo);
            else
            {
                appUpdates.RemoveAt(updateIndex);
                appUpdates.Add(Core.UpdateInfo);
            }


            // Save the SUI File
            Base.Serialize(appUpdates, Core.UserStore + appName + ".sui");

            // Save the SUA file
            Base.Serialize(Core.AppInfo, Core.UserStore + appName + ".sua");

            // Save project file
            var project = new Project {ApplicationName = appName, Sui = Core.UserStore + appName + ".sui", Sua = Core.UserStore + appName + ".sua"};
            projects.Add(project);
            Base.Serialize(projects, Core.ProjectsFile);
        }

        private static void SaveUpdate(int updateIndex)
        {

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

        #region TextBlock - Mouse Down

        #endregion

        #region MenuItem - Click

        #endregion

        #region ComboBox - Selection Changed

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            tbTitle.Foreground = e.IsGlassEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 102, 204));
        }

        #endregion

        //NOTE Method is not final, just for testing
        private void CommandLink_Click(object sender, RoutedEventArgs e)
        {
            SaveProject();

            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        private void SaveExport_Click(object sender, RoutedEventArgs e)
        {

            var cfd = new CommonSaveFileDialog
            {
                AlwaysAppendDefaultExtension = true,
                DefaultExtension = "sui",
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                DefaultFileName = Core.AppInfo.Name[0].Value,
                EnsureValidNames = true,
            };
            cfd.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Sui, "*.sui"));

            if (cfd.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.OK)
                return;
        }
    }
}