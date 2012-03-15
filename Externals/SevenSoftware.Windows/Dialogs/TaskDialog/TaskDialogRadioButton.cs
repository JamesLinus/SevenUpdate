// ***********************************************************************
// <copyright file="TaskDialogRadioButton.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Defines a radio button that can be hosted in by a <see cref="TaskDialog" /> object.</summary>
    public class TaskDialogRadioButton : TaskDialogButtonBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogRadioButton" /> class. Creates a new instance of
        ///   this class.
        /// </summary>
        public TaskDialogRadioButton()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogRadioButton" /> class. Creates a new instance of
        ///   this class with the specified name and text.
        /// </summary>
        /// <param name="name">The name for this control.</param>
        /// <param name="text">The value for this controls <see cref="TaskDialogButtonBase.Text" /> property.</param>
        public TaskDialogRadioButton(string name, string text) : base(name, text)
        {
        }
    }
}