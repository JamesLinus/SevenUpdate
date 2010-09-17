// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.Runtime.InteropServices;
using System.Threading;
using SharpBits.Base.Job;

#endregion

namespace SharpBits.Base
{
    /// <summary>
    ///   Use the IBackgroundCopyManager interface to create transfer jobs, 
    ///   retrieve an enumerator object that contains the jobs in the queue, 
    ///   and to retrieve individual jobs from the queue.
    /// </summary>
    public sealed class BitsManager : IDisposable
    {
        internal JobOwner CurrentOwner;
        private bool disposed;
        private EventHandler<BitsInterfaceNotificationEventArgs> onInterfaceError;
        private EventHandler<NotificationEventArgs> onJobAdded;
        private EventHandler<ErrorNotificationEventArgs> onJobErrored;
        private EventHandler<NotificationEventArgs> onJobModified;
        private EventHandler<NotificationEventArgs> onJobRemoved;
        private EventHandler<NotificationEventArgs> onJobTransfered;

        public BitsManager()
        {
            // Set threading apartment
            Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
            NativeMethods.CoInitializeSecurity(IntPtr.Zero, -1, IntPtr.Zero, IntPtr.Zero, RpcAuthnLevel.Connect, RpcImpLevel.Impersonate, IntPtr.Zero, EoAuthnCap.None, IntPtr.Zero);

            BackgroundCopyManager = new BackgroundCopyManager() as IBackgroundCopyManager;
            Jobs = new BitsJobs(this); // will be set correctly later after initialization
            NotificationHandler = new BitsNotification(this);
            NotificationHandler.OnJobErrorEvent += NotificationHandlerOnJobErrorEvent;
            NotificationHandler.OnJobModifiedEvent += NotificationHandlerOnJobModifiedEvent;
            NotificationHandler.OnJobTransferredEvent += NotificationHandlerOnJobTransferredEvent;
        }

        #region event handler for notication interface

        private void NotificationHandlerOnJobTransferredEvent(object sender, NotificationEventArgs e)
        {
            // route the event to the job
            if (Jobs.ContainsKey(e.Job.JobId))
            {
                var job = Jobs[e.Job.JobId];
                job.JobTransferred(sender, e);
            }
            //publish the event to other subscribers
            if (onJobTransfered != null)
                onJobTransfered(sender, e);
        }

        private void NotificationHandlerOnJobModifiedEvent(object sender, NotificationEventArgs e)
        {
            // route the event to the job
            if (Jobs.ContainsKey(e.Job.JobId))
            {
                var job = Jobs[e.Job.JobId];
                job.JobModified(sender, e);
            }
            //publish the event to other subscribers
            if (onJobModified != null)
                onJobModified(sender, e);
        }

        private void NotificationHandlerOnJobErrorEvent(object sender, ErrorNotificationEventArgs e)
        {
            // route the event to the job
            if (Jobs.ContainsKey(e.Job.JobId))
            {
                var job = Jobs[e.Job.JobId];
                job.JobError(sender, e);
            }
            //publish the event to other subscribers
            if (onJobErrored != null)
                onJobErrored(sender, e);
        }

        #endregion

        public BitsJobs Jobs { get; private set; }

        internal IBackgroundCopyManager BackgroundCopyManager { get; private set; }

        #region util methods

        public BitsVersion BitsVersion { get { return Utils.BITSVersion; } }

        #endregion

        #region convert HResult into meaningful error message

        public string GetErrorDescription(int hResult)
        {
            string description;
            BackgroundCopyManager.GetErrorDescription(hResult, Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
            return description;
        }

        #endregion

        #region notification interface

        #region internal notification handling

        internal BitsNotification NotificationHandler { get; private set; }

        internal void NotifyOnJobRemoval(BitsJob job)
        {
            if (null != onJobRemoved)
                onJobRemoved(this, new NotificationEventArgs(job));
        }

        internal void PublishException(BitsJob job, COMException exception)
        {
            if (onInterfaceError == null)
                return;
            var description = GetErrorDescription(exception.ErrorCode);
            onInterfaceError(this, new BitsInterfaceNotificationEventArgs(job, exception, description));
        }

        #endregion

        #region public events

        public event EventHandler<NotificationEventArgs> OnJobModified { add { onJobModified += value; } remove { onJobModified -= value; } }

        public event EventHandler<NotificationEventArgs> OnJobTransferred { add { onJobTransfered += value; } remove { onJobTransfered -= value; } }

        public event EventHandler<ErrorNotificationEventArgs> OnJobError { add { onJobErrored += value; } remove { onJobErrored -= value; } }

        public event EventHandler<NotificationEventArgs> OnJobAdded { add { onJobAdded += value; } remove { onJobAdded -= value; } }

        public event EventHandler<NotificationEventArgs> OnJobRemoved { add { onJobRemoved += value; } remove { onJobRemoved -= value; } }

        public event EventHandler<BitsInterfaceNotificationEventArgs> OnInterfaceError { add { onInterfaceError += value; } remove { onInterfaceError -= value; } }

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public BitsJobs EnumJobs()
        {
            return EnumJobs(JobOwner.CurrentUser);
        }

        public BitsJobs EnumJobs(JobOwner jobOwner)
        {
            if (BackgroundCopyManager == null)
                return null;
            CurrentOwner = jobOwner;
            IEnumBackgroundCopyJobs jobList;
            BackgroundCopyManager.EnumJobs((UInt32) jobOwner, out jobList);
            if (Jobs == null)
                Jobs = new BitsJobs(this, jobList);
            else
                Jobs.Update(jobList);

            return Jobs;
        }

        /// <summary>
        ///   Creates a new transfer job.
        /// </summary>
        /// <param name = "displayName">Null-terminated string that contains a display name for the job. 
        ///   Typically, the display name is used to identify the job in a user interface. 
        ///   Note that more than one job may have the same display name. Must not be NULL. 
        ///   The name is limited to 256 characters, not including the null terminator.</param>
        /// <param name = "jobType"> Type of transfer job, such as JobType.Download. For a list of transfer types, see the JobType enumeration</param>
        /// <returns />
        public BitsJob CreateJob(string displayName, JobType jobType)
        {
            Guid guid;
            IBackgroundCopyJob pJob;
            BackgroundCopyManager.CreateJob(displayName, (BG_JOB_TYPE) jobType, out guid, out pJob);
            BitsJob job;
            lock (Jobs)
            {
                job = new BitsJob(this, pJob);
                Jobs.Add(guid, job);
            }
            if (null != onJobAdded)
                onJobAdded(this, new NotificationEventArgs(job));
            return job;
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (var job in Jobs.Values)
                        job.Dispose();

                    Jobs.Clear();
                    Jobs.Dispose();
                    Marshal.ReleaseComObject(BackgroundCopyManager);
                    BackgroundCopyManager = null;
                }
            }
            disposed = true;
        }
    }
}