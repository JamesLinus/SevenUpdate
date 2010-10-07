﻿// ***********************************************************************
// Assembly         : SevenUpdate
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.WCF
{
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// </summary>
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName = "WCF.IService", CallbackContract = typeof(IServiceCallback), SessionMode = SessionMode.Required)]
    public interface IService
    {
        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="app">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/AddApp")]
        void AddApp(Sua app);

        /// <summary>
        /// </summary>
        /// <param name="apps">
        /// </param>
        /// <param name="options">
        /// </param>
        /// <param name="autoCheck">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/ChangeSettings")]
        void ChangeSettings(Collection<Sua> apps, Config options, bool autoCheck);

        /// <summary>
        /// </summary>
        /// <param name="hiddenUpdate">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/HideUpdate")]
        void HideUpdate(Suh hiddenUpdate);

        /// <summary>
        /// </summary>
        /// <param name="hiddenUpdates">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/HideUpdates")]
        void HideUpdates(Collection<Suh> hiddenUpdates);

        /// <summary>
        /// </summary>
        /// <param name="appUpdates">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/InstallUpdates")]
        void InstallUpdates(Collection<Sui> appUpdates);

        /// <summary>
        /// </summary>
        /// <param name="hiddenUpdate">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/ShowUpdate")]
        void ShowUpdate(Suh hiddenUpdate);

        /// <summary>
        /// </summary>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/Subscribe")]
        void Subscribe();

        /// <summary>
        /// </summary>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/UnSubscribe")]
        void UnSubscribe();

        #endregion
    }

    /// <summary>
    /// </summary>
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IServiceCallback
    {
        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="errorOccurred">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnDownloadCompleted")]
        void OnDownloadCompleted(bool errorOccurred);

        /// <summary>
        /// </summary>
        /// <param name="bytesTransferred">
        /// </param>
        /// <param name="bytesTotal">
        /// </param>
        /// <param name="filesTransferred">
        /// </param>
        /// <param name="filesTotal">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnDownloadProgressChanged")]
        void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal);

        /// <summary>
        /// </summary>
        /// <param name="exception">
        /// </param>
        /// <param name="type">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnErrorOccurred")]
        void OnErrorOccurred(string exception, ErrorType type);

        /// <summary>
        /// </summary>
        /// <param name="updatesInstalled">
        /// </param>
        /// <param name="updatesFailed">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnInstallCompleted")]
        void OnInstallCompleted(int updatesInstalled, int updatesFailed);

        /// <summary>
        /// </summary>
        /// <param name="updateName">
        /// </param>
        /// <param name="progress">
        /// </param>
        /// <param name="updatesComplete">
        /// </param>
        /// <param name="totalUpdates">
        /// </param>
        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IService/OnInstallProgressChanged")]
        void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates);

        #endregion
    }

    /// <summary>
    /// </summary>
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : IService, IClientChannel
    {
    }

    /// <summary>
    /// </summary>
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class ServiceClient : DuplexClientBase<IService>, IService
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="callbackInstance">
        /// </param>
        public ServiceClient(InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="callbackInstance">
        /// </param>
        /// <param name="endpointConfigurationName">
        /// </param>
        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName)
            : base(callbackInstance, endpointConfigurationName)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="callbackInstance">
        /// </param>
        /// <param name="endpointConfigurationName">
        /// </param>
        /// <param name="remoteAddress">
        /// </param>
        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="callbackInstance">
        /// </param>
        /// <param name="endpointConfigurationName">
        /// </param>
        /// <param name="remoteAddress">
        /// </param>
        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="callbackInstance">
        /// </param>
        /// <param name="binding">
        /// </param>
        /// <param name="remoteAddress">
        /// </param>
        public ServiceClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        #endregion

        #region Implemented Interfaces

        #region IService

        /// <summary>
        /// </summary>
        /// <param name="app">
        /// </param>
        public void AddApp(Sua app)
        {
            base.Channel.AddApp(app);
        }

        /// <summary>
        /// </summary>
        /// <param name="apps">
        /// </param>
        /// <param name="options">
        /// </param>
        /// <param name="autoCheck">
        /// </param>
        public void ChangeSettings(Collection<Sua> apps, Config options, bool autoCheck)
        {
            base.Channel.ChangeSettings(apps, options, autoCheck);
        }

        /// <summary>
        /// </summary>
        /// <param name="hiddenUpdate">
        /// </param>
        public void HideUpdate(Suh hiddenUpdate)
        {
            base.Channel.HideUpdate(hiddenUpdate);
        }

        /// <summary>
        /// </summary>
        /// <param name="hiddenUpdates">
        /// </param>
        public void HideUpdates(Collection<Suh> hiddenUpdates)
        {
            base.Channel.HideUpdates(hiddenUpdates);
        }

        /// <summary>
        /// </summary>
        /// <param name="appUpdates">
        /// </param>
        public void InstallUpdates(Collection<Sui> appUpdates)
        {
            base.Channel.InstallUpdates(appUpdates);
        }

        /// <summary>
        /// </summary>
        /// <param name="hiddenUpdate">
        /// </param>
        public void ShowUpdate(Suh hiddenUpdate)
        {
            base.Channel.ShowUpdate(hiddenUpdate);
        }

        /// <summary>
        /// </summary>
        public void Subscribe()
        {
            base.Channel.Subscribe();
        }

        /// <summary>
        /// </summary>
        public void UnSubscribe()
        {
            base.Channel.UnSubscribe();
        }

        #endregion

        #endregion
    }
}