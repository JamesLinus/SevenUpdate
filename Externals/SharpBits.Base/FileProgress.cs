// ***********************************************************************
// <copyright file="FileProgress.cs"
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
    using System;

    /// <summary>The <see cref="BitsFile" /> progress</summary>
    public class FileProgress
    {
        #region Constants and Fields

        /// <summary>The current file progress</summary>
        private BGFileProgress fileProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FileProgress" /> class.</summary>
        /// <param name="fileProgress">The file progress.</param>
        internal FileProgress(BGFileProgress fileProgress)
        {
            this.fileProgress = fileProgress;
        }

        #endregion

        #region Properties

        /// <summary>Gets the total number of bytes downloaded</summary>
        /// <value>The bytes total.</value>
        public ulong BytesTotal
        {
            get
            {
                return this.fileProgress.BytesTotal == ulong.MaxValue ? 0 : this.fileProgress.BytesTotal;
            }
        }

        /// <summary>Gets the total number of bytes transferred.</summary>
        /// <value>The bytes transferred.</value>
        public ulong BytesTransferred
        {
            get
            {
                return this.fileProgress.BytesTransferred;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="FileProgress" /> is completed.</summary>
        /// <value><see langword="true" /> if completed; otherwise, <see langword="false" />.</value>
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