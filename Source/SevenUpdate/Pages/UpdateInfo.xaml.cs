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
using SevenUpdate.Base;
using SevenUpdate.Controls;
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
        #region Structs

        /// <summary>
        ///   The indices of an update
        /// </summary>
        private struct Indices
        {
            /// <summary>
            ///   Gets or Sets the position of the Application information of an update
            /// </summary>
            internal int AppIndex { get; set; }

            /// <summary>
            ///   Gets or Sets the position of the update information within the Application information of an update
            /// </summary>
            internal int UpdateIndex { get; set; }
        }

        #endregion

        #region Global Vars

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
        private List<Indices> indices;

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
            if (App.IsAdmin)
                cmiHideUpdate.Icon = null;
        }

        #region Event Declarations

        /// <summary>
        ///   Occurs when the update selection has changed
        /// </summary>
        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChangedEventHandler;

        /// <summary>
        ///   Occurs when the user cancels their update selection
        /// </summary>
        internal static event EventHandler CanceledSelectionEventHandler;

        #endregion

        #region Methods

        /// <summary>
        ///   Loops through the
        ///   <see cref = "ListView" />
        ///   and updates the source when the update selection has been saved
        /// </summary>
        /// <param name = "element">The
        ///   <see cref = "DependencyObject" />
        /// </param>
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
            for (var x = 0; x < App.Applications.Count; x++)
            {
                for (var y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    updIndex++;
                    var item = (ListViewItem) listView.ItemContainerGenerator.ContainerFromItem(listView.Items[updIndex]);
                    if (item.Tag != null)
                    {
                        if (!(bool) item.Tag)
                        {
                            App.Applications[x].Updates.RemoveAt(y);
                            y--;
                            continue;
                        }
                    }

                    if (!App.Applications[x].Updates[y].Selected)
                        continue;

                    switch (App.Applications[x].Updates[y].Importance)
                    {
                        case Importance.Important:
                            count[0]++;
                            downloadSize[0] = App.Applications[x].Updates[y].Size;
                            break;
                        case Importance.Recommended:
                            if (App.Settings.IncludeRecommended)
                            {
                                downloadSize[0] = App.Applications[x].Updates[y].Size;
                                count[0]++;
                            }
                            else
                            {
                                downloadSize[1] = App.Applications[x].Updates[y].Size;
                                count[1]++;
                            }
                            break;
                        case Importance.Optional:
                        case Importance.Locale:
                            downloadSize[1] = App.Applications[x].Updates[y].Size;
                            count[1]++;
                            break;
                    }
                }

                if (App.Applications[x].Updates.Count == 0)
                {
                    App.Applications.RemoveAt(x);
                    x--;
                }
            }

            if (UpdateSelectionChangedEventHandler != null)
                UpdateSelectionChangedEventHandler(this, new UpdateSelectionChangedEventArgs(count[0], count[1], downloadSize[0], downloadSize[1]));
        }

        /// <summary>
        ///   Adds the updates to the list
        /// </summary>
        private void AddUpdates()
        {
            var selectedUpdates = new ObservableCollection<Update>();
            indices = new List<Indices>();
            var index = new Indices();

            for (var x = 0; x < App.Applications.Count; x++)
            {
                for (var y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    selectedUpdates.Add(App.Applications[x].Updates[y]);
                    index.AppIndex = x;
                    index.UpdateIndex = y;
                    indices.Add(index);
                }
            }

            var items = new Binding {Source = selectedUpdates};
            listView.SetBinding(ItemsControl.ItemsSourceProperty, items);
            selectedUpdates.CollectionChanged += SelectedUpdates_CollectionChanged;
            var myView = (CollectionView) CollectionViewSource.GetDefaultView(listView.ItemsSource);
            var groupDescription = new PropertyGroupDescription("Importance", new ImportanceGroupConverter());
            myView.GroupDescriptions.Add(groupDescription);

            AddSortBinding();
        }

        /// <summary>
        ///   Adds the
        ///   <see cref = "GridViewColumn" />
        ///   's of the
        ///   <see cref = "ListView" />
        ///   to be sorted
        /// </summary>
        private void AddSortBinding()
        {
            var gv = (GridView) listView.View;

            var col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("Name"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Importance"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("Size"));

            ListViewSorter.SetCustomSorter(listView, new ListViewExtensions.UpdateSorter());
        }

        /// <summary>
        ///   Shows the update information in the sidebar
        /// </summary>
        private void ShowLabels()
        {
            tbStatus.Visibility = Visibility.Visible;
            tbPublishedDate.Visibility = Visibility.Visible;
            tbPublishedLabel.Visibility = Visibility.Visible;
            tbUpdateDescription.Visibility = Visibility.Visible;
            tbUpdateName.Visibility = Visibility.Visible;
            tbUrlHelp.Visibility = Visibility.Visible;
            tbUrlInfo.Visibility = Visibility.Visible;
            imgIcon.Visibility = Visibility.Hidden;
            imgArrow.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///   Hides the update information in the sidebar
        /// </summary>
        private void HideLabels()
        {
            tbStatus.Visibility = Visibility.Hidden;
            tbPublishedDate.Visibility = Visibility.Hidden;
            tbPublishedLabel.Visibility = Visibility.Hidden;
            tbUpdateDescription.Visibility = Visibility.Hidden;
            tbUpdateName.Visibility = Visibility.Hidden;
            tbUrlHelp.Visibility = Visibility.Hidden;
            tbUrlInfo.Visibility = Visibility.Hidden;
            imgIcon.Visibility = Visibility.Visible;
            imgArrow.Visibility = Visibility.Hidden;
        }

        #endregion

        #region UI Events

        #region Buttons

        /// <summary>
        ///   Navigates back to the Main page
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (CanceledSelectionEventHandler != null && AreHiddenUpdates)
                CanceledSelectionEventHandler(this, new EventArgs());
            MainWindow.NavService.GoBack();
        }

        /// <summary>
        ///   Saves the selection of updates and navigates back to the Main page
        /// </summary>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            IterateVisualChild(listView);
            SaveUpdateSelection();
            MainWindow.NavService.GoBack();
        }

        #endregion

        #region TextBlocks

        /// <summary>
        ///   Underlines the text when mouse is over the
        ///   <see cref = "TextBlock" />
        /// </summary>
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock) sender);
            textBlock.TextDecorations = TextDecorations.Underline;
        }

        /// <summary>
        ///   Removes the Underlined text when mouse is leaves the
        ///   <see cref = "TextBlock" />
        /// </summary>
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock) sender);
            textBlock.TextDecorations = null;
        }

        /// <summary>
        ///   Launches the More Information
        ///   <c>Url</c>
        ///   of the update
        /// </summary>
        private void UrlInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(tbUrlInfo.Tag.ToString());
            }
            catch
            {
            }
        }

        /// <summary>
        ///   Launches the Help
        ///   <c>Url</c>
        ///   of the update
        /// </summary>
        private void UrlHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(tbUrlHelp.Tag.ToString());
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
            listView.SelectedIndex = 0;
        }

        #region ListView Related

        /// <summary>
        ///   Updates the
        ///   <see cref = "CollectionView" />
        ///   when the
        ///   <c>updateHistory</c>
        ///   collection changes
        /// </summary>
        private void SelectedUpdates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>
        ///   Shows the selected update details
        /// </summary>
        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var updateIndex = indices[listView.SelectedIndex].UpdateIndex;
            var appIndex = indices[listView.SelectedIndex].AppIndex;

            var hnh = new Suh
                          {
                              HelpUrl = App.Applications[appIndex].AppInfo.HelpUrl,
                              InfoUrl = App.Applications[appIndex].Updates[updateIndex].InfoUrl,
                              Publisher = App.Applications[appIndex].AppInfo.Publisher,
                              PublisherUrl = App.Applications[appIndex].AppInfo.AppUrl,
                              ReleaseDate = App.Applications[appIndex].Updates[updateIndex].ReleaseDate,
                              Status = UpdateStatus.Hidden,
                              Size = App.GetUpdateSize(App.Applications[appIndex].Updates[updateIndex].Files),
                              Importance = App.Applications[appIndex].Updates[updateIndex].Importance,
                              Description = App.Applications[appIndex].Updates[updateIndex].Description,
                              Name = App.Applications[appIndex].Updates[updateIndex].Name
                          };

            var item = (ListViewItem) listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);
            if (cmiHideUpdate.Header.ToString() == App.RM.GetString("HideUpdate"))
            {
                if (AdminClient.HideUpdate(hnh))
                {
                    cmiHideUpdate.Header = App.RM.GetString("ShowUpdate");
                    item.Foreground = Brushes.Gray;
                    item.Tag = false;
                }
            }
            else
            {
                if (AdminClient.ShowUpdate(hnh))
                {
                    cmiHideUpdate.Header = App.RM.GetString("HideUpdate");
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
            if (listView.SelectedIndex == -1)
                HideLabels();
            else
            {
                var appIndex = indices[listView.SelectedIndex].AppIndex;
                var updateIndex = indices[listView.SelectedIndex].UpdateIndex;
                tbUpdateDescription.Text = Base.Base.GetLocaleString(App.Applications[appIndex].Updates[updateIndex].Description);
                tbPublishedDate.Text = App.Applications[appIndex].Updates[updateIndex].ReleaseDate;
                tbUpdateName.Text = Base.Base.GetLocaleString(App.Applications[appIndex].Updates[updateIndex].Name);
                if (string.IsNullOrEmpty(App.Applications[appIndex].AppInfo.HelpUrl))
                    tbUrlHelp.Visibility = Visibility.Collapsed;
                else
                {
                    tbUrlHelp.Visibility = Visibility.Visible;
                    tbUrlHelp.Tag = App.Applications[appIndex].AppInfo.HelpUrl;
                }
                if (string.IsNullOrEmpty(App.Applications[appIndex].Updates[updateIndex].InfoUrl))
                    tbUrlInfo.Visibility = Visibility.Collapsed;
                else
                {
                    tbUrlInfo.Visibility = Visibility.Visible;
                    tbUrlInfo.Tag = App.Applications[appIndex].Updates[updateIndex].InfoUrl;
                }
                var item = (ListViewItem) listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);

                if (item.Tag != null)
                    cmiHideUpdate.Header = ((bool) item.Tag) ? App.RM.GetString("HideUpdate") : App.RM.GetString("ShowUpdate");
                else
                    cmiHideUpdate.Header = App.RM.GetString("HideUpdate");

                if (App.Applications[appIndex].Updates[updateIndex].Size > 0)
                {
                    tbStatus.Text = App.RM.GetString("ReadyToDownload");
                    imgArrow.Source = blueArrow;
                }
                else
                {
                    tbStatus.Text = App.RM.GetString("ReadyToInstall");
                    imgArrow.Source = greenArrow;
                }
                ShowLabels();
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
                expander.IsExpanded = expander.Tag.ToString() == App.RM.GetString("Optional");
            else
                expander.IsExpanded = expander.Tag.ToString() == App.RM.GetString("Important");
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