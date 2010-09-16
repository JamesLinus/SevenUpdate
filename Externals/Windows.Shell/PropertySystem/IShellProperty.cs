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

using System;

#endregion

namespace Microsoft.Windows.Shell.PropertySystem
{
    /// <summary>
    ///   Defines the properties used by a Shell Property.
    /// </summary>
    public interface IShellProperty
    {
        /// <summary>
        ///   Gets the property key that identifies this property.
        /// </summary>
        PropertyKey PropertyKey { get; }

        /// <summary>
        ///   Get the property description object.
        /// </summary>
        ShellPropertyDescription Description { get; }

        /// <summary>
        ///   Gets the case-sensitive name of the property as it is known to the system, 
        ///   regardless of its localized name.
        /// </summary>
        string CanonicalName { get; }

        /// <summary>
        ///   Gets the value for this property using the generic Object type.
        /// </summary>
        /// <remarks>
        ///   To obtain a specific type for this value, use the more strongly-typed 
        ///   <c>Property&lt;T&gt;</c> class.
        ///   You can only set a value for this type using the <c>Property&lt;T&gt;</c> 
        ///   class.
        /// </remarks>
        object ValueAsObject { get; }

        /// <summary>
        ///   Gets the <c>System.Type</c> value for this property.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        ///   Gets the image reference path and icon index associated with a property value. 
        ///   This API is only available in Windows 7.
        /// </summary>
        IconReference IconReference { get; }

        /// <summary>
        ///   Gets a formatted, Unicode string representation of a property value.
        /// </summary>
        /// <param name = "format">One or more <c>PropertyDescriptionFormat</c> flags 
        ///   chosen to produce the desired display format.</param>
        /// <returns>The formatted value as a string.</returns>
        string FormatForDisplay(PropertyDescriptionFormat format);
    }
}