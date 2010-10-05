// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace Microsoft.Windows
{
    using System;
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
        /// <param name="message">
        /// The message.
        /// </param>
        public UnexpectedInstanceAwareApplicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstanceAwareApplicationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public UnexpectedInstanceAwareApplicationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstanceAwareApplicationException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is <see langword="null"/>.
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
        /// <param name="message">
        /// The message.
        /// </param>
        public UndefinedApplicationGuidException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedApplicationGuidException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public UndefinedApplicationGuidException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedApplicationGuidException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is <see langword="null"/>.
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