// ***********************************************************************
// <copyright file="ProgressIndicator.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author>Michael Detras</author>
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
    using System.Linq;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    /// <summary>Displays a progress circle.</summary>
    [TemplatePart(Name = ElementCanvas, Type = typeof(Canvas))]
    public sealed class ProgressIndicator : RangeBase
    {
        #region Constants and Fields

        /// <summary>The storyboard.</summary>
        public static readonly DependencyProperty ElementStoryboardProperty = DependencyProperty.Register(
            "ElementStoryboard", typeof(Storyboard), typeof(ProgressIndicator));

        /// <summary>The text to display when the progress is indeterminate.</summary>
        public static readonly DependencyProperty IndeterminateTextProperty = DependencyProperty.Register(
            "IndeterminateText", typeof(string), typeof(ProgressIndicator));

        /// <summary>Indicates if the progress is indeterminate.</summary>
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
            "IsIndeterminate", typeof(bool), typeof(ProgressIndicator));

        /// <summary>Indicates if the progress is running.</summary>
        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
            "IsRunning", typeof(bool), typeof(ProgressIndicator), new FrameworkPropertyMetadata(IsRunningPropertyChanged));

        /// <summary>The element name.</summary>
        private const string ElementCanvas = "PART_Canvas";

        /// <summary>The dispatch timer.</summary>
        private readonly DispatcherTimer dispatcherTimer;

        /// <summary>The canvas.</summary>
        private Canvas canvas;

        /// <summary>The canvas elements.</summary>
        private Array canvasElements;

        /// <summary>Indicates if the progress runs clockwise.</summary>
        private bool clockwise;

        /// <summary>The index for the progress.</summary>
        private int index;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes static members of the <see cref="ProgressIndicator" /> class.</summary>
        static ProgressIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressIndicator), new FrameworkPropertyMetadata(typeof(ProgressIndicator)));
            MaximumProperty.OverrideMetadata(typeof(ProgressIndicator), new FrameworkPropertyMetadata(100.0));
        }

        /// <summary>Initializes a new instance of the <see cref="ProgressIndicator" /> class.</summary>
        public ProgressIndicator()
        {
            if (this.Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary
                { Source = new Uri("/System.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) };

            this.Resources.MergedDictionaries.Add(resourceDictionary);
            this.dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background, this.Dispatcher) { Interval = new TimeSpan(0, 0, 0, 0, 300) };
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the element storyboard.</summary>
        /// <value>The element storyboard.</value>
        public Storyboard ElementStoryboard
        {
            get
            {
                return (Storyboard)this.GetValue(ElementStoryboardProperty);
            }

            set
            {
                this.SetValue(ElementStoryboardProperty, value);
            }
        }

        /// <summary>Gets or sets the indeterminate text.</summary>
        /// <value>The indeterminate text.</value>
        public string IndeterminateText
        {
            get
            {
                return (string)this.GetValue(IndeterminateTextProperty);
            }

            set
            {
                this.SetValue(IndeterminateTextProperty, value);
            }
        }

        /// <summary>Gets or sets a value indicating whether this instance is indeterminate.</summary>
        /// <value><see langword="true" /> if this instance is indeterminate; otherwise, <see langword="false" />.</value>
        public bool IsIndeterminate
        {
            get
            {
                return (bool)this.GetValue(IsIndeterminateProperty);
            }

            set
            {
                this.SetValue(IsIndeterminateProperty, value);
            }
        }

        /// <summary>Gets or sets a value indicating whether this instance is running.</summary>
        /// <value><see langword="true" /> if this instance is running; otherwise, <see langword="false" />.</value>
        public bool IsRunning
        {
            get
            {
                return (bool)this.GetValue(IsRunningProperty);
            }

            set
            {
                this.SetValue(IsRunningProperty, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.canvas = this.GetTemplateChild(ElementCanvas) as Canvas;
            if (this.canvas == null)
            {
                return;
            }

            // Get the center of the canvas. This will be the base of the rotation.
            var centerX = this.canvas.Width / 2;
            var centerY = this.canvas.Height / 2;

            // Get the no. of degrees between each circles.
            var interval = 360.0 / this.canvas.Children.Count;
            double angle = -135;

            this.canvasElements = Array.CreateInstance(typeof(UIElement), this.canvas.Children.Count);
            this.canvas.Children.CopyTo(this.canvasElements, 0);
            this.canvas.Children.Clear();

            foreach (UIElement element in this.canvasElements)
            {
                var contentControl = new ContentControl { Content = element };

                var rotateTransform = new RotateTransform(angle, centerX, centerY);
                contentControl.RenderTransform = rotateTransform;
                angle += interval;

                this.canvas.Children.Add(contentControl);
            }
        }

        #endregion

        #region Methods

        /// <summary>Stops or starts the progress indicator based on the <see cref="IsRunning" /> property.</summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void IsRunningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressIndicator = (ProgressIndicator)d;

            if ((bool)e.NewValue)
            {
                progressIndicator.Start();
            }
            else
            {
                progressIndicator.Stop();
            }
        }

        /// <summary>Animates the progress wheel.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void Animate(object sender, EventArgs e)
        {
            if (this.canvasElements == null || this.ElementStoryboard == null)
            {
                return;
            }

            var trueIndex = this.clockwise ? this.index : this.canvasElements.Length - this.index - 1;

            var element = this.canvasElements.GetValue(trueIndex) as FrameworkElement;
            this.StartStoryboard(element);

            this.clockwise = this.index == this.canvasElements.Length - 1 ? !this.clockwise : this.clockwise;
            this.index = (this.index + 1) % this.canvasElements.Length;
        }

        /// <summary>Starts this instance.</summary>
        private void Start()
        {
            this.dispatcherTimer.Tick -= this.Animate;
            this.dispatcherTimer.Tick += this.Animate;
            this.dispatcherTimer.Start();
        }

        /// <summary>Starts the storyboard.</summary>
        /// <param name="element">The element.</param>
        private void StartStoryboard(FrameworkElement element)
        {
            NameScope.SetNameScope(this, new NameScope());
            element.Name = "Element";

            NameScope.SetNameScope(element, NameScope.GetNameScope(this));
            NameScope.GetNameScope(this).RegisterName(element.Name, element);

            var storyboard = new Storyboard();
            NameScope.SetNameScope(storyboard, NameScope.GetNameScope(this));

            foreach (var timelineClone in this.ElementStoryboard.Children.Select(timeline => timeline.Clone()))
            {
                storyboard.Children.Add(timelineClone);
                Storyboard.SetTargetName(timelineClone, element.Name);
            }

            storyboard.Begin(element);
        }

        /// <summary>Stops this instance.</summary>
        private void Stop()
        {
            this.dispatcherTimer.Stop();
            this.dispatcherTimer.Tick -= this.Animate;
        }

        #endregion
    }
}