//-----------------------------------------------------------------------
// <copyright file="RawSerializer.cs" project="SevenUpdate.Installer" assembly="SevenUpdate.Installer" solution="SevenUpdate.Installer" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Tar
{
    using System.Runtime.InteropServices;

    /// <summary>This class is intended for internal use only, by the Tar library.</summary><typeparam name="T"></typeparam>
    internal sealed class RawSerializer<T>
    {
        #region Public Methods

        /// <param name="rawData"></param><param name="position"></param> <returns></returns>
        public T RawDeserialize(byte[] rawData, int position = 0)
        {
            var rawSize = Marshal.SizeOf(typeof(T));
            if (rawSize > rawData.Length)
            {
                return default(T);
            }

            var buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.Copy(rawData, position, buffer, rawSize);
            var obj = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return obj;
        }

        /// <param name="item">
        /// </param><returns></returns>
        public byte[] RawSerialize(T item)
        {
            var rawSize = Marshal.SizeOf(typeof(T));
            var buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(item, buffer, false);
            var rawData = new byte[rawSize];
            Marshal.Copy(buffer, rawData, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawData;
        }

        #endregion
    }
}
