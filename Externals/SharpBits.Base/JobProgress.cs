// <copyright file="JobProgress.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The <c>BitsJob</c> Progress.</summary>
    public class JobProgress
    {
        /// <summary>The current progress.</summary>
        private BGJobProgress jobProgress;

        /// <summary>Initializes a new instance of the <see cref="JobProgress" /> class.</summary>
        /// <param name="jobProgress">The job progress.</param>
        internal JobProgress(BGJobProgress jobProgress)
        {
            this.jobProgress = jobProgress;
        }

        /// <summary>Gets the total number bytes downloaded.</summary>
        /// <value>The bytes total.</value>
        public ulong BytesTotal
        {
            get { return this.jobProgress.BytesTotal == ulong.MaxValue ? 0 : this.jobProgress.BytesTotal; }
        }

        /// <summary>Gets the total number of bytes transferred.</summary>
        /// <value>The bytes transferred.</value>
        public ulong BytesTransferred
        {
            get { return this.jobProgress.BytesTransferred; }
        }

        /// <summary>Gets the total number of files.</summary>
        /// <value>The files total.</value>
        public uint FilesTotal
        {
            get { return this.jobProgress.FilesTotal; }
        }

        /// <summary>Gets the number of files transferred.</summary>
        /// <value>The files transferred.</value>
        public uint FilesTransferred
        {
            get { return this.jobProgress.FilesTransferred; }
        }
    }
}