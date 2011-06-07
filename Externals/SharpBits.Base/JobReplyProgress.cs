// ***********************************************************************
// <copyright file="JobReplyProgress.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    ///   The <c>BitsJob</c> Progress.
    /// </summary>
    public class JobReplyProgress
    {
        #region Constants and Fields

        /// <summary>
        ///   The current project.
        /// </summary>
        private BGJobReplyProgress jobReplyProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <c>JobReplyProgress</c> class.
        /// </summary>
        /// <param name="jobReplyProgress">
        ///   The job reply progress.
        /// </param>
        internal JobReplyProgress(BGJobReplyProgress jobReplyProgress)
        {
            this.jobReplyProgress = jobReplyProgress;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the total number of bytes downloaded.
        /// </summary>
        public ulong BytesTotal
        {
            get
            {
                return this.jobReplyProgress.BytesTotal == ulong.MaxValue ? 0 : this.jobReplyProgress.BytesTotal;
            }
        }

        /// <summary>
        ///   Gets the total number of bytes transferred.
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