#region

using System;

#endregion

namespace Microsoft.Windows
{
    /// <summary>
    ///   Represents the method that handles the <see cref = "Microsoft.Windows.InstanceAwareApplication.StartupNextInstance" /> event.
    /// </summary>
    /// <param name = "sender"> The object that raised the event.</param>
    /// <param name = "e">The event data.</param>
    public delegate void StartupNextInstanceEventHandler(object sender, StartupNextInstanceEventArgs e);

    /// <summary>
    ///   Enumerator used to define the awareness of an application, when dealing with subsequent instances of the application itself.
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
    ///   Class used to define the arguments of another application instance startup.
    /// </summary>
    public class StartupNextInstanceEventArgs : EventArgs
    {
        /// <summary>
        ///   The application arguments.
        /// </summary>
        private readonly string[] mArgs;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "StartupNextInstanceEventArgs" /> class.
        /// </summary>
        /// <param name = "args">The args.</param>
        public StartupNextInstanceEventArgs(string[] args) : this(args, true)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "StartupNextInstanceEventArgs" /> class.
        /// </summary>
        /// <param name = "args">The args.</param>
        /// <param name = "bringToFront">If set to <c>true</c> the application main window will be brought to front.</param>
        public StartupNextInstanceEventArgs(string[] args, bool bringToFront)
        {
            if (args == null)
                args = new string[0];

            mArgs = args;
            BringToForeground = bringToFront;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the application main window has to be brought to foreground.
        /// </summary>
        /// <value><c>True</c> if the application windiow has to be brought to foreground, otherwise <c>false</c>.</value>
        public bool BringToForeground { get; set; }

        /// <summary>
        ///   Gets the arguments passed to the other application.
        /// </summary>
        /// <value>The arguments passed to the other application.</value>
        public string[] Args { get { return mArgs; } }
    }
}