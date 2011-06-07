//-----------------------------------------------------------------------
// <copyright file="ZlibBaseStream.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// </summary>
    internal class ZlibBaseStream : Stream
    {
        #region Constants and Fields

        protected internal string GzipComment;

        protected internal string GzipFileName;

        protected internal int GzipHeaderByteCount;

        protected internal StreamMode Mode = StreamMode.Undefined;

        protected internal Stream Stream;

        protected internal ZlibCodec ZlibCodec; // deferred init... new ZlibCodec();

        private const int BufferSize = ZlibConstants.WorkingBufferSizeDefault;

        private const CompressionStrategy Strategy = CompressionStrategy.Default;

        private readonly byte[] buf1 = new byte[1];

        private readonly CompressionMode compressionMode;

        private readonly Crc32 crc;

        private readonly ZlibStreamFlavor flavor;

        private readonly FlushType flushMode;

        private readonly bool leaveOpen;

        private readonly CompressionLevel level;

        private bool nomoreinput;

        private byte[] workBuffer;

        #endregion

        #region Constructors and Destructors

        /// <param name="stream">
        /// </param>
        /// <param name="compressionMode">
        /// </param>
        /// <param name="level">
        /// </param>
        /// <param name="flavor">
        /// </param>
        /// <param name="leaveOpen">
        /// </param>
        public ZlibBaseStream(
            Stream stream,
            CompressionMode compressionMode,
            CompressionLevel level,
            ZlibStreamFlavor flavor,
            bool leaveOpen)
        {
            this.flushMode = FlushType.None;

            // this._workingBuffer = new byte[WORKING_BUFFER_SIZE_DEFAULT];
            this.Stream = stream;
            this.leaveOpen = leaveOpen;
            this.compressionMode = compressionMode;
            this.flavor = flavor;
            this.level = level;

            // workitem 7159
            if (flavor == ZlibStreamFlavor.Gzip)
            {
                this.crc = new Crc32();
            }
        }

        #endregion

        #region Enums

        internal enum StreamMode
        {
            /// <summary>
            /// </summary>
            Writer,

            /// <summary>
            /// </summary>
            Reader,

            /// <summary>
            /// </summary>
            Undefined,
        }

        #endregion

        #region Properties

        public override bool CanRead
        {
            get
            {
                return this.Stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.Stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.Stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this.Stream.Length;
            }
        }

        /// <exception cref = "NotImplementedException">
        /// </exception>
        /// <exception cref = "NotImplementedException">
        /// </exception>
        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        internal int Crc32
        {
            get
            {
                return this.crc == null ? 0 : this.crc.Crc32Result;
            }
        }

        protected internal bool WantCompress
        {
            get
            {
                return this.compressionMode == CompressionMode.Compress;
            }
        }

        private byte[] WorkingBuffer
        {
            get
            {
                return this.workBuffer ?? (this.workBuffer = new byte[BufferSize]);
            }
        }

        private ZlibCodec Z
        {
            get
            {
                if (this.ZlibCodec == null)
                {
                    var wantRfc1950Header = this.flavor == ZlibStreamFlavor.Zlib;
                    this.ZlibCodec = new ZlibCodec();
                    if (this.compressionMode == CompressionMode.Decompress)
                    {
                        this.ZlibCodec.InitializeInflate(wantRfc1950Header);
                    }
                    else
                    {
                        this.ZlibCodec.Strategy = Strategy;
                        this.ZlibCodec.InitializeDeflate(this.level, wantRfc1950Header);
                    }
                }

                return this.ZlibCodec;
            }
        }

        #endregion

        #region Public Methods

        public override void Close()
        {
            if (this.Stream == null)
            {
                return;
            }

            try
            {
                this.Finish();
            }
            finally
            {
                this.End();
                if (!this.leaveOpen)
                {
                    this.Stream.Close();
                }

                this.Stream = null;
            }
        }

        public override void Flush()
        {
            this.Stream.Flush();
        }

        /// <param name="buffer">
        /// </param>
        /// <param name="offset">
        /// </param>
        /// <param name="count">
        /// </param>
        /// <exception cref = "ArgumentNullException">
        /// </exception>
        /// <exception cref = "ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref = "ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref = "ArgumentOutOfRangeException">
        /// </exception>
        /// <returns>
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // According to MS documentation, any implementation of the IO.Stream.Read function must: (a) throw an
            // exception if offset & count reference an invalid part of the buffer, or if count < 0, or if buffer is
            // null (b) return 0 only upon EOF, or if count = 0 (c) if not EOF, then return at least 1 byte, up to
            // <count> bytes

            if (this.Mode == StreamMode.Undefined)
            {
                if (!this.Stream.CanRead)
                {
                    throw new ZlibException("The stream is not readable.");
                }

                // for the first read, set up some controls.
                this.Mode = StreamMode.Reader;

                // (The first reference to _z goes through the private accessor which may initialize it.)
                this.Z.AvailableBytesIn = 0;
                if (this.flavor == ZlibStreamFlavor.Gzip)
                {
                    this.GzipHeaderByteCount = this.ReadAndValidateGzipHeader();

                    // workitem 8501: handle edge case (decompress empty stream)
                    if (this.GzipHeaderByteCount == 0)
                    {
                        return 0;
                    }
                }
            }

            if (this.Mode != StreamMode.Reader)
            {
                throw new ZlibException("Cannot Read after Writing.");
            }

            if (count == 0)
            {
                return 0;
            }

            if (this.nomoreinput && this.WantCompress)
            {
                return 0; // workitem 8557
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (offset < buffer.GetLowerBound(0))
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if ((offset + count) > buffer.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            int rc;

            // set up the output of the deflate/inflate codec:
            this.ZlibCodec.OutputBuffer = buffer;
            this.ZlibCodec.NextOut = offset;
            this.ZlibCodec.AvailableBytesOut = count;

            // This is necessary in case _workingBuffer has been resized. (new byte[]) (The first reference to
            // _workingBuffer goes through the private accessor which may initialize it.)
            this.ZlibCodec.InputBuffer = this.WorkingBuffer;

            do
            {
                // need data in _workingBuffer in order to deflate/inflate.  Here, we check if we have any.
                if ((this.ZlibCodec.AvailableBytesIn == 0) && (!this.nomoreinput))
                {
                    // No data available, so try to Read data from the captive stream.
                    this.ZlibCodec.NextIn = 0;
                    this.ZlibCodec.AvailableBytesIn = this.Stream.Read(this.workBuffer, 0, this.workBuffer.Length);
                    if (this.ZlibCodec.AvailableBytesIn == 0)
                    {
                        this.nomoreinput = true;
                    }
                }

                // we have data in InputBuffer; now compress or decompress as appropriate
                rc = this.WantCompress ? this.ZlibCodec.Deflate(this.flushMode) : this.ZlibCodec.Inflate();

                if (this.nomoreinput && (rc == ZlibConstants.ZBufError))
                {
                    return 0;
                }

                if (rc != ZlibConstants.Zok && rc != ZlibConstants.ZStreamEnd)
                {
                    throw new ZlibException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}flating:  rc={1}  msg={2}",
                            this.WantCompress ? "de" : "in",
                            rc,
                            this.ZlibCodec.Message));
                }

                if ((this.nomoreinput || rc == ZlibConstants.ZStreamEnd) && (this.ZlibCodec.AvailableBytesOut == count))
                {
                    break; // nothing more to read
                }
            }
            while (this.ZlibCodec.AvailableBytesOut > 0 && !this.nomoreinput && rc == ZlibConstants.Zok);

            // workitem 8557 is there more room in output?
            if (this.ZlibCodec.AvailableBytesOut > 0)
            {
                if (rc == ZlibConstants.Zok && this.ZlibCodec.AvailableBytesIn == 0)
                {
                    // deferred
                }

                // are we completely done reading?
                if (this.nomoreinput)
                {
                    // and in compression?
                    if (this.WantCompress)
                    {
                        // no more input data available; therefore we flush to try to complete the read
                        rc = this.ZlibCodec.Deflate(FlushType.Finish);

                        if (rc != ZlibConstants.Zok && rc != ZlibConstants.ZStreamEnd)
                        {
                            throw new ZlibException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Deflating:  rc={0}  msg={1}",
                                    rc,
                                    this.ZlibCodec.Message));
                        }
                    }
                }
            }

            rc = count - this.ZlibCodec.AvailableBytesOut;

            // calculate CRC after reading
            if (this.crc != null)
            {
                this.crc.SlurpBlock(buffer, offset, rc);
            }

            return rc;
        }

        /// <param name="offset">
        /// </param>
        /// <param name="origin">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref = "NotImplementedException">
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();

            // _outStream.Seek(offset, origin);
        }

        /// <param name="value">
        /// </param>
        public override void SetLength(long value)
        {
            this.Stream.SetLength(value);
        }

        /// <param name="buffer">
        /// </param>
        /// <param name="offset">
        /// </param>
        /// <param name="count">
        /// </param>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // workitem 7159
            // calculate the CRC on the unccompressed data  (before writing)
            if (this.crc != null)
            {
                this.crc.SlurpBlock(buffer, offset, count);
            }

            if (this.Mode == StreamMode.Undefined)
            {
                this.Mode = StreamMode.Writer;
            }
            else if (this.Mode != StreamMode.Writer)
            {
                throw new ZlibException("Cannot Write after Reading.");
            }

            if (count == 0)
            {
                return;
            }

            // first reference of z property will initialize the private var _z
            this.Z.InputBuffer = buffer;
            this.ZlibCodec.NextIn = offset;
            this.ZlibCodec.AvailableBytesIn = count;
            bool done;
            do
            {
                this.ZlibCodec.OutputBuffer = this.WorkingBuffer;
                this.ZlibCodec.NextOut = 0;
                this.ZlibCodec.AvailableBytesOut = this.workBuffer.Length;
                var rc = this.WantCompress ? this.ZlibCodec.Deflate(this.flushMode) : this.ZlibCodec.Inflate();
                if (rc != ZlibConstants.Zok && rc != ZlibConstants.ZStreamEnd)
                {
                    throw new ZlibException((this.WantCompress ? "de" : "in") + "flating: " + this.ZlibCodec.Message);
                }

                // if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
                this.Stream.Write(this.workBuffer, 0, this.workBuffer.Length - this.ZlibCodec.AvailableBytesOut);

                done = this.ZlibCodec.AvailableBytesIn == 0 && this.ZlibCodec.AvailableBytesOut != 0;

                // If GZIP and de-compress, we're done when 8 bytes remain.
                if (this.flavor == ZlibStreamFlavor.Gzip && !this.WantCompress)
                {
                    done = this.ZlibCodec.AvailableBytesIn == 8 && this.ZlibCodec.AvailableBytesOut != 0;
                }
            }
            while (!done);
        }

        #endregion

        #region Methods

        private void End()
        {
            if (this.Z == null)
            {
                return;
            }

            if (this.WantCompress)
            {
                this.ZlibCodec.EndDeflate();
            }
            else
            {
                this.ZlibCodec.EndInflate();
            }

            this.ZlibCodec = null;
        }

        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        private void Finish()
        {
            if (this.ZlibCodec == null)
            {
                return;
            }

            switch (this.Mode)
            {
                case StreamMode.Writer:
                    {
                        bool done;
                        do
                        {
                            this.ZlibCodec.OutputBuffer = this.WorkingBuffer;
                            this.ZlibCodec.NextOut = 0;
                            this.ZlibCodec.AvailableBytesOut = this.workBuffer.Length;
                            var rc = this.WantCompress
                                         ? this.ZlibCodec.Deflate(FlushType.Finish)
                                         : this.ZlibCodec.Inflate();

                            if (rc != ZlibConstants.ZStreamEnd && rc != ZlibConstants.Zok)
                            {
                                var verb = (this.WantCompress ? "de" : "in") + "flating";
                                if (this.ZlibCodec.Message == null)
                                {
                                    throw new ZlibException(
                                        string.Format(CultureInfo.InvariantCulture, "{0}: (rc = {1})", verb, rc));
                                }

                                throw new ZlibException(verb + ": " + this.ZlibCodec.Message);
                            }

                            if (this.workBuffer.Length - this.ZlibCodec.AvailableBytesOut > 0)
                            {
                                this.Stream.Write(
                                    this.workBuffer, 0, this.workBuffer.Length - this.ZlibCodec.AvailableBytesOut);
                            }

                            done = this.ZlibCodec.AvailableBytesIn == 0 && this.ZlibCodec.AvailableBytesOut != 0;

                            // If GZIP and de-compress, we're done when 8 bytes remain.
                            if (this.flavor == ZlibStreamFlavor.Gzip && !this.WantCompress)
                            {
                                done = this.ZlibCodec.AvailableBytesIn == 8 && this.ZlibCodec.AvailableBytesOut != 0;
                            }
                        }
                        while (!done);

                        this.Flush();

                        // workitem 7159
                        if (this.flavor == ZlibStreamFlavor.Gzip)
                        {
                            if (this.WantCompress)
                            {
                                // Emit the GZIP trailer: CRC32 and  size mod 2^32
                                var c1 = this.crc.Crc32Result;
                                this.Stream.Write(BitConverter.GetBytes(c1), 0, 4);
                                var c2 = (int)(this.crc.TotalBytesRead & 0x00000000FFFFFFFF);
                                this.Stream.Write(BitConverter.GetBytes(c2), 0, 4);
                            }
                            else
                            {
                                throw new ZlibException("Writing with decompression is not supported.");
                            }
                        }
                    }

                    break;
                case StreamMode.Reader:
                    if (this.flavor == ZlibStreamFlavor.Gzip)
                    {
                        if (!this.WantCompress)
                        {
                            // workitem 8501: handle edge case (decompress empty stream)
                            if (this.ZlibCodec.TotalBytesOut == 0L)
                            {
                                return;
                            }

                            // Read and potentially verify the GZIP trailer: CRC32 and size mod 2^32
                            var trailer = new byte[8];

                            // workitem 8679
                            if (this.ZlibCodec.AvailableBytesIn != 8)
                            {
                                // Make sure we have read to the end of the stream
                                Array.Copy(
                                    this.ZlibCodec.InputBuffer,
                                    this.ZlibCodec.NextIn,
                                    trailer,
                                    0,
                                    this.ZlibCodec.AvailableBytesIn);
                                var bytesNeeded = 8 - this.ZlibCodec.AvailableBytesIn;
                                var bytesRead = this.Stream.Read(trailer, this.ZlibCodec.AvailableBytesIn, bytesNeeded);
                                if (bytesNeeded != bytesRead)
                                {
                                    throw new ZlibException(
                                        string.Format(
                                            CultureInfo.InvariantCulture,
                                            "Protocol error. AvailableBytesIn={0}, expected 8",
                                            this.ZlibCodec.AvailableBytesIn + bytesRead));
                                }
                            }
                            else
                            {
                                Array.Copy(
                                    this.ZlibCodec.InputBuffer, this.ZlibCodec.NextIn, trailer, 0, trailer.Length);
                            }

                            // var crc32Expected = BitConverter.ToInt32(trailer, 0); var crc32Actual =
                            // this.crc.Crc32Result; var sizeExpected = BitConverter.ToInt32(trailer, 4); var sizeActual
                            // = (int)(this.ZlibCodec.TotalBytesOut & 0x00000000FFFFFFFF);

                            // if (crc32Actual != crc32Expected) { throw new
                            // ZlibException(String.Format(CultureInfo.InvariantCulture, "Bad CRC32 in GZIP stream.
                            // actual ({0:X8}) != expected ({1:X8})", crc32Actual, crc32Expected)); }

                            // if (sizeActual != sizeExpected) { throw new
                            // ZlibException(String.Format(CultureInfo.InvariantCulture, "Bad size in GZIP stream.
                            // actual({0}) != expected ({1})", sizeActual, sizeExpected)); }
                        }
                        else
                        {
                            throw new ZlibException("Reading with compression is not supported.");
                        }
                    }

                    break;
            }
        }

        /// <returns>
        /// </returns>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        /// <exception cref = "ZlibException">
        /// </exception>
        private int ReadAndValidateGzipHeader()
        {
            var totalBytesRead = 0;

            // read the header on the first read
            var header = new byte[10];
            var n = this.Stream.Read(header, 0, header.Length);

            // workitem 8501: handle edge case (decompress empty stream)
            if (n == 0)
            {
                return 0;
            }

            if (n != 10)
            {
                throw new ZlibException("Not a valid GZIP stream.");
            }

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
            {
                throw new ZlibException("Bad GZIP header.");
            }

            // var timet = BitConverter.ToInt32(header, 4);
            totalBytesRead += n;
            if ((header[3] & 0x04) == 0x04)
            {
                // read and discard extra field
                n = this.Stream.Read(header, 0, 2); // 2-byte length field
                totalBytesRead += n;

                var extraLength = (short)(header[0] + header[1] * 256);
                var extra = new byte[extraLength];
                n = this.Stream.Read(extra, 0, extra.Length);
                if (n != extraLength)
                {
                    throw new ZlibException("Unexpected end-of-file reading GZIP header.");
                }

                totalBytesRead += n;
            }

            if ((header[3] & 0x08) == 0x08)
            {
                this.GzipFileName = this.ReadZeroTerminatedString();
            }

            if ((header[3] & 0x10) == 0x010)
            {
                this.GzipComment = this.ReadZeroTerminatedString();
            }

            if ((header[3] & 0x02) == 0x02)
            {
                this.Read(this.buf1, 0, 1); // CRC16, ignore
            }

            return totalBytesRead;
        }

        /// <returns>
        /// </returns>
        /// <exception cref = "ZlibException">
        /// </exception>
        private string ReadZeroTerminatedString()
        {
            var list = new List<byte>();
            var done = false;
            do
            {
                // workitem 7740
                var n = this.Stream.Read(this.buf1, 0, 1);
                if (n != 1)
                {
                    throw new ZlibException("Unexpected EOF reading GZIP header.");
                }

                if (this.buf1[0] == 0)
                {
                    done = true;
                }
                else
                {
                    list.Add(this.buf1[0]);
                }
            }
            while (!done);
            var a = list.ToArray();
            return GZipStream.Iso8859Dash1.GetString(a, 0, a.Length);
        }

        #endregion

        /*
        public static void CompressBuffer(byte[] b, Stream compressor)
        {
            // workitem 8460
            using (compressor)
            {
                compressor.Write(b, 0, b.Length);
            }
        }
*/

        /*
        public static void CompressString(string s, Stream compressor)
        {
            var uncompressed = Encoding.UTF8.GetBytes(s);
            using (compressor)
            {
                compressor.Write(uncompressed, 0, uncompressed.Length);
            }
        }
*/

        /*
        public static byte[] UncompressBuffer(Stream decompressor)
        {
            // workitem 8460
            var working = new byte[1024];
            using (var output = new MemoryStream())
            {
                using (decompressor)
                {
                    int n;
                    while ((n = decompressor.Read(working, 0, working.Length)) != 0)
                    {
                        output.Write(working, 0, n);
                    }
                }

                return output.ToArray();
            }
        }
*/

        /*
        public static string UncompressString(Stream decompressor)
        {
            // workitem 8460
            var working = new byte[1024];
            var encoding = Encoding.UTF8;
            using (var output = new MemoryStream())
            {
                using (decompressor)
                {
                    int n;
                    while ((n = decompressor.Read(working, 0, working.Length)) != 0)
                    {
                        output.Write(working, 0, n);
                    }
                }

                // reset to allow read from start
                output.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(output, encoding);
                return sr.ReadToEnd();
            }
        }
*/
    }
}