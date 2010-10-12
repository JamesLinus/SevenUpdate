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
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
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

    /// <summary>Seven Update Application information</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(ObservableCollection<LocaleString>))]
    public sealed class Sua : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The <see cref = "Uri" /> for the application's website</summary>
        private Uri appUrl;

        /// <summary>The collection of localized descriptions for the application</summary>
        private ObservableCollection<LocaleString> description;

        /// <summary>The directory where the application is installed</summary>
        private string directory;

        /// <summary>The help website <see cref = "Uri" /> of the application.</summary>
        private Uri helpUrl;

        /// <summary>Indicates whether if the application is 64 bit</summary>
        private bool is64Bit;

        /// <summary>Indicates whether the SUA is enabled with Seven Update (SDK does not use this value)</summary>
        private bool isEnabled;

        /// <summary>A collection of localized application names</summary>
        private ObservableCollection<LocaleString> name;

        /// <summary>A collection of localized publisher names</summary>
        private ObservableCollection<LocaleString> publisher;

        /// <summary>The <see cref = "Uri" /> pointing to the sui file containing the application updates</summary>
        private Uri suiUrl;

        /// <summary>The name of the value to the registry key that contains the application directory location</summary>
        private string valueName;

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets the <see cref = "Uri" /> for the application's website</summary>
        /// <value>The application website</value>
        [ProtoMember(8, IsRequired = false)] [DataMember] public Uri AppUrl
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

        /// <summary>Gets or sets the collection of localized descriptions for the application</summary>
        /// <value>The application description</value>
        [ProtoMember(2)] [DataMember] public ObservableCollection<LocaleString> Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Description");
            }
        }

        /// <summary>Gets or sets the directory where the application is installed</summary>
        /// <value>The install directory</value>
        [ProtoMember(3)] [DataMember] public string Directory
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
        [ProtoMember(9, IsRequired = false)] [DataMember] public Uri HelpUrl
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
        [ProtoMember(4)] [DataMember] public bool Is64Bit
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
        [ProtoMember(5)] [DataMember] public bool IsEnabled
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

        /// <summary>Gets or sets a collection of localized application names</summary>
        /// <value>The name of the application localized</value>
        [ProtoMember(1)] [DataMember] public ObservableCollection<LocaleString> Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>Gets or sets the collection of localized publisher names</summary>
        /// <value>The publisher.</value>
        [ProtoMember(6)] [DataMember] public ObservableCollection<LocaleString> Publisher
        {
            get
            {
                return this.publisher;
            }

            set
            {
                this.publisher = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Publisher");
            }
        }

        /// <summary>Gets or sets the <see cref = "Uri" /> pointing to the sui file containing the application updates</summary>
        /// <value>The url pointing to the sui file</value>
        [ProtoMember(7)]
        [DataMember]
        public Uri SuiUrl
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
        [ProtoMember(10, IsRequired = false)] [DataMember] public string ValueName
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