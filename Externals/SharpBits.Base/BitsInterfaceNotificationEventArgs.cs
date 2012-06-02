// <copyright file="BitsInterfaceNotificationEventArgs.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The event data for the interface notification event.</summary>
    public class BitsInterfaceNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>The Com exception.</summary>
        private readonly COMException exception;

        /// <summary>Initializes a new instance of the <see cref="BitsInterfaceNotificationEventArgs" /> class.</summary>
        /// <param name="job">The job the notification is for.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="description">The description.</param>
        internal BitsInterfaceNotificationEventArgs(BitsJob job, COMException exception, string description) : base(job)
        {
            this.Description = description;
            this.exception = exception;
        }

        /// <summary>Gets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>Gets the message.</summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return this.exception.Message; }
        }

        /// <summary>Gets the error code.</summary>
        /// <value>The error code.</value>
        public int Result
        {
            get { return this.exception.ErrorCode; }
        }
    }
}