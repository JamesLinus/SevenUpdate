// <copyright file="Adler.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Zlib
{
    using Interop = System.Runtime.InteropServices;

    /// <summary>Computes an Adler-32 checksum.</summary>
    /// <remarks>The Adler checksum is similar to a CRC checksum, but faster to compute, though less reliable.  It is used in producing RFC1950 compressed streams.  The Adler checksum is a required part of the "ZLIB" standard.  Applications will almost never need to use this class directly.</remarks>
    internal static class Adler
    {
        private const int Base = 65521;

        // NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
        private const int Nmax = 5552;

        /// <param name="adler"></param><param name="buf"></param> <param name="index"></param><param name="len"></param><returns></returns>
        internal static uint Adler32(uint adler, byte[] buf, int index, int len)
        {
            if (buf == null)
            {
                return 1;
            }

            var s1 = (int)(adler & 0xffff);
            var s2 = (int)((adler >> 16) & 0xffff);

            while (len > 0)
            {
                int k = len < Nmax ? len : Nmax;
                len -= k;
                while (k >= 16)
                {
                    // s1 += (buf[index++] & 0xff); s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    k -= 16;
                }

                if (k != 0)
                {
                    do
                    {
                        s1 += buf[index++];
                        s2 += s1;
                    }
                    while (--k != 0);
                }

                s1 %= Base;
                s2 %= Base;
            }

            return (uint)((s2 << 16) | s1);
        }

        // largest prime smaller than 65536
    }
}