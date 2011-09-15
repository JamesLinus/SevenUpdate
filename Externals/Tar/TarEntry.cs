//-----------------------------------------------------------------------
// <copyright file="TarEntry.cs" project="SevenUpdate.Installer" assembly="SevenUpdate.Installer" solution="SevenUpdate.Installer" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Tar
{
    using System;

    /// <summary>Represents an entry in a TAR archive.</summary>
    public sealed class TarEntry
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the TarEntry class.</summary>
        internal TarEntry()
        {
        }

        #endregion

        #region Properties

        /// <summary>Gets the last-modified time on the file or directory.</summary>
        public DateTime Mtime { get; internal set; }

        /// <summary>Gets the name of the file contained within the entry</summary>
        public string Name { get; internal set; }

        /// <summary>Gets the size of the file contained within the entry. If the entry is a directory, this is zero.</summary>
        public int Size { get; internal set; }

        /// <summary>Gets the type of the entry.</summary>
        public TarEntryType @Type { get; internal set; }

        #endregion

        /*
        /// <summary>Gets a char representation of the type of the entry.</summary>
        public char TypeChar
        {
            get
            {
                switch (this.Type)
                {
                    case TarEntryType.FileOld:
                    case TarEntryType.File:
                    case TarEntryType.FileContiguous:
                        return 'f';
                    case TarEntryType.HardLink:
                        return 'l';
                    case TarEntryType.SymbolicLink:
                        return 's';
                    case TarEntryType.CharSpecial:
                        return 'c';
                    case TarEntryType.BlockSpecial:
                        return 'b';
                    case TarEntryType.Directory:
                        return 'd';
                    case TarEntryType.Fifo:
                        return 'p';
                    case TarEntryType.GnuLongLink:
                    case TarEntryType.GnuLongName:
                    case TarEntryType.GnuSparseFile:
                    case TarEntryType.GnuVolumeHeader:
                        return (char)this.Type;
                    default:
                        return '?';
                }
            }
        }
*/
    }
}
