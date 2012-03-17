// ***********************************************************************
// <copyright file="BitsManager.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    ///   Use the <c>IBackgroundCopyManager</c> interface to create transfer jobs, retrieve an enumerator object that
    ///   contains the jobs in the queue, and to retrieve individual jobs from the queue.
    /// </summary>
    public sealed class BitsManager : IDisposable
    {
        #region Constants and Fields

        /// <summary>Indicates if this instance is disposed.</summary>
        private bool disposed;

        /// <summary>Occurs when there is an interface error.</summary>
        private EventHandler<BitsInterfaceNotificationEventArgs> interfaceError;

        /// <summary>Occurs when a job is added.</summary>
        private EventHandler<NotificationEventArgs> jobAdded;

        /// <summary>Occurs when a job error occurs.</summary>
        private EventHandler<ErrorNotificationEventArgs> jobError;

        /// <summary>Occurs when a job is modified.</summary>
        private EventHandler<NotificationEventArgs> jobModified;

        /// <summary>Occurs when a job is removed.</summary>
        private EventHandler<NotificationEventArgs> jobRemoved;

        /// <summary>Occurs when a job has transfered.</summary>
        private EventHandler<NotificationEventArgs> jobTransferred;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BitsManager" /> class.</summary>
        public BitsManager()
        {
            // Set threading apartment
            Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);

            this.BackgroundCopyManager = new BackgroundCopyManager() as IBackgroundCopyManager;
            this.Jobs = new BitsJobsDictionary(this); // will be set correctly later after initialization
            this.NotificationHandler = new BitsNotification(this);

            this.NotificationHandler.OnJobErrorEvent -= this.NotificationHandlerOnJobErrorEvent;
            this.NotificationHandler.OnJobModifiedEvent -= this.NotificationHandlerOnJobModifiedEvent;
            this.NotificationHandler.OnJobTransferredEvent -= this.NotificationHandlerOnJobTransferredEvent;

            this.NotificationHandler.OnJobErrorEvent += this.NotificationHandlerOnJobErrorEvent;
            this.NotificationHandler.OnJobModifiedEvent += this.NotificationHandlerOnJobModifiedEvent;
            this.NotificationHandler.OnJobTransferredEvent += this.NotificationHandlerOnJobTransferredEvent;
        }

        #endregion

        #region Public Events

        /// <summary>Occurs when an interface error occurs.</summary>
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

        /// <summary>Occurs when a <c>BitsJob</c> is added.</summary>
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

        /// <summary>Occurs when a <c>BitsJob</c> error occurs.</summary>
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

        /// <summary>Occurs when a <c>BitsJob</c> is modified.</summary>
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

        /// <summary>Occurs when a <c>BitsJob</c> is removed.</summary>
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

        /// <summary>Occurs when a <c>BitsJob</c> is transfered.</summary>
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

        #region Public Properties

        /// <summary>Gets the collection of <c>BitsJob</c>.</summary>
        /// <value>The collection of <c>BitsJob</c>.</value>
        public BitsJobsDictionary Jobs { get; private set; }

        #endregion

        #region Properties

        /// <summary>Gets or sets current owner of the job.</summary>
        /// <value>The current owner.</value>
        internal JobOwner CurrentOwner { get; set; }

        /// <summary>Gets the notification handler.</summary>
        /// <value>The notification handler.</value>
        internal BitsNotification NotificationHandler { get; private set; }

        /// <summary>Gets or sets the background copy manager.</summary>
        /// <value>The background copy manager.</value>
        private IBackgroundCopyManager BackgroundCopyManager { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Creates a new transfer job.</summary>
        /// <param name="displayName">Null-terminated string that contains a display name for the job. Typically, the display name is used to identify the job in a user interface. Note that more than one job may have the same display name. Must not be <c>null</c>.The name is limited to 256 characters, not including the <c>null</c> terminator.</param>
        /// <param name="jobType">Type of transfer job, such as <c>JobType</c>.Download. For a list of transfer types,
        /// see the <see cref="JobType" /> enumeration.</param>
        /// <returns>The <c>BitsJob</c> created.</returns>
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

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(this.disposed);
            GC.SuppressFinalize(this);
        }

        /// <summary>Enumerates the collection of <c>BitsJob</c>, it also completes any job that has finished.</summary>
        /// <param name="jobOwner">The job owner.</param>
        public void EnumJobs(JobOwner jobOwner = JobOwner.CurrentUser)
        {
            if (this.BackgroundCopyManager == null)
            {
                return;
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
        }

        #endregion

        #region Methods

        /// <summary>Notifies the on job removal.</summary>
        /// <param name="job">The job to remove.</param>
        internal void NotifyOnJobRemoval(BitsJob job)
        {
            if (null != this.jobRemoved)
            {
                this.jobRemoved(this, new NotificationEventArgs(job));
            }
        }

        /// <summary>Publishes the exception.</summary>
        /// <param name="job">The job the exception occurred.</param>
        /// <param name="exception">The exception.</param>
        internal void PublishException(BitsJob job, COMException exception)
        {
            if (this.interfaceError == null)
            {
                return;
            }

            string description = this.GetErrorDescription(exception.ErrorCode);
            this.interfaceError(this, new BitsInterfaceNotificationEventArgs(job, exception, description));
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><c>True</c> to release both managed and unmanaged resources; otherwise, <c>False</c> to release only unmanaged resources.</param>
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

        /// <summary>Gets the error description.</summary>
        /// <param name="result">The h result.</param>
        /// <returns>The error description.</returns>
        private string GetErrorDescription(int result)
        {
            string description;
            this.BackgroundCopyManager.GetErrorDescription(
                result, Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
            return description;
        }

        /// <summary>Notifications the handler on job error event.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>SharpBits.Base.ErrorNotificationEventArgs</c> instance containing the event data.</param>
        private void NotificationHandlerOnJobErrorEvent(object sender, ErrorNotificationEventArgs e)
        {
            // route the event to the job
            if (this.Jobs.ContainsKey(e.Job.JobId))
            {
                BitsJob job = this.Jobs[e.Job.JobId];
                job.JobError(sender, e);
            }

            // publish the event to other subscribers
            if (this.jobError != null)
            {
                this.jobError(sender, e);
            }
        }

        /// <summary>Notifications the handler on job modified event.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>NotificationEventArgs</c> instance containing the event data.</param>
        private void NotificationHandlerOnJobModifiedEvent(object sender, NotificationEventArgs e)
        {
            // route the event to the job
            if (this.Jobs.ContainsKey(e.Job.JobId))
            {
                BitsJob job = this.Jobs[e.Job.JobId];
                job.JobModified(sender);
            }

            // publish the event to other subscribers
            if (this.jobModified != null)
            {
                this.jobModified(sender, e);
            }
        }

        /// <summary>Notifications the handler on job transferred event.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>NotificationEventArgs</c> instance containing the event data.</param>
        private void NotificationHandlerOnJobTransferredEvent(object sender, NotificationEventArgs e)
        {
            // route the event to the job
            if (this.Jobs.ContainsKey(e.Job.JobId))
            {
                BitsJob job = this.Jobs[e.Job.JobId];
                job.JobTransferred(sender);
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