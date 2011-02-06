// ***********************************************************************
// <copyright file="Sua.cs"
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
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>The current status of the update</summary>
    [ProtoContract, DataContract, DefaultValue(x86)]
    public enum Platform
    {
        /// <summary>Indicates that the application can run on 32bit or 64bit natively depending on the OS</summary>
        [ProtoEnum, EnumMember]
        AnyCPU = 2,

        /// <summary>Indicates that the application is native 32 bit</summary>
        [ProtoEnum, EnumMember]
        x86 = 0,

        /// <summary>Indicates that the application can only run on 64 bit platforms</summary>
        [ProtoEnum, EnumMember]
        x64 = 1,
    }

    /// <summary>Seven Update Application information</summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof(ObservableCollection<LocaleString>))]
    public sealed class Sua : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The <see cref = "Uri" /> for the application's website</summary>
        private string appUrl;

        /// <summary>The directory where the application is installed</summary>
        private string directory;

        /// <summary>The help website <see cref = "Uri" /> of the application.</summary>
        private string helpUrl;

        /// <summary>Indicates if the application is 64 bit</summary>
        private bool is64Bit;

        /// <summary>Indicates whether the SUA is enabled with Seven Update (SDK does not use this value)</summary>
        private bool isEnabled;

        /// <summary>Indicates the cpu platform the application can run under</summary>
        private Platform platform;

        /// <summary>The <see cref = "Uri" /> pointing to the sui file containing the application updates</summary>
        private string suiUrl;

        /// <summary>The name of the value to the registry key that contains the application directory location</summary>
        private string valueName;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Sua"/> class</summary>
        /// <param name="name">The collection of localized update names</param>
        /// <param name="publisher">The collection of localized publisher names</param>
        /// <param name="description">The collection of localized update descriptions</param>
        public Sua(ObservableCollection<LocaleString> name, ObservableCollection<LocaleString> publisher, ObservableCollection<LocaleString> description)
        {
            this.Name = name;
            this.Publisher = publisher;
            this.Description = description;

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

        /// <summary>Initializes a new instance of the <see cref="Sua"/> class</summary>
        /// <param name="name">The collection of localized update names</param>
        /// <param name="publisher">The collection of localized publisher names</param>
        public Sua(ObservableCollection<LocaleString> name, ObservableCollection<LocaleString> publisher)
        {
            this.Name = name;
            this.Publisher = publisher;

            if (this.Name == null)
            {
                this.Name = new ObservableCollection<LocaleString>();
            }

            if (this.Publisher == null)
            {
                this.Publisher = new ObservableCollection<LocaleString>();
            }

            this.Description = new ObservableCollection<LocaleString>();

            this.Name.CollectionChanged -= this.NameCollectionChanged;
            this.Description.CollectionChanged -= this.DescriptionCollectionChanged;
            this.Publisher.CollectionChanged -= this.PublisherCollectionChanged;

            this.Name.CollectionChanged += this.NameCollectionChanged;
            this.Description.CollectionChanged += this.DescriptionCollectionChanged;
            this.Publisher.CollectionChanged += this.PublisherCollectionChanged;
        }

        /// <summary>Initializes a new instance of the <see cref = "Sua" /> class</summary>
        public Sua()
        {
            this.Name = new ObservableCollection<LocaleString>();
            this.Description = new ObservableCollection<LocaleString>();
            this.Publisher = new ObservableCollection<LocaleString>();
        }

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets the <see cref = "Uri" /> for the application's website</summary>
        /// <value>The application website</value>
        [ProtoMember(8, IsRequired = false), DataMember]
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
                this.OnPropertyChanged("AppUrl");
            }
        }

        /// <summary>Gets the collection of localized descriptions for the application</summary>
        /// <value>The application description</value>
        [ProtoMember(2), DataMember]
        public ObservableCollection<LocaleString> Description { get; private set; }

        /// <summary>Gets or sets the directory where the application is installed</summary>
        /// <value>The install directory</value>
        [ProtoMember(3), DataMember]
        public string Directory
        {
            get
            {
                return this.directory;
            }

            set
            {
                this.directory = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Directory");
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

        /// <summary>Gets or sets a value indicating whether if the application is 64 bit</summary>
        /// <value><see langword = "true" /> if the application is 64 bit; otherwise, <see langword = "false" />.</value>
        [ProtoMember(4), DataMember, Obsolete]
        public bool Is64Bit
        {
            get
            {
                return this.is64Bit;
            }

            set
            {
                this.is64Bit = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Is64Bit");
            }
        }

        /// <summary>Gets or sets a value indicating whether the SUA is enabled with Seven Update (SDK does not use this value)</summary>
        /// <value><see langword = "true" /> if this instance is enabled; otherwise, <see langword = "false" />.</value>
        [ProtoMember(5), DataMember]
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.isEnabled = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>Gets the collection of localized application names</summary>
        /// <value>The name of the application localized</value>
        [ProtoMember(1), DataMember]
        public ObservableCollection<LocaleString> Name { get; private set; }

        /// <summary>Gets or sets the cpu platform the application can run under.</summary>
        [ProtoMember(11), DataMember]
        public Platform Platform
        {
            get
            {
                return this.platform;
            }

            set
            {
                this.platform = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Platform");
            }
        }

        /// <summary>Gets the collection of localized publisher names</summary>
        /// <value>The publisher.</value>
        [ProtoMember(6), DataMember]
        public ObservableCollection<LocaleString> Publisher { get; private set; }

        /// <summary>Gets or sets the <see cref = "Uri" /> pointing to the sui file containing the application updates</summary>
        /// <value>The url pointing to the sui file</value>
        [ProtoMember(7), DataMember]
        public string SuiUrl
        {
            get
            {
                return this.suiUrl;
            }

            set
            {
                this.suiUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("SuiUrl");
            }
        }

        /// <summary>Gets or sets the name of the value to the registry key that contains the application directory location</summary>
        /// <value>The name of the value.</value>
        [ProtoMember(10, IsRequired = false), DataMember]
        public string ValueName
        {
            get
            {
                return this.valueName;
            }

            set
            {
                this.valueName = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ValueName");
            }
        }

        #endregion

        #region Methods

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void PublisherCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Publisher");
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void DescriptionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Description");
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void NameCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Name");
        }

        /// <summary>When a property has changed, call the <see cref="OnPropertyChanged"/> Event</summary>
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