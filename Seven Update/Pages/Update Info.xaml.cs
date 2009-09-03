#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
// 
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

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
using SevenUpdate.Controls;
using SevenUpdate.WCF;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Update_Info.xaml
    /// </summary>
    public sealed partial class UpdateInfo : Page
    {
        #region Structs

        private struct Indexes
        {
            internal int appIndex { get; set; }
            internal int updateIndex { get; set; }
        }

        #endregion

        #region Global Vars

        private readonly BitmapImage blueArrow = new BitmapImage(new Uri("/Images/BlueArrow.png", UriKind.Relative));
        private readonly BitmapImage greenArrow = new BitmapImage(new Uri("/Images/GreenArrow.png", UriKind.Relative));
        private List<Indexes> indexes;

        #endregion

        public UpdateInfo()
        {
            InitializeComponent();
            if (App.IsAdmin)
                cmiHideUpdate.Icon = null;
        }

        #region Event Declarations

        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChangedEventHandler;

        #region EventArgs

        internal sealed class UpdateSelectionChangedEventArgs : EventArgs
        {
            public UpdateSelectionChangedEventArgs(int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
            {
                ImportantUpdates = importantUpdates;

                OptionalUpdates = optionalUpdates;

                ImportantDownloadSize = importantDownloadSize;

                OptionalDownloadSize = optionalDownloadSize;
            }

            /// <summary>
            /// Number of Important Updates selected
            /// </summary>
            internal int ImportantUpdates { get; private set; }

            /// <summary>
            /// Number of Optional Updates selected
            /// </summary>
            internal int OptionalUpdates { get; private set; }

            /// <summary>
            /// The total download size in bytes of the important updates
            /// </summary>
            internal ulong ImportantDownloadSize { get; private set; }

            /// <summary>
            /// The total download size in bytes of the optional updates
            /// </summary>
            internal ulong OptionalDownloadSize { get; private set; }
        }

        #endregion

        #endregion

        #region UI Events

        #region Buttons

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            SaveUpdateSelection();
            MainWindow.NavService.GoBack();
        }

        #endregion

        #region TextBlocks

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock) sender);
            textBlock.TextDecorations = TextDecorations.Underline;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock) sender);
            textBlock.TextDecorations = null;
        }

        private void tbUrlInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbUrlInfo.Tag != null)
                Process.Start(tbUrlInfo.Tag.ToString());
        }

        private void tbUrlHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbUrlHelp.Tag != null)
                Process.Start(tbUrlHelp.Tag.ToString());
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddUpdates();
        }

        #region listview

        private void SelectedUpdates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.Refresh();
        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var updateIndex = indexes[listView.SelectedIndex].updateIndex;
            var appIndex = indexes[listView.SelectedIndex].appIndex;

            var hnh = new SUH
                          {
                              HelpUrl = App.Applications[appIndex].HelpUrl,
                              InfoUrl = App.Applications[appIndex].Updates[updateIndex].InfoUrl,
                              Publisher = App.Applications[appIndex].Publisher,
                              PublisherUrl = App.Applications[appIndex].PublisherUrl,
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
                if (Admin.HideUpdate(hnh))
                {
                    cmiHideUpdate.Header = App.RM.GetString("ShowUpdate");
                    item.Foreground = Brushes.Gray;
                }
            }
            else
            {
                if (Admin.ShowUpdate(hnh))
                {
                    cmiHideUpdate.Header = App.RM.GetString("HideUpdate");
                    item.Foreground = Brushes.Black;
                }
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var appIndex = indexes[listView.SelectedIndex].appIndex;
            var updateIndex = indexes[listView.SelectedIndex].updateIndex;
            if (listView.SelectedIndex == -1)
                HideLabels();
            else
            {
                tbUpdateDescription.Text = Shared.GetLocaleString(App.Applications[appIndex].Updates[updateIndex].Description);
                tbPublishedDate.Text = App.Applications[appIndex].Updates[updateIndex].ReleaseDate;
                tbUpdateName.Text = Shared.GetLocaleString(App.Applications[appIndex].Updates[updateIndex].Name);
                tbUrlHelp.Tag = App.Applications[appIndex].HelpUrl;
                tbUrlInfo.Tag = App.Applications[appIndex].Updates[updateIndex].InfoUrl;
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

        #endregion

        #endregion

        #region Methods

        private static void IterateVisualChild(DependencyObject element)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is CheckBox)
                {
                    var cb = VisualTreeHelper.GetChild(element, i) as CheckBox;
                    if (cb != null)
                    {
                        var bindexp = cb.GetBindingExpression(ToggleButton.IsCheckedProperty);
                        if (bindexp != null)
                            bindexp.UpdateSource();
                    }
                    continue;
                }
                IterateVisualChild(VisualTreeHelper.GetChild(element, i));
            }
        }

        private void SaveUpdateSelection()
        {
            var count = new int[2];
            var downloadSize = new ulong[2];

            IterateVisualChild(listView);
            for (var x = 0; x < indexes.Count; x++)
            {
                var updateIndex = indexes[x].updateIndex;
                var appIndex = indexes[x].appIndex;
                if (!App.Applications[appIndex].Updates[updateIndex].Selected) {}
                else
                {
                    switch (App.Applications[indexes[x].appIndex].Updates[indexes[x].updateIndex].Importance)
                    {
                        case Importance.Important:
                            count[0]++;
                            downloadSize[0] = App.Applications[appIndex].Updates[updateIndex].Size;
                            break;
                        case Importance.Recommended:
                            if (App.Settings.IncludeRecommended)
                            {
                                downloadSize[0] = App.Applications[appIndex].Updates[updateIndex].Size;
                                count[0]++;
                            }
                            else
                            {
                                downloadSize[1] = App.Applications[appIndex].Updates[updateIndex].Size;
                                count[1]++;
                            }
                            break;
                        case Importance.Optional:
                        case Importance.Locale:
                            downloadSize[0] = App.Applications[appIndex].Updates[updateIndex].Size;
                            count[1]++;
                            break;
                    }
                }
            }
            if (UpdateSelectionChangedEventHandler != null)
                UpdateSelectionChangedEventHandler(this, new UpdateSelectionChangedEventArgs(count[0], count[1], downloadSize[0], downloadSize[1]));
        }

        /// <summary>
        /// Adds the updates to the list
        /// </summary>
        private void AddUpdates()
        {
            var selectedUpdates = new ObservableCollection<Update>();
            indexes = new List<Indexes>();
            var index = new Indexes();

            for (var x = 0; x < App.Applications.Count; x++)
            {
                for (var y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    selectedUpdates.Add(App.Applications[x].Updates[y]);
                    index.appIndex = x;
                    index.updateIndex = y;
                    indexes.Add(index);
                }
            }
            var items = new Binding {Source = selectedUpdates};
            listView.SetBinding(ItemsControl.ItemsSourceProperty, items);
            selectedUpdates.CollectionChanged += SelectedUpdates_CollectionChanged;
            AddSortBinding();
        }

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
        /// Shows the update information in the sidebar
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
        /// Hides the update information in the sidebar
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
    }
}