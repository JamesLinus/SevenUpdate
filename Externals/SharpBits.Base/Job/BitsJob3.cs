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
using SharpBits.Base.File;

#endregion

namespace SharpBits.Base.Job
{
    public partial class BitsJob : IDisposable
    {
        private readonly IBackgroundCopyJob3 job3;

        #region public properties

        #region IBackgroundCopyJob3

        public FileACLFlags FileAclFlags
        {
            get
            {
                FILE_ACL_FLAGS flags = 0;
                try
                {
                    if (job3 != null) // only supported from IBackgroundCopyJob3 and above
                        job3.GetFileACLFlags(out flags);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob3");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return (FileACLFlags) flags;
            }
            set
            {
                try
                {
                    if (job3 != null) // only supported from IBackgroundCopyJob3 and above
                        job3.SetFileACLFlags((FILE_ACL_FLAGS) value);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob3");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public void ReplaceRemotePrefix(string oldPrefix, string newPrefix)
        {
            try
            {
                if (job3 != null) // only supported from IBackgroundCopyJob3 and above
                    job3.ReplaceRemotePrefix(oldPrefix, newPrefix);
                else
                    throw new NotSupportedException("IBackgroundCopyJob3");
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        public void AddFileWithRanges(string remoteName, string localName, Collection<FileRange> fileRanges)
        {
            try
            {
                if (job3 != null && fileRanges != null) // only supported from IBackgroundCopyJob3 and above
                {
                    var ranges = new BG_FILE_RANGE[fileRanges.Count];
                    for (var i = 0; i < fileRanges.Count; i++)
                        ranges[i] = fileRanges[i].BgFileRange;
                    job3.AddFileWithRanges(remoteName, localName, (uint) fileRanges.Count, ranges);
                }
                else
                    throw new NotSupportedException("IBackgroundCopyJob3");
            }
            catch (COMException exception)
            {
                manager.PublishException(this, exception);
            }
        }

        #endregion

        #endregion
    }
}