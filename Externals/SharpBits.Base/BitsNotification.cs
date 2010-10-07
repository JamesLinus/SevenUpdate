// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************
namespace SharpBits.Base
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    using SharpBits.Base.Job;

    /// <summary>
    /// The event data for the JobNotification event
    /// </summary>
    public class JobNotificationEventArgs : EventArgs
    {
    }

    /// <summary>
    /// The event data for the Notification event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
    public class NotificationEventArgs : JobNotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="job">
        /// The job the event occurred for
        /// </param>
        internal NotificationEventArgs(BitsJob job)
        {
            this.Job = job;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the job.
        /// </summary>
        /// <value>The <see cref = "BitsJob" /> the notification occurred for</value>
        public BitsJob Job { get; private set; }

        #endregion
    }

    /// <summary>
    /// The event data for the ErrorNotification event
    /// </summary>
    public class ErrorNotificationEventArgs : NotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="job">
        /// The job the notification is for
        /// </param>
        /// <param name="error">
        /// The error that occurred
        /// </param>
        internal ErrorNotificationEventArgs(BitsJob job, BitsError error)
            : base(job)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the error.
        /// </summary>
        /// <value>The error that occurred</value>
        public BitsError Error { get; private set; }

        #endregion
    }

    /// <summary>
    /// The event data for the interface notification event
    /// </summary>
    public class BitsInterfaceNotificationEventArgs : NotificationEventArgs
    {
        #region Constants and Fields

        /// <summary>
        ///   The Com exception
        /// </summary>
        private readonly COMException exception;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsInterfaceNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="job">
        /// The job the notification is for
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
        ///   Gets the description.
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

    /// <summary>
    /// The notification class for the bits manager
    /// </summary>
    internal class BitsNotification : IBackgroundCopyCallback
    {
        #region Constants and Fields

        /// <summary>
        ///   The BITS manager
        /// </summary>
        private readonly BitsManager manager;

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> error occurs
        /// </summary>
        private EventHandler<ErrorNotificationEventArgs> errorOccurred;

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> is modified
        /// </summary>
        private EventHandler<NotificationEventArgs> onJobModified;

        /// <summary>
        ///   Occurs when a <see cref = "BitsJob" /> is transfered
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
                this.errorOccurred += value;
            }

            remove
            {
                this.errorOccurred -= value;
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
        /// <param name="copyJob">
        /// Contains job-related information, such as the number of bytes and files transferred before the error occurred. It also contains the methods to resume and cancel the job. Do not release pJob; BITS releases the interface when the JobError method returns.
        /// </param>
        /// <param name="error">
        /// Contains error information, such as the file being processed at the time the fatal error occurred and a description of the error. Do not release pError; BITS releases the interface when the JobError method returns.
        /// </param>
        public void JobError(IBackgroundCopyJob copyJob, IBackgroundCopyError error)
        {
            if (this.manager == null)
            {
                return;
            }

            BitsJob job;
            if (null == this.errorOccurred)
            {
                return;
            }

            Guid guid;
            copyJob.GetId(out guid);
            if (this.manager.Jobs.ContainsKey(guid))
            {
                job = this.manager.Jobs[guid];
            }
            else
            {
                // Update Job list to check whether the job still exists. If not, just return
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

            this.errorOccurred(this, new ErrorNotificationEventArgs(job, new BitsError(job, error)));

            // forward event
            if (job.NotificationTarget != null)
            {
                job.NotificationTarget.JobError(copyJob, error);
            }
        }

        /// <summary>
        /// Called when a job is modified.
        /// </summary>
        /// <param name="copyJob">
        /// Contains the methods for accessing property, progress, and state information of the job. Do not release pJob; BITS releases the interface when the JobModification method returns.
        /// </param>
        /// <param name="reserved">
        /// Reserved for future use.
        /// </param>
        public void JobModification(IBackgroundCopyJob copyJob, uint reserved)
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
            copyJob.GetId(out guid);
            if (this.manager.Jobs.ContainsKey(guid))
            {
                job = this.manager.Jobs[guid];
            }
            else
            {
                // Update Job list to check whether the job still exists. If not, just return
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
                job.NotificationTarget.JobModification(copyJob, reserved);
            }
        }

        /// <summary>
        /// Called when all of the files in the job have successfully transferred.
        /// </summary>
        /// <param name="copyJob">
        /// Contains job-related information, such as the time the job completed, the number of bytes transferred, and the number of files transferred. Do not release pJob; BITS releases the interface when the method returns.
        /// </param>
        public void JobTransferred(IBackgroundCopyJob copyJob)
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
            copyJob.GetId(out guid);
            if (this.manager.Jobs.ContainsKey(guid))
            {
                job = this.manager.Jobs[guid];
            }
            else
            {
                // Update Job list to check whether the job still exists. If not, just return
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
                job.NotificationTarget.JobTransferred(copyJob);
            }
        }

        #endregion

        #endregion
    }
}