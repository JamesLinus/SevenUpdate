//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base.Progress
{
    /// <summary>
    /// </summary>
    public class JobReplyProgress
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private BGJobReplyProgress jobReplyProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="jobReplyProgress">
        /// </param>
        internal JobReplyProgress(BGJobReplyProgress jobReplyProgress)
        {
            this.jobReplyProgress = jobReplyProgress;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public ulong BytesTotal
        {
            get
            {
                return this.jobReplyProgress.BytesTotal == ulong.MaxValue ? 0 : this.jobReplyProgress.BytesTotal;
            }
        }

        /// <summary>
        /// </summary>
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