// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class used to define an exception raised when something went bad during a <see cref="InstanceAwareApplication"/> startup.
    /// </summary>
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class UnexpectedInstanceAwareApplicationException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UnexpectedInstanceAwareApplicationException" /> class.
        /// </summary>
        public UnexpectedInstanceAwareApplicationException()
            : base("Unexpected exception while starting up instance aware application")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstanceAwareApplicationException"/> class.
        /// </summary>
        /// <parameter name="message">
        /// The message.
        /// </parameter>
        public UnexpectedInstanceAwareApplicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstanceAwareApplicationException"/> class.
        /// </summary>
        /// <parameter name="message">
        /// The message.
        /// </parameter>
        /// <parameter name="inner">
        /// The inner.
        /// </parameter>
        public UnexpectedInstanceAwareApplicationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstanceAwareApplicationException"/> class.
        /// </summary>
        /// <parameter name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </parameter>
        /// <parameter name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
        /// </parameter>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <parameterref name="info"/> parameter is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is <see langword="null"/> or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected UnexpectedInstanceAwareApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }

    /// <summary>
    /// Class used to define an exception raised when no application <see cref="Guid"/> was defined.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Exception class")]
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class UndefinedApplicationGuidException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UndefinedApplicationGuidException" /> class.
        /// </summary>
        public UndefinedApplicationGuidException()
            : base("No application Guid was defined")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedApplicationGuidException"/> class.
        /// </summary>
        /// <parameter name="message">
        /// The message.
        /// </parameter>
        public UndefinedApplicationGuidException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedApplicationGuidException"/> class.
        /// </summary>
        /// <parameter name="message">
        /// The message.
        /// </parameter>
        /// <parameter name="inner">
        /// The inner.
        /// </parameter>
        public UndefinedApplicationGuidException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedApplicationGuidException"/> class.
        /// </summary>
        /// <parameter name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </parameter>
        /// <parameter name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
        /// </parameter>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <parameterref name="info"/> parameter is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is <see langword="null"/> or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected UndefinedApplicationGuidException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}