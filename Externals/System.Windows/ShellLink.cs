// ***********************************************************************
// <copyright file="ShellLink.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace System.Windows
{
    using System.Runtime.InteropServices;

    /// <summary>The Shell link class</summary>
    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    public class ShellLink
    {
        #region Properties

        /// <summary>Gets or sets the shortcut command line arguments</summary>
        public extern string Arguments { get; set; }

        /// <summary>Gets or sets the shortcut description</summary>
        public extern string Description { get; set; }

        /// <summary>Gets or sets the path to the shortcut icon</summary>
        public extern string Icon { get; set; }

        /// <summary>Gets or sets the full path to the shortcut lnk file</summary>
        public extern string Location { get; set; }

        /// <summary>Gets or sets the filename for the shortcut</summary>
        public extern string Name { get; set; }

        /// <summary>Gets or sets the shortcut target</summary>
        public extern string Target { get; set; }

        #endregion
    }
}