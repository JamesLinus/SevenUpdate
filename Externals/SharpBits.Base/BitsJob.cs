// ***********************************************************************
// <copyright file="BitsJob.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************
namespace SharpBits.Base
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>Contains data about the files to download or upload using BITS</summary>
    public partial class BitsJob
    {
        #region Constants and Fields

        /// <summary>The BITS manager</summary>
        private readonly BitsManager manager;

        /// <summary>Indicates if the <see cref = "BitsJob" /> has been disposed</summary>
        private bool disposed;

        /// <summary>Data about the error</summary>
        private BitsError error;

        /// <summary>The job GUID</summary>
        private Guid guid;

        /// <summary>Occurs when a job has has an error occur</summary>
        private EventHandler<JobErrorNotificationEventArgs> jobError;

        /// <summary>Occurs when a job has been modified</summary>
        private EventHandler<JobNotificationEventArgs> jobModified;

        /// <summary>The times for the current job</summary>
        private JobTimes jobTimes;

        /// <summary>Occurs when a job as transferred</summary>
        private EventHandler<JobNotificationEventArgs> jobTransferred;

        /// <summary>The current progress of the job</summary>
        private JobProgress progress;

        /// <summary>The proxy settings of the job</summary>
        private ProxySettings proxySettings;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BitsJob"/> class.</summary>
        /// <param name="manager">The manager for the BITS</param>
        /// <param name="job">The current job</param>
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

        /// <summary>Occurs when the job occurred an error.</summary>
        public event EventHandler<JobErrorNotificationEventArgs> OnJobError
        {
            add
            {
                this.jobError += value;
            }

            remove
            {
                this.jobError -= value;
            }
        }

        /// <summary>Occurs when the job has been modified.</summary>
        public event EventHandler<JobNotificationEventArgs> OnJobModified
        {
            add
            {
                this.jobModified += value;
            }

            remove
            {
                this.jobModified -= value;
            }
        }

        /// <summary>Occurs when the job has been transferred.</summary>
        public event EventHandler<JobNotificationEventArgs> OnJobTransferred
        {
            add
            {
                this.jobTransferred += value;
            }

            remove
            {
                this.jobTransferred -= value;
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the description, max 1024 chars</summary>
        /// <value>The description.</value>
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

        /// <summary>Gets or sets the display name, max 256 chars</summary>
        /// <value>The display name.</value>
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

        /// <summary>Gets the error that occurred</summary>
        /// <value>The error.</value>
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

        /// <summary>Gets the error count.</summary>
        /// <value>The error count.</value>
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

        /// <summary>Gets the collection of <see cref = "BitsFile" />'s for the job</summary>
        /// <value>The files.</value>
        public BitsFilesCollection Files
        {
            get;
            private set;
        }

        /// <summary>Gets the GUID of the current job</summary>
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

        /// <summary>Gets the job times for the current <see cref = "BitsJob" /></summary>
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

        /// <summary>Gets the type of the current <see cref = "BitsJob" /></summary>
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

        /// <summary>Gets or sets the minimum retry delay.</summary>
        /// <value>The minimum retry delay.</value>
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

        /// <summary>Gets or sets the no progress timeout.</summary>
        /// <value>The no progress timeout.</value>
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

        /// <summary>Gets or sets the notification flags.</summary>
        /// <value>The notification flags.</value>
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

        /// <summary>Gets the SID of the job owner</summary>
        /// <value>The owner.</value>
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

        /// <summary>Gets the resolved owner name from job owner SID</summary>
        /// <value>The name of the owner.</value>
        public string OwnerName
        {
            get
            {
                try
                {
                    return Utilities.GetName(this.Owner);
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
        }

        /// <summary>Gets or sets the Job priority</summary>
        /// <remarks>Can not be set for jobs already in state Canceled or Acknowledged</remarks>
        /// <value>The priority.</value>
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

        /// <summary>Gets the progress.</summary>
        /// <value>The progress.</value>
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

        /// <summary>Gets the proxy settings.</summary>
        /// <value>The proxy settings.</value>
        public ProxySettings ProxySettings
        {
            get
            {
                return this.proxySettings ?? (this.proxySettings = new ProxySettings(this.Job));
            }
        }

        /// <summary>Gets the state.</summary>
        /// <value>The state.</value>
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

        /// <summary>Gets the current <see cref = "BitsJob" /></summary>
        /// <value>The <see cref = "BitsJob" /></value>
        internal IBackgroundCopyJob Job
        {
            get;
            private set;
        }

        /// <summary>Gets or sets the notification interface.</summary>
        /// <value>The notification interface.</value>
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

        /// <summary>Gets or sets the notification target</summary>
        /// <value>The notification target.</value>
        internal IBackgroundCopyCallback NotificationTarget
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        /// <summary>Adds the file.</summary>
        /// <param name="remoteName">Name of the remote.</param>
        /// <param name="localName">Name of the local.</param>
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

        /// <summary>Adds the file.</summary>
        /// <param name="fileInfo">The file info.</param>
        public void AddFile(BitsFileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }

            this.AddFile(fileInfo.RemoteName, fileInfo.LocalName);
        }

        /// <summary>Adds the files the current <see cref="BitsJob"/></summary>
        /// <param name="files">The files.</param>
        public void AddFiles(Collection<BitsFileInfo> files)
        {
            var fileArray = new BGFileInfo[files.Count];
            for (var i = 0; i < files.Count; i++)
            {
                fileArray[i] = files[i].BGFileInfo;
            }

            this.AddFiles(fileArray);
        }

        /// <summary>Cancels the <see cref="BitsJob"/></summary>
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

        /// <summary>Completes and removes the <see cref="BitsJob"/> from the collection</summary>
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

        /// <summary>Enumerate the <see cref="BitsFile"/> collection</summary>
        /// <returns>The collection of <see cref="BitsFile"/>'s</returns>
        public BitsFilesCollection EnumerateFiles()
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

        /// <summary>Resumes the <see cref="BitsJob"/></summary>
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

        /// <summary>Suspends the <see cref="BitsJob"/></summary>
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

        /// <summary>Takes the ownership.</summary>
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

        /// <summary>Releases unmanaged and - optionally - managed resources</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>Adds the files to the current job</summary>
        /// <param name="files">The files.</param>
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

        /// <summary>Fires the event when the job has occurred an error</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SharpBits.Base.ErrorNotificationEventArgs"/> instance containing the event data.</param>
        internal void JobError(object sender, ErrorNotificationEventArgs e)
        {
            if (null != this.jobError)
            {
                this.jobError(sender, new JobErrorNotificationEventArgs(e.Error));
            }
        }

        /// <summary>Fires the event when the job has been modified</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotificationEventArgs"/> instance containing the event data.</param>
        internal void JobModified(object sender, NotificationEventArgs e)
        {
            if (this.jobModified != null)
            {
                this.jobModified(sender, new JobNotificationEventArgs());
            }
        }

        /// <summary>Fires the event when the job has transferred</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotificationEventArgs"/> instance containing the event data.</param>
        internal void JobTransferred(object sender, NotificationEventArgs e)
        {
            if (this.jobTransferred != null)
            {
                this.jobTransferred(sender, new JobNotificationEventArgs());
            }
        }

        /// <summary>Publishes the exception.</summary>
        /// <param name="exception">The exception.</param>
        internal void PublishException(COMException exception)
        {
            this.manager.PublishException(this, exception);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources</summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
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

    /// <summary>Contains data about the files to download or upload using BITS</summary>
    public partial class BitsJob
    {
        #region Constants and Fields

        /// <summary>The current job</summary>
        private readonly IBackgroundCopyJob2 job2;

        /// <summary>The current reply progress</summary>
        private JobReplyProgress replyProgress;

        #endregion

        #region Properties

        /// <summary>Gets or sets the notify command line parameters.</summary>
        /// <value>The notify command line parameters.</value>
        public string NotifyCommandLineParameters
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        string program, parameters;
                        this.job2.GetNotifyCmdLine(out program, out parameters);
                        return parameters;
                    }

                    throw new NotSupportedException("IBackgroundCopyJob2");
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
                    if (this.job2 != null)
                    {
                        string program, parameters;
                        this.job2.GetNotifyCmdLine(out program, out parameters);
                        if (value != null)
                        {
                            // the command line program should be passed as first parameter, enclosed by quotes
                            value = string.Format(CultureInfo.CurrentCulture, "\"{0}\" {1}", program, value);
                        }

                        this.job2.SetNotifyCmdLine(program, value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>Gets or sets the notify command line program.</summary>
        /// <value>The notify command line program.</value>
        public string NotifyCommandLineProgram
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        string program, parameters;
                        this.job2.GetNotifyCmdLine(out program, out parameters);
                        return program;
                    }

                    throw new NotSupportedException("IBackgroundCopyJob2");
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
                    if (this.job2 != null)
                    {
                        if (null == value)
                        {
                            // removing command line, thus re-enabling the notification interface
                            this.job2.SetNotifyCmdLine(null, null);
                            this.NotificationInterface = this.manager.NotificationHandler;
                        }
                        else
                        {
                            this.NotificationInterface = null;
                            string program, parameters;
                            this.job2.GetNotifyCmdLine(out program, out parameters);
                            this.job2.SetNotifyCmdLine(value, parameters);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>Gets or sets the reply file name</summary>
        /// <value>The reply filename</value>
        public string ReplyFileName
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        string replyFileName;
                        this.job2.GetReplyFileName(out replyFileName);
                        return replyFileName;
                    }

                    throw new NotSupportedException("IBackgroundCopyJob2");
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
                    if (this.job2 != null)
                    {
                        // only supported from IBackgroundCopyJob2 and above
                        this.job2.SetReplyFileName(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>Gets the reply progress.</summary>
        /// <value>The reply progress.</value>
        public JobReplyProgress ReplyProgress
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        BGJobReplyProgress replyJobProgress;
                        this.job2.GetReplyProgress(out replyJobProgress);
                        this.replyProgress = new JobReplyProgress(replyJobProgress);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return null;
                }

                return this.replyProgress;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Adds the credentials.</summary>
        /// <param name="credentials">The credentials.</param>
        public void AddCredentials(BitsCredentials credentials)
        {
            try
            {
                if (this.job2 != null && credentials != null)
                {
                    // only supported from IBackgroundCopyJob2 and above
                    var authCredentials = new BGAuthCredentials
                        {
                            Scheme = (BGAuthScheme)credentials.AuthenticationScheme,
                            Target = (BGAuthTarget)credentials.AuthenticationTarget
                        };
                    authCredentials.Credentials.Basic.Password = credentials.Password;
                    authCredentials.Credentials.Basic.UserName = credentials.UserName;
                    this.job2.SetCredentials(ref authCredentials);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>Gets the reply data.</summary>
        /// <returns>The byte array containing the reply data</returns>
        public byte[] GetReplyData()
        {
            try
            {
                if (this.job2 != null)
                {
                    ulong length;
                    var bufferPtr = new IntPtr();
                    this.job2.GetReplyData(bufferPtr, out length);
                    var buffer = new byte[length];
                    Marshal.Copy(bufferPtr, buffer, 0, (int)length); // truncating length to int may be ok as the buffer could be 1MB maximum
                    return buffer;
                }

                throw new NotSupportedException("IBackgroundCopyJob2");
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
                return null;
            }
        }

        /// <summary>Removes the credentials.</summary>
        /// <param name="credentials">The credentials.</param>
        public void RemoveCredentials(BitsCredentials credentials)
        {
            try
            {
                if (this.job2 != null && credentials != null)
                {
                    // only supported from IBackgroundCopyJob2 and above
                    this.job2.RemoveCredentials((BGAuthTarget)credentials.AuthenticationTarget, (BGAuthScheme)credentials.AuthenticationScheme);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>Removes the credentials.</summary>
        /// <param name="target">The target.</param>
        /// <param name="scheme">The scheme.</param>
        public void RemoveCredentials(AuthenticationTarget target, AuthenticationScheme scheme)
        {
            try
            {
                if (this.job2 != null)
                {
                    // only supported from IBackgroundCopyJob2 and above
                    this.job2.RemoveCredentials((BGAuthTarget)target, (BGAuthScheme)scheme);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        #endregion
    }

    /// <summary>Contains data about the files to download or upload using BITS</summary>
    public partial class BitsJob : IDisposable
    {
        #region Constants and Fields

        /// <summary>The current job</summary>
        private readonly IBackgroundCopyJob3 job3;

        #endregion

        #region Properties

        /// <summary>Gets or sets the file acl flags.</summary>
        /// <value>The file acl flags.</value>
        public FileAclFlags FileAclFlags
        {
            get
            {
                BGFileAclFlags flags = 0;
                try
                {
                    if (this.job3 != null)
                    {
                        // only supported from IBackgroundCopyJob3 and above
                        this.job3.GetFileAclFlags(out flags);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob3");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return (FileAclFlags)flags;
            }

            set
            {
                try
                {
                    if (this.job3 != null)
                    {
                        // only supported from IBackgroundCopyJob3 and above
                        this.job3.SetFileAclFlags((BGFileAclFlags)value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob3");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Adds the file with ranges.</summary>
        /// <param name="remoteName">Name of the remote.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="fileRanges">The file ranges.</param>
        public void AddFileWithRanges(string remoteName, string localName, Collection<FileRange> fileRanges)
        {
            try
            {
                if (this.job3 != null && fileRanges != null)
                {
                    // only supported from IBackgroundCopyJob3 and above
                    var ranges = new BGFileRange[fileRanges.Count];
                    for (var i = 0; i < fileRanges.Count; i++)
                    {
                        ranges[i] = fileRanges[i].BGFileRange;
                    }

                    this.job3.AddFileWithRanges(remoteName, localName, (uint)fileRanges.Count, ranges);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob3");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>Replaces the remote prefix.</summary>
        /// <param name="oldPrefix">The old prefix.</param>
        /// <param name="newPrefix">The new prefix.</param>
        public void ReplaceRemotePrefix(string oldPrefix, string newPrefix)
        {
            try
            {
                if (this.job3 != null)
                {
                    // only supported from IBackgroundCopyJob3 and above
                    this.job3.ReplaceRemotePrefix(oldPrefix, newPrefix);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob3");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        #endregion
    }

    /// <summary>Contains data about the files to download or upload using BITS</summary>
    public partial class BitsJob
    {
        #region Constants and Fields

        /// <summary>The current job</summary>
        private readonly IBackgroundCopyJob4 job4;

        #endregion

        #region Properties

        /// <summary>Gets or sets the peer caching flags.</summary>
        /// <value>The peer caching flags.</value>
        public PeerCachingFlags CachingFlags
        {
            get
            {
                PeerCachingFlags peerCaching = 0;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetPeerCachingFlags(out peerCaching);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return peerCaching;
            }

            set
            {
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.SetPeerCachingFlags(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>Gets or sets the maximum download time.</summary>
        /// <value>The maximum download time.</value>
        public ulong MaximumDownloadTime
        {
            get
            {
                ulong maxTime = 0;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetMaximumDownloadTime(out maxTime);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return maxTime;
            }

            set
            {
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.SetMaximumDownloadTime(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>Gets a value indicating whether the owner is elevated</summary>
        /// <value><see langword = "true" /> if the owner is elevated; otherwise, <see langword = "false" />.</value>
        public bool OwnerElevationState
        {
            get
            {
                var elevated = false;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetOwnerElevationState(out elevated);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return elevated;
            }
        }

        /// <summary>Gets the owner integrity level.</summary>
        /// <value>The owner integrity level.</value>
        public ulong OwnerIntegrityLevel
        {
            get
            {
                ulong integrityLevel = 0;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetOwnerIntegrityLevel(out integrityLevel);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return integrityLevel;
            }
        }

        #endregion
    }
}