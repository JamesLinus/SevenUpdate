/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using SevenUpdate.Properties;
using SevenUpdate.WCF;

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Page
    {
        #region Global Vars

        ObservableCollection<SUA> userAppList;

        /// <summary>
        /// The Seven Update list location
        /// </summary>
        static Uri suaLoc = new Uri("http://ittakestime.org/su/SUlist.sul");

        #endregion

        public Options()
        {
            InitializeComponent();
            listView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
            if (App.IsAdmin)
                btnSave.Content = App.RM.GetString("Save");
        }

        #region Event Declarations

        internal static event EventHandler<SettingsSavedEventArgs> SettingsSavedEventHandler;

        #region EventArgs

        internal class SettingsSavedEventArgs : EventArgs
        {
            internal SettingsSavedEventArgs(Config settings)
            {
                this.Settings = settings;
            }

            /// <summary>
            /// The current settings for Seven Update
            /// </summary>
            internal Config Settings { get; set; }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Downloads the Seven Update Application List
        /// </summary>
        void DownloadSUL()
        {
            listView.Cursor = Cursors.Wait;

            File.Delete(Shared.userStore + @"list.sul");

            WebClient wc = new WebClient();

            wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wc_DownloadFileCompleted);

            wc.DownloadFileAsync(suaLoc, Shared.userStore + @"list.sul");
            wc.Dispose();
        }

        /// <summary>
        /// Loads the configuration and sets the UI
        /// </summary>
        void LoadSettings()
        {
            switch (App.Settings.AutoOption)
            {
                case AutoUpdateOption.Install: cbAutoUpdateMethod.SelectedIndex = 0; break;

                case AutoUpdateOption.Download: cbAutoUpdateMethod.SelectedIndex = 1; break;

                case AutoUpdateOption.Notify: cbAutoUpdateMethod.SelectedIndex = 2; break;

                case AutoUpdateOption.Never: cbAutoUpdateMethod.SelectedIndex = 3; break;
            }

            chkRecommendedUpdates.IsChecked = App.Settings.IncludeRecommended;
        }

        string Convert64BoolToString(bool Is64Bit)
        {
            if (Is64Bit)
                return "x64";
            else
                return "x86";
        }

        /// <summary>
        /// Loads the list of Seven Update applications and sets the ui
        /// </summary>
        void LoadSUL()
        {
            if (!File.Exists(Shared.userStore + @"list.sul"))
                DownloadSUL();
            else
            {
                ObservableCollection<SUA> officalAppList = Shared.DeserializeCollection<SUA>(Shared.userStore + @"list.sul");
                userAppList = Shared.DeserializeCollection<SUA>(Shared.appStore + "SUApps.sul");

                for (int x = 0; x < officalAppList.Count; x++)
                {
                    if (!Directory.Exists(Shared.ConvertPath(officalAppList[x].Directory, true, officalAppList[x].Is64Bit)))
                    {
                        officalAppList.RemoveAt(x);
                        x--;
                    }
                }

                for (int x = 0; x < userAppList.Count; x++)
                {
                    if (!Directory.Exists(Shared.ConvertPath(userAppList[x].Directory, true, userAppList[x].Is64Bit)))
                    {
                        userAppList.RemoveAt(x);
                        x--;
                    }
                    for (int y = 0; y < officalAppList.Count; y++)
                    {

                        if (officalAppList[y].Directory == userAppList[x].Directory && officalAppList[y].Source == userAppList[x].Source)
                        {
                            userAppList[y].Enabled = true;
                            break;
                        }
                        else
                        {

                            if (Directory.Exists(Shared.ConvertPath(userAppList[x].Directory, true, userAppList[x].Is64Bit)))
                            {
                                userAppList[x].Enabled = false;
                            }
                        }
                    }
                }
                listView.ItemsSource = userAppList;
            }
        }

        /// <summary>
        /// Saves the Settings
        /// </summary>
        void SaveSettings()
        {
            Config options = new Config();

            if (cbAutoUpdateMethod.SelectedIndex == 0)
                options.AutoOption = AutoUpdateOption.Install;

            if (cbAutoUpdateMethod.SelectedIndex == 1)
                options.AutoOption = AutoUpdateOption.Download;

            if (cbAutoUpdateMethod.SelectedIndex == 2)
                options.AutoOption = AutoUpdateOption.Notify;

            if (cbAutoUpdateMethod.SelectedIndex == 3)
                options.AutoOption = AutoUpdateOption.Never;

            options.IncludeRecommended = ((bool)chkRecommendedUpdates.IsChecked);


            if (cbAutoUpdateMethod.SelectedIndex == 3)
            {
                options.AutoOption = AutoUpdateOption.Never;

                Client.SaveSettings(false, options, userAppList);
            }
            else
                Client.SaveSettings(true, options, userAppList);


            if (SettingsSavedEventHandler != null)
                SettingsSavedEventHandler(this, new SettingsSavedEventArgs(options));
        }

        /// <summary>
        /// Wraps text
        /// </summary>
        /// <param name="text">The string to wrap</param>
        /// <param name="maxLength">Maximum length before wrap</param>
        /// <returns>Returns wrapped string</returns>
        static string Wrap(string text, int maxLength)
        {
            string sWrappedText = "";

            string sLine = "";

            while (text.Length > 0)
            {
                if (text.Length >= maxLength)
                {
                    sLine = text.Substring(0, maxLength);

                    if (sLine.EndsWith(" ") == false)
                        sLine = sLine.Substring(0, 1 + sLine.LastIndexOf(" "));

                    sWrappedText += sLine + Environment.NewLine;
                }
                else
                {
                    sLine = text;

                    sWrappedText += sLine;
                }

                text = text.Substring(sLine.Length);
            }
            return sWrappedText;
        }

        #endregion

        #region SUL list Methods

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Settings.Default.lastListUpdate = DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();

            Settings.Default.Save();

            tbLastUpdated.Text = App.RM.GetString("LastUpdated") + " " + Settings.Default.lastListUpdate;

            LoadSUL();

            listView.Cursor = Cursors.Arrow;
        }

        #endregion

        #region UI Events

        #region TextBlocks

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = TextDecorations.Underline;

        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = null;
        }

        private void tbRefresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DownloadSUL();
        }

        #endregion

        #region Buttons

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        #endregion

        private void cbAutoUpdateMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbAutoUpdateMethod.SelectedIndex)
            {
                case 0: imgShield.Source = App.greenShield; break;
                case 1: imgShield.Source = App.greenShield; break;
                case 2: imgShield.Source = null; break;
                case 3: imgShield.Source = App.redShield; break;


            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            tbLastUpdated.Text = App.RM.GetString("LastUpdated") + " " + Settings.Default.lastListUpdate;
            LoadSUL();

        }

        #region ListView Events

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.Thumb_DragDelta(sender, ((Thumb)e.OriginalSource));
        }

        #endregion

        #endregion
    }
}