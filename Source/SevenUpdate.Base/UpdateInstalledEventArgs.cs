// <copyright file="UpdateInstalledEventArgs.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the InstallProgressChanged event.</summary>
    [ProtoContract]
    [DataContract]
    public sealed class UpdateInstalledEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="UpdateInstalledEventArgs" /> class.</summary>
        /// <param name="update">The update information that was installed.</param>
        public UpdateInstalledEventArgs(Suh update)
        {
            this.Update = update;
        }

        /// <summary>Initializes a new instance of the <see cref="UpdateInstalledEventArgs" /> class.</summary>
        public UpdateInstalledEventArgs()
        {
        }

        /// <summary>Gets the update information that was installed.</summary>
        [ProtoMember(1)]
        [DataMember]
        public Suh Update { get; private set; }
    }
}