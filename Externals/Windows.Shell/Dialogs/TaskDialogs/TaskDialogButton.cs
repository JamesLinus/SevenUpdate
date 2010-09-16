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
    ///   Implements a button that can be hosted in a task dialog.
    /// </summary>
    public class TaskDialogButton : TaskDialogButtonBase
    {
        private bool showElevationIcon;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public TaskDialogButton()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified property settings.
        /// </summary>
        /// <param name = "name">The name of the button.</param>
        /// <param name = "text">The button label.</param>
        public TaskDialogButton(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Gets or sets a value that controls whether the elevation icon is displayed.
        /// </summary>
        public bool ShowElevationIcon
        {
            get { return showElevationIcon; }
            set
            {
                CheckPropertyChangeAllowed("ShowElevationIcon");
                showElevationIcon = value;
                ApplyPropertyChange("ShowElevationIcon");
            }
        }
    }
}