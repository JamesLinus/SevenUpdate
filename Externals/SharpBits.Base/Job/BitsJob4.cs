#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace SharpBits.Base.Job
{
    public partial class BitsJob
    {
        private readonly IBackgroundCopyJob4 job4;

        #region public properties

        #region IBackgroundCopyJob4

        public ulong MaximumDownloadTime
        {
            get
            {
                ulong maxTime = 0;
                try
                {
                    if (job4 != null) // only supported from IBackgroundCopyJob4 and above
                        job4.GetMaximumDownloadTime(out maxTime);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob4");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return maxTime;
            }
            set
            {
                try
                {
                    if (job4 != null) // only supported from IBackgroundCopyJob4 and above
                        job4.SetMaximumDownloadTime(value);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob4");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public PeerCachingFlags PeerCachingFlags
        {
            get
            {
                PEER_CACHING_FLAGS peerCaching = 0;
                try
                {
                    if (job4 != null) // only supported from IBackgroundCopyJob4 and above
                        job4.GetPeerCachingFlags(out peerCaching);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob4");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return (PeerCachingFlags) peerCaching;
            }
            set
            {
                try
                {
                    if (job4 != null) // only supported from IBackgroundCopyJob4 and above
                        job4.SetPeerCachingFlags((PEER_CACHING_FLAGS) value);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob4");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
            }
        }

        public ulong OwnerIntegrityLevel
        {
            get
            {
                ulong integrityLevel = 0;
                try
                {
                    if (job4 != null) // only supported from IBackgroundCopyJob4 and above
                        job4.GetOwnerIntegrityLevel(out integrityLevel);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob4");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return integrityLevel;
            }
        }

        public bool OwnerElevationState
        {
            get
            {
                var elevated = false;
                try
                {
                    if (job4 != null) // only supported from IBackgroundCopyJob4 and above
                        job4.GetOwnerElevationState(out elevated);
                    else
                        throw new NotSupportedException("IBackgroundCopyJob4");
                }
                catch (COMException exception)
                {
                    manager.PublishException(this, exception);
                }
                return elevated;
            }
        }

        #endregion

        #endregion
    }
}