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
    using System.Collections.Generic;

    using SharpBits.Base.Job;

    /// <summary>
    /// </summary>
    public class BitsFilesCollection : List<BitsFile>, IDisposable
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
        private IEnumBackgroundCopyFiles fileList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="job">
        /// </param>
        /// <param name="fileList">
        /// </param>
        internal BitsFilesCollection(BitsJob job, IEnumBackgroundCopyFiles fileList)
        {
            this.fileList = fileList;
            this.job = job;
            this.Refresh();
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
        internal void Refresh()
        {
            uint count;
            this.fileList.Reset();
            this.Clear();
            this.fileList.GetCount(out count);
            for (var i = 0; i < count; i++)
            {
                IBackgroundCopyFile currentFile;
                uint fetchedCount;
                this.fileList.Next(1, out currentFile, out fetchedCount);
                if (fetchedCount == 1)
                {
                    this.Add(new BitsFile(this.job, currentFile));
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // TODO: release COM resource
                    this.fileList = null;
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