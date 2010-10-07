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
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains data about the files to download or upload using BITS
    /// </summary>
    public partial class BitsJob
    {
        #region Constants and Fields

        /// <summary>
        ///   The current job
        /// </summary>
        private readonly IBackgroundCopyJob4 job4;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the peer caching flags.
        /// </summary>
        /// <value>The peer caching flags.</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public PeerCachingFlags CachingFlags
        {
            get
            {
                PeerCachingFlags peerCaching = 0;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetPeerCachingFlags(out peerCaching);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return peerCaching;
            }

            set
            {
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.SetPeerCachingFlags(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Gets or sets the maximum download time.
        /// </summary>
        /// <value>The maximum download time.</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public ulong MaximumDownloadTime
        {
            get
            {
                ulong maxTime = 0;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetMaximumDownloadTime(out maxTime);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return maxTime;
            }

            set
            {
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.SetMaximumDownloadTime(value);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the owner is elevated
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if the owner is elevated; otherwise, <see langword = "false" />.
        /// </value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public bool OwnerElevationState
        {
            get
            {
                var elevated = false;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetOwnerElevationState(out elevated);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return elevated;
            }
        }

        /// <summary>
        ///   Gets the owner integrity level.
        /// </summary>
        /// <value>The owner integrity level.</value>
        /// <exception cref = "NotSupportedException">
        /// </exception>
        public ulong OwnerIntegrityLevel
        {
            get
            {
                ulong integrityLevel = 0;
                try
                {
                    if (this.job4 != null)
                    {
                        // only supported from IBackgroundCopyJob4 and above
                        this.job4.GetOwnerIntegrityLevel(out integrityLevel);
                    }
                    else
                    {
                        throw new NotSupportedException("IBackgroundCopyJob4");
                    }
                }
                catch (COMException exception)
                {
                    this.manager.PublishException(this, exception);
                }

                return integrityLevel;
            }
        }

        #endregion
    }
}