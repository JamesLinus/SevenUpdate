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
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    using SharpBits.Base.File;

    /// <summary>
    /// </summary>
    public partial class BitsJob : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly IBackgroundCopyJob3 job3;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public FileAclFlags FileAclFlags
        {
            get
            {
                FileAclFlagss flags = 0;
                try
                {
                    if (this.job3 != null)
                    {
                        // only supported from IBackgroundCopyJob3 and above
                        this.job3.GetFileAclFlags(out flags);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob3");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return (FileAclFlags)flags;
            }

            set
            {
                try
                {
                    if (this.job3 != null)
                    {
                        // only supported from IBackgroundCopyJob3 and above
                        this.job3.SetFileAclFlags((FileAclFlagss)value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob3");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="remoteName">
        /// </param>
        /// <param name="localName">
        /// </param>
        /// <param name="fileRanges">
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void AddFileWithRanges(string remoteName, string localName, Collection<FileRange> fileRanges)
        {
            try
            {
                if (this.job3 != null && fileRanges != null)
                {
                    // only supported from IBackgroundCopyJob3 and above
                    var ranges = new BGFileRange[fileRanges.Count];
                    for (var i = 0; i < fileRanges.Count; i++)
                    {
                        ranges[i] = fileRanges[i].BGFileRange;
                    }

                    this.job3.AddFileWithRanges(remoteName, localName, (uint)fileRanges.Count, ranges);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob3");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="oldPrefix">
        /// </param>
        /// <param name="newPrefix">
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void ReplaceRemotePrefix(string oldPrefix, string newPrefix)
        {
            try
            {
                if (this.job3 != null)
                {
                    // only supported from IBackgroundCopyJob3 and above
                    this.job3.ReplaceRemotePrefix(oldPrefix, newPrefix);
                }
                else
                {
                    throw new NotSupportedException("IBackgroundCopyJob3");
                }
            }
            catch (COMException exception)
            {
                this.manager.PublishException(this, exception);
            }
        }

        #endregion
    }
}