// ***********************************************************************
// <copyright file="JobErrorNotificationEventArgs.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>The event data for the JobError event.</summary>
    public class JobErrorNotificationEventArgs : JobNotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="JobErrorNotificationEventArgs" /> class.</summary>
        /// <param name="error">The error.</param>
        internal JobErrorNotificationEventArgs(BitsError error)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>Gets the error.</summary>
        /// <value>The error that occurred.</value>
        public BitsError Error { get; private set; }

        #endregion
    }
}