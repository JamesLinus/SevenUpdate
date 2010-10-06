// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : sevenalive
// Created          : 10-05-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    /// The event data for the JobError event
    /// </summary>
    public class JobErrorNotificationEventArgs : JobNotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobErrorNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        internal JobErrorNotificationEventArgs(BitsError error)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error that occurred</value>
        public BitsError Error { get; private set; }

        #endregion
    }
}