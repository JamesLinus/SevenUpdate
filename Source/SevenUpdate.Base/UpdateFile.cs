// <copyright file="UpdateFile.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Information about a file within an update.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(FileAction))]
    public sealed class UpdateFile : INotifyPropertyChanged
    {
        /// <summary>The action to perform on the <c>UpdateFile</c>.</summary>
        FileAction action;

        /// <summary>The command line arguments to execute with the file.</summary>
        string args;

        /// <summary>The location where the file will be installed.</summary>
        string destination;

        /// <summary>The size of the file in bytes.</summary>
        ulong fileSize;

        /// <summary>The SHA-2 hash of the file.</summary>
        string hash;

        /// <summary>The download location for the file.</summary>
        string source;

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the action to perform on the <c>UpdateFile</c>.</summary>
        /// <value>The action.</value>
        [ProtoMember(1)]
        [DataMember]
        public FileAction Action
        {
            get { return this.action; }

            set
            {
                this.action = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Action");
            }
        }

        /// <summary>Gets or sets the command line arguments to execute with the file.</summary>
        /// <value>The arguments.</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string Args
        {
            get { return this.args; }

            set
            {
                this.args = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Args");
            }
        }

        /// <summary>Gets or sets the location where the file will be installed.</summary>
        /// <value>The destination.</value>
        [ProtoMember(3)]
        [DataMember]
        public string Destination
        {
            get { return this.destination; }

            set
            {
                this.destination = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Destination");
            }
        }

        /// <summary>Gets or sets the size of the file in bytes.</summary>
        /// <value>The size of the file.</value>
        [ProtoMember(5)]
        [DataMember]
        public ulong FileSize
        {
            get { return this.fileSize; }

            set
            {
                this.fileSize = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("FileSize");
            }
        }

        /// <summary>Gets or sets the SHA-2 hash of the file.</summary>
        /// <value>The SHA-2 hash of the file.</value>
        [ProtoMember(4)]
        [DataMember]
        public string Hash
        {
            get { return this.hash; }

            set
            {
                this.hash = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Hash");
            }
        }

        /// <summary>Gets or sets the location for the file within the gzip file.</summary>
        /// <value>The download location of the file.</value>
        [ProtoMember(2)]
        [DataMember]
        public string Source
        {
            get { return this.source; }

            set
            {
                this.source = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Source");
            }
        }

        /// <summary>When a property has changed, call the <c>OnPropertyChanged</c> Event.</summary>
        /// <param name="name">The name of the property that changed.</param>
        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}