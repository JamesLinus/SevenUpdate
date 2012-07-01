// <copyright file="ProgressRing.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SevenSoftware.Windows.Controls
{
    /// <summary>Displays a progress circle.</summary>
    [TemplatePart(Name = ElementCanvas, Type = typeof(Canvas))]
    public class ProgressRing : RangeBase
    {
        /// <summary>The element name.</summary>
        const string ElementCanvas = "PART_Canvas";

        /// <summary>The storyboard.</summary>
        static readonly DependencyProperty elementStoryboardProperty = DependencyProperty.Register(
            "ElementStoryboard", typeof(Storyboard), typeof(ProgressRing));

        /// <summary>The text to display when the progress is indeterminate.</summary>
        static readonly DependencyProperty indeterminateTextProperty = DependencyProperty.Register(
            "IndeterminateText", typeof(string), typeof(ProgressRing));

        /// <summary>Indicates if the progress is indeterminate.</summary>
        static readonly DependencyProperty isIndeterminateProperty = DependencyProperty.Register(
            "IsIndeterminate", typeof(bool), typeof(ProgressRing));

        /// <summary>Indicates if the progress is running.</summary>
        static readonly DependencyProperty isRunningProperty = DependencyProperty.Register(
            "IsRunning", typeof(bool), typeof(ProgressRing), new FrameworkPropertyMetadata(IsRunningPropertyChanged));

        /// <summary>The dispatch timer.</summary>
        readonly DispatcherTimer dispatcherTimer;

        /// <summary>The canvas.</summary>
        Canvas canvas;

        /// <summary>The canvas elements.</summary>
        Array canvasElements;

        /// <summary>Indicates if the progress runs clockwise.</summary>
        bool clockwise;

        /// <summary>The index for the progress.</summary>
        int index;

        /// <summary>Initializes static members of the <see cref="ProgressRing" /> class.</summary>
        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
            MaximumProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(100.0));
        }

        /// <summary>Initializes a new instance of the <see cref="ProgressRing" /> class.</summary>
        public ProgressRing()
        {
            // if (this.Resources.Count != 0)
            // {
            // return;
            // }

            // var resourceDictionary = new ResourceDictionary
            // { Source = new Uri("/SevenSoftware.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) };

            // this.Resources.MergedDictionaries.Add(resourceDictionary);
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
                {
                   Interval = new TimeSpan(0, 0, 0, 0, 300) 
                };
        }

        /// <summary>Gets or sets the element storyboard.</summary>
        /// <value>The element storyboard.</value>
        public Storyboard ElementStoryboard
        {
            get { return (Storyboard)GetValue(elementStoryboardProperty); }

            set { SetValue(elementStoryboardProperty, value); }
        }

        /// <summary>Gets or sets the indeterminate text.</summary>
        /// <value>The indeterminate text.</value>
        public string IndeterminateText
        {
            get { return (string)GetValue(indeterminateTextProperty); }

            set { SetValue(indeterminateTextProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this instance is indeterminate.</summary>
        /// <value><c>True</c> if this instance is indeterminate; otherwise, <c>False</c>.</value>
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(isIndeterminateProperty); }

            set { SetValue(isIndeterminateProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this instance is running.</summary>
        /// <value><c>True</c> if this instance is running; otherwise, <c>False</c>.</value>
        public bool IsRunning
        {
            get { return (bool)GetValue(isRunningProperty); }

            set { SetValue(isRunningProperty, value); }
        }

        /// <summary>
        ///   When overridden in a derived class, is invoked whenever application code or internal processes call <see
        ///   cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            canvas = GetTemplateChild(ElementCanvas) as Canvas;
            if (canvas == null)
            {
                return;
            }

            // Get the center of the canvas. This will be the base of the rotation.
            double centerX = canvas.Width / 2;
            double centerY = canvas.Height / 2;

            // Get the no. of degrees between each circles.
            double interval = 360.0 / canvas.Children.Count;
            double angle = -135;

            canvasElements = new UIElement[canvas.Children.Count];
            canvas.Children.CopyTo(canvasElements, 0);
            canvas.Children.Clear();

            foreach (UIElement element in canvasElements)
            {
                var contentControl = new ContentControl { Content = element };

                var rotateTransform = new RotateTransform(angle, centerX, centerY);
                contentControl.RenderTransform = rotateTransform;
                angle += interval;

                canvas.Children.Add(contentControl);
            }
        }

        /// <summary>Stops or starts the progress indicator based on the <c>IsRunning</c> property.</summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <c>System.Windows.DependencyPropertyChangedEventArgs</c> instance containing the event data.</param>
        static void IsRunningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressRing = (ProgressRing)d;

            if ((bool)e.NewValue)
            {
                progressRing.Start();
            }
            else
            {
                progressRing.Stop();
            }
        }

        /// <summary>Animates the progress wheel.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.EventArgs</c> instance containing the event data.</param>
        void Animate(object sender, EventArgs e)
        {
            if (canvasElements == null || ElementStoryboard == null)
            {
                return;
            }

            int trueIndex = clockwise ? index : canvasElements.Length - index - 1;

            var element = canvasElements.GetValue(trueIndex) as FrameworkElement;
            StartStoryboard(element);

            clockwise = index == canvasElements.Length - 1 ? !clockwise : clockwise;
            index = (index + 1) % canvasElements.Length;
        }

        /// <summary>Starts this instance.</summary>
        void Start()
        {
            dispatcherTimer.Tick -= Animate;
            dispatcherTimer.Tick += Animate;
            dispatcherTimer.Start();
        }

        /// <summary>Starts the storyboard.</summary>
        /// <param name="element">The element.</param>
        void StartStoryboard(FrameworkElement element)
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

        /// <summary>Stops this instance.</summary>
        void Stop()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= Animate;
        }
    }
}