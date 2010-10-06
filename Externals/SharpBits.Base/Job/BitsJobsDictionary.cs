// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************

namespace SharpBits.Base.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// The collection of <see cref="BitsJob"/>'s
    /// </summary>
    public class BitsJobsDictionary : Dictionary<Guid, BitsJob>, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The current BITS manager
        /// </summary>
        private readonly BitsManager manager;

        /// <summary>
        /// Indicates if the job collection as been disposed
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsJobsDictionary"/> class.
        /// </summary>
        public BitsJobsDictionary()
        {
        }

        // only required for initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsJobsDictionary"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        internal BitsJobsDictionary(BitsManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsJobsDictionary"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="jobList">The job list.</param>
        internal BitsJobsDictionary(BitsManager manager, IEnumBackgroundCopyJobs jobList)
        {
            this.manager = manager;
            this.Jobs = jobList;
            this.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsJobsDictionary"/> class.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The context.</param>
        protected BitsJobsDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.disposed = (bool)info.GetValue("disposed", typeof(bool));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <value>The jobs of the current collection</value>
        internal IEnumBackgroundCopyJobs Jobs { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("disposed", this.disposed);
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
        /// Updates the specified job list.
        /// </summary>
        /// <param name="jobList">The job list.</param>
        internal void Update(IEnumBackgroundCopyJobs jobList)
        {
            lock (this)
            {
                // avoid threading issues on list updates
                this.Jobs = jobList;
                this.Update();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // TODO: release COM resource
                    this.Jobs = null;
                    if (this.manager != null)
                    {
                        this.manager.Dispose();
                    }
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Updates the <see cref="BitsJob"/> collection
        /// </summary>
        private void Update()
        {
            uint count;
            IBackgroundCopyJob currentJob;
            BitsJob job;
            var currentList = this.ToDictionary(entry => entry.Key, entry => entry.Value);
            this.Jobs.Reset();
            this.Clear();
            this.Jobs.GetCount(out count);
            for (var i = 0; i < count; i++)
            {
                uint fetchedCount;
                this.Jobs.Next(1, out currentJob, out fetchedCount);
                if (fetchedCount != 1)
                {
                    continue;
                }

                Guid guid;
                currentJob.GetId(out guid);
                if (currentList.ContainsKey(guid))
                {
                    job = currentList[guid];
                    currentList.Remove(guid);
                }
                else
                {
                    job = new BitsJob(this.manager, currentJob);
                }

                this.Add(job.JobId, job);
            }

            foreach (var disposeJob in currentList.Values)
            {
                this.manager.NotifyOnJobRemoval(disposeJob);
                disposeJob.Dispose();
            }
        }

        #endregion
    }
}