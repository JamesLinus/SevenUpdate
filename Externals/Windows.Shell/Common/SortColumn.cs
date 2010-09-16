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

using System.Runtime.InteropServices;
using Microsoft.Windows.Shell.PropertySystem;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Stores information about how to sort a column that is displayed in the folder view.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SortColumn
    {
        /// <summary>
        ///   Creates a sort column with the specified direction for the given property.
        /// </summary>
        /// <param name = "propertyKey">Property key for the property that the user will sort.</param>
        /// <param name = "direction">The direction in which the items are sorted.</param>
        public SortColumn(PropertyKey propertyKey, SortDirection direction)
        {
            PropertyKey = propertyKey;
            Direction = direction;
        }

        /// <summary>
        ///   The ID of the column by which the user will sort. A PropertyKey structure. 
        ///   For example, for the "Name" column, the property key is PKEY_ItemNameDisplay or
        ///   <see cref = "Microsoft.Windows.Shell.PropertySystem.SystemProperties.System.ItemName" />.
        /// </summary>
        public PropertyKey PropertyKey;

        /// <summary>
        ///   The direction in which the items are sorted.
        /// </summary>
        public SortDirection Direction;
    } ;
}