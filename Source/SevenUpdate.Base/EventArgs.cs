#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace SevenUpdate
{
    /// <summary>
    ///   Provides event data for the SearchCompleted event
    /// </summary>
    public sealed class SearchCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "applications">The collection of applications to update</param>
        /// <param name = "importantCount">The number of important updates</param>
        /// <param name = "recommendedCount">The number of recommended updates</param>
        /// <param name = "optionalCount">The number of optional updates</param>
        public SearchCompletedEventArgs(IEnumerable<Sui> applications, int importantCount, int recommendedCount, int optionalCount)
        {
            Applications = applications;
            ImportantCount = importantCount;
            OptionalCount = optionalCount;
            RecommendedCount = recommendedCount;
        }

        /// <summary>
        ///   Gets a collection of applications that contain updates to install
        /// </summary>
        public IEnumerable<Sui> Applications { get; private set; }

        public int ImportantCount { get; set; }
        public int OptionalCount { get; set; }
        public int RecommendedCount { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the DownloadCompleted event
    /// </summary>
    public sealed class DownloadCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "errorOccurred"><c>true</c> is an error occurred, otherwise <c>false</c></param>
        public DownloadCompletedEventArgs(bool errorOccurred)
        {
            ErrorOccurred = errorOccurred;
        }

        /// <summary>
        ///   <c>true</c> if an error occurred, otherwise <c>false</c>
        /// </summary>
        public bool ErrorOccurred { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the DownloadProgressChanged event
    /// </summary>
    public sealed class DownloadProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "bytesTransferred">the number of bytes transferred</param>
        /// <param name = "bytesTotal">the total number of bytes to download</param>
        /// <param name = "filesTransferred">the number of files transfered</param>
        /// <param name = "filesTotal">the total number of files transfered</param>
        public DownloadProgressChangedEventArgs(ulong bytesTransferred, ulong bytesTotal, uint filesTransferred, uint filesTotal)
        {
            BytesTotal = bytesTotal;
            BytesTransferred = bytesTransferred;
            FilesTotal = filesTotal;
            FilesTransferred = filesTransferred;
        }

        /// <summary>
        ///   Gets the number of bytes transferred
        /// </summary>
        public ulong BytesTransferred { get; private set; }

        /// <summary>
        ///   Gets the total number of bytes to download
        /// </summary>
        public ulong BytesTotal { get; private set; }

        /// <summary>
        ///   Gets the number of files downloaded
        /// </summary>
        public uint FilesTransferred { get; private set; }

        /// <summary>
        ///   Gets the total number of files to download
        /// </summary>
        public uint FilesTotal { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the InstallCompleted event
    /// </summary>
    public sealed class InstallCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "updatesInstalled">the number of updates installed</param>
        /// <param name = "updatesFailed">the number of updates that failed</param>
        public InstallCompletedEventArgs(int updatesInstalled, int updatesFailed)
        {
            UpdatesInstalled = updatesInstalled;
            UpdatesFailed = updatesFailed;
        }

        /// <summary>
        ///   Gets the number of updates that have been installed
        /// </summary>
        public int UpdatesInstalled { get; private set; }

        /// <summary>
        ///   Gets the number of updates that failed.
        /// </summary>
        public int UpdatesFailed { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the InstallProgressChanged event
    /// </summary>
    public sealed class InstallProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "updateName">the name of the update currently being installed</param>
        /// <param name = "progress">the progress percentage of the installation</param>
        /// <param name = "updatesComplete">the number of updates that have been installed so far</param>
        /// <param name = "totalUpdates">the total number of updates to install</param>
        public InstallProgressChangedEventArgs(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            CurrentProgress = progress;
            TotalUpdates = totalUpdates;
            UpdatesComplete = updatesComplete;
            UpdateName = updateName;
        }

        /// <summary>
        ///   The progress percentage of the installation
        /// </summary>
        public int CurrentProgress { get; private set; }

        /// <summary>
        ///   The total number of updates to install
        /// </summary>
        public int TotalUpdates { get; private set; }

        /// <summary>
        ///   The number of updates that have been installed so far
        /// </summary>
        public int UpdatesComplete { get; private set; }

        /// <summary>
        ///   The name of the current update being installed
        /// </summary>
        public string UpdateName { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the ErrorOccurred event
    /// </summary>
    public sealed class ErrorOccurredEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "exception">the exception that occurred</param>
        /// <param name = "type">the type of error that occurred</param>
        public ErrorOccurredEventArgs(string exception, ErrorType type)
        {
            Exception = exception;
            Type = type;
        }

        /// <summary>
        ///   Gets the Exception information of the error that occurred
        /// </summary>
        public string Exception { get; private set; }

        /// <summary>
        ///   Gets the <see cref = "ErrorType" /> of the error that occurred
        /// </summary>
        public ErrorType Type { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the SerializationError event
    /// </summary>
    public sealed class SerializationErrorEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "e">The exception data</param>
        /// <param name = "file">The full path of the file</param>
        public SerializationErrorEventArgs(Exception e, string file)
        {
            Exception = e;
            File = file;
        }

        /// <summary>
        ///   Gets the exception data
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        ///   Gets the full path of the file
        /// </summary>
        public string File { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the SerializationError event
    /// </summary>
    public sealed class ProcessEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        public ProcessEventArgs(Process process)
        {
            Process = process;
        }

        /// <summary>
        ///   Gets the exception data
        /// </summary>
        public Process Process { get; private set; }
    }

    /// <summary>
    ///   Indicates a type of error that can occur
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        ///   An error that occurred while trying to download updates
        /// </summary>
        DownloadError,
        /// <summary>
        ///   An error that occurred while trying to install updates
        /// </summary>
        InstallationError,
        /// <summary>
        ///   A general network connection error
        /// </summary>
        FatalNetworkError,
        /// <summary>
        ///   An unspecified error, non fatal
        /// </summary>
        GeneralErrorNonFatal,
        /// <summary>
        ///   An unspecified error that prevents Seven Update from continuing
        /// </summary>
        FatalError,
        /// <summary>
        ///   An error that occurs while searching for updates
        /// </summary>
        SearchError
    }
}