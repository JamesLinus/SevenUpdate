// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************
namespace SharpBits.Base.Job
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using SharpBits.Base.Progress;

    /// <summary>
    /// Contains data about the files to download or upload using BITS
    /// </summary>
    public partial class BitsJob
    {
        #region Constants and Fields

        /// <summary>
        ///   The current job
        /// </summary>
        private readonly IBackgroundCopyJob2 job2;

        /// <summary>
        ///   The current reply progress
        /// </summary>
        private JobReplyProgress replyProgress;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the notify command line parameters.
        /// </summary>
        /// <value>The notify command line parameters.</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public string NotifyCommandLineParameters
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        string program, parameters;
                        this.job2.GetNotifyCmdLine(out program, out parameters);
                        return parameters;
                    }

                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return string.Empty;
                }
            }

            set
            {
                try
                {
                    if (this.job2 != null)
                    {
                        string program, parameters;
                        this.job2.GetNotifyCmdLine(out program, out parameters);
                        if (value != null)
                        {
                            // the command line program should be passed as first parameter, enclosed by quotes
                            value = string.Format(CultureInfo.CurrentCulture, "\"{0}\" {1}", program, value);
                        }

                        this.job2.SetNotifyCmdLine(program, value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Gets or sets the notify command line program.
        /// </summary>
        /// <value>The notify command line program.</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public string NotifyCommandLineProgram
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        string program, parameters;
                        this.job2.GetNotifyCmdLine(out program, out parameters);
                        return program;
                    }

                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return string.Empty;
                }
            }

            set
            {
                try
                {
                    if (this.job2 != null)
                    {
                        if (null == value)
                        {
                            // removing command line, thus re-enabling the notification interface
                            this.job2.SetNotifyCmdLine(null, null);
                            this.NotificationInterface = this.manager.NotificationHandler;
                        }
                        else
                        {
                            this.NotificationInterface = null;
                            string program, parameters;
                            this.job2.GetNotifyCmdLine(out program, out parameters);
                            this.job2.SetNotifyCmdLine(value, parameters);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Gets or sets the reply file name
        /// </summary>
        /// <value>The reply filename</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public string ReplyFileName
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        string replyFileName;
                        this.job2.GetReplyFileName(out replyFileName);
                        return replyFileName;
                    }

                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return string.Empty;
                }
            }

            set
            {
                try
                {
                    if (this.job2 != null)
                    {
                        // only supported from IBackgroundCopyJob2 and above
                        this.job2.SetReplyFileName(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Gets the reply progress.
        /// </summary>
        /// <value>The reply progress.</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public JobReplyProgress ReplyProgress
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        BGJobReplyProgress replyJobProgress;
                        this.job2.GetReplyProgress(out replyJobProgress);
                        this.replyProgress = new JobReplyProgress(replyJobProgress);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                    return null;
                }

                return this.replyProgress;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the credentials.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void AddCredentials(BitsCredentials credentials)
        {
            try
            {
                if (this.job2 != null && credentials != null)
                {
                    // only supported from IBackgroundCopyJob2 and above
                    var bgCredentials = new BGAuthCredentials
                        {
                           Scheme = (BGAuthScheme)credentials.AuthenticationScheme, Target = (BGAuthTarget)credentials.AuthenticationTarget 
                        };
                    bgCredentials.Credentials.Basic.Password = credentials.Password;
                    bgCredentials.Credentials.Basic.UserName = credentials.UserName;
                    this.job2.SetCredentials(ref bgCredentials);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// Gets the reply data.
        /// </summary>
        /// <returns>
        /// The byte array containing the reply data
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public byte[] GetReplyData()
        {
            try
            {
                if (this.job2 != null)
                {
                    ulong length;
                    var bufferPtr = new IntPtr();
                    this.job2.GetReplyData(bufferPtr, out length);
                    var buffer = new byte[length];
                    Marshal.Copy(bufferPtr, buffer, 0, (int)length); // truncating length to int may be ok as the buffer could be 1MB maximum
                    return buffer;
                }

                throw new NotSupportedException("IBackgroundCopyJob2");
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
                return null;
            }
        }

        /// <summary>
        /// Removes the credentials.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void RemoveCredentials(BitsCredentials credentials)
        {
            try
            {
                if (this.job2 != null && credentials != null)
                {
                    // only supported from IBackgroundCopyJob2 and above
                    this.job2.RemoveCredentials((BGAuthTarget)credentials.AuthenticationTarget, (BGAuthScheme)credentials.AuthenticationScheme);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// Removes the credentials.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="scheme">
        /// The scheme.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void RemoveCredentials(AuthenticationTarget target, AuthenticationScheme scheme)
        {
            try
            {
                if (this.job2 != null)
                {
                    // only supported from IBackgroundCopyJob2 and above
                    this.job2.RemoveCredentials((BGAuthTarget)target, (BGAuthScheme)scheme);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        #endregion
    }
}