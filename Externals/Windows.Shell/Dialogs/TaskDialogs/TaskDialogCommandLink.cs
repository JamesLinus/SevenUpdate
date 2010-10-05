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
    /// Represents a command-link.
    /// </summary>
    public class TaskDialogCommandLink : TaskDialogButton
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private string instruction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected TaskDialogCommandLink()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and label.
        /// </summary>
        /// <param name="name">
        /// The name for this button.
        /// </param>
        /// <param name="text">
        /// The label for this button.
        /// </param>
        protected TaskDialogCommandLink(string name, string text)
            : base(name, text)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name,label, and instruction.
        /// </summary>
        /// <param name="name">
        /// The name for this button.
        /// </param>
        /// <param name="text">
        /// The label for this button.
        /// </param>
        /// <param name="instruction">
        /// The instruction for this command link.
        /// </param>
        protected TaskDialogCommandLink(string name, string text, string instruction)
            : base(name, text)
        {
            this.instruction = instruction;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the instruction associated with this command link button.
        /// </summary>
        public string Instruction
        {
            get
            {
                return this.instruction;
            }

            set
            {
                this.instruction = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> 
        /// </returns>
        public override string ToString()
        {
            var instructionString = this.instruction ?? string.Empty;
            return this.Text + "\n" + instructionString;
        }

        #endregion
    }
}