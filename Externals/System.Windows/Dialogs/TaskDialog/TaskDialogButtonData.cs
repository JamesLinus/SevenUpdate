// <copyright file="TaskDialogButtonData.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using System.Runtime.InteropServices;

    /// <summary>Contains the data for a <see cref="TaskDialogButton"/>.</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal struct TaskDialogButtonData
    {
        /// <summary>Indicates the value to be returned when this button is selected.</summary>
        private readonly int ButtonID;

        /// <summary>The text to display on the button.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        private readonly string ButtonText;

        /// <summary>Initializes a new instance of the <see cref="TaskDialogButtonData" /> struct.</summary>
        /// <param name="buttonId">The button ID.</param>
        /// <param name="buttonText">The button text.</param>
        public TaskDialogButtonData(int buttonId, string buttonText)
        {
            this.ButtonID = buttonId;
            this.ButtonText = buttonText;
        }
    }
}