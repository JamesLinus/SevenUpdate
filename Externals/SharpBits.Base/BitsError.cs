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
using System.Runtime.InteropServices;
using System.Threading;
using SharpBits.Base.File;
using SharpBits.Base.Job;

#endregion

namespace SharpBits.Base
{
    public class BitsError
    {
        private readonly IBackgroundCopyError error;
        private readonly BitsJob job;

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
                var description = string.Empty;
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
                var description = string.Empty;
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
                var protocol = string.Empty;
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