// <copyright file="GZipStream.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Zlib
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>A class for compressing and decompressing GZIP streams.</summary>
    public class GZipStream : Stream
    {
        internal static readonly Encoding Iso8859Dash1 = Encoding.GetEncoding("iso-8859-1");

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly ZlibBaseStream baseStream;

        private string comment;

        private bool disposed;

        private string fileName;

        private bool firstReadDone;

        private int headerByteCount;

        /// <summary>Create a <c>GZipStream</c> using the specified <c>CompressionMode</c>, and explicitly specify whether the stream should be left open after Deflation or Inflation.</summary>
        /// <remarks>
        ///   <para>This constructor allows the application to request that the captive stream remain open after the deflation or inflation occurs.  By default, after<c>Close()</c> is called on the stream, the captive stream is also closed. In some cases this is not desired, for example if the stream is a memory stream that will be re-read after compressed data has been written to it.  Specify true for the <paramref name="leaveOpen" /> parameter to leave the stream open.</para>
        ///   <para>The <see cref = "CompressionMode" /> (Compress or Decompress) also establishes the "direction" of the stream.  A <c>GZipStream</c> with<c>CompressionMode.Compress</c> works only through <c>Write()</c>.  A <c>GZipStream</c> with <c>CompressionMode.Decompress</c> works only through <c>Read()</c>.</para><para>See the other overloads of this constructor for example code.</para></remarks>
        /// <param name="stream">The stream which will be read or written. This is called the "captive" stream in other places in this documentation.</param>
        /// <param name="mode">Indicates whether the GZipStream will compress or decompress.</param>
        /// <param name="leaveOpen">true if the application would like the base stream to remain open after inflation/deflation.</param>
        public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
            : this(stream, mode, CompressionLevel.Default, leaveOpen)
        {
        }

        /// <summary>Create a <c>GZipStream</c> using the specified <c>CompressionMode</c> and the specified <c>CompressionLevel</c>, and explicitly specify whether the stream should be left open after Deflation or Inflation.</summary>
        /// <remarks><para>This constructor allows the application to request that the captive stream remain open after the deflation or inflation occurs.  By default, after<c>Close()</c> is called on the stream, the captive stream is also closed. In some cases this is not desired, for example if the stream is a memory stream that will be re-read after compressed data has been written to it.  Specify true for the <paramref name="leaveOpen" /> parameter to leave the stream open.</para><para>As noted in the class documentation, the <c>CompressionMode</c> (Compress or Decompress) also establishes the "direction" of the stream.  A<c>GZipStream</c> with <c>CompressionMode.Compress</c> works only through<c>Write()</c>.  A <c>GZipStream</c> with <c>CompressionMode.Decompress</c> works only through <c>Read()</c>.</para></remarks>
        /// <example>
        ///   This example shows how to use a <c>GZipStream</c> to compress data.
        ///   <code>using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress)) { using (var raw = System.IO.File.Create(outputFile)) { using (Stream compressor = new GZipStream(raw, CompressionMode.Compress, CompressionLevel.BestCompression, true)) { byte[] buffer = new byte[WORKING_BUFFER_SIZE]; int n; while ((n= input.Read(buffer, 0, buffer.Length)) != 0) { compressor.Write(buffer, 0, n); } } } }</code>
        ///   <code lang = "VB">
        ///     Dim outputFile As String = (fileToCompress &amp; ".compressed") Using input As Stream =
        ///     File.OpenRead(fileToCompress) Using raw As FileStream = File.Create(outputFile) Using compressor As
        ///     Stream = New GZipStream(raw, CompressionMode.Compress, CompressionLevel.BestCompression, True) Dim
        ///     buffer As Byte() = New Byte(4096) {} Dim n As Integer = -1 Do While (n &lt;&gt; 0) If (n &gt; 0) Then
        ///     compressor.Write(buffer, 0, n) End If n = input.Read(buffer, 0, buffer.Length) Loop End Using End Using
        ///     End Using
        ///   </code>
        /// </example>
        /// <param name="stream">The stream which will be read or written.</param>
        /// <param name="mode">Indicates whether the GZipStream will compress or decompress.</param>
        /// <param name="level">A tuning knob to trade speed for effectiveness.</param>
        /// <param name="leaveOpen">true if the application would like the stream to remain open after inflation/deflation.</param>
        public GZipStream(
            Stream stream, 
            CompressionMode mode, 
            CompressionLevel level = CompressionLevel.Default, 
            bool leaveOpen = false)
        {
            this.baseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.Gzip, leaveOpen);
        }

        /// <summary>Indicates whether the stream can be read.</summary>
        /// <remarks>The return value depends on whether the captive stream supports reading.</remarks>
        public override bool CanRead
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("GZipStream");
                }

                return this.baseStream.Stream.CanRead;
            }
        }

        /// <summary>Indicates whether the stream supports Seek operations.</summary>
        /// <remarks>Always returns false.</remarks>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>Indicates whether the stream can be written.</summary>
        /// <remarks>The return value depends on whether the captive stream supports writing.</remarks>
        public override bool CanWrite
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("GZipStream");
                }

                return this.baseStream.Stream.CanWrite;
            }
        }

        /// <summary>The comment on the GZIP stream.</summary>
        /// <remarks>
        ///   <para>The GZIP format allows for each file to optionally have an associated comment stored with the file.  The comment is encoded with the ISO-8859-1 code page.  To include a comment in a GZIP stream you create, set this property before calling <c>Write()</c> for the first time on the <c>GZipStream</c>.</para><para>When using <c>GZipStream</c> to decompress, you can retrieve this property after the first call to <c>Read()</c>.  If no comment has been set in the GZIP bytestream, the Comment property will return <c>null</c> (<c>Nothing</c> in VB).</para></remarks>
        public string Comment
        {
            get { return this.comment; }

            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("GZipStream");
                }

                this.comment = value;
            }
        }

        /// <summary>The CRC on the GZIP stream.</summary>
        /// <remarks>This is used for internal error checking. You probably don't need to look at this property.</remarks>
        public int Crc32 { get; private set; }

        /// <summary>The FileName for the GZIP stream.</summary>
        /// <remarks>
        ///   <para>The GZIP format optionally allows each file to have an associated filename.  When compressing data (through <c>Write()</c>), set this FileName before calling <c>Write()</c> the first time on the <c>GZipStream</c>. The actual filename is encoded into the GZIP bytestream with the ISO-8859-1 code page, according to RFC 1952. It is the application's responsibility to insure that the FileName can be encoded and decoded correctly with this code page.</para><para>When decompressing (through <c>Read()</c>), you can retrieve this value any time after the first <c>Read()</c>.  In the case where there was no filename encoded into the GZIP bytestream, the property will return <c>null</c> (<c>Nothing</c> in VB).</para></remarks>
        public string FileName
        {
            get { return this.fileName; }

            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("GZipStream");
                }

                this.fileName = value;
                if (this.fileName == null)
                {
                    return;
                }

                if (this.fileName.IndexOf("/", StringComparison.Ordinal) != -1)
                {
                    this.fileName = this.fileName.Replace("/", "\\");
                }

                if (this.fileName.EndsWith("\\", StringComparison.Ordinal))
                {
                    throw new Exception("Illegal filename");
                }

                if (this.fileName.IndexOf("\\", StringComparison.Ordinal) != -1)
                {
                    // trim any leading path
                    this.fileName = Path.GetFileName(this.fileName);
                }
            }
        }

        /*
        /// <summary>This property sets the flush behavior on the stream.</summary>
        public virtual FlushType FlushMode
        {
            get
            {
                return this.BaseStream.FlushMode;
            }

            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("GZipStream");
                }

                this.BaseStream.FlushMode = value;
            }
        }
*/

        /// <summary>Gets or sets the last modified.</summary>
        /// <value>The last modified.</value>
        public DateTime? LastModified { get; set; }

        /// <summary>Reading this property always throws a <c>NotImplementedException</c>.</summary>
        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>The position of the stream pointer.</summary>
        /// <remarks>Setting this property always throws a <c>NotImplementedException</c>. Reading will return the total bytes written out, if used in writing, or the total bytes read in, if used in reading.  The count may refer to compressed bytes or uncompressed bytes, depending on how you've used the stream.</remarks>
        public override long Position
        {
            get
            {
                if (this.baseStream.Mode == ZlibBaseStream.StreamMode.Writer)
                {
                    return this.baseStream.ZlibCodec.TotalBytesOut + this.headerByteCount;
                }

                if (this.baseStream.Mode == ZlibBaseStream.StreamMode.Reader)
                {
                    return this.baseStream.ZlibCodec.TotalBytesIn + this.baseStream.GzipHeaderByteCount;
                }

                return 0;
            }

            set { throw new NotImplementedException(); }
        }

        /// <summary>Flush the stream.</summary>
        public override void Flush()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("GZipStream");
            }

            this.baseStream.Flush();
        }

        /// <summary>Read and decompress data from the source stream.</summary>
        /// <remarks>With a <c>GZipStream</c>, decompression is done through reading.</remarks>
        /// <example><code>byte[] working = new byte[WORKING_BUFFER_SIZE]; using (System.IO.Stream input = System.IO.File.OpenRead(_CompressedFile)) { using (Stream decompressor= new Zlib.GZipStream(input, CompressionMode.Decompress, true)) { using (var output = System.IO.File.Create(_DecompressedFile)) { int n; while ((n= decompressor.Read(working, 0, working.Length)) !=0) { output.Write(working, 0, n); } } } }</code></example>
        /// <param name="buffer">The buffer into which the decompressed data should be placed.</param>
        /// <param name="offset">the offset within that data array to put the first byte read.</param>
        /// <param name="count">the number of bytes to read.</param>
        /// <returns>the number of bytes actually read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("GZipStream");
            }

            int n = this.baseStream.Read(buffer, offset, count);

            // Console.WriteLine("GZipStream::Read(buffer, off({0}), c({1}) = {2}", offset, count, n);
            // Console.WriteLine( Util.FormatByteArray(buffer, offset, n) );
            if (!this.firstReadDone)
            {
                this.firstReadDone = true;
                this.FileName = this.baseStream.GzipFileName;
                this.Comment = this.baseStream.GzipComment;
            }

            return n;
        }

        /// <summary>Calling this method always throws a <see cref = "NotImplementedException" />.</summary>
        /// <param name="offset">This parameter is not used</param>
        /// <param name="origin">This parameter is not used</param>
        /// <returns>irrelevant!</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>Calling this method always throws a <see cref = "NotImplementedException" />.</summary>
        /// <param name="value">This parameter is not used</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>Write data to the stream.</summary>
        /// <remarks><para>If you wish to use the <c>GZipStream</c> to compress data while writing, you can create a <c>GZipStream</c> with <c>CompressionMode.Compress</c>, and a writable output stream.  Then call <c>Write()</c> on that <c>GZipStream</c>, providing uncompressed data as input.  The data sent to the output stream will be the compressed form of the data written.</para><para>A <c>GZipStream</c> can be used for <c>Read()</c> or <c>Write()</c>, but not both. Writing implies compression.  Reading implies decompression.</para></remarks>
        /// <param name="buffer">The buffer holding data to write to the stream.</param>
        /// <param name="offset">the offset within that data array to find the first byte to write.</param>
        /// <param name="count">the number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("GZipStream");
            }

            if (this.baseStream.Mode == ZlibBaseStream.StreamMode.Undefined)
            {
                // Console.WriteLine("GZipStream: First write");
                if (this.baseStream.WantCompress)
                {
                    // first write in compression, therefore, emit the GZIP header
                    this.headerByteCount = this.EmitHeader();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            this.baseStream.Write(buffer, offset, count);
        }

        /// <summary>Dispose the stream.</summary><param name="disposing"></param>
        /// <remarks>This may or may not result in a <c>Close()</c> call on the captive stream. See the doc on constructors that take a <c>leaveOpen</c> parameter for more information.</remarks>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!this.disposed)
                {
                    if (disposing && (this.baseStream != null))
                    {
                        this.baseStream.Close();
                        this.Crc32 = this.baseStream.Crc32;
                    }

                    this.disposed = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <returns>
        /// </returns>
        private int EmitHeader()
        {
            byte[] commentBytes = (this.Comment == null) ? null : Iso8859Dash1.GetBytes(this.Comment);
            byte[] filenameBytes = (this.FileName == null) ? null : Iso8859Dash1.GetBytes(this.FileName);

            int commentBytesLength = (this.Comment == null) ? 0 : commentBytes.Length + 1;
            int fileNameLength = (this.FileName == null) ? 0 : filenameBytes.Length + 1;

            int bufferLength = 10 + commentBytesLength + fileNameLength;
            var header = new byte[bufferLength];
            int i = 0;

            // ID
            header[i++] = 0x1F;
            header[i++] = 0x8B;

            // compression method
            header[i++] = 8;
            byte flag = 0;
            if (this.Comment != null)
            {
                flag ^= 0x10;
            }

            if (this.FileName != null)
            {
                flag ^= 0x8;
            }

            // flag
            header[i++] = flag;

            // mtime
            if (!this.LastModified.HasValue)
            {
                this.LastModified = DateTime.Now;
            }

            TimeSpan delta = this.LastModified.Value - UnixEpoch;
            var timet = (Int32)delta.TotalSeconds;
            Array.Copy(BitConverter.GetBytes(timet), 0, header, i, 4);
            i += 4;

            // xflg
            header[i++] = 0; // this field is totally useless

            // OS
            header[i++] = 0xFF; // 0xFF == unspecified

            // extra field length - only if FEXTRA is set, which it is not. header[i++]= 0; header[i++]= 0;

            // filename
            if (fileNameLength != 0)
            {
                Array.Copy(filenameBytes, 0, header, i, fileNameLength - 1);
                i += fileNameLength - 1;
                header[i++] = 0; // terminate
            }

            // comment
            if (commentBytesLength != 0)
            {
                Array.Copy(commentBytes, 0, header, i, commentBytesLength - 1);
                i += commentBytesLength - 1;
                header[i++] = 0; // terminate
            }

            this.baseStream.Stream.Write(header, 0, header.Length);

            return header.Length; // bytes written
        }

        /*
        /// <summary>Create a <c>GZipStream</c> using the specified <c>CompressionMode</c> and the specified <c>CompressionLevel</c>.</summary><remarks><para>The <c>CompressionMode</c> (Compress or Decompress) also establishes the "direction" of the stream.  A <c>GZipStream</c> with<c>CompressionMode.Compress</c> works only through <c>Write()</c>.  A<c>GZipStream</c> with <c>CompressionMode.Decompress</c> works only through <c>Read()</c>.</para></remarks>
        /// <example>
        /// This example shows how to use a <c>GZipStream</c> to compress a file into a .gz file.
        /// <code>
        /// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress)) {
        ///     using (var raw = System.IO.File.Create(fileToCompress + ".gz")) {
        ///         using (Stream compressor = new GZipStream(raw,
        ///                                                   CompressionMode.Compress,
        ///                                                   CompressionLevel.BestCompression))
        ///         {
        ///             byte[] buffer = new byte[WORKING_BUFFER_SIZE]; int n; while ((n= input.Read(buffer, 0,
        ///             buffer.Length)) != 0) {
        ///                 compressor.Write(buffer, 0, n);
        ///             }
        ///         }
        ///     }
        /// }</code> <code lang="VB">Using input As Stream = File.OpenRead(fileToCompress)
        ///     Using raw As FileStream = File.Create(fileToCompress &amp; ".gz")
        ///         Using compressor As Stream = New GZipStream(raw, CompressionMode.Compress,
        ///         CompressionLevel.BestCompression)
        ///             Dim buffer As Byte() = New Byte(4096) {} Dim n As Integer = -1 Do While (n &lt;&gt; 0)
        ///                 If (n &gt; 0) Then
        ///                     compressor.Write(buffer, 0, n)
        ///                 End If n = input.Read(buffer, 0, buffer.Length)
        ///             Loop
        ///         End Using
        ///     End Using
        /// End Using</code>
        /// </example>
        /// <param name="stream">The stream to be read or written while deflating or inflating.</param>
        /// <param name="mode">Indicates whether the <c>GZipStream</c> will compress or decompress.</param>
        /// <param name="level">A tuning knob to trade speed for effectiveness.</param>
        public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level) : this(stream, mode, level, false)
        {
        }
*/

        /*
        /// <summary>The size of the working buffer for the compression codec.</summary>
        ///
        /// <remarks>
        /// <para>The working buffer is used for all stream operations.  The default size is 1024 bytes.  The minimum size is 128 bytes. You may get better performance with a larger buffer.  Then again, you might not.  You would have to test it.</para>/// <para>Set this before the first call to <c>Read()</c> or <c>Write()</c> on the stream. If you try to set it afterwards, it will throw.</para></remarks>
        public int BufferSize
        {
            get
            {
                return this.BaseStream.BufferSize;
            }

            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("GZipStream");
                }

                if (this.BaseStream.WorkBuffer != null)
                {
                    throw new ZlibException("The working buffer is already set.");
                }

                if (value < ZlibConstants.WorkingBufferSizeMin)
                {
                    throw new ZlibException(String.Format(CultureInfo.InvariantCulture, "Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, ZlibConstants.WorkingBufferSizeMin));
                }

                this.BaseStream.BufferSize = value;
            }
        }
*/

        /*
        /// <summary> Returns the total number of bytes input so far.</summary>
        public virtual long TotalIn
        {
            get
            {
                return this.BaseStream.ZlibCodec.TotalBytesIn;
            }
        }
*/

        /*
        /// <summary> Returns the total number of bytes output so far.</summary>
        public virtual long TotalOut
        {
            get
            {
                return this.BaseStream.ZlibCodec.TotalBytesOut;
            }
        }
*/

        /*
        /// <summary>Compress a byte array into a new byte array using GZip.</summary>
        /// <remarks>Uncompress it with <see cref="GZipStream.UncompressBuffer(byte[])"/>.</remarks>
        /// <seealso cref="GZipStream.CompressString(string)"/><seealso cref="GZipStream.UncompressBuffer(byte[])"/>
        /// <param name="b">A buffer to compress.</param>
        /// <returns>The data in compressed form</returns>
        public static byte[] CompressBuffer(byte[] b)
        {
            using (var ms = new MemoryStream())
            {
                Stream compressor = new GZipStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);

                ZlibBaseStream.CompressBuffer(b, compressor);
                return ms.ToArray();
            }
        }
*/

        /*
        /// <summary>Compress a string into a byte array using GZip.</summary>
        /// <remarks>Uncompress it with <see cref="GZipStream.UncompressString(byte[])"/>.</remarks>
        /// <seealso cref="GZipStream.UncompressString(byte[])"/><seealso cref="GZipStream.CompressBuffer(byte[])"/>
        /// <param name="s">
        ///   A string to compress. The string will first be encoded using UTF8, then compressed.</param>
        /// <returns>The string in compressed form</returns>
        public static byte[] CompressString(string s)
        {
            using (var ms = new MemoryStream())
            {
                Stream compressor = new GZipStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);
                ZlibBaseStream.CompressString(s, compressor);
                return ms.ToArray();
            }
        }
*/

        /*
        /// <summary>Uncompress a GZip'ed byte array into a byte array.</summary>
        /// <seealso cref="GZipStream.CompressBuffer(byte[])"/><seealso cref="GZipStream.UncompressString(byte[])"/>
        /// <param name="compressed">
        ///   A buffer containing data that has been compressed with GZip.</param>
        /// <returns>The data in uncompressed form</returns>
        public static byte[] UncompressBuffer(byte[] compressed)
        {
            using (var input = new MemoryStream(compressed))
            {
                Stream decompressor = new GZipStream(input, CompressionMode.Decompress);

                return ZlibBaseStream.UncompressBuffer(decompressor);
            }
        }
*/

        /*
        /// <summary>Uncompress a GZip'ed byte array into a single string.</summary>
        /// <seealso cref="GZipStream.CompressString(String)"/><seealso cref="GZipStream.UncompressBuffer(byte[])"/>
        /// <param name="compressed">
        ///   A buffer containing GZIP-compressed data.</param>
        /// <returns>The uncompressed string</returns>
        public static string UncompressString(byte[] compressed)
        {
            using (var input = new MemoryStream(compressed))
            {
                Stream decompressor = new GZipStream(input, CompressionMode.Decompress);
                return ZlibBaseStream.UncompressString(decompressor);
            }
        }
*/
    }
}