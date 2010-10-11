// ***********************************************************************
// <copyright file="BitsFile.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// ***********************************************************************
namespace SharpBits.Base.File
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    using SharpBits.Base.Job;
    using SharpBits.Base.Progress;

    /// <summary>
    /// A file that can be added to a <see cref="BitsJob"/>
    /// </summary>
    public sealed partial class BitsFile : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The current job
        /// </summary>
        private readonly BitsJob job;

        /// <summary>
        ///   Indicates if the file has been disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        ///   The current <see cref = "BitsFile" />
        /// </summary>
        private IBackgroundCopyFile file;

        /// <summary>
        ///   The current <see cref = "BitsFile" /> progress
        /// </summary>
        private FileProgress progress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitsFile"/> class.
        /// </summary>
        /// <param name="job">
        /// The current job
        /// </param>
        /// <param name="file">
        /// The current file within the job
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal BitsFile(BitsJob job, IBackgroundCopyFile file)
        {
            if (null == file)
            {
                throw new ArgumentNullException(@"file");
            }

            this.file = file;
            this.file2 = file as IBackgroundCopyFile2;
            this.job = job;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the local name of the file
        /// </summary>
        /// <value>The filename of the local file</value>
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
        ///   Gets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public FileProgress Progress
        {
            get
            {
                if (null == this.progress)
                {
                    try
                    {
                        BGFileProgress fileProgress;
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
        ///   Gets or sets the remote name of the file
        /// </summary>
        /// <value>The remote name of the file</value>
        /// <exception cref = "NotSupportedException">
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.
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

    /// <summary>
    /// A file that can be added to a <see cref="BitsJob"/>
    /// </summary>
    public sealed partial class BitsFile
    {
        #region Constants and Fields

        /// <summary>
        ///   The file to download
        /// </summary>
        private readonly IBackgroundCopyFile2 file2;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the file ranges.
        /// </summary>
        /// <value>The file ranges.</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public Collection<FileRange> FileRanges
        {
            get
            {
                try
                {
                    if (this.file2 != null)
                    {
                        uint count;
                        var fileRanges = new Collection<FileRange>();
                        IntPtr rangePtr;
                        this.file2.GetFileRanges(out count, out rangePtr);
                        for (var i = 0; i < count; i++)
                        {
                            var range = (BGFileRange)Marshal.PtrToStructure(rangePtr, typeof(BGFileRange));
                            fileRanges.Add(new FileRange(range));
                            rangePtr = new IntPtr((int)rangePtr + Marshal.SizeOf(range));
                        }

                        return fileRanges;
                    }

                    throw new NotSupportedException("IBackgroundCopyFile2");
                }
                catch (COMException exception)
                {
                    this.job.PublishException(exception);
                    return new Collection<FileRange>();
                }
            }
        }

        #endregion
    }
}