// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************

namespace SharpBits.Base.Job
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    using SharpBits.Base.File;

    /// <summary>
    /// Contains data about the files to download or upload using BITS
    /// </summary>
    public partial class BitsJob : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The current job
        /// </summary>
        private readonly IBackgroundCopyJob3 job3;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the file acl flags.
        /// </summary>
        /// <value>The file acl flags.</value>
        /// <exception cref="NotSupportedException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public FileAclFlags FileAclFlags
        {
            get
            {
                BGFileAclFlags flags = 0;
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
                        this.job3.SetFileAclFlags((BGFileAclFlags)value);
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
        /// Adds the file with ranges.
        /// </summary>
        /// <param name="remoteName">Name of the remote.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="fileRanges">The file ranges.</param>
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
        /// Replaces the remote prefix.
        /// </summary>
        /// <param name="oldPrefix">The old prefix.</param>
        /// <param name="newPrefix">The new prefix.</param>
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