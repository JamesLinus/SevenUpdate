// ***********************************************************************
// <copyright file="TreeViewExtensions.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenSoftware.Windows.Controls
{
    using System;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>Contains methods that extend the <c>TreeView</c> control.</summary>
    public static class TreeViewExtensions
    {
        /// <summary>Finds the parent TreeViewItem of the current TreeViewItem.</summary>
        /// <param name="treeView">The treeview control.</param>
        /// <param name="predicate">The TreeViewItem to use as a starting point.</param>
        /// <returns>The parent TreeViewItem.</returns>
        public static TreeViewItem FindTreeViewItem(this ItemsControl treeView, Predicate<TreeViewItem> predicate)
        {
            if (treeView == null)
            {
                throw new ArgumentNullException("treeView");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return FindTreeViewItem(treeView.ItemContainerGenerator, treeView.Items, predicate);
        }

        /// <summary>Finds the TreeViewItem from the collection.</summary>
        /// <param name="parentItemContainerGenerator">The parent item container.</param>
        /// <param name="itemCollection">The item collection.</param>
        /// <param name="predicate">The TreeViewItem.</param>
        /// <returns>The TreeViewItem found.</returns>
        private static TreeViewItem FindTreeViewItem(
            ItemContainerGenerator parentItemContainerGenerator, 
            ItemCollection itemCollection, 
            Predicate<TreeViewItem> predicate)
        {
            foreach (var trvItem in
                from object item in itemCollection
                select item as TreeViewItem ?? (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(item))
            {
                if (predicate(trvItem))
                {
                    return trvItem;
                }

                TreeViewItem nestedSearchResult = FindTreeViewItem(
                    trvItem.ItemContainerGenerator, trvItem.Items, predicate);
                if (nestedSearchResult != null)
                {
                    return nestedSearchResult;
                }
            }

            return null;
        }
    }
}