// ***********************************************************************
// <copyright file="TaskDialogCommandLink.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>
    ///   Represents a command-link.
    /// </summary>
    public class TaskDialogCommandLink : TaskDialogButton
    {
        #region Constants and Fields

        /// <summary>
        ///   The instruction for the task dialog.
        /// </summary>
        private string instruction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the TaskDialogCommandLink class.
        /// </summary>
        protected TaskDialogCommandLink()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogCommandLink" /> class.
        /// </summary>
        /// <param name="name">
        ///   The name for this button.
        /// </param>
        /// <param name="text">
        ///   The label for this button.
        /// </param>
        protected TaskDialogCommandLink(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogCommandLink" /> class.
        /// </summary>
        /// <param name="name">
        ///   The name for this button.
        /// </param>
        /// <param name="text">
        ///   The label for this button.
        /// </param>
        /// <param name="instruction">
        ///   The instruction for this command link.
        /// </param>
        protected TaskDialogCommandLink(string name, string text, string instruction) : base(name, text)
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
        ///   Returns a string representation of this object.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> .
        /// </returns>
        public override string ToString()
        {
            var instructionString = this.instruction ?? string.Empty;
            return this.Text + "\n" + instructionString;
        }

        #endregion
    }
}