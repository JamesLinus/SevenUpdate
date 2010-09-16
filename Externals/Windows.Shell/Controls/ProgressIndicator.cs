#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

#endregion

namespace Microsoft.Windows.Controls
{
    [TemplatePart(Name = ElementCanvas, Type = typeof (Canvas))]
    public sealed class ProgressIndicator : RangeBase
    {
        #region Constants

        private const string ElementCanvas = "PART_Canvas";

        #endregion

        #region Private Fields

        private readonly DispatcherTimer dispatcherTimer;
        private Canvas canvas;

        private Array canvasElements;

        private bool clockwise;
        private int index;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IndeterminateTextProperty = DependencyProperty.Register("IndeterminateText", typeof (string), typeof (ProgressIndicator));

        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof (bool), typeof (ProgressIndicator));

        public static readonly DependencyProperty ElementStoryboardProperty = DependencyProperty.Register("ElementStoryboard", typeof (Storyboard), typeof (ProgressIndicator));

        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register("IsRunning", typeof (bool), typeof (ProgressIndicator),
                                                                                                  new FrameworkPropertyMetadata(IsRunningPropertyChanged));

        public string IndeterminateText { get { return (string) GetValue(IndeterminateTextProperty); } set { SetValue(IndeterminateTextProperty, value); } }
        public bool IsIndeterminate { get { return (bool) GetValue(IsIndeterminateProperty); } set { SetValue(IsIndeterminateProperty, value); } }
        public Storyboard ElementStoryboard { get { return (Storyboard) GetValue(ElementStoryboardProperty); } set { SetValue(ElementStoryboardProperty, value); } }

        public bool IsRunning { get { return (bool) GetValue(IsRunningProperty); } set { SetValue(IsRunningProperty, value); } }

        #endregion

        #region Constructors

        static ProgressIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ProgressIndicator), new FrameworkPropertyMetadata(typeof (ProgressIndicator)));
            MaximumProperty.OverrideMetadata(typeof (ProgressIndicator), new FrameworkPropertyMetadata(100.0));
        }

        public ProgressIndicator()
        {
            if (Resources.Count != 0)
                return;
            var resourceDictionary = new ResourceDictionary {Source = new Uri("/Windows.Shell;component/Resources/Dictionary.xaml", UriKind.Relative)};

            Resources.MergedDictionaries.Add(resourceDictionary);
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher) {Interval = new TimeSpan(0, 0, 0, 0, 300)};
        }

        #endregion

        #region Private Methods

        private static void IsRunningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressIndicator = (ProgressIndicator) d;

            if ((bool) e.NewValue)
                progressIndicator.Start();
            else
                progressIndicator.Stop();
        }

        private void Start()
        {
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void Stop()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= DispatcherTimer_Tick;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (canvasElements == null || ElementStoryboard == null)
                return;
            var trueIndex = clockwise ? index : canvasElements.Length - index - 1;

            var element = canvasElements.GetValue(trueIndex) as FrameworkElement;
            StartStoryboard(element);

            clockwise = index == canvasElements.Length - 1 ? !clockwise : clockwise;
            index = (index + 1)%canvasElements.Length;
        }

        private void StartStoryboard(FrameworkElement element)
        {
            NameScope.SetNameScope(this, new NameScope());
            element.Name = "Element";

            NameScope.SetNameScope(element, NameScope.GetNameScope(this));
            NameScope.GetNameScope(this).RegisterName(element.Name, element);

            var storyboard = new Storyboard();
            NameScope.SetNameScope(storyboard, NameScope.GetNameScope(this));

            foreach (var timelineClone in ElementStoryboard.Children.Select(timeline => timeline.Clone()))
            {
                storyboard.Children.Add(timelineClone);
                Storyboard.SetTargetName(timelineClone, element.Name);
            }

            storyboard.Begin(element);
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            canvas = GetTemplateChild(ElementCanvas) as Canvas;
            if (canvas == null)
                return;
            // Get the center of the canvas. This will be the base of the rotation.
            var centerX = canvas.Width/2;
            var centerY = canvas.Height/2;

            // Get the no. of degrees between each circles.
            var interval = 360.0/canvas.Children.Count;
            double angle = -135;

            canvasElements = Array.CreateInstance(typeof (UIElement), canvas.Children.Count);
            canvas.Children.CopyTo(canvasElements, 0);
            canvas.Children.Clear();

            foreach (UIElement element in canvasElements)
            {
                var contentControl = new ContentControl {Content = element};

                var rotateTransform = new RotateTransform(angle, centerX, centerY);
                contentControl.RenderTransform = rotateTransform;
                angle += interval;

                canvas.Children.Add(contentControl);
            }
        }

        #endregion
    }
}