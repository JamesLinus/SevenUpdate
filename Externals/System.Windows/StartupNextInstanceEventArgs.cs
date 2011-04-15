// ***********************************************************************
// <copyright file="StartupNextInstanceEventArgs.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) BladeWise. All rights reserved.
// </copyright>
// <author username="BladeWise">BladeWise</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://wpfinstanceawareapp.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************

namespace System.Windows
{
    /// <summary>Class used to define the arguments of another application instance startup.</summary>
    public sealed class StartupNextInstanceEventArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>The application arguments.</summary>
        private readonly string[] args;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="StartupNextInstanceEventArgs" /> class.</summary>
        /// <param name="args">The arguments passed to the program</param>
        /// <param name="bringToFront">If set to <see langword="true" /> the application main window will be brought to front.</param>
        public StartupNextInstanceEventArgs(string[] args, bool bringToFront = true)
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

        /// <summary>Gets a value indicating whether the application main window has to be brought to foreground.</summary>
        /// <value><see langword="true" /> if the application window has to be brought to foreground, otherwise <see langword="false" /></value>
        public bool BringToForeground { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>Gets the arguments passed to the other application.</summary>
        /// <returns>Returns the arguments passed to the application</returns>
        /// <value>The arguments passed to the other application.</value>
        public string[] GetArgs()
        {
            return this.args;
        }

        #endregion
    }
}