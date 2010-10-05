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
    using System;

    /// <summary>
    /// </summary>
    public class FileProgress
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private BGFileProgress fileProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="fileProgress">
        /// </param>
        internal FileProgress(BGFileProgress fileProgress)
        {
            this.fileProgress = fileProgress;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public ulong BytesTotal
        {
            get
            {
                return this.fileProgress.BytesTotal == ulong.MaxValue ? 0 : this.fileProgress.BytesTotal;
            }
        }

        /// <summary>
        /// </summary>
        public ulong BytesTransferred
        {
            get
            {
                return this.fileProgress.BytesTransferred;
            }
        }

        /// <summary>
        /// </summary>
        public bool Completed
        {
            get
            {
                return Convert.ToBoolean(this.fileProgress.Completed);
            }
        }

        #endregion
    }
}