// ***********************************************************************
// <copyright file="FileTime.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

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
