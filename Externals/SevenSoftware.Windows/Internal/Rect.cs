// <copyright file="Rect.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows.Internal
{
    using System.Runtime.InteropServices;

    /// <summary>Defines the coordinates of the upper-left and lower-right corners of a rectangle.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        /// <summary>The x-coordinate of the upper-left corner of the rectangle.</summary>
        public readonly int Left;

        /// <summary>The y-coordinate of the upper-left corner of the rectangle.</summary>
        public readonly int Top;

        /// <summary>The x-coordinate of the lower-right corner of the rectangle.</summary>
        public readonly int Right;

        /// <summary>The y-coordinate of the lower-right corner of the rectangle.</summary>
        public readonly int Bottom;
    }
}