// <copyright file="DwmBlurBehind.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Runtime.InteropServices;

namespace SevenSoftware.Windows.Internal
{
    /// <summary>Specifies Desktop Window Manager (DWM) blur behind properties.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DwmBlurBehind
    {
        /// <summary>A bitwise combination of DWM Blur Behind Constants values indicating which members are set.</summary>
        public BlurBehindOptions Flags;

        /// <summary>
        ///   Registers the window handle to DWM blur behind; <c>False</c> to unregister the window handle from DWM blur
        ///   behind.
        /// </summary>
        public bool Enable;

        /// <summary>
        ///   The region within the client area to apply the blur behind. A <c>null</c> value will apply the blur behind
        ///   the entire client area.
        /// </summary>
        public IntPtr RegionBlur;

        /// <summary>Window's colorization should transition to match the maximized windows; otherwise, <c>False</c>.</summary>
        public bool TransitionOnMaximized;
    }
}