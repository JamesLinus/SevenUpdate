// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.Runtime.InteropServices;
using SharpBits.Base.Job;

#endregion

namespace SharpBits.Base
{

    #region delegates

    //replaced with Generic eventhandlers

    #endregion

    #region Notification Event Arguments

    public class JobNotificationEventArgs : EventArgs
    {
    }

    public class JobErrorNotificationEventArgs : JobNotificationEventArgs
    {
        internal JobErrorNotificationEventArgs(BitsError error)
        {
            Error = error;
        }

        public BitsError Error { get; private set; }
    }

    public class NotificationEventArgs : JobNotificationEventArgs
    {
        internal NotificationEventArgs(BitsJob job)
        {
            Job = job;
        }

        public BitsJob Job { get; private set; }
    }

    public class ErrorNotificationEventArgs : NotificationEventArgs
    {
        internal ErrorNotificationEventArgs(BitsJob job, BitsError error) : base(job)
        {
            Error = error;
        }

        public BitsError Error { get; private set; }
    }

    public class BitsInterfaceNotificationEventArgs : NotificationEventArgs
    {
        private readonly COMException exception;

        internal BitsInterfaceNotificationEventArgs(BitsJob job, COMException exception, string description) : base(job)
        {
            Description = description;
            this.exception = exception;
        }

        public string Message { get { return exception.Message; } }

        public string Description { get; private set; }

        public int HResult { get { return exception.ErrorCode; } }
    }

    #endregion

    internal class BitsNotification : IBackgroundCopyCallback
    {
        private readonly BitsManager manager;
        private EventHandler<ErrorNotificationEventArgs> onJobErrored;
        private EventHandler<NotificationEventArgs> onJobModified;
        private EventHandler<NotificationEventArgs> onJobTransfered;

        internal BitsNotification(BitsManager manager)
        {
            this.manager = manager;
        }

        #region IBackgroundCopyCallback Members

        public void JobTransferred(IBackgroundCopyJob pJob)
        {
            if (manager == null)
                return;
            BitsJob job;
            if (null == onJobTransfered)
                return;
            Guid guid;
            pJob.GetId(out guid);
            if (manager.Jobs.ContainsKey(guid))
                job = manager.Jobs[guid];
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return
                manager.EnumJobs(manager.CurrentOwner);
                if (manager.Jobs.ContainsKey(guid))
                    job = manager.Jobs[guid];
                else
                    return;
            }
            onJobTransfered(this, new NotificationEventArgs(job));
            //forward event
            if (job.NotificationTarget != null)
                job.NotificationTarget.JobTransferred(pJob);
        }

        public void JobError(IBackgroundCopyJob pJob, IBackgroundCopyError pError)
        {
            if (manager == null)
                return;
            BitsJob job;
            if (null == onJobErrored)
                return;
            Guid guid;
            pJob.GetId(out guid);
            if (manager.Jobs.ContainsKey(guid))
                job = manager.Jobs[guid];
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return
                manager.EnumJobs(manager.CurrentOwner);
                if (manager.Jobs.ContainsKey(guid))
                    job = manager.Jobs[guid];
                else
                    return;
            }
            onJobErrored(this, new ErrorNotificationEventArgs(job, new BitsError(job, pError)));
            //forward event
            if (job.NotificationTarget != null)
                job.NotificationTarget.JobError(pJob, pError);
        }

        public void JobModification(IBackgroundCopyJob pJob, uint dwReserved)
        {
            if (manager == null)
                return;
            BitsJob job;
            if (null == onJobModified)
                return;
            Guid guid;
            pJob.GetId(out guid);
            if (manager.Jobs.ContainsKey(guid))
                job = manager.Jobs[guid];
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return
                manager.EnumJobs(manager.CurrentOwner);
                if (manager.Jobs.ContainsKey(guid))
                    job = manager.Jobs[guid];
                else
                    return;
            }
            onJobModified(this, new NotificationEventArgs(job));
            //forward event
            if (job.NotificationTarget != null)
                job.NotificationTarget.JobModification(pJob, dwReserved);
        }

        #endregion

        public event EventHandler<NotificationEventArgs> OnJobModifiedEvent { add { onJobModified += value; } remove { onJobModified -= value; } }

        public event EventHandler<NotificationEventArgs> OnJobTransferredEvent { add { onJobTransfered += value; } remove { onJobTransfered -= value; } }

        public event EventHandler<ErrorNotificationEventArgs> OnJobErrorEvent { add { onJobErrored += value; } remove { onJobErrored -= value; } }
    }
}