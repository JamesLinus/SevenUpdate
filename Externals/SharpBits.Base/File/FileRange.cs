// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.
namespace SharpBits.Base.File
{
    public class FileRange
    {
        internal FileRange(BG_FILE_RANGE fileRange)
        {
            BgFileRange = fileRange;
        }

        public FileRange(ulong initialOffset, ulong length)
        {
            BgFileRange = new BG_FILE_RANGE {InitialOffset = initialOffset, Length = length};
        }

        public ulong InitialOffset { get { return BgFileRange.InitialOffset; } }

        public ulong Length { get { return BgFileRange.Length; } }

        internal BG_FILE_RANGE BgFileRange { get; private set; }
    }
}