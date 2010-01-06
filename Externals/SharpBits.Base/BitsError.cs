#region

using System;
using System.Runtime.InteropServices;
using System.Threading;
using SharpBits.Base.File;
using SharpBits.Base.Job;

#endregion

namespace SharpBits.Base
{
    public class BitsError
    {
        private IBackgroundCopyError error;
        private BitsJob job;

        internal BitsError(BitsJob job, IBackgroundCopyError error)
        {
            if (null == error)
                throw new ArgumentNullException("IBackgroundCopyError");
            this.error = error;
            this.job = job;
        }

        public string Description
        {
            get
            {
                string description = string.Empty;
                try
                {
                    error.GetErrorDescription(Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return description;
            }
        }

        public string ContextDescription
        {
            get
            {
                string description = string.Empty;
                try
                {
                    error.GetErrorContextDescription(Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return description;
            }
        }

        public string Protocol
        {
            get
            {
                string protocol = string.Empty;
                try
                {
                    error.GetProtocol(out protocol);
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return protocol;
            }
        }

        public BitsFile File
        {
            get
            {
                try
                {
                    IBackgroundCopyFile errorFile;
                    error.GetFile(out errorFile);
                    return new BitsFile(job, errorFile);
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return null; //couldn't create new job
            }
        }

        public ErrorContext ErrorContext
        {
            get
            {
                try
                {
                    BG_ERROR_CONTEXT context;
                    int errorCode;
                    error.GetError(out context, out errorCode);
                    return (ErrorContext) context;
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return ErrorContext.UnknownError;
            }
        }

        public int ErrorCode
        {
            get
            {
                var errorCode = 0;
                try
                {
                    BG_ERROR_CONTEXT context;
                    error.GetError(out context, out errorCode);
                    return errorCode;
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                }
                return errorCode;
            }
        }
    }
}