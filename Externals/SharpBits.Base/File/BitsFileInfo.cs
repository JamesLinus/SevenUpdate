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

namespace SharpBits.Base.File
{
    public abstract class BitsFileInfo
    {
        internal BitsFileInfo(BG_FILE_INFO fileInfo)
        {
            BgFileInfo = fileInfo;
        }

        public BitsFileInfo(string remoteName, string localName)
        {
            BgFileInfo = new BG_FILE_INFO {RemoteName = remoteName, LocalName = localName};
        }

        public string RemoteName { get { return BgFileInfo.RemoteName; } }

        public string LocalName { get { return BgFileInfo.LocalName; } }

        internal BG_FILE_INFO BgFileInfo { get; private set; }
    }
}