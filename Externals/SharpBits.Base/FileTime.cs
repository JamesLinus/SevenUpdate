// <copyright file="FileTime.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>This structure is a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
    internal struct FileTime
    {
        /// <summary>Specifies the low 32 bits of the file time.</summary>
        public readonly uint DWLowDateTime;

        /// <summary>Specifies the high 32 bits of the file time.</summary>
        public readonly uint DWHighDateTime;
    }
}