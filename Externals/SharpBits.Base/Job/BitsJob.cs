#region

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using SharpBits.Base.File;
using SharpBits.Base.Progress;

#endregion

namespace SharpBits.Base.Job
{
    public partial class BitsJob
    {
        #region member fields

        internal IBackgroundCopyCallback NotificationTarget;
        private bool disposed;

        private BitsError error;
        private BitsFiles files;
        private Guid guid;
        private IBackgroundCopyJob job;
        private JobTimes jobTimes;
        private BitsManager manager;
        //notification
        private EventHandler<JobErrorNotificationEventArgs> onJobErrored;
        private EventHandler<JobNotificationEventArgs> onJobModified;
        private EventHandler<JobNotificationEventArgs> onJobTransfered;
        private JobProgress progress;
        private ProxySettings proxySettings;

        #endregion

        #region .ctor

        internal BitsJob(BitsManager manager, IBackgroundCopyJob job)
        {
            this.manager = manager;
            this.job = job;
            job2 = this.job as IBackgroundCopyJob2;
            job3 = this.job as IBackgroundCopyJob3;
            job4 = this.job as IBackgroundCopyJob4;

            // store existing notification handler and route message to this as well
            // otherwise it may break system download jobs
            if (NotificationInterface != null)
                NotificationTarget = NotificationInterface; //pointer to the existing one;
            NotificationInterface = manager.NotificationHandler; //notification interface will be disabled when NotifyCmd is set
        }

        #endregion

        #region public properties

        #region IBackgroundCopyJob

