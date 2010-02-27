namespace SharpBits.Base.Progress
{
    public class JobReplyProgress
    {
        private BG_JOB_REPLY_PROGRESS jobReplyProgress;

        internal JobReplyProgress(BG_JOB_REPLY_PROGRESS jobReplyProgress)
        {
            this.jobReplyProgress = jobReplyProgress;
        }

        public ulong BytesTotal
        {
            get
            {
                if (jobReplyProgress.BytesTotal == ulong.MaxValue)
                    return 0;
                return jobReplyProgress.BytesTotal;
            }
        }

        public ulong BytesTransferred
        {
            get { return jobReplyProgress.BytesTransferred; }
        }
    }
}