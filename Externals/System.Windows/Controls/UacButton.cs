// ***********************************************************************
// <copyright file="UacButton.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace System.Windows.Controls
{
    using System.ComponentModel;
    using System.Windows.Internal;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>Provides a WPF button that displays a UAC Shield icon when required.</summary>
    public sealed class UacButton : Button, INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>Dependency Property - Specifies the text to display on the button.</summary>
        private static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(UacButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender, OnButtonTextChanged));

        /// <summary>Dependency Property - Indicates if the UAC Shield is desired on the button.</summary>
        private static readonly DependencyProperty IsShieldNeededProperty = DependencyProperty.Register("IsShieldNeeded", typeof(bool), typeof(UacButton), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender, OnIsShieldNeededChanged));

        /// <summary>The UAC shield.</summary>
        private static readonly BitmapImage Shield = new BitmapImage(new Uri(@"pack://application:,,,/System.Windows;component/Images/Shield.png", UriKind.Absolute));

        /// <summary>The disabled shield image.</summary>
        private static readonly BitmapImage ShieldDisabled = new BitmapImage(new Uri(@"pack://application:,,,/System.Windows;component/Images/ShieldDisabled.png", UriKind.Absolute));

        /// <summary>Dependency Property - The shield icon to display.</summary>
        private static readonly DependencyProperty ShieldIconProperty = DependencyProperty.Register("ShieldIcon", typeof(ImageSource), typeof(Button), new FrameworkPropertyMetadata(Shield, FrameworkPropertyMetadataOptions.AffectsRender, OnShieldIconChanged));

        /// <summary>Indicates if the Uac shield is needed.</summary>
        private static readonly bool ShieldNeeded = !NativeMethods.IsUserAdmin;

        /// <summary>The text for the button.</summary>
        private static readonly string Text = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UacButton" /> class.</summary>
        public UacButton()
        {
            this.Loaded -= this.OnLoaded;
            this.Loaded += this.OnLoaded;
            this.IsEnabledChanged -= this.ChangeUacIcon;
            this.IsEnabledChanged += this.ChangeUacIcon;
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            var imgShield = new Image { Source = this.IsEnabled ? Shield : ShieldDisabled, Stretch = Stretch.None, Margin = new Thickness(0, 0, 5, 0) };
            stackPanel.Children.Add(imgShield);

            var textBlock = new TextBlock { Text = Text, VerticalAlignment = VerticalAlignment.Center };
            stackPanel.Children.Add(textBlock);
            this.Content = stackPanel;
        }

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets the text to display on the button.</summary>
        public string ButtonText
        {
            get
            {
                return (string)this.GetValue(ButtonTextProperty);
            }

            set
            {
                this.SetValue(ButtonTextProperty, value);
            }
        }

        /// <summary>Gets a value indicating whether the shield is desired and the OS supports elevation.</summary>
        public bool IsShieldDisplayed
        {
            get
            {
                return ShieldNeeded && this.IsShieldNeeded;
            }
        }

        /// <summary>Gets or sets a value indicating whether the caller desires the <see cref="ShieldIcon" /> to be displayed.  This is a dependency property.</summary>
        /// <value>A Boolean that indicates if the <see cref="ShieldIcon" /> caller wants the <see cref="ShieldIcon" />  displayed.</value>
        /// <remarks>This is only an indication of desire.  If the operating system does not support UAC or the user is already elevated, any request to display is ignored.</remarks>
        public bool IsShieldNeeded
        {
            get
            {
                return (bool)this.GetValue(IsShieldNeededProperty);
            }

            set
            {
                this.SetValue(IsShieldNeededProperty, value);
            }
        }

        /// <summary>Gets or sets the icon so show when elevation is required.  This is a dependency property.</summary>
        /// <value>An <see cref="ImageSource" /> that represents a graphic to be displayed .</value>
        public ImageSource ShieldIcon
        {
            get
            {
                return (ImageSource)this.GetValue(ShieldIconProperty);
            }

            set
            {
                this.SetValue(ShieldIconProperty, value);
            }
        }

        /// <summary>Gets or sets <see cref="ToolTip" /> shown when elevation has been preformed.</summary>
        /// <value>A string that is used as the <see cref="ToolTip" /> when elevation is complete.</value>
        public object ToolTipElevated { get; set; }

        /// <summary>Gets or sets <see cref="ToolTip" /> shown when elevation has not been preformed.</summary>
        /// <value>A string that is used as the <see cref="ToolTip" /> when elevation is required.</value>
        public object ToolTipNotElevated { get; set; }

        #endregion

        #region Methods

        /// <summary>Handles a change to the <see cref="ButtonText" /> property.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnButtonTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (UacButton)obj;
            var stackPanel = (StackPanel)me.Content;
            if (stackPanel != null)
            {
                var textBlock = (TextBlock)stackPanel.Children[1];
                if (textBlock != null)
                {
                    textBlock.Text = e.NewValue.ToString();
                }
            }

            me.ToolTip = me.GetToolTip();
            me.OnPropertyChanged("ButtonText");
        }

        /// <summary>Handles a change to the <see cref="IsShieldNeeded" /> property.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <remarks>Adds or removes the UACShieldAdorner as appropriate</remarks>
        private static void OnIsShieldNeededChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var showShield = (bool)e.NewValue && ShieldNeeded;
            var me = (UacButton)obj;
            var sp = (StackPanel)me.Content;
            var imageShield = (Image)sp.Children[0];
            if (imageShield != null)
            {
                imageShield.Visibility = showShield ? Visibility.Visible : Visibility.Collapsed;
            }

            me.ToolTip = me.GetToolTip();
            me.OnPropertyChanged("IsShieldNeeded");
        }

        /// <summary>Handles a change to the <see cref="ShieldIcon" /> property.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnShieldIconChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (UacButton)obj;
            var sp = (StackPanel)me.Content;
            var imageShield = (Image)sp.Children[0];
            if (imageShield != null)
            {
                imageShield.Source = (ImageSource)e.NewValue;
            }

            me.ToolTip = me.GetToolTip();
            me.OnPropertyChanged("ShieldIcon");
        }

        /// <summary>Changes the UAC icon.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void ChangeUacIcon(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsShieldDisplayed)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                this.ShieldIcon = Shield;
            }
            else
            {
                this.ShieldIcon = ShieldDisabled;
            }
        }

        /// <summary>Returns current "actual" <see cref="ToolTip" />.</summary>
        /// <returns>If both <see cref="ToolTipElevated" /> and <see cref="ToolTipNotElevated" /> are <see langword="null" />,<see cref="Button.ToolTip" /> is returned.Otherwise <see cref="ToolTipElevated" /> or <see cref="ToolTipNotElevated" /> is returned based on <see cref="IsShieldNeeded" />.</returns>
        /// <remarks>
        /// </remarks>
        private object GetToolTip()
        {
            if (this.ToolTipElevated == null && this.ToolTipNotElevated == null)
            {
                return this.ToolTip;
            }

            return this.IsShieldNeeded ? this.ToolTipNotElevated : this.ToolTipElevated;
        }

        /// <summary>Called when the control is loaded.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ToolTip = this.GetToolTip();
        }

        /// <summary>When a property has changed, call the <see cref="OnPropertyChanged" /> Event.</summary>
        /// <param name="name">The property name that has changed.</param>
        private void OnPropertyChanged(string name)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}