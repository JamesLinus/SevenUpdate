// <copyright file="JobReplyProgress.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The <c>BitsJob</c> Progress.</summary>
    public class JobReplyProgress
    {
        /// <summary>The current project.</summary>
        private BGJobReplyProgress jobReplyProgress;

        /// <summary>Initializes a new instance of the <see cref="JobReplyProgress" /> class.</summary>
        /// <param name="jobReplyProgress">The job reply progress.</param>
        internal JobReplyProgress(BGJobReplyProgress jobReplyProgress)
        {
            this.jobReplyProgress = jobReplyProgress;
        }

        /// <summary>Gets the total number of bytes downloaded.</summary>
        public ulong BytesTotal
        {
            get { return this.jobReplyProgress.BytesTotal == ulong.MaxValue ? 0 : this.jobReplyProgress.BytesTotal; }
        }

        /// <summary>Gets the total number of bytes transferred.</summary>
        /// <value>The bytes transferred.</value>
        public ulong BytesTransferred
        {
            get { return this.jobReplyProgress.BytesTransferred; }
        }
    }
}