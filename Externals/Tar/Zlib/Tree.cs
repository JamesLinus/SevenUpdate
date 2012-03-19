// <copyright file="Tree.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Zlib
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// </summary>
    internal sealed class Tree
    {
        internal static readonly sbyte[] BlOrder = new sbyte[]
            {
               16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 
            };

        // see definition of array dist_code below internal const int DIST_CODE_LEN = 512;
        internal static readonly int[] DistanceBase = new[]
            {
                0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 384, 512, 768, 1024, 1536, 2048, 3072, 
                4096, 6144, 8192, 12288, 16384, 24576
            };

        internal static readonly int[] ExtraBlbits = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7 };

        internal static readonly int[] ExtraDistanceBits = new[]
            {
               0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13 
            };

        internal static readonly int[] ExtraLengthBits = new[]
            {
               0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0 
            };

        internal static readonly int[] LengthBase = new[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 20, 24, 28, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224
                , 0
            };

        internal static readonly sbyte[] LengthCode = new sbyte[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 12, 12, 13, 13, 13, 13, 14, 14, 14, 14, 15, 15
                , 15, 15, 16, 16, 16, 16, 16, 16, 16, 16, 17, 17, 17, 17, 17, 17, 17, 17, 18, 18, 18, 18, 18, 18, 18, 18, 
                19, 19, 19, 19, 19, 19, 19, 19, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 21, 21, 21
                , 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 
                22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24
                , 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 25, 25, 
                25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25
                , 25, 25, 25, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 
                26, 26, 26, 26, 26, 26, 26, 26, 26, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27
                , 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28
            };

        internal short[] DynTree; // the dynamic tree

        internal int MaxCode; // largest code with non zero frequency

        internal StaticTree StaticTree; // the corresponding static tree

        private const int HeapSize = 2 * InternalConstants.LCodes + 1;

        private static readonly sbyte[] DistCode = new sbyte[]
            {
                0, 1, 2, 3, 4, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 9, 9, 9, 9, 10, 10, 10
                , 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 
                11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12
                , 12, 12, 12, 12, 12, 12, 12, 12, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 
                13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14
                , 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 
                14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 15, 15
                , 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 
                15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15
                , 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 16, 17, 18, 18, 19, 19, 20, 20, 20, 20, 21, 21, 21, 21, 22, 22
                , 22, 22, 22, 22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 
                24, 24, 24, 24, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 26, 26, 26, 26, 26, 26, 26
                , 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 27, 
                27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27
                , 27, 27, 27, 27, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 
                28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28
                , 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 
                29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29
                , 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29
            };

        /// <summary>Map from a distance to a distance code.</summary>
        /// <remarks>No side effects. _dist_code[256] and _dist_code[257] are never used.</remarks>
        internal static int DistanceCode(int dist)
        {
            return (dist < 256) ? DistCode[dist] : DistCode[256 + SharedUtils.UrShift(dist, 7)];
        }

        internal void BuildTree(DeflateManager s)
        {
            short[] tree = this.DynTree;
            short[] stree = this.StaticTree.TreeCodes;
            int elems = this.StaticTree.Elems;
            int n; // iterate over heap elements
            int maxCode = -1; // largest code with non zero frequency
            int node; // new node being created

            // Construct the initial heap, with least frequent element in heap[1]. The sons of heap[n] are heap[2*n] and
            // heap[2*n+1]. heap[0] is not used.
            s.HeapLen = 0;
            s.HeapMax = HeapSize;

            for (n = 0; n < elems; n++)
            {
                if (tree[n * 2] != 0)
                {
                    s.Heap[++s.HeapLen] = maxCode = n;
                    s.Depth[n] = 0;
                }
                else
                {
                    tree[n * 2 + 1] = 0;
                }
            }

            // The pkzip format requires that at least one distance code exists, and that at least one bit should be
            // sent even if there is only one possible code. So to avoid special checks later on we force at least two
            // codes of non zero frequency.
            while (s.HeapLen < 2)
            {
                node = s.Heap[++s.HeapLen] = maxCode < 2 ? ++maxCode : 0;
                tree[node * 2] = 1;
                s.Depth[node] = 0;
                s.OptLen--;
                if (stree != null)
                {
                    s.StaticLen -= stree[node * 2 + 1];
                }

                // node is 0 or 1 so it does not have extra bits
            }

            this.MaxCode = maxCode;

            // The elements heap[heap_len/2+1 .. heap_len] are leaves of the tree, establish sub-heaps of increasing
            // lengths:
            for (n = s.HeapLen / 2; n >= 1; n--)
            {
                s.Pqdownheap(tree, n);
            }

            // Construct the Huffman tree by repeatedly combining the least two frequent nodes.
            node = elems; // next internal node of the tree
            do
            {
                // n = node of least frequency
                n = s.Heap[1];
                s.Heap[1] = s.Heap[s.HeapLen--];
                s.Pqdownheap(tree, 1);
                int m = s.Heap[1]; // iterate over heap elements

                s.Heap[--s.HeapMax] = n; // keep the nodes sorted by frequency
                s.Heap[--s.HeapMax] = m;

                // Create a new node father of n and m
                tree[node * 2] = unchecked((short)(tree[n * 2] + tree[m * 2]));
                s.Depth[node] = (sbyte)(Math.Max((byte)s.Depth[n], (byte)s.Depth[m]) + 1);
                tree[n * 2 + 1] = tree[m * 2 + 1] = (short)node;

                // and insert the new node in the heap
                s.Heap[1] = node++;
                s.Pqdownheap(tree, 1);
            }
            while (s.HeapLen >= 2);

            s.Heap[--s.HeapMax] = s.Heap[1];

            // At this point, the fields freq and dad are set. We can now generate the bit lengths.
            this.GenBitlen(s);

            // The field len is now set, we can generate the bit codes
            GenCodes(tree, maxCode, s.BlCount);
        }

        private static int BiReverse(int code, int len)
        {
            int res = 0;
            do
            {
                res |= code & 1;
                code >>= 1; // SharedUtils.URShift(code, 1);
                res <<= 1;
            }
            while (--len > 0);
            return res >> 1;
        }

        private static void GenCodes(IList<short> tree, int maxCode, IList<short> blCount)
        {
            var nextCode = new short[InternalConstants.MaxBits + 1]; // next code value for each bit length
            short code = 0; // running code value
            int bits; // bit index
            int n; // code index

            // The distribution counts are first used to generate the code values without bit reversal.
            for (bits = 1; bits <= InternalConstants.MaxBits; bits++)
            {
                unchecked
                {
                    nextCode[bits] = code = (short)((code + blCount[bits - 1]) << 1);
                }
            }

            // Check that the bit counts in bl_count are consistent. The last code must be all ones. Assert (code +
            // bl_count[MAX_BITS]-1 == (1<<MAX_BITS)-1, "inconsistent bit counts"); Tracev((stderr,"\ngen_codes:
            // max_code %d ", max_code));

            for (n = 0; n <= maxCode; n++)
            {
                int len = tree[n * 2 + 1];
                if (len == 0)
                {
                    continue;
                }

                // Now reverse the bits
                tree[n * 2] = unchecked((short)BiReverse(nextCode[len]++, len));
            }
        }

        /// <param name="s">
        /// </param>
        private void GenBitlen(DeflateManager s)
        {
            short[] tree = this.DynTree;
            short[] stree = this.StaticTree.TreeCodes;
            int[] extra = this.StaticTree.ExtraBits;
            int baseRenamed = this.StaticTree.ExtraBase;
            int maxLength = this.StaticTree.MaxLength;
            int h; // heap index
            int n; // iterate over the tree elements
            int bits; // bit length
            int overflow = 0; // number of elements with bit length too large

            for (bits = 0; bits <= InternalConstants.MaxBits; bits++)
            {
                s.BlCount[bits] = 0;
            }

            // In a first pass, compute the optimal bit lengths (which may overflow in the case of the bit length tree).
            tree[s.Heap[s.HeapMax] * 2 + 1] = 0; // root of the heap

            for (h = s.HeapMax + 1; h < HeapSize; h++)
            {
                n = s.Heap[h];
                bits = tree[tree[n * 2 + 1] * 2 + 1] + 1;
                if (bits > maxLength)
                {
                    bits = maxLength;
                    overflow++;
                }

                tree[n * 2 + 1] = (short)bits;

                // We overwrite tree[n*2+1] which is no longer needed
                if (n > this.MaxCode)
                {
                    continue; // not a leaf node
                }

                s.BlCount[bits]++;
                int xbits = 0; // extra bits
                if (n >= baseRenamed)
                {
                    xbits = extra[n - baseRenamed];
                }

                short f = tree[n * 2]; // frequency
                s.OptLen += f * (bits + xbits);
                if (stree != null)
                {
                    s.StaticLen += f * (stree[n * 2 + 1] + xbits);
                }
            }

            if (overflow == 0)
            {
                return;
            }

            // This happens for example on obj2 and pic of the Calgary corpus Find the first bit length which could
            // increase:
            do
            {
                bits = maxLength - 1;
                while (s.BlCount[bits] == 0)
                {
                    bits--;
                }

                s.BlCount[bits]--; // move one leaf down the tree
                s.BlCount[bits + 1] = (short)(s.BlCount[bits + 1] + 2); // move one overflow item as its brother
                s.BlCount[maxLength]--;

                // The brother of the overflow item also moves one step up, but this does not affect
                // bl_count[max_length]
                overflow -= 2;
            }
            while (overflow > 0);

            for (bits = maxLength; bits != 0; bits--)
            {
                n = s.BlCount[bits];
                while (n != 0)
                {
                    int m = s.Heap[--h]; // iterate over the tree elements
                    if (m > this.MaxCode)
                    {
                        continue;
                    }

                    if (tree[m * 2 + 1] != bits)
                    {
                        s.OptLen = (int)(s.OptLen + (bits - (long)tree[m * 2 + 1]) * tree[m * 2]);
                        tree[m * 2 + 1] = (short)bits;
                    }

                    n--;
                }
            }
        }

        // The lengths of the bit length codes are sent in order of decreasing probability, to avoid transmitting the
        // lengths for unused bit length codes.

        /*
        internal const int BufSize = 8 * 2;
*/

        // Compute the optimal bit lengths for a tree and update the total bit length for the current block. IN
        // assertion: the fields freq and dad are set, heap[heap_max] and above are the tree nodes sorted by increasing
        // frequency. OUT assertions: the field len is set to the optimal bit length, the array bl_count contains the
        // frequencies for each bit length. The length opt_len is updated; static_len is also updated if stree is not
        // null.

        // Reverse the first len bits of a code, using straightforward code (a faster method would use a table) IN
        // assertion: 1 <= len <= 15
    }
}