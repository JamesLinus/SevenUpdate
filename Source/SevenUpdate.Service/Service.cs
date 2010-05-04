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

using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.Service
{
    /// <summary>
    ///   Class containing events and delegates for the EventService
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class Service : IService
    {
        #region Events

        public static event EventHandler<OnSettingsChangedEventArgs> OnSettingsChanged;

        public static event EventHandler<OnSetUpdatesEventArgs> OnSetUpdates;

        public static event EventHandler<OnAddAppEventArgs> OnAddApp;

        public static event EventHandler<OnShowUpdateEventArgs> OnShowUpdate;

        public static event EventHandler<OnHideUpdateEventArgs> OnHideUpdate;

        public static event EventHandler<OnHideUpdatesEventArgs> OnHideUpdates;

        #endregion

        #region Event Args

        #region Nested type: OnAddAppEventArgs

        /// <summary>
        ///   Provides event data the AddApp event
        /// </summary>
        public sealed class OnAddAppEventArgs : EventArgs
        {
            /// <summary>
            ///   Contains event data associated with this event
            /// </summary>
            public OnAddAppEventArgs(Sua app)
            {
                App = app;
            }

            /// <summary>
            ///   The app to add to the Seven Update list
            /// </summary>
            public Sua App { get; private set; }
        }

        #endregion

        #region Nested type: OnHideUpdateEventArgs

        /// <summary>
        ///   Provides event data the HideUpdate event
        /// </summary>
        public sealed class OnHideUpdateEventArgs : EventArgs
        {
            /// <summary>
            ///   Contains event data associated with this event
            /// </summary>
            public OnHideUpdateEventArgs(Suh hiddenUpdate)
            {
                HiddenUpdate = hiddenUpdate;
            }

            /// <summary>
            ///   The app to hide
            /// </summary>
            public Suh HiddenUpdate { get; private set; }
        }

        #endregion

        #region Nested type: OnHideUpdatesEventArgs

        /// <summary>
        ///   Provides event data the HideUpdate event
        /// </summary>
        public sealed class OnHideUpdatesEventArgs : EventArgs
        {
            /// <summary>
            ///   Contains event data associated with this event
            /// </summary>
            public OnHideUpdatesEventArgs(Collection<Suh> hiddenUpdates)
            {
                HiddenUpdates = hiddenUpdates;
            }

            /// <summary>
            ///   The app to hide
            /// </summary>
            public Collection<Suh> HiddenUpdates { get; private set; }
        }

        #endregion

        #region Nested type: OnSetUpdatesEventArgs

        /// <summary>
        ///   Provides event data the SetUpdate event
        /// </summary>
        public sealed class OnSetUpdatesEventArgs : EventArgs
        {
            /// <summary>
            ///   Contains event data associated with this event
            /// </summary>
            public OnSetUpdatesEventArgs(Collection<Sui> appUpdates)
            {
                AppUpdates = appUpdates;
            }

            /// <summary>
            ///   The apps to update
            /// </summary>
            public Collection<Sui> AppUpdates { get; private set; }
        }

        #endregion

        #region Nested type: OnSettingsChangedEventArgs

        /// <summary>
        ///   Provides event data the OnSettingsChanged event
        /// </summary>
        public sealed class OnSettingsChangedEventArgs : EventArgs
        {
            /// <summary>
            ///   Contains event data associated with this event
            /// </summary>
            public OnSettingsChangedEventArgs(Collection<Sua> apps, Config options, bool autoOn)
            {
                Apps = apps;
                Options = options;
                AutoOn = autoOn;
            }

            /// <summary>
            ///   The apps that Seven Update will update
            /// </summary>
            public Collection<Sua> Apps { get; private set; }

            /// <summary>
            ///   The apps that Seven Update will update
            /// </summary>
            public Config Options { get; private set; }

            /// <summary>
            ///   Gets or Sets a value indicating if auto updates should be enabled
            /// </summary>
            public bool AutoOn { get; private set; }
        }

        #endregion

        #region Nested type: OnShowUpdateEventArgs

        /// <summary>
        ///   Provides event data the ShowUpdate event
        /// </summary>
        public sealed class OnShowUpdateEventArgs : EventArgs
        {
            /// <summary>
            ///   Contains event data associated with this event
            /// </summary>
            public OnShowUpdateEventArgs(Suh hiddenUpdate)
            {
                HiddenUpdate = hiddenUpdate;
            }

            /// <summary>
            ///   The app to unhide
            /// </summary>
            public Suh HiddenUpdate { get; private set; }
        }

        #endregion

        #endregion

        #region Delegates

        /// <summary>
        ///   A callback delegate for a WCF Event
        /// </summary>
        public delegate void CallbackDelegate();

        /// <summary>
        ///   A callback delegate for the <see cref = "DownloadCompleted" /> event
        /// </summary>
        /// <param name = "errorOccurred"><c>true</c> if an error occurred, otherwise <c>false</c></param>
        public delegate void DownloadCompletedCallbackDelegate(bool errorOccurred);

        /// <summary>
        ///   A callback delegate for the <see cref = "DownloadProgressChanged" /> event
        /// </summary>
        /// <param name = "bytesTransferred">The number of bytes downloaded</param>
        /// <param name = "bytesTotal">The total number of bytes to download</param>
        /// <param name = "filesTransferred">The number of files downloaded</param>
        /// <param name = "filesTotal">The total number of files to download</param>
        public delegate void DownloadProgressChangedCallbackDelegate(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal);

        /// <summary>
        ///   A callback delegate for the <see cref = "DownloadProgressChanged" /> event
        /// </summary>
        /// <param name = "exception">The exception data</param>
        /// <param name = "type">The <see cref = "ErrorType" /> of the error that occurred</param>
        public delegate void ErrorOccurredCallbackDelegate(string exception, ErrorType type);

        /// <summary>
        ///   A callback delegate for the <see cref = "InstallCompleted" /> event
        /// </summary>
        /// <param name = "updatesInstalled">The number of updates installed</param>
        /// <param name = "updatesFailed">The number of failed updates</param>
        public delegate void InstallCompletedCallbackDelegate(int updatesInstalled, int updatesFailed);

        /// <summary>
        ///   A callback Delegate for a WCF Event
        /// </summary>
        /// <param name = "updateName">The name of the update being installed</param>
        /// <param name = "progress">The progress of the update being installed</param>
        /// <param name = "updatesCompleted">The number of updates completed</param>
        /// <param name = "totalUpdates">The total number of updates being installed</param>
        public delegate void InstallProgressCallbackDelegate(string updateName, int progress, int updatesCompleted, int totalUpdates);

        #endregion

        #region Callbacks

        /// <summary>
        ///   Occurs when the download of updates has completed
        /// </summary>
        public static DownloadCompletedCallbackDelegate DownloadCompleted;

        /// <summary>
        ///   Occurs when the install progress has changed
        /// </summary>
        public static DownloadProgressChangedCallbackDelegate DownloadProgressChanged;

        /// <summary>
        ///   Occurs when a error occurs when downloading or installing updates
        /// </summary>
        public static ErrorOccurredCallbackDelegate ErrorOccurred;

        /// <summary>
        ///   Occurs when the installation of updates has completed
        /// </summary>
        public static InstallCompletedCallbackDelegate InstallCompleted;

        /// <summary>
        ///   Occurs when the install progress has changed
        /// </summary>
        public static InstallProgressCallbackDelegate InstallProgressChanged;

        /// <summary>
        ///   Raises an event when the client is connected
        /// </summary>
        public static event CallbackDelegate ClientConnected;

        /// <summary>
        ///   Raises an event when the client disconnects
        /// </summary>
        public static event CallbackDelegate ClientDisconnected;

        #endregion

        #region IService Members

        /// <summary>
        ///   Subscribes to the WCF
        /// </summary>
        public void Subscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IServiceCallBack>();
            InstallCompleted += callback.OnInstallCompleted;
            InstallProgressChanged += callback.OnInstallProgressChanged;
            DownloadProgressChanged += callback.OnDownloadProgressChanged;
            DownloadCompleted += callback.OnDownloadCompleted;
            ErrorOccurred += callback.OnErrorOccurred;
            ClientConnected();
        }

        /// <summary>
        ///   Unsubscribes from the client
        /// </summary>
        public void UnSubscribe()
        {
            InstallCompleted = null;
            InstallProgressChanged = null;
            DownloadCompleted = null;
            DownloadProgressChanged = null;
            ClientDisconnected();
        }

        public void AddApp(Sua app)
        {
            if (OnAddApp != null)
                OnAddApp(this, new OnAddAppEventArgs(app));
        }

        public void SetUpdates(Collection<Sui> appUpdates)
        {
            if (OnSetUpdates != null)
                OnSetUpdates(this, new OnSetUpdatesEventArgs(appUpdates));
        }

        public void ShowUpdate(Suh hiddenUpdate)
        {
            if (OnShowUpdate != null)
                OnShowUpdate(this, new OnShowUpdateEventArgs(hiddenUpdate));
        }

        public void HideUpdate(Suh hiddenUpdate)
        {
            if (OnHideUpdate != null)
                OnHideUpdate(this, new OnHideUpdateEventArgs(hiddenUpdate));
        }

        public void HideUpdates(Collection<Suh> hiddenUpdates)
        {
            if (OnHideUpdates != null)
                OnHideUpdates(this, new OnHideUpdatesEventArgs(hiddenUpdates));
        }

        public void ChangeSettings(Collection<Sua> apps, Config options, bool autoOn)
        {
            if (OnSettingsChanged != null)
                OnSettingsChanged(this, new OnSettingsChangedEventArgs(apps, options, autoOn));
        }

        #endregion
    }
}