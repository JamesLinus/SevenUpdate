//-----------------------------------------------------------------------
// <copyright file="Tar.cs" project="SevenUpdate.Installer" assembly="SevenUpdate.Installer" solution="SevenUpdate.Installer" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Tar
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    using GZipStream = Zlib.GZipStream;

    /// <summary>A class to create, list, or extract TAR archives. This is the primary, central class for the Tar library.</summary><remarks>Bugs: <list type = "bullet"> <item>does not read or write bzip2 compressed tarballs  (.tar.bz2)</item> <item>uses Marshal.StructureToPtr and thus requires a LinkDemand, full trust.d </item></list></remarks>
    public class Tar
    {
        #region Constants and Fields

        private Stream outFS;

        private RawSerializer<HeaderBlock> s;

        #endregion

        #region Properties

        private RawSerializer<HeaderBlock> Serializer
        {
            get
            {
                return this.s ?? (this.s = new RawSerializer<HeaderBlock>());
            }
        }

        private Options TarOptions { get; set; }

        #endregion

        #region Public Methods

        /// <summary>Create a tar archive with the given name, and containing the given set of files or directories, and using GZIP compression by default.</summary>
        /// <param name="outputFile">The name of the tar archive to create. The file must not exist at the time of the call.</param>
        /// <param name="filesOrDirectories">A list of filenames and/or directory names to be added to the archive.</param>
        /// <param name="options">The options to use during Tar operation.</param>
        public static void CreateArchive(
            string outputFile, IEnumerable<string> filesOrDirectories, Options options = null)
        {
            if (options == null)
            {
                options = new Options { Compression = TarCompression.GZip };
            }

            var tar = new Tar { TarOptions = options };
            tar.InternalCreateArchive(outputFile, filesOrDirectories);
        }

        /// <summary>Extract the named tar archive to a specified directory</summary>
        /// <param name="archive">The name of the tar archive to extract.</param>
        /// <param name="extractDirectory">The directory to extract the archive</param>
        /// <param name="options">A set of options for extracting.</param>
        /// <returns>A <c>ReadOnlyCollection</c> of TarEntry instances contained within the archive.</returns><example><code lang = "C#"> // extract a regular tar archive, placing files in the current dir: Tar.Extract("MyArchive.tar"); // extract a compressed tar archive, placing files in the current dir: Tar.Extract("Archive2.tgz")</code></example>
        public static ReadOnlyCollection<TarEntry> Extract(
            string archive, string extractDirectory, Options options = null)
        {
            return ListOrExtract(archive, extractDirectory, true, options).AsReadOnly();
        }

        /// <summary>Get a list of the TarEntry items contained within the named archive.</summary>
        /// <param name="archive">The name of the tar archive.</param>
        /// <returns>A <c>ReadOnlyCollection</c> of TarEntry instances contained within the archive.</returns><example><code lang = "C#"> private void ListContents(string archiveName) { var list = Tar.List(archiveName); foreach (var item in list) { Console.WriteLine("{0,-20}  {1,9}  {2}", item.Mtime.ToString("u"), item.Size, item.Name); } Console.WriteLine(new String('-', 66)); Console.WriteLine("                                 {0} entries", list.Count); }</code></example>
        public static ReadOnlyCollection<TarEntry> List(string archive)
        {
            return ListOrExtract(archive, null, false, null).AsReadOnly();
        }

        /// <summary>Adds a directory to the tar archive</summary>
        /// <param name="dirName">The path to the directory</param><param name="parent"></param>
        public void AddDirectory(string dirName, string parent = null)
        {
            if (parent == null)
            {
                parent = dirName;
            }

            // insure trailing slash
            if (!dirName.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                dirName += Path.DirectorySeparatorChar;
            }

            if (this.TarOptions.StatusWriter != null)
            {
                this.TarOptions.StatusWriter.WriteLine("{0}", dirName);
            }

            var dir = dirName.Replace(parent, null).TrimSlash();
            if (dir != string.Empty)
            {
                // add the block for the dir, right here.
                var hb = HeaderBlock.CreateHeaderBlock();
                hb.InsertName(dir, dirName);
                hb.TypeFlag = 5 + (byte)'0';
                hb.SetSize(0); // some impls use agg size of all files contained
                hb.SetChksum();
                var block = this.Serializer.RawSerialize(hb);
                this.outFS.Write(block, 0, block.Length);
            }

            // add the files:
            var filenames = Directory.GetFiles(dirName);
            foreach (var filename in filenames)
            {
                this.AddFile(filename, dir);
            }

            // add the subdirectories:
            var dirnames = Directory.GetDirectories(dirName);
            foreach (var d in dirnames)
            {
                // handle reparse points
                var a = File.GetAttributes(d);
                if ((a & FileAttributes.ReparsePoint) == 0)
                {
                    // not a symlink
                    this.AddDirectory(d, Path.GetDirectoryName(dirName));
                }
                else if (this.TarOptions.FollowSymLinks)
                {
                    // isa symlink, and we want to follow it
                    this.AddDirectory(d, Path.GetDirectoryName(dirName));
                }
                else
                {
                    // not following symlinks; add it
                    this.AddSymlink(d);
                }
            }
        }

        /// <summary>Adds a file to the tar Archive</summary>
        /// <param name="fullName">The file to add to the archive</param><param name="directory"></param>
        public void AddFile(string fullName, string directory = null)
        {
            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.GetDirectoryName(fullName);
            }

            // is it a symlink (ReparsePoint)?
            var a = File.GetAttributes(fullName);
            if ((a & FileAttributes.ReparsePoint) != 0)
            {
                this.AddSymlink(fullName);
                return;
            }

            if (this.TarOptions.StatusWriter != null)
            {
                this.TarOptions.StatusWriter.WriteLine("{0}", fullName);
            }

            var hb = HeaderBlock.CreateHeaderBlock();

            var file = Path.Combine(directory, Path.GetFileName(fullName));

            if (file == fullName)
            {
                file = Path.GetFileName(fullName);
            }

            hb.InsertName(file, fullName);
            hb.TypeFlag = (byte)TarEntryType.File; // 0 + (byte)'0' ;
            var fi = new FileInfo(fullName);
            hb.SetSize((int)fi.Length);
            hb.SetChksum();
            var block = this.Serializer.RawSerialize(hb);
            this.outFS.Write(block, 0, block.Length);

            using (var fs = File.Open(fullName, FileMode.Open, FileAccess.Read))
            {
                Array.Clear(block, 0, block.Length);
                while (fs.Read(block, 0, block.Length) > 0)
                {
                    this.outFS.Write(block, 0, block.Length); // not n!!
                    Array.Clear(block, 0, block.Length);
                }
            }
        }

        #endregion

        #region Methods

        /// <param name="archive"></param><param name="extractDirectory"></param> <param name="wantExtract"></param><param name="options"></param><returns></returns>
        private static List<TarEntry> ListOrExtract(
            string archive, string extractDirectory, bool wantExtract, Options options)
        {
            var t = new Tar { TarOptions = options ?? new Options() };
            return t.InternalListOrExtract(archive, extractDirectory, wantExtract);
        }

        /// <summary>Adds a symbolic link to a Tar archive</summary>
        /// <param name="name">The filename of the symbolic link</param>
        private void AddSymlink(string name)
        {
            if (this.TarOptions.StatusWriter != null)
            {
                this.TarOptions.StatusWriter.WriteLine("{0}", name);
            }

            // add the block for the symlink, right here.
            var hb = HeaderBlock.CreateHeaderBlock();
            hb.InsertName(name, null);
            hb.InsertLinkName(name);
            hb.TypeFlag = (byte)TarEntryType.SymbolicLink;
            hb.SetSize(0);
            hb.SetChksum();
            var block = this.Serializer.RawSerialize(hb);
            this.outFS.Write(block, 0, block.Length);
        }

        /// <param name="outputFile"></param><param name="files"></param> <exception cref = "InvalidOperationException"></exception><exception cref = "InvalidOperationException"></exception>
        /// <exception cref = "InvalidOperationException"></exception><exception cref = "InvalidOperationException"></exception> <exception cref = "InvalidOperationException"></exception>
        private void InternalCreateArchive(string outputFile, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                throw new InvalidOperationException("You must specify an output file.");
            }

            if (File.Exists(outputFile))
            {
                throw new InvalidOperationException("The output file you specified already exists.");
            }

            if (Directory.Exists(outputFile))
            {
                throw new InvalidOperationException("The output file you specified is a directory.");
            }

            var fcount = 0;
            try
            {
                using (this.outFS = this.InternalGetOutputArchiveStream(outputFile))
                {
                    foreach (var f in files)
                    {
                        fcount++;

                        if (Directory.Exists(f))
                        {
                            this.AddDirectory(f);
                        }
                        else if (File.Exists(f))
                        {
                            this.AddFile(f);
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                string.Format("The file you specified ({0}) was not found.", f));
                        }
                    }

                    if (fcount < 1)
                    {
                        throw new InvalidOperationException(
                            "Specify one or more input files to place into the archive.");
                    }

                    // terminator
                    var block = new byte[512];
                    this.outFS.Write(block, 0, block.Length);
                    this.outFS.Write(block, 0, block.Length);
                }
            }
            finally
            {
                if (fcount < 1)
                {
                    try
                    {
                        File.Delete(outputFile);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <param name="name"></param><param name="extractDirectory"></param> <returns></returns>
        private Stream InternalGetExtractOutputStream(string name, string extractDirectory)
        {
            if (this.TarOptions.Overwrite || !File.Exists(name))
            {
                if (this.TarOptions.StatusWriter != null)
                {
                    this.TarOptions.StatusWriter.WriteLine("{0}", name);
                }

                var file = Path.Combine(extractDirectory, name);
                var directory = Path.GetDirectoryName(file);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return File.Open(file, FileMode.Create, FileAccess.ReadWrite);
            }

            if (this.TarOptions.StatusWriter != null)
            {
                this.TarOptions.StatusWriter.WriteLine("{0} (not overwriting)", name);
            }

            return null;
        }

        /// <param name="archive">
        /// </param><returns></returns>
        private Stream InternalGetInputStream(string archive)
        {
            if (archive.EndsWith(".tgz") || archive.EndsWith(".tar.gz") ||
                this.TarOptions.Compression == TarCompression.GZip)
            {
                var fs = File.Open(archive, FileMode.Open, FileAccess.Read);
                return new GZipStream(fs, CompressionMode.Decompress, false);
            }

            return File.Open(archive, FileMode.Open, FileAccess.Read);
        }

        /// <param name="filename">
        /// </param>
        /// <returns></returns> <exception cref = "Exception"></exception>
        private Stream InternalGetOutputArchiveStream(string filename)
        {
            switch (this.TarOptions.Compression)
            {
                case TarCompression.None:
                    return File.Open(filename, FileMode.Create, FileAccess.ReadWrite);

                case TarCompression.GZip:
                    {
                        var fs = File.Open(filename, FileMode.Create, FileAccess.ReadWrite);
                        return new GZipStream(fs, CompressionMode.Compress);
                    }

                default:
                    throw new Exception("bad state");
            }
        }

        /// <param name="archive"></param><param name="extractDirectory"></param> <param name="wantExtract"></param>
        /// <returns></returns>
        private List<TarEntry> InternalListOrExtract(string archive, string extractDirectory, bool wantExtract)
        {
            var entryList = new List<TarEntry>();
            var block = new byte[512];
            var blocksToMunch = 0;
            var remainingBytes = 0;
            Stream output = null;
            var mtime = DateTime.Now;
            string name = null;
            TarEntry entry = null;
            var deferredDirTimestamp = new Dictionary<string, DateTime>();

            if (!File.Exists(archive))
            {
                throw new FileNotFoundException("The archive does not exist", archive);
            }

            using (var fs = this.InternalGetInputStream(archive))
            {
                while (fs.Read(block, 0, block.Length) > 0)
                {
                    if (blocksToMunch > 0)
                    {
                        if (output != null)
                        {
                            var bytesToWrite = (block.Length < remainingBytes) ? block.Length : remainingBytes;

                            output.Write(block, 0, bytesToWrite);
                            remainingBytes -= bytesToWrite;
                        }

                        blocksToMunch--;

                        if (blocksToMunch == 0)
                        {
                            if (output != null)
                            {
                                if (output is MemoryStream)
                                {
                                    entry.Name =
                                        name = Encoding.ASCII.GetString((output as MemoryStream).ToArray()).TrimNull();
                                }

                                output.Close();
                                output.Dispose();

                                if (output is FileStream && !this.TarOptions.DoNotSetTime)
                                {
                                    File.SetLastWriteTimeUtc(Path.Combine(extractDirectory, name), mtime);
                                }

                                output = null;
                            }
                        }

                        continue;
                    }

                    var hb = this.Serializer.RawDeserialize(block);

                    if (!hb.VerifyChksum())
                    {
                        throw new Exception("header checksum is invalid.");
                    }

                    // if this is the first entry, or if the prior entry is not a GnuLongName
                    if (entry == null || entry.Type != TarEntryType.GnuLongName)
                    {
                        name = hb.GetName();
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        break; // EOF
                    }

                    mtime = hb.GetMtime();
                    remainingBytes = hb.GetSize();

                    if (hb.TypeFlag == 0)
                    {
                        hb.TypeFlag = (byte)'0'; // coerce old-style GNU type to posix tar type
                    }

                    entry = new TarEntry
                        {
                            Name = name,
                            Mtime = mtime,
                            Size = remainingBytes,
                            @Type = (TarEntryType)hb.TypeFlag
                        };

                    if (entry.Type != TarEntryType.GnuLongName)
                    {
                        entryList.Add(entry);
                    }

                    blocksToMunch = (remainingBytes > 0) ? ((remainingBytes - 1) / 512) + 1 : 0;

                    if (entry.Type == TarEntryType.GnuLongName)
                    {
                        if (name != "././@LongLink")
                        {
                            if (wantExtract)
                            {
                                throw new Exception(
                                    string.Format(
                                        "unexpected name for type 'L' (expected '././@LongLink', got '{0}')", name));
                            }
                        }

                        // for GNU long names, we extract the long name info into a memory stream
                        output = new MemoryStream();
                        continue;
                    }

                    if (!wantExtract)
                    {
                    }
                    else
                    {
                        switch (entry.Type)
                        {
                            case TarEntryType.Directory:
                                if (!Directory.Exists(Path.Combine(extractDirectory, name)))
                                {
                                    Directory.CreateDirectory(Path.Combine(extractDirectory, name));

                                    // cannot set the time on the directory now, or it will be updated
                                    // by future file writes.  Defer until after all file writes are done.
                                    if (!this.TarOptions.DoNotSetTime)
                                    {
                                        deferredDirTimestamp.Add(
                                            Path.Combine(extractDirectory, name).TrimSlash(), mtime);
                                    }
                                }
                                else if (this.TarOptions.Overwrite)
                                {
                                    if (!this.TarOptions.DoNotSetTime)
                                    {
                                        deferredDirTimestamp.Add(
                                            Path.Combine(extractDirectory, name).TrimSlash(), mtime);
                                    }
                                }

                                break;

                            case TarEntryType.FileOld:
                            case TarEntryType.File:
                            case TarEntryType.FileContiguous:
                                var p = Path.GetDirectoryName(Path.Combine(extractDirectory, name));
                                if (!string.IsNullOrEmpty(p))
                                {
                                    if (!Directory.Exists(p))
                                    {
                                        Directory.CreateDirectory(p);
                                    }
                                }

                                output = this.InternalGetExtractOutputStream(name, extractDirectory);
                                break;

                            case TarEntryType.GnuVolumeHeader:
                            case TarEntryType.CharSpecial:
                            case TarEntryType.BlockSpecial:

                                // do nothing on extract
                                break;

                            case TarEntryType.SymbolicLink:
                                break;

                            default:
                                throw new Exception(string.Format("unsupported entry type ({0})", hb.TypeFlag));
                        }
                    }
                }
            }

            // apply the deferred timestamps on the directories
            if (deferredDirTimestamp.Count > 0)
            {
                foreach (var key in deferredDirTimestamp.Keys)
                {
                    Directory.SetLastWriteTimeUtc(key, deferredDirTimestamp[key]);
                }
            }

            return entryList;
        }

        #endregion

        /*
        [DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode)]
        private static extern int CreateSymbolicLink(string symLinkFileName, string targetFileName, int flags);
*/
    }
}
