// ***********************************************************************
// <copyright file="ServiceCallBack.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate
{
    using System;

    using SevenUpdate.WCF;

    /// <summary>
    /// Contains callback methods for WCF
    /// </summary>
    internal sealed class ServiceCallBack : IServiceCallback
    {
        #region Events

        /// <summary>
        ///   Occurs when the download completed.
        /// </summary>
        public static event EventHandler<DownloadCompletedEventArgs> DownloadDone;

        /// <summary>
        ///   Occurs when the download progress changed
        /// </summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>
        ///   Occurs when an error has occurred when downloading or installing updates
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        ///   Occurs when the installation completed.
        /// </summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallDone;

        /// <summary>
        ///   Occurs when the installation progress changed
        /// </summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;

        #endregion

        #region Implemented Interfaces

        #region IServiceCallback

        /// <summary>
        /// Occurs when the download of updates has completed
        /// </summary>
        /// <param name="errorOccurred">
        /// <c>true</c> if an error occurred, otherwise <c>false</c>
        /// </param>
        public void OnDownloadCompleted(bool errorOccurred)
        {
            DownloadDone(this, new DownloadCompletedEventArgs(errorOccurred));
        }

        /// <summary>
        /// Occurs when the download progress has changed
        /// </summary>
        /// <param name="bytesTransferred">
        /// the number of bytes downloaded
        /// </param>
        /// <param name="bytesTotal">
        /// the total number of bytes to download
        /// </param>
        /// <param name="filesTransferred">
        /// The number of files downloaded
        /// </param>
        /// <param name="filesTotal">
        /// The total number of files to download
        /// </param>
        public void OnDownloadProgressChanged(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal)
        {
            DownloadProgressChanged(this, new DownloadProgressChangedEventArgs(bytesTransferred, bytesTotal, filesTransferred, filesTotal));
        }

        /// <summary>
        /// Occurs when a error occurs when downloading or installing updates
        /// </summary>
        /// <param name="exception">
        /// the exception that occurred
        /// </param>
        /// <param name="type">
        /// the type of error that occurred
        /// </param>
        public void OnErrorOccurred(string exception, ErrorType type)
        {
            ErrorOccurred(this, new ErrorOccurredEventArgs(exception, type));
        }

        /// <summary>
        /// Occurs when the installation of updates has completed
        /// </summary>
        /// <param name="installedUpdates">
        /// the number of updates installed
        /// </param>
        /// <param name="failedUpdates">
        /// the number of failed updates
        /// </param>
        public void OnInstallCompleted(int installedUpdates, int failedUpdates)
        {
            InstallDone(this, new InstallCompletedEventArgs(installedUpdates, failedUpdates));
        }

        /// <summary>
        /// Occurs when the install progress has changed
        /// </summary>
        /// <param name="updateName">
        /// the name of the update being installed
        /// </param>
        /// <param name="progress">
        /// the progress percentage completion
        /// </param>
        /// <param name="updatesComplete">
        /// the number of updates that have already been installed
        /// </param>
        /// <param name="totalUpdates">
        /// the total number of updates being installed
        /// </param>
        public void OnInstallProgressChanged(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            InstallProgressChanged(this, new InstallProgressChangedEventArgs(updateName, progress, updatesComplete, totalUpdates));
        }

        #endregion

        #endregion
    }
}