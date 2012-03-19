// <copyright file="BitsFile.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    /// <summary>A file that can be added to a <c>BitsJob</c>.</summary>
    public sealed partial class BitsFile : IDisposable
    {
        /// <summary>The current job.</summary>
        private readonly BitsJob job;

        /// <summary>Indicates if the file has been disposed.</summary>
        private bool disposed;

        /// <summary>The current <c>BitsFile</c>.</summary>
        private IBackgroundCopyFile file;

        /// <summary>The current <c>BitsFile</c> progress.</summary>
        private FileProgress progress;

        /// <summary>Initializes a new instance of the <see cref="BitsFile" /> class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are <c>null</c>.</exception>
        /// <param name="job">The current job.</param>
        /// <param name="file">The current file within the job.</param>
        internal BitsFile(BitsJob job, IBackgroundCopyFile file)
        {
            if (null == file)
            {
                throw new ArgumentNullException(@"file", @"Parameter IBackgroundCopyFile cannot be a null reference");
            }

            this.file = file;
            this.file2 = file as IBackgroundCopyFile2;
            this.job = job;
        }

        /// <summary>Gets the local name of the file.</summary>
        /// <value>The filename of the local file.</value>
        public string LocalName
        {
            get
            {
                string name = string.Empty;
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

        /// <summary>Gets the progress.</summary>
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

        /// <summary>Gets or sets the remote name of the file.</summary>
        /// <value>The remote name of the file.</value>
        public string RemoteName
        {
            get
            {
                string name = string.Empty;
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

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><c>True</c> to release both managed and unmanaged resources; otherwise, <c>False</c> to release only unmanaged resources.</param>
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
    }

    /// <summary>A file that can be added to a <c>BitsJob</c>.</summary>
    public sealed partial class BitsFile
    {
        /// <summary>The file to download.</summary>
        private readonly IBackgroundCopyFile2 file2;

        /// <summary>Gets the file ranges.</summary>
        /// <value>The file ranges.</value>
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
                        for (int i = 0; i < count; i++)
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
    }
}