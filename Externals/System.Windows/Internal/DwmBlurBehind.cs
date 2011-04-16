// ***********************************************************************
// <copyright file="DwmBlurBehind.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
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

    /// <summary>Specifies Desktop Window Manager (DWM) blur behind properties.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DwmBlurBehind
    {
        /// <summary>A bitwise combination of DWM Blur Behind Constants values indicating which members are set.</summary>
        public BlurBehindOptions Flags;

        /// <summary>Registers the window handle to DWM blur behind; <see langword="false" /> to unregister the window handle from DWM blur behind.</summary>
        public bool Enable;

        /// <summary>The region within the client area to apply the blur behind. A <see langword="null" /> value will apply the blur behind the entire client area.</summary>
        public IntPtr RegionBlur;

        /// <summary>Window's colorization should transition to match the maximized windows; otherwise, <see langword="false" />.</summary>
        public bool TransitionOnMaximized;
    }
}