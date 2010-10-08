// ***********************************************************************
// <copyright file="DownloadCompletedEventArgs.cs"
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
    /// Provides event data for the DownloadCompleted event
    /// </summary>
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
}