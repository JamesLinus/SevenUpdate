//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    using SharpBits.Base.Job;

    /// <summary>
    /// Use the IBackgroundCopyManager interface to create transfer jobs, 
    ///   retrieve an enumerator object that contains the jobs in the queue, 
    ///   and to retrieve individual jobs from the queue.
    /// </summary>
    public sealed class BitsManager : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal JobOwner CurrentOwner;

        /// <summary>
        /// </summary>
        private bool disposed;

        /// <summary>
        /// </summary>
        private EventHandler<BitsInterfaceNotificationEventArgs> onInterfaceError;

        /// <summary>
        /// </summary>
        private EventHandler<NotificationEventArgs> onJobAdded;

        /// <summary>
        /// </summary>
        private EventHandler<ErrorNotificationEventArgs> onJobErrored;

        /// <summary>
        /// </summary>
        private EventHandler<NotificationEventArgs> onJobModified;

        /// <summary>
        /// </summary>
        private EventHandler<NotificationEventArgs> onJobRemoved;

        /// <summary>
        /// </summary>
        private EventHandler<NotificationEventArgs> onJobTransfered;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public BitsManager()
        {
            // Set threading apartment
            Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
            NativeMethods.COInitializeSecurity(
                IntPtr.Zero, -1, IntPtr.Zero, IntPtr.Zero, RpcAuthnLevels.Connect, RpcImpLevel.Impersonate, IntPtr.Zero, EoAuthnCap.None, IntPtr.Zero);

            this.BackgroundCopyManager = new BackgroundCopyManager() as IBackgroundCopyManager;
            this.Jobs = new BitsJobsDictionary(this); // will be set correctly later after initialization
            this.NotificationHandler = new BitsNotification(this);
            this.NotificationHandler.OnJobErrorEvent += this.NotificationHandlerOnJobErrorEvent;
            this.NotificationHandler.OnJobModifiedEvent += this.NotificationHandlerOnJobModifiedEvent;
            this.NotificationHandler.OnJobTransferredEvent += this.NotificationHandlerOnJobTransferredEvent;
        }

        #endregion

        #region Events

        /// <summary>
        /// </summary>
        public event EventHandler<BitsInterfaceNotificationEventArgs> OnInterfaceError
        {
            add
            {
                this.onInterfaceError += value;
            }

            remove
            {
                this.onInterfaceError -= value;
            }
        }

        /// <summary>
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobAdded
        {
            add
            {
                this.onJobAdded += value;
            }

            remove
            {
                this.onJobAdded -= value;
            }
        }

        /// <summary>
        /// </summary>
        public event EventHandler<ErrorNotificationEventArgs> OnJobError
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
        public event EventHandler<NotificationEventArgs> OnJobModified
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
        public event EventHandler<NotificationEventArgs> OnJobRemoved
        {
            add
            {
                this.onJobRemoved += value;
            }

            remove
            {
                this.onJobRemoved -= value;
            }
        }

        /// <summary>
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobTransferred
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
        /// </summary>
        public BitsVersion BitsVersion
        {
            get
            {
                return Utils.BitsVersion;
            }
        }

        /// <summary>
        /// </summary>
        public BitsJobsDictionary Jobs { get; private set; }

        /// <summary>
        /// </summary>
        internal IBackgroundCopyManager BackgroundCopyManager { get; private set; }

        /// <summary>
        /// </summary>
        internal BitsNotification NotificationHandler { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new transfer job.
        /// </summary>
        /// <param name="displayName">
        /// Null-terminated string that contains a display name for the job. 
        ///   Typically, the display name is used to identify the job in a user interface. 
        ///   Note that more than one job may have the same display name. Must not be NULL. 
        ///   The name is limited to 256 characters, not including the null terminator.
        /// </param>
        /// <param name="jobType">
        /// Type of transfer job, such as JobType.Download. For a list of transfer types, see the JobType enumeration
        /// </param>
        /// <returns>
        /// </returns>
        public BitsJob CreateJob(string displayName, JobType jobType)
        {
            Guid guid;
            IBackgroundCopyJob pJob;
            this.BackgroundCopyManager.CreateJob(displayName, (BGJobType)jobType, out guid, out pJob);
            BitsJob job;
            lock (this.Jobs)
            {
                job = new BitsJob(this, pJob);
                this.Jobs.Add(guid, job);
            }

            if (null != this.onJobAdded)
            {
                this.onJobAdded(this, new NotificationEventArgs(job));
            }

            return job;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public BitsJobsDictionary EnumJobs()
        {
            return this.EnumJobs(JobOwner.CurrentUser);
        }

        /// <summary>
        /// </summary>
        /// <param name="jobOwner">
        /// </param>
        /// <returns>
        /// </returns>
        public BitsJobsDictionary EnumJobs(JobOwner jobOwner)
        {
            if (this.BackgroundCopyManager == null)
            {
                return null;
            }

            this.CurrentOwner = jobOwner;
            IEnumBackgroundCopyJobs jobList;
            this.BackgroundCopyManager.EnumJobs((UInt32)jobOwner, out jobList);
            if (this.Jobs == null)
            {
                this.Jobs = new BitsJobsDictionary(this, jobList);
            }
            else
            {
                this.Jobs.Update(jobList);
            }

            return this.Jobs;
        }

        /// <summary>
        /// </summary>
        /// <param name="hResult">
        /// </param>
        /// <returns>
        /// </returns>
        public string GetErrorDescription(int hResult)
        {
            string description;
            this.BackgroundCopyManager.GetErrorDescription(hResult, Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
            return description;
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
        /// <param name="job">
        /// </param>
        internal void NotifyOnJobRemoval(BitsJob job)
        {
            if (null != this.onJobRemoved)
            {
                this.onJobRemoved(this, new NotificationEventArgs(job));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="job">
        /// </param>
        /// <param name="exception">
        /// </param>
        internal void PublishException(BitsJob job, COMException exception)
        {
            if (this.onInterfaceError == null)
            {
                return;
            }

            var description = this.GetErrorDescription(exception.ErrorCode);
            this.onInterfaceError(this, new BitsInterfaceNotificationEventArgs(job, exception, description));
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    foreach (var job in this.Jobs.Values)
                    {
                        job.Dispose();
                    }

                    this.Jobs.Clear();
                    this.Jobs.Dispose();
                    Marshal.ReleaseComObject(this.BackgroundCopyManager);
                    this.BackgroundCopyManager = null;
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void NotificationHandlerOnJobErrorEvent(object sender, ErrorNotificationEventArgs e)
        {
            // route the event to the job
            if (this.Jobs.ContainsKey(e.Job.JobId))
            {
                var job = this.Jobs[e.Job.JobId];
                job.JobError(sender, e);
            }

            // publish the event to other subscribers
            if (this.onJobErrored != null)
            {
                this.onJobErrored(sender, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void NotificationHandlerOnJobModifiedEvent(object sender, NotificationEventArgs e)
        {
            // route the event to the job
            if (this.Jobs.ContainsKey(e.Job.JobId))
            {
                var job = this.Jobs[e.Job.JobId];
                job.JobModified(sender, e);
            }

            // publish the event to other subscribers
            if (this.onJobModified != null)
            {
                this.onJobModified(sender, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void NotificationHandlerOnJobTransferredEvent(object sender, NotificationEventArgs e)
        {
            // route the event to the job
            if (this.Jobs.ContainsKey(e.Job.JobId))
            {
                var job = this.Jobs[e.Job.JobId];
                job.JobTransferred(sender, e);
            }

            // publish the event to other subscribers
            if (this.onJobTransfered != null)
            {
                this.onJobTransfered(sender, e);
            }
        }

        #endregion
    }
}