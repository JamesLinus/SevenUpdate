// ***********************************************************************
// <copyright file="TaskDialogRadioButton.cs"
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
    /// <summary>Defines a radio button that can be hosted in by a <see cref="TaskDialog" /> object.</summary>
    public class TaskDialogRadioButton : TaskDialogButtonBase
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TaskDialogRadioButton" /> class.</summary>
        protected TaskDialogRadioButton()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TaskDialogRadioButton" /> class.</summary>
        /// <param name="name">The name for this control.</param>
        /// <param name="text">The value for this controls<see cref="TaskDialogButtonBase.Text" /> property.</param>
        protected TaskDialogRadioButton(string name, string text)
            : base(name, text)
        {
        }

        #endregion
    }
}