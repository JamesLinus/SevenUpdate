namespace System.Windows
{
    /// <summary>
    /// Represents the method that handles the <see cref="InstanceAwareApplication.StartupNextInstance"/> event.
    /// </summary>
    /// <param name="sender">
    /// The object that raised the event.
    /// </param>
    /// <param name="e">
    /// The event data.
    /// </param>
    public delegate void StartupNextInstanceEventHandler(object sender, StartupNextInstanceEventArgs e);

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
        public bool BringToForeground { get; private set; }

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