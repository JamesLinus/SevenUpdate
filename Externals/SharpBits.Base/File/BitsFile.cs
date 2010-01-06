#region

using System;
using System.Runtime.InteropServices;
using SharpBits.Base.Job;
using SharpBits.Base.Progress;

#endregion

namespace SharpBits.Base.File
{
    public partial class BitsFile : IDisposable
    {
        private bool disposed;
        private IBackgroundCopyFile file;
        private BitsJob job;
        private FileProgress progress;

        internal BitsFile(BitsJob job, IBackgroundCopyFile file)
        {
            if (null == file)
                throw new ArgumentNullException("IBackgroundCopyFile");
            this.file = file;
            file2 = file as IBackgroundCopyFile2;
            this.job = job;
        }

        #region public properties

        public string LocalName
        {
            get
            {
                string name = string.Empty;
                try
                {
                    file.GetLocalName(out name);
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return name;
            }
        }

        public string RemoteName
        {
            get
            {
                string name = string.Empty;
                try
                {
                    file.GetRemoteName(out name);
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return name;
            }
            set //supported in IBackgroundCopyFile2
            {
                try
                {
                    if (file2 != null)
                    {
                        file2.SetRemoteName(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyFile2");
                    }
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
            }
        }

        public FileProgress Progress
        {
            get
            {
                if (null == this.progress)
                {
                    BG_FILE_PROGRESS progress;
                    try
                    {
                        this.file.GetProgress(out progress);
                        this.progress = new FileProgress(progress);
                    }
                    catch (COMException exception)
                    {
                        this.job.PublishException(exception);
                    }
                }
                return this.progress;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: release COM resource
                    file = null;
                }
            }
            disposed = true;
        }
    }
}