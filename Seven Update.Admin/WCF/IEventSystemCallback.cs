#region GNU Public License v3

// Copyright 2007-2010 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
//  
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//  
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.ServiceModel;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.Admin.WCF
{
    /// <summary>
    /// Callback methods for the WCF Service
    /// </summary>
    internal interface IEventSystemCallback
    {
        /// <summary>
        /// Occurs when the download has completed
        /// </summary>
        /// <param name="errorOccurred"><c>true</c> if an error occurred, otherwise <c>false</c></param>
        [OperationContract(IsOneWay = true)]
        void OnDownloadCompleted(bool errorOccurred);

        /// <summary>
        /// Occurs when the installation of updates has completed
        /// </summary>
        /// <param name="updatesInstalled">The number of updates installed</param>
        /// <param name="updatesFailed">The number of failed updates</param>
        [OperationContract(IsOneWay = true)]
        void OnInstallCompleted(int updatesInstalled, int updatesFailed);

        /// <summary>
        /// Occurs when an error occurs
        /// </summary>
        /// <param name="exception">The exception data</param>
        /// <param name="type">The <see cref="ErrorType" /> of the error that occurred</param>
        [OperationContract(IsOneWay = true)]
        void OnErrorOccurred(Exception exception, ErrorType type);

        /// <summary>
        /// Occurs when the download progress has changed
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes downloaded</param>
        /// <param name="bytesTotal">The total number of bytes to download</param>
        [OperationContract(IsOneWay = true)]
        void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal);

        /// <summary>
        /// Occurs when the installation progress has changed
        /// </summary>
        /// <param name="updateName">The name of the update that is being installed</param>
        /// <param name="progress">The current update progress</param>
        /// <param name="updatesComplete">The number of updates that have completed</param>
        /// <param name="totalUpdates">The total number of updates</param>
        [OperationContract(IsOneWay = true)]
        void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates);
    }
}