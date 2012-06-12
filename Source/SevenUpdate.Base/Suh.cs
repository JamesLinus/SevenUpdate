// <copyright file="Suh.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Information about an update, used by History and Hidden Updates. Not used by the SDK.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(UpdateStatus))]
    [KnownType(typeof(Importance))]
    [KnownType(typeof(ObservableCollection<LocaleString>))]
    public sealed class Suh : INotifyPropertyChanged
    {
        /// <summary>The <c>Uri</c> for the application's website.</summary>
        string appUrl;

        /// <summary>The help website <c>Uri</c> of the application.</summary>
        string helpUrl;

        /// <summary>The importance of the update.</summary>
        Importance importance;

        /// <summary>The url pointing to a resource to find more information about the update.</summary>
        string infoUrl;

        /// <summary>The formatted date string when the update was installed.</summary>
        string installDate;

        /// <summary>The formatted date string depicting the release date of the update.</summary>
        string releaseDate;

        /// <summary>The current status of the update.</summary>
        UpdateStatus status;

        /// <summary>The total download size in bytes of the update.</summary>
        ulong updateSize;

        /// <summary>Initializes a new instance of the <see cref="Suh" /> class.</summary>
        /// <param name="name">The collection of localized update names.</param>
        /// <param name="publisher">The collection of localized publisher names.</param>
        /// <param name="description">The collection of localized update descriptions.</param>
        public Suh(
            ObservableCollection<LocaleString> name, 
            ObservableCollection<LocaleString> publisher, 
            ObservableCollection<LocaleString> description)
        {
            this.Name = name;
            this.Description = description;
            this.Publisher = publisher;

            if (this.Name == null)
            {
                this.Name = new ObservableCollection<LocaleString>();
            }

            if (this.Description == null)
            {
                this.Description = new ObservableCollection<LocaleString>();
            }

            if (this.Publisher == null)
            {
                this.Publisher = new ObservableCollection<LocaleString>();
            }
        }

        /// <summary>Initializes a new instance of the <see cref="Suh" /> class.</summary>
        public Suh()
        {
            this.Name = new ObservableCollection<LocaleString>();
            this.Publisher = new ObservableCollection<LocaleString>();
            this.Description = new ObservableCollection<LocaleString>();
        }

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the <c>Uri</c> for the application's website.</summary>
        /// <value>The application website.</value>
        [ProtoMember(8)]
        [DataMember]
        public string AppUrl
        {
            get { return this.appUrl; }

            set
            {
                this.appUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("PublisherUrl");
            }
        }

        /// <summary>Gets the collection localized update descriptions.</summary>
        /// <value>The localized description for the update.</value>
        [ProtoMember(2)]
        [DataMember]
        public ObservableCollection<LocaleString> Description { get; private set; }

        /// <summary>Gets or sets the help website <c>Uri</c> of the application.</summary>
        /// <value>The help and support website for the application.</value>
        [ProtoMember(9, IsRequired = false)]
        [DataMember]
        public string HelpUrl
        {
            get { return this.helpUrl; }

            set
            {
                this.helpUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("HelpUrl");
            }
        }

        /// <summary>Gets or sets the importance of the update.</summary>
        /// <value>The importance.</value>
        [ProtoMember(3)]
        [DataMember]
        public Importance Importance
        {
            get { return this.importance; }

            set
            {
                this.importance = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Importance");
            }
        }

        /// <summary>Gets or sets the url pointing to a resource to find more information about the update.</summary>
        /// <value>The info URL.</value>
        [ProtoMember(10, IsRequired = false)]
        [DataMember]
        public string InfoUrl
        {
            get { return this.infoUrl; }

            set
            {
                this.infoUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("InfoUrl");
            }
        }

        /// <summary>Gets or sets the formatted date string when the update was installed.</summary>
        /// <value>The formatted install date string (MM/DD/YYYY).</value>
        [ProtoMember(11)]
        [DataMember]
        public string InstallDate
        {
            get { return this.installDate; }

            set
            {
                this.installDate = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("InstallDate");
            }
        }

        /// <summary>Gets the collection of localized update names.</summary>
        /// <value>The localized update names.</value>
        [ProtoMember(1)]
        [DataMember]
        public ObservableCollection<LocaleString> Name { get; private set; }

        /// <summary>Gets the collection of localized publisher names.</summary>
        /// <value>The publisher.</value>
        [ProtoMember(7)]
        [DataMember]
        public ObservableCollection<LocaleString> Publisher { get; private set; }

        /// <summary>Gets or sets the formatted date string depicting the release date of the update.</summary>
        /// <value>The release date in a formatted string MM/DD/YYYY.</value>
        [ProtoMember(5)]
        [DataMember]
        public string ReleaseDate
        {
            get { return this.releaseDate; }

            set
            {
                this.releaseDate = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ReleaseDate");
            }
        }

        /// <summary>Gets or sets the current status of the update.</summary>
        /// <value>The status.</value>
        [ProtoMember(4)]
        [DataMember]
        public UpdateStatus Status
        {
            get { return this.status; }

            set
            {
                this.status = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Status");
            }
        }

        /// <summary>Gets or sets the total download size in bytes of the update.</summary>
        /// <value>The total download size of the update.</value>
        [ProtoMember(6)]
        [DataMember]
        public ulong UpdateSize
        {
            get { return this.updateSize; }

            set
            {
                this.updateSize = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("UpdateSize");
            }
        }

        /// <summary>When a property has changed, call the <c>OnPropertyChanged</c> Event.</summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}