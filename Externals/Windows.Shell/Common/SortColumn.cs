//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Shell.PropertySystem;

    /// <summary>
    /// Stores information about how to sort a column that is displayed in the folder view.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SortColumn
    {
        /// <summary>
        /// Creates a sort column with the specified direction for the given property.
        /// </summary>
        /// <param name="propertyKey">
        /// Property key for the property that the user will sort.
        /// </param>
        /// <param name="direction">
        /// The direction in which the items are sorted.
        /// </param>
        public SortColumn(PropertyKey propertyKey, SortDirection direction)
        {
            this.PropertyKey = propertyKey;
            this.Direction = direction;
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

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator ==(SortColumn x, SortColumn y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator !=(SortColumn x, SortColumn y)
        {
            throw new NotImplementedException();
        }
    } ;
}