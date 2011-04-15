// ***********************************************************************
// <copyright file="IElevatedProcessCallback.cs"
//            project="SevenUpdate.Service"
//            assembly="SevenUpdate.Service"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate.Service
{
    using System.ServiceModel;

    using ProtoBuf.ServiceModel;

    /// <summary>Contains callbacks/events to relay back to the server</summary>
    [ServiceContract(Namespace = "http://sevenupdate.com", CallbackContract = typeof(IElevatedProcess))]
    public interface IElevatedProcessCallback
    {
        #region Public Methods

        /// <summary>Occurs when the process starts</summary>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void ElevatedProcessStarted();

        /// <summary>Occurs when the process as exited</summary>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void ElevatedProcessStopped();

        /// <summary>Occurs when the download has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e);

        /// <summary>Occurs when the download progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);

        /// <summary>Occurs when an error occurs</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnErrorOccurred(object sender, ErrorOccurredEventArgs e);

        /// <summary>Occurs when the installation of updates has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnInstallCompleted(object sender, InstallCompletedEventArgs e);

        /// <summary>Occurs when the installation progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        [OperationContract(IsOneWay = false), ProtoBehavior]
        void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e);

        #endregion
    }
}