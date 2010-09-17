// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.

#region

using System;

#endregion

namespace SharpBits.Base.Progress
{
    public class FileProgress
    {
        private BG_FILE_PROGRESS fileProgress;

        internal FileProgress(BG_FILE_PROGRESS fileProgress)
        {
            this.fileProgress = fileProgress;
        }

        public ulong BytesTotal { get { return fileProgress.BytesTotal == ulong.MaxValue ? 0 : fileProgress.BytesTotal; } }

        public ulong BytesTransferred { get { return fileProgress.BytesTransferred; } }

        public bool Completed { get { return Convert.ToBoolean(fileProgress.Completed); } }
    }
}