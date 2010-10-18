﻿// ***********************************************************************
// <copyright file="MyServiceHost.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
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
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.ServiceModel;

    using SevenUpdate.Service;

    /// <summary>Contains methods to start the WCF service host</summary>
    internal static class MyServiceHost
    {
        #region Properties

        /// <summary>Gets or sets the <see cref="ServiceHost" /> instance</summary>
        internal static ServiceHost Instance { get; set; }

        #endregion

        /// <summary>Starts the service</summary>
        internal static void StartService()
        {
            var binding = new NetNamedPipeBinding
                {
                    Name = "uacbinding",
                    Security =
                        {
                            Mode = NetNamedPipeSecurityMode.Transport
                        }
                };

            var baseAddress = new Uri("net.pipe://localhost/sevenupdate/");

            Instance = new ServiceHost(typeof(WcfService), baseAddress);
            Instance.AddServiceEndpoint(typeof(IElevatedProcessCallback), binding, baseAddress).Behaviors.Add(new ProtoBuf.ServiceModel.ProtoEndpointBehavior());
            Instance.Open();
        }

        /// <summary>Stops the service</summary>
        internal static void StopService()
        {
            if (Instance == null)
            {
                return;
            }

            // Call StopService from your shutdown logic (i.e. dispose method)
            if (Instance.State != CommunicationState.Closed)
            {
                Instance.Close();
            }
            Instance = null;
        }
    }
}