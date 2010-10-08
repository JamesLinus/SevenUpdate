// ***********************************************************************
// <copyright file="ErrorOccurredEventArgs.cs"
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
    /// Provides event data for the ErrorOccurred event
    /// </summary>
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
}