// ***********************************************************************
// Assembly         : SevenUpdate.Base
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Indicates a type of error that can occur
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

    /// <summary>
    /// Provides event data for the SearchCompleted event
    /// </summary>
    public sealed class SearchCompletedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="applications">
        /// The collection of applications to update
        /// </param>
        /// <param name="importantCount">
        /// The number of important updates
        /// </param>
        /// <param name="recommendedCount">
        /// The number of recommended updates
        /// </param>
        /// <param name="optionalCount">
        /// The number of optional updates
        /// </param>
        public SearchCompletedEventArgs(IEnumerable<Sui> applications, int importantCount, int recommendedCount, int optionalCount)
        {
            this.Applications = applications;
            this.ImportantCount = importantCount;
            this.OptionalCount = optionalCount;
            this.RecommendedCount = recommendedCount;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a collection of applications that contain updates to install
        /// </summary>
        /// <value>The applications.</value>
        public IEnumerable<Sui> Applications { get; private set; }

        /// <summary>
        ///   Gets or sets the important updates count.
        /// </summary>
        /// <value>The important count.</value>
        public int ImportantCount { get; set; }

        /// <summary>
        ///   Gets or sets the optional updates count.
        /// </summary>
        /// <value>The optional count.</value>
        public int OptionalCount { get; set; }

        /// <summary>
        ///   Gets the recommended updates count.
        /// </summary>
        /// <value>The recommended count.</value>
        public int RecommendedCount { get; private set; }

        #endregion
    }

    /// <summary>
    /// Provides event data for the DownloadCompleted event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
    public sealed class DownloadCompletedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="errorOccurred">
        /// <c>true</c> is an error occurred, otherwise <c>false</c>
        /// </param>
        public DownloadCompletedEventArgs(bool errorOccurred)
        {
            this.ErrorOccurred = errorOccurred;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether an error occurred
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if an error occurred otherwise, <see langword = "false" />.
        /// </value>
        public bool ErrorOccurred { get; private set; }

        #endregion
    }

    /// <summary>
    /// Provides event data for the DownloadProgressChanged event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
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

    /// <summary>
    /// Provides event data for the InstallCompleted event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
    public sealed class InstallCompletedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="updatesInstalled">
        /// the number of updates installed
        /// </param>
        /// <param name="updatesFailed">
        /// the number of updates that failed
        /// </param>
        public InstallCompletedEventArgs(int updatesInstalled, int updatesFailed)
        {
            this.UpdatesInstalled = updatesInstalled;
            this.UpdatesFailed = updatesFailed;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the number of updates that failed.
        /// </summary>
        /// <value>The updates failed.</value>
        public int UpdatesFailed { get; private set; }

        /// <summary>
        ///   Gets the number of updates that have been installed
        /// </summary>
        /// <value>The updates installed.</value>
        public int UpdatesInstalled { get; private set; }

        #endregion
    }

    /// <summary>
    /// Provides event data for the InstallProgressChanged event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
    public sealed class InstallProgressChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="updateName">
        /// the name of the update currently being installed
        /// </param>
        /// <param name="progress">
        /// the progress percentage of the installation
        /// </param>
        /// <param name="updatesComplete">
        /// the number of updates that have been installed so far
        /// </param>
        /// <param name="totalUpdates">
        /// the total number of updates to install
        /// </param>
        public InstallProgressChangedEventArgs(string updateName, int progress, int updatesComplete, int totalUpdates)
        {
            this.CurrentProgress = progress;
            this.TotalUpdates = totalUpdates;
            this.UpdatesComplete = updatesComplete;
            this.UpdateName = updateName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the progress percentage of the installation
        /// </summary>
        /// <value>The current progress.</value>
        public int CurrentProgress { get; private set; }

        /// <summary>
        ///   Gets the total number of updates to install
        /// </summary>
        /// <value>The total updates.</value>
        public int TotalUpdates { get; private set; }

        /// <summary>
        ///   Gets the name of the current update being installed
        /// </summary>
        /// <value>The name of the update.</value>
        public string UpdateName { get; private set; }

        /// <summary>
        ///   Gets the number of updates that have been installed so far
        /// </summary>
        /// <value>The updates complete.</value>
        public int UpdatesComplete { get; private set; }

        #endregion
    }

    /// <summary>
    /// Provides event data for the ErrorOccurred event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
    public sealed class ErrorOccurredEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorOccurredEventArgs"/> class.
        /// </summary>
        /// <param name="exception">
        /// the exception that occurred
        /// </param>
        /// <param name="type">
        /// the type of error that occurred
        /// </param>
        public ErrorOccurredEventArgs(string exception, ErrorType type)
        {
            this.Exception = exception;
            this.Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the Exception information of the error that occurred
        /// </summary>
        /// <value>The exception.</value>
        public string Exception { get; private set; }

        /// <summary>
        ///   Gets the <see cref = "ErrorType" /> of the error that occurred
        /// </summary>
        /// <value>The type of error that occurred</value>
        public ErrorType Type { get; private set; }

        #endregion
    }

    /// <summary>
    /// Provides event data for the SerializationError event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
    public sealed class SerializationErrorEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationErrorEventArgs"/> class.
        /// </summary>
        /// <param name="e">
        /// The exception data
        /// </param>
        /// <param name="file">
        /// The full path of the file
        /// </param>
        public SerializationErrorEventArgs(Exception e, string file)
        {
            this.Exception = e;
            this.File = file;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the exception data
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; private set; }

        /// <summary>
        ///   Gets the full path of the file
        /// </summary>
        /// <value>The file that the serialization error occurred for</value>
        public string File { get; private set; }

        #endregion
    }

    /// <summary>
    /// Provides event data for the SerializationError event
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "EventArgs")]
    public sealed class ProcessEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        /// <param name="process">
        /// The process.
        /// </param>
        public ProcessEventArgs(Process process)
        {
            this.Process = process;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the exception data
        /// </summary>
        /// <value>The process.</value>
        public Process Process { get; private set; }

        #endregion
    }
}