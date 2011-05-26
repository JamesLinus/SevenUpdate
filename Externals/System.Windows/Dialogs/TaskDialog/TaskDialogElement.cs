// ***********************************************************************
// <copyright file="TaskDialogElement.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>The task dialog elements.</summary>
    internal enum TaskDialogElement
    {
        /// <summary>The main portion of the dialog.</summary>
        Content,

        /// <summary>Content in the expander.</summary>
        ExpandedInformation,

        /// <summary>The footer of the dialog.</summary>
        Footer,

        /// <summary>The main instructions for the dialog.</summary>
        MainInstruction
    }
}