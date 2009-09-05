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
    /// <summary>
    /// Class containing events and delegates for the EventService
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class EventService : IEventSystem
    {
        #region Delegates

        /// <summary>
        /// A callback Delegate for a WCF Event
        /// </summary>
        public delegate void CallbackDelegate();

        /// <summary>
        /// A callback Delegate for a WCF Event
        /// </summary>
        /// <typeparam name="T">The argument Type</typeparam>
        /// <param name="t">An argument</param>
        public delegate void CallbackDelegate<T>(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The argument Type</typeparam>
        /// <typeparam name="TY">The argument Type</typeparam>
        /// <param name="t">An argument</param>
        /// <param name="y">An argument</param>
        public delegate void CallbackDelegate<T, TY>(T t, TY y);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The argument Type</typeparam>
        /// <typeparam name="TY">The argument Type</typeparam>
        /// <typeparam name="TZ">The argument Type</typeparam>
        /// <param name="t">An argument</param>
        /// <param name="y">An argument</param>
        /// <param name="z">An argument</param>
        public delegate void CallbackDelegate<T, TY, TZ>(T t, TY y, TZ z);

        /// <summary>
        /// A callback Delegate for a WCF Event
        /// </summary>
        /// <param name="updateName">The name of the update being installed</param>
        /// <param name="progress">The progress of the update being installed</param>
        /// <param name="updatesCompleted">The number of updates completed</param>
        /// <param name="totalUpdates">The total number of updates being installed</param>
        public delegate void InstallProgressCallbackDelegate(string updateName, int progress, int updatesCompleted, int totalUpdates);

        #endregion

        /// <summary>
        /// Occurs when the download of updates has completed
        /// </summary>
        public static CallbackDelegate<bool> DownloadCompleted;

        /// <summary>
        /// Occurs when the install progress has changed
        /// </summary>
        public static CallbackDelegate<ulong, ulong> DownloadProgressChanged;

        /// <summary>
        /// Occurs when a error occurs when downloading or installing updates
        /// </summary>
        public static CallbackDelegate<Exception, ErrorType> ErrorOccurred;

        /// <summary>
        /// Occurs when the installation of updates has completed
        /// </summary>
        public static CallbackDelegate<int, int> InstallCompleted;

        /// <summary>
        /// Occurs when the install progress has changed
        /// </summary>
        public static InstallProgressCallbackDelegate InstallProgressChanged;

        #region IEventSystem Members

        /// <summary>
        /// Subscribes to the WCF
        /// </summary>
        public void Subscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IEventSystemCallback>();
            InstallCompleted += callback.OnInstallCompleted;
            InstallProgressChanged += callback.OnInstallProgressChanged;
            DownloadProgressChanged += callback.OnDownloadProgressChanged;
            DownloadCompleted += callback.OnDownloadCompleted;
            ErrorOccurred += callback.OnErrorOccurred;
            var obj = (ICommunicationObject) callback;
            obj.Closed += EventService_Closed;
            ClientConnected();
        }

        /// <summary>
        /// unsubscribes from the client
        /// </summary>
        public void UnSubscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IEventSystemCallback>();
            InstallCompleted -= callback.OnInstallCompleted;
            InstallProgressChanged -= callback.OnInstallProgressChanged;
            DownloadCompleted -= callback.OnDownloadCompleted;
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

        /// <summary>
        /// Occurs when the <see cref="EventService"/> had closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EventService_Closed(object sender, EventArgs e) {}
    }
}