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
    /// Defines a radio button that can be hosted in by a 
    ///   <see cref="TaskDialog"/> object.
    /// </summary>
    public class TaskDialogRadioButton : TaskDialogButtonBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TaskDialogRadioButton" /> class.
        /// </summary>
        protected TaskDialogRadioButton()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogRadioButton"/> class.
        /// </summary>
        /// <parameter name="name">
        /// The name for this control.
        /// </parameter>
        /// <parameter name="text">
        /// The value for this controls
        ///   <see cref="P:Microsoft.Windows.Dialogs.TaskDialogButtonBase.Text"/> property.
        /// </parameter>
        protected TaskDialogRadioButton(string name, string text)
            : base(name, text)
        {
        }

        #endregion
    }
}