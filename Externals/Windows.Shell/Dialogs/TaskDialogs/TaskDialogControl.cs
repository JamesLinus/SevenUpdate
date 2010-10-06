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
    /// Declares the abstract base class for all custom task dialog controls.
    /// </summary>
    public abstract class TaskDialogControl : DialogControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogControl"/> class.
        /// </summary>
        protected TaskDialogControl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogControl"/> class.
        /// </summary>
        /// <param name="name">The name for this control.</param>
        protected TaskDialogControl(string name)
            : base(name)
        {
        }

        #endregion
    }
}