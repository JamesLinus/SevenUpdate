// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#endregion

namespace SharpBits.Base.File
{
    public sealed partial class BitsFile
    {
        private readonly IBackgroundCopyFile2 file2;

        #region public properties

        public Collection<FileRange> FileRanges
        {
            get
            {
                try
                {
                    if (file2 != null)
                    {
                        uint count;
                        var fileRanges = new Collection<FileRange>();
                        IntPtr rangePtr;
                        file2.GetFileRanges(out count, out rangePtr);
                        for (var i = 0; i < count; i++)
                        {
                            var range = (BG_FILE_RANGE) Marshal.PtrToStructure(rangePtr, typeof (BG_FILE_RANGE));
                            fileRanges.Add(new FileRange(range));
                            rangePtr = new IntPtr((int) rangePtr + Marshal.SizeOf(range));
                        }
                        return fileRanges;
                    }
                    throw new NotSupportedException("IBackgroundCopyFile2");
                }
                catch (COMException exception)
                {
                    job.PublishException(exception);
                    return new Collection<FileRange>();
                }
            }
        }

        #endregion
    }
}