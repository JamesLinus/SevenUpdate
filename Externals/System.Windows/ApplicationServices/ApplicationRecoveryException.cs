// ***********************************************************************
// <copyright file="ApplicationRecoveryException.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    /// <summary>
    ///   This exception is thrown when there are problems with registering, unregistering or updating applications
    ///   using Application Restart Recovery.
    /// </summary>
    [Serializable]
    public class ApplicationRecoveryException : ExternalException
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ApplicationRecoveryException" /> class.</summary>
        public ApplicationRecoveryException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ApplicationRecoveryException" /> class.</summary>
        /// <param name="message">A custom message for the exception.</param>
        public ApplicationRecoveryException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ApplicationRecoveryException" /> class.</summary>
        /// <param name="message">A custom message for the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public ApplicationRecoveryException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }

        /// <summary>Initializes a new instance of the <see cref="ApplicationRecoveryException" /> class.</summary>
        /// <param name="message">A custom message for the exception.</param>
        /// <param name="errorCode">An error code (hresult) from which to generate the exception.</param>
        public ApplicationRecoveryException(string message, int errorCode)
            : base(message, errorCode)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ApplicationRecoveryException" /> class.</summary>
        /// <param name="info">Serialization info from which to create exception.</param>
        /// <param name="context">Streaming context from which to create exception.</param>
        protected ApplicationRecoveryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }

        #endregion
    }
}
