// ***********************************************************************
// <copyright file="Suh.cs"
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
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The current status of the update</summary>
    [ProtoContract, DataContract, DefaultValue(Successful)]
    public enum UpdateStatus
    {
        /// <summary>Indicates that the update installation failed</summary>
        [ProtoEnum, EnumMember]
        Failed = 0,

        /// <summary>Indicates that the update is hidden</summary>
        [ProtoEnum, EnumMember]
        Hidden = 1,

        /// <summary>Indicates that the update is visible</summary>
        [ProtoEnum, EnumMember]
        Visible = 2,

        /// <summary>Indicates that the update installation succeeded</summary>
        [ProtoEnum, EnumMember]
        Successful = 3
    }

    /// <summary>Information about an update, used by History and Hidden Updates. Not used by the SDK</summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof(UpdateStatus)), KnownType(typeof(Importance)), KnownType(typeof(ObservableCollection<LocaleString>))]
    public sealed class Suh : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The collection of localized update descriptions</summary>
        private readonly ObservableCollection<LocaleString> description;

        /// <summary>The collection of localized update names</summary>
        private readonly ObservableCollection<LocaleString> name;

        /// <summary>The collection of localized publisher names</summary>
        private readonly ObservableCollection<LocaleString> publisher;

        /// <summary>The <see cref = "Uri" /> for the application's website</summary>
        private string appUrl;

        /// <summary>The help website <see cref = "Uri" /> of the application</summary>
        private string helpUrl;

        /// <summary>The importance of the update</summary>
        private Importance importance;

        /// <summary>The url pointing to a resource to find more information about the update</summary>
        private string infoUrl;

        /// <summary>The formatted date string when the update was installed</summary>
        private string installDate;

        /// <summary>The formatted date string depicting the release date of the update</summary>
        private string releaseDate;

        /// <summary>The current status of the update</summary>
        private UpdateStatus status;

        /// <summary>The total download size in bytes of the update</summary>
        private ulong updateSize;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "Suh" /> class</summary>
        /// <param name = "name">The collection of localized update names</param>
        /// <param name = "publisher">The collection of localized publisher names</param>
        /// <param name = "description">The collection of localized update descriptions</param>
        public Suh(ObservableCollection<LocaleString> name, ObservableCollection<LocaleString> publisher, ObservableCollection<LocaleString> description)
        {
            this.name = name;
            this.description = description;
            this.publisher = publisher;

            if (this.name == null)
            {
                this.name = new ObservableCollection<LocaleString>();
            }

            if (this.description == null)
            {
                this.description = new ObservableCollection<LocaleString>();
            }

            if (this.publisher == null)
            {
                this.publisher = new ObservableCollection<LocaleString>();
            }
        }

        /// <summary>Initializes a new instance of the <see cref = "Suh" /> class</summary>
        public Suh()
        {
        }

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets the <see cref = "Uri" /> for the application's website</summary>
        /// <value>The application website</value>
        [ProtoMember(8), DataMember]
        public string AppUrl
        {
            get
            {
                return this.appUrl;
            }

            set
            {
                this.appUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("PublisherUrl");
            }
        }

        /// <summary>Gets the collection localized update descriptions</summary>
        /// <value>The localized description for the update</value>
        [ProtoMember(2), DataMember]
        public ObservableCollection<LocaleString> Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>Gets or sets the help website <see cref = "Uri" /> of the application</summary>
        /// <value>The help and support website for the application</value>
        [ProtoMember(9, IsRequired = false), DataMember]
        public string HelpUrl
        {
            get
            {
                return this.helpUrl;
            }

            set
            {
                this.helpUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("HelpUrl");
            }
        }

        /// <summary>Gets or sets the importance of the update</summary>
        /// <value>The importance</value>
        [ProtoMember(3), DataMember]
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

        /// <summary>Gets or sets the url pointing to a resource to find more information about the update</summary>
        /// <value>The info URL.</value>
        [ProtoMember(10, IsRequired = false), DataMember]
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

        /// <summary>Gets or sets the formatted date string when the update was installed</summary>
        /// <value>The formatted install date string (MM/DD/YYYY).</value>
        [ProtoMember(11), DataMember]
        public string InstallDate
        {
            get
            {
                return this.installDate;
            }

            set
            {
                this.installDate = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("InstallDate");
            }
        }

        /// <summary>Gets the collection of localized update names</summary>
        /// <value>The localized update names</value>
        [ProtoMember(1), DataMember]
        public ObservableCollection<LocaleString> Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>Gets the collection of localized publisher names</summary>
        /// <value>The publisher.</value>
        [ProtoMember(7), DataMember]
        public ObservableCollection<LocaleString> Publisher
        {
            get
            {
                return this.publisher;
            }
        }

        /// <summary>Gets or sets the formatted date string depicting the release date of the update</summary>
        /// <value>The release date in a formatted string MM/DD/YYYY</value>
        [ProtoMember(5), DataMember]
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

        /// <summary>Gets or sets the current status of the update</summary>
        /// <value>The status.</value>
        [ProtoMember(4), DataMember]
        public UpdateStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Status");
            }
        }

        /// <summary>Gets or sets the total download size in bytes of the update</summary>
        /// <value>The total download size of the update</value>
        [ProtoMember(6), DataMember]
        public ulong UpdateSize
        {
            get
            {
                return this.updateSize;
            }

            set
            {
                this.updateSize = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("UpdateSize");
            }
        }

        #endregion

        #region Methods

        /// <summary>When a property has changed, call the <see cref = "OnPropertyChanged" /> Event</summary>
        /// <param name = "propertyName">The name of the property that changed</param>
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