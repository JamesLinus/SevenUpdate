// ***********************************************************************
// <copyright file="IconUnion.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///   NOTE: We include a "spacer" so that the struct size varies on 64-bit architectures. NOTE: Packing must be set
    ///   to 4 to make this work on 64-bit platforms. NOTE: Do not convert to auto properties and do not change layout or it will break!
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
    internal struct IconUnion
    {
        /// <summary>The index for the icon to display has the main icon.</summary>
        [FieldOffset(0)]
        private int mainIcon;

        /// <summary>This field is used to adjust the length of the structure on 32/64bit OS.</summary>
        [FieldOffset(0)]
        private IntPtr spacer;

        /// <summary>Initializes a new instance of the <see cref="IconUnion" /> struct.</summary>
        /// <param name="i">The index for the icon.</param>
        internal IconUnion(int i)
        {
            this.spacer = IntPtr.Zero;
            this.mainIcon = i;
        }

        /// <summary>Gets the handle to the Icon</summary>
        public int MainIcon
        {
            get
            {
                return this.mainIcon;
            }
        }
    }
}