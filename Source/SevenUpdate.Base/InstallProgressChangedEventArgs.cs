// ***********************************************************************
// <copyright file="InstallProgressChangedEventArgs.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
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
    using System;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the InstallProgressChanged event.</summary>
    [ProtoContract]
    [DataContract]
    public sealed class InstallProgressChangedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="InstallProgressChangedEventArgs" /> class.</summary>
        /// <param name="updateName">The name of the update currently being installed.</param>
        /// <param name="progress">The progress percentage of the installation.</param>
        /// <param name="updatesComplete">The number of updates that have been installed so far.</param>
        /// <param name="totalUpdates">The total number of updates to install.</param>
        public InstallProgressChangedEventArgs(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            this.CurrentProgress = progress;
            this.TotalUpdates = totalUpdates;
            this.UpdatesComplete = updatesComplete;
            this.UpdateName = updateName;
        }

        /// <summary>Initializes a new instance of the <see cref="InstallProgressChangedEventArgs" /> class.</summary>
        public InstallProgressChangedEventArgs()
        {
        }

        /// <summary>Gets the progress percentage of the installation.</summary>
        /// <value>The current progress.</value>
        [ProtoMember(1)]
        [DataMember]
        public int CurrentProgress { get; private set; }

        /// <summary>Gets the total number of updates to install.</summary>
        /// <value>The total updates.</value>
        [ProtoMember(2)]
        [DataMember]
        public int TotalUpdates { get; private set; }

        /// <summary>Gets the name of the current update being installed.</summary>
        /// <value>The name of the update.</value>
        [ProtoMember(3)]
        [DataMember]
        public string UpdateName { get; private set; }

        /// <summary>Gets the number of updates that have been installed so far.</summary>
        /// <value>The updates complete.</value>
        [ProtoMember(4)]
        [DataMember]
        public int UpdatesComplete { get; private set; }
    }
}