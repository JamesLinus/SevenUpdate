// <copyright file="DownloadProgressChangedEventArgs.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the DownloadProgressChanged event.</summary>
    [ProtoContract]
    [DataContract]
    public sealed class DownloadProgressChangedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="DownloadProgressChangedEventArgs" /> class.</summary>
        /// <param name="bytesTransferred">The number of bytes transferred.</param>
        /// <param name="bytesTotal">The total number of bytes to download.</param>
        /// <param name="filesTransferred">The number of files transfered.</param>
        /// <param name="filesTotal">The total number of files transfered.</param>
        public DownloadProgressChangedEventArgs(
            ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal)
        {
            this.BytesTotal = bytesTotal;
            this.BytesTransferred = bytesTransferred;
            this.FilesTotal = filesTotal;
            this.FilesTransferred = filesTransferred;
        }

        /// <summary>Initializes a new instance of the <see cref="DownloadProgressChangedEventArgs" /> class.</summary>
        public DownloadProgressChangedEventArgs()
        {
        }

        /// <summary>Gets the total number of bytes to download.</summary>
        /// <value>The bytes total.</value>
        [ProtoMember(1)]
        [DataMember]
        public ulong BytesTotal { get; private set; }

        /// <summary>Gets the number of bytes transferred.</summary>
        /// <value>The bytes transferred.</value>
        [ProtoMember(2)]
        [DataMember]
        public ulong BytesTransferred { get; private set; }

        /// <summary>Gets the total number of files to download.</summary>
        /// <value>The files total.</value>
        [ProtoMember(3)]
        [DataMember]
        public uint FilesTotal { get; private set; }

        /// <summary>Gets the number of files downloaded.</summary>
        /// <value>The files transferred.</value>
        [ProtoMember(4)]
        [DataMember]
        public uint FilesTransferred { get; private set; }
    }
}