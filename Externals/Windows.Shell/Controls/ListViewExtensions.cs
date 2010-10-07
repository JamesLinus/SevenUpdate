// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Contains methods that extend the <see cref="ListView"/> control
    /// </summary>
    public static class ListViewExtensions
    {
        #region Public Methods

        /// <summary>
        /// Limits resizing of a <see cref="GridViewColumn"/>
        /// </summary>
        /// <parameter name="e">
        /// The Thumb object of the <see cref="GridViewColumn"/> to limit it's size
        /// </parameter>
        public static void LimitColumnSize(Thumb e)
        {
            var senderAsThumb = e;
            var header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (header == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty((string)header.Column.Header))
            {
                if (header.Column.ActualWidth < 125)
                {
                    header.Column.Width = 125;
                }
            }
            else
            {
                header.Column.Width = 25;
            }
        }

        #endregion
    }
}