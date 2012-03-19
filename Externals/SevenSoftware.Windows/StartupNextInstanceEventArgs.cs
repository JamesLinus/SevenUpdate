// <copyright file="StartupNextInstanceEventArgs.cs" project="SevenSoftware.Windows">BladeWise</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace SevenSoftware.Windows
{
    using System;

    /// <summary>Class used to define the arguments of another application instance startup.</summary>
    public sealed class StartupNextInstanceEventArgs : EventArgs
    {
        /// <summary>The application arguments.</summary>
        private readonly string[] args;

        /// <summary>Initializes a new instance of the <see cref="StartupNextInstanceEventArgs" /> class.</summary>
        /// <param name="args">The arguments passed to the program.</param>
        /// <param name="bringToFront">If set to <c>True</c> the application main window will be brought to front.</param>
        public StartupNextInstanceEventArgs(string[] args, bool bringToFront = true)
        {
            if (args == null)
            {
                args = new string[0];
            }

            this.args = args;
            this.BringToForeground = bringToFront;
        }

        /// <summary>Gets a value indicating whether the application main window has to be brought to foreground.</summary>
        /// <value><c>True</c> if the application window has to be brought to foreground, otherwise <c>False</c>.</value>
        public bool BringToForeground { get; private set; }

        /// <summary>Gets the arguments passed to the other application.</summary>
        /// <returns>Returns the arguments passed to the application.</returns>
        /// <value>The arguments passed to the other application.</value>
        public string[] GetArgs()
        {
            return this.args;
        }
    }
}