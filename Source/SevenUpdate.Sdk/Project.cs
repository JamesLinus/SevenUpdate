// ***********************************************************************
// <copyright file="Project.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
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
namespace SevenUpdate.Sdk
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Contains data specifying the application name and it's updates</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Sua))]
    [KnownType(typeof(ObservableCollection<Update>))]
    public class Project : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The localized name of the application</summary>
        private string applicationName;

        /// <summary>The collection of localized update names</summary>
        private ObservableCollection<string> updateNames;

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets the localized application name</summary>
        /// <value>The name of the application.</value>
        [ProtoMember(1)] [DataMember] public string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                this.applicationName = value;
                this.OnPropertyChanged("ApplicationName");
            }
        }

        /// <summary>Gets or sets the update names.</summary>
        /// <value>The update names.</value>
        [ProtoMember(2)] [DataMember] public ObservableCollection<string> UpdateNames
        {
            get
            {
                return this.updateNames;
            }

            set
            {
                this.updateNames = value;
                this.OnPropertyChanged("UpdateNames");
            }
        }

        #endregion

        #region Methods

        /// <summary>When a property has changed, call the <see cref="OnPropertyChanged"/> Event</summary>
        /// <param name="name">The name of the property changed</param>
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