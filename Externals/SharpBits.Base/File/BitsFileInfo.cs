namespace SharpBits.Base.File
{
    public class BitsFileInfo
    {
        private BG_FILE_INFO fileInfo;

        internal BitsFileInfo(BG_FILE_INFO fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public BitsFileInfo(string remoteName, string localName)
        {
            fileInfo = new BG_FILE_INFO {RemoteName = remoteName, LocalName = localName};
        }

        public string RemoteName
        {
            get { return fileInfo.RemoteName; }
        }

        public string LocalName
        {
            get { return fileInfo.LocalName; }
        }

        internal BG_FILE_INFO _BG_FILE_INFO
        {
            get { return fileInfo; }
        }
    }
}