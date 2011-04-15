// ***********************************************************************
// <copyright file="Margins.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
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
    using System.Runtime.InteropServices;

    /// <summary>Defines the margins of windows that have visual styles applied.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        /// <summary>Initializes a new instance of the <see cref="Margins"/> struct.</summary>
        /// <param name="fullWindow">if set to <see langword="true"/> the margin is set to the full window.</param>
        public Margins(bool fullWindow) : this()
        {
            this.LeftWidth = this.RightWidth = this.TopHeight = this.BottomHeight = fullWindow ? -1 : 0;
        }

        /// <summary>Initializes a new instance of the <see cref="Margins"/> struct.</summary>
        /// <param name="left">Width of the left border that retains its size.</param>
        /// <param name="top">Height of the top border that retains its size.</param>
        /// <param name="right">Width of the right border that retains its size.</param>
        /// <param name="bottom">Height of the bottom border that retains its size.</param>
        public Margins(int left, int top, int right, int bottom) : this()
        {
            this.LeftWidth = left;
            this.RightWidth = right;
            this.TopHeight = top;
            this.BottomHeight = bottom;
        }

        /// <summary>Gets the width of the left border that retains its size.</summary>
        public int LeftWidth { get; private set; }

        /// <summary>Gets the width of the right border that retains its size.</summary>
        public int RightWidth { get; private set; }

        /// <summary>Gets the height of the top border that retains its size.</summary>
        public int TopHeight { get; private set; }

        /// <summary>Gets the height of the bottom border that retains its size.</summary>
        public int BottomHeight { get; private set; }
    }
}