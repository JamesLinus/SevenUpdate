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

using System.Collections.ObjectModel;
using System.ServiceModel;
using ProtoBuf.ServiceModel;


#endregion

namespace SevenUpdate.Service
{
    /// <summary>
    ///   Methods for the Event Service
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof (IServiceCallBack))]
    public interface IService
    {
        /// <summary>
        ///   Subscribes to the event service
        /// </summary>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void Subscribe();

        /// <summary>
        ///   UnSubscribes from the event service
        /// </summary>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void UnSubscribe();

        [OperationContract(IsOneWay = true), ProtoBehavior]
        void AddApp(Sua app);

        /// <summary>
        ///   Gets a list containing SUI's
        /// </summary>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void InstallUpdates(Collection<Sui> appUpdates);

        [OperationContract(IsOneWay = true), ProtoBehavior]
        void ShowUpdate(Suh hiddenUpdate);

        [OperationContract(IsOneWay = true), ProtoBehavior]
        void HideUpdate(Suh hiddenUpdate);

        [OperationContract(IsOneWay = true), ProtoBehavior]
        void HideUpdates(Collection<Suh> hiddenUpdates);

        [OperationContract(IsOneWay = true), ProtoBehavior]
        void ChangeSettings(Collection<Sua> apps, Config options, bool autoCheck);
    }

    /// <summary>
    ///   Callback methods for the WCF Service
    /// </summary>
    public interface IServiceCallBack
    {
        /// <summary>
        ///   Occurs when the download has completed
        /// </summary>
        /// <param name = "errorOccurred"><c>true</c> if an error occurred, otherwise <c>false</c></param>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void OnDownloadCompleted(bool errorOccurred);

        /// <summary>
        ///   Occurs when the installation of updates has completed
        /// </summary>
        /// <param name = "updatesInstalled">The number of updates installed</param>
        /// <param name = "updatesFailed">The number of failed updates</param>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void OnInstallCompleted(int updatesInstalled, int updatesFailed);

        /// <summary>
        ///   Occurs when an error occurs
        /// </summary>
        /// <param name = "exception">The exception data</param>
        /// <param name = "type">The <see cref = "ErrorType" /> of the error that occurred</param>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void OnErrorOccurred(string exception, ErrorType type);

        /// <summary>
        ///   Occurs when the download progress has changed
        /// </summary>
        /// <param name = "bytesTransferred">The number of bytes downloaded</param>
        /// <param name = "bytesTotal">The total number of bytes to download</param>
        /// <param name = "filesTransferred">The number of files downloaded</param>
        /// <param name = "filesTotal">The total number of files to download</param>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal);

        /// <summary>
        ///   Occurs when the installation progress has changed
        /// </summary>
        /// <param name = "updateName">The name of the update that is being installed</param>
        /// <param name = "progress">The current update progress</param>
        /// <param name = "updatesComplete">The number of updates that have completed</param>
        /// <param name = "totalUpdates">The total number of updates</param>
        [OperationContract(IsOneWay = true), ProtoBehavior]
        void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates);
    }
}