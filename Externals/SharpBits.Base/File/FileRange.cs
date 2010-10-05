//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base.File
{
    /// <summary>
    /// </summary>
    public class FileRange
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="initialOffset">
        /// </param>
        /// <param name="length">
        /// </param>
        public FileRange(ulong initialOffset, ulong length)
        {
            this.BGFileRange = new BGFileRange { InitialOffset = initialOffset, Length = length };
        }

        /// <summary>
        /// </summary>
        /// <param name="fileRange">
        /// </param>
        internal FileRange(BGFileRange fileRange)
        {
            this.BGFileRange = fileRange;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public ulong InitialOffset
        {
            get
            {
                return this.BGFileRange.InitialOffset;
            }
        }

        /// <summary>
        /// </summary>
        public ulong Length
        {
            get
            {
                return this.BGFileRange.Length;
            }
        }

        /// <summary>
        /// </summary>
        internal BGFileRange BGFileRange { get; private set; }

        #endregion
    }
}