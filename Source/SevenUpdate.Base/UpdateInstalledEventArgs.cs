// ***********************************************************************
// <copyright file="UpdateInstalledEventArgs.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
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

    /// <summary>Provides event data for the InstallProgressChanged event.</summary>
    [ProtoContract, DataContract]
    public sealed class UpdateInstalledEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UpdateInstalledEventArgs" /> class.</summary>
        /// <param name="update">The update information that was installed.</param>
        public UpdateInstalledEventArgs(Suh update)
        {
            this.Update = update;
        }

        /// <summary>Initializes a new instance of the <see cref="UpdateInstalledEventArgs" /> class.</summary>
        public UpdateInstalledEventArgs()
        {
        }

        #endregion

        #region Properties

        /// <summary>Gets the update information that was installed.</summary>
        [ProtoMember(1), DataMember]
        public Suh Update { get; private set; }

        #endregion
    }
}