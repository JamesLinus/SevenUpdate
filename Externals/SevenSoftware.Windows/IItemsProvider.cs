// ***********************************************************************
// <copyright file="IItemsProvider.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SteamCDRTool" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="Robert Baker">Robert</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
// This file is part of SteamCDRTool.
//   SteamCDRTool is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//   License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//   later version. SteamCDRTool is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//   even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
//   License for more details. You should have received a copy of the GNU General Public  License
//   along with SteamCDRTool.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenSoftware.Windows
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a provider of collection details.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IItemsProvider<T>
    {
        /// <summary>
        /// Gets the total number of items available.
        /// </summary>
        /// <returns>The total number of items to fetch.</returns>
        int Count { get; }

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns>An IList containing the items in the specified range.</returns>
        IList<T> FetchRange(int startIndex, int count);
    }
}