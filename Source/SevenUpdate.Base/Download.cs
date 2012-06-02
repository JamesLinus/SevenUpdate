// <copyright file="Download.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;

    using SharpBits.Base;

    /// <summary>A class containing methods to download updates.</summary>
    public static class Download
    {
        /// <summary>Gets a value indicating whether to cancel the current download.</summary>
        private static bool cancelDownload;

        /// <summary>The directory containing the app update files.</summary>
        private static string downloadDirectory;

        /// <summary>Gets a value indicating whether an error has occurred.</summary>
        private static bool errorOccurred;

        /// <summary>The download job name.</summary>
        private static string jobName;

        /// <summary>Manager for Background Intelligent Transfer Service.</summary>
        private static BitsManager manager;

        /// <summary>Occurs when the download completed.</summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        /// <summary>Occurs when the download progress changed.</summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>Gets a value indicating whether Seven update is currently downloading updates.</summary>
        public static bool IsDownloading { get; private set; }

        /// <summary>Cancel the downloading of updates.</summary>
        public static void CancelDownload()
        {
            cancelDownload = true;
        }

        /// <summary>Downloads the updates using BITS.</summary>
        /// <param name="appUpdates">The application updates to download.</param>
        /// <param name="downloadName">The name of the job.</param>
        /// <param name="downloadLocation">The directory where the files are downloaded are stored.</param>
        /// <param name="isPriority">If set to <c>True</c> the updates will download with priority.</param>
        public static void DownloadUpdates(
            Collection<Sui> appUpdates, string downloadName, string downloadLocation, bool isPriority = false)
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
            manager.EnumJobs();
            manager.EnumJobs(JobOwner.AllUsers);

            int jobCount = 0;

            // Loops through the jobs, if a Seven Update job is found, try to resume, if not cancel it.
            foreach (var job in manager.Jobs.Values.Where(job => job.DisplayName == jobName).ToList())
            {
                jobCount++;
                job.EnumerateFiles();
                if (job.Files.Count < 1)
                {
                    job.Cancel();
                    jobCount--;
                }

                if (job.State == JobState.Suspended)
                {
                    job.Resume();
                }

                if (job.State == JobState.Transferred)
                {
                    job.Complete();
                    IsDownloading = false;
                    if (DownloadCompleted != null)
                    {
                        DownloadCompleted(null, new DownloadCompletedEventArgs(false));
                    }

                    return;
                }

                if ((job.State != JobState.Error && job.State != JobState.TransientError) && job.ErrorCount <= 0)
                {
                    continue;
                }

                Utilities.ReportError(
                    new WebException(job.Error.Description + " " + job.Error.ErrorCode), ErrorType.DownloadError);
                job.Cancel();
                jobCount--;
            }

            if (jobCount > 0)
            {
                return;
            }

            BitsJob bitsJob = manager.CreateJob(jobName, JobType.Download);
            bitsJob.Priority = isPriority ? JobPriority.Foreground : JobPriority.Normal;

            bitsJob.NotificationOptions = NotificationOptions.JobErrorOccurred | NotificationOptions.JobModified
                                          | NotificationOptions.JobTransferred;
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
                bitsJob.Cancel();
                IsDownloading = false;
                if (DownloadCompleted != null)
                {
                    DownloadCompleted(null, new DownloadCompletedEventArgs(false));
                }
            }
        }

        /// <summary>Downloads the application updates.</summary>
        /// <param name="application">The Sui containing the update info.</param>
        /// <param name="bitsJob">The bits job that will download the update.</param>
        private static void DownloadUpdates(Sui application, ref BitsJob bitsJob)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (bitsJob == null)
            {
                throw new ArgumentNullException("bitsJob");
            }

            for (int y = 0; y < application.Updates.Count; y++)
            {
                // Create download directory consisting of application name and update title
                string downloadDir = Path.Combine(downloadDirectory, application.Updates[y].Name[0].Value);

                Directory.CreateDirectory(downloadDir);

                for (int z = 0; z < application.Updates[y].Files.Count; z++)
                {
                    if (application.Updates[y].Files[z].Action == FileAction.Delete
                        || application.Updates[y].Files[z].Action == FileAction.UnregisterThenDelete
                        || application.Updates[y].Files[z].Action == FileAction.CompareOnly)
                    {
                        continue;
                    }

                    string destination = Path.GetFileName(application.Updates[y].Files[z].Destination);
                    if (string.IsNullOrWhiteSpace(destination))
                    {
                        throw new InvalidOperationException();
                    }

                    if (Utilities.GetHash(Path.Combine(downloadDir, destination))
                        == application.Updates[y].Files[z].Hash)
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(Path.Combine(downloadDir, destination));
                    }
                    catch (IOException)
                    {
                    }

                    bitsJob.AddFile(
                        new Uri(application.Updates[y].Files[z].Source).AbsoluteUri, 
                        Path.Combine(downloadDir, destination));
                }
            }
        }

        /// <summary>Reports when a download completes.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>SharpBits.Base.NotificationEventArgs</c> instance containing the event data.</param>
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

        /// <summary>Reports a download error.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>SharpBits.Base.ErrorNotificationEventArgs</c> instance containing the event data.</param>
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

            string error = null;
            if (e.Error != null)
            {
                error = " " + e.Error.Description + " " + e.Error.ContextDescription;

                if (e.Error.File != null)
                {
                    error += " " + e.Error.File.RemoteName;
                }
            }

            Utilities.ReportError(new WebException(error), ErrorType.DownloadError);
            return;
        }

        /// <summary>Reports the download progress.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>SharpBits.Base.NotificationEventArgs</c> instance containing the event data.</param>
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

            var eventArgs = new DownloadProgressChangedEventArgs(
                e.Job.Progress.BytesTransferred, 
                e.Job.Progress.BytesTotal, 
                e.Job.Progress.FilesTransferred, 
                e.Job.Progress.FilesTotal);
            DownloadProgressChanged(null, eventArgs);
        }
    }
}