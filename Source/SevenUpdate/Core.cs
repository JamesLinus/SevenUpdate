using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using SevenUpdate.Pages;

namespace SevenUpdate
{
    public class Core : INotifyPropertyChanged
    {
        /// <summary>
        ///   Occurs when the user cancels their update selection
        /// </summary>
        internal static event EventHandler UpdateActionChanged;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
        private static Core instance;

        public static Core Instance
        {

            get { return instance ?? (instance = new Core()); }
        }

        #region Fields

        private static UpdateAction updateAction;

        #endregion

        #region Properties

        public UpdateAction UpdateAction
        {
            get { return updateAction; }
            set
            {
                updateAction = value;
                OnPropertyChanged("UpdateAction");
                if (UpdateActionChanged != null)
                    UpdateActionChanged(this, new EventArgs());
            }
        }

        /// <summary>
        ///   Gets a collection of software that Seven Update can check for updates
        /// </summary>
        private static IEnumerable<Sua> AppsToUpdate { get { return Base.Deserialize<Collection<Sua>>(Base.AppsFile); } }

        /// <summary>
        ///   Gets the update configuration settings
        /// </summary>
        public static Config Settings
        {
            get
            {
                var t = Base.Deserialize<Config>(Base.ConfigFile);
                return t ?? new Config { AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false };
            }
        }

        /// <summary>
        ///   Gets or Sets a collection of applications to update
        /// </summary>
        internal static Collection<Sui> Applications { get; set; }

        /// <summary>
        ///   Gets a value indicating if an auto check is being performed
        /// </summary>
        internal static bool IsAutoCheck { get; set; }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress
        /// </summary>
        internal static bool IsInstallInProgress { get; set; }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress and Seven Update was started after an autocheck
        /// </summary>
        internal static bool IsReconnect { get; set; }

        #endregion


        #region Recount Methods

        /// <summary>
        ///   Gets the total size of a single update
        /// </summary>
        /// <param name = "files">the collection of files of an update</param>
        /// <returns>a ulong value of the size of the update</returns>
        internal static ulong GetUpdateSize(IEnumerable<UpdateFile> files)
        {
            return files.Aggregate<UpdateFile, ulong>(0, (current, t) => current + t.FileSize);
        }

        #endregion

        /// <summary>
        ///   Checks for updates
        /// </summary>
        /// <param name = "auto"><c>true</c> if it's called because of an auto update check, otherwise <c>false</c></param>
        internal static void CheckForUpdates(bool auto)
        {
            if (auto)
            {
                if (!IsInstallInProgress && !Base.RebootNeeded)
                    CheckForUpdates();
            }
            else
                CheckForUpdates();
        }

        /// <summary>
        ///   Checks for updates
        /// </summary>
        internal static void CheckForUpdates()
        {
            if (!IsInstallInProgress)
            {
                if (Base.RebootNeeded == false)
                {
                    Instance.UpdateAction = UpdateAction.CheckingForUpdates;
                    Properties.Settings.Default.lastUpdateCheck = DateTime.Now;
                    IsInstallInProgress = true;
                    Search.SearchForUpdatesAync(AppsToUpdate);
                }
                else
                {
                    Instance.UpdateAction = UpdateAction.RebootNeeded;
                    MessageBox.Show(Properties.Resources.RebootNeededFirst, Properties.Resources.SevenUpdate, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show(Properties.Resources.AlreadyUpdating, Properties.Resources.SevenUpdate, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
