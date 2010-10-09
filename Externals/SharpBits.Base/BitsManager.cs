// ***********************************************************************
// <copyright file="BitsManager.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar/author>
// ***********************************************************************
namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    using SharpBits.Base.Job;

    /// <summary>
    /// Use the <see cref="IBackgroundCopyManager"/> interface to create transfer jobs, 
    ///   retrieve an enumerator object that contains the jobs in the queue, 
    ///   and to retrieve individual jobs from the queue.
    /// </summary>
    public sealed class BitsManager : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   Indicates if this instance is disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        ///   Occurs when there is an interface error
        /// </summary>
        private EventHandler<BitsInterfaceNotificationEventArgs> interfaceError;

        /// <summary>
        ///   Occurs when a job is added
        /// </summary>
        private EventHandler<NotificationEventArgs> jobAdded;

        /// <summary>
        ///   Occurs when a job error occurs
        /// </summary>
        private EventHandler<ErrorNotificationEventArgs> jobError;

        /// <summary>
        ///   Occurs when a job is modified
        /// </summary>
        private EventHandler<NotificationEventArgs> jobModified;

        /// <summary>
        ///   Occurs when a job is removed
        /// </summary>
        private EventHandler<NotificationEventArgs> jobRemoved;

        /// <summary>
        ///   Occurs when a job has transfered
        /// </summary>
        private EventHandler<NotificationEventArgs> jobTransferred;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BitsManager" /> class.
        /// </summary>
        public BitsManager()
        {
            // Set threading apartment
            Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
            NativeMethods.COInitializeSecurity(
                IntPtr.Zero, -1, IntPtr.Zero, IntPtr.Zero, RpcAuthLevels.Connect, RpcImpLevel.Impersonate, IntPtr.Zero, EoAuthCap.None, IntPtr.Zero);

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
        ///   Occurs when an interface error occurs
        /// </summary>
        public event EventHandler<BitsInterfaceNotificationEventArgs> OnInterfaceError
        {
            add
            {
                this.interfaceError += value;
            }

            remove
            {
                this.interfaceError -= value;
            }
        }

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> is added
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobAdded
        {
            add
            {
                this.jobAdded += value;
            }

            remove
            {
                this.jobAdded -= value;
            }
        }

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> error occurs
        /// </summary>
        public event EventHandler<ErrorNotificationEventArgs> OnJobError
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

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> is modified
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobModified
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

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> is removed
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobRemoved
        {
            add
            {
                this.jobRemoved += value;
            }

            remove
            {
                this.jobRemoved -= value;
            }
        }

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> is transfered
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobTransferred
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

        /// <summary>
        ///   Gets the bits version.
        /// </summary>
        /// <value>The bits version.</value>
        public BitsVersion BitsVersion
        {
            get
            {
                return Utilities.BitsVersion;
            }
        }

        /// <summary>
        ///   Gets the collection of <see cref = "BitsJob" />
        /// </summary>
        /// <value>The collection of <see cref = "BitsJob" /></value>
        public BitsJobsDictionary Jobs { get; private set; }

        /// <summary>
        ///   Gets the background copy manager.
        /// </summary>
        /// <value>The background copy manager.</value>
        internal IBackgroundCopyManager BackgroundCopyManager { get; private set; }

        /// <summary>
        ///   Gets or sets current owner of the job
        /// </summary>
        /// <value>The current owner.</value>
        internal JobOwner CurrentOwner { get; set; }

        /// <summary>
        ///   Gets the notification handler.
        /// </summary>
        /// <value>The notification handler.</value>
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
        ///   The name is limited to 256 characters, not including the <see langword="null"/> terminator.
        /// </param>
        /// <param name="jobType">
        /// Type of transfer job, such as <see cref="JobType"/>.Download. For a list of transfer types, see the <see cref="JobType"/> enumeration
        /// </param>
        /// <returns>
        /// The <see cref="BitsJob"/> created
        /// </returns>
        public BitsJob CreateJob(string displayName, JobType jobType)
        {
            Guid guid;
            IBackgroundCopyJob copyJob;
            this.BackgroundCopyManager.CreateJob(displayName, (BGJobType)jobType, out guid, out copyJob);
            BitsJob job;
            lock (this.Jobs)
            {
                job = new BitsJob(this, copyJob);
                this.Jobs.Add(guid, job);
            }

            if (null != this.jobAdded)
            {
                this.jobAdded(this, new NotificationEventArgs(job));
            }

            return job;
        }

        /// <summary>
        /// Enumerates the collection of <see cref="BitsJob"/>
        /// </summary>
        /// <returns>
        /// The collection of <see cref="BitsJob"/>
        /// </returns>
        public BitsJobsDictionary EnumJobs()
        {
            return this.EnumJobs(JobOwner.CurrentUser);
        }

        /// <summary>
        /// Enumerates the collection of <see cref="BitsJob"/>
        /// </summary>
        /// <param name="jobOwner">
        /// The job owner.
        /// </param>
        /// <returns>
        /// The collection of <see cref="BitsJob"/>
        /// </returns>
        public BitsJobsDictionary EnumJobs(JobOwner jobOwner)
        {
            if (this.BackgroundCopyManager == null)
            {
                return null;
            }

            this.CurrentOwner = jobOwner;
            IEnumBackgroundCopyJobs jobList;
            this.BackgroundCopyManager.EnumJobs((uint)jobOwner, out jobList);
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
        /// Gets the error description.
        /// </summary>
        /// <param name="result">
        /// The h result.
        /// </param>
        /// <returns>
        /// The error description
        /// </returns>
        public string GetErrorDescription(int result)
        {
            string description;
            this.BackgroundCopyManager.GetErrorDescription(result, Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
            return description;
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
        /// Notifies the on job removal.
        /// </summary>
        /// <param name="job">
        /// The job to remove
        /// </param>
        internal void NotifyOnJobRemoval(BitsJob job)
        {
            if (null != this.jobRemoved)
            {
                this.jobRemoved(this, new NotificationEventArgs(job));
            }
        }

        /// <summary>
        /// Publishes the exception.
        /// </summary>
        /// <param name="job">
        /// The job the exception occurred
        /// </param>
        /// <param name="exception">
        /// The exception
        /// </param>
        internal void PublishException(BitsJob job, COMException exception)
        {
            if (this.interfaceError == null)
            {
                return;
            }

            var description = this.GetErrorDescription(exception.ErrorCode);
            this.interfaceError(this, new BitsInterfaceNotificationEventArgs(job, exception, description));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.
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
        /// Notifications the handler on job error event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="SharpBits.Base.ErrorNotificationEventArgs"/> instance containing the event data.
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
            if (this.jobError != null)
            {
                this.jobError(sender, e);
            }
        }

        /// <summary>
        /// Notifications the handler on job modified event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="SharpBits.Base.NotificationEventArgs"/> instance containing the event data.
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
            if (this.jobModified != null)
            {
                this.jobModified(sender, e);
            }
        }

        /// <summary>
        /// Notifications the handler on job transferred event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="SharpBits.Base.NotificationEventArgs"/> instance containing the event data.
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
            if (this.jobTransferred != null)
            {
                this.jobTransferred(sender, e);
            }
        }

        #endregion
    }
}