//-----------------------------------------------------------------------
// <copyright file="TarEntryType.cs" project="SevenUpdate.Installer" assembly="SevenUpdate.Installer" solution="SevenUpdate.Installer" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Tar
{
    /// <summary>the type of Tar Entry</summary>
    public enum TarEntryType : byte
    {
        /// <summary>a file (old version)</summary>
        FileOld = 0,

        /// <summary>a file</summary>
        File = 48,

        /// <summary>a hard link</summary>
        HardLink = 49,

        /// <summary>a symbolic link</summary>
        SymbolicLink = 50,

        /// <summary>a char special device</summary>
        CharSpecial = 51,

        /// <summary>a block special device</summary>
        BlockSpecial = 52,

        /// <summary>a directory</summary>
        Directory = 53,

        /// <summary>a pipe</summary>
        Fifo = 54,

        /// <summary>Contiguous file</summary>
        FileContiguous = 55,

        /// <summary>a GNU Long name?</summary>
        GnuLongLink = (byte)'K', // "././@LongLink"

        /// <summary>a GNU Long name?</summary>
        GnuLongName = (byte)'L', // "././@LongLink"

        /// <summary>a GNU sparse file</summary>
        GnuSparseFile = (byte)'S',

        /// <summary>a GNU volume header</summary>
        GnuVolumeHeader = (byte)'V',
    }
}
