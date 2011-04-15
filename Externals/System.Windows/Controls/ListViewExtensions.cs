// ***********************************************************************
// <copyright file="ListViewExtensions.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
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
    /// <summary>Contains methods that extend the <see cref="ListView" /> control</summary>
    public static class ListViewExtensions
    {
        #region Public Methods

        /// <summary>Limits resizing of a <see cref="GridViewColumn" /></summary>
        /// <param name="control">The Thumb object of the <see cref="GridViewColumn" /> to limit it's size</param>
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

        #endregion
    }
}