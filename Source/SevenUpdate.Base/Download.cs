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
using SharpBits.Base;

#endregion

namespace SevenUpdate
{
    /// <summary>
    ///   A class containing methods to download updates
    /// </summary>
    public static class Download
    {
        #region Fields

        /// <summary>
        ///   Manager for Background Intelligent Transfer Service
        /// </summary>
        private static BitsManager manager;

        private static bool errorOccurred;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the download completed.
        /// </summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        /// <summary>
        ///   Occurs when the download progress changed
        /// </summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        #endregion

        #region Download Methods

        /// <summary>
        ///   Downloads the updates using BITS
        /// </summary>
        /// <param name = "appUpdates">The application updates to download</param>
        /// <param name="isPriority"></param>
        public static void DownloadUpdates(Collection<Sui> appUpdates, bool isPriority = false)
        {
            if (appUpdates == null)
                return;
            if (appUpdates.Count < 1)
                return;

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
            if (isPriority)
                bitsJob.Priority = JobPriority.ForeGround;
            bitsJob.NotificationFlags = NotificationFlags.JobErrorOccured | NotificationFlags.JobModified | NotificationFlags.JobTransferred;
            bitsJob.NoProgressTimeout = 60;
            bitsJob.MinimumRetryDelay = 60;
            for (var x = 0; x < appUpdates.Count; x++)
            {
                for (var y = 0; y < appUpdates[x].Updates.Count; y++)
                {
                    // Create download directory consisting of appname and update title
                    var downloadDir = Base.AllUserStore + @"downloads\" + appUpdates[x].Updates[y].Name[0].Value;

                    Directory.CreateDirectory(downloadDir);

                    for (var z = 0; z < appUpdates[x].Updates[y].Files.Count; z++)
                    {
                        appUpdates[x].Updates[y].Files[z].Destination = Base.ConvertPath(appUpdates[x].Updates[y].Files[z].Destination, appUpdates[x].AppInfo.Directory, appUpdates[x].AppInfo.ValueName,
                                                                                         appUpdates[x].AppInfo.Is64Bit);

                        if (appUpdates[x].Updates[y].Files[z].Action == FileAction.Delete || appUpdates[x].Updates[y].Files[z].Action == FileAction.UnregisterThenDelete ||
                            appUpdates[x].Updates[y].Files[z].Action == FileAction.CompareOnly)
                            continue;
                        if (Base.GetHash(downloadDir + @"\" + Path.GetFileName(appUpdates[x].Updates[y].Files[z].Destination)) == appUpdates[x].Updates[y].Files[z].Hash)
                            continue;
                        try
                        {
                            try
                            {
                                File.Delete(downloadDir + @"\" + Path.GetFileName(appUpdates[x].Updates[y].Files[z].Destination));
                            }
                            catch
                            {
                            }
                            var url = new Uri(Base.ConvertPath(appUpdates[x].Updates[y].Files[z].Source, appUpdates[x].Updates[y].DownloadUrl, null, appUpdates[x].AppInfo.Is64Bit));

                            bitsJob.AddFile(url.AbsoluteUri, downloadDir + @"\" + Path.GetFileName(appUpdates[x].Updates[y].Files[z].Destination));
                        }
                        catch (Exception e)
                        {
                            Base.ReportError(e, Base.AllUserStore);
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
                    Base.ReportError(e, Base.AllUserStore);
                    return;
                }
            }
            else
            {
                manager.Dispose();
                manager = null;
                if (DownloadCompleted != null)
                    DownloadCompleted(null, new DownloadCompletedEventArgs(false));
            }
        }

        #endregion

        #region Events

        #region Event Handlers

        private static void ManagerOnJobModified(object sender, NotificationEventArgs e)
        {
            if (File.Exists(Base.AllUserStore + "abort.lock"))
            {
                File.Delete(Base.AllUserStore + "abort.lock");
                return;
            }

            if (e.Job == null)
                return;

            if (e.Job.DisplayName != "SevenUpdate")
                return;

            if (e.Job.State == JobState.Error)
                return;

            if (DownloadProgressChanged != null && e.Job.Progress.BytesTotal > 0 && e.Job.Progress.BytesTransferred > 0)
                DownloadProgressChanged(null,
                                        new DownloadProgressChangedEventArgs(e.Job.Progress.BytesTransferred, e.Job.Progress.BytesTotal, e.Job.Progress.FilesTransferred, e.Job.Progress.FilesTotal));
        }

        private static void ManagerOnJobError(object sender, ErrorNotificationEventArgs e)
        {
            if (e.Job == null)
                return;

            if (e.Job.DisplayName != "SevenUpdate")
                return;

            if (e.Job.State != JobState.Error)
                return;

            errorOccurred = true;

            if (e.Job.Error.File != null)
                Base.ReportError(e.Job.Error.File.RemoteName + " - " + e.Job.Error.Description, Base.AllUserStore);
            else
                Base.ReportError(e.Job.Error.ContextDescription + " - " + e.Job.Error.Description, Base.AllUserStore);

            try
            {
                e.Job.Cancel();
                manager.Dispose();
                manager = null;
            }
            catch
            {
            }
            return;
        }

        private static void ManagerOnJobTransferred(object sender, NotificationEventArgs e)
        {
            if (File.Exists(Base.AllUserStore + "abort.lock"))
            {
                File.Delete(Base.AllUserStore + "abort.lock");
                return;
            }

            if (e.Job == null)
                return;

            if (e.Job.DisplayName != "SevenUpdate")
                return;

            if (e.Job.State != JobState.Transferred)
                return;

            e.Job.Complete();
            if (DownloadCompleted != null)
                DownloadCompleted(null, new DownloadCompletedEventArgs(errorOccurred));

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


        }

        #endregion

        #endregion
    }
}