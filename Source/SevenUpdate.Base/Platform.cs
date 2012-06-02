// <copyright file="Platform.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The current status of the update.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(X86)]
    public enum Platform
    {
        /// <summary>Indicates that the application is native 32 bit.</summary>
        [ProtoEnum]
        [EnumMember]
        X86 = 0, 

        /// <summary>Indicates that the application can only run on 64 bit platforms.</summary>
        [ProtoEnum]
        [EnumMember]
        X64 = 1, 

        /// <summary>Indicates that the application can run on 32bit or 64bit natively depending on the OS.</summary>
        [ProtoEnum]
        [EnumMember]
        AnyCpu = 2, 
    }
}