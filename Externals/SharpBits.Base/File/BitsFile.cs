//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base.File
{
    using System;
    using System.Runtime.InteropServices;

    using SharpBits.Base.Job;
    using SharpBits.Base.Progress;

    /// <summary>
    /// </summary>
    public sealed partial class BitsFile : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly BitsJob job;

        /// <summary>
        /// </summary>
        private bool disposed;

        /// <summary>
        /// </summary>
        private IBackgroundCopyFile file;

        /// <summary>
        /// </summary>
        private FileProgress progress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="job">
        /// </param>
        /// <param name="file">
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal BitsFile(BitsJob job, IBackgroundCopyFile file)
        {
            if (null == file)
            {
                throw new ArgumentNullException("IBackgroundCopyFile");
            }

            this.file = file;
            this.file2 = file as IBackgroundCopyFile2;
            this.job = job;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string LocalName
        {
            get
            {
                var name = string.Empty;
                try
                {
                    this.file.GetLocalName(out name);
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return name;
            }
        }

        /// <summary>
        /// </summary>
        public FileProgress Progress
        {
            get
            {
                if (null == this.progress)
                {
                    BGFileProgress fileProgress;
                    try
                    {
                        this.file.GetProgress(out fileProgress);
                        this.progress = new FileProgress(fileProgress);
                    }
                    catch (COMException exception)
                    {
                        this.job.PublishException(exception);
                    }
                }

                return this.progress;
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public string RemoteName
        {
            get
            {
                var name = string.Empty;
                try
                {
                    this.file.GetRemoteName(out name);
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }

                return name;
            }

            set
            {
                // supported in IBackgroundCopyFile2
                try
                {
                    if (this.file2 != null)
                    {
                        this.file2.SetRemoteName(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyFile2");
                    }
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                }
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // TODO: release COM resource
                    this.file = null;
                    if (this.job != null)
                    {
                        this.job.Dispose();
                    }
                }
            }

            this.disposed = true;
        }

        #endregion
    }
}