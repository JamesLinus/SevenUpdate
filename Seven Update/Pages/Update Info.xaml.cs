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
using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        Collection<Indexes> indexes;

        BitmapImage greenArrow = new BitmapImage(new Uri("/Images/GreenArrow.png", UriKind.Relative));
        BitmapImage blueArrow = new BitmapImage(new Uri("/Images/BlueArrow.png", UriKind.Relative));

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

        #region Buttons

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            SaveUpdateSelection();
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        #endregion

        #region TextBlocks

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

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddUpdates();
        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            int updateIndex = indexes[listView.SelectedIndex].updateIndex;
            int appIndex = indexes[listView.SelectedIndex].appIndex;

            UpdateInformation hnh = new UpdateInformation();

            hnh.ApplicationName = App.Applications[appIndex].Name;

            hnh.HelpUrl = App.Applications[appIndex].HelpUrl;

            hnh.InfoUrl = App.Applications[appIndex].Updates[updateIndex].InfoUrl;

            hnh.Publisher = App.Applications[appIndex].Publisher;

            hnh.PublisherUrl = App.Applications[appIndex].PublisherUrl;

            hnh.ReleaseDate = App.Applications[appIndex].Updates[updateIndex].ReleaseDate;

            hnh.Status = UpdateStatus.Hidden;

            hnh.Size = App.GetUpdateSize(App.Applications[appIndex].Updates[updateIndex].Files);

            hnh.Importance = App.Applications[appIndex].Updates[updateIndex].Importance;

            hnh.Description = App.Applications[appIndex].Updates[updateIndex].Description;

            hnh.UpdateTitle = App.Applications[appIndex].Updates[updateIndex].Title;

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

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int appIndex = indexes[listView.SelectedIndex].appIndex;
            int updateIndex = indexes[listView.SelectedIndex].updateIndex;
            if (listView.SelectedIndex == -1)
                HideLabels();
            else
            {
                tbUpdateDescription.Text = App.GetLocaleString(App.Applications[appIndex].Updates[updateIndex].Description);
                tbPublishedDate.Text = App.Applications[appIndex].Updates[updateIndex].ReleaseDate;
                tbUpdateTitle.Text = App.GetLocaleString(App.Applications[appIndex].Updates[updateIndex].Title);
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

        #region Methods

        void IterateVisualChild(DependencyObject element)
        {
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    if (VisualTreeHelper.GetChild(element, i) is System.Windows.Controls.CheckBox)
                    {
                        System.Windows.Controls.CheckBox cb = VisualTreeHelper.GetChild(element, i) as System.Windows.Controls.CheckBox;
                        BindingExpression bindexp = cb.GetBindingExpression(CheckBox.IsCheckedProperty);
                        bindexp.UpdateSource();
                        continue;
                    }
                    IterateVisualChild(VisualTreeHelper.GetChild(element, i));
                }
            }
            catch (Exception)
            {
            }
        }

        private void SaveUpdateSelection()
        {
            int[] count = new int[2];
            ulong[] downloadSize = new ulong[2];

            int appIndex = 0, updateIndex = 0;

            IterateVisualChild(listView);
            for (int x = 0; x < indexes.Count; x++)
            {
                updateIndex = indexes[x].updateIndex;
                appIndex = indexes[x].appIndex;
                if (App.Applications[appIndex].Updates[updateIndex].Selected == true)
                {
                    switch (App.Applications[indexes[x].appIndex].Updates[indexes[x].updateIndex].Importance)
                    {
                        case Importance.Important: count[0]++;
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
        void AddUpdates()
        {
            ObservableCollection<Update> selectedUpdates = new ObservableCollection<Update>();
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
            tbStatus.Visibility = Visibility.Visible;
            tbPublishedDate.Visibility = Visibility.Visible;
            tbPublishedLabel.Visibility = Visibility.Visible;
            tbUpdateDescription.Visibility = Visibility.Visible;
            tbUpdateTitle.Visibility = Visibility.Visible;
            tbUrlHelp.Visibility = Visibility.Visible;
            tbUrlInfo.Visibility = Visibility.Visible;
            imgIcon.Visibility = Visibility.Hidden;
            imgArrow.Visibility = Visibility.Visible;
            
        }

        /// <summary>
        /// Hides the update information in the sidebar
        /// </summary>
        void HideLabels()
        {
            tbStatus.Visibility = Visibility.Hidden;
            tbPublishedDate.Visibility = Visibility.Hidden;
            tbPublishedLabel.Visibility = Visibility.Hidden;
            tbUpdateDescription.Visibility = Visibility.Hidden;
            tbUpdateTitle.Visibility = Visibility.Hidden;
            tbUrlHelp.Visibility = Visibility.Hidden;
            tbUrlInfo.Visibility = Visibility.Hidden;
            imgIcon.Visibility = Visibility.Visible;
            imgArrow.Visibility = Visibility.Hidden;
        }

        #endregion
    }


}
