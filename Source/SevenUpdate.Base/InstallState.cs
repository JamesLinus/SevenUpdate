// ***********************************************************************
// <copyright file="InstallState.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
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

namespace SevenUpdate
{
    /// <summary>The Msi Component install state.</summary>
    internal enum InstallState
    {
        /// <summary>The component being requested is disabled on the computer.</summary>
        NotUsed = -7,

        /// <summary>The configuration data is corrupt.</summary>
        BadConfig = -6,

        /// <summary>The installation is incomplete.</summary>
        Incomplete = -5,

        /// <summary>The component source is inaccessible.</summary>
        SourceAbsent = -4,

        /// <summary>One of the function parameters is invalid.</summary>
        InvalidArg = -2,

        /// <summary>The product code or component ID is unknown.</summary>
        Unknown = -1,

        /// <summary>The shortcut is advertised.</summary>
        Advertised = 1,

        /// <summary>The component has been removed.</summary>
        Removed = 1,

        /// <summary>The component is not installed.</summary>
        Absent = 2,

        /// <summary>The component is installed locally.</summary>
        Local = 3,

        /// <summary>The component is installed to run from source.</summary>
        Source = 4,
    }
}