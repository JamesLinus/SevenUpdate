// <copyright file="UpdateHistory.xaml.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Pages
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenSoftware.Windows;

    using SevenUpdate.Windows;

    /// <summary>Interaction logic for UpdateHistory.xaml.</summary>
    public partial class UpdateHistory
    {
        /// <summary>Gets or sets a collection of SUH items.</summary>
        private ObservableCollection<Suh> updateHistory;

        /// <summary>Initializes a new instance of the <see cref="UpdateHistory" /> class.</summary>
        public UpdateHistory()
        {
            this.InitializeComponent();

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

        /// <summary>Gets the update history and loads it to the listView.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void GetHistory(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(App.HistoryFile))
            {
                return;
            }

            this.updateHistory = Utilities.Deserialize<ObservableCollection<Suh>>(App.HistoryFile);
            if (this.updateHistory == null)
            {
                return;
            }

            this.lvUpdateHistory.ItemsSource = this.updateHistory;
            this.updateHistory.CollectionChanged -= this.RefreshDataView;
            this.updateHistory.CollectionChanged += this.RefreshDataView;
        }

        /// <summary>Goes back to the Main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            Core.NavigateToMainPage();
        }

        /// <summary>Updates the <c>CollectionView</c> when the collection changes.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>NotifyCollectionChangedEventArgs</c> instance containing the event data.</param>
        private void RefreshDataView(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
            {
                return;
            }

            ICollectionView dataView = CollectionViewSource.GetDefaultView(this.lvUpdateHistory.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>Shows the selected update details.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void ShowDetails(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || this.lvUpdateHistory.SelectedIndex == -1)
            {
                return;
            }

            var details = new UpdateDetails();
            details.ShowDialog(this.updateHistory[this.lvUpdateHistory.SelectedIndex]);
        }

        /// <summary>Shows the selected update details.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void ShowDetailsDialog(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(this.updateHistory[this.lvUpdateHistory.SelectedIndex]);
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
    }
}