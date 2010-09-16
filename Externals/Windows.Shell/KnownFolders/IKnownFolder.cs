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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents a registered or known folder in the system.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public interface IKnownFolder : IDisposable, IEnumerable<ShellObject>
    {
        /// <summary>
        ///   Gets the path for this known folder.
        /// </summary>
        string Path { get; }

        /// <summary>
        ///   Gets the category designation for this known folder.
        /// </summary>
        FolderCategory Category { get; }

        /// <summary>
        ///   Gets this known folder's canonical name.
        /// </summary>
        string CanonicalName { get; }

        /// <summary>
        ///   Gets this known folder's description.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///   Gets the unique identifier for this known folder's parent folder.
        /// </summary>
        Guid ParentId { get; }

        /// <summary>
        ///   Gets this known folder's relative path.
        /// </summary>
        string RelativePath { get; }

        /// <summary>
        ///   Gets this known folder's parsing name.
        /// </summary>
        string ParsingName { get; }

        /// <summary>
        ///   Gets this known folder's tool tip text.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        ///   Gets the resource identifier for this 
        ///   known folder's tool tip text.
        /// </summary>
        string TooltipResourceId { get; }

        /// <summary>
        ///   Gets this known folder's localized name.
        /// </summary>
        string LocalizedName { get; }

        /// <summary>
        ///   Gets the resource identifier for this 
        ///   known folder's localized name.
        /// </summary>
        string LocalizedNameResourceId { get; }

        /// <summary>
        ///   Gets this known folder's security attributes.
        /// </summary>
        string Security { get; }

        /// <summary>
        ///   Gets this known folder's file attributes, 
        ///   such as "read-only".
        /// </summary>
        FileAttributes FileAttributes { get; }

        /// <summary>
        ///   Gets an value that describes this known folder's behaviors.
        /// </summary>
        DefinitionOptions DefinitionOptions { get; }

        /// <summary>
        ///   Gets the unique identifier for this known folder's type.
        /// </summary>
        Guid FolderTypeId { get; }

        /// <summary>
        ///   Gets a string representation of this known folder's type.
        /// </summary>
        string FolderType { get; }

        /// <summary>
        ///   Gets the unique identifier for this known folder.
        /// </summary>
        Guid FolderId { get; }

        /// <summary>
        ///   Gets a value that indicates whether this known folder's path exists on the computer.
        /// </summary>
        /// <remarks>
        ///   If this property value is <b>false</b>, 
        ///   the folder might be a virtual folder (<see cref = "Category" /> property will
        ///   be <see cref = "FolderCategory.Virtual" /> for virtual folders)
        /// </remarks>
        bool PathExists { get; }

        /// <summary>
        ///   Gets a value that states whether this known folder 
        ///   can have its path set to a new value, 
        ///   including any restrictions on the redirection.
        /// </summary>
        RedirectionCapabilities Redirection { get; }
    }
}