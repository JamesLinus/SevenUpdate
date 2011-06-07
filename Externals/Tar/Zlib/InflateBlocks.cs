//-----------------------------------------------------------------------
// <copyright file="InflateBlocks.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    using System;

    /// <summary>
    /// </summary>
    internal sealed class InflateBlocks
    {
        #region Constants and Fields

        internal readonly ZlibCodec Codec; // pointer back to this zlib stream

        internal readonly int End; // one byte after sliding window

        internal int Bitb; // bit buffer

        internal int Bitk; // bits in bit buffer

        internal int ReadAt; // window read pointer

        internal byte[] Window; // sliding window

        internal int WriteAt; // window write pointer

        private const int Many = 1440;

        private static readonly int[] Border = new[]
            {
                16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
            };

        private readonly int[] bb = new int[1]; // bit length tree depth

        private readonly object checkfn; // check function

        private readonly InflateCodes codes = new InflateCodes(); // if CODES, current state

        private readonly InfTree inftree = new InfTree();

        private readonly int[] tb = new int[1]; // bit length decoding tree

        private int[] blens; // bit lengths of codes

        private uint check; // check on output

        private int[] hufts; // single malloc for tree space

        private int index; // index into blens (or border)

        private int last; // true if this block is the last block

        private int left; // if STORED, bytes left to copy

        private InflateBlockMode mode; // current inflate_block mode

        private int table; // table lengths (14 bits)

        #endregion

        #region Constructors and Destructors

        internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
        {
            this.Codec = codec;
            this.hufts = new int[Many * 3];
            this.Window = new byte[w];
            this.End = w;
            this.checkfn = checkfn;
            this.mode = InflateBlockMode.Type;
            this.Reset();
        }

        #endregion

        #region Enums

        private enum InflateBlockMode
        {
            /// <summary>
            /// </summary>
            Type = 0, // get type bits (3, including end bit)

            /// <summary>
            /// </summary>
            Lens = 1, // get lengths for stored

            /// <summary>
            /// </summary>
            Stored = 2, // processing stored block

            /// <summary>
            /// </summary>
            Table = 3, // get table lengths

            /// <summary>
            /// </summary>
            Btree = 4, // get bit lengths tree for a dynamic block

            /// <summary>
            /// </summary>
            Dtree = 5, // get length, distance trees for a dynamic block

            /// <summary>
            /// </summary>
            Codes = 6, // processing fixed or dynamic block

            /// <summary>
            /// </summary>
            Dry = 7, // output remaining window bytes

            /// <summary>
            /// </summary>
            Done = 8, // finished last block, done

            /// <summary>
            /// </summary>
            Bad = 9, // ot a data error--stuck here
        }

        #endregion

        #region Methods

        /// <param name="r">
        /// </param>
        /// <returns>
        /// </returns>
        internal int Flush(int r)
        {
            for (var pass = 0; pass < 2; pass++)
            {
                int bytes;
                if (pass == 0)
                {
                    // compute number of bytes to copy as far as end of window
                    bytes = (this.ReadAt <= this.WriteAt ? this.WriteAt : this.End) - this.ReadAt;
                }
                else
                {
                    // compute bytes to copy
                    bytes = this.WriteAt - this.ReadAt;
                }

                // workitem 8870
                if (bytes == 0)
                {
                    if (r == ZlibConstants.ZBufError)
                    {
                        r = ZlibConstants.Zok;
                    }

                    return r;
                }

                if (bytes > this.Codec.AvailableBytesOut)
                {
                    bytes = this.Codec.AvailableBytesOut;
                }

                if (bytes != 0 && r == ZlibConstants.ZBufError)
                {
                    r = ZlibConstants.Zok;
                }

                // update counters
                this.Codec.AvailableBytesOut -= bytes;
                this.Codec.TotalBytesOut += bytes;

                // update check information
                if (this.checkfn != null)
                {
                    this.Codec.Adler32 = this.check = Adler.Adler32(this.check, this.Window, this.ReadAt, bytes);
                }

                // copy as far as end of window
                Array.Copy(this.Window, this.ReadAt, this.Codec.OutputBuffer, this.Codec.NextOut, bytes);
                this.Codec.NextOut += bytes;
                this.ReadAt += bytes;

                // see if more to copy at beginning of window
                if (this.ReadAt == this.End && pass == 0)
                {
                    // wrap pointers
                    this.ReadAt = 0;
                    if (this.WriteAt == this.End)
                    {
                        this.WriteAt = 0;
                    }
                }
                else
                {
                    pass++;
                }
            }

            // done
            return r;
        }

        internal void Free()
        {
            this.Reset();
            this.Window = null;
            this.hufts = null;
        }

        /// <param name="r">
        /// </param>
        /// <returns>
        /// </returns>
        internal int Process(int r)
        {
            // copy input/output information to locals (UPDATE macro restores)
            var p = this.Codec.NextIn;
            var n = this.Codec.AvailableBytesIn;
            var b = this.Bitb;
            var k = this.Bitk;

            var q = this.WriteAt;
            var m = q < this.ReadAt ? this.ReadAt - q - 1 : this.End - q;

            // process input based on current state
            while (true)
            {
                int t; // temporary storage
                switch (this.mode)
                {
                    case InflateBlockMode.Type:

                        while (k < 3)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Zok;
                            }
                            else
                            {
                                this.Bitb = b;
                                this.Bitk = k;
                                this.Codec.AvailableBytesIn = n;
                                this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                this.Codec.NextIn = p;
                                this.WriteAt = q;
                                return this.Flush(r);
                            }

                            n--;
                            b |= (this.Codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        t = b & 7;
                        this.last = t & 1;

                        switch ((uint)t >> 1)
                        {
                            case 0: // stored
                                b >>= 3;
                                k -= 3;
                                t = k & 7; // go to byte boundary
                                b >>= t;
                                k -= t;
                                this.mode = InflateBlockMode.Lens; // get length of stored block
                                break;

                            case 1: // fixed
                                var bl = new int[1];
                                var bd = new int[1];
                                var tl = new int[1][];
                                var td = new int[1][];
                                InfTree.InflateTreesFixed(bl, bd, tl, td);
                                this.codes.Init(bl[0], bd[0], tl[0], 0, td[0], 0);
                                b >>= 3;
                                k -= 3;
                                this.mode = InflateBlockMode.Codes;
                                break;

                            case 2: // dynamic
                                b >>= 3;
                                k -= 3;
                                this.mode = InflateBlockMode.Table;
                                break;

                            case 3: // illegal
                                b >>= 3;
                                k -= 3;
                                this.mode = InflateBlockMode.Bad;
                                this.Codec.Message = "invalid block type";
                                r = ZlibConstants.ZDataError;
                                this.Bitb = b;
                                this.Bitk = k;
                                this.Codec.AvailableBytesIn = n;
                                this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                this.Codec.NextIn = p;
                                this.WriteAt = q;
                                return this.Flush(r);
                        }

                        break;

                    case InflateBlockMode.Lens:

                        while (k < 32)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Zok;
                            }
                            else
                            {
                                this.Bitb = b;
                                this.Bitk = k;
                                this.Codec.AvailableBytesIn = n;
                                this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                this.Codec.NextIn = p;
                                this.WriteAt = q;
                                return this.Flush(r);
                            }

                            n--;
                            b |= (this.Codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        if ((((~b) >> 16) & 0xffff) != (b & 0xffff))
                        {
                            this.mode = InflateBlockMode.Bad;
                            this.Codec.Message = "invalid stored block lengths";
                            r = ZlibConstants.ZDataError;

                            this.Bitb = b;
                            this.Bitk = k;
                            this.Codec.AvailableBytesIn = n;
                            this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                            this.Codec.NextIn = p;
                            this.WriteAt = q;
                            return this.Flush(r);
                        }

                        this.left = b & 0xffff;
                        b = k = 0; // dump bits
                        this.mode = this.left != 0
                                        ? InflateBlockMode.Stored
                                        : (this.last != 0 ? InflateBlockMode.Dry : InflateBlockMode.Type);
                        break;

                    case InflateBlockMode.Stored:
                        if (n == 0)
                        {
                            this.Bitb = b;
                            this.Bitk = k;
                            this.Codec.AvailableBytesIn = n;
                            this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                            this.Codec.NextIn = p;
                            this.WriteAt = q;
                            return this.Flush(r);
                        }

                        if (m == 0)
                        {
                            if (q == this.End && this.ReadAt != 0)
                            {
                                q = 0;
                                m = q < this.ReadAt ? this.ReadAt - q - 1 : this.End - q;
                            }

                            if (m == 0)
                            {
                                this.WriteAt = q;
                                r = this.Flush(r);
                                q = this.WriteAt;
                                m = q < this.ReadAt ? this.ReadAt - q - 1 : this.End - q;
                                if (q == this.End && this.ReadAt != 0)
                                {
                                    q = 0;
                                    m = q < this.ReadAt ? this.ReadAt - q - 1 : this.End - q;
                                }

                                if (m == 0)
                                {
                                    this.Bitb = b;
                                    this.Bitk = k;
                                    this.Codec.AvailableBytesIn = n;
                                    this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                    this.Codec.NextIn = p;
                                    this.WriteAt = q;
                                    return this.Flush(r);
                                }
                            }
                        }

                        r = ZlibConstants.Zok;

                        t = this.left;
                        if (t > n)
                        {
                            t = n;
                        }

                        if (t > m)
                        {
                            t = m;
                        }

                        Array.Copy(this.Codec.InputBuffer, p, this.Window, q, t);
                        p += t;
                        n -= t;
                        q += t;
                        m -= t;
                        if ((this.left -= t) != 0)
                        {
                            break;
                        }

                        this.mode = this.last != 0 ? InflateBlockMode.Dry : InflateBlockMode.Type;
                        break;

                    case InflateBlockMode.Table:

                        while (k < 14)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Zok;
                            }
                            else
                            {
                                this.Bitb = b;
                                this.Bitk = k;
                                this.Codec.AvailableBytesIn = n;
                                this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                this.Codec.NextIn = p;
                                this.WriteAt = q;
                                return this.Flush(r);
                            }

                            n--;
                            b |= (this.Codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        this.table = t = b & 0x3fff;
                        if ((t & 0x1f) > 29 || ((t >> 5) & 0x1f) > 29)
                        {
                            this.mode = InflateBlockMode.Bad;
                            this.Codec.Message = "too many length or distance symbols";
                            r = ZlibConstants.ZDataError;

                            this.Bitb = b;
                            this.Bitk = k;
                            this.Codec.AvailableBytesIn = n;
                            this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                            this.Codec.NextIn = p;
                            this.WriteAt = q;
                            return this.Flush(r);
                        }

                        t = 258 + (t & 0x1f) + ((t >> 5) & 0x1f);
                        if (this.blens == null || this.blens.Length < t)
                        {
                            this.blens = new int[t];
                        }
                        else
                        {
                            Array.Clear(this.blens, 0, t);

                            // for (int i = 0; i < t; i++) { blens[i] = 0; }
                        }

                        b >>= 14;
                        k -= 14;

                        this.index = 0;
                        this.mode = InflateBlockMode.Btree;
                        goto case InflateBlockMode.Btree;

                    case InflateBlockMode.Btree:
                        while (this.index < 4 + (this.table >> 10))
                        {
                            while (k < 3)
                            {
                                if (n != 0)
                                {
                                    r = ZlibConstants.Zok;
                                }
                                else
                                {
                                    this.Bitb = b;
                                    this.Bitk = k;
                                    this.Codec.AvailableBytesIn = n;
                                    this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                    this.Codec.NextIn = p;
                                    this.WriteAt = q;
                                    return this.Flush(r);
                                }

                                n--;
                                b |= (this.Codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            this.blens[Border[this.index++]] = b & 7;

                            b >>= 3;
                            k -= 3;
                        }

                        while (this.index < 19)
                        {
                            this.blens[Border[this.index++]] = 0;
                        }

                        this.bb[0] = 7;
                        t = this.inftree.InflateTreesBits(this.blens, this.bb, this.tb, this.hufts, this.Codec);
                        if (t != ZlibConstants.Zok)
                        {
                            r = t;
                            if (r == ZlibConstants.ZDataError)
                            {
                                this.blens = null;
                                this.mode = InflateBlockMode.Bad;
                            }

                            this.Bitb = b;
                            this.Bitk = k;
                            this.Codec.AvailableBytesIn = n;
                            this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                            this.Codec.NextIn = p;
                            this.WriteAt = q;
                            return this.Flush(r);
                        }

                        this.index = 0;
                        this.mode = InflateBlockMode.Dtree;
                        goto case InflateBlockMode.Dtree;

                    case InflateBlockMode.Dtree:
                        while (true)
                        {
                            t = this.table;
                            if (!(this.index < 258 + (t & 0x1f) + ((t >> 5) & 0x1f)))
                            {
                                break;
                            }

                            t = this.bb[0];

                            while (k < t)
                            {
                                if (n != 0)
                                {
                                    r = ZlibConstants.Zok;
                                }
                                else
                                {
                                    this.Bitb = b;
                                    this.Bitk = k;
                                    this.Codec.AvailableBytesIn = n;
                                    this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                    this.Codec.NextIn = p;
                                    this.WriteAt = q;
                                    return this.Flush(r);
                                }

                                n--;
                                b |= (this.Codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            t = this.hufts[(this.tb[0] + (b & InternalInflateConstants.InflateMask[t])) * 3 + 1];
                            var c = this.hufts[(this.tb[0] + (b & InternalInflateConstants.InflateMask[t])) * 3 + 2];

                            if (c < 16)
                            {
                                b >>= t;
                                k -= t;
                                this.blens[this.index++] = c;
                            }
                            else
                            {
                                // c == 16..18
                                var i = c == 18 ? 7 : c - 14;
                                var j = c == 18 ? 11 : 3;

                                while (k < (t + i))
                                {
                                    if (n != 0)
                                    {
                                        r = ZlibConstants.Zok;
                                    }
                                    else
                                    {
                                        this.Bitb = b;
                                        this.Bitk = k;
                                        this.Codec.AvailableBytesIn = n;
                                        this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                        this.Codec.NextIn = p;
                                        this.WriteAt = q;
                                        return this.Flush(r);
                                    }

                                    n--;
                                    b |= (this.Codec.InputBuffer[p++] & 0xff) << k;
                                    k += 8;
                                }

                                b >>= t;
                                k -= t;

                                j += b & InternalInflateConstants.InflateMask[i];

                                b >>= i;
                                k -= i;

                                i = this.index;
                                t = this.table;
                                if (i + j > 258 + (t & 0x1f) + ((t >> 5) & 0x1f) || (c == 16 && i < 1))
                                {
                                    this.blens = null;
                                    this.mode = InflateBlockMode.Bad;
                                    this.Codec.Message = "invalid bit length repeat";
                                    r = ZlibConstants.ZDataError;

                                    this.Bitb = b;
                                    this.Bitk = k;
                                    this.Codec.AvailableBytesIn = n;
                                    this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                    this.Codec.NextIn = p;
                                    this.WriteAt = q;
                                    return this.Flush(r);
                                }

                                c = (c == 16) ? this.blens[i - 1] : 0;
                                do
                                {
                                    this.blens[i++] = c;
                                }
                                while (--j != 0);
                                this.index = i;
                            }
                        }

                        this.tb[0] = -1;
                        {
                            var bl = new[] { 9 }; // must be <= 9 for lookahead assumptions
                            var bd = new[] { 6 }; // must be <= 9 for lookahead assumptions
                            var tl = new int[1];
                            var td = new int[1];

                            t = this.table;
                            t = this.inftree.InflateTreesDynamic(
                                257 + (t & 0x1f),
                                1 + ((t >> 5) & 0x1f),
                                this.blens,
                                bl,
                                bd,
                                tl,
                                td,
                                this.hufts,
                                this.Codec);

                            if (t != ZlibConstants.Zok)
                            {
                                if (t == ZlibConstants.ZDataError)
                                {
                                    this.blens = null;
                                    this.mode = InflateBlockMode.Bad;
                                }

                                r = t;

                                this.Bitb = b;
                                this.Bitk = k;
                                this.Codec.AvailableBytesIn = n;
                                this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                                this.Codec.NextIn = p;
                                this.WriteAt = q;
                                return this.Flush(r);
                            }

                            this.codes.Init(bl[0], bd[0], this.hufts, tl[0], this.hufts, td[0]);
                        }

                        this.mode = InflateBlockMode.Codes;
                        goto case InflateBlockMode.Codes;

                    case InflateBlockMode.Codes:
                        this.Bitb = b;
                        this.Bitk = k;
                        this.Codec.AvailableBytesIn = n;
                        this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                        this.Codec.NextIn = p;
                        this.WriteAt = q;

                        r = this.codes.Process(this, r);
                        if (r != ZlibConstants.ZStreamEnd)
                        {
                            return this.Flush(r);
                        }

                        r = ZlibConstants.Zok;
                        p = this.Codec.NextIn;
                        n = this.Codec.AvailableBytesIn;
                        b = this.Bitb;
                        k = this.Bitk;
                        q = this.WriteAt;
                        m = q < this.ReadAt ? this.ReadAt - q - 1 : this.End - q;

                        if (this.last == 0)
                        {
                            this.mode = InflateBlockMode.Type;
                            break;
                        }

                        this.mode = InflateBlockMode.Dry;
                        goto case InflateBlockMode.Dry;

                    case InflateBlockMode.Dry:
                        this.WriteAt = q;
                        r = this.Flush(r);
                        q = this.WriteAt;
                        m = q < this.ReadAt ? this.ReadAt - q - 1 : this.End - q;
                        if (this.ReadAt != this.WriteAt)
                        {
                            this.Bitb = b;
                            this.Bitk = k;
                            this.Codec.AvailableBytesIn = n;
                            this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                            this.Codec.NextIn = p;
                            this.WriteAt = q;
                            return this.Flush(r);
                        }

                        this.mode = InflateBlockMode.Done;
                        goto case InflateBlockMode.Done;

                    case InflateBlockMode.Done:
                        r = ZlibConstants.ZStreamEnd;
                        this.Bitb = b;
                        this.Bitk = k;
                        this.Codec.AvailableBytesIn = n;
                        this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                        this.Codec.NextIn = p;
                        this.WriteAt = q;
                        return this.Flush(r);

                    case InflateBlockMode.Bad:
                        r = ZlibConstants.ZDataError;

                        this.Bitb = b;
                        this.Bitk = k;
                        this.Codec.AvailableBytesIn = n;
                        this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                        this.Codec.NextIn = p;
                        this.WriteAt = q;
                        return this.Flush(r);

                    default:
                        r = ZlibConstants.ZStreamError;

                        this.Bitb = b;
                        this.Bitk = k;
                        this.Codec.AvailableBytesIn = n;
                        this.Codec.TotalBytesIn += p - this.Codec.NextIn;
                        this.Codec.NextIn = p;
                        this.WriteAt = q;
                        return this.Flush(r);
                }
            }
        }

        /// <returns>
        /// </returns>
        internal uint Reset()
        {
            var oldCheck = this.check;
            this.mode = InflateBlockMode.Type;
            this.Bitk = 0;
            this.Bitb = 0;
            this.ReadAt = this.WriteAt = 0;

            if (this.checkfn != null)
            {
                this.Codec.Adler32 = this.check = Adler.Adler32(0, null, 0, 0);
            }

            return oldCheck;
        }

        #endregion

        // Table for deflate from PKZIP's appnote.txt.

        // mode independent information

        /*
        internal void SetDictionary(byte[] d, int start, int n)
        {
            Array.Copy(d, start, this.Window, 0, n);
            this.ReadAt = this.WriteAt = n;
        }
*/

        // Returns true if inflate is currently at the end of a block generated by Z_SYNC_FLUSH or Z_FULL_FLUSH.

        /*
        internal int SyncPoint()
        {
            return this.mode == InflateBlockMode.LENS ? 1 : 0;
        }
*/

        // copy as much as possible from the sliding window to the output area
    }
}