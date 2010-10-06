// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace Microsoft.Windows
{
    using System;

    /// <summary>
    /// Represents the method that handles the <see cref="Microsoft.Windows.InstanceAwareApplication.StartupNextInstance"/> event.
    /// </summary>
    /// <param name="sender">
    /// The object that raised the event.
    /// </param>
    /// <param name="e">
    /// The event data.
    /// </param>
    public delegate void StartupNextInstanceEventHandler(object sender, StartupNextInstanceEventArgs e);

    /// <summary>
    /// Enumerator used to define the awareness of an application, when dealing with subsequent instances of the application itself.
    /// </summary>
    public enum ApplicationInstanceAwareness : byte
    {
        /// <summary>
        ///   The awareness is global, meaning that the first application instance is aware of any other instances running on the host.
        /// </summary>
        Host = 0x00, 

        /// <summary>
        ///   The awareness is local, meaning that the first application instance is aware only of other instances running in the current user session.
        /// </summary>
        UserSession = 0x01
    }

    /// <summary>
    /// Class used to define the arguments of another application instance startup.
    /// </summary>
    public class StartupNextInstanceEventArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>
        ///   The application arguments.
        /// </summary>
        private readonly string[] args;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupNextInstanceEventArgs"/> class.
        /// </summary>
        /// <param name="args">
        /// The arguments passed to the program
        /// </param>
        public StartupNextInstanceEventArgs(string[] args)
            : this(args, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupNextInstanceEventArgs"/> class.
        /// </summary>
        /// <param name="args">
        /// The arguments passed to the program
        /// </param>
        /// <param name="bringToFront">
        /// If set to <c>true</c> the application main window will be brought to front.
        /// </param>
        public StartupNextInstanceEventArgs(string[] args, bool bringToFront)
        {
            if (args == null)
            {
                args = new string[0];
            }

            this.args = args;
            this.BringToForeground = bringToFront;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether the application main window has to be brought to foreground.
        /// </summary>
        /// <value>
        ///   <c>True</c> if the application window has to be brought to foreground, otherwise <c>false</c>.
        /// </value>
        public bool BringToForeground { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the arguments passed to the other application.
        /// </summary>
        /// <returns>Returns the arguments passed to the application</returns>
        /// <value>
        /// The arguments passed to the other application.
        /// </value>
        public string[] GetArgs()
        {
            return this.args;
        }

        #endregion
    }
}