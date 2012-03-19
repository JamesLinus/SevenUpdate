// <copyright file="BGFileInfo.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_FILE_INFO structure provides the local and remote names of the file to transfer.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
    internal struct BGFileInfo
    {
        /// <summary>Remote Name for the File.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string RemoteName;

        /// <summary>Local Name for the file.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string LocalName;
    }
}