// ***********************************************************************
// <copyright file="UpdateHistory.xaml.cs"
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
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using SevenUpdate.Windows;

    /// <summary>
    /// Interaction logic for Update_History.xaml
    /// </summary>
    public partial class UpdateHistory
    {
        #region Constants and Fields

        /// <summary>
        ///   The location of the update history file
        /// </summary>
        private static readonly string HistoryFile = Utilities.AllUserStore + @"History.suh";

        /// <summary>
        ///   Gets or Sets a collection of SUH items
        /// </summary>
        private ObservableCollection<Suh> updateHistory;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   The constructor for the Update History page
        /// </summary>
        public UpdateHistory()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the update history and loads it to the listView
        /// </summary>
        private void GetHistory()
        {
            this.updateHistory = Utilities.Deserialize<ObservableCollection<Suh>>(HistoryFile);
            if (this.updateHistory == null)
            {
                return;
            }

            this.lvUpdateHistory.ItemsSource = this.updateHistory;
            this.updateHistory.CollectionChanged += this.History_CollectionChanged;
        }

        /// <summary>
        /// Updates the <see cref="CollectionView"/> when the <c>updateHistory</c> collection changes
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void History_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
            {
                return;
            }

            var dataView = CollectionViewSource.GetDefaultView(this.lvUpdateHistory.ItemsSource);
            dataView.Refresh();
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
            if (e.ClickCount != 2 || this.lvUpdateHistory.SelectedIndex == -1)
            {
                return;
            }

            var details = new UpdateDetails();
            details.ShowDialog(this.updateHistory[this.lvUpdateHistory.SelectedIndex]);
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
            details.ShowDialog(this.updateHistory[this.lvUpdateHistory.SelectedIndex]);
        }

        /// <summary>
        /// Loads the update history when the page is loaded
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.GetHistory();
        }

        #endregion
    }
}