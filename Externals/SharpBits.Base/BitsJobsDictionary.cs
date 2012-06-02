// <copyright file="BitsJobsDictionary.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>The collection of <c>BitsJob</c>'s.</summary>
    [Serializable]
    public sealed class BitsJobsDictionary : Dictionary<Guid, BitsJob>, IDisposable
    {
        /// <summary>The current BITS manager.</summary>
        private readonly BitsManager manager;

        /// <summary>Indicates if the job collection as been disposed.</summary>
        private bool disposed;

        /// <summary>Initializes a new instance of the <see cref="BitsJobsDictionary" /> class.</summary>
        public BitsJobsDictionary()
        {
        }

        // only required for initialization

        /// <summary>Initializes a new instance of the <see cref="BitsJobsDictionary" /> class.</summary>
        /// <param name="manager">The manager.</param>
        internal BitsJobsDictionary(BitsManager manager)
        {
            this.manager = manager;
        }

        /// <summary>Initializes a new instance of the <see cref="BitsJobsDictionary" /> class.</summary>
        /// <param name="manager">The manager.</param>
        /// <param name="jobList">The job list.</param>
        internal BitsJobsDictionary(BitsManager manager, IEnumBackgroundCopyJobs jobList)
        {
            this.manager = manager;
            this.Jobs = jobList;
            this.Update();
        }

        /// <summary>Initializes a new instance of the <see cref="BitsJobsDictionary" /> class.</summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        private BitsJobsDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            this.disposed = (bool)info.GetValue("disposed", typeof(bool));
        }

        /// <summary>Gets or sets the jobs.</summary>
        /// <value>The jobs of the current collection.</value>
        private IEnumBackgroundCopyJobs Jobs { get; set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Gets the object data.</summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
            info.AddValue("disposed", this.disposed);
        }

        /// <summary>Updates the specified job list.</summary>
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

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><c>True</c> to release both managed and unmanaged resources; otherwise, <c>False</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
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

        /// <summary>Updates the <c>BitsJob</c> collection.</summary>
        private void Update()
        {
            uint count;
            Dictionary<Guid, BitsJob> currentList = this.ToDictionary(entry => entry.Key, entry => entry.Value);
            this.Jobs.Reset();
            this.Clear();
            this.Jobs.GetCount(out count);
            for (int i = 0; i < count; i++)
            {
                uint fetchedCount;
                IBackgroundCopyJob currentJob;
                this.Jobs.Next(1, out currentJob, out fetchedCount);
                if (fetchedCount != 1)
                {
                    continue;
                }

                Guid guid;
                currentJob.GetId(out guid);
                BitsJob job;
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
    }
}