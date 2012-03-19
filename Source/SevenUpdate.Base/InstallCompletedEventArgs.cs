// <copyright file="InstallCompletedEventArgs.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the InstallCompleted event.</summary>
    [ProtoContract]
    [DataContract]
    public sealed class InstallCompletedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="InstallCompletedEventArgs" /> class.</summary>
        /// <param name="updatesInstalled">The number of updates installed.</param>
        /// <param name="updatesFailed">The number of updates that failed.</param>
        public InstallCompletedEventArgs(int updatesInstalled, int updatesFailed)
        {
            this.UpdatesInstalled = updatesInstalled;
            this.UpdatesFailed = updatesFailed;
        }

        /// <summary>Initializes a new instance of the <see cref="InstallCompletedEventArgs" /> class.</summary>
        public InstallCompletedEventArgs()
        {
        }

        /// <summary>Gets the number of updates that failed.</summary>
        /// <value>The updates failed.</value>
        [ProtoMember(1)]
        [DataMember]
        public int UpdatesFailed { get; private set; }

        /// <summary>Gets the number of updates that have been installed.</summary>
        /// <value>The updates installed.</value>
        [ProtoMember(2)]
        [DataMember]
        public int UpdatesInstalled { get; private set; }
    }
}