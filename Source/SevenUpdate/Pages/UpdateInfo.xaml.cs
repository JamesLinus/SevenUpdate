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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SevenUpdate.Converters;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    ///   Interaction logic for Update_Info.xaml
    /// </summary>
    public sealed partial class UpdateInfo
    {
        #region Fields

        /// <summary>
        ///   Gets or Sets a list of indices relating to the current Update Collection
        /// </summary>
        private List<int> appIndices;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or Sets a value indicating to expand the Optional Updates Group by default.
        /// </summary>
        internal static bool DisplayOptionalUpdates { private get; set; }

        #endregion

        /// <summary>
        ///   Constructor for the Update Info Page
        /// </summary>
        public UpdateInfo()
        {
            InitializeComponent();
            miUpdate.DataContext = Core.Instance.IsAdmin;
        }

        #region Event Declarations

        /// <summary>
        ///   Occurs when the update selection has changed
        /// </summary>
        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChanged;

        #endregion

        #region Methods

        /// <summary>
        ///   Loops through the <see cref = "ListView" /> and updates the source when the update selection has been saved
        /// </summary>
        /// <param name = "element">The <see cref = "DependencyObject" /></param>
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
                            bindingExpression.UpdateSource();
                    }
                    continue;
                }

                IterateVisualChild(VisualTreeHelper.GetChild(element, i));
            }
        }

        /// <summary>
        ///   Saves the update selection
        /// </summary>
        private void SaveUpdateSelection()
        {
            var count = new int[2];
            var downloadSize = new ulong[2];
            var updIndex = -1;
            for (var x = 0; x < Core.Applications.Count; x++)
            {
                for (var y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    updIndex++;
                    var item = (ListViewItem) lvUpdates.ItemContainerGenerator.ContainerFromItem(lvUpdates.Items[updIndex]);
                    if (item.Tag != null)
                    {
                        if (!(bool) item.Tag)
                        {
                            Core.Applications[x].Updates.RemoveAt(y);
                            y--;
                            continue;
                        }
                    }

                    if (!Core.Applications[x].Updates[y].Selected)
                        continue;

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
                    continue;
                Core.Applications.RemoveAt(x);
                x--;
            }

            if (UpdateSelectionChanged != null)
                UpdateSelectionChanged(this, new UpdateSelectionChangedEventArgs(count[0], count[1], downloadSize[0], downloadSize[1]));
        }

        /// <summary>
        ///   Adds the updates to the list
        /// </summary>
        private void AddUpdates()
        {
            var selectedUpdates = new ObservableCollection<Update>();
            appIndices = new List<int>();

            for (var x = 0; x < Core.Applications.Count; x++)
            {
                appIndices.Add(x);
                foreach (var t in Core.Applications[x].Updates)
                    selectedUpdates.Add(t);
            }

            lvUpdates.ItemsSource = selectedUpdates;
            var myView = (CollectionView) CollectionViewSource.GetDefaultView(lvUpdates.ItemsSource);
            var groupDescription = new PropertyGroupDescription("Importance", new ImportanceToString());
            if (myView.GroupDescriptions != null)
                myView.GroupDescriptions.Add(groupDescription);
        }

        #endregion

        #region UI Events

        #region Buttons

        /// <summary>
        ///   Navigates back to the Main page
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Core.NavService.GoBack();
        }

        /// <summary>
        ///   Saves the selection of updates and navigates back to the Main page
        /// </summary>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            IterateVisualChild(lvUpdates);
            SaveUpdateSelection();
            Core.NavService.GoBack();
        }

        #endregion

        #region TextBlocks

        /// <summary>
        ///   Launches the More Information <c>Url</c> of the update
        /// </summary>
        private void UrlInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(((Update) lvUpdates.SelectedItem).InfoUrl);
            }
            catch
            {
            }
        }

        /// <summary>
        ///   Launches the Help <c>Url</c> of the update
        /// </summary>
        private void UrlHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(Core.Applications[appIndices[lvUpdates.SelectedIndex]].AppInfo.HelpUrl);
            }
            catch
            {
            }
        }

        #endregion

        /// <summary>
        ///   Loads the updates found into the UI
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddUpdates();
            lvUpdates.SelectedIndex = 0;
        }

        #region ListView Related

        /// <summary>
        ///   Shows the selected update details
        /// </summary>
        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var appIndex = appIndices[lvUpdates.SelectedIndex];

            var update = lvUpdates.SelectedItem as Update;
            if (update == null)
                return;
            var hnh = new Suh
                          {
                              HelpUrl = Core.Applications[appIndex].AppInfo.HelpUrl,
                              InfoUrl = update.InfoUrl,
                              Publisher = Core.Applications[appIndex].AppInfo.Publisher,
                              PublisherUrl = Core.Applications[appIndex].AppInfo.AppUrl,
                              ReleaseDate = update.ReleaseDate,
                              Status = UpdateStatus.Hidden,
                              UpdateSize = Core.GetUpdateSize(update.Files),
                              Importance = update.Importance,
                              Description = update.Description,
                              Name = update.Name
                          };

            if (!update.Hidden)
            {
                if (AdminClient.HideUpdate(hnh))
                {
                    update.Hidden = true;
                    update.Selected = false;
                }
            }
            else
            {
                if (AdminClient.ShowUpdate(hnh))
                    update.Hidden = false;
            }
        }

        /// <summary>
        ///   Expands the group expander based on the which link was clicked from the main page
        /// </summary>
        private void Expander_Loaded(object sender, RoutedEventArgs e)
        {
            var expander = e.Source as Expander;
            if (expander == null)
                return;
            if (DisplayOptionalUpdates)
                expander.IsExpanded = expander.Tag.ToString() == Properties.Resources.Optional;
            else
                expander.IsExpanded = expander.Tag.ToString() == Properties.Resources.Important;
        }

        #endregion

        #endregion
    }

    #region EventArgs

    /// <summary>
    ///   Provides event data for the UpdateSelection event
    /// </summary>
    internal sealed class UpdateSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "importantUpdates">The number of Important updates selected</param>
        /// <param name = "optionalUpdates">The number of Optional updates selected</param>
        /// <param name = "importantDownloadSize">A value indicating the download size of the Important updates</param>
        /// <param name = "optionalDownloadSize">A value indicating the download size of the Optional updates</param>
        public UpdateSelectionChangedEventArgs(int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
        {
            ImportantUpdates = importantUpdates;

            OptionalUpdates = optionalUpdates;

            ImportantDownloadSize = importantDownloadSize;

            OptionalDownloadSize = optionalDownloadSize;
        }

        /// <summary>
        ///   Gets the number of Important Updates selected
        /// </summary>
        internal int ImportantUpdates { get; private set; }

        /// <summary>
        ///   Gets the number of Optional Updates selected
        /// </summary>
        internal int OptionalUpdates { get; private set; }

        /// <summary>
        ///   Gets the total download size in bytes of the important updates
        /// </summary>
        internal ulong ImportantDownloadSize { get; private set; }

        /// <summary>
        ///   Gets the total download size in bytes of the optional updates
        /// </summary>
        internal ulong OptionalDownloadSize { get; private set; }
    }

    #endregion
}