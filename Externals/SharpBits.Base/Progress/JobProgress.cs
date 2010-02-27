namespace SharpBits.Base.Progress
{
    public class JobProgress
    {
        private BG_JOB_PROGRESS jobProgress;

        internal JobProgress(BG_JOB_PROGRESS jobProgress)
        {
            this.jobProgress = jobProgress;
        }

        public ulong BytesTotal
        {
            get
            {
                if (jobProgress.BytesTotal == ulong.MaxValue)
                    return 0;
                return jobProgress.BytesTotal;
            }
        }

        public ulong BytesTransferred
        {
            get { return jobProgress.BytesTransferred; }
        }

        public uint FilesTotal
        {
            get { return jobProgress.FilesTotal; }
        }

        public uint FilesTransferred
        {
            get { return jobProgress.FilesTransferred; }
        }
    }
}