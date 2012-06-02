// <copyright file="ErrorType.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Indicates a type of error that can occur.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(GeneralError)]
    public enum ErrorType
    {
        /// <summary>An error that occurred while trying to download updates.</summary>
        [ProtoEnum]
        [EnumMember]
        DownloadError, 

        /// <summary>An error that occurred while trying to install updates.</summary>
        [ProtoEnum]
        [EnumMember]
        InstallationError, 

        /// <summary>A general network connection error.</summary>
        [ProtoEnum]
        [EnumMember]
        FatalNetworkError, 

        /// <summary>An unspecified error, non fatal.</summary>
        [ProtoEnum]
        [EnumMember]
        GeneralError, 

        /// <summary>An unspecified error that prevents Seven Update from continuing.</summary>
        [ProtoEnum]
        [EnumMember]
        FatalError, 

        /// <summary>An error that occurs while searching for updates.</summary>
        [ProtoEnum]
        [EnumMember]
        SearchError
    }
}