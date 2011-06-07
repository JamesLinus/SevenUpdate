//-----------------------------------------------------------------------
// <copyright file="DeflateManager.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    using System;
    using System.Globalization;

    /// <summary>
    /// </summary>
    internal sealed class DeflateManager
    {
        #region Constants and Fields

        internal readonly short[] BlCount = new short[InternalConstants.MaxBits + 1];

        internal readonly sbyte[] Depth = new sbyte[2 * InternalConstants.LCodes + 1];

        internal readonly int[] Heap = new int[2 * InternalConstants.LCodes + 1];

        internal int HeapLen; // number of elements in the heap

        internal int HeapMax; // element of largest frequency

        internal int NextPending; // index of next pending byte to output to the stream

        // Buffer for distances. To simplify the code, d_buf and l_buf have the same number of elements. To use
        // different lengths, an extra flag array would be necessary.

        internal int OptLen; // bit length of current block with optimal trees

        internal byte[] Pending; // output still pending - waiting to be compressed

        internal int PendingCount; // number of bytes in the pending buffer

        internal int StaticLen; // bit length of current block with static trees

        private const int BufSize = 8 * 2;

        private const int BusyState = 113;

        private const int DynTrees = 2;

        private const int EndBlock = 256;

        private const int FinishState = 666;

        private const int HeapSize = 2 * InternalConstants.LCodes + 1;

        private const int InitState = 42;

        private const int MaxMatch = 258;

        private const int MemLevelDefault = 8;

        private const int MemLevelMax = 9;

        private const int MinLookahead = MaxMatch + MinMatch + 1;

        private const int MinMatch = 3;

        private const int PresetDict = 0x20;

        private const int StaticTrees = 1;

        private const int ZAscii = 1;

        private const int ZDeflated = 8;

        private const int ZUnknown = 2;

        private static readonly string[] ErrorMessage = new[]
            {
                "need dictionary", "stream end", string.Empty, "file error", "stream error", "data error",
                "insufficient memory", "buffer error", "incompatible version", string.Empty
            };

        private static readonly int StoredBlock;

        // The three kinds of block type

        private static readonly int ZBinary;

        private readonly short[] blTree; // Huffman tree for bit lengths

        private readonly short[] dynDtree; // distance tree

        private readonly short[] dynLtree; // literal and length tree

        private readonly Tree treeBitLengths = new Tree(); // desc for bit length tree

        private readonly Tree treeDistances = new Tree(); // desc for distance tree

        private readonly Tree treeLiterals = new Tree(); // desc for literal tree

        private short biBuf;

        // Number of valid bits in bi_buf.  All bits above the last valid bit
        // are always zero.

        private int biValid;

        // Window position at the beginning of the current output block. Gets negative when the window is moved
        // backwards.

        private int blockStart;

        private ZlibCodec codec; // the zlib encoder/decoder

        // Insert new strings in the hash table only if the match length is not greater than this length. This saves
        // time but degrades compression. max_insert_length is used only for compression levels <= 3.

        private CompressionLevel compressionLevel; // compression level (1..9)

        private CompressionStrategy compressionStrategy; // favor or force Huffman coding

        private Config config;

        private sbyte dataType; // UNKNOWN, BINARY or ASCII

        private CompressFunc deflateFunction;

        private int distanceOffset; // index into pending; points to distance data??

        private int hashBits; // log2(hash_size)

        private int hashMask; // hash_size-1

        // Number of bits by which ins_h must be shifted at each input step. It must be such that after MIN_MATCH steps,
        // the oldest byte no longer takes part in the hash key, that is: hash_shift * MIN_MATCH >= hash_bits

        private int hashShift;

        private int hashSize; // number of elements in hash table

        private short[] head; // Heads of the hash chains or NIL.

        // heap used to build the Huffman trees

        private int insH; // hash index of string to be inserted

        private int lastEobLen; // bit length of EOB code for last block

        private int lastFlush; // value of flush param for previous deflate call

        // The sons of heap[n] are heap[2*n] and heap[2*n+1]. heap[0] is not used. The same heap array is used to build
        // all trees.

        // Depth of each subtree used as tie breaker for trees of equal frequency

        private int lastLit; // running index in l_buf

        private int lengthOffset; // index for literals or lengths 

        private int litBufsize;

        private int lookAhead; // number of valid bytes ahead in window

        private int matchAvailable; // set if previous match exists

        private int matchLength; // length of best match

        private int matchStart; // start of matching string

        private int matches; // number of string matches in current block

        private short[] prev;

        private int prevLength;

        private int prevMatch; // previous match

        private bool rfc1950BytesEmitted;

        private int status; // as the name implies

        private int strStart; // start of string to insert into.....????

        private int winBits; // log2(w_size)  (8..16)

        private int winMask; // w_size - 1

        private int winSize; // LZ77 window size (32K by default)

        private bool wantRfc1950HeaderBytes = true;

        private byte[] window;

        // Sliding window. Input bytes are read into the second half of the window, and move to the first half later to
        // keep a dictionary of at least wSize bytes. With this organization, matches are limited to a distance of
        // wSize-MAX_MATCH bytes, but this ensures that IO is always performed with a length multiple of the block size.
        // To do: use the user input buffer as sliding window.

        private int windowSize;

        #endregion

        #region Constructors and Destructors

        internal DeflateManager()
        {
            this.dynLtree = new short[HeapSize * 2];
            this.dynDtree = new short[(2 * InternalConstants.DCodes + 1) * 2]; // distance tree
            this.blTree = new short[(2 * InternalConstants.BlCodes + 1) * 2]; // Huffman tree for bit lengths
        }

        #endregion

        #region Delegates

        /// <param name="flush">
        /// </param>
        private delegate BlockState CompressFunc(FlushType flush);

        #endregion

        #region Properties

        internal bool WantRfc1950HeaderBytes
        {
            get
            {
                return this.wantRfc1950HeaderBytes;
            }

            set
            {
                this.wantRfc1950HeaderBytes = value;
            }
        }

        #endregion

        #region Methods

        /// <param name="flush">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        internal int Deflate(FlushType flush)
        {
            if (this.codec.OutputBuffer == null || (this.codec.InputBuffer == null && this.codec.AvailableBytesIn != 0) ||
                (this.status == FinishState && flush != FlushType.Finish))
            {
                this.codec.Message = ErrorMessage[ZlibConstants.ZNeedDict - ZlibConstants.ZStreamError];
                throw new ZlibException(
                    string.Format(CultureInfo.InvariantCulture, "Something is fishy. [{0}]", this.codec.Message));

                // return ZlibConstants.Z_STREAM_ERROR;
            }

            if (this.codec.AvailableBytesOut == 0)
            {
                this.codec.Message = ErrorMessage[ZlibConstants.ZNeedDict - ZlibConstants.ZBufError];
                throw new ZlibException("OutputBuffer is full (AvailableBytesOut == 0)");

                // return ZlibConstants.Z_BUF_ERROR;
            }

            var oldFlush = this.lastFlush;
            this.lastFlush = (int)flush;

            // Write the zlib (rfc1950) header bytes
            if (this.status == InitState)
            {
                var header = (ZDeflated + ((this.winBits - 8) << 4)) << 8;
                var levelFlags = (((int)this.compressionLevel - 1) & 0xff) >> 1;

                if (levelFlags > 3)
                {
                    levelFlags = 3;
                }

                header |= levelFlags << 6;
                if (this.strStart != 0)
                {
                    header |= PresetDict;
                }

                header += 31 - (header % 31);

                this.status = BusyState;

                // putShortMSB(header);
                unchecked
                {
                    this.Pending[this.PendingCount++] = (byte)(header >> 8);
                    this.Pending[this.PendingCount++] = (byte)header;
                }

                // Save the adler32 of the preset dictionary:
                if (this.strStart != 0)
                {
                    // putShortMSB((int)(SharedUtils.URShift(_codec._Adler32, 16)));
                    // putShortMSB((int)((UInt64)_codec._Adler32 >> 16)); putShortMSB((int)(_codec._Adler32 & 0xffff));
                    this.Pending[this.PendingCount++] = (byte)((this.codec.Adler32 & 0xFF000000) >> 24);
                    this.Pending[this.PendingCount++] = (byte)((this.codec.Adler32 & 0x00FF0000) >> 16);
                    this.Pending[this.PendingCount++] = (byte)((this.codec.Adler32 & 0x0000FF00) >> 8);
                    this.Pending[this.PendingCount++] = (byte)(this.codec.Adler32 & 0x000000FF);
                }

                this.codec.Adler32 = Adler.Adler32(0, null, 0, 0);
            }

            // Flush as much pending output as possible
            if (this.PendingCount != 0)
            {
                this.codec.FlushPending();
                if (this.codec.AvailableBytesOut == 0)
                {
                    // System.out.println("  avail_out==0");
                    // Since avail_out is 0, deflate will be called again with more output space, but possibly with both
                    // pending and avail_in equal to zero. There won't be anything to do, but this is not an error
                    // situation so make sure we return OK instead of BUF_ERROR at next call of deflate:
                    this.lastFlush = -1;
                    return ZlibConstants.Zok;
                }

                // Make sure there is something to do and avoid duplicate consecutive flushes. For repeated and useless
                // calls with Z_FINISH, we keep returning Z_STREAM_END instead of Z_BUFF_ERROR.
            }
            else if (this.codec.AvailableBytesIn == 0 && (int)flush <= oldFlush && flush != FlushType.Finish)
            {
                // workitem 8557 Not sure why this needs to be an error. pendingCount == 0, which means there's nothing
                // to deflate. And the caller has not asked for a FlushType.Finish, but...
                // that seems very non-fatal.  We can just say "OK" and do nthing.

                // _codec.Message = z_errmsg[ZlibConstants.Z_NEED_DICT - (ZlibConstants.Z_BUF_ERROR)]; throw new
                // ZlibException("AvailableBytesIn == 0 && flush<=old_flush && flush != FlushType.Finish");
                return ZlibConstants.Zok;
            }

            // User must not provide more input after the first FINISH:
            if (this.status == FinishState && this.codec.AvailableBytesIn != 0)
            {
                this.codec.Message = ErrorMessage[ZlibConstants.ZNeedDict - ZlibConstants.ZBufError];
                throw new ZlibException("status == FINISH_STATE && _codec.AvailableBytesIn != 0");
            }

            // Start a new block or continue the current one.
            if (this.codec.AvailableBytesIn != 0 || this.lookAhead != 0 ||
                (flush != FlushType.None && this.status != FinishState))
            {
                var bstate = this.deflateFunction(flush);

                if (bstate == BlockState.FinishStarted || bstate == BlockState.FinishDone)
                {
                    this.status = FinishState;
                }

                if (bstate == BlockState.NeedMore || bstate == BlockState.FinishStarted)
                {
                    if (this.codec.AvailableBytesOut == 0)
                    {
                        this.lastFlush = -1; // avoid BUF_ERROR next call, see above
                    }

                    return ZlibConstants.Zok;

                    // If flush != Z_NO_FLUSH && avail_out == 0, the next call of deflate should use the same flush
                    // parameter to make sure that the flush is complete. So we don't have to output an empty block
                    // here, this will be done at next call. This also ensures that for a very small output buffer, we
                    // emit at most one empty block.
                }

                if (bstate == BlockState.BlockDone)
                {
                    if (flush == FlushType.Partial)
                    {
                        this.TrAlign();
                    }
                    else
                    {
                        // FlushType.Full or FlushType.Sync
                        this.TrStoredBlock(0, 0, false);

                        // For a full flush, this empty block will be recognized as a special marker by inflate_sync().
                        if (flush == FlushType.Full)
                        {
                            // clear hash (forget the history)
                            for (var i = 0; i < this.hashSize; i++)
                            {
                                this.head[i] = 0;
                            }
                        }
                    }

                    this.codec.FlushPending();
                    if (this.codec.AvailableBytesOut == 0)
                    {
                        this.lastFlush = -1; // avoid BUF_ERROR at next call, see above
                        return ZlibConstants.Zok;
                    }
                }
            }

            if (flush != FlushType.Finish)
            {
                return ZlibConstants.Zok;
            }

            if (!this.WantRfc1950HeaderBytes || this.rfc1950BytesEmitted)
            {
                return ZlibConstants.ZStreamEnd;
            }

            // Write the zlib trailer (adler32)
            this.Pending[this.PendingCount++] = (byte)((this.codec.Adler32 & 0xFF000000) >> 24);
            this.Pending[this.PendingCount++] = (byte)((this.codec.Adler32 & 0x00FF0000) >> 16);
            this.Pending[this.PendingCount++] = (byte)((this.codec.Adler32 & 0x0000FF00) >> 8);
            this.Pending[this.PendingCount++] = (byte)(this.codec.Adler32 & 0x000000FF);

            // putShortMSB((int)(SharedUtils.URShift(_codec._Adler32, 16))); putShortMSB((int)(_codec._Adler32 &
            // 0xffff));
            this.codec.FlushPending();

            // If avail_out is zero, the application will call deflate again to flush the rest.
            this.rfc1950BytesEmitted = true; // write the trailer only once!

            return this.PendingCount != 0 ? ZlibConstants.Zok : ZlibConstants.ZStreamEnd;
        }

        // Compress as much as possible from the input stream, return the current block state. This function does not
        // perform lazy evaluation of matches and inserts new strings in the dictionary only for unmatched strings or
        // for short matches. It is used only for the fast compression options.

        /// <param name="codec">
        /// </param>
        /// <param name="level">
        /// </param>
        /// <param name="bits">
        /// </param>
        /// <param name="compressionStrategy">
        /// </param>
        /// <returns>
        /// </returns>
        internal int Initialize(
            ZlibCodec codec, CompressionLevel level, int bits, CompressionStrategy compressionStrategy)
        {
            return this.Initialize(codec, level, bits, MemLevelDefault, compressionStrategy);
        }

        /// <param name="codec">
        /// </param>
        /// <param name="level">
        /// </param>
        /// <param name="windowBits">
        /// </param>
        /// <param name="memLevel">
        /// </param>
        /// <param name="strategy">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        internal int Initialize(
            ZlibCodec codec,
            CompressionLevel level,
            int windowBits = ZlibConstants.WindowBitsMax,
            int memLevel = MemLevelDefault,
            CompressionStrategy strategy = CompressionStrategy.Default)
        {
            this.codec = codec;
            this.codec.Message = null;

            // validation
            if (windowBits < 9 || windowBits > 15)
            {
                throw new ZlibException("windowBits must be in the range 9..15.");
            }

            if (memLevel < 1 || memLevel > MemLevelMax)
            {
                throw new ZlibException(
                    string.Format(CultureInfo.InvariantCulture, "memLevel must be in the range 1.. {0}", MemLevelMax));
            }

            this.codec.Dstate = this;

            this.winBits = windowBits;
            this.winSize = 1 << this.winBits;
            this.winMask = this.winSize - 1;

            this.hashBits = memLevel + 7;
            this.hashSize = 1 << this.hashBits;
            this.hashMask = this.hashSize - 1;
            this.hashShift = (this.hashBits + MinMatch - 1) / MinMatch;

            this.window = new byte[this.winSize * 2];
            this.prev = new short[this.winSize];
            this.head = new short[this.hashSize];

            // for memLevel==8, this will be 16384, 16k
            this.litBufsize = 1 << (memLevel + 6);

            // Use a single array as the buffer for data pending compression, the output distance codes, and the output
            // length codes (aka tree). orig comment: This works just fine since the average output size for
            // (length,distance) codes is <= 24 bits.
            this.Pending = new byte[this.litBufsize * 4];
            this.distanceOffset = this.litBufsize;
            this.lengthOffset = (1 + 2) * this.litBufsize;

            // So, for memLevel 8, the length of the pending buffer is 65536. 64k. The first 16k are pending bytes. The
            // middle slice, of 32k, is used for distance codes. The final 16k are length codes.
            this.compressionLevel = level;
            this.compressionStrategy = strategy;

            this.Reset();
            return ZlibConstants.Zok;
        }

        /// <param name="tree">
        /// </param>
        /// <param name="k">
        /// </param>
        internal void Pqdownheap(short[] tree, int k)
        {
            var v = this.Heap[k];
            var j = k << 1; // left son of k
            while (j <= this.HeapLen)
            {
                // Set j to the smallest of the two sons:
                if (j < this.HeapLen && IsSmaller(tree, this.Heap[j + 1], this.Heap[j], this.Depth))
                {
                    j++;
                }

                // Exit if v is smaller than both sons
                if (IsSmaller(tree, v, this.Heap[j], this.Depth))
                {
                    break;
                }

                // Exchange v with the smallest son
                this.Heap[k] = this.Heap[j];
                k = j;

                // And continue down the tree, setting j to the left son of k
                j <<= 1;
            }

            this.Heap[k] = v;
        }

        // lm_init

        /// <param name="tree">
        /// </param>
        /// <param name="n">
        /// </param>
        /// <param name="m">
        /// </param>
        /// <param name="depth">
        /// </param>
        /// <returns>
        /// </returns>
        private static bool IsSmaller(short[] tree, int n, int m, sbyte[] depth)
        {
            var tn2 = tree[n * 2];
            var tm2 = tree[m * 2];
            return tn2 < tm2 || (tn2 == tm2 && depth[n] <= depth[m]);
        }

        // Scan a literal or distance tree to determine the frequencies of the codes in the bit length tree.

        // Flush the bit buffer, keeping at most 7 bits in it.

        private void BiFlush()
        {
            if (this.biValid == 16)
            {
                this.Pending[this.PendingCount++] = (byte)this.biBuf;
                this.Pending[this.PendingCount++] = (byte)(this.biBuf >> 8);
                this.biBuf = 0;
                this.biValid = 0;
            }
            else if (this.biValid >= 8)
            {
                // put_byte((byte)bi_buf);
                this.Pending[this.PendingCount++] = (byte)this.biBuf;
                this.biBuf >>= 8;
                this.biValid -= 8;
            }
        }

        // Flush the bit buffer and align the output on a byte boundary

        private void BiWindup()
        {
            if (this.biValid > 8)
            {
                this.Pending[this.PendingCount++] = (byte)this.biBuf;
                this.Pending[this.PendingCount++] = (byte)(this.biBuf >> 8);
            }
            else if (this.biValid > 0)
            {
                // put_byte((byte)bi_buf);
                this.Pending[this.PendingCount++] = (byte)this.biBuf;
            }

            this.biBuf = 0;
            this.biValid = 0;
        }

        /// <returns>
        /// </returns>
        private int BuildBlTree()
        {
            int maxBlindex; // index of last bit length code of non zero freq

            // Determine the bit length frequencies for literal and distance trees
            this.ScanTree(this.dynLtree, this.treeLiterals.MaxCode);
            this.ScanTree(this.dynDtree, this.treeDistances.MaxCode);

            // Build the bit length tree:
            this.treeBitLengths.BuildTree(this);

            // opt_len now includes the length of the tree representations, except the lengths of the bit lengths codes
            // and the 5+5+4 bits for the counts.

            // Determine the number of bit length codes to send. The pkzip format requires that at least 4 bit length
            // codes be sent. (appnote.txt says 3 but the actual value used is 4.)
            for (maxBlindex = InternalConstants.BlCodes - 1; maxBlindex >= 3; maxBlindex--)
            {
                if (this.blTree[Tree.BlOrder[maxBlindex] * 2 + 1] != 0)
                {
                    break;
                }
            }

            // Update opt_len to include the bit length tree and counts
            this.OptLen += 3 * (maxBlindex + 1) + 5 + 5 + 4;

            return maxBlindex;
        }

        // Copy a stored block, storing first the length and its one's complement if requested.

        /// <param name="buf">
        /// </param>
        /// <param name="len">
        /// </param>
        /// <param name="header">
        /// </param>
        private void CopyBlock(int buf, int len, bool header)
        {
            this.BiWindup(); // align on byte boundary
            this.lastEobLen = 8; // enough lookahead for inflate

            if (header)
            {
                unchecked
                {
                    // put_short((short)len);
                    this.Pending[this.PendingCount++] = (byte)len;
                    this.Pending[this.PendingCount++] = (byte)(len >> 8);

                    // put_short((short)~len);
                    this.Pending[this.PendingCount++] = (byte)~len;
                    this.Pending[this.PendingCount++] = (byte)(~len >> 8);
                }
            }

            this.PutBytes(this.window, buf, len);
        }

        /// <param name="flush">
        /// </param>
        /// <returns>
        /// </returns>
        private BlockState DeflateFast(FlushType flush)
        {
            // short hash_head = 0; // head of the hash chain
            var hashHead = 0; // head of the hash chain

            while (true)
            {
                // Make sure that we always have enough lookahead, except at the end of the input file. We need
                // MAX_MATCH bytes for the next match, plus MIN_MATCH bytes to insert the string following the next
                // match.
                if (this.lookAhead < MinLookahead)
                {
                    this.FillWindow();
                    if (this.lookAhead < MinLookahead && flush == FlushType.None)
                    {
                        return BlockState.NeedMore;
                    }

                    if (this.lookAhead == 0)
                    {
                        break; // flush the current block
                    }
                }

                // Insert the string window[strstart .. strstart+2] in the dictionary, and set hash_head to the head of
                // the hash chain:
                if (this.lookAhead >= MinMatch)
                {
                    this.insH = ((this.insH << this.hashShift) ^ (this.window[this.strStart + (MinMatch - 1)] & 0xff)) &
                                this.hashMask;

                    // prev[strstart&w_mask]=hash_head=head[ins_h];
                    hashHead = this.head[this.insH] & 0xffff;
                    this.prev[this.strStart & this.winMask] = this.head[this.insH];
                    this.head[this.insH] = unchecked((short)this.strStart);
                }

                // Find the longest match, discarding those <= prev_length. At this point we have always match_length <
                // MIN_MATCH

                if (hashHead != 0L && ((this.strStart - hashHead) & 0xffff) <= this.winSize - MinLookahead)
                {
                    // To simplify the code, we prevent matches with the string of window index 0 (in particular we have
                    // to avoid a match of the string with itself at the start of the input file).
                    if (this.compressionStrategy != CompressionStrategy.HuffmanOnly)
                    {
                        this.matchLength = this.LongestMatch(hashHead);
                    }

                    // longest_match() sets match_start
                }

                bool bflush; // set if current block must be flushed
                if (this.matchLength >= MinMatch)
                {
                    // check_match(strstart, match_start, match_length);
                    bflush = this.TrTally(this.strStart - this.matchStart, this.matchLength - MinMatch);

                    this.lookAhead -= this.matchLength;

                    // Insert new strings in the hash table only if the match length is not too large. This saves time
                    // but degrades compression.
                    if (this.matchLength <= this.config.MaxLazy && this.lookAhead >= MinMatch)
                    {
                        this.matchLength--; // string at strstart already in hash table
                        do
                        {
                            this.strStart++;

                            this.insH = ((this.insH << this.hashShift) ^
                                         (this.window[this.strStart + (MinMatch - 1)] & 0xff)) & this.hashMask;

                            // prev[strstart&w_mask]=hash_head=head[ins_h];
                            hashHead = this.head[this.insH] & 0xffff;
                            this.prev[this.strStart & this.winMask] = this.head[this.insH];
                            this.head[this.insH] = unchecked((short)this.strStart);

                            // strstart never exceeds WSIZE-MAX_MATCH, so there are always MIN_MATCH bytes ahead.
                        }
                        while (--this.matchLength != 0);
                        this.strStart++;
                    }
                    else
                    {
                        this.strStart += this.matchLength;
                        this.matchLength = 0;
                        this.insH = this.window[this.strStart] & 0xff;

                        this.insH = ((this.insH << this.hashShift) ^ (this.window[this.strStart + 1] & 0xff)) &
                                    this.hashMask;

                        // If lookahead < MIN_MATCH, ins_h is garbage, but it does not matter since it will be
                        // recomputed at next deflate call.
                    }
                }
                else
                {
                    // No match, output a literal byte
                    bflush = this.TrTally(0, this.window[this.strStart] & 0xff);
                    this.lookAhead--;
                    this.strStart++;
                }

                if (!bflush)
                {
                    continue;
                }

                this.FlushBlockOnly(false);
                if (this.codec.AvailableBytesOut == 0)
                {
                    return BlockState.NeedMore;
                }
            }

            this.FlushBlockOnly(flush == FlushType.Finish);
            if (this.codec.AvailableBytesOut == 0)
            {
                return flush == FlushType.Finish ? BlockState.FinishStarted : BlockState.NeedMore;
            }

            return flush == FlushType.Finish ? BlockState.FinishDone : BlockState.BlockDone;
        }

        /// <param name="flush">
        /// </param>
        /// <returns>
        /// </returns>
        private BlockState DeflateNone(FlushType flush)
        {
            // Stored blocks are limited to 0xffff bytes, pending is limited to pending_buf_size, and each stored block
            // has a 5 byte header:
            var maxBlockSize = 0xffff;

            if (maxBlockSize > this.Pending.Length - 5)
            {
                maxBlockSize = this.Pending.Length - 5;
            }

            // Copy as much as possible from input to output:
            while (true)
            {
                // Fill the window as much as possible:
                if (this.lookAhead <= 1)
                {
                    this.FillWindow();
                    if (this.lookAhead == 0 && flush == FlushType.None)
                    {
                        return BlockState.NeedMore;
                    }

                    if (this.lookAhead == 0)
                    {
                        break; // flush the current block
                    }
                }

                this.strStart += this.lookAhead;
                this.lookAhead = 0;

                // Emit a stored block if pending will be full:
                var maxStart = this.blockStart + maxBlockSize;
                if (this.strStart == 0 || this.strStart >= maxStart)
                {
                    // strstart == 0 is possible when wraparound on 16-bit machine
                    this.lookAhead = this.strStart - maxStart;
                    this.strStart = maxStart;

                    this.FlushBlockOnly(false);
                    if (this.codec.AvailableBytesOut == 0)
                    {
                        return BlockState.NeedMore;
                    }
                }

                // Flush if we may have to slide, otherwise block_start may become negative and the data will be gone:
                if (this.strStart - this.blockStart < this.winSize - MinLookahead)
                {
                    continue;
                }

                this.FlushBlockOnly(false);
                if (this.codec.AvailableBytesOut == 0)
                {
                    return BlockState.NeedMore;
                }
            }

            this.FlushBlockOnly(flush == FlushType.Finish);
            if (this.codec.AvailableBytesOut == 0)
            {
                return (flush == FlushType.Finish) ? BlockState.FinishStarted : BlockState.NeedMore;
            }

            return flush == FlushType.Finish ? BlockState.FinishDone : BlockState.BlockDone;
        }

        // Same as above, but achieves better compression. We use a lazy evaluation for matches: a match is finally
        // adopted only if there is no better match at the next window position.

        /// <param name="flush">
        /// </param>
        /// <returns>
        /// </returns>
        private BlockState DeflateSlow(FlushType flush)
        {
            // short hash_head = 0;    // head of hash chain
            var hashHead = 0; // head of hash chain

            // Process the input block.
            while (true)
            {
                // Make sure that we always have enough lookahead, except at the end of the input file. We need
                // MAX_MATCH bytes for the next match, plus MIN_MATCH bytes to insert the string following the next
                // match.

                if (this.lookAhead < MinLookahead)
                {
                    this.FillWindow();
                    if (this.lookAhead < MinLookahead && flush == FlushType.None)
                    {
                        return BlockState.NeedMore;
                    }

                    if (this.lookAhead == 0)
                    {
                        break; // flush the current block
                    }
                }

                // Insert the string window[strstart .. strstart+2] in the dictionary, and set hash_head to the head of
                // the hash chain:

                if (this.lookAhead >= MinMatch)
                {
                    this.insH = ((this.insH << this.hashShift) ^ (this.window[this.strStart + (MinMatch - 1)] & 0xff)) &
                                this.hashMask;

                    // prev[strstart&w_mask]=hash_head=head[ins_h];
                    hashHead = this.head[this.insH] & 0xffff;
                    this.prev[this.strStart & this.winMask] = this.head[this.insH];
                    this.head[this.insH] = unchecked((short)this.strStart);
                }

                // Find the longest match, discarding those <= prev_length.
                this.prevLength = this.matchLength;
                this.prevMatch = this.matchStart;
                this.matchLength = MinMatch - 1;

                if (hashHead != 0 && this.prevLength < this.config.MaxLazy &&
                    ((this.strStart - hashHead) & 0xffff) <= this.winSize - MinLookahead)
                {
                    // To simplify the code, we prevent matches with the string of window index 0 (in particular we have
                    // to avoid a match of the string with itself at the start of the input file).
                    if (this.compressionStrategy != CompressionStrategy.HuffmanOnly)
                    {
                        this.matchLength = this.LongestMatch(hashHead);
                    }

                    // longest_match() sets match_start
                    if (this.matchLength <= 5 &&
                        (this.compressionStrategy == CompressionStrategy.Filtered ||
                         (this.matchLength == MinMatch && this.strStart - this.matchStart > 4096)))
                    {
                        // If prev_match is also MIN_MATCH, match_start is garbage but we will ignore the current match
                        // anyway.
                        this.matchLength = MinMatch - 1;
                    }
                }

                // If there was a match at the previous step and the current match is not better, output the previous
                // match:
                bool bflush; // set if current block must be flushed
                if (this.prevLength >= MinMatch && this.matchLength <= this.prevLength)
                {
                    var maxInsert = this.strStart + this.lookAhead - MinMatch;

                    // Do not insert strings in hash table beyond this.

                    // check_match(strstart-1, prev_match, prev_length);
                    bflush = this.TrTally(this.strStart - 1 - this.prevMatch, this.prevLength - MinMatch);

                    // Insert in hash table all strings up to the end of the match. strstart-1 and strstart are already
                    // inserted. If there is not enough lookahead, the last two strings are not inserted in the hash
                    // table.
                    this.lookAhead -= this.prevLength - 1;
                    this.prevLength -= 2;
                    do
                    {
                        if (++this.strStart > maxInsert)
                        {
                            continue;
                        }

                        this.insH = ((this.insH << this.hashShift) ^
                                     (this.window[this.strStart + (MinMatch - 1)] & 0xff)) & this.hashMask;

                        // prev[strstart&w_mask]=hash_head=head[ins_h];
                        hashHead = this.head[this.insH] & 0xffff;
                        this.prev[this.strStart & this.winMask] = this.head[this.insH];
                        this.head[this.insH] = unchecked((short)this.strStart);
                    }
                    while (--this.prevLength != 0);
                    this.matchAvailable = 0;
                    this.matchLength = MinMatch - 1;
                    this.strStart++;

                    if (bflush)
                    {
                        this.FlushBlockOnly(false);
                        if (this.codec.AvailableBytesOut == 0)
                        {
                            return BlockState.NeedMore;
                        }
                    }
                }
                else if (this.matchAvailable != 0)
                {
                    // If there was no match at the previous position, output a single literal. If there was a match but
                    // the current match is longer, truncate the previous match to a single literal.
                    bflush = this.TrTally(0, this.window[this.strStart - 1] & 0xff);

                    if (bflush)
                    {
                        this.FlushBlockOnly(false);
                    }

                    this.strStart++;
                    this.lookAhead--;
                    if (this.codec.AvailableBytesOut == 0)
                    {
                        return BlockState.NeedMore;
                    }
                }
                else
                {
                    // There is no previous match to compare with, wait for the next step to decide.
                    this.matchAvailable = 1;
                    this.strStart++;
                    this.lookAhead--;
                }
            }

            if (this.matchAvailable != 0)
            {
                this.TrTally(0, this.window[this.strStart - 1] & 0xff);
                this.matchAvailable = 0;
            }

            this.FlushBlockOnly(flush == FlushType.Finish);

            if (this.codec.AvailableBytesOut == 0)
            {
                return flush == FlushType.Finish ? BlockState.FinishStarted : BlockState.NeedMore;
            }

            return flush == FlushType.Finish ? BlockState.FinishDone : BlockState.BlockDone;
        }

        private void FillWindow()
        {
            do
            {
                var more = this.windowSize - this.lookAhead - this.strStart;

                // Amount of free space at the end of the window.

                // Deal with !@#$% 64K limit:
                int n;
                if (more == 0 && this.strStart == 0 && this.lookAhead == 0)
                {
                    more = this.winSize;
                }
                else if (more == -1)
                {
                    // Very unlikely, but possible on 16 bit machine if strstart == 0 and lookahead == 1 (input done one
                    // byte at time)
                    more--;

                    // If the window is almost full and there is insufficient lookahead, move the upper half to the
                    // lower one to make room in the upper half.
                }
                else if (this.strStart >= this.winSize + this.winSize - MinLookahead)
                {
                    Array.Copy(this.window, this.winSize, this.window, 0, this.winSize);
                    this.matchStart -= this.winSize;
                    this.strStart -= this.winSize; // we now have strstart >= MAX_DIST
                    this.blockStart -= this.winSize;

                    // Slide the hash table (could be avoided with 32 bit values at the expense of memory usage). We
                    // slide even when level == 0 to keep the hash table consistent if we switch back to level > 0
                    // later. (Using level 0 permanently is not an optimal usage of zlib, so we don't care about this
                    // pathological case.)
                    n = this.hashSize;
                    var p = n;
                    int m;
                    do
                    {
                        m = this.head[--p] & 0xffff;
                        this.head[p] = (short)((m >= this.winSize) ? (m - this.winSize) : 0);
                    }
                    while (--n != 0);

                    n = this.winSize;
                    p = n;
                    do
                    {
                        m = this.prev[--p] & 0xffff;
                        this.prev[p] = (short)((m >= this.winSize) ? (m - this.winSize) : 0);

                        // If n is not on any hash chain, prev[n] is garbage but its value will never be used.
                    }
                    while (--n != 0);
                    more += this.winSize;
                }

                if (this.codec.AvailableBytesIn == 0)
                {
                    return;
                }

                // If there was no sliding: strstart <= WSIZE+MAX_DIST-1 && lookahead <= MIN_LOOKAHEAD - 1 && more ==
                // window_size - lookahead - strstart => more >= window_size - (MIN_LOOKAHEAD-1 + WSIZE + MAX_DIST-1) =>
                // more >= window_size - 2*WSIZE + 2 In the BIG_MEM or MMAP case (not yet supported),
                // window_size == input_size + MIN_LOOKAHEAD  &&
                // strstart + s->lookahead <= input_size => more >= MIN_LOOKAHEAD. Otherwise, window_size == 2*WSIZE so
                // more >= 2. If there was sliding, more >= WSIZE. So in all cases, more >= 2.
                n = this.codec.ReadBuf(this.window, this.strStart + this.lookAhead, more);
                this.lookAhead += n;

                // Initialize the hash value now that we have some input:
                if (this.lookAhead >= MinMatch)
                {
                    this.insH = this.window[this.strStart] & 0xff;
                    this.insH = ((this.insH << this.hashShift) ^ (this.window[this.strStart + 1] & 0xff)) &
                                this.hashMask;
                }

                // If the whole input has less than MIN_MATCH bytes, ins_h is garbage, but this is not important since
                // only literal bytes will be emitted.
            }
            while (this.lookAhead < MinLookahead && this.codec.AvailableBytesIn != 0);
        }

        /*
        internal int End()
        {
            if (this.Status != InitState && this.Status != BusyState && this.Status != FinishState)
            {
                return ZlibConstants.ZStreamError;
            }

            // Deallocate in reverse order of allocations:
            this.Pending = null;
            this.Head = null;
            this.Prev = null;
            this.Window = null;

            // free dstate=null;
            return this.Status == BusyState ? ZlibConstants.ZDataError : ZlibConstants.Zok;
        }
*/

        /// <param name="eof">
        /// </param>
        private void FlushBlockOnly(bool eof)
        {
            this.TrFlushBlock(this.blockStart >= 0 ? this.blockStart : -1, this.strStart - this.blockStart, eof);
            this.blockStart = this.strStart;
            this.codec.FlushPending();
        }

        private void InitializeBlocks()
        {
            // Initialize the trees.
            for (var i = 0; i < InternalConstants.LCodes; i++)
            {
                this.dynLtree[i * 2] = 0;
            }

            for (var i = 0; i < InternalConstants.DCodes; i++)
            {
                this.dynDtree[i * 2] = 0;
            }

            for (var i = 0; i < InternalConstants.BlCodes; i++)
            {
                this.blTree[i * 2] = 0;
            }

            this.dynLtree[EndBlock * 2] = 1;
            this.OptLen = this.StaticLen = 0;
            this.lastLit = this.matches = 0;
        }

        private void InitializeLazyMatch()
        {
            this.windowSize = 2 * this.winSize;

            // clear the hash - workitem 9063
            Array.Clear(this.head, 0, this.hashSize);

            // for (int i = 0; i < hash_size; i++) head[i] = 0;
            this.config = Config.Lookup(this.compressionLevel);
            this.SetDeflater();

            this.strStart = 0;
            this.blockStart = 0;
            this.lookAhead = 0;
            this.matchLength = this.prevLength = MinMatch - 1;
            this.matchAvailable = 0;
            this.insH = 0;
        }

        // Initialize the tree data structures for a new zlib stream.

        private void InitializeTreeData()
        {
            this.treeLiterals.DynTree = this.dynLtree;
            this.treeLiterals.StaticTree = StaticTree.Literals;

            this.treeDistances.DynTree = this.dynDtree;
            this.treeDistances.StaticTree = StaticTree.Distances;

            this.treeBitLengths.DynTree = this.blTree;
            this.treeBitLengths.StaticTree = StaticTree.BitLengths;

            this.biBuf = 0;
            this.biValid = 0;
            this.lastEobLen = 8; // enough lookahead for inflate

            // Initialize the first block of the first file:
            this.InitializeBlocks();
        }

        /// <param name="curMatch">
        /// </param>
        /// <returns>
        /// </returns>
        private int LongestMatch(int curMatch)
        {
            var chainLength = this.config.MaxChainLength; // max hash chain length
            var scan = this.strStart; // current string
            var bestLen = this.prevLength; // best match length so far
            var limit = this.strStart > (this.winSize - MinLookahead) ? this.strStart - (this.winSize - MinLookahead) : 0;

            var niceLength = this.config.NiceLength;

            // Stop when cur_match becomes <= limit. To simplify the code, we prevent matches with the string of window
            // index 0.
            var wmask = this.winMask;

            var strend = this.strStart + MaxMatch;
            var scanEnd1 = this.window[scan + bestLen - 1];
            var scanEnd = this.window[scan + bestLen];

            // The code is optimized for HASH_BITS >= 8 and MAX_MATCH-2 multiple of 16. It is easy to get rid of this
            // optimization if necessary.

            // Do not waste too much time if we already have a good match:
            if (this.prevLength >= this.config.GoodLength)
            {
                chainLength >>= 2;
            }

            // Do not look for matches beyond the end of the input. This is necessary to make deflate deterministic.
            if (niceLength > this.lookAhead)
            {
                niceLength = this.lookAhead;
            }

            do
            {
                var match = curMatch; // matched string

                // Skip to next match if the match length cannot increase or if the match length is less than 2:
                if (this.window[match + bestLen] != scanEnd || this.window[match + bestLen - 1] != scanEnd1 ||
                    this.window[match] != this.window[scan] || this.window[++match] != this.window[scan + 1])
                {
                    continue;
                }

                // The check at best_len-1 can be removed because it will be made again later. (This heuristic is not
                // always a win.) It is not necessary to compare scan[2] and match[2] since they are always equal when
                // the other bytes match, given that the hash keys are equal and that HASH_BITS >= 8.
                scan += 2;
                match++;

                // We check for insufficient lookahead only every 8th comparison; the 256th check will be made at
                // strstart+258.
                do
                {
                }
                while (this.window[++scan] == this.window[++match] && this.window[++scan] == this.window[++match] &&
                       this.window[++scan] == this.window[++match] && this.window[++scan] == this.window[++match] &&
                       this.window[++scan] == this.window[++match] && this.window[++scan] == this.window[++match] &&
                       this.window[++scan] == this.window[++match] && this.window[++scan] == this.window[++match] &&
                       scan < strend);

                var len = MaxMatch - (strend - scan); // length of current match
                scan = strend - MaxMatch;

                if (len <= bestLen)
                {
                    continue;
                }

                this.matchStart = curMatch;
                bestLen = len;
                if (len >= niceLength)
                {
                    break;
                }

                scanEnd1 = this.window[scan + bestLen - 1];
                scanEnd = this.window[scan + bestLen];
            }
            while ((curMatch = this.prev[curMatch & wmask] & 0xffff) > limit && --chainLength != 0);

            if (bestLen <= this.lookAhead)
            {
                return bestLen;
            }

            return this.lookAhead;
        }

        /// <param name="p">
        /// </param>
        /// <param name="start">
        /// </param>
        /// <param name="len">
        /// </param>
        private void PutBytes(byte[] p, int start, int len)
        {
            Array.Copy(p, start, this.Pending, this.PendingCount, len);
            this.PendingCount += len;
        }

        private void Reset()
        {
            this.codec.TotalBytesIn = this.codec.TotalBytesOut = 0;
            this.codec.Message = null;

            // strm.data_type = Z_UNKNOWN;
            this.PendingCount = 0;
            this.NextPending = 0;

            this.rfc1950BytesEmitted = false;

            this.status = this.WantRfc1950HeaderBytes ? InitState : BusyState;
            this.codec.Adler32 = Adler.Adler32(0, null, 0, 0);

            this.lastFlush = (int)FlushType.None;

            this.InitializeTreeData();
            this.InitializeLazyMatch();
        }

        /// <param name="tree">
        /// </param>
        /// <param name="maxCode">
        /// </param>
        private void ScanTree(short[] tree, int maxCode)
        {
            int n; // iterates over all tree elements
            var prevlen = -1; // last emitted length
            int nextlen = tree[0 * 2 + 1]; // length of next code
            var count = 0; // repeat count of the current code
            var maxCount = 7; // max repeat count
            var minCount = 4; // min repeat count

            if (nextlen == 0)
            {
                maxCount = 138;
                minCount = 3;
            }

            tree[(maxCode + 1) * 2 + 1] = 0x7fff; // guard //??

            for (n = 0; n <= maxCode; n++)
            {
                var curlen = nextlen; // length of current code
                nextlen = tree[(n + 1) * 2 + 1];
                if (++count < maxCount && curlen == nextlen)
                {
                    continue;
                }

                if (count < minCount)
                {
                    this.blTree[curlen * 2] = (short)(this.blTree[curlen * 2] + count);
                }
                else if (curlen != 0)
                {
                    if (curlen != prevlen)
                    {
                        this.blTree[curlen * 2]++;
                    }

                    this.blTree[InternalConstants.Rep36 * 2]++;
                }
                else if (count <= 10)
                {
                    this.blTree[InternalConstants.Repz310 * 2]++;
                }
                else
                {
                    this.blTree[InternalConstants.Repz11138 * 2]++;
                }

                count = 0;
                prevlen = curlen;
                if (nextlen == 0)
                {
                    maxCount = 138;
                    minCount = 3;
                }
                else if (curlen == nextlen)
                {
                    maxCount = 6;
                    minCount = 3;
                }
                else
                {
                    maxCount = 7;
                    minCount = 4;
                }
            }
        }

        /// <param name="lcodes">
        /// </param>
        /// <param name="dcodes">
        /// </param>
        /// <param name="blcodes">
        /// </param>
        private void SendAllTrees(int lcodes, int dcodes, int blcodes)
        {
            int rank; // index in bl_order

            this.SendBits(lcodes - 257, 5); // not +255 as stated in appnote.txt
            this.SendBits(dcodes - 1, 5);
            this.SendBits(blcodes - 4, 4); // not -3 as stated in appnote.txt
            for (rank = 0; rank < blcodes; rank++)
            {
                this.SendBits(this.blTree[Tree.BlOrder[rank] * 2 + 1], 3);
            }

            this.SendTree(this.dynLtree, lcodes - 1); // literal tree
            this.SendTree(this.dynDtree, dcodes - 1); // distance tree
        }

        /// <param name="value">
        /// </param>
        /// <param name="length">
        /// </param>
        private void SendBits(int value, int length)
        {
            var len = length;
            unchecked
            {
                if (this.biValid > BufSize - len)
                {
                    // int val = value; bi_buf |= (val << bi_valid);
                    this.biBuf |= (short)((value << this.biValid) & 0xffff);

                    // put_short(bi_buf);
                    this.Pending[this.PendingCount++] = (byte)this.biBuf;
                    this.Pending[this.PendingCount++] = (byte)(this.biBuf >> 8);

                    this.biBuf = (short)((uint)value >> (BufSize - this.biValid));
                    this.biValid += len - BufSize;
                }
                else
                {
                    // bi_buf |= (value) << bi_valid;
                    this.biBuf |= (short)((value << this.biValid) & 0xffff);
                    this.biValid += len;
                }
            }
        }

        /// <param name="c">
        /// </param>
        /// <param name="tree">
        /// </param>
        private void SendCode(int c, short[] tree)
        {
            var c2 = c * 2;
            this.SendBits(tree[c2] & 0xffff, tree[c2 + 1] & 0xffff);
        }

        /// <param name="ltree">
        /// </param>
        /// <param name="dtree">
        /// </param>
        private void SendCompressedBlock(short[] ltree, short[] dtree)
        {
            var lx = 0; // running index in l_buf

            if (this.lastLit != 0)
            {
                do
                {
                    var ix = this.distanceOffset + lx * 2;
                    var distance = ((this.Pending[ix] << 8) & 0xff00) | (this.Pending[ix + 1] & 0xff);

                    // distance of matched string
                    var lc = this.Pending[this.lengthOffset + lx] & 0xff;

                    // match length or unmatched char (if dist == 0)
                    lx++;

                    if (distance == 0)
                    {
                        this.SendCode(lc, ltree); // send a literal byte
                    }
                    else
                    {
                        // literal or match pair Here, lc is the match length - MIN_MATCH
                        int code = Tree.LengthCode[lc]; // the code to send

                        // send the length code
                        this.SendCode(code + InternalConstants.Literals + 1, ltree);
                        var extra = Tree.ExtraLengthBits[code]; // number of extra bits to send
                        if (extra != 0)
                        {
                            // send the extra length bits
                            lc -= Tree.LengthBase[code];
                            this.SendBits(lc, extra);
                        }

                        distance--; // dist is now the match distance - 1
                        code = Tree.DistanceCode(distance);

                        // send the distance code
                        this.SendCode(code, dtree);

                        extra = Tree.ExtraDistanceBits[code];
                        if (extra != 0)
                        {
                            // send the extra distance bits
                            distance -= Tree.DistanceBase[code];
                            this.SendBits(distance, extra);
                        }
                    }

                    // Check that the overlay between pending and d_buf+l_buf is ok:
                }
                while (lx < this.lastLit);
            }

            this.SendCode(EndBlock, ltree);
            this.lastEobLen = ltree[EndBlock * 2 + 1];
        }

        /// <param name="tree">
        /// </param>
        /// <param name="maxCode">
        /// </param>
        private void SendTree(short[] tree, int maxCode)
        {
            int n; // iterates over all tree elements
            var prevlen = -1; // last emitted length
            int nextlen = tree[0 * 2 + 1]; // length of next code
            var count = 0; // repeat count of the current code
            var maxCount = 7; // max repeat count
            var minCount = 4; // min repeat count

            if (nextlen == 0)
            {
                maxCount = 138;
                minCount = 3;
            }

            for (n = 0; n <= maxCode; n++)
            {
                var curlen = nextlen; // length of current code
                nextlen = tree[(n + 1) * 2 + 1];
                if (++count < maxCount && curlen == nextlen)
                {
                    continue;
                }

                if (count < minCount)
                {
                    do
                    {
                        this.SendCode(curlen, this.blTree);
                    }
                    while (--count != 0);
                }
                else if (curlen != 0)
                {
                    if (curlen != prevlen)
                    {
                        this.SendCode(curlen, this.blTree);
                        count--;
                    }

                    this.SendCode(InternalConstants.Rep36, this.blTree);
                    this.SendBits(count - 3, 2);
                }
                else if (count <= 10)
                {
                    this.SendCode(InternalConstants.Repz310, this.blTree);
                    this.SendBits(count - 3, 3);
                }
                else
                {
                    this.SendCode(InternalConstants.Repz11138, this.blTree);
                    this.SendBits(count - 11, 7);
                }

                count = 0;
                prevlen = curlen;
                if (nextlen == 0)
                {
                    maxCount = 138;
                    minCount = 3;
                }
                else if (curlen == nextlen)
                {
                    maxCount = 6;
                    minCount = 3;
                }
                else
                {
                    maxCount = 7;
                    minCount = 4;
                }
            }
        }

        private void SetDataType()
        {
            var n = 0;
            var asciiFreq = 0;
            var binFreq = 0;
            while (n < 7)
            {
                binFreq += this.dynLtree[n * 2];
                n++;
            }

            while (n < 128)
            {
                asciiFreq += this.dynLtree[n * 2];
                n++;
            }

            while (n < InternalConstants.Literals)
            {
                binFreq += this.dynLtree[n * 2];
                n++;
            }

            this.dataType = (sbyte)(binFreq > (asciiFreq >> 2) ? ZBinary : ZAscii);
        }

        private void SetDeflater()
        {
            switch (this.config.Flavor)
            {
                case DeflateFlavor.Store:
                    this.deflateFunction = this.DeflateNone;
                    break;
                case DeflateFlavor.Fast:
                    this.deflateFunction = this.DeflateFast;
                    break;
                case DeflateFlavor.Slow:
                    this.deflateFunction = this.DeflateSlow;
                    break;
            }
        }

        /*
        
        internal int SetDictionary(byte[] dictionary)
        {
            var length = dictionary.Length;
            var index = 0;

            if (dictionary == null || this.Status != InitState)
            {
                throw new ZlibException("Stream error.");
            }

            this.Codec._Adler32 = Adler.Adler32(this.Codec._Adler32, dictionary, 0, dictionary.Length);

            if (length < MinMatch)
            {
                return ZlibConstants.Zok;
            }

            if (length > this.WSize - MinLookahead)
            {
                length = this.WSize - MinLookahead;
                index = dictionary.Length - length; // use the tail of the dictionary
            }

            Array.Copy(dictionary, index, this.Window, 0, length);
            this.StrStart = length;
            this.BlockStart = length;

            // Insert all strings in the hash table (except for the last two bytes). s->lookahead stays null, so
            // s->ins_h will be recomputed at the next call of fill_window.
            this.InsH = this.Window[0] & 0xff;
            this.InsH = ((this.InsH << this.HashShift) ^ (this.Window[1] & 0xff)) & this.HashMask;

            for (var n = 0; n <= length - MinMatch; n++)
            {
                this.InsH = ((this.InsH << this.HashShift) ^ (this.Window[n + (MinMatch - 1)] & 0xff)) & this.HashMask;
                this.Prev[n & this.WMask] = this.Head[this.InsH];
                this.Head[this.InsH] = (short)n;
            }

            return ZlibConstants.Zok;
        }
*/

        /*
        internal int SetParams(CompressionLevel level, CompressionStrategy strategy)
        {
            var result = ZlibConstants.Zok;

            if (this.CompressionLevel != level)
            {
                var newConfig = Config.Lookup(level);

                // change in the deflate flavor (Fast vs slow vs none)?
                if (newConfig.Flavor != this.config.Flavor && this.Codec.TotalBytesIn != 0)
                {
                    // Flush the last buffer:
                    result = this.Codec.Deflate(FlushType.Partial);
                }

                this.CompressionLevel = level;
                this.config = newConfig;
                this.SetDeflater();
            }

            // no need to flush with change in strategy?  Really? 
            this.CompressionStrategy = strategy;

            return result;
        }
*/

        private void TrAlign()
        {
            this.SendBits(StaticTrees << 1, 3);
            this.SendCode(EndBlock, StaticTree.LengthAndLiteralsTreeCodes);

            this.BiFlush();

            // Of the 10 bits for the empty block, we have already sent (10 - bi_valid) bits. The lookahead for the last
            // real code (before the EOB of the previous block) was thus at least one plus the length of the EOB plus
            // what we have just sent of the empty static block.
            if (1 + this.lastEobLen + 10 - this.biValid < 9)
            {
                this.SendBits(StaticTrees << 1, 3);
                this.SendCode(EndBlock, StaticTree.LengthAndLiteralsTreeCodes);
                this.BiFlush();
            }

            this.lastEobLen = 7;
        }

        /// <param name="buf">
        /// </param>
        /// <param name="storedLen">
        /// </param>
        /// <param name="eof">
        /// </param>
        private void TrFlushBlock(int buf, int storedLen, bool eof)
        {
            int optLenb, staticLenb; // opt_len and static_len in bytes
            var maxBlindex = 0; // index of last bit length code of non zero freq

            // Build the Huffman trees unless a stored block is forced
            if (this.compressionLevel > 0)
            {
                // Check if the file is ascii or binary
                if (this.dataType == ZUnknown)
                {
                    this.SetDataType();
                }

                // Construct the literal and distance trees
                this.treeLiterals.BuildTree(this);

                this.treeDistances.BuildTree(this);

                // At this point, opt_len and static_len are the total bit lengths of the compressed block data,
                // excluding the tree representations.

                // Build the bit length tree for the above two trees, and get the index in bl_order of the last bit
                // length code to send.
                maxBlindex = this.BuildBlTree();

                // Determine the best encoding. Compute first the block length in bytes
                optLenb = (this.OptLen + 3 + 7) >> 3;
                staticLenb = (this.StaticLen + 3 + 7) >> 3;

                if (staticLenb <= optLenb)
                {
                    optLenb = staticLenb;
                }
            }
            else
            {
                optLenb = staticLenb = storedLen + 5; // force a stored block
            }

            if (storedLen + 4 <= optLenb && buf != -1)
            {
                // 4: two words for the lengths The test buf != NULL is only necessary if LIT_BUFSIZE > WSIZE. Otherwise
                // we can't have processed more than WSIZE input bytes since the last block flush, because compression
                // would have been successful. If LIT_BUFSIZE <= WSIZE, it is never too late to transform a block into a
                // stored block.
                this.TrStoredBlock(buf, storedLen, eof);
            }
            else if (staticLenb == optLenb)
            {
                this.SendBits((StaticTrees << 1) + (eof ? 1 : 0), 3);
                this.SendCompressedBlock(StaticTree.LengthAndLiteralsTreeCodes, StaticTree.DistTreeCodes);
            }
            else
            {
                this.SendBits((DynTrees << 1) + (eof ? 1 : 0), 3);
                this.SendAllTrees(this.treeLiterals.MaxCode + 1, this.treeDistances.MaxCode + 1, maxBlindex + 1);
                this.SendCompressedBlock(this.dynLtree, this.dynDtree);
            }

            // The above check is made mod 2^32, for files larger than 512 MB and uLong implemented on 32 bits.
            this.InitializeBlocks();

            if (eof)
            {
                this.BiWindup();
            }
        }

        /// <param name="buf">
        /// </param>
        /// <param name="storedLen">
        /// </param>
        /// <param name="eof">
        /// </param>
        private void TrStoredBlock(int buf, int storedLen, bool eof)
        {
            this.SendBits((StoredBlock << 1) + (eof ? 1 : 0), 3); // send block type
            this.CopyBlock(buf, storedLen, true); // with header
        }

        /// <param name="dist">
        /// </param>
        /// <param name="lc">
        /// </param>
        /// <returns>
        /// </returns>
        private bool TrTally(int dist, int lc)
        {
            this.Pending[this.distanceOffset + this.lastLit * 2] = unchecked((byte)((uint)dist >> 8));
            this.Pending[this.distanceOffset + this.lastLit * 2 + 1] = unchecked((byte)dist);
            this.Pending[this.lengthOffset + this.lastLit] = unchecked((byte)lc);
            this.lastLit++;

            if (dist == 0)
            {
                // lc is the unmatched char
                this.dynLtree[lc * 2]++;
            }
            else
            {
                this.matches++;

                // Here, lc is the match length - MIN_MATCH
                dist--; // dist = match distance - 1
                this.dynLtree[(Tree.LengthCode[lc] + InternalConstants.Literals + 1) * 2]++;
                this.dynDtree[Tree.DistanceCode(dist) * 2]++;
            }

            if ((this.lastLit & 0x1fff) == 0 && (int)this.compressionLevel > 2)
            {
                // Compute an upper bound for the compressed length
                var outLength = this.lastLit << 3;
                var inLength = this.strStart - this.blockStart;
                int dcode;
                for (dcode = 0; dcode < InternalConstants.DCodes; dcode++)
                {
                    outLength = (int)(outLength + this.dynDtree[dcode * 2] * (5L + Tree.ExtraDistanceBits[dcode]));
                }

                outLength >>= 3;
                if ((this.matches < (this.lastLit / 2)) && outLength < inLength / 2)
                {
                    return true;
                }
            }

            return (this.lastLit == this.litBufsize - 1) || (this.lastLit == this.litBufsize);

            // dinoch - wraparound? We avoid equality with lit_bufsize because of wraparound at 64K on 16 bit machines
            // and because stored blocks are restricted to 64K-1 bytes.
        }

        #endregion
    }
}