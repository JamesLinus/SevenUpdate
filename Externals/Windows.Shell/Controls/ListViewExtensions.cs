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