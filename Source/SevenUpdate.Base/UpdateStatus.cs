// ***********************************************************************
// <copyright file="UpdateStatus.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
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

    /// <summary>The current status of the update.</summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Successful)]
    public enum UpdateStatus
    {
        /// <summary>Indicates that the update installation failed.</summary>
        [ProtoEnum]
        [EnumMember]
        Failed = 0, 

        /// <summary>Indicates that the update is hidden.</summary>
        [ProtoEnum]
        [EnumMember]
        Hidden = 1, 

        /// <summary>Indicates that the update is visible.</summary>
        [ProtoEnum]
        [EnumMember]
        Visible = 2, 

        /// <summary>Indicates that the update installation succeeded.</summary>
        [ProtoEnum]
        [EnumMember]
        Successful = 3
    }
}