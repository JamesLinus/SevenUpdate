// ***********************************************************************
// <copyright file="Config.cs"
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
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Automatic Update option Seven Update can use</summary>
    [ProtoContract, DataContract, DefaultValue(Install)]
    public enum AutoUpdateOption
    {
        /// <summary>Download and Installs updates automatically</summary>
        [ProtoEnum, EnumMember]
        Install = 0,

        /// <summary>Downloads Updates automatically</summary>
        [ProtoEnum, EnumMember]
        Download = 1,

        /// <summary>Only checks and notifies the user of updates</summary>
        [ProtoEnum, EnumMember]
        Notify = 2,

        /// <summary>No automatic checking</summary>
        [ProtoEnum, EnumMember]
        Never = 3
    }

    /// <summary>Configuration options</summary>
    [ProtoContract, DataContract(IsReference = true)]
    public sealed class Config : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The automatic update setting</summary>
        private AutoUpdateOption autoOption;

        /// <summary>A value that indicates whether to treat <see cref = "Importance.Recommended" /> updates the same as <see cref = "Importance.Important" /> updates</summary>
        private bool includeRecommended;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "Config" /> class</summary>
        public Config()
        {
        }

        #endregion

        #region Events

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>Gets or sets which automatic update option Seven Update should use</summary>
        /// <value>The automatic update option</value>
        [ProtoMember(1), DataMember]
        public AutoUpdateOption AutoOption
        {
            get
            {
                return this.autoOption;
            }

            set
            {
                this.autoOption = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AutoOption");
            }
        }

        /// <summary>Gets or sets a value indicating whether Seven Update is to included recommended updates when automatically downloading updates</summary>
        /// <value><see langword = "true" /> if recommended updates should be treated as important updates otherwise, <see langword = "false" />.</value>
        [ProtoMember(2), DataMember]
        public bool IncludeRecommended
        {
            get
            {
                return this.includeRecommended;
            }

            set
            {
                this.includeRecommended = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("IncludeRecommended");
            }
        }

        #endregion

        #region Methods

        /// <summary>When a property has changed, call the <see cref = "OnPropertyChanged" /> Event</summary>
        /// <param name = "name">The name of the property that changed</param>
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