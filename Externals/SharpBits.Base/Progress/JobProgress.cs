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
    public class JobProgress
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private BGJobProgress jobProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="jobProgress">
        /// </param>
        internal JobProgress(BGJobProgress jobProgress)
        {
            this.jobProgress = jobProgress;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public ulong BytesTotal
        {
            get
            {
                return this.jobProgress.BytesTotal == ulong.MaxValue ? 0 : this.jobProgress.BytesTotal;
            }
        }

        /// <summary>
        /// </summary>
        public ulong BytesTransferred
        {
            get
            {
                return this.jobProgress.BytesTransferred;
            }
        }

        /// <summary>
        /// </summary>
        public uint FilesTotal
        {
            get
            {
                return this.jobProgress.FilesTotal;
            }
        }

        /// <summary>
        /// </summary>
        public uint FilesTransferred
        {
            get
            {
                return this.jobProgress.FilesTransferred;
            }
        }

        #endregion
    }
}