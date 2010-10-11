// ***********************************************************************
// <copyright file="DownloadProgressChangedEventArgs.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt">GNU General Public License Version 3</license>
// ***********************************************************************
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
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