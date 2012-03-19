// <copyright file="AdornerLabel.cs" project="SevenSoftware.Windows">Ben Dewey</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License" />

namespace SevenSoftware.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>The label to display on the <c>InfoTextBox</c>.</summary>
    public class AdornerLabel : Adorner
    {
        /// <summary>The textBlock.</summary>
        private readonly TextBlock textBlock;

        /// <summary>Initializes a new instance of the <see cref="AdornerLabel" /> class.</summary>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="label">The label.</param>
        /// <param name="labelStyle">The label style.</param>
        public AdornerLabel(UIElement adornedElement, string label, Style labelStyle) : base(adornedElement)
        {
            this.textBlock = new TextBlock { Style = labelStyle, Text = label };
        }

        /// <summary>Gets the number of visual child elements within this element.</summary>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        ///   When overridden in a derived class, positions child elements and determines a size for a <see
        ///   cref="T:System.Windows.FrameworkElement" /> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.textBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        ///   Overrides <c>M:System.Windows.Media.Visual.GetVisualChild(System.Int32)</c>, and returns a child at the
        ///   specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>The requested child element. This should not return <c>null</c>; if the provided index is out of range, an exception is thrown.</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.textBlock;
        }

        /// <summary>Implements any custom measuring behavior for the adorner.</summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>A <c>T:System.Windows.Size</c> object representing the amount of layout space needed by the adorner.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.textBlock.Measure(constraint);
            return constraint;
        }
    }
}