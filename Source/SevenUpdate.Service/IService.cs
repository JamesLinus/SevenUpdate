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
    public interface IServiceCallback
    {
        #region Public Methods

        /// <summary>Occurs when the download has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e);

        /// <summary>Occurs when the download progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);

        /// <summary>Occurs when an error occurs</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnErrorOccurred(object sender, ErrorOccurredEventArgs e);

        /// <summary>Occurs when the installation of updates has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnInstallCompleted(object sender, InstallCompletedEventArgs e);

        /// <summary>Occurs when the installation progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = true)]
        [ProtoBehavior]
        void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e);

        #endregion
    }

    /// <summary>Methods for the Event Service</summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IServiceCallback))]
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