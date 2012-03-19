// <copyright file="AutoUpdateOption.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Automatic Update option Seven Update can use.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Install)]
    public enum AutoUpdateOption
    {
        /// <summary>Download and Installs updates automatically.</summary>
        [ProtoEnum]
        [EnumMember]
        Install = 0, 

        /// <summary>Downloads Updates automatically.</summary>
        [ProtoEnum]
        [EnumMember]
        Download = 1, 

        /// <summary>Only checks and notifies the user of updates.</summary>
        [ProtoEnum]
        [EnumMember]
        Notify = 2, 

        /// <summary>No automatic checking.</summary>
        [ProtoEnum]
        [EnumMember]
        Never = 3
    }
}