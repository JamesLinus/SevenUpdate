// ***********************************************************************
// <copyright file="RestoreUpdates.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using SevenUpdate.Windows;

    /// <summary>
    /// Interaction logic for Update_History.xaml
    /// </summary>
    public sealed partial class RestoreUpdates
    {
        #region Constants and Fields

        /// <summary>
        ///   Gets or sets a collection of SUH items
        /// </summary>
        private ObservableCollection<Suh> hiddenUpdates;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RestoreUpdates" /> class.
        /// </summary>
        public RestoreUpdates()
        {
            this.InitializeComponent();
            this.lvHiddenUpdates.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(this.RestrictColumn), true);
            this.btnRestore.IsShieldNeeded = !Core.Instance.IsAdmin;
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when one or more hidden updates have been restored
        /// </summary>
        public static event EventHandler<EventArgs> RestoredHiddenUpdate;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the hidden updates and loads them in the <see cref="ListView"/>
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void GetHiddenUpdates(object sender, RoutedEventArgs e)
        {
            this.hiddenUpdates = Utilities.Deserialize<ObservableCollection<Suh>>(Utilities.HiddenFile);
            if (this.hiddenUpdates == null)
            {
                return;
            }

            this.lvHiddenUpdates.ItemsSource = this.hiddenUpdates;
        }

        /// <summary>
        /// Un hides one or more updates and navigates to the Main page
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void RestoreUpdate(object sender, RoutedEventArgs e)
        {
            for (var x = 0; x < this.hiddenUpdates.Count; x++)
            {
                if (this.hiddenUpdates[x].Status != UpdateStatus.Visible)
                {
                    continue;
                }

                this.hiddenUpdates.RemoveAt(x);
                x--;
            }

            if (AdminClient.HideUpdates(this.hiddenUpdates))
            {
                if (RestoredHiddenUpdate != null)
                {
                    RestoredHiddenUpdate(this, new EventArgs());
                }
            }

            App.NavService.GoBack();
        }

        /// <summary>
        /// Limit the size of the <see cref="GridViewColumn"/> when it's being resized
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.
        /// </param>
        private void RestrictColumn(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize((Thumb)e.OriginalSource);
        }

        /// <summary>
        /// Shows the selected update details
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.
        /// </param>
        private void ShowDetails(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || this.lvHiddenUpdates.SelectedIndex == -1)
            {
                return;
            }

            var details = new UpdateDetails();
            details.ShowDialog(this.hiddenUpdates[this.lvHiddenUpdates.SelectedIndex]);
        }

        /// <summary>
        /// Shows the selected update details
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void ShowDetailsDialog(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(this.hiddenUpdates[this.lvHiddenUpdates.SelectedIndex]);
        }

        /// <summary>
        /// Updates the UI when an update check box is clicked
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void UpdateUIOnUpdateSelection(object sender, RoutedEventArgs e)
        {
            var checkedCount = this.hiddenUpdates.Count(t => t.Status == UpdateStatus.Visible);

            this.tbSelectedUpdates.Text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.TotalSelectedUpdates, checkedCount);
            this.btnRestore.IsEnabled = checkedCount > 0;
        }

        #endregion
    }
}