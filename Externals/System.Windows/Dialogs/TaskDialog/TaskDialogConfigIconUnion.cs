// <copyright file="TaskDialogConfigIconUnion.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.Dialogs
{
    using System.Runtime.InteropServices;

    /// <summary>Contains the data for a <see cref="TaskDialogIcon"/>.</summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
    internal struct TaskDialogConfigIconUnion
    {
        /// <summary>The main icon Id to display.</summary>
        [FieldOffset(0)]
        private readonly int MainIcon;

        /// <summary>The footer icon id to display.</summary>
        [FieldOffset(0)]
        private readonly int Icon;

        /// <summary>The pointer to the space.</summary>
        [FieldOffset(0)]
        private readonly IntPtr Spacer;

        /// <summary>Initializes a new instance of the <see cref="TaskDialogConfigIconUnion" /> struct.</summary>
        /// <param name="i">The icon identifier.</param>
        internal TaskDialogConfigIconUnion(int i)
        {
            this.Spacer = IntPtr.Zero;
            this.Icon = 0;
            this.MainIcon = i;
        }
    }
}