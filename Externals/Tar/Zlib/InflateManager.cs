//-----------------------------------------------------------------------
// <copyright file="InflateManager.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    using System.Globalization;

    /// <summary>
    /// </summary>
    internal sealed class InflateManager
    {
        #region Constants and Fields

        private const int PresetDict = 0x20;

        private const int ZDeflated = 8;

        private InflateBlocks blocks; // current inflate_blocks state

        private ZlibCodec codec; // pointer back to this zlib stream

        // mode dependent information

        // if CHECK, check values to compare

        private uint computedCheck; // computed check value

        private uint expectedCheck; // stream check value

        // if BAD, inflateSync's marker bytes count

        /*
        private static readonly byte[] Mark = new byte[] { 0, 0, 0xff, 0xff };
*/

        private bool handleRfc1950HeaderBytes = true;

        private int method; // if FLAGS, method byte

        private InflateManagerMode mode; // current inflate mode

        private int wbits; // log2(window size)  (8..15, defaults to 15)

        #endregion

        #region Constructors and Destructors

        public InflateManager(bool expectRfc1950HeaderBytes)
        {
            this.handleRfc1950HeaderBytes = expectRfc1950HeaderBytes;
        }

        #endregion

        #region Enums

        private enum InflateManagerMode
        {
            /// <summary>
            /// </summary>
            Method = 0, // waiting for method byte

            /// <summary>
            /// </summary>
            Flag = 1, // waiting for flag byte

            /// <summary>
            /// </summary>
            DicT4 = 2, // four dictionary check bytes to go

            /// <summary>
            /// </summary>
            DicT3 = 3, // three dictionary check bytes to go

            /// <summary>
            /// </summary>
            DicT2 = 4, // two dictionary check bytes to go

            /// <summary>
            /// </summary>
            DicT1 = 5, // one dictionary check byte to go

            /// <summary>
            /// </summary>
            DicT0 = 6, // waiting for inflateSetDictionary

            /// <summary>
            /// </summary>
            Blocks = 7, // decompressing blocks

            /// <summary>
            /// </summary>
            ChecK4 = 8, // four check bytes to go

            /// <summary>
            /// </summary>
            ChecK3 = 9, // three check bytes to go

            /// <summary>
            /// </summary>
            ChecK2 = 10, // two check bytes to go

            /// <summary>
            /// </summary>
            ChecK1 = 11, // one check byte to go

            /// <summary>
            /// </summary>
            Done = 12, // finished check, done

            /// <summary>
            /// </summary>
            Bad = 13, // got an error--stay here
        }

        #endregion

        #region Properties

        internal bool HandleRfc1950HeaderBytes
        {
            get
            {
                return this.handleRfc1950HeaderBytes;
            }

            set
            {
                this.handleRfc1950HeaderBytes = value;
            }
        }

        #endregion

        #region Methods

        /// <returns>
        /// </returns>
        internal int End()
        {
            if (this.blocks != null)
            {
                this.blocks.Free();
            }

            this.blocks = null;
            return ZlibConstants.Zok;
        }

        /// <returns></returns> <exception cref = "ZlibException"></exception><exception cref = "ZlibException"></exception><exception cref = "ZlibException"></exception>
        internal int Inflate()
        {
            if (this.codec.InputBuffer == null)
            {
                throw new ZlibException("InputBuffer is null. ");
            }

            // int f = (flush == FlushType.Finish) ? ZlibConstants.Z_BUF_ERROR : ZlibConstants.Z_OK;

            // workitem 8870
            const int f = ZlibConstants.Zok;
            var r = ZlibConstants.ZBufError;

            while (true)
            {
                switch (this.mode)
                {
                    case InflateManagerMode.Method:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        if (((this.method = this.codec.InputBuffer[this.codec.NextIn++]) & 0xf) != ZDeflated)
                        {
                            this.mode = InflateManagerMode.Bad;
                            this.codec.Message = string.Format(
                                CultureInfo.CurrentCulture, "unknown compression method (0x{0:X2})", this.method);
                            break;
                        }

                        if ((this.method >> 4) + 8 > this.wbits)
                        {
                            this.mode = InflateManagerMode.Bad;
                            this.codec.Message = string.Format(
                                CultureInfo.CurrentCulture, "invalid window size ({0})", (this.method >> 4) + 8);
                            break;
                        }

                        this.mode = InflateManagerMode.Flag;
                        break;

                    case InflateManagerMode.Flag:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        var b = this.codec.InputBuffer[this.codec.NextIn++] & 0xff;

                        if ((((this.method << 8) + b) % 31) != 0)
                        {
                            this.mode = InflateManagerMode.Bad;
                            this.codec.Message = "incorrect header check";
                            break;
                        }

                        this.mode = ((b & PresetDict) == 0) ? InflateManagerMode.Blocks : InflateManagerMode.DicT4;
                        break;

                    case InflateManagerMode.DicT4:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck = (uint)((this.codec.InputBuffer[this.codec.NextIn++] << 24) & 0xff000000);
                        this.mode = InflateManagerMode.DicT3;
                        break;

                    case InflateManagerMode.DicT3:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck += (uint)((this.codec.InputBuffer[this.codec.NextIn++] << 16) & 0x00ff0000);
                        this.mode = InflateManagerMode.DicT2;
                        break;

                    case InflateManagerMode.DicT2:

                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck += (uint)((this.codec.InputBuffer[this.codec.NextIn++] << 8) & 0x0000ff00);
                        this.mode = InflateManagerMode.DicT1;
                        break;

                    case InflateManagerMode.DicT1:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck += (uint)(this.codec.InputBuffer[this.codec.NextIn++] & 0x000000ff);
                        this.codec.Adler32 = this.expectedCheck;
                        this.mode = InflateManagerMode.DicT0;
                        return ZlibConstants.ZNeedDict;

                    case InflateManagerMode.DicT0:
                        this.mode = InflateManagerMode.Bad;
                        this.codec.Message = "need dictionary";
                        return ZlibConstants.ZStreamError;

                    case InflateManagerMode.Blocks:
                        r = this.blocks.Process(r);
                        if (r == ZlibConstants.ZDataError)
                        {
                            this.mode = InflateManagerMode.Bad;
                            break;
                        }

                        if (r == ZlibConstants.Zok)
                        {
                            r = f;
                        }

                        if (r != ZlibConstants.ZStreamEnd)
                        {
                            return r;
                        }

                        r = f;
                        this.computedCheck = this.blocks.Reset();
                        if (!this.HandleRfc1950HeaderBytes)
                        {
                            this.mode = InflateManagerMode.Done;
                            return ZlibConstants.ZStreamEnd;
                        }

                        this.mode = InflateManagerMode.ChecK4;
                        break;

                    case InflateManagerMode.ChecK4:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck = (uint)((this.codec.InputBuffer[this.codec.NextIn++] << 24) & 0xff000000);
                        this.mode = InflateManagerMode.ChecK3;
                        break;

                    case InflateManagerMode.ChecK3:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck += (uint)((this.codec.InputBuffer[this.codec.NextIn++] << 16) & 0x00ff0000);
                        this.mode = InflateManagerMode.ChecK2;
                        break;

                    case InflateManagerMode.ChecK2:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck += (uint)((this.codec.InputBuffer[this.codec.NextIn++] << 8) & 0x0000ff00);
                        this.mode = InflateManagerMode.ChecK1;
                        break;

                    case InflateManagerMode.ChecK1:
                        if (this.codec.AvailableBytesIn == 0)
                        {
                            return r;
                        }

                        r = f;
                        this.codec.AvailableBytesIn--;
                        this.codec.TotalBytesIn++;
                        this.expectedCheck += (uint)(this.codec.InputBuffer[this.codec.NextIn++] & 0x000000ff);
                        if (this.computedCheck != this.expectedCheck)
                        {
                            this.mode = InflateManagerMode.Bad;
                            this.codec.Message = "incorrect data check";
                            break;
                        }

                        this.mode = InflateManagerMode.Done;
                        return ZlibConstants.ZStreamEnd;

                    case InflateManagerMode.Done:
                        return ZlibConstants.ZStreamEnd;

                    case InflateManagerMode.Bad:
                        throw new ZlibException(
                            string.Format(CultureInfo.CurrentCulture, "Bad state ({0})", this.codec.Message));

                    default:
                        throw new ZlibException("Stream error.");
                }
            }
        }

        /// <param name="codec">
        /// </param><param name="w"></param>
        /// <returns></returns> <exception cref = "ZlibException"></exception>
        internal int Initialize(ZlibCodec codec, int w)
        {
            this.codec = codec;
            this.codec.Message = null;
            this.blocks = null;

            // handle undocumented nowrap option (no zlib header or check) nowrap = 0; if (w < 0) { w = - w; nowrap = 1;
            // }

            // set window size
            if (w < 8 || w > 15)
            {
                this.End();
                throw new ZlibException("Bad window size.");

                // return ZlibConstants.Z_STREAM_ERROR;
            }

            this.wbits = w;

            this.blocks = new InflateBlocks(codec, this.HandleRfc1950HeaderBytes ? this : null, 1 << w);

            // reset state
            this.Reset();
            return ZlibConstants.Zok;
        }

        /// <returns>
        /// </returns>
        private int Reset()
        {
            this.codec.TotalBytesIn = this.codec.TotalBytesOut = 0;
            this.codec.Message = null;
            this.mode = this.HandleRfc1950HeaderBytes ? InflateManagerMode.Method : InflateManagerMode.Blocks;
            this.blocks.Reset();
            return ZlibConstants.Zok;
        }

        #endregion

        // preset dictionary flag in zlib header

        /*
        public InflateManager()
        {
        }
*/

        /*
        
        internal int SetDictionary(byte[] dictionary)
        {
            var index = 0;
            var length = dictionary.Length;
            if (this.mode != InflateManagerMode.DicT0)
            {
                throw new ZlibException("Stream error.");
            }

            if (Adler.Adler32(1, dictionary, 0, dictionary.Length) != this.Codec._Adler32)
            {
                return ZlibConstants.ZDataError;
            }

            this.Codec._Adler32 = Adler.Adler32(0, null, 0, 0);

            if (length >= (1 << this.Wbits))
            {
                length = (1 << this.Wbits) - 1;
                index = dictionary.Length - length;
            }

            this.Blocks.SetDictionary(dictionary, index, length);
            this.mode = InflateManagerMode.BLOCKS;
            return ZlibConstants.Zok;
        }
*/

        /*
        internal int Sync()
        {
            int n; // number of bytes to look at

            // set up
            if (this.mode != InflateManagerMode.Bad)
            {
                this.mode = InflateManagerMode.Bad;
                this.Marker = 0;
            }

            if ((n = this.Codec.AvailableBytesIn) == 0)
            {
                return ZlibConstants.ZBufError;
            }

            var p = this.Codec.NextIn;
            var m = this.Marker;

            // search
            while (n != 0 && m < 4)
            {
                if (this.Codec.GetInputBuffer()[p] == Mark[m])
                {
                    m++;
                }
                else if (this.Codec.GetInputBuffer()[p] != 0)
                {
                    m = 0;
                }
                else
                {
                    m = 4 - m;
                }

                p++;
                n--;
            }

            // restore
            this.Codec.TotalBytesIn += p - this.Codec.NextIn;
            this.Codec.NextIn = p;
            this.Codec.AvailableBytesIn = n;
            this.Marker = m;

            // return no joy or set up to restart on a new block
            if (m != 4)
            {
                return ZlibConstants.ZDataError;
            }

            var r = this.Codec.TotalBytesIn;
            var w = this.Codec.TotalBytesOut;
            this.Reset();
            this.Codec.TotalBytesIn = r;
            this.Codec.TotalBytesOut = w;
            this.mode = InflateManagerMode.BLOCKS;
            return ZlibConstants.Zok;
        }
*/

        /*
        /// <summary>This function is used by one PPP implementation to provide an additional safety check. PPP uses Z_SYNC_FLUSH but removes the length bytes of the resulting empty stored block. When decompressing, PPP checks that at the end of input packet, inflate is waiting for these length bytes.</summary>
        /// <returns>Returns true if inflate is currently at the end of a block generated by Z_SYNC_FLUSH or Z_FULL_FLUSH.</returns>
        internal int SyncPoint()
        {
            return this.Blocks.SyncPoint();
        }
*/
    }
}
