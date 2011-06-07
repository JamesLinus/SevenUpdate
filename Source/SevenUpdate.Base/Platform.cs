// ***********************************************************************
// <copyright file="Platform.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
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
    ///   The current status of the update.
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(X86)]
    public enum Platform
    {
        /// <summary>
        ///   Indicates that the application is native 32 bit.
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        X86 = 0,

        /// <summary>
        ///   Indicates that the application can only run on 64 bit platforms.
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        X64 = 1,

        /// <summary>
        ///   Indicates that the application can run on 32bit or 64bit natively depending on the OS.
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        AnyCpu = 2,
    }
}