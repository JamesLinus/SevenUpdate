// <copyright file="NotificationEventArgs.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The event data for the Notification event.</summary>
    public class NotificationEventArgs : JobNotificationEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="NotificationEventArgs" /> class.</summary>
        /// <param name="job">The job the event occurred for.</param>
        internal NotificationEventArgs(BitsJob job)
        {
            this.Job = job;
        }

        /// <summary>Gets the job.</summary>
        /// <value>The <c>BitsJob</c> the notification occurred for.</value>
        public BitsJob Job { get; private set; }
    }
}