// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows
{
    /// <summary>
    /// Represents the method that handles the <see cref="InstanceAwareApplication.StartupNextInstance"/> event.
    /// </summary>
    /// <parameter name="sender">
    /// The object that raised the event.
    /// </parameter>
    /// <parameter name="e">
    /// The event data.
    /// </parameter>
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
        /// <parameter name="args">
        /// The arguments passed to the program
        /// </parameter>
        public StartupNextInstanceEventArgs(string[] args)
            : this(args, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupNextInstanceEventArgs"/> class.
        /// </summary>
        /// <parameter name="args">
        /// The arguments passed to the program
        /// </parameter>
        /// <parameter name="bringToFront">
        /// If set to <c>true</c> the application main window will be brought to front.
        /// </parameter>
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
        /// <returns>
        /// Returns the arguments passed to the application
        /// </returns>
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