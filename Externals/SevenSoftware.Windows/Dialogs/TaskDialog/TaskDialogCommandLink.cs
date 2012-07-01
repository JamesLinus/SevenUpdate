// <copyright file="TaskDialogCommandLink.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System;
using System.Globalization;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Represents a command-link.</summary>
    public class TaskDialogCommandLink : TaskDialogButton
    {
        /// <summary>The instruction text for the commandlink.</summary>
        string instruction;

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

        /// <summary>Gets or sets the instruction associated with this command link button.</summary>
        public string Instruction
        {
            get { return instruction; }

            set { instruction = value; }
        }

        /// <summary>Returns a string representation of this object.</summary>
        /// <returns>A <see cref="string" /></returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture, 
                "{0}{1}{2}", 
                Text ?? string.Empty, 
                (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(instruction)) ? Environment.NewLine : string.Empty, 
                instruction ?? string.Empty);
        }
    }
}