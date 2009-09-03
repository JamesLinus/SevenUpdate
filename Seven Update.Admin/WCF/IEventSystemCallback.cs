#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
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
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.ServiceModel;

#endregion

namespace SevenUpdate.WCF
{
    internal interface IEventSystemCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnDownloadDone(bool errorOccurred);

        [OperationContract(IsOneWay = true)]
        void OnInstallDone(int updatesInstalled, int updatesFailed);

        [OperationContract(IsOneWay = true)]
        void OnErrorOccurred(Exception e, ErrorType type);

        [OperationContract(IsOneWay = true)]
        void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal);

        [OperationContract(IsOneWay = true)]
        void OnInstallProgressChanged(string updateTitle, int progress, int updatesComplete, int totalUpdates);
    }
}