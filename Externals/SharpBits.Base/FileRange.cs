// <copyright file="FileRange.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The File range.</summary>
    public class FileRange
    {
        /// <summary>Initializes a new instance of the <see cref="FileRange" /> class.</summary>
        /// <param name="initialOffset">The initial offset.</param>
        /// <param name="length">The length.</param>
        public FileRange(ulong initialOffset, ulong length)
        {
            this.BGFileRange = new BGFileRange { InitialOffset = initialOffset, Length = length };
        }

        /// <summary>Initializes a new instance of the <see cref="FileRange" /> class.</summary>
        /// <param name="fileRange">The file range.</param>
        internal FileRange(BGFileRange fileRange)
        {
            this.BGFileRange = fileRange;
        }

        /// <summary>Gets the initial offset.</summary>
        /// <value>The initial offset.</value>
        public ulong InitialOffset
        {
            get { return this.BGFileRange.InitialOffset; }
        }

        /// <summary>Gets the length.</summary>
        /// <value>The length.</value>
        public ulong Length
        {
            get { return this.BGFileRange.Length; }
        }

        /// <summary>Gets the BG file range.</summary>
        /// <value>The BG file range.</value>
        internal BGFileRange BGFileRange { get; private set; }
    }
}