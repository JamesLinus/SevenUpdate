// <copyright file="TarCompression.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

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