// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Dialogs.TaskDialogs
{
    /// <summary>
    /// Defines a radio button that can be hosted in by a 
    ///   <see cref="TaskDialog"/> object.
    /// </summary>
    public class TaskDialogRadioButton : TaskDialogButtonBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogRadioButton"/> class.
        /// </summary>
        protected TaskDialogRadioButton()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogRadioButton"/> class.
        /// </summary>
        /// <param name="name">The name for this control.</param>
        /// <param name="text">The value for this controls
        /// <see cref="P:Microsoft.Windows.Dialogs.TaskDialogButtonBase.Text"/> property.</param>
        protected TaskDialogRadioButton(string name, string text)
            : base(name, text)
        {
        }

        #endregion
    }
}