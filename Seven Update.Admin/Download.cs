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
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using SevenUpdate.WCF;
using SharpBits.Base;

#endregion

namespace SevenUpdate
{
    /// <summary>
    /// A class containing methods to download updates
    /// </summary>
    internal class Download
    {
        #region Global Vars

        /// <summary>
        /// Manager for Background Intelligent Transfer Service
        /// </summary>
        private static BitsManager manager;

        /// <summary>
        /// Collection of updates
        /// </summary>
        private static Collection<SUI> updates;

        #endregion

        #region Download Methods

        /// <summary>
        /// Cancels all BITS Downloads
        /// </summary>
        internal static bool CancelDownload()
        {
            if (manager != null)
            {
                foreach (var job in manager.Jobs.Values.Where(job => job.DisplayName == "Seven Update"))
                {
                    try
                    {
                        job.Cancel();
                    }
                    catch (Exception)
                    {
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Downloads the updates using BITS
        /// </summary>
        /// <param name="applications">the Collection of applications and updates</param>
        /// <param name="priority">the Priority of the download</param>
        internal static void DownloadUpdates(Collection<SUI> applications, JobPriority priority)
        {
            if (applications == null)
            {
                if (EventService.ErrorOccurred != null && App.IsClientConnected)
                    EventService.ErrorOccurred(new Exception("Applications file could not be found"), ErrorType.FatalError);

                Shared.ReportError("Applications file could not be found", Shared.AllUserStore);
                Environment.Exit(0);
            }
            else
            {
                if (applications.Count < 1)
                {
                    if (EventService.ErrorOccurred != null && App.IsClientConnected)
                        EventService.ErrorOccurred(new Exception("Applications file could not be found"), ErrorType.DownloadError);

                    Shared.ReportError("Applications file could not be found", Shared.AllUserStore);
                    Environment.Exit(0);
                }
            }

            // When done with the temp list
            File.Delete(Shared.UserStore + "Apps.sui");

            // Makes the passed applicaytion collection global
            updates = applications;

            // It's a new manager class
            manager = new BitsManager();

            // Assign the event handlers
            manager.OnJobTransferred += ManagerOnJobTransferred;
            manager.OnJobError += ManagerOnJobError;
            manager.OnJobModified += ManagerOnJobModified;

            // Load the BITS Jobs for the entire machine
            manager.EnumJobs(JobOwner.AllUsers);

            // Loops through the jobs, if a Seven Update job is found, try to resume, if not 
            foreach (var job in manager.Jobs.Values.Where(job => job.DisplayName == "Seven Update"))
            {
                job.EnumFiles();
                if (job.Files.Count < 1 || (job.State != JobState.Transferring && job.State != JobState.Suspended))
                {
                    try
                    {
                        job.Cancel();
                    }
                    catch (Exception)
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
                    catch (Exception)
                    {
                        try
                        {
                            job.Cancel();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            var bitsJob = manager.CreateJob("Seven Update", JobType.Download);
            bitsJob.NotificationFlags = NotificationFlags.JobErrorOccured | NotificationFlags.JobModified | NotificationFlags.JobTransferred;
            bitsJob.Priority = priority;
            for (var x = 0; x < applications.Count; x++)
            {
                for (var y = 0; y < applications[x].Updates.Count; y++)
                {
                    // Create download directory consisting of appname and update title
                    var downloadDir = Shared.AllUserStore + @"downloads\" + applications[x].Updates[y].Name[0].Value;

                    Directory.CreateDirectory(downloadDir);

                    for (var z = 0; z < applications[x].Updates[y].Files.Count; z++)
                    {
                        var fileDestination = Shared.ConvertPath(applications[x].Updates[y].Files[z].Destination, applications[x].Directory, applications[x].Is64Bit);

                        if (applications[x].Updates[y].Files[z].Action == FileAction.Delete || applications[x].Updates[y].Files[z].Action == FileAction.UnregisterAndDelete)
                            continue;
                        if (Shared.GetHash(downloadDir + @"\" + Path.GetFileName(fileDestination)) == applications[x].Updates[y].Files[z].Hash)
                            continue;
                        try
                        {
                            try
                            {
                                File.Delete(downloadDir + @"\" + Path.GetFileName(fileDestination));
                            }
                            catch (Exception)
                            {
                            }
                            var url = new Uri(Shared.ConvertPath(applications[x].Updates[y].Files[z].Source, applications[x].Updates[y].DownloadUrl, applications[x].Is64Bit));
                            bitsJob.AddFile(url.AbsoluteUri, downloadDir + @"\" + Path.GetFileName(fileDestination));
                        }
                        catch (Exception e)
                        {
                            if (EventService.ErrorOccurred != null && App.IsClientConnected)
                                EventService.ErrorOccurred(e, ErrorType.DownloadError);
                            Shared.ReportError(e.Message, Shared.AllUserStore);
                        }
                    }
                }
            }

            bitsJob.EnumFiles();
            
            if (bitsJob.Files.Count > 0)
            {
                try
                {
                    bitsJob.Resume();
                }
                catch (Exception e)
                {
                    Shared.ReportError(e.Message, Shared.AllUserStore);
                    if (EventService.ErrorOccurred != null && App.IsClientConnected)
                        EventService.ErrorOccurred(e, ErrorType.FatalError);
                    Environment.Exit(0);
                }
            }
            else
            {
                manager.Dispose();
                manager = null;
                Install.InstallUpdates(updates);
            }
        }

        #endregion

        #region Events

        #region Event Handlers

        /// <summary>
        /// Calls the DownloadProgressChanged Event and updates the <see cref="System.Windows.Forms.NotifyIcon" /> when the job progress has changed
        /// </summary>
        private static void ManagerOnJobModified(object sender, NotificationEventArgs e)
        {
            if (App.Abort)
                Environment.Exit(0);
            if (e.Job.DisplayName != "Seven Update" || e.Job.State != JobState.Transferring)
                return;

            if (e.Job.Progress.BytesTransferred <= 0)
                return;

            if (EventService.DownloadProgressChanged != null && App.IsClientConnected)
                EventService.DownloadProgressChanged(e.Job.Progress.BytesTransferred, e.Job.Progress.BytesTotal);

            if (App.NotifyIcon != null && e.Job.Progress.FilesTransferred > 0 && e.Job.Progress.BytesTotal > 0)
            {
                Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon,
                                                           App.RM.GetString("DownloadingUpdates") + " (" + Shared.ConvertFileSize(e.Job.Progress.BytesTotal) + ", " +
                                                           (e.Job.Progress.BytesTransferred*100/e.Job.Progress.BytesTotal).ToString("F0") + " % " + App.RM.GetString("Complete") + ")");
            }
        }

        /// <summary>
        /// Calls the ErrorOccurred Event and updates the <see cref="System.Windows.Forms.NotifyIcon" /> when an error occurs
        /// </summary>
        private static void ManagerOnJobError(object sender, ErrorNotificationEventArgs e)
        {

            if (e.Job.DisplayName != "Seven Update")
                return;
           
            try
            {
                e.Job.Cancel();
                manager.Dispose();
                manager = null;
            }
            catch (Exception)
            {
            }
            Shared.ReportError(e.Error.File + " - " + e.Error.File, Shared.AllUserStore);
            if (EventService.ErrorOccurred != null && App.IsClientConnected)
                EventService.ErrorOccurred(new Exception(e.Error.File + " - " + e.Error.File), ErrorType.DownloadError);
            Environment.Exit(0);
        }

        /// <summary>
        /// Calls the DownloadComplete Event and updates the <see cref="System.Windows.Forms.NotifyIcon" /> when the job has been downloaded or Calls the InstallUpdates Method
        /// </summary>
        private static void ManagerOnJobTransferred(object sender, NotificationEventArgs e)
        {
            if (App.Abort)
                Environment.Exit(0);
            if (e.Job.DisplayName == "Seven Update")
            {
                if (e.Job.State == JobState.Transferred)
                {
                    e.Job.Complete();

                    manager.OnJobTransferred -= ManagerOnJobTransferred;
                    manager.OnJobError -= ManagerOnJobError;
                    manager.OnJobModified -= ManagerOnJobModified;
                    try
                    {
                        manager.Dispose();
                        manager = null;
                    }
                    catch (Exception)
                    {
                    }

                    if (App.Settings.AutoOption == AutoUpdateOption.Install || Environment.GetCommandLineArgs()[0] == "Install")
                    {
                        if (EventService.DownloadCompleted != null && App.IsClientConnected)
                            EventService.DownloadCompleted(false);
                        Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon, App.NotifyType.InstallStarted);
                        Install.InstallUpdates(updates);
                    }
                    else
                        Application.Current.Dispatcher.BeginInvoke(App.UpdateNotifyIcon, App.NotifyType.DownloadComplete);
                }
            }
            else
                e.Job.Resume();
        }

        #region EventArgs

        /// <summary>
        /// Provides event data for the DownloadCompleted event
        /// </summary>
        public class DownloadCompletedEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="errorOccurred"><c>true</c> is an error occurred, otherwise <c>false</c> </param>
            /// <param name="applications"> A collection of Applications that were downloaded</param>
            public DownloadCompletedEventArgs(bool errorOccurred, Collection<SUI> applications)
            {
                ErrorOccurred = errorOccurred;
                Applications = applications;
            }

            /// <summary>
            /// <c>true</c> is an error occurred, otherwise <c>false</c>
            /// </summary>
            public bool ErrorOccurred { get; private set; }

            /// <summary>
            /// A collection of Applications that were downloaded
            /// </summary>
            public Collection<SUI> Applications { get; private set; }
        }

        #endregion

        #endregion

        #endregion
    }
}