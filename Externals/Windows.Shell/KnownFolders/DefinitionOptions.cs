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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Specifies behaviors for known folders.
    /// </summary>
    [Flags]
    public enum DefinitionOptions
    {
        /// <summary>
        ///   No behaviors are defined.
        /// </summary>
        None = 0x0,
        /// <summary>
        ///   Prevents a per-user known folder from being 
        ///   redirected to a network location.
        /// </summary>
        LocalRedirectOnly = 0x2,

        /// <summary>
        ///   The known folder can be roamed through PC-to-PC synchronization.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Roamable", Justification = "This is following the native API")] Roamable = 0x4,

        /// <summary>
        ///   Creates the known folder when the user first logs on.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Precreate", Justification = "This is following the native API")] Precreate = 0x8
    }
}