// <copyright file="About.xaml.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Windows
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Internal;

    /// <summary>Interaction logic for About.xaml.</summary>
    public sealed partial class About
    {
        /// <summary>Initializes a new instance of the <see cref="About" /> class.</summary>
        public About()
        {
            this.InitializeComponent();

            if (App.IsDev)
            {
                this.Title += " - " + Properties.Resources.DevChannel;
            }

            if (App.IsBeta)
            {
                this.Title += " - " + Properties.Resources.BetaChannel;
            }

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            this.tbVersion.Text = version.ToString();

            this.MouseLeftButtonDown -= Core.EnableDragOnGlass;
            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged -= this.ChangeWindowChrome;
            AeroGlass.CompositionChanged += this.ChangeWindowChrome;
        }

        /// <summary>Enables Aero Glass on the Window.</summary>
        /// <param name="e">The <c>EventArgs</c> instance containing the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this, new Margins(0, 0, 0, 41));

            if (AeroGlass.IsGlassEnabled)
            {
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
                this.Background = Brushes.Transparent;
            }
            else
            {
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
                this.Background = Brushes.White;
            }
        }

        /// <summary>Changes the Window Background when Aero Glass is enabled or disabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void ChangeWindowChrome(object sender, CompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
                this.Background = Brushes.Transparent;
                AeroGlass.EnableGlass(this, new Margins(0, 0, 0, 41));
            }
            else
            {
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
                this.Background = Brushes.White;
            }
        }

        /// <summary>Closes the About window.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>Enables the ability to drag the window on glass.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsGlassEnabled && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>Opens a browser and navigates to the Uri.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Navigation.RequestNavigateEventArgs</c> instance containing the event data.</param>
        private void NavigateToUri(object sender, RequestNavigateEventArgs e)
        {
            Utilities.StartProcess(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}