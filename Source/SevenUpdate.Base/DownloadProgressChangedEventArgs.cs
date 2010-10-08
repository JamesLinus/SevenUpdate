// ***********************************************************************
// <copyright file="DownloadProgressChangedEventArgs.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate
{
    using System;

    /// <summary>
    /// Provides event data for the DownloadProgressChanged event
    /// </summary>
    public sealed class DownloadProgressChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="bytesTransferred">
        /// the number of bytes transferred
        /// </param>
        /// <param name="bytesTotal">
        /// the total number of bytes to download
        /// </param>
        /// <param name="filesTransferred">
        /// the number of files transfered
        /// </param>
        /// <param name="filesTotal">
        /// the total number of files transfered
        /// </param>
        public DownloadProgressChangedEventArgs(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal)
        {
            this.BytesTotal = bytesTotal;
            this.BytesTransferred = bytesTransferred;
            this.FilesTotal = filesTotal;
            this.FilesTransferred = filesTransferred;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the total number of bytes to download
        /// </summary>
        /// <value>The bytes total.</value>
        public ulong BytesTotal { get; private set; }

        /// <summary>
        ///   Gets the number of bytes transferred
        /// </summary>
        /// <value>The bytes transferred.</value>
        public ulong BytesTransferred { get; private set; }

        /// <summary>
        ///   Gets the total number of files to download
        /// </summary>
        /// <value>The files total.</value>
        public uint FilesTotal { get; private set; }

        /// <summary>
        ///   Gets the number of files downloaded
        /// </summary>
        /// <value>The files transferred.</value>
        public uint FilesTransferred { get; private set; }

        #endregion
    }
}