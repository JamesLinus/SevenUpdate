// <copyright file="MainWindow.xaml.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Windows
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Navigation;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Internal;

    using SevenUpdate.Properties;

    /// <summary>Interaction logic for MainWindow.xaml.</summary>
    [ContentProperty]
    [TemplatePart(Name = @"PART_NavWinCP", Type = typeof(ContentPresenter))]
    public sealed partial class MainWindow
    {
        /// <summary>Initializes a new instance of the <see cref="MainWindow" /> class.</summary>
        public MainWindow()
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

            AeroGlass.CompositionChanged -= this.ChangeWindowChrome;
            AeroGlass.CompositionChanged += this.ChangeWindowChrome;

            App.TaskBar = this.taskBar;
            NavService = this.NavigationService;

            Core.NavigateToMainPage();
            App.ProcessArgs(App.Args);
        }

        /// <summary>Gets the <c>NavigationService</c> for the current window.</summary>
        /// <value>The nav service.</value>
        internal static NavigationService NavService { get; private set; }

        /// <summary>Enables Aero Glass on the Window.</summary>
        /// <param name="e">The <c>EventArgs</c> instance containing the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this, new Margins(0, 32, 0, 41));
            this.Background = AeroGlass.IsGlassEnabled ? Brushes.Transparent : Brushes.White;
        }

        /// <summary>Changes the Window Background when Aero Glass is enabled or disabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        void ChangeWindowChrome(object sender, CompositionChangedEventArgs e)
        {
            this.Background = e.IsGlassEnabled ? Brushes.Transparent : Brushes.White;

            if (!e.IsGlassEnabled)
            {
                return;
            }

            AeroGlass.EnableGlass(this, new Margins(0, 32, 0, 41));
        }

        /// <summary>Enables the ability to drag the window on glass.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsGlassEnabled && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>Sets the Height and Width of the window from the settings.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        void LoadWindowSize(object sender, RoutedEventArgs e)
        {
            this.Height = Settings.Default.WindowHeight;
            this.Width = Settings.Default.WindowWidth;
        }

        /// <summary>When Seven Update is closing, save the Window Width and Height in the settings.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.ComponentModel.CancelEventArgs</c> instance containing the event data.</param>
        void SaveWindowSize(object sender, CancelEventArgs e)
        {
            Settings.Default.WindowHeight = this.Height;
            Settings.Default.WindowWidth = this.Width;
            Settings.Default.Save();
            WcfService.Disconnect();
            Application.Current.Shutdown(0);
        }
    }
}