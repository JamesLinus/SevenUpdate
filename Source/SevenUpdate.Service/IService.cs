// ***********************************************************************
// <copyright file="IService.cs"
//            project="SevenUpdate.Service"
//            assembly="SevenUpdate.Service"
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
namespace SevenUpdate.Service
{
    using System.Collections.ObjectModel;
    using System.ServiceModel;

    using ProtoBuf.ServiceModel;

    /// <summary>Callback methods for the WCF Service</summary>
    public interface IServiceCallBack
    {
        #region Public Methods

        /// <summary>Occurs when the download has completed</summary>
        /// <param name="errorOccurred"><see langword = "true" /> if an error occurred, otherwise <see langword = "false" /></param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnDownloadCompleted(bool errorOccurred);

        /// <summary>Occurs when the download progress has changed</summary>
        /// <param name="bytesTransferred">The number of bytes downloaded</param>
        /// <param name="bytesTotal">The total number of bytes to download</param>
        /// <param name="filesTransferred">The number of files downloaded</param>
        /// <param name="filesTotal">The total number of files to download</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal);

        /// <summary>Occurs when an error occurs</summary>
        /// <param name="exception">The exception data</param>
        /// <param name="type">The <see cref="ErrorType"/> of the error that occurred</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnErrorOccurred(string exception, ErrorType type);

        /// <summary>Occurs when the installation of updates has completed</summary>
        /// <param name="updatesInstalled">The number of updates installed</param>
        /// <param name="updatesFailed">The number of failed updates</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnInstallCompleted(int updatesInstalled, int updatesFailed);

        /// <summary>Occurs when the installation progress has changed</summary>
        /// <param name="updateName">The name of the update that is being installed</param>
        /// <param name="progress">The current update progress</param>
        /// <param name="updatesComplete">The number of updates that have completed</param>
        /// <param name="totalUpdates">The total number of updates</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates);

        #endregion
    }

    /// <summary>Methods for the Event Service</summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IServiceCallBack))]
    internal interface IService
    {
        #region Public Methods

        /// <summary>Adds an application to Seven Update, so it can manage updates for it.</summary>
        /// <param name="application">The application to add</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void AddApp(Sua application);

        /// <summary>Changes the program settings</summary>
        /// <param name="applications">The applications to enable update checking</param>
        /// <param name="options">The Seven Update settings</param>
        /// <param name="autoCheck">if set to <see langword="true"/> automatic updates will be enabled</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void ChangeSettings(Collection<Sua> applications, Config options, bool autoCheck);

        /// <summary>Hides a single update</summary>
        /// <param name="hiddenUpdate">The update to hide</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void HideUpdate(Suh hiddenUpdate);

        /// <summary>Hides a collection of <see cref="Suh"/> to hide</summary>
        /// <param name="hiddenUpdates">The collection of updates to hide</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void HideUpdates(Collection<Suh> hiddenUpdates);

        /// <summary>Gets a collection of <see cref="Sui"/></summary>
        /// <param name="appUpdates">The collection of applications and updates to install</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void InstallUpdates(Collection<Sui> appUpdates);

        /// <summary>The update to show and remove from hidden updates</summary>
        /// <param name="hiddenUpdate">The hidden update to show</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void ShowUpdate(Suh hiddenUpdate);

        /// <summary>Subscribes to the WCF service</summary>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void Subscribe();

        /// <summary>Un subscribes from the WCF service</summary>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void Unsubscribe();

        #endregion
    }
}