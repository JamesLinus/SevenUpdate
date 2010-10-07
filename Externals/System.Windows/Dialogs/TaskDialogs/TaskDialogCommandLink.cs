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
    /// Represents a command-link.
    /// </summary>
    public class TaskDialogCommandLink : TaskDialogButton
    {
        #region Constants and Fields

        /// <summary>
        ///   The instruction for the task dialog
        /// </summary>
        private string instruction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TaskDialogCommandLink" /> class.
        /// </summary>
        protected TaskDialogCommandLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogCommandLink"/> class.
        /// </summary>
        /// <parameter name="name">
        /// The name for this button.
        /// </parameter>
        /// <parameter name="text">
        /// The label for this button.
        /// </parameter>
        protected TaskDialogCommandLink(string name, string text)
            : base(name, text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogCommandLink"/> class.
        /// </summary>
        /// <parameter name="name">
        /// The name for this button.
        /// </parameter>
        /// <parameter name="text">
        /// The label for this button.
        /// </parameter>
        /// <parameter name="instruction">
        /// The instruction for this command link.
        /// </parameter>
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