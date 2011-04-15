// ***********************************************************************
// <copyright file="TaskDialogExpandedDetailsLocation.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.Dialogs
{
    /// <summary>Specifies the options for expand/collapse sections in dialogs.</summary>
    public enum TaskDialogExpandedDetailsLocation
    {
        /// <summary>Do not show the content.</summary>
        Hide, 

        /// <summary>Show the content.</summary>
        ExpandContent, 

        /// <summary>Expand the footer content.</summary>
        ExpandFooter
    }
}