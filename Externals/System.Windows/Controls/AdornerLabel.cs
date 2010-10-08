// ***********************************************************************
// <copyright file="AdornerLabel.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// The label to display on the <see cref="InfoTextBox"/>
    /// </summary>
    public class AdornerLabel : Adorner
    {
        #region Constants and Fields

        /// <summary>
        ///   The textBlock
        /// </summary>
        private readonly TextBlock textBlock;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornerLabel"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="labelStyle">
        /// The label style.
        /// </param>
        public AdornerLabel(UIElement adornedElement, string label, Style labelStyle)
            : base(adornedElement)
        {
            this.textBlock = new TextBlock { Style = labelStyle, Text = label };
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.textBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the requested child element in the collection.
        /// </param>
        /// <returns>
        /// The requested child element. This should not return <see langword="null"/>; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.textBlock;
        }

        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">
        /// A size to constrain the adorner to.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.textBlock.Measure(constraint);
            return constraint;
        }

        #endregion
    }
}