// ***********************************************************************
// Assembly         : SevenUpdate.Service
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;

    using Microsoft.Win32;

    /// <summary>
    /// Class containing events and delegates for the EventService
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public sealed class WcfService : IService
    {
        #region Constants and Fields

        /// <summary>
        ///   Occurs when the download of updates has completed
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "WCF Service Delegate")]
        public static DownloadCompletedCallbackDelegate DownloadCompleted;

        /// <summary>
        ///   Occurs when the install progress has changed
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "WCF Service Delegate")]
        public static DownloadProgressChangedCallbackDelegate DownloadProgressChanged;

        /// <summary>
        ///   Occurs when a error occurs when downloading or installing updates
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "WCF Service Delegate")]
        public static ErrorOccurredCallbackDelegate ErrorOccurred;

        /// <summary>
        ///   Occurs when the installation of updates has completed
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "WCF Service Delegate")]
        public static InstallCompletedCallbackDelegate InstallCompleted;

        /// <summary>
        ///   Occurs when the install progress has changed
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "WCF Service Delegate")]
        public static InstallProgressCallbackDelegate InstallProgressChanged;

        #endregion

        #region Delegates

        /// <summary>
        /// A callback delegate for a WCF Event
        /// </summary>
        public delegate void CallbackDelegate();

        /// <summary>
        /// A callback Delegate for a WCF Event
        /// </summary>
        /// <param name="apps">
        /// The applications to download
        /// </param>
        public delegate void DownloadCallbackDelegate(Collection<Sui> apps);

        /// <summary>
        /// A callback delegate for the <see cref="DownloadCompleted"/> event
        /// </summary>
        /// <param name="errorOccurred">
        /// <c>true</c> if an error occurred, otherwise <c>false</c>
        /// </param>
        public delegate void DownloadCompletedCallbackDelegate(bool errorOccurred);

        /// <summary>
        /// A callback delegate for the <see cref="DownloadProgressChanged"/> event
        /// </summary>
        /// <param name="bytesTransferred">
        /// The number of bytes downloaded
        /// </param>
        /// <param name="bytesTotal">
        /// The total number of bytes to download
        /// </param>
        /// <param name="filesTransferred">
        /// The number of files downloaded
        /// </param>
        /// <param name="filesTotal">
        /// The total number of files to download
        /// </param>
        public delegate void DownloadProgressChangedCallbackDelegate(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal);

        /// <summary>
        /// A callback delegate for the <see cref="DownloadProgressChanged"/> event
        /// </summary>
        /// <param name="exception">
        /// The exception data
        /// </param>
        /// <param name="type">
        /// The <see cref="ErrorType"/> of the error that occurred
        /// </param>
        public delegate void ErrorOccurredCallbackDelegate(string exception, ErrorType type);

        /// <summary>
        /// A callback delegate for the <see cref="InstallCompleted"/> event
        /// </summary>
        /// <param name="updatesInstalled">
        /// The number of updates installed
        /// </param>
        /// <param name="updatesFailed">
        /// The number of failed updates
        /// </param>
        public delegate void InstallCompletedCallbackDelegate(int updatesInstalled, int updatesFailed);

        /// <summary>
        /// A callback Delegate for a WCF Event
        /// </summary>
        /// <param name="updateName">
        /// The name of the update being installed
        /// </param>
        /// <param name="progress">
        /// The progress of the update being installed
        /// </param>
        /// <param name="updatesCompleted">
        /// The number of updates completed
        /// </param>
        /// <param name="totalUpdates">
        /// The total number of updates being installed
        /// </param>
        public delegate void InstallProgressCallbackDelegate(string updateName, int progress, int updatesCompleted, int totalUpdates);

        #endregion

        #region Events

        /// <summary>
        ///   Raises an event when the client is connected
        /// </summary>
        public static event CallbackDelegate ClientConnected;

        /// <summary>
        ///   Raises an event when the client disconnects
        /// </summary>
        public static event CallbackDelegate ClientDisconnected;

        /// <summary>
        ///   Raises an event when the client disconnects
        /// </summary>
        public static event DownloadCallbackDelegate DownloadUpdates;

        #endregion

        #region Implemented Interfaces

        #region IService

        /// <summary>
        /// Adds an application to Seven Update, so it can manage updates for it.
        /// </summary>
        /// <param name="app">
        /// The application to add to Seven Update
        /// </param>
        public void AddApp(Sua app)
        {
            var sul = Utilities.Deserialize<Collection<Sua>>(Utilities.AppsFile);
            var exists = false;

            foreach (var t in sul.Where(t => t.Directory == app.Directory && t.Is64Bit == app.Is64Bit))
            {
                exists = true;
            }

            if (exists)
            {
                return;
            }

            sul.Add(app);

            Utilities.Serialize(sul, Utilities.AppsFile);
        }

        /// <summary>
        /// Changes the program settings
        /// </summary>
        /// <param name="apps">
        /// The applications Seven Update will check and manage updates for
        /// </param>
        /// <param name="options">
        /// The Seven Update settings
        /// </param>
        /// <param name="autoCheck">
        /// if set to <see langword="true"/> automatic updates will be enabled
        /// </param>
        public void ChangeSettings(Collection<Sua> apps, Config options, bool autoCheck)
        {
            if (!autoCheck)
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    // ReSharper disable PossibleNullReferenceException
                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Seven Update Automatic Checking", false);

                    // ReSharper restore PossibleNullReferenceException
                }
                else
                {
                    Utilities.StartProcess(@"schtasks.exe", "/Change /Disable /TN \"SevenUpdate.Admin\"");
                }
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    Registry.SetValue(
                        @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", 
                        "Seven Update Automatic Checking", 
                        Utilities.AppDir + @"SevenUpdate.Helper.exe ");
                }
                else
                {
                    Utilities.StartProcess(@"schtasks.exe", "/Change /Enable /TN \"SevenUpdate.Admin\"");
                }
            }

            Utilities.Serialize(apps, Utilities.AppsFile);
            Utilities.Serialize(options, Utilities.ConfigFile);
        }

        /// <summary>
        /// Hides a single update
        /// </summary>
        /// <param name="hiddenUpdate">
        /// The update to hide
        /// </param>
        public void HideUpdate(Suh hiddenUpdate)
        {
            var hidden = Utilities.Deserialize<Collection<Suh>>(Utilities.HiddenFile) ?? new Collection<Suh>();
            hidden.Add(hiddenUpdate);

            Utilities.Serialize(hidden, Utilities.HiddenFile);
        }

        /// <summary>
        /// Hides a collection of <see cref="Suh"/> to hide
        /// </summary>
        /// <param name="hiddenUpdates">
        /// The collection of updates to hide
        /// </param>
        public void HideUpdates(Collection<Suh> hiddenUpdates)
        {
            Utilities.Serialize(hiddenUpdates, Utilities.HiddenFile);
        }

        /// <summary>
        /// Gets a collection of <see cref="Sui"/>
        /// </summary>
        /// <param name="appUpdates">
        /// The collection of applications and updates to install
        /// </param>
        public void InstallUpdates(Collection<Sui> appUpdates)
        {
            try
            {
                if (File.Exists(Utilities.AllUserStore + "abort.lock"))
                {
                    File.Delete(Utilities.AllUserStore + "abort.lock");
                }
            }
            catch (Exception f)
            {
                Utilities.ReportError(f, Utilities.AllUserStore);
            }

            if (DownloadUpdates != null)
            {
                DownloadUpdates(appUpdates);
            }
        }

        /// <summary>
        /// The update to show and remove from hidden updates
        /// </summary>
        /// <param name="hiddenUpdate">
        /// The hidden update to show
        /// </param>
        public void ShowUpdate(Suh hiddenUpdate)
        {
            var show = Utilities.Deserialize<Collection<Suh>>(Utilities.HiddenFile) ?? new Collection<Suh>();

            if (show.Count == 0)
            {
                File.Delete(Utilities.HiddenFile);
            }
            else
            {
                show.Remove(hiddenUpdate);
                Utilities.Serialize(show, Utilities.HiddenFile);
            }
        }

        /// <summary>
        /// Subscribes to the WCF service
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
            if (ClientConnected != null)
            {
                ClientConnected();
            }
        }

        /// <summary>
        /// Un subscribes from the wcf service
        /// </summary>
        public void UNSubscribe()
        {
            InstallCompleted = null;
            InstallProgressChanged = null;
            DownloadCompleted = null;
            DownloadProgressChanged = null;
            if (ClientDisconnected != null)
            {
                ClientDisconnected();
            }
        }

        #endregion

        #endregion
    }
}