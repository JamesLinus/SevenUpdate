// ***********************************************************************
// <copyright file="UpdateInfo.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
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

    using SevenUpdate.Converters;
    using SevenUpdate.Windows;

    /// <summary>Interaction logic for Update_Info.xaml</summary>
    public sealed partial class UpdateInfo
    {
        #region Constants and Fields

        /// <summary>Gets or sets a list of indices relating to the current Update Collection</summary>
        private List<int> appIndices;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "UpdateInfo" /> class.</summary>
        public UpdateInfo()
        {
            this.InitializeComponent();
            this.miUpdate.DataContext = Core.Instance.IsAdmin;
        }

        #endregion

        #region Events

        /// <summary>Occurs when the update selection has changed</summary>
        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether to expand the Optional Updates Group by default.</summary>
        /// <value><see langword = "true" /> to expand the optional updates; otherwise, <see langword = "false" />.</value>
        internal static bool DisplayOptionalUpdates { private get; set; }

        #endregion

        #region Methods

        /// <summary>Loops through the <see cref="ListView"/> and updates the source when the update selection has been saved</summary>
        /// <param name="element">The <see cref="DependencyObject"/></param>
        private static void IterateVisualChild(DependencyObject element)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is CheckBox)
                {
                    var cb = VisualTreeHelper.GetChild(element, i) as CheckBox;
                    if (cb != null)
                    {
                        var bindingExpression = cb.GetBindingExpression(ToggleButton.IsCheckedProperty);
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

        /// <summary>Adds the updates to the list</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddUpdates(object sender, RoutedEventArgs e)
        {
            var selectedUpdates = new ObservableCollection<Update>();
            this.appIndices = new List<int>();

            for (var x = 0; x < Core.Applications.Count; x++)
            {
                this.appIndices.Add(x);
                foreach (var t in Core.Applications[x].Updates)
                {
                    selectedUpdates.Add(t);
                }
            }

            this.lvUpdates.ItemsSource = selectedUpdates;
            var myView = (CollectionView)CollectionViewSource.GetDefaultView(this.lvUpdates.ItemsSource);
            var groupDescription = new PropertyGroupDescription("Importance", new ImportanceToStringConverter());
            if (myView.GroupDescriptions != null)
            {
                myView.GroupDescriptions.Add(groupDescription);
            }

            this.lvUpdates.SelectedIndex = 0;
        }

        /// <summary>Navigates back to the Main page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        /// <summary>Launches the Help <c>Url</c> of the update</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void NavigateToHelpUrl(object sender, MouseButtonEventArgs e)
        {
            Utilities.StartProcess(Core.Applications[this.appIndices[this.lvUpdates.SelectedIndex]].AppInfo.HelpUrl);
        }

        /// <summary>Launches the More Information <c>Url</c> of the update</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void NavigateToInfoUrl(object sender, MouseButtonEventArgs e)
        {
            Utilities.StartProcess(((Update)this.lvUpdates.SelectedItem).InfoUrl);
        }

        /// <summary>Saves the selection of updates and navigates back to the Main page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveUpdateSelection(object sender, RoutedEventArgs e)
        {
            IterateVisualChild(this.lvUpdates);
            var count = new int[2];
            var downloadSize = new ulong[2];
            var updIndex = -1;
            for (var x = 0; x < Core.Applications.Count; x++)
            {
                for (var y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    updIndex++;
                    var item = (ListViewItem)this.lvUpdates.ItemContainerGenerator.ContainerFromItem(this.lvUpdates.Items[updIndex]);
                    if (item.Tag != null)
                    {
                        if (!(bool)item.Tag)
                        {
                            Core.Applications[x].Updates.RemoveAt(y);
                            y--;
                            continue;
                        }
                    }

                    if (!Core.Applications[x].Updates[y].Selected)
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

                if (Core.Applications[x].Updates.Count != 0)
                {
                    continue;
                }

                Core.Applications.RemoveAt(x);
                x--;
            }

            if (UpdateSelectionChanged != null)
            {
                UpdateSelectionChanged(this, new UpdateSelectionChangedEventArgs(count[0], count[1], downloadSize[0], downloadSize[1]));
            }

            MainWindow.NavService.GoBack();
        }

        /// <summary>Expands the group expander based on the which link was clicked from the main page</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
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
            }
            else
            {
                expander.IsExpanded = expander.Tag.ToString() == Properties.Resources.Important;
            }
        }

        /// <summary>Shows or Hides the selected update</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowOrHideUpdate(object sender, RoutedEventArgs e)
        {
            var appIndex = this.appIndices[this.lvUpdates.SelectedIndex];

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

        #endregion

        /// <summary>Provides event data for the UpdateSelection event</summary>
        internal sealed class UpdateSelectionChangedEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="UpdateSelectionChangedEventArgs"/> class.</summary>
            /// <param name="importantUpdates">The number of Important updates selected</param>
            /// <param name="optionalUpdates">The number of Optional updates selected</param>
            /// <param name="importantDownloadSize">A value indicating the download size of the Important updates</param>
            /// <param name="optionalDownloadSize">A value indicating the download size of the Optional updates</param>
            public UpdateSelectionChangedEventArgs(int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
            {
                this.ImportantUpdates = importantUpdates;

                this.OptionalUpdates = optionalUpdates;

                this.ImportantDownloadSize = importantDownloadSize;

                this.OptionalDownloadSize = optionalDownloadSize;
            }

            #endregion

            #region Properties

            /// <summary>Gets the total download size in bytes of the important updates</summary>
            internal ulong ImportantDownloadSize { get; private set; }

            /// <summary>Gets the number of Important Updates selected</summary>
            internal int ImportantUpdates { get; private set; }

            /// <summary>Gets the total download size in bytes of the optional updates</summary>
            internal ulong OptionalDownloadSize { get; private set; }

            /// <summary>Gets the number of Optional Updates selected</summary>
            internal int OptionalUpdates { get; private set; }

            #endregion
        }
    }
}