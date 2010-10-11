﻿// ***********************************************************************
// <copyright file="ErrorOccurredEventArgs.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
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

    /// <summary>Indicates a type of error that can occur</summary>
    public enum ErrorType
    {
        /// <summary>An error that occurred while trying to download updates</summary>
        DownloadError, 

        /// <summary>An error that occurred while trying to install updates</summary>
        InstallationError, 

        /// <summary>A general network connection error</summary>
        FatalNetworkError, 

        /// <summary>An unspecified error, non fatal</summary>
        GeneralErrorNonFatal, 

        /// <summary>An unspecified error that prevents Seven Update from continuing</summary>
        FatalError, 

        /// <summary>An error that occurs while searching for updates</summary>
        SearchError
    }

    /// <summary>Provides event data for the ErrorOccurred event</summary>
    public sealed class ErrorOccurredEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ErrorOccurredEventArgs"/> class.</summary>
        /// <param name="exception">the exception that occurred</param>
        /// <param name="type">the type of error that occurred</param>
        public ErrorOccurredEventArgs(string exception, ErrorType type)
        {
            this.Exception = exception;
            this.Type = type;
        }

        #endregion

        #region Properties

        /// <summary>Gets the Exception information of the error that occurred</summary>
        /// <value>The exception.</value>
        public string Exception { get; private set; }

        /// <summary>Gets the <see cref = "ErrorType" /> of the error that occurred</summary>
        /// <value>The type of error that occurred</value>
        public ErrorType Type { get; private set; }

        #endregion
    }
}