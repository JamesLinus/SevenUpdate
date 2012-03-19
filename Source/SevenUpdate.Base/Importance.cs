// <copyright file="Importance.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Contains the UpdateType of the update.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Important)]
    public enum Importance
    {
        /// <summary>Important update.</summary>
        [ProtoEnum]
        [EnumMember]
        Important = 0, 

        /// <summary>Locale or language.</summary>
        [ProtoEnum]
        [EnumMember]
        Locale = 1, 

        /// <summary>Optional update.</summary>
        [ProtoEnum]
        [EnumMember]
        Optional = 2, 

        /// <summary>Recommended update.</summary>
        [ProtoEnum]
        [EnumMember]
        Recommended = 3
    }
}