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
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Dwm;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateList.xaml
    /// </summary>
    public sealed partial class UpdateList
    {
        #region Constructors

        /// <summary>
        ///   The constructor for the UpdateList page
        /// </summary>
        public UpdateList()
        {
            InitializeComponent();

            if (Environment.OSVersion.Version.Major < 6)
                return;

            MouseLeftButtonDown += Core.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
        }

        #endregion

        private void LoadProjects()
        {
            var projects = Base.Deserialize<Collection<Project>>(Core.ProjectsFile) ?? new Collection<Project>();


            if (projects.Count <= 0)
                return;

            foreach (Project app in projects)
            {
                var appName = new TreeViewItem { Header = app.ApplicationName };

                foreach (string name in app.UpdateNames)
                {
                    var updates = new TreeViewItem { Header = name };
                    appName.Items.Add(updates);
                }
                treeView.Items.Add(appName);
            }
        }

        #region UI Events

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            tbTitle.Foreground = e.IsGlassEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 102, 204));
        }

        #endregion

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
             Task.Factory.StartNew(LoadProjects);
        }

        #endregion
    }
}