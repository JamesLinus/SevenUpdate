// <copyright file="ErrorNotificationEventArgs.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The event data for the ErrorNotification event.</summary>
    public class ErrorNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="ErrorNotificationEventArgs" /> class.</summary>
        /// <param name="job">The job the notification is for.</param>
        /// <param name="error">The error that occurred.</param>
        internal ErrorNotificationEventArgs(BitsJob job, BitsError error) : base(job)
        {
            this.Error = error;
        }

        /// <summary>Gets the error.</summary>
        /// <value>The error that occurred.</value>
        public BitsError Error { get; private set; }
    }
}