// ***********************************************************************
// <copyright file="BGFileInfo.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_FILE_INFO structure provides the local and remote names of the file to transfer</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
    internal struct BGFileInfo
    {
        /// <summary>Remote Name for the File</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string RemoteName;

        /// <summary>Local Name for the file</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string LocalName;
    }
}