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
using SharpBits.Base.Job;
using SharpBits.Base.Progress;

#endregion

namespace SharpBits.Base.File
{
    public sealed partial class BitsFile : IDisposable
    {
        private readonly BitsJob job;
        private bool disposed;
        private IBackgroundCopyFile file;
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
                var name = string.Empty;
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
                var name = string.Empty;
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