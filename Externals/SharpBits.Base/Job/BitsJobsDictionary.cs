//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base.Job
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class BitsJobsDictionary : Dictionary<Guid, BitsJob>, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly BitsManager manager;

        /// <summary>
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public BitsJobsDictionary()
        {
        }

        // only required for initialization

        /// <summary>
        /// </summary>
        /// <param name="manager">
        /// </param>
        internal BitsJobsDictionary(BitsManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// </summary>
        /// <param name="manager">
        /// </param>
        /// <param name="jobList">
        /// </param>
        internal BitsJobsDictionary(BitsManager manager, IEnumBackgroundCopyJobs jobList)
        {
            this.manager = manager;
            this.Jobs = jobList;
            this.Update();
        }

        /// <summary>
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
        protected BitsJobsDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.disposed = (bool)info.GetValue("disposed", typeof(bool));
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        internal IEnumBackgroundCopyJobs Jobs { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
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
        /// <param name="jobList">
        /// </param>
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
        /// </summary>
        /// <param name="disposing">
        /// </param>
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