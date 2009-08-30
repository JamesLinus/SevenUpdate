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
        static System.Net.BITS.Manager manager;

        #endregion

        #region Download Methods

        /// <summary>
        /// Cancels all BITS Downloads
        /// </summary>
        internal static bool CancelDownload()
        {
            if (manager != null)
            {
                manager.Jobs.Update();

                for (int z = 0; z < manager.Jobs.Count; z++)
                {
                    if (manager.Jobs[z].DisplayName.Contains("Seven Update") && manager.Jobs[z].CanCancel)
                    {
                        try
                        {
                            manager.Jobs[z].Cancel();
                        }
                        catch (Exception) { }
                        try
                        {
                            manager.Jobs.RemoveAt(z);
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
            manager = new System.Net.BITS.Manager();
            manager.OnTransferred += new EventHandler<System.Net.BITS.JobTransferredEventArgs>(manager_OnTransferred);
            manager.OnError += new EventHandler<System.Net.BITS.JobErrorEventArgs>(manager_OnError);
            manager.OnModfication += new EventHandler<System.Net.BITS.JobModificationEventArgs>(manager_OnModfication);
            try
            {
                manager.Jobs.Update();
            }
            catch (System.Net.BITS.BITSException) { }

            for (int z = 0; z < manager.Jobs.Count; z++)
            {
                if (manager.Jobs[z].DisplayName.StartsWith("Seven Update"))
                {
                    if (manager.Jobs[z].Files.Count < 1 || (manager.Jobs[z].State != System.Net.BITS.JobState.Transferring && manager.Jobs[z].State != System.Net.BITS.JobState.Suspended))
                    {
                        try
                        {
                            if (manager.Jobs[z].CanCancel)
                                manager.Jobs[z].Cancel();
                            manager.Jobs.RemoveAt(z);
                            z--;
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        try
                        {
                            manager.Jobs[z].Resume();
                            manager.Jobs[z].Activate();
                            return;

                        }
                        catch (System.Net.BITS.BITSException)
                        {
                            try
                            {
                                if (manager.Jobs[z].CanCancel)
                                    manager.Jobs[z].Cancel();
                                manager.Jobs.RemoveAt(z);
                                z--;
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }

            System.Net.BITS.Job job = new System.Net.BITS.Job("Seven Update" + new Random().Next(99), System.Net.BITS.JobType.Download, System.Net.BITS.JobPriority.Foreground);

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
                                    job.Files.Add(url.AbsoluteUri, downloadDir + @"\" + Path.GetFileName(fileDest));
                                }
                                catch (Exception e)
                                {
                                    try
                                    {
                                        manager.Jobs.CloseAllJobs();
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
                if (job.Files.Count > 0)
                {
                    manager.Jobs.Add(job);
                    if (manager.Jobs.Count > 0)
                    {
                        job.Resume();
                    }
                }
                else
                {
                    try
                    {
                        manager.Jobs.CloseAllJobs();
                        manager.Dispose();
                        manager = null;
                    }
                    catch (Exception) { }

                    if (DownloadDoneEventHandler != null)
                        DownloadDoneEventHandler(null, new DownloadDoneEventArgs(false, applications));
                }
            }
            catch (System.Net.BITS.BITSException e)
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
        internal static System.Net.BITS.JobState GetDownloadState
        {
            get
            {
                if (manager != null)
                    try
                    {
                        for (int x = 0; x < manager.Jobs.Count; x++)
                        {
                            if (manager.Jobs[x].DisplayName.Contains("Seven Update"))
                                return manager.Jobs[x].State;
                        }
                        return System.Net.BITS.JobState.Inactive;
                    }
                    catch { return System.Net.BITS.JobState.Transferred; }
                else
                    return System.Net.BITS.JobState.Inactive;
            }
        }

        #endregion

        #region Events

        #region Event Handlers

        /// <summary>
        /// When an error occurs it adds it into the history, and starts the next one available.
        /// </summary>
        /// <param name="e">JobError EventArgs, contains error information</param>
        static void manager_OnError(object sender, System.Net.BITS.JobErrorEventArgs e)
        {
            if (e.Job.DisplayName.Contains("Seven Update"))
            {
                manager.Jobs.Remove(e.Job.Id);

                try
                {
                    manager.Jobs.CloseAllJobs();
                    manager.Dispose();
                    manager = null;
                }
                catch (Exception) { }
                if (WCF.EventService.ErrorOccurred != null)
                    WCF.EventService.ErrorOccurred(e.Job.Error.Description + " " + e.Error.File.RemoteFileName);

                if (DownloadDoneEventHandler != null)
                    DownloadDoneEventHandler(null, new DownloadDoneEventArgs(true, null));
            }

        }

        /// <summary>
        /// Calls the UpdateProgress event handler passing event args
        /// </summary>
        /// <param name="e">JobModification EventArgs, contains information about the job</param>
        static void manager_OnModfication(object sender, System.Net.BITS.JobModificationEventArgs e)
        {
            if (Abort)
                Environment.Exit(0);
            if (e.Job.DisplayName.Contains("Seven Update") && e.Job.State == System.Net.BITS.JobState.Transferring)
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

        /// <summary>
        /// When a job is complete, increment the counter and start the next one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">JobTransfered EventArgs, contains information about the job</param>
        static void manager_OnTransferred(object sender, System.Net.BITS.JobTransferredEventArgs e)
        {
            if (Abort)
                Environment.Exit(0);
            try
            {
                if (e.Job.DisplayName.Contains("Seven Update"))
                {
                    if (e.Job.State == System.Net.BITS.JobState.Transferred)
                    {
                        e.Job.Complete();

                        manager.OnTransferred -= new EventHandler<System.Net.BITS.JobTransferredEventArgs>(manager_OnTransferred);
                        manager.OnError -= new EventHandler<System.Net.BITS.JobErrorEventArgs>(manager_OnError);
                        manager.OnModfication -= new EventHandler<System.Net.BITS.JobModificationEventArgs>(manager_OnModfication);

                        try
                        {
                            manager.Jobs.CloseAllJobs();
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
                        manager.Jobs[manager.Jobs.IndexOf(e.Job)].Resume();
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
