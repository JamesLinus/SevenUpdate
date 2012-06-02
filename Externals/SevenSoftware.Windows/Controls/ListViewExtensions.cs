// <copyright file="ListViewExtensions.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>Contains methods that extend the <c>ListView</c> control.</summary>
    public static class ListViewExtensions
    {
        /// <summary>Limits resizing of a <c>GridViewColumn</c>.</summary>
        /// <param name="control">The Thumb object of the <c>GridViewColumn</c> to limit it's size.</param>
        public static void LimitColumnSize(FrameworkElement control)
        {
            if (control == null)
            {
                return;
            }

            var header = control.TemplatedParent as GridViewColumnHeader;
            if (header == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty((string)header.Column.Header))
            {
                if (header.Column.ActualWidth < 100)
                {
                    header.Column.Width = 100;
                }
            }
            else
            {
                header.Column.Width = 25;
            }
        }
    }
}