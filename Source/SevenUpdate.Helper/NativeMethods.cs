// <copyright file="NativeMethods.cs" project="SevenUpdate.Helper">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Helper
{
    using System.Runtime.InteropServices;

    /// <summary>The Win32 native methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>Moves the file using the windows command.</summary>
        /// <param name="sourceFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="newFileName">The new name of the file or directory on the local computer.</param>
        /// <param name="flags">The flags that determine how to move the file.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero (0). To get extended error information, call GetLastError.</returns>
        [DllImport(@"kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MoveFileExW(string sourceFileName, string newFileName, int flags);
    }
}