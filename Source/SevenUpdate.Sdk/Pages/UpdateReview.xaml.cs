// ***********************************************************************
// <copyright file="UpdateReview.xaml.cs" project="SevenUpdate.Sdk" assembly="SevenUpdate.Sdk" solution="SevenUpdate" company="Seven Software">
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
    using System.Windows;
    using System.Windows.Media;

    using SevenSoftware.Windows;

    /// <summary>Interaction logic for UpdateRegistry.xaml.</summary>
    public sealed partial class UpdateReview
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UpdateReview" /> class.</summary>
        public UpdateReview()
        {
            this.InitializeComponent();
            this.DataContext = Core.UpdateInfo;
            this.MouseLeftButtonDown -= Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged -= this.UpdateUI;

            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged += this.UpdateUI;
            this.tbTitle.Foreground = AeroGlass.IsGlassEnabled
                                          ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 51, 153));
        }

        #endregion

        #region Methods

        /// <summary>Saves and exports the Project.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void SaveExportProject(object sender, RoutedEventArgs e)
        {
            Core.SaveProject(true);
        }

        /// <summary>Saves the Project.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void SaveProject(object sender, RoutedEventArgs e)
        {
            Core.SaveProject();
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
        {
            this.tbTitle.Foreground = e.IsGlassEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 51, 153));
        }

        #endregion
    }
}