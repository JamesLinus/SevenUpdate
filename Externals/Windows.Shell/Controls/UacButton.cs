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
    #region Class UACShieldButton

    /// <summary>
    ///   Provides a WPF button that displays a UAC Shield icon when required
    /// </summary>
    public class UacButton : Button, INotifyPropertyChanged
    {
        #region UACShieldButton - Fields

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
        private static readonly DependencyProperty IsShieldNeededProperty = DependencyProperty.Register("IsShieldNeeded", typeof (bool), typeof (UacButton),
                                                                                                        new FrameworkPropertyMetadata(true,
                                                                                                                                      FrameworkPropertyMetadataOptions.Inherits |
                                                                                                                                      FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                                                                      OnIsShieldNeededChanged));
        /// <summary>
        ///   Dependency Property - The shield icon to display
        /// </summary>
        private static readonly DependencyProperty ShieldIconProperty;

        private static bool isShieldNeeded;
        private UacShieldAdorner adorner;
        private bool isShieldVisible;

        #endregion

        #region UACShieldButton - Methods - INotifyPropetyChanged

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region UACShieldButton - Properties

        /// <summary>
        ///   Gets/Sets if the caller desires the <see cref = "ShieldIcon" /> to be displayed.  This is a dependency property.
        /// </summary>
        /// <value>A bool that indicates if the <see cref = "ShieldIcon" /> caller wants the <see cref = "ShieldIcon" />  displayed</value>
        /// <remarks>
        ///   This is only an indication of desire.  If the operating system does not support UAC or the user is already
        ///   elevated, any request to display is ignored.
        /// </remarks>
        public bool IsShieldNeeded { get { return (bool) GetValue(IsShieldNeededProperty); } set { SetValue(IsShieldNeededProperty, value); } }

        /// <summary>
        ///   Gets/Sets the icon so show when elevation is required.  This is a dependency property.
        /// </summary>
        /// <value>An ImageSoruce that represents a graphic to be displayed </value>
        public ImageSource ShieldIcon { get { return (ImageSource) GetValue(ShieldIconProperty); } set { SetValue(ShieldIconProperty, value); } }

        /// <summary>
        ///   Indicates thst the shield is desired and the OS supports elevation
        /// </summary>
        public bool IsShieldDisplayed { get { return isShieldNeeded && IsShieldNeeded; } }

        /// <summary>
        ///   Gets/Sets ToolTip shown when elevation has been preformed
        /// </summary>
        /// <value>A string that is used as the ToolTip when elevation is complete</value>
        public object ToolTipElevated { get; set; }

        /// <summary>
        ///   Gets/Sets ToolTip shown when elevation has not been preformed
        /// </summary>
        /// <value>A string that is used as the ToolTip when elevation is required</value>
        public object ToolTipNotElevated { get; set; }

        #endregion

        #region UACShieldButton - Constructors

        /// <summary>
        ///   Creates the default shield icon and registers the dependency property with it
        /// </summary>
        static UacButton()
        {
            ShieldIconProperty = DependencyProperty.Register("ShieldIcon", typeof (ImageSource), typeof (Button),
                                                             new FrameworkPropertyMetadata(Shield, FrameworkPropertyMetadataOptions.AffectsRender, OnShieldIconChanged));
/*
 *			uacsb_needs_shield_display indicates if the current OS/application/user needs a UAC shield displayed.
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
            
        }

        void UacButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsShieldDisplayed)
                if ((bool)e.NewValue)
                    ShieldIcon = Shield;
                else
                    ShieldIcon = ShieldDisabled;
        }

        #endregion

        #region UACShieldButton - Methods

        /// <summary>
        ///   Creates/Gets current UACShieldAdorner based on the <see cref = "ShieldIcon" />
        /// </summary>
        /// <remarks>
        ///   If there is no current, a new UACShieldAdorner is created wit the current <see cref = "ShieldIcon" />
        ///   and made current.  The current UACShieldAdorner is returned.
        /// </remarks>
        private UacShieldAdorner GetUacShieldAdorner()
        {
            //Content = "     " + Content;
            return adorner ?? (adorner = new UacShieldAdorner(this, ShieldIcon));
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
            if (IsShieldDisplayed)
                ShieldAdormentAdd();
            ToolTip = GetToolTip();
        }

        /// <summary>
        ///   Add the shield image as an adorner
        /// </summary>
        private void ShieldAdormentAdd()
        {
            if (!isShieldVisible)
            {
                try
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (adornerLayer != null)
                        adornerLayer.Add(GetUacShieldAdorner());
                }
                catch
                {
                }
                isShieldVisible = true;
                IsEnabledChanged += UacButton_IsEnabledChanged;
                ShieldIcon = IsEnabled ? Shield : ShieldDisabled;

            }
        }

        /// <summary>
        ///   Removes the shield image adorner
        /// </summary>
        private void ShieldAdormentRemove()
        {
            if (isShieldVisible)
            {
                try
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (adornerLayer != null)
                        adornerLayer.Remove(GetUacShieldAdorner());
                }
                catch
                {
                }
                isShieldVisible = false;
                IsEnabledChanged -= UacButton_IsEnabledChanged;
            }
        }

        #endregion

        #region UACShieldButton - Methods - Dependency Property Callbacks

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
            bool showShield = (bool) e.NewValue && isShieldNeeded;
            var me = (UacButton) obj;

            if (showShield)
                me.ShieldAdormentAdd();
            else
                me.ShieldAdormentRemove();
            me.ToolTip = me.GetToolTip();
            me.FirePropertyChangedEvent("IsShieldNeeded");
            me.MinWidth = me.ActualWidth + 16;
        }

        /// <summary>
        ///   Handles a change to the <see cref = "ShieldIcon" /> property
        /// </summary>
        /// <param name = "obj"></param>
        /// <param name = "e"></param>
        /// <remarks>
        ///   Removes any exising adorner
        ///   <para>
        ///     Clears the adorner to force a new one to be created when needed.
        ///   </para>
        ///   <para>
        ///     If <see cref = "IsShieldNeeded" /> is ture, it adds the new UACShieldAdorner
        ///   </para>
        /// </remarks>
        private static void OnShieldIconChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (UacButton) obj;

/*
 *			Remove any existing adorner with the old image
 * */
            me.ShieldAdormentRemove();
/*
 *			Force a new UACShieldAdorner to be created on next use
 * */
            me.adorner = null;
            if (me.IsShieldDisplayed)
                me.ShieldAdormentAdd();
            me.FirePropertyChangedEvent("ShieldIcon");
        }

        #endregion

        private void FirePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #endregion

    #region Class UACShieldAdorner

    /// <summary>
    ///   Provides an adorner to display the provided shield icon
    /// </summary>
    public class UacShieldAdorner : Adorner
    {

        #region UACShieldAdorner - Fields

        private FrameworkElement adornedElement;
        private ImageSource imageSource;

        #endregion

        #region UACShieldAdorner - Constructors

        /// <summary>
        ///   Creates an instance of the <see cref = "UacShieldAdorner" /> class
        /// </summary>
        /// <param name = "adornedElement">The <see cref = "FrameworkElement" />to adorn.
        ///   Typically this will be an instance of the <see cref = "UacButton" />class</param>
        /// <param name = "shieldImage"></param>
        public UacShieldAdorner(FrameworkElement adornedElement, ImageSource shieldImage) : base(adornedElement)
        {
            this.adornedElement = adornedElement;
            imageSource = shieldImage;
        }

        #endregion

        #region UACShieldAdorner - Methods

        /// <summary>
        ///   Overrides <see cref = "Adorner.OnRender" />to draw the shield image
        ///   at a given size and location.
        /// </summary>
        /// <param name = "drawingContext"></param>
        /// <remarks>
        ///   The size of the rectangle to use to display the image is determined here
        ///   instead of once in the constructor so that it will automaticaly
        ///   resize as its hosting element resizes.
        /// </remarks>
        protected override void OnRender(DrawingContext drawingContext)
        {
            double maxHeight = adornedElement.ActualHeight - 4;

            base.OnRender(drawingContext);
            if (adornedElement.Visibility == Visibility.Visible && maxHeight > 0)
            {
                double imageHeight = Math.Min(maxHeight, imageSource.Height);
                double yOffset = (adornedElement.ActualHeight - imageHeight)/2.0;
                var rect = new Rect(5, yOffset, (imageSource.Width*imageHeight)/imageSource.Height, imageHeight);
                drawingContext.DrawImage(imageSource, rect);
            }
        }

        #endregion
    }

    #endregion
}