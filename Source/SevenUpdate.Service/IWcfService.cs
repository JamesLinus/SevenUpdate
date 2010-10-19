// ***********************************************************************
// <copyright file="IWcfService.cs"
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

    /// <summary>Contains callbacks/events to relay back to the server</summary>
    [ServiceContract(Namespace = "http://sevenupdate.com", CallbackContract = typeof(IElevatedProcess))]
    public interface IElevatedProcessCallback
    {
        /// <summary>Occurs when the process starts</summary>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void ElevatedProcessStarted();

        /// <summary>Occurs when the process as exited</summary>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void ElevatedProcessStopped();

        /// <summary>Occurs when the download has completed</summary>
        /// <param name = "sender">The sender of the event</param>
        /// <param name = "e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e);

        /// <summary>Occurs when the download progress has changed</summary>
        /// <param name = "sender">The sender of the event</param>
        /// <param name = "e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);

        /// <summary>Occurs when an error occurs</summary>
        /// <param name = "sender">The sender of the event</param>
        /// <param name = "e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnErrorOccurred(object sender, ErrorOccurredEventArgs e);

        /// <summary>Occurs when the installation of updates has completed</summary>
        /// <param name = "sender">The sender of the event</param>
        /// <param name = "e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnInstallCompleted(object sender, InstallCompletedEventArgs e);

        /// <summary>Occurs when the installation progress has changed</summary>
        /// <param name = "sender">The sender of the event</param>
        /// <param name = "e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e);
    }

    /// <summary>Contains callbacks/events to relay back to the client</summary>
    [ServiceContract(Namespace = "http://sevenupdate.com")]
    public interface IElevatedProcess
    {
        /// <summary>Adds an application to Seven Update, so it can manage updates for it.</summary>
        /// <param name = "application">The application to add</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void AddApp(Sua application);

        /// <summary>Changes the program settings</summary>
        /// <param name = "applications">The applications to enable update checking</param>
        /// <param name = "options">The Seven Update settings</param>
        /// <param name = "autoCheck">if set to <see langword = "true" /> automatic updates will be enabled</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void ChangeSettings(Collection<Sua> applications, Config options, bool autoCheck);

        /// <summary>Hides a single update</summary>
        /// <param name = "hiddenUpdate">The update to hide</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void HideUpdate(Suh hiddenUpdate);

        /// <summary>Hides a collection of <see cref = "Suh" /> to hide</summary>
        /// <param name = "hiddenUpdates">The collection of updates to hide</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void HideUpdates(Collection<Suh> hiddenUpdates);

        /// <summary>Gets a collection of <see cref = "Sui" /></summary>
        /// <param name = "applicationUpdates">The collection of applications and updates to install</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void InstallUpdates(Collection<Sui> applicationUpdates);

        /// <summary>The update to show and remove from hidden updates</summary>
        /// <param name = "hiddenUpdate">The hidden update to show</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void ShowUpdate(Suh hiddenUpdate);

        /// <summary>Requests shutdown of the admin process. App will only shutdown if it's not installing updates. To shutdown when updates are being installed, execute the admin process with the 'Abort' argument</summary>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void Shutdown();
    }
}