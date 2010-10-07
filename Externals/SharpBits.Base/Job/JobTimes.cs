// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************
namespace SharpBits.Base.Job
{
    using System;

    /// <summary>
    /// Data containing various <see cref="DateTime"/>'s about the <see cref="BitsJob"/>
    /// </summary>
    public class JobTimes
    {
        #region Constants and Fields

        /// <summary>
        ///   The current <see cref = "JobTimes" /> for the <see cref = "BitsJob" />
        /// </summary>
        private BGJobTimes jobTimes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobTimes"/> class.
        /// </summary>
        /// <param name="jobTimes">
        /// The job times.
        /// </param>
        internal JobTimes(BGJobTimes jobTimes)
        {
            this.jobTimes = jobTimes;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        public DateTime CreationTime
        {
            get
            {
                return Utilities.FileTimeToDateTime(this.jobTimes.CreationTime);
            }
        }

        /// <summary>
        ///   Gets the modification time.
        /// </summary>
        /// <value>The modification time.</value>
        public DateTime ModificationTime
        {
            get
            {
                return Utilities.FileTimeToDateTime(this.jobTimes.ModificationTime);
            }
        }

        /// <summary>
        ///   Gets the transfer completion time.
        /// </summary>
        /// <value>The transfer completion time.</value>
        public DateTime TransferCompletionTime
        {
            get
            {
                return Utilities.FileTimeToDateTime(this.jobTimes.TransferCompletionTime);
            }
        }

        #endregion
    }
}