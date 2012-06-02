// <copyright file="IBackgroundCopyFile.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///   The IBackgroundCopyFile interface contains information about a file that is part of a job. For example, you
    ///   can use the interfaces methods to retrieve the local and remote names of the file and transfer progress
    ///   information.
    /// </summary>
    [Guid("01B7BD23-FB88-4A77-8490-5891D3E4653A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IBackgroundCopyFile
    {
        /// <summary>Retrieves the remote name of the file.</summary>
        /// <param name="val">A string that contains the remote name of the file to transfer. The name is fully qualified. Call the CoTaskMemFree function to free ppName when done. .</param>
        void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>Retrieves the local name of the file.</summary>
        /// <param name="val">A string that contains the name of the file on the client.</param>
        void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>Retrieves the progress of the file transfer.</summary>
        /// <param name="val">Structure whose members indicate the progress of the file transfer. For details on the type of progress information available, see the BG_FILEProgress structure.</param>
        void GetProgress(out BGFileProgress val);
    }
}