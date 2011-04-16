// ***********************************************************************
// <copyright file="TaskDialogControl.cs"
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
    /// <summary>Declares the abstract base class for all custom task dialog controls.</summary>
    public abstract class TaskDialogControl : DialogControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TaskDialogControl" /> class.</summary>
        protected TaskDialogControl()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TaskDialogControl" /> class.</summary>
        /// <param name="name">The name for this control.</param>
        protected TaskDialogControl(string name)
            : base(name)
        {
        }

        #endregion
    }
}