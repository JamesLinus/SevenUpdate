//-----------------------------------------------------------------------
// <copyright file="InflateCodes.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
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
    internal sealed class InflateCodes
    {
        #region Constants and Fields

        private const int Badcode = 9; // x: got error

        private const int Copy = 5; // o: copying bytes in window, waiting for space

        private const int Dist = 3; // i: get distance next

        private const int Distext = 4; // i: getting distance extra

        private const int End = 8; // x: got eob and all data flushed

        private const int Len = 1; // i: get length/literal/eob next

        private const int Lenext = 2; // i: getting length extra (have base)

        private const int Lit = 6; // o: got literal, waiting for output space

        private const int Start = 0; // x: set up for LEN

        private const int Wash = 7; // o: got eob, possibly still output waiting

        private int bitsToGet; // bits to get for extra

        private byte dbits; // dtree bits decoder per branch

        private int dist; // distance back to copy from

        private int[] dtree; // distance tree

        private int dtreeIndex; // distance tree

        private byte lbits; // ltree bits decoded per branch

        private int len;

        private int lit;

        private int[] ltree; // literal/length/eob tree

        private int ltreeIndex; // literal/length/eob tree

        private int mode; // current inflate_codes mode

        private int need; // bits needed

        private int[] tree; // pointer into tree

        private int treeIndex;

        #endregion

        #region Methods

        internal void Init(int bl, int bd, int[] tl, int tlIndex, int[] td, int tdIndex)
        {
            this.mode = Start;
            this.lbits = (byte)bl;
            this.dbits = (byte)bd;
            this.ltree = tl;
            this.ltreeIndex = tlIndex;
            this.dtree = td;
            this.dtreeIndex = tdIndex;
            this.tree = null;
        }

        internal int Process(InflateBlocks blocks, int r)
        {
            var z = blocks.Codec;

            // copy input/output information to locals (UPDATE macro restores)
            var p = z.NextIn; // input data pointer
            var n = z.AvailableBytesIn;
            var b = blocks.Bitb; // bit buffer
            var k = blocks.Bitk; // bits in bit buffer
            var q = blocks.WriteAt;
            var m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;

            // process input and output based on current state
            while (true)
            {
                int tindex; // temporary pointer
                int j; // temporary storage
                int e; // extra bits or operation
                switch (this.mode)
                {
                        // waiting for "i:"=input, "o:"=output, "x:"=nothing
                    case Start: // x: set up for LEN
                        if (m >= 258 && n >= 10)
                        {
                            blocks.Bitb = b;
                            blocks.Bitk = k;
                            z.AvailableBytesIn = n;
                            z.TotalBytesIn += p - z.NextIn;
                            z.NextIn = p;
                            blocks.WriteAt = q;
                            r = InflateFast(
                                this.lbits,
                                this.dbits,
                                this.ltree,
                                this.ltreeIndex,
                                this.dtree,
                                this.dtreeIndex,
                                blocks,
                                z);

                            p = z.NextIn;
                            n = z.AvailableBytesIn;
                            b = blocks.Bitb;
                            k = blocks.Bitk;
                            q = blocks.WriteAt;
                            m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;

                            if (r != ZlibConstants.Zok)
                            {
                                this.mode = (r == ZlibConstants.ZStreamEnd) ? Wash : Badcode;
                                break;
                            }
                        }

                        this.need = this.lbits;
                        this.tree = this.ltree;
                        this.treeIndex = this.ltreeIndex;

                        this.mode = Len;
                        goto case Len;

                    case Len: // i: get length/literal/eob next
                        j = this.need;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Zok;
                            }
                            else
                            {
                                blocks.Bitb = b;
                                blocks.Bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.WriteAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        tindex = (this.treeIndex + (b & InternalInflateConstants.InflateMask[j])) * 3;

                        b >>= this.tree[tindex + 1];
                        k -= this.tree[tindex + 1];

                        e = this.tree[tindex];

                        if (e == 0)
                        {
                            // literal
                            this.lit = this.tree[tindex + 2];
                            this.mode = Lit;
                            break;
                        }

                        if ((e & 16) != 0)
                        {
                            // length
                            this.bitsToGet = e & 15;
                            this.len = this.tree[tindex + 2];
                            this.mode = Lenext;
                            break;
                        }

                        if ((e & 64) == 0)
                        {
                            // next table
                            this.need = e;
                            this.treeIndex = tindex / 3 + this.tree[tindex + 2];
                            break;
                        }

                        if ((e & 32) != 0)
                        {
                            // end of block
                            this.mode = Wash;
                            break;
                        }

                        this.mode = Badcode; // invalid code
                        z.Message = "invalid literal/length code";
                        r = ZlibConstants.ZDataError;

                        blocks.Bitb = b;
                        blocks.Bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.WriteAt = q;
                        return blocks.Flush(r);

                    case Lenext: // i: getting length extra (have base)
                        j = this.bitsToGet;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Zok;
                            }
                            else
                            {
                                blocks.Bitb = b;
                                blocks.Bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.WriteAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        this.len += b & InternalInflateConstants.InflateMask[j];

                        b >>= j;
                        k -= j;

                        this.need = this.dbits;
                        this.tree = this.dtree;
                        this.treeIndex = this.dtreeIndex;
                        this.mode = Dist;
                        goto case Dist;

                    case Dist: // i: get distance next
                        j = this.need;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Zok;
                            }
                            else
                            {
                                blocks.Bitb = b;
                                blocks.Bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.WriteAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        tindex = (this.treeIndex + (b & InternalInflateConstants.InflateMask[j])) * 3;

                        b >>= this.tree[tindex + 1];
                        k -= this.tree[tindex + 1];

                        e = this.tree[tindex];
                        if ((e & 0x10) != 0)
                        {
                            // distance
                            this.bitsToGet = e & 15;
                            this.dist = this.tree[tindex + 2];
                            this.mode = Distext;
                            break;
                        }

                        if ((e & 64) == 0)
                        {
                            // next table
                            this.need = e;
                            this.treeIndex = tindex / 3 + this.tree[tindex + 2];
                            break;
                        }

                        this.mode = Badcode; // invalid code
                        z.Message = "invalid distance code";
                        r = ZlibConstants.ZDataError;

                        blocks.Bitb = b;
                        blocks.Bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.WriteAt = q;
                        return blocks.Flush(r);

                    case Distext: // i: getting distance extra
                        j = this.bitsToGet;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Zok;
                            }
                            else
                            {
                                blocks.Bitb = b;
                                blocks.Bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.WriteAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        this.dist += b & InternalInflateConstants.InflateMask[j];

                        b >>= j;
                        k -= j;

                        this.mode = Copy;
                        goto case Copy;

                    case Copy: // o: copying bytes in window, waiting for space
                        var f = q - this.dist; // pointer to copy strings from
                        while (f < 0)
                        {
                            // modulo window size-"while" instead
                            f += blocks.End; // of "if" handles invalid distances
                        }

                        while (this.len != 0)
                        {
                            if (m == 0)
                            {
                                if (q == blocks.End && blocks.ReadAt != 0)
                                {
                                    q = 0;
                                    m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;
                                }

                                if (m == 0)
                                {
                                    blocks.WriteAt = q;
                                    r = blocks.Flush(r);
                                    q = blocks.WriteAt;
                                    m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;

                                    if (q == blocks.End && blocks.ReadAt != 0)
                                    {
                                        q = 0;
                                        m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;
                                    }

                                    if (m == 0)
                                    {
                                        blocks.Bitb = b;
                                        blocks.Bitk = k;
                                        z.AvailableBytesIn = n;
                                        z.TotalBytesIn += p - z.NextIn;
                                        z.NextIn = p;
                                        blocks.WriteAt = q;
                                        return blocks.Flush(r);
                                    }
                                }
                            }

                            blocks.Window[q++] = blocks.Window[f++];
                            m--;

                            if (f == blocks.End)
                            {
                                f = 0;
                            }

                            this.len--;
                        }

                        this.mode = Start;
                        break;

                    case Lit: // o: got literal, waiting for output space
                        if (m == 0)
                        {
                            if (q == blocks.End && blocks.ReadAt != 0)
                            {
                                q = 0;
                                m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;
                            }

                            if (m == 0)
                            {
                                blocks.WriteAt = q;
                                r = blocks.Flush(r);
                                q = blocks.WriteAt;
                                m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;

                                if (q == blocks.End && blocks.ReadAt != 0)
                                {
                                    q = 0;
                                    m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;
                                }

                                if (m == 0)
                                {
                                    blocks.Bitb = b;
                                    blocks.Bitk = k;
                                    z.AvailableBytesIn = n;
                                    z.TotalBytesIn += p - z.NextIn;
                                    z.NextIn = p;
                                    blocks.WriteAt = q;
                                    return blocks.Flush(r);
                                }
                            }
                        }

                        r = ZlibConstants.Zok;

                        blocks.Window[q++] = (byte)this.lit;
                        m--;

                        this.mode = Start;
                        break;

                    case Wash: // o: got eob, possibly more output
                        if (k > 7)
                        {
                            // return unused byte, if any
                            k -= 8;
                            n++;
                            p--; // can always return one
                        }

                        blocks.WriteAt = q;
                        r = blocks.Flush(r);
                        q = blocks.WriteAt;
                        m = q < blocks.ReadAt ? blocks.ReadAt - q - 1 : blocks.End - q;

                        if (blocks.ReadAt != blocks.WriteAt)
                        {
                            blocks.Bitb = b;
                            blocks.Bitk = k;
                            z.AvailableBytesIn = n;
                            z.TotalBytesIn += p - z.NextIn;
                            z.NextIn = p;
                            blocks.WriteAt = q;
                            return blocks.Flush(r);
                        }

                        this.mode = End;
                        goto case End;

                    case End:
                        r = ZlibConstants.ZStreamEnd;
                        blocks.Bitb = b;
                        blocks.Bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.WriteAt = q;
                        return blocks.Flush(r);

                    case Badcode: // x: got error

                        r = ZlibConstants.ZDataError;

                        blocks.Bitb = b;
                        blocks.Bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.WriteAt = q;
                        return blocks.Flush(r);

                    default:
                        r = ZlibConstants.ZStreamError;

                        blocks.Bitb = b;
                        blocks.Bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.WriteAt = q;
                        return blocks.Flush(r);
                }
            }
        }

        private static int InflateFast(
            int bl, int bd, int[] tl, int tlIndex, int[] td, int tdIndex, InflateBlocks s, ZlibCodec z)
        {
            int c; // bytes to copy

            // load input, output, bit values
            var p = z.NextIn;
            var n = z.AvailableBytesIn;
            var b = s.Bitb;
            var k = s.Bitk;
            var q = s.WriteAt;
            var m = q < s.ReadAt ? s.ReadAt - q - 1 : s.End - q;

            // initialize masks
            var ml = InternalInflateConstants.InflateMask[bl];
            var md = InternalInflateConstants.InflateMask[bd];

            // do until not enough input or output space for fast loop
            do
            {
                // assume called with m >= 258 && n >= 10 get literal/length code
                while (k < 20)
                {
                    // max bits for literal/length code
                    n--;
                    b |= (z.InputBuffer[p++] & 0xff) << k;
                    k += 8;
                }

                var t = b & ml; // temporary pointer
                var tp = tl; // temporary pointer
                var tpIndex = tlIndex; // temporary pointer
                var tpIndexT3 = (tpIndex + t) * 3; // (tp_index+t)*3
                int e; // extra bits or operation
                if ((e = tp[tpIndexT3]) == 0)
                {
                    b >>= tp[tpIndexT3 + 1];
                    k -= tp[tpIndexT3 + 1];

                    s.Window[q++] = (byte)tp[tpIndexT3 + 2];
                    m--;
                    continue;
                }

                do
                {
                    b >>= tp[tpIndexT3 + 1];
                    k -= tp[tpIndexT3 + 1];

                    if ((e & 16) != 0)
                    {
                        e &= 15;
                        c = tp[tpIndexT3 + 2] + (b & InternalInflateConstants.InflateMask[e]);

                        b >>= e;
                        k -= e;

                        // decode distance base of block to copy
                        while (k < 15)
                        {
                            // max bits for distance code
                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        t = b & md;
                        tp = td;
                        tpIndex = tdIndex;
                        tpIndexT3 = (tpIndex + t) * 3;
                        e = tp[tpIndexT3];

                        do
                        {
                            b >>= tp[tpIndexT3 + 1];
                            k -= tp[tpIndexT3 + 1];

                            if ((e & 16) != 0)
                            {
                                // get extra bits to add to distance base
                                e &= 15;
                                while (k < e)
                                {
                                    // get extra bits (up to 13)
                                    n--;
                                    b |= (z.InputBuffer[p++] & 0xff) << k;
                                    k += 8;
                                }

                                var d = tp[tpIndexT3 + 2] + (b & InternalInflateConstants.InflateMask[e]);

                                // distance back to copy from
                                b >>= e;
                                k -= e;

                                // do the copy
                                m -= c;
                                int r; // copy source pointer
                                if (q >= d)
                                {
                                    // offset before dest just copy
                                    r = q - d;
                                    if (q - r > 0 && 2 > (q - r))
                                    {
                                        s.Window[q++] = s.Window[r++]; // minimum count is three,
                                        s.Window[q++] = s.Window[r++]; // so unroll loop a little
                                        c -= 2;
                                    }
                                    else
                                    {
                                        Array.Copy(s.Window, r, s.Window, q, 2);
                                        q += 2;
                                        r += 2;
                                        c -= 2;
                                    }
                                }
                                else
                                {
                                    // else offset after destination
                                    r = q - d;
                                    do
                                    {
                                        r += s.End; // force pointer in window
                                    }
                                    while (r < 0); // covers invalid distances
                                    e = s.End - r;
                                    if (c > e)
                                    {
                                        // if source crosses,
                                        c -= e; // wrapped copy
                                        if (q - r > 0 && e > (q - r))
                                        {
                                            do
                                            {
                                                s.Window[q++] = s.Window[r++];
                                            }
                                            while (--e != 0);
                                        }
                                        else
                                        {
                                            Array.Copy(s.Window, r, s.Window, q, e);
                                            q += e;
                                            r += e;
                                            e = 0;
                                        }

                                        r = 0; // copy rest from start of window
                                    }
                                }

                                // copy all or what's left
                                if (q - r > 0 && c > (q - r))
                                {
                                    do
                                    {
                                        s.Window[q++] = s.Window[r++];
                                    }
                                    while (--c != 0);
                                }
                                else
                                {
                                    Array.Copy(s.Window, r, s.Window, q, c);
                                    q += c;
                                    r += c;
                                    c = 0;
                                }

                                break;
                            }

                            if ((e & 64) == 0)
                            {
                                t += tp[tpIndexT3 + 2];
                                t += b & InternalInflateConstants.InflateMask[e];
                                tpIndexT3 = (tpIndex + t) * 3;
                                e = tp[tpIndexT3];
                            }
                            else
                            {
                                z.Message = "invalid distance code";

                                c = z.AvailableBytesIn - n;
                                c = (k >> 3) < c ? k >> 3 : c;
                                n += c;
                                p -= c;
                                k -= c << 3;

                                s.Bitb = b;
                                s.Bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                s.WriteAt = q;

                                return ZlibConstants.ZDataError;
                            }
                        }
                        while (true);
                        break;
                    }

                    if ((e & 64) == 0)
                    {
                        t += tp[tpIndexT3 + 2];
                        t += b & InternalInflateConstants.InflateMask[e];
                        tpIndexT3 = (tpIndex + t) * 3;
                        if ((e = tp[tpIndexT3]) == 0)
                        {
                            b >>= tp[tpIndexT3 + 1];
                            k -= tp[tpIndexT3 + 1];
                            s.Window[q++] = (byte)tp[tpIndexT3 + 2];
                            m--;
                            break;
                        }
                    }
                    else if ((e & 32) != 0)
                    {
                        c = z.AvailableBytesIn - n;
                        c = (k >> 3) < c ? k >> 3 : c;
                        n += c;
                        p -= c;
                        k -= c << 3;

                        s.Bitb = b;
                        s.Bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        s.WriteAt = q;

                        return ZlibConstants.ZStreamEnd;
                    }
                    else
                    {
                        z.Message = "invalid literal/length code";

                        c = z.AvailableBytesIn - n;
                        c = (k >> 3) < c ? k >> 3 : c;
                        n += c;
                        p -= c;
                        k -= c << 3;

                        s.Bitb = b;
                        s.Bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        s.WriteAt = q;

                        return ZlibConstants.ZDataError;
                    }
                }
                while (true);
            }
            while (m >= 258 && n >= 10);

            // not enough input or output--restore pointers and return
            c = z.AvailableBytesIn - n;
            c = (k >> 3) < c ? k >> 3 : c;
            n += c;
            p -= c;
            k -= c << 3;

            s.Bitb = b;
            s.Bitk = k;
            z.AvailableBytesIn = n;
            z.TotalBytesIn += p - z.NextIn;
            z.NextIn = p;
            s.WriteAt = q;

            return ZlibConstants.Zok;
        }

        #endregion

        // waiting for "i:"=input, "o:"=output, "x:"=nothing

        // if EXT or COPY, where and how much

        // Called with number of bytes left to write in window at least 258 (the maximum string length) and number of
        // input bytes available
        // at least ten.  The ten bytes are six bytes for the longest length/
        // distance pair plus four bytes for overloading the bit buffer.
    }
}
