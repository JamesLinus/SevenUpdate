//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    /// Declares the abstract base class for all custom task dialog controls.
    /// </summary>
    public abstract class TaskDialogControl : DialogControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of a task dialog control.
        /// </summary>
        protected TaskDialogControl()
        {
        }

        /// <summary>
        /// Creates a new instance of a task dialog control with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name for this control.
        /// </param>
        protected TaskDialogControl(string name)
            : base(name)
        {
        }

        #endregion
    }
}