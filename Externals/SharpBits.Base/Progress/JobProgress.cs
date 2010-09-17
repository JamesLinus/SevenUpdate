// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.
namespace SharpBits.Base.Progress
{
    public class JobProgress
    {
        private BG_JOB_PROGRESS jobProgress;

        internal JobProgress(BG_JOB_PROGRESS jobProgress)
        {
            this.jobProgress = jobProgress;
        }

        public ulong BytesTotal { get { return jobProgress.BytesTotal == ulong.MaxValue ? 0 : jobProgress.BytesTotal; } }

        public ulong BytesTransferred { get { return jobProgress.BytesTransferred; } }

        public uint FilesTotal { get { return jobProgress.FilesTotal; } }

        public uint FilesTransferred { get { return jobProgress.FilesTransferred; } }
    }
}