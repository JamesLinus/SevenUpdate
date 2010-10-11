// ***********************************************************************
// <copyright file="Sui.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt">GNU General Public License Version 3</license>
// ***********************************************************************
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
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
namespace SevenUpdate
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>
    /// The collection of updates and the application info.
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Sua))]
    [KnownType(typeof(ObservableCollection<Update>))]
    public sealed class Sui : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The application information
        /// </summary>
        private Sua appInfo;

        /// <summary>
        ///   A collection of updates for the application
        /// </summary>
        private ObservableCollection<Update> updates;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the  software information for the application updates.
        /// </summary>
        [ProtoMember(2)] [DataMember] public Sua AppInfo
        {
            get
            {
                return this.appInfo;
            }

            set
            {
                this.appInfo = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AppInfo");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of updates for the application
        /// </summary>
        [ProtoMember(1)] [DataMember] public ObservableCollection<Update> Updates
        {
            get
            {
                return this.updates;
            }

            set
            {
                this.updates = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Updates");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="name">
        /// The name of the property that changed
        /// </param>
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