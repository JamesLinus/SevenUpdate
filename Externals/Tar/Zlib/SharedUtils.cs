// <copyright file="SharedUtils.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Zlib
{
    /// <summary>
    /// </summary>
    internal static class SharedUtils
    {
        /// <summary>Performs an unsigned bitwise right shift with the specified number</summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static int UrShift(int number, int bits)
        {
            return (int)((uint)number >> bits);
        }

        /*
        /// <summary>Reads a number of characters from the current source TextReader and writesthe data to the target array at the specified index.</summary>
        /// <param name="sourceTextReader">The source TextReader to read from</param>
        /// <param name="target">Contains the array of characteres read from the source TextReader.</param>
        /// <param name="start">The starting index of the target array.</param>
        /// <param name="count">The maximum number of characters to read from the source TextReader.</param>
        /// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source TextReader. Returns -1 if the end of the stream isreached.</returns>
        public static int ReadInput(TextReader sourceTextReader, byte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0)
            {
                return 0;
            }

            var charArray = new char[target.Length];
            var bytesRead = sourceTextReader.Read(charArray, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0)
            {
                return -1;
            }

            for (var index = start; index < start + bytesRead; index++)
            {
                target[index] = (byte)charArray[index];
            }

            return bytesRead;
        }
*/

        /*
        internal static byte[] ToByteArray(string source)
        {
            return Encoding.UTF8.GetBytes(source);
        }
*/

        /*
        internal static char[] ToCharArray(byte[] array)
        {
            return Encoding.UTF8.GetChars(array);
        }
*/
    }
}