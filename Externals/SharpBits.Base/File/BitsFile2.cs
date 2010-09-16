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