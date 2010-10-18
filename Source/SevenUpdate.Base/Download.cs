// ***********************************************************************
// <copyright file="Download.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    using SharpBits.Base;

    /// <summary>A class containing methods to download updates</summary>
    public static class Download
    {
        #region Constants and Fields

        /// <summary>Gets a value indicating whether an error has occurred</summary>
        private static bool errorOccurred;

        /// <summary>Manager for Background Intelligent Transfer Service</summary>
        private static BitsManager manager;

        #endregion

        #region Events

        /// <summary>Occurs when the download completed.</summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        /// <summary>Occurs when the download progress changed</summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether Seven update is currently downloading updates</summary>
        public static bool IsDownloading { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>Downloads the updates using BITS</summary>
        /// <param name="appUpdates">The application updates to download</param>
        /// <param name="isPriority">if set to <see langword="true"/> the updates will download with priority</param>
        public static void DownloadUpdates(Collection<Sui> appUpdates, bool isPriority = false)
        {
            if (appUpdates == null)
            {
                throw new ArgumentNullException(@"appUpdates");
            }

            if (appUpdates.Count < 1)
            {
                throw new ArgumentOutOfRangeException(@"appUpdates");
            }

            IsDownloading = true;

            // It's a new manager class
            manager = new BitsManager();

            // Assign the event handlers
            manager.OnJobTransferred += ReportDownloadComplete;
            manager.OnJobError += ReportDownloadError;
            manager.OnJobModified += ReportDownloadProgress;

            // Load the BITS Jobs for the entire machine and current user
            try
            {
                manager.EnumJobs(JobOwner.CurrentUser);
                manager.EnumJobs(JobOwner.AllUsers);
            }
            catch (BitsError)
            {
            }

            // Loops through the jobs, if a Seven Update job is found, try to resume, if not 
            foreach (var job in manager.Jobs.Values.Where(job => job.DisplayName == "SevenUpdate"))
            {
                job.EnumerateFiles();

                if (job.Files.Count < 1 || (job.State != JobState.Transferring && job.State != JobState.Suspended))
                {
                    try
                    {
                        job.Cancel();
                    }
                    catch (BitsError)
                    {
                    }
                }
                else
                {
                    if (job.ErrorCount < 1 && job.State == JobState.Suspended)
                    {
                        job.Resume();
                    }
                    else
                    {
                        job.Cancel();
                    }
                }
            }

            var bitsJob = manager.CreateJob("SevenUpdate", JobType.Download);
            if (isPriority)
            {
                bitsJob.Priority = JobPriority.ForeGround;
            }

            bitsJob.NotificationFlags = NotificationFlags.JobErrorOccurred | NotificationFlags.JobModified | NotificationFlags.JobTransferred;
            bitsJob.NoProgressTimeout = 60;
            bitsJob.MinimumRetryDelay = 60;
            for (var x = 0; x < appUpdates.Count; x++)
            {
                DownloadUpdates(appUpdates[x], ref bitsJob);
            }

            bitsJob.EnumerateFiles();
            if (bitsJob.Files.Count > 0)
            {
                try
                {
                    bitsJob.Resume();
                }
                catch (BitsError e)
                {
                    Utilities.ReportError(e, Utilities.AllUserStore);
                    return;
                }
            }
            else
            {
                manager.Dispose();
                manager = null;
                IsDownloading = false;
                if (DownloadCompleted != null)
                {
                    DownloadCompleted(null, new DownloadCompletedEventArgs(false));
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>Downloads the application updates</summary>
        /// <param name="application">The Sui containing the update info</param>
        /// <param name="bitsJob">The bits job that will download the update.</param>
        private static void DownloadUpdates(Sui application, ref BitsJob bitsJob)
        {
            for (var y = 0; y < application.Updates.Count; y++)
            {
                // Create download directory consisting of application name and update title
                var downloadDir = Utilities.AllUserStore + @"downloads\" + application.Updates[y].Name[0].Value;

                Directory.CreateDirectory(downloadDir);

                for (var z = 0; z < application.Updates[y].Files.Count; z++)
                {
                    if (application.Updates[y].Files[z].Action == FileAction.Delete || application.Updates[y].Files[z].Action == FileAction.UnregisterThenDelete ||
                        application.Updates[y].Files[z].Action == FileAction.CompareOnly)
                    {
                        continue;
                    }

                    if (Utilities.GetHash(downloadDir + @"\" + Path.GetFileName(application.Updates[y].Files[z].Destination)) == application.Updates[y].Files[z].Hash)
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(downloadDir + @"\" + Path.GetFileName(application.Updates[y].Files[z].Destination));
                    }
                    catch (IOException)
                    {
                    }

                    bitsJob.AddFile(new Uri(application.Updates[y].Files[z].Source).AbsoluteUri, downloadDir + @"\" + Path.GetFileName(application.Updates[y].Files[z].Destination));
                }
            }
        }

        /// <summary>Reports when a download completes</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SharpBits.Base.NotificationEventArgs"/> instance containing the event data.</param>
        private static void ReportDownloadComplete(object sender, NotificationEventArgs e)
        {
            if (File.Exists(Utilities.AllUserStore + "abort.lock"))
            {
                File.Delete(Utilities.AllUserStore + "abort.lock");
                return;
            }

            if (e.Job == null)
            {
                return;
            }

            if (e.Job.DisplayName != "SevenUpdate")
            {
                return;
            }

            if (e.Job.State != JobState.Transferred)
            {
                return;
            }

            IsDownloading = false;
            e.Job.Complete();

            if (DownloadCompleted != null)
            {
                DownloadCompleted(null, new DownloadCompletedEventArgs(errorOccurred));
            }

            manager.OnJobTransferred -= ReportDownloadComplete;
            manager.OnJobError -= ReportDownloadError;
            manager.OnJobModified -= ReportDownloadProgress;
            try
            {
                manager.Dispose();
                manager = null;
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>Reports a download error</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SharpBits.Base.ErrorNotificationEventArgs"/> instance containing the event data.</param>
        private static void ReportDownloadError(object sender, ErrorNotificationEventArgs e)
        {
            if (e.Job == null)
            {
                return;
            }

            if (e.Job.DisplayName != "SevenUpdate")
            {
                return;
            }

            if (e.Job.State != JobState.Error)
            {
                return;
            }

            errorOccurred = true;

            if (e.Job.Error.File != null)
            {
                Utilities.ReportError(e.Job.Error.File.RemoteName + @" - " + e.Job.Error.Description, Utilities.AllUserStore);
            }
            else
            {
                Utilities.ReportError(e.Job.Error.ContextDescription + @" - " + e.Job.Error.Description, Utilities.AllUserStore);
            }

            if (e.Job.State != JobState.Canceled)
            {
                e.Job.Cancel();
            }

            try
            {
                manager.Dispose();
                manager = null;
            }
            catch (ObjectDisposedException)
            {
            }

            return;
        }

        /// <summary>Reports the download progress</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SharpBits.Base.NotificationEventArgs"/> instance containing the event data.</param>
        private static void ReportDownloadProgress(object sender, NotificationEventArgs e)
        {
            if (File.Exists(Utilities.AllUserStore + "abort.lock"))
            {
                File.Delete(Utilities.AllUserStore + "abort.lock");
                return;
            }

            if (e.Job == null)
            {
                return;
            }

            if (e.Job.DisplayName != "SevenUpdate")
            {
                return;
            }

            if (e.Job.State == JobState.Error)
            {
                return;
            }

            if (DownloadProgressChanged == null || e.Job.Progress.BytesTransferred <= 0)
            {
                return;
            }

            var eventArgs = new DownloadProgressChangedEventArgs(e.Job.Progress.BytesTransferred, e.Job.Progress.BytesTotal, e.Job.Progress.FilesTransferred, e.Job.Progress.FilesTotal);
            DownloadProgressChanged(null, eventArgs);
        }

        #endregion
    }
}