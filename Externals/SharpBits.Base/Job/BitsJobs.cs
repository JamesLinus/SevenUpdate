#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

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