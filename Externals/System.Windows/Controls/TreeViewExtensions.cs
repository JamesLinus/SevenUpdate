// ***********************************************************************
// <copyright file="TreeViewExtensions.cs"
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
    using System.Linq;

    /// <summary>Contains methods that extend the <see cref="TreeView"/> control</summary>
    public static class TreeViewExtensions
    {
        #region Public Methods

        /// <summary>Finds the parent TreeViewItem of the current TreeViewItem</summary>
        /// <param name="treeView">The treeview control</param>
        /// <param name="predicate">The TreeViewItem to use as a starting point</param>
        /// <returns>The parent TreeViewItem</returns>
        public static TreeViewItem FindTreeViewItem(this TreeView treeView, Predicate<TreeViewItem> predicate)
        {
            return FindTreeViewItem(treeView.ItemContainerGenerator, treeView.Items, predicate);
        }

        #endregion

        #region Methods

        /// <summary>Finds the TreeViewItem from the collection</summary>
        /// <param name="parentItemContainerGenerator">The parent item container</param>
        /// <param name="itemCollection">The item collection</param>
        /// <param name="predicate">The TreeViewItem</param>
        /// <returns>The TreeViewItem found</returns>
        private static TreeViewItem FindTreeViewItem(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, Predicate<TreeViewItem> predicate)
        {
            foreach (var trvItem in from object item in itemCollection select item as TreeViewItem ?? (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(item))
            {
                if (predicate(trvItem))
                {
                    return trvItem;
                }

                var nestedSearchResut = FindTreeViewItem(trvItem.ItemContainerGenerator, trvItem.Items, predicate);
                if (nestedSearchResut != null)
                {
                    return nestedSearchResut;
                }
            }

            return null;
        }

        #endregion
    }
}