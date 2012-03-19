// <copyright file="JobErrorNotificationEventArgs.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The event data for the JobError event.</summary>
    public class JobErrorNotificationEventArgs : JobNotificationEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="JobErrorNotificationEventArgs" /> class.</summary>
        /// <param name="error">The error.</param>
        internal JobErrorNotificationEventArgs(BitsError error)
        {
            this.Error = error;
        }

        /// <summary>Gets the error.</summary>
        /// <value>The error that occurred.</value>
        public BitsError Error { get; private set; }
    }
}