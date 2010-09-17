// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.
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