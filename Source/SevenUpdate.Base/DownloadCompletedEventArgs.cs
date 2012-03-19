// <copyright file="DownloadCompletedEventArgs.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the DownloadCompleted event.</summary>
    [ProtoContract]
    [DataContract]
    public sealed class DownloadCompletedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="DownloadCompletedEventArgs" /> class.</summary>
        /// <param name="errorOccurred"><c>True</c> if an error occurred, otherwise <c>False</c>.</param>
        public DownloadCompletedEventArgs(bool errorOccurred)
        {
            this.ErrorOccurred = errorOccurred;
        }

        /// <summary>Initializes a new instance of the <see cref="DownloadCompletedEventArgs" /> class.</summary>
        public DownloadCompletedEventArgs()
        {
        }

        /// <summary>Gets a value indicating whether an error occurred.</summary>
        /// <value><c>True</c> if an error occurred otherwise, <c>False</c>.</value>
        [ProtoMember(1, IsRequired = false)]
        [DataMember]
        public bool ErrorOccurred { get; private set; }
    }
}