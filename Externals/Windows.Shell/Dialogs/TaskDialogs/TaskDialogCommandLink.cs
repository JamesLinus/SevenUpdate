#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Represents a command-link.
    /// </summary>
    public abstract class TaskDialogCommandLink : TaskDialogButton
    {
        private string instruction;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public TaskDialogCommandLink()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name and label.
        /// </summary>
        /// <param name = "name">The name for this button.</param>
        /// <param name = "text">The label for this button.</param>
        public TaskDialogCommandLink(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name,label, and instruction.
        /// </summary>
        /// <param name = "name">The name for this button.</param>
        /// <param name = "text">The label for this button.</param>
        /// <param name = "instruction">The instruction for this command link.</param>
        public TaskDialogCommandLink(string name, string text, string instruction) : base(name, text)
        {
            this.instruction = instruction;
        }

        /// <summary>
        ///   Gets or sets the instruction associated with this command link button.
        /// </summary>
        public string Instruction { get { return instruction; } set { instruction = value; } }

        /// <summary>
        ///   Returns a string representation of this object.
        /// </summary>
        /// <returns> A <see cref = "System.String" /> </returns>
        public override string ToString()
        {
            var instructionString = (instruction ?? "");
            return Text + "\n" + instructionString;
        }
    }
}