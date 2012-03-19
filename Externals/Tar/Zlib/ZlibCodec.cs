// <copyright file="ZlibCodec.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Zlib
{
    using System;
    using System.Globalization;
    using System.IO.Compression;
    using System.Runtime.InteropServices;

    /// <summary>Encoder and Decoder for ZLIB and DEFLATE (IETF RFC1950 and RFC1951).</summary>
    /// <remarks>This class compresses and decompresses data according to the Deflate algorithm and optionally, the ZLIB format, as documented in <see href = "http://www.ietf.org/rfc/rfc1950.txt">RFC 1950 - ZLIB</see> and <see href = "http://www.ietf.org/rfc/rfc1951.txt">RFC 1951 - DEFLATE</see>.</remarks>
    [Guid("ebc25cf6-9120-4283-b972-0e5520d0000D")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public sealed class ZlibCodec
    {
        internal uint Adler32;

        internal DeflateManager Dstate;

        internal InflateManager Istate;

        /// <summary>The compression level to use in this codec.  Useful only in compression mode.</summary>
        private CompressionLevel compressLevel = CompressionLevel.Default; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        /// <summary>The compression strategy to use.</summary>
        /// <remarks>This is only effective in compression.  The theory offered by ZLIB is that different strategies could potentially produce significant differences in compression behavior for different data sets.  Unfortunately I don't have any good recommendations for how to set it differently.  When I tested changing the strategy I got minimally differen compression performance. It's best to leave this property alone if you don't have a good feel for it.  Or, you may want to produce a test harness that runs through the different strategy options and evaluates them on different file types. If you do that, let me know your results.</remarks>
        private CompressionStrategy strategy = CompressionStrategy.Default; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        /// <summary>The number of Window Bits to use.</summary>
        /// <remarks>This gauges the size of the sliding window, and hence the compression effectiveness as well as memory consumption. It's best to just leave this  setting alone if you don't know what it is.  The maximum value is 15 bits, which implies a 32k window.</remarks>
        private int windowBits = ZlibConstants.WindowBitsDefault; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        /// <summary>Create a ZlibCodec.</summary>
        /// <remarks>If you use this default constructor, you will later have to explicitly call InitializeInflate() or InitializeDeflate() before using the ZlibCodec to compress or decompress.</remarks>
        public ZlibCodec()
        {
        }

        /// <summary>Create a ZlibCodec that either compresses or decompresses.</summary>
        /// <param name="mode">Indicates whether the codec should compress (deflate) or decompress (inflate).</param>
        public ZlibCodec(CompressionMode mode)
        {
            if (mode == CompressionMode.Compress)
            {
                int rc = this.InitializeDeflate();
                if (rc != ZlibConstants.Zok)
                {
                    throw new ZlibException("Cannot initialize for deflate.");
                }
            }
            else if (mode == CompressionMode.Decompress)
            {
                int rc = this.InitializeInflate();
                if (rc != ZlibConstants.Zok)
                {
                    throw new ZlibException("Cannot initialize for inflate.");
                }
            }
            else
            {
                throw new ZlibException("Invalid ZlibStreamFlavor.");
            }
        }

        /// <summary>Gets or sets the available bytes in.</summary>
        /// <value>The available bytes in.</value>
        internal int AvailableBytesIn { get; set; }

        /// <summary>Gets or sets the available bytes out.</summary>
        /// <value>The available bytes out.</value>
        internal int AvailableBytesOut { get; set; }

        /// <summary>Gets or sets the input buffer.</summary>
        /// <value>The input buffer.</value>
        internal byte[] InputBuffer { get; set; }

        /// <summary>Gets or sets the message.</summary>
        /// <value>The message.</value>
        internal string Message { get; set; }

        /// <summary>Gets or sets the next in.</summary>
        /// <value>The next in.</value>
        internal int NextIn { get; set; }

        /// <summary>Gets or sets the next out.</summary>
        /// <value>The next out.</value>
        internal int NextOut { get; set; }

        /// <summary>Gets or sets the output buffer.</summary>
        /// <value>The output buffer.</value>
        internal byte[] OutputBuffer { get; set; }

        /// <summary>Gets or sets the strategy.</summary>
        /// <value>The strategy.</value>
        internal CompressionStrategy Strategy
        {
            get { return this.strategy; }

            set { this.strategy = value; }
        }

        /// <summary>Gets or sets the total bytes in.</summary>
        /// <value>The total bytes in.</value><remarks></remarks>
        internal long TotalBytesIn { get; set; }

        /// <summary>Gets or sets the total bytes out.</summary>
        /// <value>The total bytes out.</value><remarks></remarks>
        internal long TotalBytesOut { get; set; }

        /// <summary>Gets or sets the compress level.</summary>
        /// <value>The compress level.</value><remarks></remarks>
        private CompressionLevel CompressLevel
        {
            get { return this.compressLevel; }

            set { this.compressLevel = value; }
        }

        /// <summary>Gets or sets the window bits.</summary>
        /// <value>The window bits.</value><remarks></remarks>
        private int WindowBits
        {
            get { return this.windowBits; }

            set { this.windowBits = value; }
        }

        /// <summary>Deflate one batch of data.</summary>
        /// <remarks>You must have set InputBuffer and OutputBuffer before calling this method.</remarks>
        /// <example>
        ///   <code>
        ///     private void DeflateBuffer(CompressionLevel level) { int bufferSize = 1024; byte[] buffer = new
        ///     byte[bufferSize]; ZlibCodec compressor = new ZlibCodec();
        /// 
        ///     Console.WriteLine("\n============================================"); Console.WriteLine("Size of Buffer
        ///     to Deflate: {0} bytes.", UncompressedBytes.Length); MemoryStream ms = new MemoryStream();
        /// 
        ///     int rc = compressor.InitializeDeflate(level);
        /// 
        ///     compressor.InputBuffer = UncompressedBytes; compressor.NextIn = 0; compressor.AvailableBytesIn =
        ///     UncompressedBytes.Length;
        /// 
        ///     compressor.OutputBuffer = buffer;
        /// 
        ///     // pass 1: deflate do { compressor.NextOut = 0; compressor.AvailableBytesOut = buffer.Length; rc =
        ///     compressor.Deflate(FlushType.None);
        /// 
        ///     if (rc != ZlibConstants.Z_OK &amp;&amp; rc != ZlibConstants.Z_STREAM_END) throw new
        ///     Exception("deflating: " + compressor.Message);
        /// 
        ///     ms.Write(compressor.OutputBuffer, 0, buffer.Length - compressor.AvailableBytesOut); } while
        ///     (compressor.AvailableBytesIn &gt; 0 || compressor.AvailableBytesOut == 0);
        /// 
        ///     // pass 2: finish and flush do { compressor.NextOut = 0; compressor.AvailableBytesOut = buffer.Length;
        ///     rc = compressor.Deflate(FlushType.Finish);
        /// 
        ///     if (rc != ZlibConstants.Z_STREAM_END &amp;&amp; rc != ZlibConstants.Z_OK) throw new
        ///     Exception("deflating: " + compressor.Message);
        /// 
        ///     if (buffer.Length - compressor.AvailableBytesOut &gt; 0) ms.Write(buffer, 0, buffer.Length -
        ///     compressor.AvailableBytesOut); } while (compressor.AvailableBytesIn &gt; 0 ||
        ///     compressor.AvailableBytesOut == 0);
        /// 
        ///     compressor.EndDeflate();
        /// 
        ///     ms.Seek(0, SeekOrigin.Begin); CompressedBytes = new byte[compressor.TotalBytesOut];
        ///     ms.Read(CompressedBytes, 0, CompressedBytes.Length); }
        ///   </code>
        /// </example>
        /// <param name="flush">whether to flush all data as you deflate. Generally you will want to use Z_NO_FLUSH here, in a series of calls to Deflate(), and then call EndDeflate() to flush everything.</param>
        /// <returns>Z_OK if all goes well.</returns>
        internal int Deflate(FlushType flush)
        {
            if (this.Dstate == null)
            {
                throw new ZlibException("No Deflate State!");
            }

            return this.Dstate.Deflate(flush);
        }

        /// <summary>End a deflation session.</summary>
        /// <remarks>Call this after making a series of one or more calls to Deflate(). All buffers are flushed.</remarks>
        /// <returns>Z_OK if all goes well.</returns>
        internal int EndDeflate()
        {
            if (this.Dstate == null)
            {
                throw new ZlibException("No Deflate State!");
            }

            // TODO: dinoch Tue, 03 Nov 2009  15:39 (test this)
            // int ret = dstate.End();
            this.Dstate = null;
            return ZlibConstants.Zok; // ret;
        }

        /// <summary>Ends an inflation session.</summary>
        /// <remarks>Call this after successively calling Inflate().  This will cause all buffers to be flushed.  After calling this you cannot call Inflate() without a intervening call to one of the InitializeInflate() overloads.</remarks>
        /// <returns>Z_OK if everything goes well.</returns>
        internal int EndInflate()
        {
            if (this.Istate == null)
            {
                throw new ZlibException("No Inflate State!");
            }

            int ret = this.Istate.End();
            this.Istate = null;
            return ret;
        }

        /// <exception cref = "ZlibException">
        /// </exception>
        internal void FlushPending()
        {
            int len = this.Dstate.PendingCount;

            if (len > this.AvailableBytesOut)
            {
                len = this.AvailableBytesOut;
            }

            if (len == 0)
            {
                return;
            }

            if (this.Dstate.Pending.Length <= this.Dstate.NextPending || this.OutputBuffer.Length <= this.NextOut
                || this.Dstate.Pending.Length < (this.Dstate.NextPending + len)
                || this.OutputBuffer.Length < (this.NextOut + len))
            {
                throw new ZlibException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        "Invalid State. (pending.Length={0}, pendingCount={1})", 
                        this.Dstate.Pending.Length, 
                        this.Dstate.PendingCount));
            }

            Array.Copy(this.Dstate.Pending, this.Dstate.NextPending, this.OutputBuffer, this.NextOut, len);

            this.NextOut += len;
            this.Dstate.NextPending += len;
            this.TotalBytesOut += len;
            this.AvailableBytesOut -= len;
            this.Dstate.PendingCount -= len;
            if (this.Dstate.PendingCount == 0)
            {
                this.Dstate.NextPending = 0;
            }
        }

        /// <summary>Inflate the data in the InputBuffer, placing the result in the OutputBuffer.</summary>
        /// <remarks>You must have set InputBuffer and OutputBuffer, NextIn and NextOut, and AvailableBytesIn and  AvailableBytesOut  before calling this method.</remarks>
        /// <example>
        ///   <code>
        ///     private void InflateBuffer() { int bufferSize = 1024; byte[] buffer = new byte[bufferSize]; ZlibCodec
        ///     decompressor = new ZlibCodec();
        /// 
        ///     Console.WriteLine("\n============================================"); Console.WriteLine("Size of Buffer
        ///     to Inflate: {0} bytes.", CompressedBytes.Length); MemoryStream ms = new MemoryStream(DecompressedBytes);
        /// 
        ///     int rc = decompressor.InitializeInflate();
        /// 
        ///     decompressor.InputBuffer = CompressedBytes; decompressor.NextIn = 0; decompressor.AvailableBytesIn =
        ///     CompressedBytes.Length;
        /// 
        ///     decompressor.OutputBuffer = buffer;
        /// 
        ///     // pass 1: inflate do { decompressor.NextOut = 0; decompressor.AvailableBytesOut = buffer.Length; rc =
        ///     decompressor.Inflate(FlushType.None);
        /// 
        ///     if (rc != ZlibConstants.Z_OK &amp;&amp; rc != ZlibConstants.Z_STREAM_END) throw new
        ///     Exception("inflating: " + decompressor.Message);
        /// 
        ///     ms.Write(decompressor.OutputBuffer, 0, buffer.Length - decompressor.AvailableBytesOut); } while
        ///     (decompressor.AvailableBytesIn &gt; 0 || decompressor.AvailableBytesOut == 0);
        /// 
        ///     // pass 2: finish and flush do { decompressor.NextOut = 0; decompressor.AvailableBytesOut =
        ///     buffer.Length; rc = decompressor.Inflate(FlushType.Finish);
        /// 
        ///     if (rc != ZlibConstants.Z_STREAM_END &amp;&amp; rc != ZlibConstants.Z_OK) throw new
        ///     Exception("inflating: " + decompressor.Message);
        /// 
        ///     if (buffer.Length - decompressor.AvailableBytesOut &gt; 0) ms.Write(buffer, 0, buffer.Length -
        ///     decompressor.AvailableBytesOut); } while (decompressor.AvailableBytesIn &gt; 0 ||
        ///     decompressor.AvailableBytesOut == 0);
        /// 
        ///     decompressor.EndInflate(); }
        ///   </code>
        /// </example>
        /// <returns>Z_OK if everything goes well.</returns>
        internal int Inflate()
        {
            if (this.Istate == null)
            {
                throw new ZlibException("No Inflate State!");
            }

            return this.Istate.Inflate();
        }

        /// <summary>Initialize the ZlibCodec for deflation operation.</summary>
        /// <remarks>The codec will use the MAX window bits and the default level of compression.</remarks>
        /// <example>
        ///   <code>
        ///     int bufferSize = 40000; byte[] CompressedBytes = new byte[bufferSize]; byte[] DecompressedBytes = new
        ///     byte[bufferSize];
        ///  
        ///     ZlibCodec compressor = new ZlibCodec();
        ///  
        ///     compressor.InitializeDeflate(CompressionLevel.Default);
        ///  
        ///     compressor.InputBuffer = System.Text.ASCIIEncoding.ASCII.GetBytes(TextToCompress); compressor.NextIn =
        ///     0; compressor.AvailableBytesIn = compressor.InputBuffer.Length;
        ///  
        ///     compressor.OutputBuffer = CompressedBytes; compressor.NextOut = 0; compressor.AvailableBytesOut =
        ///     CompressedBytes.Length;
        ///  
        ///     while (compressor.TotalBytesIn != TextToCompress.Length &amp;&amp; compressor.TotalBytesOut &lt;
        ///     bufferSize) { compressor.Deflate(FlushType.None); }
        ///  
        ///     while (true) { int rc= compressor.Deflate(FlushType.Finish); if (rc == ZlibConstants.Z_STREAM_END)
        ///     break; }
        ///  
        ///     compressor.EndDeflate();
        ///   </code>
        /// </example>
        /// <returns>Z_OK if all goes well. You generally don't need to check the return code.</returns>
        internal int InitializeDeflate()
        {
            return this.InternalInitializeDeflate(true);
        }

        /*
        /// <summary>Initialize the ZlibCodec for deflation operation, using the specified CompressionLevel.</summary>
        /// <remarks>The codec will use the maximum window bits (15) and the specifiedCompressionLevel.  It will emit a ZLIB stream as it compresses.</remarks>
        /// <param name="level">The compression level for the codec.</param>
        /// <returns>Z_OK if all goes well.</returns>
        public int InitializeDeflate(CompressionLevel level)
        {
            this.CompressLevel = level;
            return this.InternalInitializeDeflate(true);
        }
*/

        /// <summary>Initialize the ZlibCodec for deflation operation, using the specified CompressionLevel, and the explicit flag governing whether to emit an RFC1950 header byte pair.</summary>
        /// <remarks>The codec will use the maximum window bits (15) and the specified CompressionLevel. If you want to generate a zlib stream, you should specify true for wantRfc1950Header. In this case, the library will emit a ZLIB header, as defined in <see href = "http://www.ietf.org/rfc/rfc1950.txt">RFC 1950</see>, in the compressed stream.</remarks>
        /// <param name="level">The compression level for the codec.</param>
        /// <param name="wantRfc1950Header">whether to emit an initial RFC1950 byte pair in the compressed stream.</param>
        /// <returns>Z_OK if all goes well.</returns>
        internal int InitializeDeflate(CompressionLevel level, bool wantRfc1950Header)
        {
            this.CompressLevel = level;
            return this.InternalInitializeDeflate(wantRfc1950Header);
        }

        /*
        /// <summary>Initialize the ZlibCodec for deflation operation, using the specified CompressionLevel, and the specified number of window bits. </summary>
        /// <remarks>The codec will use the specified number of window bits and the specified CompressionLevel.</remarks>
        /// <param name="level">The compression level for the codec.</param>
        /// <param name="bits">the number of window bits to use.  If you don't know what this means, don't use this method.</param>
        /// <returns>Z_OK if all goes well.</returns>
        public int InitializeDeflate(CompressionLevel level, int bits)
        {
            this.CompressLevel = level;
            this.WindowBits = bits;
            return this.InternalInitializeDeflate(true);
        }
*/

        /*
        /// <summary>Initialize the ZlibCodec for deflation operation, using the specified CompressionLevel, thespecified number of window bits, and the explicit flag governing whether to emit an RFC1950 header bytepair.</summary>
        /// <param name="level">The compression level for the codec.</param>
        /// <param name="bits">the number of window bits to use.  If you don't know what this means, don't use this method.</param>
        /// <param name="wantRfc1950Header">whether to emit an initial RFC1950 byte pair in the compressed stream.</param>
        /// <returns>Z_OK if all goes well.</returns>
        public int InitializeDeflate(CompressionLevel level, int bits, bool wantRfc1950Header)
        {
            this.CompressLevel = level;
            this.WindowBits = bits;
            return this.InternalInitializeDeflate(wantRfc1950Header);
        }
*/

        /// <summary>Initialize the inflation state.</summary>
        /// <remarks>It is not necessary to call this before using the ZlibCodec to inflate data; It is implicitly called when you call the constructor.</remarks>
        /// <returns>Z_OK if everything goes well.</returns>
        internal int InitializeInflate()
        {
            return this.InitializeInflate(this.WindowBits);
        }

        /// <summary>Initialize the inflation state with an explicit flag to govern the handling of RFC1950 header bytes.</summary>
        /// <remarks>By default, the ZLIB header defined in <see href = "http://www.ietf.org/rfc/rfc1950.txt">RFC 1950</see> is expected.  If you want to read a zlib stream you should specify true for expectRfc1950Header.  If you have a deflate stream, you will want to specify false. It is only necessary to invoke this initializer explicitly if you want to specify false.</remarks>
        /// <param name="expectRfc1950Header">whether to expect an RFC1950 header byte pair when reading the stream of data to be inflated.</param>
        /// <returns>Z_OK if everything goes well.</returns>
        internal int InitializeInflate(bool expectRfc1950Header)
        {
            return this.InitializeInflate(this.WindowBits, expectRfc1950Header);
        }

        /// <summary>Initialize the ZlibCodec for inflation, with the specified number of window bits.</summary>
        /// <param name="windowBits">The number of window bits to use. If you need to ask what that is, then you shouldn't be calling this initializer.</param>
        /// <returns>Z_OK if all goes well.</returns>
        internal int InitializeInflate(int windowBits)
        {
            this.WindowBits = windowBits;
            return this.InitializeInflate(windowBits, true);
        }

        /// <summary>Initialize the inflation state with an explicit flag to govern the handling of RFC1950 header bytes.</summary>
        /// <remarks>If you want to read a zlib stream you should specify true for expectRfc1950Header. In this case, the library will expect to find a ZLIB header, as defined in <see href = "http://www.ietf.org/rfc/rfc1950.txt">RFC 1950</see>, in the compressed stream.  If you will be reading a DEFLATE or GZIP stream, which does not have such a header, you will want to specify false.</remarks>
        /// <param name="windowBits">The number of window bits to use. If you need to ask what that is, then you shouldn't be calling this initializer.</param>
        /// <param name="expectRfc1950Header">whether to expect an RFC1950 header byte pair when reading the stream of data to be inflated.</param>
        /// <returns>Z_OK if everything goes well.</returns>
        internal int InitializeInflate(int windowBits, bool expectRfc1950Header)
        {
            this.WindowBits = windowBits;
            if (this.Dstate != null)
            {
                throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");
            }

            this.Istate = new InflateManager(expectRfc1950Header);
            return this.Istate.Initialize(this, windowBits);
        }

        /*
        /// <summary>Reset a codec for another deflation session.</summary>
        /// <remarks>Call this to reset the deflation state.  For example if a thread is deflatingnon-consecutive blocks, you can call Reset() after the Deflate(Sync) of the first block and before the nextDeflate(None) of the second block.</remarks>
        public void ResetDeflate()
        {
            if (this.Dstate == null)
            {
                throw new ZlibException("No Deflate State!");
            }

            this.Dstate.Reset();
        }
*/

        /*
        /// <summary>Set the CompressionStrategy and CompressionLevel for a deflation session.</summary>
        /// <param name="level">the level of compression to use.</param>
        /// <param name="strategy">the strategy to use for compression.</param>
        /// <returns>Z_OK if all goes well.</returns>
        public int SetDeflateParams(CompressionLevel level, CompressionStrategy strategy)
        {
            if (this.Dstate == null)
            {
                throw new ZlibException("No Deflate State!");
            }

            return this.Dstate.SetParams(level, strategy);
        }
*/

        /*
        /// <summary>Set the dictionary to be used for either Inflation or Deflation.</summary>
        /// <param name="dictionary">The dictionary bytes to use.</param>
        /// <returns>Z_OK if all goes well.</returns>
        public int SetDictionary(byte[] dictionary)
        {
            if (this.Istate != null)
            {
                return this.Istate.SetDictionary(dictionary);
            }

            if (this.Dstate != null)
            {
                return this.Dstate.SetDictionary(dictionary);
            }

            throw new ZlibException("No Inflate or Deflate state!");
        }
*/

        // Read a new buffer from the current input stream, update the adler32
        // and total number of bytes read.  All deflate() input goes through
        // this function so some applications may wish to modify it to avoid allocating a large strm->next_in buffer and
        // copying from it. (See also flush_pending()).

        /// <param name="buf"></param><param name="start"></param> <param name="size"></param><returns></returns>
        internal int ReadBuf(byte[] buf, int start, int size)
        {
            int len = this.AvailableBytesIn;

            if (len > size)
            {
                len = size;
            }

            if (len == 0)
            {
                return 0;
            }

            this.AvailableBytesIn -= len;

            if (this.Dstate.WantRfc1950HeaderBytes)
            {
                this.Adler32 = Adler.Adler32(this.Adler32, this.InputBuffer, this.NextIn, len);
            }

            Array.Copy(this.InputBuffer, this.NextIn, buf, start, len);
            this.NextIn += len;
            this.TotalBytesIn += len;
            return len;
        }

        /// <param name="wantRfc1950Header">
        /// </param>
        /// <returns></returns> <exception cref = "ZlibException"></exception>
        private int InternalInitializeDeflate(bool wantRfc1950Header)
        {
            if (this.Istate != null)
            {
                throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");
            }

            this.Dstate = new DeflateManager { WantRfc1950HeaderBytes = wantRfc1950Header };

            return this.Dstate.Initialize(this, this.CompressLevel, this.WindowBits, this.Strategy);
        }

        /*
        /// <summary>The Adler32 checksum on the data transferred through the codec so far. You probably don't need to look at this.</summary>
        public int Adler32
        {
            get
            {
                return (int)this._Adler32;
            }
        }
*/

        /*
        /// <summary>I don't know what this does!</summary>
        /// <returns>Z_OK if everything goes well.</returns>
        public int SyncInflate()
        {
            if (this.Istate == null)
            {
                throw new ZlibException("No Inflate State!");
            }

            return this.Istate.Sync();
        }
*/

        // Flush as much pending output as possible. All deflate() output goes through this function so some
        // applications may wish to modify it to avoid allocating a large strm->next_out buffer and copying into it.
        // (See also read_buf()).
    }
}