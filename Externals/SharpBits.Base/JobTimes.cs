// <copyright file="JobTimes.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System;

    /// <summary>Data containing various <c>DateTime</c>'s about the <c>BitsJob</c>.</summary>
    public class JobTimes
    {
        /// <summary>The current <c>JobTimes</c> for the <c>BitsJob</c>.</summary>
        BGJobTimes jobTimes;

        /// <summary>Initializes a new instance of the <see cref="JobTimes" /> class.</summary>
        /// <param name="jobTimes">The job times.</param>
        internal JobTimes(BGJobTimes jobTimes)
        {
            this.jobTimes = jobTimes;
        }

        /// <summary>Gets the creation time.</summary>
        /// <value>The creation time.</value>
        public DateTime CreationTime
        {
            get { return Utilities.FileTimeToDateTime(this.jobTimes.CreationTime); }
        }

        /// <summary>Gets the modification time.</summary>
        /// <value>The modification time.</value>
        public DateTime ModificationTime
        {
            get { return Utilities.FileTimeToDateTime(this.jobTimes.ModificationTime); }
        }

        /// <summary>Gets the transfer completion time.</summary>
        /// <value>The transfer completion time.</value>
        public DateTime TransferCompletionTime
        {
            get { return Utilities.FileTimeToDateTime(this.jobTimes.TransferCompletionTime); }
        }
    }
}