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
    /// Defines a radio button that can be hosted in by a 
    ///   <see cref="TaskDialog"/> object.
    /// </summary>
    public class TaskDialogRadioButton : TaskDialogButtonBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected TaskDialogRadioButton()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with
        ///   the specified name and text.
        /// </summary>
        /// <param name="name">
        /// The name for this control.
        /// </param>
        /// <param name="text">
        /// The value for this controls 
        ///   <see cref="P:Microsoft.Windows.Dialogs.TaskDialogButtonBase.Text"/> property.
        /// </param>
        protected TaskDialogRadioButton(string name, string text)
            : base(name, text)
        {
        }

        #endregion
    }
}