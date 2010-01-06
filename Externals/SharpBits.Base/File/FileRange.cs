namespace SharpBits.Base.File
{
    public class FileRange
    {
        private BG_FILE_RANGE fileRange;

        internal FileRange(BG_FILE_RANGE fileRange)
        {
            this.fileRange = fileRange;
        }

        public FileRange(ulong initialOffset, ulong length)
        {
            fileRange = new BG_FILE_RANGE {InitialOffset = initialOffset, Length = length};
        }

        public ulong InitialOffset { get { return fileRange.InitialOffset; } }

        public ulong Length { get { return fileRange.Length; } }

        internal BG_FILE_RANGE _BG_FILE_RANGE { get { return fileRange; } }
    }
}