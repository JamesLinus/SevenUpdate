// ***********************************************************************
// <copyright file="TaskDialogResult.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    /// <summary>Indicates the various buttons and options clicked by the user on the task dialog.</summary>
    public enum TaskDialogResult
    {
        /// <summary>No button was selected.</summary>
        None = 0x0000, 

        /// <summary>"OK" button was clicked</summary>
        Ok = 0x0001, 

        /// <summary>"Yes" button was clicked</summary>
        Yes = 0x0002, 

        /// <summary>"No" button was clicked</summary>
        No = 0x0004, 

        /// <summary>"Cancel" button was clicked</summary>
        Cancel = 0x0008, 

        /// <summary>"Retry" button was clicked</summary>
        Retry = 0x0010, 

        /// <summary>"Close" button was clicked</summary>
        Close = 0x0020, 

        /// <summary>A custom button was clicked.</summary>
        CustomButtonClicked = 0x0100, 
    }
}
