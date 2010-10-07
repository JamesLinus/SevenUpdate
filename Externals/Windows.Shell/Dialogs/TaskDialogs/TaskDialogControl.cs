// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    /// <summary>
    /// Declares the abstract base class for all custom task dialog controls.
    /// </summary>
    public abstract class TaskDialogControl : DialogControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TaskDialogControl" /> class.
        /// </summary>
        protected TaskDialogControl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogControl"/> class.
        /// </summary>
        /// <parameter name="name">
        /// The name for this control.
        /// </parameter>
        protected TaskDialogControl(string name)
            : base(name)
        {
        }

        #endregion
    }
}