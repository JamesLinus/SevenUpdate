// <copyright file="UpdateInfo.xaml.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Controls;
    using SevenSoftware.Windows.Dialogs.TaskDialog;
    using SevenSoftware.Windows.ValidationRules;

    using SevenUpdate.Sdk.Windows;

    /// <summary>Interaction logic for UpdateInfo.xaml.</summary>
    public sealed partial class UpdateInfo
    {
        /// <summary>Initializes a new instance of the <see cref="UpdateInfo" /> class.</summary>
        public UpdateInfo()
        {
            this.InitializeComponent();
            this.DataContext = Core.UpdateInfo;
            if (string.IsNullOrWhiteSpace(Core.UpdateInfo.ReleaseDate))
            {
                Core.UpdateInfo.ReleaseDate = DateTime.Now.ToShortDateString();
            }

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

        /// <summary>Fires the OnPropertyChanged Event with the collection changes.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data.</param>
        void ChangeDescription(object sender, RoutedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;

            Core.UpdateLocaleStrings(textBox.Text, Core.UpdateInfo.Description);
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data.</param>
        void ChangeName(object sender, RoutedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;

            Core.UpdateLocaleStrings(textBox.Text, Core.UpdateInfo.Name);
        }

        /// <summary>Determines whether this instance has errors.</summary>
        /// <returns><c>True</c> if this instance has errors; otherwise, <c>False</c>.</returns>
        bool HasErrors()
        {
            return this.tbxUpdateName.HasError || this.tbxUpdateDetails.HasError || this.tbxSourceLocation.HasError
                   || this.imgReleaseDate.Visibility == Visibility.Visible;
        }

        /// <summary>Loads the <c>LocaleString</c>'s for the <c>Update</c> into the UI.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Controls.SelectionChangedEventArgs</c> instance containing the event data.</param>
        void LoadLocaleStrings(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxUpdateName == null || this.cbxLocale.SelectedIndex < 0)
            {
                return;
            }

            Utilities.Locale = ((ComboBoxItem)this.cbxLocale.SelectedItem).Tag.ToString();

            bool found = false;

            // Load Values
            foreach (var t in Core.UpdateInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateName.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxUpdateName.Text = null;
            }

            found = false;

            // Load Values
            foreach (var t in Core.UpdateInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateDetails.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxUpdateDetails.Text = null;
            }
        }

        /// <summary>Loads the <c>Update</c> information to the UI.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        void LoadUI(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbxUpdateDetails.Text))
            {
                this.tbxUpdateDetails.HasError = true;
                this.tbxUpdateDetails.ToolTip = Properties.Resources.InputRequired;
            }
            else
            {
                this.tbxUpdateDetails.HasError = false;
                this.tbxUpdateDetails.ToolTip = null;
            }

            if (string.IsNullOrWhiteSpace(this.tbxUpdateName.Text))
            {
                this.tbxUpdateName.HasError = true;
                this.tbxUpdateName.ToolTip = Properties.Resources.InputRequired;
            }
            else
            {
                this.tbxUpdateName.HasError = false;
                this.tbxUpdateName.ToolTip = null;
            }

            var urlRule = new UrlInputRule { IsRequired = true };
            this.tbxSourceLocation.HasError = !urlRule.Validate(this.tbxSourceLocation.Text, null).IsValid;
            urlRule.IsRequired = false;

            this.tbxLicenseUrl.HasError = !urlRule.Validate(this.tbxLicenseUrl.Text, null).IsValid;
            this.tbxUpdateInfoUrl.HasError = !urlRule.Validate(this.tbxUpdateInfoUrl.Text, null).IsValid;

            // Load Values
            foreach (var t in Core.UpdateInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateDetails.Text = t.Value;
            }

            foreach (var t in Core.UpdateInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateName.Text = t.Value;
            }
        }

        /// <summary>Moves on to the next pages if no errors are present.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(Core.UpdateFilesPage);
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>Navigates to the main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(Core.MainPage);
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        void UpdateUI(object sender, CompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>Validates the textbox to see if required input exists.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data.</param>
        void ValidateInputRequired(object sender, TextChangedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.HasError = true;
                textBox.ToolTip = Properties.Resources.InputRequired;
            }
            else
            {
                textBox.HasError = false;
                textBox.ToolTip = null;
            }
        }

        /// <summary>Validates input to see if it's a valid URL.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data.</param>
        void ValidateUrlInput(object sender, TextChangedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;

            if (textBox == null)
            {
                return;
            }

            textBox.HasError =
                !new UrlInputRule { IsRequired = textBox.Name == @"tbxSourceLocation" }.Validate(textBox.Text, null).
                     IsValid;
            textBox.ToolTip = textBox.HasError ? Properties.Resources.UrlNotValid : null;
        }
    }
}