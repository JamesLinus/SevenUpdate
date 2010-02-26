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

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.WCF
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0"),
     ServiceContract(ConfigurationName = "WCF.IEventSystem", CallbackContract = typeof (IEventSystemCallback), SessionMode = SessionMode.Required)]
    
    public interface IEventSystem
    {
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IEventSystem/Subscribe")]
        void Subscribe();

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IEventSystem/UnSubscribe")]
        void UnSubscribe();
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IEventSystemCallback
    {
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IEventSystem/OnDownloadCompleted")]
        void OnDownloadCompleted(bool errorOccurred);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IEventSystem/OnInstallCompleted")]
        void OnInstallCompleted(int updatesInstalled, int updatesFailed);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IEventSystem/OnErrorOccurred")]
        void OnErrorOccurred(Exception exception, ErrorType type);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IEventSystem/OnDownloadProgressChanged")]
        void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IEventSystem/OnInstallProgressChanged")]
        void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates);
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IEventSystemChannel : IEventSystem, IClientChannel
    {
    }

    [DebuggerStepThrough, GeneratedCode("System.ServiceModel", "4.0.0.0")]
    
    public class EventSystemClient : DuplexClientBase<IEventSystem>, IEventSystem
    {
        public EventSystemClient(InstanceContext callbackInstance) : base(callbackInstance)
        {
        }

        public EventSystemClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public EventSystemClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public EventSystemClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public EventSystemClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        #region IEventSystem Members

        public void Subscribe()
        {
            Channel.Subscribe();
        }

        public void UnSubscribe()
        {
            Channel.UnSubscribe();
        }

        #endregion
    }
}