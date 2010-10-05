// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

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

    using Microsoft.Windows.Controls;

    using SevenUpdate.Windows;

    /// <summary>
    /// Interaction logic for Update_History.xaml
    /// </summary>
    public sealed partial class RestoreUpdates
    {
        #region Constants and Fields

        /// <summary>
        ///   Gets or Sets a collection of SUH items
        /// </summary>
        private ObservableCollection<Suh> hiddenUpdates;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   The constructor for Restore Updates page
        /// </summary>
        public RestoreUpdates()
        {
            this.InitializeComponent();
            this.lvHiddenUpdates.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(this.Thumb_DragDelta), true);
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
        /// Updates the UI when an update check box is clicked
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkedCount = this.hiddenUpdates.Count(t => t.Status == UpdateStatus.Visible);

            this.tbSelectedUpdates.Text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.TotalSelectedUpdates, checkedCount);
            this.btnRestore.IsEnabled = checkedCount > 0;
        }

        /// <summary>
        /// Gets the hidden updates and loads them in the lvHiddenUpdates
        /// </summary>
        private void GetHiddenUpdates()
        {
            this.hiddenUpdates = Base.Deserialize<ObservableCollection<Suh>>(Base.HiddenFile);
            if (this.hiddenUpdates == null)
            {
                return;
            }

            this.lvHiddenUpdates.ItemsSource = this.hiddenUpdates;
        }

        /// <summary>
        /// Shows the selected update details
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
        /// </param>
        /// <param name="e">
        /// </param>
        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(this.hiddenUpdates[this.lvHiddenUpdates.SelectedIndex]);
        }

        /// <summary>
        /// Loads the collection of hidden updates when the page is loaded
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.GetHiddenUpdates();
        }

        /// <summary>
        /// Unhides one or more updates and navigates to the Main page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Restore_Click(object sender, RoutedEventArgs e)
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

            Core.NavService.GoBack();
        }

        /// <summary>
        /// Limit the size of the <see cref="GridViewColumn"/> when it's being resized
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize((Thumb)e.OriginalSource);
        }

        #endregion
    }
}