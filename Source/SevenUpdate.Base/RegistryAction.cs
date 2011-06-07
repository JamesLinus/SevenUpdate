// ***********************************************************************
// <copyright file="RegistryAction.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
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

    /// <summary>
    ///   Contains the Actions you can perform to the registry.
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Add)]
    public enum RegistryAction
    {
        /// <summary>
        ///   Adds a registry entry to the machine.
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Add = 0,

        /// <summary>
        ///   Deletes a registry key on the machine.
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        DeleteKey = 1,

        /// <summary>
        ///   Deletes a value of a registry key on the machine.
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        DeleteValue = 2
    }
}