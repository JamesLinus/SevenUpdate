// ***********************************************************************
// <copyright file="DownloadCompletedEventArgs.cs"
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