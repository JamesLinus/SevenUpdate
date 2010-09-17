// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.
namespace SharpBits.Base.Progress
{
    public class JobReplyProgress
    {
        private BG_JOB_REPLY_PROGRESS jobReplyProgress;

        internal JobReplyProgress(BG_JOB_REPLY_PROGRESS jobReplyProgress)
        {
            this.jobReplyProgress = jobReplyProgress;
        }

        public ulong BytesTotal { get { return jobReplyProgress.BytesTotal == ulong.MaxValue ? 0 : jobReplyProgress.BytesTotal; } }

        public ulong BytesTransferred { get { return jobReplyProgress.BytesTransferred; } }
    }
}