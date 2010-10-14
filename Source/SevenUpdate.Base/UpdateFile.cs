// ***********************************************************************
// <copyright file="UpdateFile.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The action to perform on the file</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Update)]
    public enum FileAction
    {
        /// <summary>Updates a file</summary>
        [ProtoEnum]
        [EnumMember]
        Update = 0,

        /// <summary>Updates a file, only if it exist</summary>
        [ProtoEnum]
        [EnumMember]
        UpdateIfExist = 1,

        /// <summary>Updates a file, then registers the dll</summary>
        [ProtoEnum]
        [EnumMember]
        UpdateThenRegister = 2,

        /// <summary>Updates a file, then executes it</summary>
        [ProtoEnum]
        [EnumMember]
        UpdateThenExecute = 3,

        /// <summary>Compares a file, but does not update it</summary>
        [ProtoEnum]
        [EnumMember]
        CompareOnly = 4,

        /// <summary>Executes a file, can be on system or be downloaded</summary>
        [ProtoEnum]
        [EnumMember]
        Execute = 5,

        /// <summary>Deletes a file</summary>
        [ProtoEnum]
        [EnumMember]
        Delete = 6,

        /// <summary>Executes a file, then deletes it</summary>
        [ProtoEnum]
        [EnumMember]
        ExecuteThenDelete = 7,

        /// <summary>Unregisters a dll, then deletes it</summary>
        [ProtoEnum]
        [EnumMember]
        UnregisterThenDelete = 8,
    }

    /// <summary>Information about a file within an update</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(FileAction))]
    public sealed class UpdateFile : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The action to perform on the <see cref = "UpdateFile" /></summary>
        private FileAction action;

        /// <summary>The command line arguments to execute with the file</summary>
        private string args;

        /// <summary>The location where the file will be installed</summary>
        private string destination;

        /// <summary>The size of the file in bytes</summary>
        private ulong fileSize;

        /// <summary>The SHA-2 hash of the file</summary>
        private string hash;

        /// <summary>The download location for the file</summary>
        private string source;

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets the action to perform on the <see cref = "UpdateFile" /></summary>
        /// <value>The action.</value>
        [ProtoMember(1)]
        [DataMember]
        public FileAction Action
        {
            get
            {
                return this.action;
            }

            set
            {
                this.action = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Action");
            }
        }

        /// <summary>Gets or sets the command line arguments to execute with the file</summary>
        /// <value>The arguments</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string Args
        {
            get
            {
                return this.args;
            }

            set
            {
                this.args = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Args");
            }
        }

        /// <summary>Gets or sets the location where the file will be installed</summary>
        /// <value>The destination.</value>
        [ProtoMember(3)]
        [DataMember]
        public string Destination
        {
            get
            {
                return this.destination;
            }

            set
            {
                this.destination = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Destination");
            }
        }

        /// <summary>Gets or sets the size of the file in bytes</summary>
        /// <value>The size of the file.</value>
        [ProtoMember(5)]
        [DataMember]
        public ulong FileSize
        {
            get
            {
                return this.fileSize;
            }

            set
            {
                this.fileSize = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("FileSize");
            }
        }

        /// <summary>Gets or sets the SHA-2 hash of the file</summary>
        /// <value>The SHA-2 hash of the file.</value>
        [ProtoMember(4)]
        [DataMember]
        public string Hash
        {
            get
            {
                return this.hash;
            }

            set
            {
                this.hash = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Hash");
            }
        }

        /// <summary>Gets or sets the download location for the file</summary>
        /// <value>The download location of the file</value>
        [ProtoMember(2)]
        [DataMember]
        public string Source
        {
            get
            {
                return this.source;
            }

            set
            {
                this.source = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Source");
            }
        }

        #endregion

        #region Methods

        /// <summary>When a property has changed, call the <see cref="OnPropertyChanged"/> Event</summary>
        /// <param name="name">The name of the property that changed</param>
        private void OnPropertyChanged(string name)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}