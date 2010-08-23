#region GNU Public License Version 3

// Copyright 2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Provides a WPF button that displays a UAC Shield icon when required
    /// </summary>
    public class UacButton : Button, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        ///   The disabled shield image
        /// </summary>
        private static readonly BitmapImage ShieldDisabled = new BitmapImage(new Uri("pack://application:,,,/Windows.Shell;component/Images/ShieldDisabled.png", UriKind.Absolute));

        /// <summary>
        ///   The UAC shield
        /// </summary>
        private static readonly BitmapImage Shield = new BitmapImage(new Uri("pack://application:,,,/Windows.Shell;component/Images/Shield.png", UriKind.Absolute));


        /// <summary>
        ///   Dependency Property - Indicates if the UAC Shield is desired on the button
        /// </summary>
        private static readonly DependencyProperty IsShieldNeededProperty = DependencyProperty.Register("IsShieldNeeded", typeof(bool), typeof(UacButton),
                                                                                                        new FrameworkPropertyMetadata(true,
                                                                                                                                      FrameworkPropertyMetadataOptions.Inherits |
                                                                                                                                      FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                                                                      OnIsShieldNeededChanged));


        /// <summary>
        ///   Dependency Property - Specifies the text to dispay on the button
        /// </summary>
        private static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(UacButton),
                                                                                                        new FrameworkPropertyMetadata(null,
                                                                                                                                      FrameworkPropertyMetadataOptions.Inherits |
                                                                                                                                      FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                                                                      OnButtonTextChanged));
        /// <summary>
        ///   Dependency Property - The shield icon to display
        /// </summary>
        private static readonly DependencyProperty ShieldIconProperty;

        private static bool isShieldNeeded;
        private static string buttonText = "";

        #endregion

        #region Methods - INotifyPropertyChanged

        #region Properties

        /// <summary>
        ///   Gets or sets if the caller desires the <see cref = "ShieldIcon" /> to be displayed.  This is a dependency property.
        /// </summary>
        /// <value>A bool that indicates if the <see cref = "ShieldIcon" /> caller wants the <see cref = "ShieldIcon" />  displayed</value>
        /// <remarks>
        ///   This is only an indication of desire.  If the operating system does not support UAC or the user is already
        ///   elevated, any request to display is ignored.
        /// </remarks>
        public bool IsShieldNeeded { get { return (bool)GetValue(IsShieldNeededProperty); } set { SetValue(IsShieldNeededProperty, value); } }

        /// <summary>
        ///   Gets or sets the icon so show when elevation is required.  This is a dependency property.
        /// </summary>
        /// <value>An ImageSoruce that represents a graphic to be displayed </value>
        public ImageSource ShieldIcon { get { return (ImageSource)GetValue(ShieldIconProperty); } set { SetValue(ShieldIconProperty, value); } }

        /// <summary>
        ///   Gets or sets the text to display on the button
        /// </summary>
        public string ButtonText { get { return ((string)GetValue(ButtonTextProperty)); } set { SetValue(ButtonTextProperty, value); } }

        /// <summary>
        ///   Gets or sets a value indicating if the shield is desired and the OS supports elevation
        /// </summary>
        public bool IsShieldDisplayed { get { return isShieldNeeded && IsShieldNeeded; } }

        /// <summary>
        ///   Gets or sets ToolTip shown when elevation has been preformed
        /// </summary>
        /// <value>A string that is used as the ToolTip when elevation is complete</value>
        public object ToolTipElevated { get; set; }

        /// <summary>
        ///   Gets or sets ToolTip shown when elevation has not been preformed
        /// </summary>
        /// <value>A string that is used as the ToolTip when elevation is required</value>
        public object ToolTipNotElevated { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Creates the default shield icon and registers the dependency property with it
        /// </summary>
        static UacButton()
        {
            ShieldIconProperty = DependencyProperty.Register("ShieldIcon", typeof(ImageSource), typeof(Button),
                                                             new FrameworkPropertyMetadata(Shield, FrameworkPropertyMetadataOptions.AffectsRender, OnShieldIconChanged));

            ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(Button),
                                                 new FrameworkPropertyMetadata(buttonText, FrameworkPropertyMetadataOptions.AffectsRender, OnButtonTextChanged));
            /*
             *			indicates if the current OS/application/user needs a UAC shield displayed.
             *			If the OS is not UAC enabled or the users is already elevated then no shield is required.
             *			If IsShieldDisplayed is false (no shield required) the most of this class becomes a no-op.
             *			
             *			The use of the call to IsUserAnAdmin is intential to detect elevation on Vista and above, not
             *			membership in the Administrator group.
             * */
            if (Environment.OSVersion.Version.Major >= 6) // Vista or higher
                isShieldNeeded = !Internal.CoreNativeMethods.IsUserAnAdmin(); // If already an admin don't bother

        }

        /// <summary>
        ///   Initializes an instance of the <see cref = "UacButton" /> class
        /// </summary>
        public UacButton()
        {
            Loaded += OnLoaded;
            IsEnabledChanged += UacButton_IsEnabledChanged;
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            var imgShield = new System.Windows.Controls.Image { Source = IsEnabled ? Shield : ShieldDisabled, Stretch = Stretch.None, Margin = new Thickness(0, 0, 5, 0) };
            stackPanel.Children.Add(imgShield);

            var textBlock = new TextBlock { Text = buttonText };
            stackPanel.Children.Add(textBlock);
            Content = stackPanel;

        }

        #endregion

        #region Methods

        void UacButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsShieldDisplayed)
                if ((bool)e.NewValue)
                    ShieldIcon = Shield;
                else
                    ShieldIcon = ShieldDisabled;
        }

        /// <summary>
        ///   Returns current "actual" ToolTip
        /// </summary>
        /// <returns>
        ///   If both <see cref = "ToolTipElevated" /> and <see cref = "ToolTipNotElevated" /> are null,
        ///   <see cref = "Button.ToolTip" /> is returned.
        ///   Otherwise <see cref = "ToolTipElevated" /> or <see cref = "ToolTipNotElevated" /> is rturned
        ///   based on <see cref = "IsShieldNeeded" />
        /// </returns>
        /// <remarks>
        /// </remarks>
        private object GetToolTip()
        {
            if (ToolTipElevated == null && ToolTipNotElevated == null)
                return ToolTip;
            return IsShieldNeeded ? ToolTipNotElevated : ToolTipElevated;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ToolTip = GetToolTip();
        }

        #endregion

        #region Methods - Dependency Property Callbacks

        /// <summary>
        ///   Handles a change to the <see cref = "ShieldIcon" /> property
        /// </summary>
        /// <param name = "obj"></param>
        /// <param name = "e"></param>
        private static void OnShieldIconChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (UacButton)obj;
            var sp = ((StackPanel)me.Content);
            var imageShield = ((System.Windows.Controls.Image)sp.Children[0]);
            if (imageShield != null)
            {
                imageShield.Source = (ImageSource)e.NewValue;
            }
            me.ToolTip = me.GetToolTip(); ;
            me.OnPropertyChanged("ShieldIcon");
        }

        /// <summary>
        ///   Handles a change to the <see cref = "ButtonText" /> property
        /// </summary>
        /// <param name = "obj"></param>
        /// <param name = "e"></param>
        private static void OnButtonTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

            var me = (UacButton)obj;
            var stackPanel = ((StackPanel)me.Content);
            if (stackPanel != null)
            {
                var textBlock = ((TextBlock)stackPanel.Children[1]);
                if (textBlock != null)
                    textBlock.Text = e.NewValue.ToString();
            }
            me.ToolTip = me.GetToolTip();
            me.OnPropertyChanged("ButtonText");
        }

        /// <summary>
        ///   Handles a change to the <see cref = "IsShieldNeeded" /> property
        /// </summary>
        /// <param name = "obj"></param>
        /// <param name = "e"></param>
        /// <remarks>
        ///   Adds or removes the UACShieldAdorner as appropriate
        ///   <para>
        ///     Sets the <see cref = "Button.ToolTip" /> as appropriate
        ///   </para>
        /// </remarks>
        private static void OnIsShieldNeededChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var showShield = (bool)e.NewValue && isShieldNeeded;
            var me = (UacButton)obj;
            var sp = ((StackPanel)me.Content);
            var imageShield = ((System.Windows.Controls.Image)sp.Children[0]);
            if (imageShield != null)
            {
                imageShield.Visibility = showShield ? Visibility.Visible : Visibility.Collapsed;
            }
            me.ToolTip = me.GetToolTip();
            me.OnPropertyChanged("IsShieldNeeded");
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

        #endregion
}