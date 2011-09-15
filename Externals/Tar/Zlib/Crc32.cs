//-----------------------------------------------------------------------
// <copyright file="CRC32.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    using System.Runtime.InteropServices;

    /// <summary>Calculates a 32bit Cyclic Redundancy Checksum (CRC) using the same polynomial used by Zip. This type is used internally by DotNetZip; it is generally not used directly by applications wishing to create, read, or manipulate zip archive files.</summary>
    [Guid("ebc25cf6-9120-4283-b972-0e5520d0000C")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public sealed class Crc32
    {
        #region Constants and Fields

        private static readonly uint[] Crc32Table;

        private uint runningCrc32Result = 0xFFFFFFFF;

        #endregion

        #region Constructors and Destructors

        static Crc32()
        {
            unchecked
            {
                // PKZip specifies CRC32 with a polynomial of 0xEDB88320; This is also the CRC-32 polynomial used bby
                // Ethernet, FDDI, bzip2, gzip, and others. Often the polynomial is shown reversed as 0x04C11DB7. For
                // more details, see http://en.wikipedia.org/wiki/Cyclic_redundancy_check
                const uint polynomial = 0xEDB88320;
                uint i;

                Crc32Table = new uint[256];

                for (i = 0; i < 256; i++)
                {
                    var dwCrc = i;
                    uint j;
                    for (j = 8; j > 0; j--)
                    {
                        if ((dwCrc & 1) == 1)
                        {
                            dwCrc = (dwCrc >> 1) ^ polynomial;
                        }
                        else
                        {
                            dwCrc >>= 1;
                        }
                    }

                    Crc32Table[i] = dwCrc;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>Indicates the current CRC for all blocks slurped in.</summary>
        internal int Crc32Result
        {
            get
            {
                // return one's complement of the running result
                return unchecked((int)(~this.runningCrc32Result));
            }
        }

        /// <summary>indicates the total number of bytes read on the CRC stream. This is used when writing the ZipDirEntry when compressing files.</summary>
        internal long TotalBytesRead { get; private set; }

        #endregion

        #region Methods

        /// <summary>Update the value for the running CRC32 using the given block of bytes. This is useful when using the CRC32() class in a Stream.</summary>
        /// <param name="block">block of bytes to slurp</param>
        /// <param name="offset">starting point in the block</param>
        /// <param name="count">how many bytes within the block to slurp</param>
        internal void SlurpBlock(byte[] block, int offset, int count)
        {
            if (block == null)
            {
                throw new ZlibException("The data buffer must not be null.");
            }

            for (var i = 0; i < count; i++)
            {
                var x = offset + i;
                this.runningCrc32Result = (this.runningCrc32Result >> 8) ^
                                          Crc32Table[block[x] ^ (this.runningCrc32Result & 0x000000FF)];
            }

            this.TotalBytesRead += count;
        }

        #endregion

        /*
        private const int BufferSize = 8192;
*/

        /*
        /// <summary>Combines the given CRC32 value with the current running total.</summary>
        /// <remarks>This is useful when using a divide-and-conquer approach to calculating a CRC. Multiple threads caneach calculate a CRC32 on a segment of the data, and then combine the individual CRC32 values at theend.</remarks>
        /// <param name="crc">the crc value to be combined with this one</param>
        /// <param name="length">the length of data the CRC value was calculated on</param>
        public void Combine(int crc, int length)
        {
            var even = new uint[32]; // even-power-of-two zeros operator
            var odd = new uint[32]; // odd-power-of-two zeros operator

            if (length == 0)
            {
                return;
            }

            var crc1 = ~this.runningCrc32Result;
            var crc2 = (uint)crc;

            // put operator for one zero bit in odd
            odd[0] = 0xEDB88320; // the CRC-32 polynomial
            uint row = 1;
            for (var i = 1; i < 32; i++)
            {
                odd[i] = row;
                row <<= 1;
            }

            // put operator for two zero bits in even
            this.Gf2MatrixSquare(even, odd);

            // put operator for four zero bits in odd
            this.Gf2MatrixSquare(odd, even);

            var len2 = (uint)length;

            // apply len2 zeros to crc1 (first square will put the operator for one zero byte, eight zero bits, in even)
            do
            {
                // apply zeros operator for this bit of len2
                this.Gf2MatrixSquare(even, odd);

                if ((len2 & 1) == 1)
                {
                    crc1 = this.Gf2MatrixTimes(even, crc1);
                }

                len2 >>= 1;

                if (len2 == 0)
                {
                    break;
                }

                // another iteration of the loop with odd and even swapped
                this.Gf2MatrixSquare(odd, even);
                if ((len2 & 1) == 1)
                {
                    crc1 = this.Gf2MatrixTimes(odd, crc1);
                }

                len2 >>= 1;
            }
            while (len2 != 0);

            crc1 ^= crc2;

            this.runningCrc32Result = ~crc1;

            // return (int) crc1;
            return;
        }
*/

        /*
        /// <summary>Get the CRC32 for the given (word,byte) combo.  This is a computationdefined by PKzip.</summary>
        /// <param name="w">The word to start with.</param>
        /// <param name="b">The byte to combine it with.</param>
        /// <returns>The CRC-ized result.</returns>
        public int ComputeCrc32(int w, byte b)
        {
            return this.InternalComputeCrc32((UInt32)w, b);
        }
*/

        /*
        /// <summary>Returns the CRC32 for the specified stream.</summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <returns>the CRC32 calculation</returns>
        public int GetCrc32(Stream input)
        {
            return this.GetCrc32AndCopy(input, null);
        }
*/

        /*
        /// <summary>Returns the CRC32 for the specified stream, and writes the input into the output stream.</summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <param name="output">The stream into which to deflate the input</param>
        /// <returns>the CRC32 calculation</returns>
        public int GetCrc32AndCopy(Stream input, Stream output)
        {
            if (input == null)
            {
                throw new ZlibException("The input stream must not be null.");
            }

            unchecked
            {
                // UInt32 crc32Result; crc32Result = 0xFFFFFFFF;
                var buffer = new byte[BufferSize];
                const int readSize = BufferSize;

                this.totalBytesRead = 0;
                var count = input.Read(buffer, 0, readSize);
                if (output != null)
                {
                    output.Write(buffer, 0, count);
                }

                this.totalBytesRead += count;
                while (count > 0)
                {
                    this.SlurpBlock(buffer, 0, count);
                    count = input.Read(buffer, 0, readSize);
                    if (output != null)
                    {
                        output.Write(buffer, 0, count);
                    }

                    this.totalBytesRead += count;
                }

                return (int)(~this.runningCrc32Result);
            }
        }
*/

        /*
        internal int InternalComputeCrc32(uint w, byte b)
        {
            return (int)(Crc32Table[(w ^ b) & 0xFF] ^ (w >> 8));
        }
*/

        // pre-initialize the crc table for speed of lookup.

        /*
        private void Gf2MatrixSquare(uint[] square, uint[] mat)
        {
            for (var i = 0; i < 32; i++)
            {
                square[i] = this.Gf2MatrixTimes(mat, mat[i]);
            }
        }
*/

        /*
        private uint Gf2MatrixTimes(uint[] matrix, uint vec)
        {
            uint sum = 0;
            var i = 0;
            while (vec != 0)
            {
                if ((vec & 0x01) == 0x01)
                {
                    sum ^= matrix[i];
                }

                vec >>= 1;
                i++;
            }

            return sum;
        }
*/
    }
}
