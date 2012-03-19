// <copyright file="RegistryAction.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Contains the Actions you can perform to the registry.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Add)]
    public enum RegistryAction
    {
        /// <summary>Adds a registry entry to the machine.</summary>
        [ProtoEnum]
        [EnumMember]
        Add = 0, 

        /// <summary>Deletes a registry key on the machine.</summary>
        [ProtoEnum]
        [EnumMember]
        DeleteKey = 1, 

        /// <summary>Deletes a value of a registry key on the machine.</summary>
        [ProtoEnum]
        [EnumMember]
        DeleteValue = 2
    }
}