// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#endregion

namespace SharpBits.Base.Job
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class BitsJobs : Dictionary<Guid, BitsJob>, IDisposable
    {
        private readonly BitsManager manager;
        private bool disposed;

        //only required for initialization
        internal BitsJobs(BitsManager manager)
        {
            this.manager = manager;
        }

        internal BitsJobs(BitsManager manager, IEnumBackgroundCopyJobs jobList)
        {
            this.manager = manager;
            Jobs = jobList;
            Update();
        }

        internal IEnumBackgroundCopyJobs Jobs { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal void Update(IEnumBackgroundCopyJobs jobList)
        {
            lock (this) //avoid threading issues on list updates
            {
                Jobs = jobList;
                Update();
            }
        }

        private void Update()
        {
            uint count;
            IBackgroundCopyJob currentJob;
            BitsJob job;
            var currentList = this.ToDictionary(entry => entry.Key, entry => entry.Value);
            Jobs.Reset();
            Clear();
            Jobs.GetCount(out count);
            for (var i = 0; i < count; i++)
            {
                uint fetchedCount;
                Jobs.Next(1, out currentJob, out fetchedCount);
                if (fetchedCount != 1)
                    continue;
                Guid guid;
                currentJob.GetId(out guid);
                if (currentList.ContainsKey(guid))
                {
                    job = currentList[guid];
                    currentList.Remove(guid);
                }
                else
                    job = new BitsJob(manager, currentJob);
                Add(job.JobId, job);
            }

            foreach (var disposeJob in currentList.Values)
            {
                manager.NotifyOnJobRemoval(disposeJob);
                disposeJob.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: release COM resource
                    Jobs = null;
                }
            }
            disposed = true;
        }
    }
}