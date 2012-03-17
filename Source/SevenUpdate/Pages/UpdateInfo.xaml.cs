// ***********************************************************************
// <copyright file="UpdateInfo.xaml.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
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
// <summary>
//   Interaction logic for UpdateInfo.xaml
// .</summary> ***********************************************************************

namespace SevenUpdate.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenSoftware.Windows;

    using SevenUpdate.Converters;

    /// <summary>Interaction logic for Update_Info.xaml.</summary>
    public sealed partial class UpdateInfo
    {
        /// <summary>Gets or sets a list of indices relating to the current Update Collection.</summary>
        private List<int> appIndices;

        /// <summary>Initializes a new instance of the <see cref="UpdateInfo" /> class.</summary>
        public UpdateInfo()
        {
            this.InitializeComponent();
            this.miUpdate.DataContext = Core.Instance.IsAdmin;

            this.MouseLeftButtonDown -= Core.EnableDragOnGlass;
            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged -= this.UpdateUI;
            AeroGlass.CompositionChanged += this.UpdateUI;

            if (AeroGlass.IsGlassEnabled)
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

        /// <summary>Occurs when the update selection has changed.</summary>
        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChanged;

        /// <summary>Gets or sets a value indicating whether to expand the Optional Updates Group by default.</summary>
        /// <value><c>True</c> to expand the optional updates; otherwise, <c>False</c>.</value>
        internal static bool DisplayOptionalUpdates { private get; set; }

        /// <summary>Loops through the <c>ListView</c> and updates the source when the update selection has been saved.</summary>
        /// <param name="element">The <c>DependencyObject</c>.</param>
        private static void IterateVisualChild(DependencyObject element)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is CheckBox)
                {
                    var cb = VisualTreeHelper.GetChild(element, i) as CheckBox;
                    if (cb != null)
                    {
                        BindingExpression bindingExpression = cb.GetBindingExpression(ToggleButton.IsCheckedProperty);
                        if (bindingExpression != null)
                        {
                            bindingExpression.UpdateSource();
                        }
                    }

                    continue;
                }

                IterateVisualChild(VisualTreeHelper.GetChild(element, i));
            }
        }

        /// <summary>Adds the updates to the list.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void AddUpdates(object sender, RoutedEventArgs e)
        {
            var selectedUpdates = new ObservableCollection<Update>();
            this.appIndices = new List<int>();
            for (int x = 0; x < Core.Applications.Count; x++)
            {
                this.appIndices.Add(x);
                foreach (var t in Core.Applications[x].Updates)
                {
                    selectedUpdates.Add(t);
                }
            }

            this.lvUpdates.ItemsSource = selectedUpdates;
            var view = (CollectionView)CollectionViewSource.GetDefaultView(this.lvUpdates.ItemsSource);
            var groupDescription = new PropertyGroupDescription("Importance", new ImportanceToStringConverter());
            if (view.GroupDescriptions != null)
            {
                view.GroupDescriptions.Add(groupDescription);
            }

            this.lvUpdates.SelectedIndex = 0;
        }

        /// <summary>Launches the Help <c>Url</c> of the update.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void NavigateToHelpUrl(object sender, MouseButtonEventArgs e)
        {
            Utilities.StartProcess(Core.Applications[this.appIndices[this.lvUpdates.SelectedIndex]].AppInfo.HelpUrl);
        }

        /// <summary>Launches the More Information <c>Url</c> of the update.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void NavigateToInfoUrl(object sender, MouseButtonEventArgs e)
        {
            Utilities.StartProcess(((Update)this.lvUpdates.SelectedItem).InfoUrl);
        }

        /// <summary>Navigates back to the Main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            Core.NavigateToMainPage();
        }

        /// <summary>Saves the selection of updates and navigates back to the Main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void SaveUpdateSelection(object sender, RoutedEventArgs e)
        {
            IterateVisualChild(this.lvUpdates);
            var count = new int[2];
            var downloadSize = new ulong[2];
            int updIndex = -1;
            for (int x = 0; x < Core.Applications.Count; x++)
            {
                for (int y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    updIndex++;

                    if (!((Update)this.lvUpdates.Items[updIndex]).Selected)
                    {
                        continue;
                    }

                    switch (Core.Applications[x].Updates[y].Importance)
                    {
                        case Importance.Important:
                            count[0]++;
                            downloadSize[0] = Core.Applications[x].Updates[y].Size;
                            break;
                        case Importance.Recommended:
                            if (Core.Settings.IncludeRecommended)
                            {
                                downloadSize[0] = Core.Applications[x].Updates[y].Size;
                                count[0]++;
                            }
                            else
                            {
                                downloadSize[1] = Core.Applications[x].Updates[y].Size;
                                count[1]++;
                            }

                            break;
                        case Importance.Optional:
                        case Importance.Locale:
                            downloadSize[1] = Core.Applications[x].Updates[y].Size;
                            count[1]++;
                            break;
                    }
                }
            }

            if (UpdateSelectionChanged != null)
            {
                UpdateSelectionChanged(
                        this, new UpdateSelectionChangedEventArgs(count[0], count[1], downloadSize[0], downloadSize[1]));
            }

            Core.NavigateToMainPage();
        }

        /// <summary>Expands the group expander based on the which link was clicked from the main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void SetExpanded(object sender, RoutedEventArgs e)
        {
            var expander = e.Source as Expander;
            if (expander == null)
            {
                return;
            }

            if (DisplayOptionalUpdates)
            {
                expander.IsExpanded = expander.Tag.ToString() == Properties.Resources.Optional;

                if (!Core.Settings.IncludeRecommended && expander.Tag.ToString() == Properties.Resources.Recommended)
                {
                    expander.IsExpanded = true;
                }
            }
            else
            {
                expander.IsExpanded = expander.Tag.ToString() == Properties.Resources.Important;

                if (Core.Settings.IncludeRecommended && expander.Tag.ToString() == Properties.Resources.Recommended)
                {
                    expander.IsExpanded = true;
                }
            }
        }

        /// <summary>Shows or Hides the selected update.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void ShowOrHideUpdate(object sender, RoutedEventArgs e)
        {
            int appIndex = this.appIndices[this.lvUpdates.SelectedIndex];

            var update = this.lvUpdates.SelectedItem as Update;
            if (update == null)
            {
                return;
            }

            var hnh = new Suh(update.Name, Core.Applications[appIndex].AppInfo.Publisher, update.Description)
                {
                        HelpUrl = Core.Applications[appIndex].AppInfo.HelpUrl, 
                        InfoUrl = update.InfoUrl, 
                        AppUrl = Core.Applications[appIndex].AppInfo.AppUrl, 
                        ReleaseDate = update.ReleaseDate, 
                        Status = UpdateStatus.Hidden, 
                        UpdateSize = Core.GetUpdateSize(update.Files), 
                        Importance = update.Importance, 
                };

            if (!update.Hidden)
            {
                if (WcfService.HideUpdate(hnh))
                {
                    update.Hidden = true;
                    update.Selected = false;
                }
            }
            else
            {
                if (WcfService.ShowUpdate(hnh))
                {
                    update.Hidden = false;
                }
            }
        }

        /// <summary>Changes the UI depending on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
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

        /// <summary>Provides event data for the UpdateSelection event.</summary>
        internal sealed class UpdateSelectionChangedEventArgs : EventArgs
        {
            /// <summary>Initializes a new instance of the <see cref="UpdateSelectionChangedEventArgs" /> class.</summary>
            /// <param name="importantUpdates">The number of Important updates selected.</param>
            /// <param name="optionalUpdates">The number of Optional updates selected.</param>
            /// <param name="importantDownloadSize">A value indicating the download size of the Important updates.</param>
            /// <param name="optionalDownloadSize">A value indicating the download size of the Optional updates.</param>
            public UpdateSelectionChangedEventArgs(
                    int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
            {
                this.ImportantUpdates = importantUpdates;

                this.OptionalUpdates = optionalUpdates;

                this.ImportantDownloadSize = importantDownloadSize;

                this.OptionalDownloadSize = optionalDownloadSize;
            }

            /// <summary>Gets the total download size in bytes of the important updates.</summary>
            internal ulong ImportantDownloadSize { get; private set; }

            /// <summary>Gets the number of Important Updates selected.</summary>
            internal int ImportantUpdates { get; private set; }

            /// <summary>Gets the total download size in bytes of the optional updates.</summary>
            internal ulong OptionalDownloadSize { get; private set; }

            /// <summary>Gets the number of Optional Updates selected.</summary>
            internal int OptionalUpdates { get; private set; }
        }
    }
}