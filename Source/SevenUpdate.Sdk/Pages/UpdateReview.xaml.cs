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
        }

        #endregion

        //NOTE Method is not final, just for testing
        private void CommandLink_Click(object sender, RoutedEventArgs e)
        {
            // Base.Deserialize<Collection<Sui>>(Core.ProjectsFile) ??
            var projects = new Collection<Sui>();
            // TODO implement check for an update, to remove the old and add the new for project editing.


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

            if (projects.Count < 1)
            {
                var project = new Sui { AppInfo = Core.AppInfo, Updates = new ObservableCollection<Update> { Core.UpdateInfo } };
                projects.Add(project);
            }

            // Save the projects file
            Base.Serialize(projects, Core.ProjectsFile);

            var updates = new Collection<Update> {Core.UpdateInfo};

            // Save the SUI File
            Base.Serialize(updates, cfd.FileName);

            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion
    }
}