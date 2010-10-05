//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs.Controls
{
    using System.Windows.Markup;

    /// <summary>
    /// Defines the properties and constructors for all prominent controls in the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public abstract class CommonFileDialogProminentControl : CommonFileDialogControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogProminentControl()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogProminentControl(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name="name">
        /// The name of this control.
        /// </param>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogProminentControl(string name, string text)
            : base(name, text)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the prominent value of this control.
        /// </summary>
        /// <remarks>
        ///   Only one control can be specified as prominent. If more than one control is specified prominent, 
        ///   then an 'E_UNEXPECTED' exception will be thrown when these controls are added to the dialog. 
        ///   A group box control can only be specified as prominent if it contains one control and that control is of type 'CommonFileDialogProminentControl'.
        /// </remarks>
        public bool IsProminent { get; set; }

        #endregion
    }
}