#region

using System;
using System.Runtime.InteropServices;
using SharpBits.Base.Job;
using SharpBits.Base.Progress;

#endregion

namespace SharpBits.Base.File
{
    public sealed partial class BitsFile : IDisposable
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
                        file2.SetRemoteName(value);
                    else
                        throw new NotSupportedException("IBackgroundCopyFile2");
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
                if (null == progress)
                {
                    BG_FILE_PROGRESS fileProgress;
                    try
                    {
                        file.GetProgress(out fileProgress);
                        progress = new FileProgress(fileProgress);
                    }
                    catch (COMException exception)
                    {
                        job.PublishException(exception);
                    }
                }
                return progress;
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

        private void Dispose(bool disposing)
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