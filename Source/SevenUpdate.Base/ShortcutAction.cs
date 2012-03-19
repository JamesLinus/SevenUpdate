// <copyright file="ShortcutAction.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The action to preform on the shortcut.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Add)]
    public enum ShortcutAction
    {
        /// <summary>Adds a shortcut.</summary>
        [ProtoEnum]
        [EnumMember]
        Add = 0, 

        /// <summary>Updates a shortcut only if it exists.</summary>
        [ProtoEnum]
        [EnumMember]
        Update = 1, 

        /// <summary>Deletes a shortcut.</summary>
        [ProtoEnum]
        [EnumMember]
        Delete = 2
    }
}