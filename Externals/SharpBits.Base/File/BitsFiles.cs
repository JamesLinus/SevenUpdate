#region

using System;
using System.Collections.Generic;
using SharpBits.Base.Job;

#endregion

namespace SharpBits.Base.File
{
    public class BitsFiles : List<BitsFile>, IDisposable
    {
        private bool disposed;
        private IEnumBackgroundCopyFiles fileList;
        private BitsJob job;

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
            for (int i = 0; i < count; i++)
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