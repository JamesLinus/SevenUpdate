// <copyright file="ErrorOccurredEventArgs.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the ErrorOccurred event.</summary>
    [ProtoContract]
    [DataContract]
    public sealed class ErrorOccurredEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="ErrorOccurredEventArgs" /> class.</summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="type">The type of error that occurred.</param>
        public ErrorOccurredEventArgs(string exception, ErrorType type)
        {
            this.Exception = exception;
            this.ErrorType = type;
        }

        /// <summary>Initializes a new instance of the <see cref="ErrorOccurredEventArgs" /> class.</summary>
        public ErrorOccurredEventArgs()
        {
        }

        /// <summary>Gets the <c>ErrorType</c> of the error that occurred.</summary>
        /// <value>The type of error that occurred.</value>
        [ProtoMember(1)]
        [DataMember]
        public ErrorType ErrorType { get; private set; }

        /// <summary>Gets the Exception information of the error that occurred.</summary>
        /// <value>The exception.</value>
        [ProtoMember(2)]
        [DataMember]
        public string Exception { get; private set; }
    }
}