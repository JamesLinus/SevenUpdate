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
    ///   Defines a common class for all task dialog bar controls, such as the progress and marquee bars.
    /// </summary>
    public class TaskDialogBar : TaskDialogControl
    {
        private TaskDialogProgressBarState state;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public TaskDialogBar()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name = "name">The name for this control.</param>
        protected TaskDialogBar(string name) : base(name)
        {
        }

        /// <summary>
        ///   Gets or sets the state of the progress bar.
        /// </summary>
        public TaskDialogProgressBarState State
        {
            get { return state; }
            set
            {
                CheckPropertyChangeAllowed("State");
                state = value;
                ApplyPropertyChange("State");
            }
        }

        /// <summary>
        ///   Resets the state of the control to normal.
        /// </summary>
        protected internal virtual void Reset()
        {
            state = TaskDialogProgressBarState.Normal;
        }
    }
}