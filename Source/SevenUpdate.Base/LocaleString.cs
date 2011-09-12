// ***********************************************************************
// <copyright file="Localestring.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
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
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Contains a string indicating the language and a value.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    public sealed class LocaleString : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The ISO language code.</summary>
        private string lang;

        /// <summary>The value of the string.</summary>
        private string value;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the LocaleString class.</summary>
        public LocaleString()
        {
        }

        /// <summary>Initializes a new instance of the <c>LocaleString</c> class.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="lang">The an ISO language code for the value.</param>
        public LocaleString(string value, string lang)
        {
            this.Lang = lang;
            this.Value = value;
        }

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets an ISO language code.</summary>
        /// <value>The iso code.</value>
        [ProtoMember(1)]
        [DataMember]
        public string Lang
        {
            get
            {
                return this.lang;
            }

            set
            {
                this.lang = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Lang");
            }
        }

        /// <summary>Gets or sets the value of the string.</summary>
        /// <value>The value.</value>
        [ProtoMember(2)]
        [DataMember]
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Value");
            }
        }

        #endregion

        #region Methods

        /// <summary>When a property has changed, call the <c>OnPropertyChanged</c> Event.</summary>
        /// <param name="name">The name of the property that changed.</param>
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