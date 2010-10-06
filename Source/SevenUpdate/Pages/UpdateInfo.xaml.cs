// ***********************************************************************
// Assembly         : SevenUpdate
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenUpdate.Converters;

    /// <summary>
    /// Interaction logic for Update_Info.xaml
    /// </summary>
    public sealed partial class UpdateInfo
    {
        #region Constants and Fields

        /// <summary>
        ///   Gets or Sets a list of indices relating to the current Update Collection
        /// </summary>
        private List<int> appIndices;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructor for the Update Info Page
        /// </summary>
        public UpdateInfo()
        {
            this.InitializeComponent();
            this.miUpdate.DataContext = Core.Instance.IsAdmin;
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the update selection has changed
        /// </summary>
        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or Sets a value indicating to expand the Optional Updates Group by default.
        /// </summary>
        internal static bool DisplayOptionalUpdates { private get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Loops through the <see cref="ListView"/> and updates the source when the update selection has been saved
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject"/>
        /// </param>
        private static void IterateVisualChild(DependencyObject element)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is CheckBox)
                {
                    var cb = VisualTreeHelper.GetChild(element, i) as CheckBox;
                    if (cb != null)
                    {
                        var bindingExpression = cb.GetBindingExpression(ToggleButton.IsCheckedProperty);
                        if (bindingExpression != null)
                        {
                            bindingExpression.UpdateSource();
                        }
                    }

                    continue;
                }

                IterateVisualChild(VisualTreeHelper.GetChild(element, i));
            }
        }

        /// <summary>
        /// Adds the updates to the list
        /// </summary>
        private void AddUpdates()
        {
            var selectedUpdates = new ObservableCollection<Update>();
            this.appIndices = new List<int>();

            for (var x = 0; x < Core.Applications.Count; x++)
            {
                this.appIndices.Add(x);
                foreach (var t in Core.Applications[x].Updates)
                {
                    selectedUpdates.Add(t);
                }
            }

            this.lvUpdates.ItemsSource = selectedUpdates;
            var myView = (CollectionView)CollectionViewSource.GetDefaultView(this.lvUpdates.ItemsSource);
            var groupDescription = new PropertyGroupDescription("Importance", new ImportanceToString());
            if (myView.GroupDescriptions != null)
            {
                myView.GroupDescriptions.Add(groupDescription);
            }
        }

        /// <summary>
        /// Navigates back to the Main page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Core.NavService.GoBack();
        }

        /// <summary>
        /// Expands the group expander based on the which link was clicked from the main page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Expander_Loaded(object sender, RoutedEventArgs e)
        {
            var expander = e.Source as Expander;
            if (expander == null)
            {
                return;
            }

            if (DisplayOptionalUpdates)
            {
                expander.IsExpanded = expander.Tag.ToString() == Properties.Resources.Optional;
            }
            else
            {
                expander.IsExpanded = expander.Tag.ToString() == Properties.Resources.Important;
            }
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
            var appIndex = this.appIndices[this.lvUpdates.SelectedIndex];

            var update = this.lvUpdates.SelectedItem as Update;
            if (update == null)
            {
                return;
            }

            var hnh = new Suh
                {
                    HelpUrl = Core.Applications[appIndex].AppInfo.HelpUrl, 
                    InfoUrl = update.InfoUrl, 
                    Publisher = Core.Applications[appIndex].AppInfo.Publisher, 
                    AppUrl = Core.Applications[appIndex].AppInfo.AppUrl, 
                    ReleaseDate = update.ReleaseDate, 
                    Status = UpdateStatus.Hidden, 
                    UpdateSize = Core.GetUpdateSize(update.Files), 
                    Importance = update.Importance, 
                    Description = update.Description, 
                    Name = update.Name
                };

            if (!update.Hidden)
            {
                if (AdminClient.HideUpdate(hnh))
                {
                    update.Hidden = true;
                    update.Selected = false;
                }
            }
            else
            {
                if (AdminClient.ShowUpdate(hnh))
                {
                    update.Hidden = false;
                }
            }
        }

        /// <summary>
        /// Saves the selection of updates and navigates back to the Main page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            IterateVisualChild(this.lvUpdates);
            this.SaveUpdateSelection();
            Core.NavService.GoBack();
        }

        /// <summary>
        /// Loads the updates found into the UI
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.AddUpdates();
            this.lvUpdates.SelectedIndex = 0;
        }

        /// <summary>
        /// Saves the update selection
        /// </summary>
        private void SaveUpdateSelection()
        {
            var count = new int[2];
            var downloadSize = new ulong[2];
            var updIndex = -1;
            for (var x = 0; x < Core.Applications.Count; x++)
            {
                for (var y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    updIndex++;
                    var item = (ListViewItem)this.lvUpdates.ItemContainerGenerator.ContainerFromItem(this.lvUpdates.Items[updIndex]);
                    if (item.Tag != null)
                    {
                        if (!(bool)item.Tag)
                        {
                            Core.Applications[x].Updates.RemoveAt(y);
                            y--;
                            continue;
                        }
                    }

                    if (!Core.Applications[x].Updates[y].Selected)
                    {
                        continue;
                    }

                    switch (Core.Applications[x].Updates[y].Importance)
                    {
                        case Importance.Important:
                            count[0]++;
                            downloadSize[0] = Core.Applications[x].Updates[y].Size;
                            break;
                        case Importance.Recommended:
                            if (Core.Settings.IncludeRecommended)
                            {
                                downloadSize[0] = Core.Applications[x].Updates[y].Size;
                                count[0]++;
                            }
                            else
                            {
                                downloadSize[1] = Core.Applications[x].Updates[y].Size;
                                count[1]++;
                            }

                            break;
                        case Importance.Optional:
                        case Importance.Locale:
                            downloadSize[1] = Core.Applications[x].Updates[y].Size;
                            count[1]++;
                            break;
                    }
                }

                if (Core.Applications[x].Updates.Count != 0)
                {
                    continue;
                }

                Core.Applications.RemoveAt(x);
                x--;
            }

            if (UpdateSelectionChanged != null)
            {
                UpdateSelectionChanged(this, new UpdateSelectionChangedEventArgs(count[0], count[1], downloadSize[0], downloadSize[1]));
            }
        }

        /// <summary>
        /// Launches the Help <c>Url</c> of the update
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void UrlHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(Core.Applications[this.appIndices[this.lvUpdates.SelectedIndex]].AppInfo.HelpUrl);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Launches the More Information <c>Url</c> of the update
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void UrlInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(((Update)this.lvUpdates.SelectedItem).InfoUrl);
            }
            catch
            {
            }
        }

        #endregion
    }

    #region EventArgs

    /// <summary>
    /// Provides event data for the UpdateSelection event
    /// </summary>
    internal sealed class UpdateSelectionChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Contains event data associated with this event
        /// </summary>
        /// <param name="importantUpdates">
        /// The number of Important updates selected
        /// </param>
        /// <param name="optionalUpdates">
        /// The number of Optional updates selected
        /// </param>
        /// <param name="importantDownloadSize">
        /// A value indicating the download size of the Important updates
        /// </param>
        /// <param name="optionalDownloadSize">
        /// A value indicating the download size of the Optional updates
        /// </param>
        public UpdateSelectionChangedEventArgs(int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
        {
            this.ImportantUpdates = importantUpdates;

            this.OptionalUpdates = optionalUpdates;

            this.ImportantDownloadSize = importantDownloadSize;

            this.OptionalDownloadSize = optionalDownloadSize;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the total download size in bytes of the important updates
        /// </summary>
        internal ulong ImportantDownloadSize { get; private set; }

        /// <summary>
        ///   Gets the number of Important Updates selected
        /// </summary>
        internal int ImportantUpdates { get; private set; }

        /// <summary>
        ///   Gets the total download size in bytes of the optional updates
        /// </summary>
        internal ulong OptionalDownloadSize { get; private set; }

        /// <summary>
        ///   Gets the number of Optional Updates selected
        /// </summary>
        internal int OptionalUpdates { get; private set; }

        #endregion
    }

    #endregion
}