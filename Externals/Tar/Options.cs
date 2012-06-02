// <copyright file="Options.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Tar
{
    using System.IO;

    /// <summary>Specifies the options to use for tar creation or extraction</summary>
    public sealed class Options
    {
        /// <summary>Gets or sets the compression to use. Applies only during archive creation. Ignored during extraction.</summary>
        public TarCompression Compression { get; set; }

        /// <summary>Gets or sets a value indicating whether the modified times of the extracted entries is NOT set according to the time set in the archive.  By default, the modified time is set.</summary>
        public bool DoNotSetTime { get; set; }

        /// <summary>Gets or sets a value indicating whether to follow symbolic links when creating archives.</summary>
        public bool FollowSymLinks { get; set; }

        /// <summary>Gets or sets a value indicating whether to overwrite existing files when extracting archives.</summary>
        public bool Overwrite { get; set; }

        /// <summary>Gets or sets a TextWriter to which verbose status messages will be  written during operation.</summary>
        /// <remarks><para>Use this to see messages emitted by the Tar logic. You can use this whether Extracting or creating an archive.</para></remarks><example><code lang = "C#"> var options = new Tar.Options(); options.StatusWriter = Console.Out; Tar.Extract("Archive2.tgz", options);</code></example>
        public TextWriter StatusWriter { get; set; }
    }
}