//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base.Job
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    using SharpBits.Base.File;
    using SharpBits.Base.Progress;

    /// <summary>
    /// </summary>
    public partial class BitsJob
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal IBackgroundCopyCallback NotificationTarget;

        /// <summary>
        /// </summary>
        private readonly BitsManager manager;

        /// <summary>
        /// </summary>
        private bool disposed;

        /// <summary>
        /// </summary>
        private BitsError error;

        /// <summary>
        /// </summary>
        private Guid guid;

        /// <summary>
        /// </summary>
        private JobTimes jobTimes;

        // notification
        /// <summary>
        /// </summary>
        private EventHandler<JobErrorNotificationEventArgs> onJobErrored;

        /// <summary>
        /// </summary>
        private EventHandler<JobNotificationEventArgs> onJobModified;

        /// <summary>
        /// </summary>
        private EventHandler<JobNotificationEventArgs> onJobTransfered;

        /// <summary>
        /// </summary>
        private JobProgress progress;

        /// <summary>
        /// </summary>
        private ProxySettings proxySettings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="manager">
        /// </param>
        /// <param name="job">
        /// </param>
        internal BitsJob(BitsManager manager, IBackgroundCopyJob job)
        {
            this.manager = manager;
            this.Job = job;
            this.job2 = this.Job as IBackgroundCopyJob2;
            this.job3 = this.Job as IBackgroundCopyJob3;
            this.job4 = this.Job as IBackgroundCopyJob4;

            // store existing notification handler and route message to this as well
            // otherwise it may break system download jobs
            if (this.NotificationInterface != null)
            {
                this.NotificationTarget = this.NotificationInterface; // pointer to the existing one;
            }

            this.NotificationInterface = manager.NotificationHandler; // notification interface will be disabled when NotifyCmd is set
        }

        #endregion

        #region Events

        /// <summary>
        /// </summary>
        public event EventHandler<JobErrorNotificationEventArgs> OnJobError
        {
            add
            {
                this.onJobErrored += value;
            }

            remove
            {
                this.onJobErrored -= value;
            }
        }

        /// <summary>
        /// </summary>
        public event EventHandler<JobNotificationEventArgs> OnJobModified
        {
            add
            {
                this.onJobModified += value;
            }

            remove
            {
                this.onJobModified -= value;
            }
        }

        /// <summary>
        /// </summary>
        public event EventHandler<JobNotificationEventArgs> OnJobTransferred
        {
            add
            {
                this.onJobTransfered += value;
            }

            remove
            {
                this.onJobTransfered -= value;
            }
        }

        #endregion

        #region Properties

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
                    this.Job.GetDescription(out description);
                    return description;
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return string.Empty;
                }
            }

            set
            {
                try
                {
                    this.Job.SetDescription(value);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

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
                    this.Job.GetDisplayName(out displayName);
                    return displayName;
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return string.Empty;
                }
            }

            set
            {
                try
                {
                    this.Job.SetDisplayName(value);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        /// </summary>
        public BitsError Error
        {
            get
            {
                try
                {
                    var state = this.State;
                    if (state == JobState.Error || state == JobState.TransientError)
                    {
                        if (null == this.error)
                        {
                            IBackgroundCopyError copyError;
                            this.Job.GetError(out copyError);
                            if (null != copyError)
                            {
                                this.error = new BitsError(this, copyError);
                            }
                        }
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return this.error;
            }
        }

        /// <summary>
        /// </summary>
        public ulong ErrorCount
        {
            get
            {
                ulong count = 0;
                try
                {
                    this.Job.GetErrorCount(out count);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return count;
            }
        }

        /// <summary>
        /// </summary>
        public BitsFilesCollection Files { get; private set; }

        /// <summary>
        /// </summary>
        public Guid JobId
        {
            get
            {
                try
                {
                    if (this.guid == Guid.Empty)
                    {
                        this.Job.GetId(out this.guid);
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return this.guid;
            }
        }

        /// <summary>
        /// </summary>
        public JobTimes JobTimes
        {
            get
            {
                try
                {
                    BGJobTimes times;
                    this.Job.GetTimes(out times);
                    this.jobTimes = new JobTimes(times);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return this.jobTimes;
            }
        }

        /// <summary>
        /// </summary>
        public JobType JobType
        {
            get
            {
                var jobType = BGJobType.Unknown;
                try
                {
                    this.Job.GetType(out jobType);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return (JobType)jobType;
            }
        }

        /// <summary>
        /// </summary>
        public uint MinimumRetryDelay
        {
            get
            {
                uint seconds = 0;
                try
                {
                    this.Job.GetMinimumRetryDelay(out seconds);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return seconds;
            }

            set
            {
                try
                {
                    this.Job.SetMinimumRetryDelay(value);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        /// </summary>
        public uint NoProgressTimeout
        {
            get
            {
                uint seconds = 0;
                try
                {
                    this.Job.GetNoProgressTimeout(out seconds);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return seconds;
            }

            set
            {
                try
                {
                    this.Job.SetNoProgressTimeout(value);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        /// </summary>
        public NotificationFlags NotificationFlags
        {
            get
            {
                BGJobNotificationTypes flags = 0;
                try
                {
                    this.Job.GetNotifyFlags(out flags);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return (NotificationFlags)flags;
            }

            set
            {
                try
                {
                    this.Job.SetNotifyFlags((BGJobNotificationTypes)value);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
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
                    this.Job.GetOwner(out owner);
                    return owner;
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
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
                    return Utils.GetName(this.Owner);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
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
                var priority = BGJobPriority.Normal;
                try
                {
                    this.Job.GetPriority(out priority);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return (JobPriority)priority;
            }

            set
            {
                try
                {
                    this.Job.SetPriority((BGJobPriority)value);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        /// </summary>
        public JobProgress Progress
        {
            get
            {
                try
                {
                    BGJobProgress jobProgress;
                    this.Job.GetProgress(out jobProgress);
                    this.progress = new JobProgress(jobProgress);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return this.progress;
            }
        }

        /// <summary>
        /// </summary>
        public ProxySettings ProxySettings
        {
            get
            {
                return this.proxySettings ?? (this.proxySettings = new ProxySettings(this.Job));
            }
        }

        /// <summary>
        /// </summary>
        public JobState State
        {
            get
            {
                var state = BGJobState.Unknown;
                try
                {
                    this.Job.GetState(out state);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return (JobState)state;
            }
        }

        /// <summary>
        /// </summary>
        internal IBackgroundCopyJob Job { get; private set; }

        /// <summary>
        /// </summary>
        internal IBackgroundCopyCallback NotificationInterface
        {
            get
            {
                object notificationInterface = null;
                try
                {
                    this.Job.GetNotifyInterface(out notificationInterface);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return notificationInterface as IBackgroundCopyCallback;
            }

            set
            {
                try
                {
                    this.Job.SetNotifyInterface(value);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="remoteName">
        /// </param>
        /// <param name="localName">
        /// </param>
        public void AddFile(string remoteName, string localName)
        {
            try
            {
                this.Job.AddFile(remoteName, localName);
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fileInfo">
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public void AddFile(BitsFileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }

            this.AddFile(fileInfo.RemoteName, fileInfo.LocalName);
        }

        /// <summary>
        /// </summary>
        /// <param name="files">
        /// </param>
        public void AddFiles(Collection<BitsFileInfo> files)
        {
            var fileArray = new BGFileInfo[files.Count];
            for (var i = 0; i < files.Count; i++)
            {
                fileArray[i] = files[i].BGFileInfo;
            }

            AddFiles(fileArray);
        }

        /// <summary>
        /// </summary>
        public void Cancel()
        {
            try
            {
                this.Job.Cancel();
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// </summary>
        public void Complete()
        {
            try
            {
                this.Job.Complete();
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public BitsFilesCollection EnumFiles()
        {
            try
            {
                IEnumBackgroundCopyFiles fileList;
                this.Job.EnumFiles(out fileList);
                this.Files = new BitsFilesCollection(this, fileList);
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }

            return this.Files;
        }

        /// <summary>
        /// </summary>
        public void Resume()
        {
            try
            {
                this.Job.Resume();
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// </summary>
        public void Suspend()
        {
            try
            {
                this.Job.Suspend();
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// </summary>
        public void TakeOwnership()
        {
            try
            {
                this.Job.TakeOwnership();
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="files">
        /// </param>
        internal void AddFiles(BGFileInfo[] files)
        {
            try
            {
                var count = Convert.ToUInt32(files.Length);
                this.Job.AddFileSet(count, files);
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        internal void JobError(object sender, ErrorNotificationEventArgs e)
        {
            if (null != this.onJobErrored)
            {
                this.onJobErrored(sender, new JobErrorNotificationEventArgs(e.Error));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        internal void JobModified(object sender, NotificationEventArgs e)
        {
            if (this.onJobModified != null)
            {
                this.onJobModified(sender, new JobNotificationEventArgs());
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        internal void JobTransferred(object sender, NotificationEventArgs e)
        {
            if (this.onJobTransfered != null)
            {
                this.onJobTransfered(sender, new JobNotificationEventArgs());
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="exception">
        /// </param>
        internal void PublishException(COMException exception)
        {
            this.manager.PublishException(this, exception);
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.NotificationTarget != null)
                    {
                        this.NotificationInterface = this.NotificationTarget;
                    }

                    if (this.Files != null)
                    {
                        this.Files.Dispose();
                    }

                    this.Job = null;
                    if (this.manager != null)
                    {
                        this.manager.Dispose();
                    }
                }
            }

            this.disposed = true;
        }

        #endregion
    }
}