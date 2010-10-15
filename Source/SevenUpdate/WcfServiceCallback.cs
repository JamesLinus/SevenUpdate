// ***********************************************************************
// <copyright file="WcfServiceCallback.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
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
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System;

    using SevenUpdate.Service;

    /// <summary>Contains callback methods for WCF</summary>
    internal sealed class WcfServiceCallback : IWcfServiceCallback
    {
        #region Events

        /// <summary>Occurs when the download completed.</summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadDone;

        /// <summary>Occurs when the download progress changed</summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>Occurs when an error has occurred when downloading or installing updates</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>Occurs when the installation completed.</summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallDone;

        /// <summary>Occurs when the installation progress changed</summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;

        #endregion

        #region Implementation of IWcfServiceCallback

        /// <summary>Occurs when the download of updates has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            DownloadDone(this, e);
        }

        /// <summary>Occurs when the download progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged(this, e);
        }

        /// <summary>Occurs when a error occurs when downloading or installing updates</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            ErrorOccurred(this, e);
        }

        /// <summary>Occurs when the installation of updates has completed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnInstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            InstallDone(this, e);
        }

        /// <summary>Occurs when the install progress has changed</summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event data</param>
        public void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            InstallProgressChanged(this, e);
        }

        #endregion
    }
}