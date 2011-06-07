// ***********************************************************************
// <copyright file="IBackgroundCopyFile2.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///   The IBackgroundCopyFile2 interface contains information about a file that is part of a job. The
    ///   IBackgroundCopyFile2 interface is used to specify a new remote name for the file and retrieve the list of
    ///   ranges to download.
    /// </summary>
    [Guid("83E81B93-0873-474D-8A8C-F2018B1A939C")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImportAttribute]
    internal interface IBackgroundCopyFile2
    {
        /// <summary>
        ///   Retrieves the remote name of the file.
        /// </summary>
        /// <param name="val">
        ///   A string that contains the remote name of the file to transfer. The name is fully qualified. Call the CoTaskMemFree function to free ppName when done. .
        /// </param>
        void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        ///   Retrieves the local name of the file.
        /// </summary>
        /// <param name="val">
        ///   A string that contains the name of the file on the client.
        /// </param>
        void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        ///   Retrieves the progress of the file transfer.
        /// </summary>
        /// <param name="val">
        ///   Structure whose members indicate the progress of the file transfer. For details on the type of progress information available, see the BG_FILEProgress structure.
        /// </param>
        void GetProgress(out BGFileProgress val);

        /// <summary>
        ///   Retrieves the ranges that could be downloaded from the remote file.
        /// </summary>
        /// <param name="rangeCount">
        ///   Number of elements in Ranges.
        /// </param>
        /// <param name="ranges">
        ///   Array of BG_FILE_RANGE structures that specify the ranges to download.
        /// </param>
        void GetFileRanges(out uint rangeCount, out IntPtr /*BG_FILE_RANGE[]*/ ranges);

        /// <summary>
        ///   Changes the remote name to a new Url in a download job.
        /// </summary>
        /// <param name="remoteName">
        ///   A string that contains the name of the file on the server.
        /// </param>
        void SetRemoteName([MarshalAs(UnmanagedType.LPWStr)] string remoteName);
    }
}