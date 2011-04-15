// ***********************************************************************
// <copyright file="ErrorNotificationEventArgs.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>The event data for the ErrorNotification event</summary>
    public class ErrorNotificationEventArgs : NotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ErrorNotificationEventArgs" /> class.</summary>
        /// <param name="job">The job the notification is for</param>
        /// <param name="error">The error that occurred</param>
        internal ErrorNotificationEventArgs(BitsJob job, BitsError error)
            : base(job)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>Gets the error.</summary>
        /// <value>The error that occurred</value>
        public BitsError Error { get; private set; }

        #endregion
    }
}