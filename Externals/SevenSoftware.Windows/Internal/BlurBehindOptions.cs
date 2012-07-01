// <copyright file="BlurBehindOptions.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;

namespace SevenSoftware.Windows.Internal
{
    /// <summary>The blur behind flags/options.</summary>
    [Flags]
    public enum BlurBehindOptions : uint
    {
        /// <summary>Enables blur behind.</summary>
        BlurBehindEnable = 0x00000001, 

        /// <summary>The blur behind region.</summary>
        BlurBehindRegion = 0x00000002, 

        /// <summary>True to show effects with maximizing.</summary>
        TransitionOnMaximized = 0x00000004
    }
}