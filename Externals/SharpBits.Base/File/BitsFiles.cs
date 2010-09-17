// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.Collections.Generic;
using SharpBits.Base.Job;

#endregion

namespace SharpBits.Base.File
{
    public class BitsFiles : List<BitsFile>, IDisposable
    {
        private readonly BitsJob job;
        private bool disposed;
        private IEnumBackgroundCopyFiles fileList;

        internal BitsFiles(BitsJob job, IEnumBackgroundCopyFiles fileList)
        {
            this.fileList = fileList;
            this.job = job;
            Refresh();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal void Refresh()
        {
            uint count;
            fileList.Reset();
            Clear();
            fileList.GetCount(out count);
            for (var i = 0; i < count; i++)
            {
                IBackgroundCopyFile currentFile;
                uint fetchedCount;
                fileList.Next(1, out currentFile, out fetchedCount);
                if (fetchedCount == 1)
                    Add(new BitsFile(job, currentFile));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: release COM resource
                    fileList = null;
                }
            }
            disposed = true;
        }
    }
}