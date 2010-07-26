#region

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Contains methods that extend the <see cref = "ListView" /> control
    /// </summary>
    public static class ListViewExtensions
    {
        /// <summary>
        ///   Limits resizing of a <see cref = "GridViewColumn" />
        /// </summary>
        /// <param name = "e">The Thumb object of the <see cref = "GridViewColumn" /> to limit it's size</param>
        public static void LimitColumnSize(Thumb e)
        {
            var senderAsThumb = e;
            var header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (header == null)
                return;
            if (!string.IsNullOrEmpty(((string) header.Column.Header)))
            {
                if (header.Column.ActualWidth < 125)
                    header.Column.Width = 125;
            }
            else
                header.Column.Width = 25;
        }
    }
}