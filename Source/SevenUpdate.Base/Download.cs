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

        /// <summary>Gets a value indicating whether to cancel the current download</summary>
        private static bool cancelDownload;

        /// <summary>The directory containing the app update files</summary>
        private static string downloadDirectory;

        /// <summary>Gets a value indicating whether an error has occurred</summary>
        private static bool errorOccurred;

        /// <summary>The download job name</summary>
        private static string jobName;

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

        /// <summary>Cancel the downloading of updates</summary>
        public static void CancelDownload()
        {
            cancelDownload = true;
        }

        /// <summary>Downloads the updates using BITS</summary>
        /// <param name="appUpdates">The application updates to download</param>
        /// <param name="downloadName">The name of the job</param>
        /// <param name="downloadLocation">The directory where the files are downloaded are stored</param>
        public static void DownloadUpdates(Collection<Sui> appUpdates, string downloadName, string downloadLocation)
        {
            DownloadUpdates(appUpdates, downloadName, downloadLocation, false);
        }

        /// <summary>Downloads the updates using BITS</summary>
        /// <param name="appUpdates">The application updates to download</param>
        /// <param name="downloadName">The name of the job</param>
        /// <param name="downloadLocation">The directory where the files are downloaded are stored</param>
        /// <param name="isPriority">if set to <see langword="true"/> the updates will download with priority</param>
        public static void DownloadUpdates(Collection<Sui> appUpdates, string downloadName, string downloadLocation, bool isPriority)
        {
            downloadDirectory = downloadLocation;
            jobName = downloadName;
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
            manager.OnJobTransferred -= ReportDownloadComplete;
            manager.OnJobError -= ReportDownloadError;
            manager.OnJobModified -= ReportDownloadProgress;
            manager.OnJobTransferred += ReportDownloadComplete;
            manager.OnJobError += ReportDownloadError;
            manager.OnJobModified += ReportDownloadProgress;

            // Load the BITS Jobs for the entire machine and current user
            manager.EnumJobs(JobOwner.CurrentUser);
            manager.EnumJobs(JobOwner.AllUsers);

            // Loops through the jobs, if a Seven Update job is found, try to resume, if not 
            foreach (var job in manager.Jobs.Values.Where(job => job.DisplayName == jobName))
            {
                job.EnumerateFiles();

                if (job.Files.Count < 1 || (job.State != JobState.Transferring && job.State != JobState.Suspended))
                {
                    job.Cancel();
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

            var bitsJob = manager.CreateJob(jobName, JobType.Download);
            if (isPriority)
            {
                bitsJob.Priority = JobPriority.Foreground;
            }

            bitsJob.NotificationOptions = NotificationOptions.JobErrorOccurred | NotificationOptions.JobModified | NotificationOptions.JobTransferred;
            bitsJob.NoProgressTimeout = 60;
            bitsJob.MinimumRetryDelay = 60;
            foreach (var update in appUpdates)
            {
                DownloadUpdates(update, ref bitsJob);
            }

            bitsJob.EnumerateFiles();
            if (bitsJob.Files.Count > 0)
            {
                bitsJob.Resume();
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
                var downloadDir = Path.Combine(downloadDirectory, application.Updates[y].Name[0].Value);

                Directory.CreateDirectory(downloadDir);

                for (var z = 0; z < application.Updates[y].Files.Count; z++)
                {
                    if (application.Updates[y].Files[z].Action == FileAction.Delete || application.Updates[y].Files[z].Action == FileAction.UnregisterThenDelete ||
                        application.Updates[y].Files[z].Action == FileAction.CompareOnly)
                    {
                        continue;
                    }

                    if (Utilities.GetHash(downloadDir + Path.GetFileName(application.Updates[y].Files[z].Destination)) == application.Updates[y].Files[z].Hash)
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(Path.Combine(downloadDir, Path.GetFileName(application.Updates[y].Files[z].Destination)));
                    }
                    catch (IOException)
                    {
                    }

                    bitsJob.AddFile(new Uri(application.Updates[y].Files[z].Source).AbsoluteUri, Path.Combine(downloadDir, Path.GetFileName(application.Updates[y].Files[z].Destination)));
                }
            }
        }

        /// <summary>Reports when a download completes</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="SharpBits.Base.NotificationEventArgs"/> instance containing the event data.</param>
        private static void ReportDownloadComplete(object sender, NotificationEventArgs e)
        {
            if (e.Job == null)
            {
                return;
            }

            if (e.Job.DisplayName != jobName)
            {
                return;
            }

            if (e.Job.State != JobState.Transferred)
            {
                return;
            }

            IsDownloading = false;
            e.Job.Complete();

            if (cancelDownload)
            {
                Environment.Exit(0);
            }

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
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="SharpBits.Base.ErrorNotificationEventArgs"/> instance containing the event data.</param>
        private static void ReportDownloadError(object sender, ErrorNotificationEventArgs e)
        {
            if (e.Job == null)
            {
                return;
            }

            if (e.Job.DisplayName != jobName)
            {
                return;
            }

            if (e.Job.State != JobState.Error)
            {
                return;
            }

            errorOccurred = true;

            var exception = new Exception(e.Job.Error.File + " " + e.Job.Error.Description + " " + e.Job.Error.ErrorCode + " " + e.Job.Error.ContextDescription);

            Utilities.ReportError(exception, ErrorType.DownloadError);

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
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="SharpBits.Base.NotificationEventArgs"/> instance containing the event data.</param>
        private static void ReportDownloadProgress(object sender, NotificationEventArgs e)
        {
            if (cancelDownload)
            {
                e.Job.Cancel();
                Environment.Exit(0);
            }

            if (e.Job == null)
            {
                return;
            }

            if (e.Job.DisplayName != jobName)
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