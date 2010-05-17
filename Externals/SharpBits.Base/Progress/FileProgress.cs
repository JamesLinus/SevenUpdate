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

        public ulong BytesTotal
        {
            get
            {
                if (fileProgress.BytesTotal == ulong.MaxValue)
                    return 0;
                return fileProgress.BytesTotal;
            }
        }

        public ulong BytesTransferred { get { return fileProgress.BytesTransferred; } }

        public bool Completed { get { return Convert.ToBoolean(fileProgress.Completed); } }
    }
}