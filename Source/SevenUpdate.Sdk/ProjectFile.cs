#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using ProtoBuf;

#endregion

namespace SevenUpdate.Sdk
{
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (Sua)), KnownType(typeof (ObservableCollection<Update>))]
    public class Project : INotifyPropertyChanged
    {
        private string applicationName;
        private ObservableCollection<string> updateNames;

        [ProtoMember(1), DataMember]
        public string ApplicationName
        {
            get { return applicationName; }
            set
            {
                applicationName = value;
                OnPropertyChanged("ApplicationName");
            }
        }

        [ProtoMember(2), DataMember]
        public ObservableCollection<string> UpdateNames
        {
            get { return updateNames; }
            set
            {
                updateNames = value;
                OnPropertyChanged("UpdateNames");
            }
        }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}