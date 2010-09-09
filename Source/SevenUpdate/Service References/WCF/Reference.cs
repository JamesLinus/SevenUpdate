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

using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

#endregion

namespace SevenUpdate.WCF
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0"), ServiceContract(ConfigurationName = "WCF.IService", CallbackContract = typeof (IServiceCallback), SessionMode = SessionMode.Required)]
    
    public interface IService
    {
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/Subscribe")]
        void Subscribe();

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/UnSubscribe")]
        void UnSubscribe();

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/AddApp")]
        void AddApp(Sua app);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/InstallUpdates")]
        void InstallUpdates(Collection<Sui> appUpdates);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/ShowUpdate")]
        void ShowUpdate(Suh hiddenUpdate);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/HideUpdate")]
        void HideUpdate(Suh hiddenUpdate);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/HideUpdates")]
        void HideUpdates(Collection<Suh> hiddenUpdates);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/ChangeSettings")]
        void ChangeSettings(Collection<Sua> apps, Config options, bool autoCheck);
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IServiceCallback
    {
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnDownloadCompleted")]
        void OnDownloadCompleted(bool errorOccurred);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnInstallCompleted")]
        void OnInstallCompleted(int updatesInstalled, int updatesFailed);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnErrorOccurred")]
        void OnErrorOccurred(string exception, ErrorType type);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnDownloadProgressChanged")]
        void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnInstallProgressChanged")]
        void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates);
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : IService, IClientChannel
    {
    }

    [DebuggerStepThrough, GeneratedCode("System.ServiceModel", "4.0.0.0")]
    
    public class ServiceClient : DuplexClientBase<IService>, IService
    {
        public ServiceClient(InstanceContext callbackInstance) : base(callbackInstance)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        #region IService Members

        public void Subscribe()
        {
            base.Channel.Subscribe();
        }

        public void UnSubscribe()
        {
            base.Channel.UnSubscribe();
        }

        public void AddApp(Sua app)
        {
            base.Channel.AddApp(app);
        }

        public void InstallUpdates(Collection<Sui> appUpdates)
        {
            base.Channel.InstallUpdates(appUpdates);
        }

        public void ShowUpdate(Suh hiddenUpdate)
        {
            base.Channel.ShowUpdate(hiddenUpdate);
        }

        public void HideUpdate(Suh hiddenUpdate)
        {
            base.Channel.HideUpdate(hiddenUpdate);
        }

        public void HideUpdates(Collection<Suh> hiddenUpdates)
        {
            base.Channel.HideUpdates(hiddenUpdates);
        }

        public void ChangeSettings(Collection<Sua> apps, Config options, bool autoCheck)
        {
            base.Channel.ChangeSettings(apps, options, autoCheck);
        }

        #endregion
    }
}