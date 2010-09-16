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
using System.Collections.Generic;
using SharpBits.Base.Job;

#endregion

namespace SharpBits.Base.File
{
    public class BitsFiles : List<BitsFile>, IDisposable
    {
        private readonly BitsJob job;
        private bool disposed;
        private IEnumBackgroundCopyFiles fileList;

        internal BitsFiles(BitsJob job, IEnumBackgroundCopyFiles fileList)
        {
            this.fileList = fileList;
            this.job = job;
            Refresh();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal void Refresh()
        {
            uint count;
            fileList.Reset();
            Clear();
            fileList.GetCount(out count);
            for (var i = 0; i < count; i++)
            {
                IBackgroundCopyFile currentFile;
                uint fetchedCount;
                fileList.Next(1, out currentFile, out fetchedCount);
                if (fetchedCount == 1)
                    Add(new BitsFile(job, currentFile));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: release COM resource
                    fileList = null;
                }
            }
            disposed = true;
        }
    }
}