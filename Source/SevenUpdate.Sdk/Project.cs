// <copyright file="Project.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Contains data specifying the application name and it's updates.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Sua))]
    [KnownType(typeof(ObservableCollection<Update>))]
    public class Project : INotifyPropertyChanged
    {
        /// <summary>The collection of localized update names.</summary>
        readonly ObservableCollection<string> updateNames = new ObservableCollection<string>();

        /// <summary>The localized name of the application.</summary>
        string applicationName;

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the localized application name.</summary>
        /// <value>The name of the application.</value>
        [ProtoMember(1)]
        [DataMember]
        public string ApplicationName
        {
            get { return this.applicationName; }

            set
            {
                this.applicationName = value;
                this.OnPropertyChanged("ApplicationName");
            }
        }

        /// <summary>Gets or sets the last used sua filename when the project was exported.</summary>
        [ProtoMember(3)]
        [DataMember]
        public string ExportedSuaFileName { get; set; }

        /// <summary>Gets or sets the last used sui filename when the project was exported.</summary>
        [ProtoMember(4)]
        [DataMember]
        public string ExportedSuiFileName { get; set; }

        /// <summary>Gets the update names.</summary>
        /// <value>The update names.</value>
        [ProtoMember(2)]
        [DataMember]
        public ObservableCollection<string> UpdateNames
        {
            get { return this.updateNames; }
        }

        /// <summary>When a property has changed, call the <c>OnPropertyChanged</c> Event.</summary>
        /// <param name="name">The name of the property changed.</param>
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