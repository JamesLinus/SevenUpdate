// <copyright file="IconUnion.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System;
using System.Runtime.InteropServices;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>
    ///   NOTE: We include a "spacer" so that the struct size varies on 64-bit architectures. NOTE: Packing must be set
    ///   to 4 to make this work on 64-bit platforms. NOTE: Do not convert to auto properties and do not change layout or it will break!
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
    internal struct IconUnion
    {
        /// <summary>The index for the icon to display has the main icon.</summary>
        [FieldOffset(0)]
        int mainIcon;

        /// <summary>This field is used to adjust the length of the structure on 32/64bit OS.</summary>
        [FieldOffset(0)]
        IntPtr spacer;

        /// <summary>Initializes a new instance of the <see cref="IconUnion" /> struct.</summary>
        /// <param name="i">The index for the icon.</param>
        internal IconUnion(int i)
        {
            spacer = IntPtr.Zero;
            mainIcon = i;
        }

        /// <summary>Gets the handle to the Icon</summary>
        public int MainIcon
        {
            get { return mainIcon; }
        }
    }
}