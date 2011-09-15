// ***********************************************************************
// <copyright file="DialogShowState.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>Dialog Show State</summary>
    public enum DialogShowState
    {
        /// <summary>The dialog is about to be shown</summary>
        PreShow, 

        /// <summary>The dialog is shown</summary>
        Showing, 

        /// <summary>The dialog is currently closing</summary>
        Closing, 

        /// <summary>The dialog is closed</summary>
        Closed
    }
}
