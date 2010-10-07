// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************
namespace SharpBits.Base.File
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    using SharpBits.Base.Job;

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