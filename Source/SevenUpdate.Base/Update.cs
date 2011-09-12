// ***********************************************************************
// <copyright file="Update.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Information on how to install a software update.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(ObservableCollection<LocaleString>))]
    [KnownType(typeof(UpdateFile))]
    [KnownType(typeof(RegistryItem))]
    [KnownType(typeof(Shortcut))]
    [KnownType(typeof(Importance))]
    public sealed class Update : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The source main location to download files for the update.</summary>
        private string downloadUrl;

        /// <summary>Indicates if the update is hidden.</summary>
        private bool hidden;

        /// <summary>Indicates the importance type of the update.</summary>
        private Importance importance;

        /// <summary>The Uri pointing to a resource to find more information about the update.</summary>
        private string infoUrl;

        /// <summary>The Uri pointing to the software license for the application/update.</summary>
        private string licenseUrl;

        /// <summary>The formatted date string depicting the release date of the update.</summary>
        private string releaseDate;

        /// <summary>Indicates if the update is selected.</summary>
        private bool selected;

        /// <summary>The total download size in bytes of the update.</summary>
        private ulong size;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <c>Update</c> class.</summary>
        /// <param name="name">The collection of localized update names.</param>
        /// <param name="description">The collection localized update descriptions.</param>
        /// <param name="files">The collection of localized update files.</param>
        /// <param name="registryItems">The collection of registry keys and values to perform actions on in the update.</param>
        /// <param name="shortcuts">The collection of shortcuts to perform actions on in the update.</param>
        public Update(
            ObservableCollection<LocaleString> name,
            ObservableCollection<LocaleString> description,
            ObservableCollection<UpdateFile> files,
            ObservableCollection<RegistryItem> registryItems,
            ObservableCollection<Shortcut> shortcuts)
        {
            this.Name = name;
            this.Description = description;
            this.Shortcuts = shortcuts;
            this.RegistryItems = registryItems;
            this.Files = files;

            if (this.Name == null)
            {
                this.Name = new ObservableCollection<LocaleString>();
            }

            if (this.Description == null)
            {
                this.Description = new ObservableCollection<LocaleString>();
            }

            if (this.Shortcuts == null)
            {
                this.Shortcuts = new ObservableCollection<Shortcut>();
            }

            if (this.RegistryItems == null)
            {
                this.RegistryItems = new ObservableCollection<RegistryItem>();
            }

            if (this.Files == null)
            {
                this.Files = new ObservableCollection<UpdateFile>();
            }

            this.Name.CollectionChanged -= this.NameCollectionChanged;
            this.Description.CollectionChanged -= this.DescriptionCollectionChanged;

            this.Name.CollectionChanged += this.NameCollectionChanged;
            this.Description.CollectionChanged += this.DescriptionCollectionChanged;
        }

        /// <summary>Initializes a new instance of the Update class.</summary>
        public Update()
        {
            this.Name = new ObservableCollection<LocaleString>();
            this.Description = new ObservableCollection<LocaleString>();
            this.Files = new ObservableCollection<UpdateFile>();
            this.RegistryItems = new ObservableCollection<RegistryItem>();
            this.Shortcuts = new ObservableCollection<Shortcut>();

            this.Name.CollectionChanged -= this.NameCollectionChanged;
            this.Description.CollectionChanged -= this.DescriptionCollectionChanged;
            this.Name.CollectionChanged += this.NameCollectionChanged;
            this.Description.CollectionChanged += this.DescriptionCollectionChanged;
        }

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets the collection of localized update descriptions.</summary>
        /// <value>The localized description for the update.</value>
        [ProtoMember(2)]
        [DataMember]
        public ObservableCollection<LocaleString> Description { get; private set; }

        /// <summary>Gets or sets the source main location to download files for the update.</summary>
        /// <value>The url to download the update files.</value>
        [ProtoMember(3)]
        [DataMember]
        public string DownloadUrl
        {
            get
            {
                return this.downloadUrl;
            }

            set
            {
                this.downloadUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("DownloadUrl");
            }
        }

        /// <summary>Gets the collection of files to perform actions on in the update.</summary>
        /// <value>The files.</value>
        [ProtoMember(8, IsRequired = false)]
        [DataMember]
        public ObservableCollection<UpdateFile> Files { get; private set; }

        /// <summary>Gets or sets a value indicating whether the update is hidden.</summary>
        /// <value><c>True</c> if hidden; otherwise, <c>False</c>.</value>
        [ProtoIgnore]
        [IgnoreDataMember]
        public bool Hidden
        {
            get
            {
                return this.hidden;
            }

            set
            {
                this.hidden = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Hidden");
            }
        }

        /// <summary>Gets or sets the importance of the update.</summary>
        /// <value>The importance.</value>
        [ProtoMember(4)]
        [DataMember]
        public Importance Importance
        {
            get
            {
                return this.importance;
            }

            set
            {
                this.importance = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Importance");
            }
        }

        /// <summary>Gets or sets the url pointing to a resource to find more information about the update.</summary>
        /// <value>The info URL.</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string InfoUrl
        {
            get
            {
                return this.infoUrl;
            }

            set
            {
                this.infoUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("InfoUrl");
            }
        }

        /// <summary>Gets or sets the url pointing to the software license for the application/update.</summary>
        /// <value>The url pointing to the software license.</value>
        [ProtoMember(7, IsRequired = false)]
        [DataMember]
        public string LicenseUrl
        {
            get
            {
                return this.licenseUrl;
            }

            set
            {
                this.licenseUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("LicenseUrl");
            }
        }

        /// <summary>Gets the collection of localized update names.</summary>
        /// <value>The localized update names.</value>
        [ProtoMember(1)]
        [DataMember]
        public ObservableCollection<LocaleString> Name { get; private set; }

        /// <summary>Gets the collection of registry keys and values to perform actions on in the update.</summary>
        /// <value>The registry items.</value>
        [ProtoMember(9, IsRequired = false)]
        [DataMember]
        public ObservableCollection<RegistryItem> RegistryItems { get; private set; }

        /// <summary>Gets or sets the formatted date string depicting the release date of the update.</summary>
        /// <value>The release date in a formatted string MM/DD/YYYY.</value>
        [ProtoMember(5)]
        [DataMember]
        public string ReleaseDate
        {
            get
            {
                return this.releaseDate;
            }

            set
            {
                this.releaseDate = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ReleaseDate");
            }
        }

        /// <summary>Gets or sets a value indicating whether the update is selected (not used in the SDK).</summary>
        /// <value><c>True</c> if selected; otherwise, <c>False</c>.</value>
        [ProtoIgnore]
        [IgnoreDataMember]
        public bool Selected
        {
            get
            {
                return this.selected;
            }

            set
            {
                this.selected = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Selected");
            }
        }

        /// <summary>Gets the collection of shortcuts to perform actions on in the update.</summary>
        /// <value>The shortcuts.</value>
        [ProtoMember(10, IsRequired = false)]
        [DataMember]
        public ObservableCollection<Shortcut> Shortcuts { get; private set; }

        /// <summary>Gets the total download size in bytes of the update.</summary>
        /// <value>The total download size of the update.</value>
        [ProtoMember(11, IsRequired = false)]
        [DataMember]
        public ulong Size
        {
            get
            {
                return this.size;
            }

            internal set
            {
                this.size = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Size");
            }
        }

        #endregion

        #region Methods

        /// <summary>Fires the OnPropertyChanged Event with the collection changes.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data.</param>
        private void DescriptionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Description");
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data.</param>
        private void NameCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Name");
        }

        /// <summary>When a property has changed, call the <c>OnPropertyChanged</c> Event.</summary>
        /// <param name="propertyName">The name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}