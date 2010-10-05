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
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// </summary>
    public sealed partial class BitsFile
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly IBackgroundCopyFile2 file2;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        /// <exception cref="NotSupportedException">
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