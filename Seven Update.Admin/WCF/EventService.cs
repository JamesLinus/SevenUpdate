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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class EventService : IEventSystem
    {
        #region Delegates

        public delegate void CallbackDelegate();

        public delegate void CallbackDelegate<T>(T t);

        public delegate void CallbackDelegate<T, TY>(T t, TY y);

        public delegate void CallbackDelegate<T, TY, TZ>(T t, TY y, TZ z);

        public delegate void InstallProgressCallbackDelegate(string updateTitle, int progress, int updatesCompleted, int totalUpdates);

        #endregion

        /// <summary>
        /// Raises an event when the download is completed
        /// </summary>
        public static CallbackDelegate<bool> DownloadDone;

        public static CallbackDelegate<ulong, ulong> DownloadProgressChanged;
        public static CallbackDelegate<string, ErrorType> ErrorOccurred;

        /// <summary>
        /// Raises an event when the installation is completed
        /// </summary>
        public static CallbackDelegate<int, int> InstallDone;

        /// <summary>
        /// Raises an event when the installation progress changes
        /// </summary>
        public static InstallProgressCallbackDelegate InstallProgressChanged;

        #region IEventSystem Members

        /// <summary>
        /// Subscribes to the WCF
        /// </summary>
        public void Subscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IEventSystemCallback>();
            InstallDone += callback.OnInstallDone;
            InstallProgressChanged += callback.OnInstallProgressChanged;
            DownloadProgressChanged += callback.OnDownloadProgressChanged;
            DownloadDone += callback.OnDownloadDone;
            ErrorOccurred += callback.OnErrorOccurred;
            var obj = (ICommunicationObject) callback;
            obj.Closed += EventService_Closed;
            ClientConnected();
        }

        /// <summary>
        /// Unsubscribes from the client
        /// </summary>
        public void Unsubscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IEventSystemCallback>();
            InstallDone -= callback.OnInstallDone;
            InstallProgressChanged -= callback.OnInstallProgressChanged;
            DownloadDone -= callback.OnDownloadDone;
            DownloadProgressChanged -= callback.OnDownloadProgressChanged;
            ClientDisconnected();
        }

        #endregion

        /// <summary>
        /// Raises an event when the client is connected
        /// </summary>
        public static event CallbackDelegate ClientConnected;

        /// <summary>
        /// Raises an event when the client disconnects
        /// </summary>
        public static event CallbackDelegate ClientDisconnected;

        private static void EventService_Closed(object sender, EventArgs e) {}
    }
}