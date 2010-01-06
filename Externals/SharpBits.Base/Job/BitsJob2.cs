#region

using System;
using System.Runtime.InteropServices;
using SharpBits.Base.Progress;

#endregion

namespace SharpBits.Base.Job
{
    public partial class BitsJob : IDisposable
    {
        private IBackgroundCopyJob2 job2;
        private JobReplyProgress replyProgress;

        #region public properties

        #region IBackgroundCopyJob2

        public string NotifyCommandLineProgram
        {
            set
            {
                try
                {
                    if (job2 != null)
                    {
                        if (null == value) //removing command line, thus re-enabling the notification interface
                        {
                            job2.SetNotifyCmdLine(null, null);
                            NotificationInterface = manager.NotificationHandler;
                        }
                        else
                        {
                            NotificationInterface = null;
                            string program, parameters;
                            job2.GetNotifyCmdLine(out program, out parameters);
                            job2.SetNotifyCmdLine(value, parameters);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
            get
            {
                try
                {
                    if (job2 != null)
                    {
                        string program, parameters;
                        job2.GetNotifyCmdLine(out program, out parameters);
                        return program;
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
        }

        public string NotifyCommandLineParameters
        {
            set
            {
                try
                {
                    if (job2 != null)
                    {
                        string program, parameters;
                        job2.GetNotifyCmdLine(out program, out parameters);
                        if (value != null) //the command line program should be passed as first parameter, enclosed by quotes
                        {
                            value = string.Format("\"{0}\" {1}", program, value);
                        }
                        job2.SetNotifyCmdLine(program, value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
            get
            {
                try
                {
                    if (job2 != null)
                    {
                        string program, parameters;
                        job2.GetNotifyCmdLine(out program, out parameters);
                        return parameters;
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
        }

        public string ReplyFileName
        {
            get
            {
                try
                {
                    if (job2 != null)
                    {
                        string replyFileName;
                        job2.GetReplyFileName(out replyFileName);
                        return replyFileName;
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    if (job2 != null) // only supported from IBackgroundCopyJob2 and above
                    {
                        job2.SetReplyFileName(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public Byte[] ReplyData
        {
            get
            {
                try
                {
                    if (job2 != null)
                    {
                        ulong length;
                        var bufferPtr = new IntPtr();
                        job2.GetReplyData(bufferPtr, out length);
                        var buffer = new Byte[length];
                        Marshal.Copy(bufferPtr, buffer, 0, (int) length); //truncating length to int may be ok as the buffer could be 1MB maximum
                        return buffer;
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return null;
                }
            }
        }

        public JobReplyProgress ReplyProgress
        {
            get
            {
                try
                {
                    if (this.job2 != null)
                    {
                        BG_JOB_REPLY_PROGRESS replyProgress;
                        this.job2.GetReplyProgress(out replyProgress);
                        this.replyProgress = new JobReplyProgress(replyProgress);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob2");
                    }
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                    return null;
                }
                return this.replyProgress;
            }
        }

        public void AddCredentials(BitsCredentials credentials)
        {
            try
            {
                if (job2 != null && credentials != null) // only supported from IBackgroundCopyJob2 and above
                {
                    var bgCredentials = new BG_AUTH_CREDENTIALS();
                    bgCredentials.Scheme = (BG_AUTH_SCHEME) credentials.AuthenticationScheme;
                    bgCredentials.Target = (BG_AUTH_TARGET) credentials.AuthenticationTarget;
                    bgCredentials.Credentials.Basic.Password = credentials.Password.ToString();
                    bgCredentials.Credentials.Basic.UserName = credentials.UserName.ToString();
                    job2.SetCredentials(ref bgCredentials);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void RemoveCredentials(BitsCredentials credentials)
        {
            try
            {
                if (job2 != null && credentials != null) // only supported from IBackgroundCopyJob2 and above
                {
                    job2.RemoveCredentials((BG_AUTH_TARGET) credentials.AuthenticationTarget, (BG_AUTH_SCHEME) credentials.AuthenticationScheme);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void RemoveCredentials(AuthenticationTarget target, AuthenticationScheme scheme)
        {
            try
            {
                if (job2 != null) // only supported from IBackgroundCopyJob2 and above
                {
                    job2.RemoveCredentials((BG_AUTH_TARGET) target, (BG_AUTH_SCHEME) scheme);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob2");
                }
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        #endregion

        #endregion
    }
}