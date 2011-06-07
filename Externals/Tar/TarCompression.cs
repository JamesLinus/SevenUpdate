//-----------------------------------------------------------------------
// <copyright file="TarCompression.cs" project="SevenUpdate.Installer" assembly="SevenUpdate.Installer" solution="SevenUpdate.Installer" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Tar
{
    /// <summary>Represents an entry in a TAR archive.</summary>
    public enum TarCompression
    {
        /// <summary>No compression - just a vanilla tar.</summary>
        None = 0,

        /// <summary>GZIP compression is applied to the tar to produce a .tgz file</summary>
        GZip,
    }
}