// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************
namespace SharpBits.Base.Progress
{
    using SharpBits.Base.Job;

    /// <summary>
    /// The <see cref="BitsJob"/> Progress
    /// </summary>
    public class JobReplyProgress
    {
        #region Constants and Fields

        /// <summary>
        ///   The current project
        /// </summary>
        private BGJobReplyProgress jobReplyProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobReplyProgress"/> class.
        /// </summary>
        /// <param name="jobReplyProgress">
        /// The job reply progress.
        /// </param>
        internal JobReplyProgress(BGJobReplyProgress jobReplyProgress)
        {
            this.jobReplyProgress = jobReplyProgress;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the total number of bytes downloaded
        /// </summary>
        public ulong BytesTotal
        {
            get
            {
                return this.jobReplyProgress.BytesTotal == ulong.MaxValue ? 0 : this.jobReplyProgress.BytesTotal;
            }
        }

        /// <summary>
        ///   Gets the total number of bytes transferred
        /// </summary>
        /// <value>The bytes transferred.</value>
        public ulong BytesTransferred
        {
            get
            {
                return this.jobReplyProgress.BytesTransferred;
            }
        }

        #endregion
    }
}