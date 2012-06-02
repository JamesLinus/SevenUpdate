// <copyright file="Margins.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows.Internal
{
    using System.Runtime.InteropServices;

    /// <summary>Defines the margins of windows that have visual styles applied.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        /// <summary>Initializes a new instance of the <see cref="Margins" /> struct.</summary>
        /// <param name="fullWindow">If set to <c>True</c> the margin is set to the full window.</param>
        public Margins(bool fullWindow) : this()
        {
            this.LeftWidth = this.RightWidth = this.TopHeight = this.BottomHeight = fullWindow ? -1 : 0;
        }

        /// <summary>Initializes a new instance of the <see cref="Margins" /> struct.</summary>
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