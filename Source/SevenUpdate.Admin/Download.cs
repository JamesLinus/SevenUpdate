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
using System.IO;
using System.Linq;
using System.Windows;
using SevenUpdate.Base;
using SharpBits.Base;

#endregion

namespace SevenUpdate.Admin
{
    /// <summary>
    ///   A class containing methods to download updates
    /// </summary>
    internal static class Download
    {
        #region Global Vars

        /// <summary>
        ///   Manager for Background Intelligent Transfer Service
        /// </summary>
        private static BitsManager manager;

        #endregion

        #region Download Methods

        /// <summary>
        ///   Downloads the updates using BITS
        /// </summary>
        /// <param name = "App.AppUpdates">the Collection of App.AppUpdates and updates</param>
        /// <param name = "priority">the Priority of the download</param>
        internal static void DownloadUpdates(JobPriority priority)
        {
            if (App.AppUpdates == null)
            {
                if (Service.Service.ErrorOccurred != null && App.IsClientConnected)
                    Service.Service.ErrorOccurred(@"Error recieving Sui collection from the WCF wire", ErrorType.DownloadError);

                Base.Base.ReportError(@"Error recieving Sui collection from the WCF wir", Base.Base.AllUserStore);
                App.ShutdownApp();
            }
            else
            {
                if (App.AppUpdates.Count < 1)
                {
                    if (Service.Service.ErrorOccurred != null && App.IsClientConnected)
                        Service.Service.ErrorOccurred(@"Error recieving Sui collection from the WCF wire", ErrorType.DownloadError);

                    Base.Base.ReportError(@"Error recieving Sui collection from the WCF wire", Base.Base.AllUserStore);
                    App.ShutdownApp();
                }
            }

            // It's a new manager class
            manager = new BitsManager();

            // Assign the event handlers
            manager.OnJobTransferred += ManagerOnJobTransferred;
            manager.OnJobError += ManagerOnJobError;
            manager.OnJobModified += ManagerOnJobModified;

            // Load the BITS Jobs for the entire machine and current user
            try
            {
                manager.EnumJobs(JobOwner.CurrentUser);
                manager.EnumJobs(JobOwner.AllUsers);
            }
            catch
            {
            }

            // Loops through the jobs, if a Seven Update job is found, try to resume, if not 
            foreach (var job in manager.Jobs.Values.Where(job => job.DisplayName == "SevenUpdate"))
            {
                try
                {
                    job.EnumFiles();
                }
                catch
                {
                }
                if (job.Files.Count < 1 || (job.State != JobState.Transferring && job.State != JobState.Suspended))
                {
                    try
                    {
                        job.Cancel();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        job.Resume();
                        return;
                    }
                    catch
                    {
                        try
                        {
                            job.Cancel();
                        }
                        catch
                        {
                        }
                    }
                }
            }

            var bitsJob = manager.CreateJob("SevenUpdate", JobType.Download);
            bitsJob.NotificationFlags = NotificationFlags.JobErrorOccured | NotificationFlags.JobModified | NotificationFlags.JobTransferred;
            bitsJob.Priority = priority;
            bitsJob.NoProgressTimeout = 60;
            bitsJob.MinimumRetryDelay = 60;
            for (var x = 0; x < App.AppUpdates.Count; x++)
            {
                for (var y = 0; y < App.AppUpdates[x].Updates.Count; y++)
                {
                    // Create download directory consisting of appname and update title
                    var downloadDir = Base.Base.AllUserStore + @"downloads\" + App.AppUpdates[x].Updates[y].Name[0].Value;

                    Directory.CreateDirectory(downloadDir);

                    for (var z = 0; z < App.AppUpdates[x].Updates[y].Files.Count; z++)
                    {
                        var fileDestination = App.AppUpdates[x].Updates[y].Files[z].Destination;

                        if (App.AppUpdates[x].Updates[y].Files[z].Action == FileAction.Delete || App.AppUpdates[x].Updates[y].Files[z].Action == FileAction.UnregisterThenDelete ||
                            App.AppUpdates[x].Updates[y].Files[z].Action == FileAction.CompareOnly)
                            continue;
                        if (Base.Base.GetHash(downloadDir + @"\" + Path.GetFileName(fileDestination)) == App.AppUpdates[x].Updates[y].Files[z].Hash)
                            continue;
                        try
                        {
                            try
                            {
                                File.Delete(downloadDir + @"\" + Path.GetFileName(fileDestination));
                            }
                            catch
                            {
                            }
                            var url = new Uri(Base.Base.ConvertPath(App.AppUpdates[x].Updates[y].Files[z].Source, App.AppUpdates[x].Updates[y].DownloadUrl, App.AppUpdates[x].AppInfo.Is64Bit));

                            bitsJob.AddFile(url.AbsoluteUri, downloadDir + @"\" + Path.GetFileName(fileDestination));
                        }
                        catch (Exception e)
                        {
                            if (Service.Service.ErrorOccurred != null && App.IsClientConnected)
                                Service.Service.ErrorOccurred(e.Message, ErrorType.DownloadError);
                            Base.Base.ReportError(e, Base.Base.AllUserStore);
                        }
                    }
                }
            }

            try
            {
                bitsJob.EnumFiles();
            }
            catch
            {
            }

            if (bitsJob.Files.Count > 0)
            {
                try
                {
                    bitsJob.Resume();
                }
                catch (Exception e)
                {
                    Base.Base.ReportError(e, Base.Base.AllUserStore);
                    if (Service.Service.ErrorOccurred != null && App.IsClientConnected)
                        Service.Service.ErrorOccurred(e.Message, ErrorType.DownloadError);
                    App.ShutdownApp();
                }
            }
            else
            {
                manager.Dispose();
                manager = null;
                Install.InstallUpdates();
            }
        }

        #endregion

        #region Events

        #region Event Handlers

        /// <summary>
        ///   Calls the DownloadProgressChanged Event and updates the
        ///   <see cref = "System.Windows.Forms.NotifyIcon" />
        ///   when the job progress has changed
        /// </summary>
        private static void ManagerOnJobModified(object sender, NotificationEventArgs e)
        {
            if (File.Exists(Base.Base.AllUserStore + "abort.lock"))
            {
                File.Delete(Base.Base.AllUserStore + "abort.lock");
                App.ShutdownApp();
            }

            if (e.Job == null)
                return;

            if (e.Job.DisplayName != "SevenUpdate")
                return;

            if (e.Job.State == JobState.Error)
                return;

            if (Service.Service.DownloadProgressChanged != null && App.IsClientConnected)
                Service.Service.DownloadProgressChanged(e.Job.Progress.BytesTransferred, e.Job.Progress.BytesTotal, e.Job.Progress.FilesTransferred, e.Job.Progress.FilesTotal);

            if (App.NotifyIcon == null)
                return;

            if (e.Job.Progress.BytesTotal > 0 && e.Job.Progress.BytesTransferred > 0)
            {
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                           App.RM.GetString("DownloadingUpdates") + " (" + Base.Base.ConvertFileSize(e.Job.Progress.BytesTotal) + ", " +
                                                           (e.Job.Progress.BytesTransferred*100/e.Job.Progress.BytesTotal).ToString("F0") + " % " + App.RM.GetString("Complete") +
                                                           ")");
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                           App.RM.GetString("DownloadingUpdates") + " (" + e.Job.Progress.FilesTransferred + " " + App.RM.GetString("OutOf") + " " +
                                                           e.Job.Progress.FilesTotal + " " + App.RM.GetString("Files") + " " + App.RM.GetString("Complete") + ")");
            }
        }

        /// <summary>
        ///   Calls the ErrorOccurred Event and updates the
        ///   <see cref = "System.Windows.Forms.NotifyIcon" />
        ///   when an error occurs
        /// </summary>
        private static void ManagerOnJobError(object sender, ErrorNotificationEventArgs e)
        {
            if (e.Job == null)
                return;

            if (e.Job.DisplayName != "SevenUpdate")
                return;

            if (e.Job.State != JobState.Error)
                return;

            if (e.Job.Error.File != null)
            {
                Base.Base.ReportError(e.Job.Error.File.RemoteName + " - " + e.Job.Error.Description, Base.Base.AllUserStore);
                if (Service.Service.ErrorOccurred != null && App.IsClientConnected)
                    Service.Service.ErrorOccurred(e.Job.Error.File.RemoteName + " - " + e.Job.Error.Description, ErrorType.DownloadError);
            }
            else
            {
                Base.Base.ReportError(e.Job.Error.ContextDescription + " - " + e.Job.Error.Description, Base.Base.AllUserStore);
                if (Service.Service.ErrorOccurred != null && App.IsClientConnected)
                    Service.Service.ErrorOccurred(e.Job.Error.ContextDescription + " - " + e.Job.Error.Description, ErrorType.DownloadError);
            }

            try
            {
                e.Job.Complete();
                manager.Dispose();
                manager = null;
            }
            catch
            {
            }
            App.ShutdownApp();
        }

        /// <summary>
        ///   Calls the DownloadComplete Event and updates the
        ///   <see cref = "System.Windows.Forms.NotifyIcon" />
        ///   when the job has been downloaded or Calls the InstallUpdates Method
        /// </summary>
        private static void ManagerOnJobTransferred(object sender, NotificationEventArgs e)
        {
            if (File.Exists(Base.Base.AllUserStore + "abort.lock"))
            {
                File.Delete(Base.Base.AllUserStore + "abort.lock");
                App.ShutdownApp();
            }

            if (e.Job == null)
                return;

            if (e.Job.DisplayName != "SevenUpdate")
                return;

            if (e.Job.State != JobState.Transferred)
                return;

            e.Job.Complete();

            manager.OnJobTransferred -= ManagerOnJobTransferred;
            manager.OnJobError -= ManagerOnJobError;
            manager.OnJobModified -= ManagerOnJobModified;
            try
            {
                manager.Dispose();
                manager = null;
            }
            catch
            {
            }
            if (App.Settings.AutoOption == AutoUpdateOption.Install || App.IsInstall)
            {
                if (Service.Service.DownloadCompleted != null && App.IsClientConnected)
                    Service.Service.DownloadCompleted(false);
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon, App.NotifyType.InstallStarted);
                Install.InstallUpdates();
            }
            else
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon, App.NotifyType.DownloadComplete);
        }

        #endregion

        #endregion
    }
}