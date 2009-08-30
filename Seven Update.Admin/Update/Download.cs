/*Copyright 2007-09 Robert Baker, aka Seven ALive.
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
using System.Collections.ObjectModel;
using System.IO;
using SharpBits.Base;

namespace SevenUpdate
{
    class Download
    {
        #region Global Vars

        /// <summary>
        /// Specifies if the installation has been aborted
        /// </summary>
        internal static bool Abort { get; set; }

        static Collection<Application> updates;

        /// <summary>
        /// Manager for Background Intelligent Transfer Service
        /// </summary>
        static BitsManager manager;

        #endregion

        #region Download Methods

        /// <summary>
        /// Cancels all BITS Downloads
        /// </summary>
        internal static bool CancelDownload()
        {
            if (manager != null)
            {
                foreach (BitsJob job in manager.Jobs.Values)
                {

                    if (job.DisplayName == "Seven Update")
                    {
                        try
                        {
                            job.Cancel();
                        }
                        catch (Exception) { }

                        return true;
                    }
                }
            }

            return false;
        }

        internal static void DownloadUpdates(Collection<Application> applications)
        {
            updates = applications;
            manager = new SharpBits.Base.BitsManager();
            manager.OnJobTransferred += new EventHandler<NotificationEventArgs>(manager_OnJobTransferred);
            manager.OnJobError += new EventHandler<ErrorNotificationEventArgs>(manager_OnJobError);
            manager.OnJobModified += new EventHandler<NotificationEventArgs>(manager_OnJobModified);
            manager.EnumJobs(JobOwner.AllUsers);
            Shared.ReportError("Download Started", Shared.appStore);
            foreach (BitsJob job in manager.Jobs.Values)
            {

                if (job.DisplayName == "Seven Update")
                {
                    job.EnumFiles();
                    if (job.Files.Count < 1 || (job.State != JobState.Transferring && job.State != JobState.Suspended))
                    {
                        try
                        {
                            job.Cancel();
                        }
                        catch (Exception) { }
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
                            catch (Exception) { }
                        }
                    }

                }
            }

            BitsJob bitsJob = manager.CreateJob("Seven Update", JobType.Download);
            bitsJob.NotificationFlags = NotificationFlags.JobErrorOccured | NotificationFlags.JobModified | NotificationFlags.JobTransferred;
            string fileDest;
            string downloadDir;
            for (int x = 0; x < applications.Count; x++)
            {
                for (int y = 0; y < applications[x].Updates.Count; y++)
                {
                    // Create download directory consisting of appname and update title
                    downloadDir = Shared.appStore + @"downloads\" + Shared.GetLocaleString(applications[x].Name) + @"\" + Shared.GetLocaleString(applications[x].Updates[y].Title);
                    Directory.CreateDirectory(downloadDir);

                    for (int z = 0; z < applications[x].Updates[y].Files.Count; z++)
                    {
                        fileDest = Shared.ConvertPath(applications[x].Updates[y].Files[z].Destination, applications[x].Directory, applications[x].Is64Bit);

                        if (applications[x].Updates[y].Files[z].Action != FileAction.Delete && applications[x].Updates[y].Files[z].Action != FileAction.UnregisterAndDelete)
                        {
                            if (Shared.GetHash(downloadDir + @"\" + Path.GetFileName(fileDest)) != applications[x].Updates[y].Files[z].Hash)
                            {
                                try
                                {
                                    try
                                    {
                                        File.Delete(downloadDir + @"\" + Path.GetFileName(fileDest));
                                    }
                                    catch (Exception) { }
                                    Uri url = new Uri(Shared.ConvertPath(applications[x].Updates[y].Files[z].Source, applications[x].Updates[y].DownloadDirectory, applications[x].Is64Bit));
                                    bitsJob.AddFile(url.AbsoluteUri, downloadDir + @"\" + Path.GetFileName(fileDest));
                                }
                                catch (Exception e)
                                {
                                    try
                                    {
                                        manager.Dispose();
                                        manager = null;
                                    }
                                    catch (Exception) { }

                                    if (WCF.EventService.ErrorOccurred != null)
                                        WCF.EventService.ErrorOccurred(e.Message);

                                    if (DownloadDoneEventHandler != null)
                                        DownloadDoneEventHandler(null, new DownloadDoneEventArgs(true, applications));
                                }
                            }
                        }
                    }
                }
            }
            try
            {
                bitsJob.EnumFiles();
                if (bitsJob.Files.Count > 0)
                {
                    bitsJob.Resume();
                }
                else
                {
                    try
                    {
                        manager.Dispose();
                        manager = null;
                    }
                    catch (Exception) { }
                    Install.InstallUpdates(updates);
                }
            }
            catch (Exception e)
            {
                Shared.ReportError(e.Message, Shared.appStore);
                if (WCF.EventService.ErrorOccurred != null)
                    WCF.EventService.ErrorOccurred(e.Message);

                if (DownloadDoneEventHandler != null)
                    DownloadDoneEventHandler(null, new DownloadDoneEventArgs(true, applications));
            }
        }

        /// <summary>
        /// Returns the current state of the current BITS Job
        /// </summary>
        internal static SharpBits.Base.JobState GetDownloadState
        {
            get
            {
                foreach (BitsJob job in manager.Jobs.Values)
                {
                    if (job.DisplayName == "Seven Update")
                        return job.State;
                }
                return JobState.Transferred;
            }
        }

        #endregion

        #region Events

        #region Event Handlers

        static void manager_OnJobModified(object sender, NotificationEventArgs e)
        {
            if (Abort)
                Environment.Exit(0);
            if (e.Job.DisplayName == "Seven Update" && e.Job.State == JobState.Transferring)
            {
                try
                {
                    if (WCF.EventService.DownloadProgressChanged != null && e.Job.Progress.BytesTransferred > 0)
                        WCF.EventService.DownloadProgressChanged(e.Job.Progress.BytesTransferred, e.Job.Progress.BytesTotal);

                    if (App.NotifyIcon != null && e.Job.Progress.FilesTransferred > 0)
                    {
                        DispatcherObjectDelegates.BeginInvoke<string>(App.app.Dispatcher, App.UpdateNotifyIcon, App.RM.GetString("DownloadingUpdates") + " (" +
                        Shared.ConvertFileSize(e.Job.Progress.BytesTotal) + ", " + (e.Job.Progress.BytesTransferred * 100 / e.Job.Progress.BytesTotal).ToString("F0") + " % " + App.RM.GetString("Complete") + ")");
                    }

                }
                catch (Exception) { }
            }
        }

        static void manager_OnJobError(object sender, ErrorNotificationEventArgs e)
        {
            if (e.Job.DisplayName == "Seven Update")
            {
                e.Job.Cancel();

                try
                {
                    manager.Dispose();
                    manager = null;
                }
                catch (Exception) { }
                if (WCF.EventService.ErrorOccurred != null)
                    WCF.EventService.ErrorOccurred(e.Job.Error.Description + " " + e.Error.File.RemoteName);

                if (DownloadDoneEventHandler != null)
                    DownloadDoneEventHandler(null, new DownloadDoneEventArgs(true, null));
            }

        }

        static void manager_OnJobTransferred(object sender, NotificationEventArgs e)
        {
            if (Abort)
                Environment.Exit(0);
            try
            {
                if (e.Job.DisplayName == "Seven Update")
                {
                    if (e.Job.State == JobState.Transferred)
                    {
                        e.Job.Complete();

                        manager.OnJobTransferred -= manager_OnJobTransferred;
                        manager.OnJobError -= manager_OnJobError;
                        manager.OnJobModified -= manager_OnJobModified;

                        try
                        {
                            manager.Dispose();
                            manager = null;
                        }
                        catch (Exception) { }
                        if (DownloadDoneEventHandler != null)
                            DownloadDoneEventHandler(null, new DownloadDoneEventArgs(false, updates));

                        if (WCF.EventService.DownloadDone != null)
                            WCF.EventService.DownloadDone(false);
                    }
                    else
                    {
                        e.Job.Resume();
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Raises an event when the downloading of updates have completed.
        /// </summary>
        public static event EventHandler<DownloadDoneEventArgs> DownloadDoneEventHandler;

        #region EventArgs

        public class DownloadDoneEventArgs : EventArgs
        {
            public DownloadDoneEventArgs(bool ErrorOccurred, Collection<Application> Applications)
            {
                this.ErrorOccurred = ErrorOccurred;
                this.Applications = Applications;
            }

            /// <summary>
            /// Indicates if error occurred
            /// </summary>
            public bool ErrorOccurred { get; set; }

            /// <summary>
            /// Specifies if a reboot is needed
            /// </summary>
            public Collection<Application> Applications { get; set; }
        }

        #endregion

        #endregion

        #endregion
    }
}
