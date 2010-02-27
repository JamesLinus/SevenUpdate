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
        private IBackgroundCopyJob3 job3;

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
                    for (int i = 0; i < fileRanges.Count; i++)
                        ranges[i] = fileRanges[i]._BG_FILE_RANGE;
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