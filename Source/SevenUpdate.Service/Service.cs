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
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Win32;

#endregion

namespace SevenUpdate.Service
{
    /// <summary>
    ///   Class containing events and delegates for the EventService
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public sealed class Service : IService
    {
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

            InstallCompleted -= callback.OnInstallCompleted;
            InstallProgressChanged -= callback.OnInstallProgressChanged;
            DownloadProgressChanged -= callback.OnDownloadProgressChanged;
            DownloadCompleted -= callback.OnDownloadCompleted;
            ErrorOccurred -= callback.OnErrorOccurred;

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
            var sul = Base.Deserialize<Collection<Sua>>(Base.AppsFile);
            var exists = false;

            foreach (var t in sul.Where(t => t.Directory == app.Directory && t.Is64Bit == app.Is64Bit))
                exists = true;
            if (exists)
                return;
            sul.Add(app);

            Base.Serialize(sul, Base.AppsFile);
        }

        public void InstallUpdates(Collection<Sui> appUpdates)
        {
            try
            {
                if (File.Exists(Base.AllUserStore + "abort.lock"))
                    File.Delete(Base.AllUserStore + "abort.lock");
            }
            catch (Exception f)
            {
                Base.ReportError(f, Base.AllUserStore);
            }
            Task.Factory.StartNew(() => Download.DownloadUpdates(appUpdates));
        }

        public void ShowUpdate(Suh hiddenUpdate)
        {
            var show = Base.Deserialize<Collection<Suh>>(Base.HiddenFile) ?? new Collection<Suh>();

            if (show.Count == 0)
                File.Delete(Base.HiddenFile);
            else
            {
                show.Remove(hiddenUpdate);
                Base.Serialize(show, Base.HiddenFile);
            }
        }

        public void HideUpdate(Suh hiddenUpdate)
        {
            var hidden = Base.Deserialize<Collection<Suh>>(Base.HiddenFile) ?? new Collection<Suh>();
            hidden.Add(hiddenUpdate);

            Base.Serialize(hidden, Base.HiddenFile);
        }

        public void HideUpdates(Collection<Suh> hiddenUpdates)
        {
            Base.Serialize(hiddenUpdates, Base.HiddenFile);
        }

        public void ChangeSettings(Collection<Sua> apps, Config options, bool autoOn)
        {
            if (!autoOn)
            {
                if (Environment.OSVersion.Version.Major < 6)
                    // ReSharper disable PossibleNullReferenceException
                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Seven Update Automatic Checking", false);
                    // ReSharper restore PossibleNullReferenceException
                else
                    Base.StartProcess("schtasks.exe", "/Change /Disable /TN \"SevenUpdate.Admin\"");
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", "Seven Update Automatic Checking", Base.AppDir + @"SevenUpdate.Helper.exe ");
                else
                    Base.StartProcess("schtasks.exe", "/Change /Enable /TN \"SevenUpdate.Admin\"");
            }
            Base.Serialize(apps, Base.AppsFile);
            Base.Serialize(options, Base.ConfigFile);
        }

        #endregion
    }
}