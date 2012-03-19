// <copyright file="UpdateReview.xaml.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    using SevenSoftware.Windows;

    using SevenUpdate.Sdk.Windows;

    /// <summary>Interaction logic for UpdateRegistry.xaml.</summary>
    public sealed partial class UpdateReview
    {
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

        /// <summary>Saves and exports the Project.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void SaveExportProject(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Core.SaveProject(true)).ContinueWith(
                delegate { this.Dispatcher.BeginInvoke(new Action(() => MainWindow.NavService.Navigate(Core.MainPage))); });
        }

        /// <summary>Saves the Project.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void SaveProject(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Core.SaveProject()).ContinueWith(
                delegate { this.Dispatcher.BeginInvoke(new Action(() => MainWindow.NavService.Navigate(Core.MainPage))); });
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
        {
            this.tbTitle.Foreground = e.IsGlassEnabled ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 51, 153));
        }
    }
}