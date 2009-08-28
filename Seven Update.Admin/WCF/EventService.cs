/*Copyright 2007, 2008 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.ServiceModel;
namespace SevenUpdate.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class EventService : IEventSystem
    {
        #region Callback Delegates

        public delegate void CallbackDelegate();
        public delegate void CallbackDelegate<T>(T t);
        public delegate void ProgressCallbackDelegate<T>(T bytesTransferred, T bytesTotal);
        public delegate void CallbackDelegate<T, Y>(T updateTitle, Y progress, Y updatesCompleted, Y totalUpdates);

        /// <summary>
        /// Raises an event when the client is connected
        /// </summary>
        public static event CallbackDelegate ClientConnected;

        /// <summary>
        /// Raises an event when the client disconnects
        /// </summary>
        public static event CallbackDelegate ClientDisconnected;

        public static CallbackDelegate<string> ErrorOccurred;

        /// <summary>
        /// Raises an event when the download is completed
        /// </summary>
        public static CallbackDelegate<bool> DownloadDone;

        /// <summary>
        /// Raises an event when the installation is completed
        /// </summary>
        public static CallbackDelegate<bool> InstallDone;

        /// <summary>
        /// Raises an event when the installation progress changes
        /// </summary>
        public static CallbackDelegate<string, int> InstallProgressChanged;

        public static ProgressCallbackDelegate<ulong> DownloadProgressChanged;

        #endregion

        #region IEventSystem Members

        /// <summary>
        /// Subscribes to the ESB
        /// </summary>
        public void Subscribe()
        {
            IEventSystemCallback callback = OperationContext.Current.GetCallbackChannel<IEventSystemCallback>();
            InstallDone += callback.OnInstallDone;
            InstallProgressChanged += callback.OnInstallProgressChanged;
            DownloadProgressChanged += callback.OnDownloadProgressChanged;
            DownloadDone += callback.OnDownloadDone;
            ErrorOccurred += callback.OnErrorOccurred;
            ICommunicationObject obj = (ICommunicationObject)callback;
            obj.Closed += new EventHandler(EventService_Closed);
            ClientConnected();
        }

        void EventService_Closed(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Unsubscribes from the client
        /// </summary>
        public void Unsubscribe()
        {
            IEventSystemCallback callback = OperationContext.Current.GetCallbackChannel<IEventSystemCallback>();
            InstallDone -= callback.OnInstallDone;
            InstallProgressChanged -= callback.OnInstallProgressChanged;
            DownloadDone -= callback.OnDownloadDone;
            DownloadProgressChanged -= callback.OnDownloadProgressChanged;
            ClientDisconnected();
        }

        #endregion
    }
}
