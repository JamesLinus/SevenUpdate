// ***********************************************************************
// <copyright file="JobProgress.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>The <see cref="BitsJob" /> Progress</summary>
    public class JobProgress
    {
        #region Constants and Fields

        /// <summary>The current progress</summary>
        private BGJobProgress jobProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="JobProgress" /> class.</summary>
        /// <param name="jobProgress">The job progress.</param>
        internal JobProgress(BGJobProgress jobProgress)
        {
            this.jobProgress = jobProgress;
        }

        #endregion

        #region Properties

        /// <summary>Gets the total number bytes downloaded</summary>
        /// <value>The bytes total.</value>
        public ulong BytesTotal
        {
            get
            {
                return this.jobProgress.BytesTotal == ulong.MaxValue ? 0 : this.jobProgress.BytesTotal;
            }
        }

        /// <summary>Gets the total number of bytes transferred.</summary>
        /// <value>The bytes transferred.</value>
        public ulong BytesTransferred
        {
            get
            {
                return this.jobProgress.BytesTransferred;
            }
        }

        /// <summary>Gets the total number of files</summary>
        /// <value>The files total.</value>
        public uint FilesTotal
        {
            get
            {
                return this.jobProgress.FilesTotal;
            }
        }

        /// <summary>Gets the number of files transferred.</summary>
        /// <value>The files transferred.</value>
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