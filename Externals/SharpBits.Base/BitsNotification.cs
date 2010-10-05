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

    using SharpBits.Base.Job;

    #region delegates

    // replaced with Generic eventhandlers
    #endregion

    #region Notification Event Arguments

    /// <summary>
    /// </summary>
    public class JobNotificationEventArgs : EventArgs
    {
    }

    /// <summary>
    /// </summary>
    public class NotificationEventArgs : JobNotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="job">
        /// The job.
        /// </param>
        internal NotificationEventArgs(BitsJob job)
        {
            this.Job = job;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public BitsJob Job { get; private set; }

        #endregion
    }

    /// <summary>
    /// </summary>
    public class ErrorNotificationEventArgs : NotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="job">
        /// </param>
        /// <param name="error">
        /// </param>
        internal ErrorNotificationEventArgs(BitsJob job, BitsError error)
            : base(job)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public BitsError Error { get; private set; }

        #endregion
    }

    /// <summary>
    /// </summary>
    public class BitsInterfaceNotificationEventArgs : NotificationEventArgs
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly COMException exception;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsInterfaceNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="job">
        /// The job.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        internal BitsInterfaceNotificationEventArgs(BitsJob job, COMException exception, string description)
            : base(job)
        {
            this.Description = description;
            this.exception = exception;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        ///   Gets the H result.
        /// </summary>
        /// <value>The H result.</value>
        public int HResult
        {
            get
            {
                return this.exception.ErrorCode;
            }
        }

        /// <summary>
        ///   Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get
            {
                return this.exception.Message;
            }
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// </summary>
    internal class BitsNotification : IBackgroundCopyCallback
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly BitsManager manager;

        /// <summary>
        /// </summary>
        private EventHandler<ErrorNotificationEventArgs> onJobErrored;

        /// <summary>
        /// </summary>
        private EventHandler<NotificationEventArgs> onJobModified;

        /// <summary>
        /// </summary>
        private EventHandler<NotificationEventArgs> onJobTransfered;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsNotification"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        internal BitsNotification(BitsManager manager)
        {
            this.manager = manager;
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when [on job error event].
        /// </summary>
        public event EventHandler<ErrorNotificationEventArgs> OnJobErrorEvent
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
        ///   Occurs when [on job modified event].
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobModifiedEvent
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
        ///   Occurs when [on job transferred event].
        /// </summary>
        public event EventHandler<NotificationEventArgs> OnJobTransferredEvent
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

        #region Implemented Interfaces

        #region IBackgroundCopyCallback

        /// <summary>
        /// Called when an error occurs.
        /// </summary>
        /// <param name="pJob">
        /// Contains job-related information, such as the number of bytes and files transferred before the error occurred. It also contains the methods to resume and cancel the job. Do not release pJob; BITS releases the interface when the JobError method returns.
        /// </param>
        /// <param name="pError">
        /// Contains error information, such as the file being processed at the time the fatal error occurred and a description of the error. Do not release pError; BITS releases the interface when the JobError method returns.
        /// </param>
        public void JobError(IBackgroundCopyJob pJob, IBackgroundCopyError pError)
        {
            if (this.manager == null)
            {
                return;
            }

            BitsJob job;
            if (null == this.onJobErrored)
            {
                return;
            }

            Guid guid;
            pJob.GetId(out guid);
            if (this.manager.Jobs.ContainsKey(guid))
            {
                job = this.manager.Jobs[guid];
            }
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return
                this.manager.EnumJobs(this.manager.CurrentOwner);
                if (this.manager.Jobs.ContainsKey(guid))
                {
                    job = this.manager.Jobs[guid];
                }
                else
                {
                    return;
                }
            }

            this.onJobErrored(this, new ErrorNotificationEventArgs(job, new BitsError(job, pError)));

            // forward event
            if (job.NotificationTarget != null)
            {
                job.NotificationTarget.JobError(pJob, pError);
            }
        }

        /// <summary>
        /// Called when a job is modified.
        /// </summary>
        /// <param name="pJob">
        /// Contains the methods for accessing property, progress, and state information of the job. Do not release pJob; BITS releases the interface when the JobModification method returns.
        /// </param>
        /// <param name="dwReserved">
        /// Reserved for future use.
        /// </param>
        public void JobModification(IBackgroundCopyJob pJob, uint dwReserved)
        {
            if (this.manager == null)
            {
                return;
            }

            BitsJob job;
            if (null == this.onJobModified)
            {
                return;
            }

            Guid guid;
            pJob.GetId(out guid);
            if (this.manager.Jobs.ContainsKey(guid))
            {
                job = this.manager.Jobs[guid];
            }
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return
                this.manager.EnumJobs(this.manager.CurrentOwner);
                if (this.manager.Jobs.ContainsKey(guid))
                {
                    job = this.manager.Jobs[guid];
                }
                else
                {
                    return;
                }
            }

            this.onJobModified(this, new NotificationEventArgs(job));

            // forward event
            if (job.NotificationTarget != null)
            {
                job.NotificationTarget.JobModification(pJob, dwReserved);
            }
        }

        /// <summary>
        /// Called when all of the files in the job have successfully transferred.
        /// </summary>
        /// <param name="pJob">
        /// Contains job-related information, such as the time the job completed, the number of bytes transferred, and the number of files transferred. Do not release pJob; BITS releases the interface when the method returns.
        /// </param>
        public void JobTransferred(IBackgroundCopyJob pJob)
        {
            if (this.manager == null)
            {
                return;
            }

            BitsJob job;
            if (null == this.onJobTransfered)
            {
                return;
            }

            Guid guid;
            pJob.GetId(out guid);
            if (this.manager.Jobs.ContainsKey(guid))
            {
                job = this.manager.Jobs[guid];
            }
            else
            {
                // Update Joblist to check whether the job still exists. If not, just return
                this.manager.EnumJobs(this.manager.CurrentOwner);
                if (this.manager.Jobs.ContainsKey(guid))
                {
                    job = this.manager.Jobs[guid];
                }
                else
                {
                    return;
                }
            }

            this.onJobTransfered(this, new NotificationEventArgs(job));

            // forward event
            if (job.NotificationTarget != null)
            {
                job.NotificationTarget.JobTransferred(pJob);
            }
        }

        #endregion

        #endregion
    }
}