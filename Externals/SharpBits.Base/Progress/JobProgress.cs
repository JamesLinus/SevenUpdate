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

namespace SharpBits.Base.Progress
{
    public class JobProgress
    {
        private BG_JOB_PROGRESS jobProgress;

        internal JobProgress(BG_JOB_PROGRESS jobProgress)
        {
            this.jobProgress = jobProgress;
        }

        public ulong BytesTotal { get { return jobProgress.BytesTotal == ulong.MaxValue ? 0 : jobProgress.BytesTotal; } }

        public ulong BytesTransferred { get { return jobProgress.BytesTransferred; } }

        public uint FilesTotal { get { return jobProgress.FilesTotal; } }

        public uint FilesTransferred { get { return jobProgress.FilesTransferred; } }
    }
}