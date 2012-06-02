// <copyright file="ProgressRing.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows.Controls
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    /// <summary>Displays a progress circle.</summary>
    [TemplatePart(Name = ElementCanvas, Type = typeof(Canvas))]
    public class ProgressRing : RangeBase
    {
        /// <summary>The element name.</summary>
        private const string ElementCanvas = "PART_Canvas";

        /// <summary>The storyboard.</summary>
        private static readonly DependencyProperty elementStoryboardProperty =
            DependencyProperty.Register("ElementStoryboard", typeof(Storyboard), typeof(ProgressRing));

        /// <summary>The text to display when the progress is indeterminate.</summary>
        private static readonly DependencyProperty indeterminateTextProperty =
            DependencyProperty.Register("IndeterminateText", typeof(string), typeof(ProgressRing));

        /// <summary>Indicates if the progress is indeterminate.</summary>
        private static readonly DependencyProperty isIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProgressRing));

        /// <summary>Indicates if the progress is running.</summary>
        private static readonly DependencyProperty isRunningProperty = DependencyProperty.Register(
            "IsRunning", typeof(bool), typeof(ProgressRing), new FrameworkPropertyMetadata(IsRunningPropertyChanged));

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
            this.dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background, this.Dispatcher)
                {
                   Interval = new TimeSpan(0, 0, 0, 0, 300) 
                };
        }

        /// <summary>Gets or sets the element storyboard.</summary>
        /// <value>The element storyboard.</value>
        public Storyboard ElementStoryboard
        {
            get { return (Storyboard)this.GetValue(elementStoryboardProperty); }

            set { this.SetValue(elementStoryboardProperty, value); }
        }

        /// <summary>Gets or sets the indeterminate text.</summary>
        /// <value>The indeterminate text.</value>
        public string IndeterminateText
        {
            get { return (string)this.GetValue(indeterminateTextProperty); }

            set { this.SetValue(indeterminateTextProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this instance is indeterminate.</summary>
        /// <value><c>True</c> if this instance is indeterminate; otherwise, <c>False</c>.</value>
        public bool IsIndeterminate
        {
            get { return (bool)this.GetValue(isIndeterminateProperty); }

            set { this.SetValue(isIndeterminateProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this instance is running.</summary>
        /// <value><c>True</c> if this instance is running; otherwise, <c>False</c>.</value>
        public bool IsRunning
        {
            get { return (bool)this.GetValue(isRunningProperty); }

            set { this.SetValue(isRunningProperty, value); }
        }

        /// <summary>
        ///   When overridden in a derived class, is invoked whenever application code or internal processes call <see
        ///   cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.canvas = this.GetTemplateChild(ElementCanvas) as Canvas;
            if (this.canvas == null)
            {
                return;
            }

            // Get the center of the canvas. This will be the base of the rotation.
            double centerX = this.canvas.Width / 2;
            double centerY = this.canvas.Height / 2;

            // Get the no. of degrees between each circles.
            double interval = 360.0 / this.canvas.Children.Count;
            double angle = -135;

            this.canvasElements = new UIElement[this.canvas.Children.Count];
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

        /// <summary>Stops or starts the progress indicator based on the <c>IsRunning</c> property.</summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <c>System.Windows.DependencyPropertyChangedEventArgs</c> instance containing the event data.</param>
        private static void IsRunningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        private void Animate(object sender, EventArgs e)
        {
            if (this.canvasElements == null || this.ElementStoryboard == null)
            {
                return;
            }

            int trueIndex = this.clockwise ? this.index : this.canvasElements.Length - this.index - 1;

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
    }
}