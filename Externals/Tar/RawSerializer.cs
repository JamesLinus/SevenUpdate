// <copyright file="RawSerializer.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Tar
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>This class is intended for internal use only, by the Tar library.</summary><typeparam name="T"></typeparam>
    internal sealed class RawSerializer<T>
    {
        /// <param name="rawData"></param><param name="position"></param> <returns></returns>
        public T RawDeserialize(byte[] rawData, int position = 0)
        {
            int rawSize = Marshal.SizeOf(typeof(T));
            if (rawSize > rawData.Length)
            {
                return default(T);
            }

            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.Copy(rawData, position, buffer, rawSize);
            var obj = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return obj;
        }

        /// <param name="item">
        /// </param><returns></returns>
        public byte[] RawSerialize(T item)
        {
            int rawSize = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(item, buffer, false);
            var rawData = new byte[rawSize];
            Marshal.Copy(buffer, rawData, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawData;
        }
    }
}