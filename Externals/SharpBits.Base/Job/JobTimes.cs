//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base.Job
{
    using System;

    /// <summary>
    /// </summary>
    public class JobTimes
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private BGJobTimes jobTimes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="jobTimes">
        /// </param>
        internal JobTimes(BGJobTimes jobTimes)
        {
            this.jobTimes = jobTimes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                return Utils.FileTime2DateTime(this.jobTimes.CreationTime);
            }
        }

        /// <summary>
        /// </summary>
        public DateTime ModificationTime
        {
            get
            {
                return Utils.FileTime2DateTime(this.jobTimes.ModificationTime);
            }
        }

        /// <summary>
        /// </summary>
        public DateTime TransferCompletionTime
        {
            get
            {
                return Utils.FileTime2DateTime(this.jobTimes.TransferCompletionTime);
            }
        }

        #endregion
    }
}