        /// <summary>
        ///   Display Name, max 256 chars
        /// </summary>
        public string DisplayName
        {
            get
            {
                try
                {
                    string displayName;
                    job.GetDisplayName(out displayName);
                    return displayName;
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    job.SetDisplayName(value);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Description, max 1024 chars
        /// </summary>
        public string Description
        {
            get
            {
                try
                {
                    string description;
                    job.GetDescription(out description);
                    return description;
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    job.SetDescription(value);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        ///   SID of the job owner
        /// </summary>
        public string Owner
        {
            get
            {
                try
                {
                    string owner;
                    job.GetOwner(out owner);
                    return owner;
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///   resolved owner name from job owner SID
        /// </summary>
        public string OwnerName
        {
            get
            {
                try
                {
                    return Utils.GetName(Owner);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///   Job priority
        ///   can not be set for jobs already in state Canceled or Acknowledged
        /// </summary>
        public JobPriority Priority
        {
            get
            {
                BG_JOB_PRIORITY priority = BG_JOB_PRIORITY.BG_JOB_PRIORITY_NORMAL;
                try
                {
                    job.GetPriority(out priority);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return (JobPriority) priority;
            }
            set
            {
                try
                {
                    job.SetPriority((BG_JOB_PRIORITY) value);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public JobProgress Progress
        {
            get
            {
                try
                {
                    BG_JOB_PROGRESS jobProgress;
                    job.GetProgress(out jobProgress);
                    progress = new JobProgress(jobProgress);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return progress;
            }
        }

        public BitsFiles Files { get { return files; } }

        public ulong ErrorCount
        {
            get
            {
                ulong count = 0;
                try
                {
                    job.GetErrorCount(out count);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return count;
            }
        }

        public BitsError Error
        {
            get
            {
                try
                {
                    JobState state = State;
                    if (state == JobState.Error || state == JobState.TransientError)
                    {
                        if (null == error)
                        {
                            IBackgroundCopyError copyError;
                            job.GetError(out copyError);
                            if (null != copyError)
                                error = new BitsError(this, copyError);
                        }
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return error;
            }
        }

        public uint MinimumRetryDelay
        {
            get
            {
                uint seconds = 0;
                try
                {
                    job.GetMinimumRetryDelay(out seconds);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return seconds;
            }
            set
            {
                try
                {
                    job.SetMinimumRetryDelay(value);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public uint NoProgressTimeout
        {
            get
            {
                uint seconds = 0;
                try
                {
                    job.GetNoProgressTimeout(out seconds);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return seconds;
            }
            set
            {
                try
                {
                    job.SetNoProgressTimeout(value);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public Guid JobId
        {
            get
            {
                try
                {
                    if (guid == Guid.Empty)
                        job.GetId(out guid);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return guid;
            }
        }

        public JobState State
        {
            get
            {
                BG_JOB_STATE state = BG_JOB_STATE.BG_JOB_STATE_UNKNOWN;
                try
                {
                    job.GetState(out state);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return (JobState) state;
            }
        }

        public JobTimes JobTimes
        {
            get
            {
                try
                {
                    BG_JOB_TIMES times;
                    job.GetTimes(out times);
                    jobTimes = new JobTimes(times);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return jobTimes;
            }
        }

        public JobType JobType
        {
            get
            {
                BG_JOB_TYPE jobType = BG_JOB_TYPE.BG_JOB_TYPE_UNKNOWN;
                try
                {
                    job.GetType(out jobType);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return (JobType) jobType;
            }
        }

        public ProxySettings ProxySettings { get { return proxySettings ?? (proxySettings = new ProxySettings(job)); } }

        public NotificationFlags NotificationFlags
        {
            get
            {
                BG_JOB_NOTIFICATION_TYPE flags = 0;
                try
                {
                    job.GetNotifyFlags(out flags);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return (NotificationFlags) flags;
            }
            set
            {
                try
                {
                    job.SetNotifyFlags((BG_JOB_NOTIFICATION_TYPE) value);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        internal IBackgroundCopyCallback NotificationInterface
        {
            get
            {
                object notificationInterface = null;
                try
                {
                    job.GetNotifyInterface(out notificationInterface);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return notificationInterface as IBackgroundCopyCallback;
            }
            set
            {
                try
                {
                    job.SetNotifyInterface(value);
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public BitsFiles EnumFiles()
        {
            try
            {
                IEnumBackgroundCopyFiles fileList;
                job.EnumFiles(out fileList);
                files = new BitsFiles(this, fileList);
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
            return files;
        }

        public void Suspend()
        {
            try
            {
                job.Suspend();
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void Resume()
        {
            try
            {
                job.Resume();
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void Cancel()
        {
            try
            {
                job.Cancel();
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void Complete()
        {
            try
            {
                job.Complete();
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void TakeOwnership()
        {
            try
            {
                job.TakeOwnership();
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void AddFile(string remoteName, string localName)
        {
            try
            {
                job.AddFile(remoteName, localName);
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void AddFile(BitsFileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");
            AddFile(fileInfo.RemoteName, fileInfo.LocalName);
        }

        internal void AddFiles(BG_FILE_INFO[] files)
        {
            try
            {
                uint count = Convert.ToUInt32(files.Length);
                job.AddFileSet(count, files);
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void AddFiles(Collection<BitsFileInfo> files)
        {
            var fileArray = new BG_FILE_INFO[files.Count];
            for (int i = 0; i < files.Count; i++)
                fileArray[i] = files[i]._BG_FILE_INFO;
            AddFiles(fileArray);
        }

        #endregion

        #endregion

        #region notification

        internal void JobTransferred(object sender, NotificationEventArgs e)
        {
            if (onJobTransfered != null)
                onJobTransfered(sender, new JobNotificationEventArgs());
        }

        internal void JobModified(object sender, NotificationEventArgs e)
        {
            if (onJobModified != null)
                onJobModified(sender, new JobNotificationEventArgs());
        }

        internal void JobError(object sender, ErrorNotificationEventArgs e)
        {
            if (null != onJobErrored)
                onJobErrored(sender, new JobErrorNotificationEventArgs(e.Error));
        }

        #endregion

        #region public events

        public event EventHandler<JobNotificationEventArgs> OnJobModified { add { onJobModified += value; } remove { onJobModified -= value; } }

        public event EventHandler<JobNotificationEventArgs> OnJobTransferred { add { onJobTransfered += value; } remove { onJobTransfered -= value; } }

        public event EventHandler<JobErrorNotificationEventArgs> OnJobError { add { onJobErrored += value; } remove { onJobErrored -= value; } }

        #endregion

        #region internal

        internal IBackgroundCopyJob Job { get { return job; } }

        internal void PublishException(COMException exception)
        {
            manager.PublishException(this, exception);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (NotificationTarget != null)
                        NotificationInterface = NotificationTarget;
                    if (files != null)
                        files.Dispose();
                    job = null;
                }
            }
            disposed = true;
        }
    }
}