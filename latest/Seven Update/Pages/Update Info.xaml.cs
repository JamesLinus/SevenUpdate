/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Update_Info.xaml
    /// </summary>
    public partial class UpdateInfo : Page
    {
        #region Structs

        private struct Indexes
        {
            internal int appIndex { get; set; }
            internal int updateIndex { get; set; }
        }

        #endregion

        #region Global Vars

        ObservableCollection<Update> selectedUpdates;
        Collection<Indexes> indexes;
        #endregion

        public UpdateInfo()
        {
            InitializeComponent();
            if (App.IsAdmin())
                cmiHideUpdate.Icon = null;
        }

        #region Event Declarations

        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChangedEventHandler;

        #region EventArgs

        internal class UpdateSelectionChangedEventArgs : EventArgs
        {
            public UpdateSelectionChangedEventArgs(int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
            {
                this.ImportantUpdates = importantUpdates;

                this.OptionalUpdates = optionalUpdates;

                this.ImportantDownloadSize = importantDownloadSize;

                this.OptionalDownloadSize = optionalDownloadSize;
            }

            /// <summary>
            /// Number of Important Updates selected
            /// </summary>
            internal int ImportantUpdates { get; set; }

            /// <summary>
            /// Number of Optional Updates selected
            /// </summary>
            internal int OptionalUpdates { get; set; }

            /// <summary>
            /// The total download size in bytes of the important updates
            /// </summary>
            internal ulong ImportantDownloadSize { get; set; }

            /// <summary>
            /// The total download size in bytes of the optional updates
            /// </summary>
            internal ulong OptionalDownloadSize { get; set; }
        }

        #endregion

        #endregion

        #region UI Events

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            SaveUpdateSelection();
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = TextDecorations.Underline;

        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Main.LastPageVisited = "UpdateInfo";
            AddUpdates();
        }

        private void tbUrlInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbUrlInfo.Tag != null)
                System.Diagnostics.Process.Start(tbUrlInfo.Tag.ToString());
        }

        private void tbUrlHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbUrlHelp.Tag != null)
                System.Diagnostics.Process.Start(tbUrlHelp.Tag.ToString());
        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            int updateIndex = listView.SelectedIndex;
            int appIndex = indexes[updateIndex].appIndex;

            UpdateInformation hnh = new UpdateInformation();

            hnh.ApplicationName = App.Applications[appIndex].Name;

            hnh.HelpUrl = App.Applications[appIndex].HelpUrl;

            hnh.InfoUrl = selectedUpdates[updateIndex].InfoUrl;

            hnh.Publisher = App.Applications[appIndex].Publisher;

            hnh.PublisherUrl = App.Applications[appIndex].PublisherUrl;

            hnh.ReleaseDate = selectedUpdates[updateIndex].ReleaseDate;

            hnh.Status = UpdateStatus.Hidden;

            hnh.Size = App.GetUpdateSize(selectedUpdates[updateIndex].Files);

            hnh.Importance = selectedUpdates[updateIndex].Importance;

            hnh.Description = selectedUpdates[updateIndex].Description;

            hnh.UpdateTitle = selectedUpdates[updateIndex].Title;

            ListViewItem item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);

            if (cmiHideUpdate.Header.ToString() == App.RM.GetString("HideUpdate"))
            {
                if (SevenUpdate.WCF.Client.HideUpdate(hnh))
                {
                    cmiHideUpdate.Header = App.RM.GetString("ShowUpdate");
                    item.Foreground = System.Windows.Media.Brushes.Gray;

                }

            }
            else
            {
                if (SevenUpdate.WCF.Client.ShowUpdate(hnh))
                {
                    cmiHideUpdate.Header = App.RM.GetString("HideUpdate");
                    item.Foreground = System.Windows.Media.Brushes.Black;

                }
            }
        }

        #endregion

        #region Methods

        private void SaveUpdateSelection()
        {
            int[] count = new int[2];
            ulong[] downloadSize = new ulong[2];
            for (int x = 0; x < indexes.Count; x++)
            {
                App.Applications[indexes[x].appIndex].Updates[indexes[x].updateIndex].Selected = selectedUpdates[x].Selected;
                if (selectedUpdates[x].Selected)
                {
                    switch (selectedUpdates[x].Importance)
                    {
                        case Importance.Important: count[0]++;
                            downloadSize[0] = App.GetUpdateSize(selectedUpdates[x].Files, App.GetLocaleString(selectedUpdates[x].Title), App.GetLocaleString(App.Applications[indexes[x].appIndex].Name),
                                App.Applications[indexes[x].appIndex].Directory, App.Applications[indexes[x].appIndex].Is64Bit);
                            break;
                        case Importance.Recommended:
                            if (App.Settings.IncludeRecommended)
                            {
                                downloadSize[0] = App.GetUpdateSize(selectedUpdates[x].Files, App.GetLocaleString(selectedUpdates[x].Title), App.GetLocaleString(App.Applications[indexes[x].appIndex].Name),
                                    App.Applications[indexes[x].appIndex].Directory, App.Applications[indexes[x].appIndex].Is64Bit);
                                count[0]++;
                            }
                            else
                            {
                                downloadSize[1] = App.GetUpdateSize(selectedUpdates[x].Files, App.GetLocaleString(selectedUpdates[x].Title), App.GetLocaleString(App.Applications[indexes[x].appIndex].Name),
                                     App.Applications[indexes[x].appIndex].Directory, App.Applications[indexes[x].appIndex].Is64Bit);
                                count[1]++;
                            }
                            break;
                        case Importance.Optional:
                        case Importance.Locale:
                            downloadSize[0] = App.GetUpdateSize(selectedUpdates[x].Files, App.GetLocaleString(selectedUpdates[x].Title), App.GetLocaleString(App.Applications[indexes[x].appIndex].Name),
                                App.Applications[indexes[x].appIndex].Directory, App.Applications[indexes[x].appIndex].Is64Bit);
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
        void AddUpdates()
        {
            selectedUpdates = new ObservableCollection<Update>();
            indexes = new Collection<Indexes>();
            Indexes index = new Indexes();
            for (int x = 0; x < App.Applications.Count; x++)
            {
                for (int y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    selectedUpdates.Add(App.Applications[x].Updates[y]);
                    index.appIndex = x;
                    index.updateIndex = y;
                    indexes.Add(index);
                }
            }
            Binding items = new Binding();
            items.Source = selectedUpdates;
            listView.SetBinding(ItemsControl.ItemsSourceProperty, items);
        }

        /// <summary>
        /// Shows the update information in the sidebar
        /// </summary>
        void ShowLabels()
        {
            tbPublishedDate.Visibility = Visibility.Visible;
            tbPublishedLabel.Visibility = Visibility.Visible;
            tbUpdateDescription.Visibility = Visibility.Visible;
            tbUpdateTitle.Visibility = Visibility.Visible;
            tbUrlHelp.Visibility = Visibility.Visible;
            tbUrlInfo.Visibility = Visibility.Visible;
            imgIcon.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Hides the update information in the sidebar
        /// </summary>
        void HideLabels()
        {
            tbPublishedDate.Visibility = Visibility.Hidden;
            tbPublishedLabel.Visibility = Visibility.Hidden;
            tbUpdateDescription.Visibility = Visibility.Hidden;
            tbUpdateTitle.Visibility = Visibility.Hidden;
            tbUrlHelp.Visibility = Visibility.Hidden;
            tbUrlInfo.Visibility = Visibility.Hidden;
            imgIcon.Visibility = Visibility.Visible;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedIndex == -1)
                HideLabels();
            else
            {
                tbUpdateDescription.Text = App.GetLocaleString(selectedUpdates[listView.SelectedIndex].Description);
                tbPublishedDate.Text = selectedUpdates[listView.SelectedIndex].ReleaseDate;
                tbUpdateTitle.Text = App.GetLocaleString(selectedUpdates[listView.SelectedIndex].Title);
                tbUrlHelp.Tag = App.Applications[indexes[listView.SelectedIndex].appIndex].HelpUrl;
                tbUrlInfo.Tag = selectedUpdates[listView.SelectedIndex].InfoUrl;

                ShowLabels();
            }

        }

        #endregion
    }
}
