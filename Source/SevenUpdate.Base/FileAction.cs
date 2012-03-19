// <copyright file="FileAction.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The action to perform on the file.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Update)]
    public enum FileAction
    {
        /// <summary>Updates a file.</summary>
        [ProtoEnum]
        [EnumMember]
        Update = 0, 

        /// <summary>Updates a file, only if it exist.</summary>
        [ProtoEnum]
        [EnumMember]
        UpdateIfExist = 1, 

        /// <summary>Updates a file, then registers the dll.</summary>
        [ProtoEnum]
        [EnumMember]
        UpdateThenRegister = 2, 

        /// <summary>Updates a file, then executes it.</summary>
        [ProtoEnum]
        [EnumMember]
        UpdateThenExecute = 3, 

        /// <summary>Compares a file, but does not update it.</summary>
        [ProtoEnum]
        [EnumMember]
        CompareOnly = 4, 

        /// <summary>Executes a file, can be on system or be downloaded.</summary>
        [ProtoEnum]
        [EnumMember]
        Execute = 5, 

        /// <summary>Deletes a file.</summary>
        [ProtoEnum]
        [EnumMember]
        Delete = 6, 

        /// <summary>Executes a file, then deletes it.</summary>
        [ProtoEnum]
        [EnumMember]
        ExecuteThenDelete = 7, 

        /// <summary>Unregisters a dll, then deletes it.</summary>
        [ProtoEnum]
        [EnumMember]
        UnregisterThenDelete = 8, 
    }
}