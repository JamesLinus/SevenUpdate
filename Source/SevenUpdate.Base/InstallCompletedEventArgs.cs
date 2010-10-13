// ***********************************************************************
// <copyright file="InstallCompletedEventArgs.cs"
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
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the InstallCompleted event</summary>
    [ProtoContract, DataContract]
    public sealed class InstallCompletedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="InstallCompletedEventArgs"/> class.</summary>
        /// <param name="updatesInstalled">the number of updates installed</param>
        /// <param name="updatesFailed">the number of updates that failed</param>
        public InstallCompletedEventArgs(int updatesInstalled, int updatesFailed)
        {
            this.UpdatesInstalled = updatesInstalled;
            this.UpdatesFailed = updatesFailed;
        }

        #endregion

        #region Properties

        /// <summary>Gets the number of updates that failed.</summary>
        /// <value>The updates failed.</value>
        [ProtoMember(1), DataMember]
        public int UpdatesFailed { get; private set; }

        /// <summary>Gets the number of updates that have been installed</summary>
        /// <value>The updates installed.</value>
        [ProtoMember(2), DataMember]
        public int UpdatesInstalled { get; private set; }

        #endregion
    }
}