// <copyright file="BitsError.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>Bits error.</summary>
    public class BitsError
    {
        /// <summary>The error that occurred.</summary>
        private readonly IBackgroundCopyError error;

        /// <summary>The job the error occurred on.</summary>
        private readonly BitsJob job;

        /// <summary>Initializes a new instance of the <see cref="BitsError" /> class.</summary>
        /// <param name="job">The job the error occurred on.</param>
        /// <param name="error">The error that occurred.</param>
        internal BitsError(BitsJob job, IBackgroundCopyError error)
        {
            if (null == error)
            {
                throw new ArgumentNullException(@"error");
            }

            this.error = error;
            this.job = job;
        }

        /// <summary>Gets the context description.</summary>
        /// <value>The context description.</value>
        public string ContextDescription
        {
            get
            {
                string description = string.Empty;
                try
                {
                    this.error.GetErrorContextDescription(
                        Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return description;
            }
        }

        /// <summary>Gets the description.</summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                string description = string.Empty;
                try
                {
                    this.error.GetErrorDescription(
                        Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return description;
            }
        }

        /// <summary>Gets the error code.</summary>
        /// <value>The error code.</value>
        public int ErrorCode
        {
            get
            {
                int errorCode = 0;
                try
                {
                    BGErrorContext context;
                    this.error.GetError(out context, out errorCode);
                    return errorCode;
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return errorCode;
            }
        }

        /// <summary>Gets the error context.</summary>
        /// <value>The error context.</value>
        public ErrorContext ErrorContext
        {
            get
            {
                try
                {
                    BGErrorContext context;
                    int errorCode;
                    this.error.GetError(out context, out errorCode);
                    return (ErrorContext)context;
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return ErrorContext.UnknownError;
            }
        }

        /// <summary>Gets the file.</summary>
        /// <value>The file that occurred the error.</value>
        public BitsFile File
        {
            get
            {
                try
                {
                    IBackgroundCopyFile errorFile;
                    this.error.GetFile(out errorFile);
                    return new BitsFile(this.job, errorFile);
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return null; // couldn't create new job
            }
        }

        /// <summary>Gets the protocol.</summary>
        /// <value>The protocol.</value>
        public string Protocol
        {
            get
            {
                string protocol = string.Empty;
                try
                {
                    this.error.GetProtocol(out protocol);
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return protocol;
            }
        }
    }
}