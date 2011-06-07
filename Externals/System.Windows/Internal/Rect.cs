// ***********************************************************************
// <copyright file="Rect.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
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

namespace System.Windows.Internal
{
    using Runtime.InteropServices;

    /// <summary>
    ///   Defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        /// <summary>
        ///   The x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public readonly int Left;

        /// <summary>
        ///   The y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public readonly int Top;

        /// <summary>
        ///   The x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public readonly int Right;

        /// <summary>
        ///   The y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public readonly int Bottom;
    }
}