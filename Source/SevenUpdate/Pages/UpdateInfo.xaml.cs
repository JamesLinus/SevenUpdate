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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Internal;
using SevenUpdate.Converters;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{

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

    /// <summary>
    ///   Interaction logic for Update_Info.xaml
    /// </summary>
    public sealed partial class UpdateInfo : Page
    {

        #region Fields

        /// <summary>
        ///   Gets an image of a blue arrow
        /// </summary>
        private readonly BitmapImage blueArrow = new BitmapImage(new Uri("/Images/BlueArrow.png", UriKind.Relative));

        /// <summary>
        ///   Gets an image of a green arrow
        /// </summary>
        private readonly BitmapImage greenArrow = new BitmapImage(new Uri("/Images/GreenArrow.png", UriKind.Relative));

        /// <summary>
        ///   Gets or Sets a list of indices relating to the current Update Collection
        /// </summary>
        private List<int> appIndices;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or Sets a value indicating if one or more updates were hidden
        /// </summary>
        private bool AreHiddenUpdates { get; set; }

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
            if (CoreNativeMethods.IsUserAnAdmin())
                cmiHideUpdate.Icon = null;
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
        /// <param name = "element">The <see cref = "DependencyObject" / />
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

                if (Core.Applications[x].Updates.Count == 0)
                {
                    Core.Applications.RemoveAt(x);
                    x--;
                }
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
                for (var y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    selectedUpdates.Add(Core.Applications[x].Updates[y]);
                }
            }

            var items = new Binding {Source = selectedUpdates};
            lvUpdates.SetBinding(ItemsControl.ItemsSourceProperty, items);
            selectedUpdates.CollectionChanged += SelectedUpdates_CollectionChanged;
            var myView = (CollectionView) CollectionViewSource.GetDefaultView(lvUpdates.ItemsSource);
            var groupDescription = new PropertyGroupDescription("Importance", new ImportanceToString());
            if (myView.GroupDescriptions != null)
                myView.GroupDescriptions.Add(groupDescription);

            AddSortBinding();
        }

        /// <summary>
        ///   Adds the <see cref = "GridViewColumn" />'s of the <see cref = "ListView" /> to be sorted
        /// </summary>
        private void AddSortBinding()
        {
            var gv = (GridView) lvUpdates.View;

            var col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("Name"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Importance"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("Size"));

            ListViewSorter.SetCustomSorter(lvUpdates, new CustomComparers.UpdateSorter());
        }

        #endregion

        #region UI Events

        #region Buttons

        /// <summary>
        ///   Navigates back to the Main page
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        /// <summary>
        ///   Saves the selection of updates and navigates back to the Main page
        /// </summary>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            IterateVisualChild(lvUpdates);
            SaveUpdateSelection();
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
        ///   Updates the <see cref = "CollectionView" /> when the <c>updateHistory</c> collection changes
        /// </summary>
        private void SelectedUpdates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(lvUpdates.ItemsSource);
            dataView.Refresh();
        }

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

            var item = (ListViewItem) lvUpdates.SelectedItem;
            if (cmiHideUpdate.Header.ToString() == Properties.Resources.HideUpdate)
            {
                if (AdminClient.HideUpdate(hnh))
                {
                    cmiHideUpdate.Header = Properties.Resources.ShowUpdate;
                    item.Foreground = Brushes.Gray;
                    item.Tag = false;
                }
            }
            else
            {
                if (AdminClient.ShowUpdate(hnh))
                {
                    cmiHideUpdate.Header = Properties.Resources.HideUpdate;
                    item.Foreground = Brushes.Black;
                    item.Tag = true;
                }
            }
        }

        /// <summary>
        ///   Shows the selected update details in the sidebar when the selection changes
        /// </summary>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvUpdates.SelectedIndex == -1)
                return;
            var item = (ListViewItem)lvUpdates.ItemContainerGenerator.ContainerFromItem(lvUpdates.SelectedItem);
            if (item == null)
                return;

            var appIndex = appIndices[lvUpdates.SelectedIndex];

            cmiHideUpdate.IsEnabled = Base.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, true) != Core.Applications[appIndex].AppInfo.Directory;

            
            

            if (item.Tag != null)
                cmiHideUpdate.Header = ((bool)item.Tag) ? Properties.Resources.HideUpdate : Properties.Resources.ShowUpdate;
            else
                cmiHideUpdate.Header = Properties.Resources.HideUpdate;

            var update = lvUpdates.SelectedItem as Update;

            if (update.Size > 0)
            {
                lblStatus.Text = Properties.Resources.ReadyToDownload;
                imgArrow.Source = blueArrow;
            }
            else
            {
                lblStatus.Text = Properties.Resources.ReadyToInstall;
                imgArrow.Source = greenArrow;
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

        private void CheckBoxIsEnabled_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool) e.NewValue) == false)
            {
                var chkbox = sender as CheckBox;
                if (chkbox != null)
                {
                    chkbox.IsChecked = false;
                    chkbox.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
                }
                AreHiddenUpdates = true;
            }
        }

        #endregion

        #endregion
    }
}