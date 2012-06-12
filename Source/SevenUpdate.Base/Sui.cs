// <copyright file="Sui.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The collection of updates and the application info.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Sua))]
    [KnownType(typeof(ObservableCollection<Update>))]
    public sealed class Sui : INotifyPropertyChanged
    {
        /// <summary>The application information.</summary>
        Sua appInfo;

        /// <summary>Initializes a new instance of the <see cref="Sui" /> class.</summary>
        /// <param name="updates">The collection of updates for the application.</param>
        public Sui(ObservableCollection<Update> updates)
        {
            this.Updates = updates;

            if (this.Updates != null)
            {
                return;
            }

            this.Updates = new ObservableCollection<Update>();
        }

        /// <summary>Initializes a new instance of the <see cref="Sui" /> class.</summary>
        public Sui()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Sui" /> class.</summary>
        /// <param name="appInfo">The software information for the application updates.</param>
        /// <param name="updates">The collection of updates for the application.</param>
        public Sui(Sua appInfo, ObservableCollection<Update> updates)
        {
            this.AppInfo = appInfo;
            this.Updates = updates;
        }

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the software information for the application updates.</summary>
        [ProtoMember(2)]
        [DataMember]
        public Sua AppInfo
        {
            get { return this.appInfo; }

            set
            {
                this.appInfo = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AppInfo");
            }
        }

        /// <summary>Gets the collection of updates for the application.</summary>
        [ProtoMember(1)]
        [DataMember]
        public ObservableCollection<Update> Updates { get; private set; }

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