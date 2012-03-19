// <copyright file="UpdateStatus.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The current status of the update.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Successful)]
    public enum UpdateStatus
    {
        /// <summary>Indicates that the update installation failed.</summary>
        [ProtoEnum]
        [EnumMember]
        Failed = 0, 

        /// <summary>Indicates that the update is hidden.</summary>
        [ProtoEnum]
        [EnumMember]
        Hidden = 1, 

        /// <summary>Indicates that the update is visible.</summary>
        [ProtoEnum]
        [EnumMember]
        Visible = 2, 

        /// <summary>Indicates that the update installation succeeded.</summary>
        [ProtoEnum]
        [EnumMember]
        Successful = 3
    }
}