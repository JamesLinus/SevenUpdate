// ***********************************************************************
// <copyright file="TaskDialogButtonData.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    using System.Runtime.InteropServices;

    /// <summary>Contains the data for a <c>TaskDialogButton</c>.</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal struct TaskDialogButtonData
    {
        /// <summary>Gets or sets the id for the button.</summary>
        internal int ButtonId;

        /// <summary>Gets or sets the button text.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string ButtonText;

        /// <summary>Initializes a new instance of the <see cref="TaskDialogButtonData" /> struct.</summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="text">The text.</param>
        public TaskDialogButtonData(int buttonId, string text)
        {
            this.ButtonId = buttonId;
            this.ButtonText = text;
        }
    }
}
