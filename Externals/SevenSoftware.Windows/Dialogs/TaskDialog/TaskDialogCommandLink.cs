// ***********************************************************************
// <copyright file="TaskDialogCommandLink.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;
    using System.Globalization;

    /// <summary>Represents a command-link.</summary>
    public class TaskDialogCommandLink : TaskDialogButton
    {
        #region Constants and Fields

        /// <summary>The instruction text for the commandlink.</summary>
        private string instruction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogCommandLink" /> class. Creates a new instance of
        ///   this class.
        /// </summary>
        public TaskDialogCommandLink()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogCommandLink" /> class. Creates a new instance of
        ///   this class with the specified name and label.
        /// </summary>
        /// <param name="name">The name for this button.</param>
        /// <param name="text">The label for this button.</param>
        public TaskDialogCommandLink(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogCommandLink" /> class. Creates a new instance of
        ///   this class with the specified name,label, and instruction.
        /// </summary>
        /// <param name="name">The name for this button.</param>
        /// <param name="text">The label for this button.</param>
        /// <param name="instruction">The instruction for this command link.</param>
        public TaskDialogCommandLink(string name, string text, string instruction) : base(name, text)
        {
            this.instruction = instruction;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the instruction associated with this command link button.</summary>
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

        #region Public Methods and Operators

        /// <summary>Returns a string representation of this object.</summary>
        /// <returns>A <see cref="string" /></returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture, 
                "{0}{1}{2}", 
                this.Text ?? string.Empty, 
                (string.IsNullOrEmpty(this.Text) || string.IsNullOrEmpty(this.instruction))
                    ? Environment.NewLine : string.Empty, 
                this.instruction ?? string.Empty);
        }

        #endregion
    }
}