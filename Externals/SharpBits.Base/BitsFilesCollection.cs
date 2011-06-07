// ***********************************************************************
// <copyright file="BitsFilesCollection.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    ///   Collection of <c>BitsFile</c>.
    /// </summary>
    public sealed class BitsFilesCollection : Collection<BitsFile>, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The current job in the collection.
        /// </summary>
        private readonly BitsJob job;

        /// <summary>
        ///   Indicates of the files has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        ///   Gets a list of the <c>BitsFile</c>.
        /// </summary>
        private IEnumBackgroundCopyFiles fileList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <c>BitsFilesCollection</c> class.
        /// </summary>
        /// <param name="job">
        ///   The current job.
        /// </param>
        /// <param name="fileList">
        ///   The file list.
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
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
        ///   Refreshes the <c>BitsFile</c> collection.
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
        ///   Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>True</c> to release both managed and unmanaged resources; otherwise, <c>False</c> to release only
        ///   unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
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