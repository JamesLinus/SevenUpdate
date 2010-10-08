// ***********************************************************************
// <copyright file="Options.xaml.cs"
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
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Navigation;

    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options
    {
        #region Constants and Fields

        /// <summary>
        ///   The Seven Update list location
        /// </summary>
        private const string SulLocation = @"http://sevenupdate.com/apps/Apps.sul";

        /// <summary>
        ///   A collection of SUA's that Seven Update can update
        /// </summary>
        private static ObservableCollection<Sua> machineAppList;

        /// <summary>
        /// </summary>
        private Config config;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   The constructor for the Options Page
        /// </summary>
        public Options()
        {
            this.InitializeComponent();
            this.lvApps.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(this.Thumb_DragDelta), true);
            this.btnSave.IsShieldNeeded = !Core.Instance.IsAdmin;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Downloads the Seven Update Application List
        /// </summary>
        private void DownloadSul()
        {
            try
            {
                this.LoadSul(Utilities.Deserialize<ObservableCollection<Sua>>(Utilities.DownloadFile(SulLocation), SulLocation));
            }
            catch (WebException)
            {
                this.LoadSul();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
            }
            catch (Exception)
            {
            }

            e.Handled = true;
        }

        /// <summary>
        /// Loads the list of Seven Update applications and sets the UI, if no application list was downloaded, load the stored list on the system
        /// </summary>
        /// <param name="officialAppList">
        /// </param>
        private void LoadSul(ObservableCollection<Sua> officialAppList = null)
        {
            machineAppList = Utilities.Deserialize<ObservableCollection<Sua>>(Utilities.ApplicationsFile);

            if (machineAppList != null)
            {
                for (var x = 0; x < machineAppList.Count; x++)
                {
                    if (
                        Directory.Exists(
                            Utilities.IsRegistryKey(machineAppList[x].Directory)
                                ? Utilities.GetRegistryValue(machineAppList[x].Directory, machineAppList[x].ValueName, machineAppList[x].Is64Bit)
                                : Utilities.ConvertPath(machineAppList[x].Directory, true, machineAppList[x].Is64Bit)) && machineAppList[x].IsEnabled)
                    {
                        continue;
                    }

                    // Remove the application from the list if it is no longer installed or not enabled
                    machineAppList.RemoveAt(x);
                    x--;
                }
            }

            if (officialAppList != null)
            {
                for (var x = 0; x < officialAppList.Count; x++)
                {
                    if (
                        !Directory.Exists(
                            Utilities.IsRegistryKey(officialAppList[x].Directory)
                                ? Utilities.GetRegistryValue(officialAppList[x].Directory, officialAppList[x].ValueName, officialAppList[x].Is64Bit)
                                : Utilities.ConvertPath(officialAppList[x].Directory, true, officialAppList[x].Is64Bit)))
                    {
                        // Remove the application from the list if it is not installed
                        officialAppList.RemoveAt(x);
                        x--;
                    }
                    else
                    {
                        if (machineAppList == null)
                        {
                            continue;
                        }

                        for (var y = 0; y < machineAppList.Count; y++)
                        {
                            // Check if the app in both lists are the same
                            if (officialAppList[x].Directory == machineAppList[y].Directory && officialAppList[x].Is64Bit == machineAppList[y].Is64Bit)
                            {
                                // if (officialAppList[x].Source != machineAppList[y].Source)
                                // continue;
                                officialAppList[x].IsEnabled = machineAppList[y].IsEnabled;
                                machineAppList.RemoveAt(y);
                                y--;
                            }

                            continue;
                        }
                    }
                }

                if (machineAppList != null)
                {
                    foreach (var t in machineAppList)
                    {
                        officialAppList.Add(t);
                    }
                }
            }

            if (officialAppList != null)
            {
                machineAppList = officialAppList;
            }

            this.Dispatcher.BeginInvoke(this.UpdateList);
        }

        /// <summary>
        /// Loads the settings and SUA list when the page is loaded
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.lvApps.Cursor = Cursors.Wait;
            this.config = Core.Settings;
            this.DataContext = this.config;

            Task.Factory.StartNew(this.DownloadSul);
        }

        /// <summary>
        /// Saves the settings and goes back to the Main page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            AdminClient.SaveSettings(this.config.AutoOption != AutoUpdateOption.Never, this.config, machineAppList);
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

        /// <summary>
        /// Updates the list with the <see cref="machineAppList"/>
        /// </summary>
        private void UpdateList()
        {
            this.lvApps.Cursor = Cursors.Arrow;
            if (machineAppList != null)
            {
                this.lvApps.ItemsSource = machineAppList;
            }
        }

        #endregion
    }
}