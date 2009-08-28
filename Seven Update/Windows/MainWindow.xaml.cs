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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using SevenUpdate.Properties;

namespace SevenUpdate.Windows
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    [ContentPropertyAttribute]
    [TemplatePartAttribute(Name = "PART_NavWinCP", Type = typeof(ContentPresenter))]
    public partial class MainWindow : NavigationWindow
    {
        
        public MainWindow()
        {
            InitializeComponent();

            ns = this.NavigationService;

            /// Make the NotifyIcon in the resources global and accessbile via the App Class
            App.NotifyIcon = (Avalon.Windows.Controls.NotifyIcon)FindResource("NotifyIcon");
 
            /// Set the notifyicon to the localized program name
            App.NotifyIcon.Text = App.RM.GetString("SevenUpdate");
        }

        internal static NavigationService ns;

        internal static bool IsHidden { get; set; }

        private void NavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Height = Settings.Default.windowHeight;
            Width = Settings.Default.windowWidth;
        }

        private void NavigationWindow_StateChanged(object sender, System.EventArgs e)
        {
            if (App.InstallInProgress)
            {

                if (this.WindowState == WindowState.Minimized)
                {
                    ShowInTaskbar = false;
                }
                if (WindowState == WindowState.Normal)
                {
                    ShowInTaskbar = true;
                }
            }

        }

        private void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.windowHeight = Height;
            Settings.Default.windowWidth = Width;
            Settings.Default.Save();

            if (App.InstallInProgress )
            {
                e.Cancel = true;
                ShowInTaskbar = false;
                Hide();
                App.NotifyIcon.Visibility = Visibility.Visible;
            }
        }

        #region Notification Icon

        void NotifyIcon_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            IsHidden = false;
            App.NotifyIcon.Visibility = Visibility.Hidden;
        }

        void NotifyIcon_BalloonTipClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.infoPopUp = false;
            Settings.Default.Save();
            this.Show();
            this.ShowInTaskbar = true;
            IsHidden = false;
            App.NotifyIcon.Visibility = Visibility.Hidden; ;
            if (App.NotifyIcon.Text.Contains(App.RM.GetString("DownloadAndInstallUpdates")))
            {
                ns.Navigate(new Uri(@"Pages\Update Info.xaml", UriKind.Relative));
            }
        }

        #endregion
    }
}
