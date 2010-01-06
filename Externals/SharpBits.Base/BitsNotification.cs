#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
//  
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//  
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

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
        private BitsError error;

        internal JobErrorNotificationEventArgs(BitsError error)
        {
            this.error = error;
        }

        public BitsError Error { get { return error; } }
    }

    public class NotificationEventArgs : JobNotificationEventArgs
    {
        private BitsJob job;

        internal NotificationEventArgs(BitsJob job)
        {
            this.job = job;
        }

        public BitsJob Job { get { return job; } }
    }

    public class ErrorNotificationEventArgs : NotificationEventArgs
    {
        private BitsError error;

        internal ErrorNotificationEventArgs(BitsJob job, BitsError error) : base(job)
        {
            this.error = error;
        }

        public BitsError Error { get { return error; } }
    }

    public class BitsInterfaceNotificationEventArgs : NotificationEventArgs
    {
        private string description;
        private COMException exception;

        internal BitsInterfaceNotificationEventArgs(BitsJob job, COMException exception, string description) : base(job)
        {
            this.description = description;
            this.exception = exception;
        }

        public string Message { get { return exception.Message; } }

        public string Description { get { return description; } }

        public int HResult { get { return exception.ErrorCode; } }
    }

    #endregion

    internal class BitsNotification : IBackgroundCopyCallback
    {
        private BitsManager manager;
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
            if (null != onJobTransfered)
            {
                Guid guid;
                pJob.GetId(out guid);
                if (manager.Jobs.ContainsKey(guid))
                {
                    job = manager.Jobs[guid];
                }
                else
                {
                    // Update Joblist to check whether the job still exists. If not, just return
                    manager.EnumJobs(manager.CurrentOwner);
                    if (manager.Jobs.ContainsKey(guid))
                    {
                        job = manager.Jobs[guid];
                    }
                    else
                        return;
                }
                onJobTransfered(this, new NotificationEventArgs(job));
                //forward event
                if (job.NotificationTarget != null)
                    job.NotificationTarget.JobTransferred(pJob);
            }
        }

        public void JobError(IBackgroundCopyJob pJob, IBackgroundCopyError pError)
        {
            if (manager == null)
                return;
            BitsJob job;
            if (null != onJobErrored)
            {
                Guid guid;
                pJob.GetId(out guid);
                if (manager.Jobs.ContainsKey(guid))
                {
                    job = manager.Jobs[guid];
                }
                else
                {
                    // Update Joblist to check whether the job still exists. If not, just return
                    manager.EnumJobs(manager.CurrentOwner);
                    if (manager.Jobs.ContainsKey(guid))
                    {
                        job = manager.Jobs[guid];
                    }
                    else
                        return;
                }
                onJobErrored(this, new ErrorNotificationEventArgs(job, new BitsError(job, pError)));
                //forward event
                if (job.NotificationTarget != null)
                    job.NotificationTarget.JobError(pJob, pError);
            }
        }

        public void JobModification(IBackgroundCopyJob pJob, uint dwReserved)
        {
            if (manager == null)
                return;
            BitsJob job;
            if (null != onJobModified)
            {
                Guid guid;
                pJob.GetId(out guid);
                if (manager == null)
                    return;
                if (manager.Jobs.ContainsKey(guid))
                {
                    job = manager.Jobs[guid];
                }
                else
                {
                    // Update Joblist to check whether the job still exists. If not, just return
                    manager.EnumJobs(manager.CurrentOwner);
                    if (manager.Jobs.ContainsKey(guid))
                    {
                        job = manager.Jobs[guid];
                    }
                    else
                        return;
                }
                onJobModified(this, new NotificationEventArgs(job));
                //forward event
                if (job.NotificationTarget != null)
                    job.NotificationTarget.JobModification(pJob, dwReserved);

            }
        }

        #endregion

        public event EventHandler<NotificationEventArgs> OnJobModifiedEvent { add { onJobModified += value; } remove { onJobModified -= value; } }

        public event EventHandler<NotificationEventArgs> OnJobTransferredEvent { add { onJobTransfered += value; } remove { onJobTransfered -= value; } }

        public event EventHandler<ErrorNotificationEventArgs> OnJobErrorEvent { add { onJobErrored += value; } remove { onJobErrored -= value; } }
    }
}