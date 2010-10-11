// ***********************************************************************
// <copyright file="SearchCompletedEventArgs.cs"
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
    using System.Collections.Generic;

    /// <summary>Provides event data for the SearchCompleted event</summary>
    public sealed class SearchCompletedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SearchCompletedEventArgs"/> class.</summary>
        /// <param name="applications">The collection of applications to update</param>
        /// <param name="importantCount">The number of important updates</param>
        /// <param name="recommendedCount">The number of recommended updates</param>
        /// <param name="optionalCount">The number of optional updates</param>
        public SearchCompletedEventArgs(IEnumerable<Sui> applications, int importantCount, int recommendedCount, int optionalCount)
        {
            this.Applications = applications;
            this.ImportantCount = importantCount;
            this.OptionalCount = optionalCount;
            this.RecommendedCount = recommendedCount;
        }

        #endregion

        #region Properties

        /// <summary>Gets a collection of applications that contain updates to install</summary>
        /// <value>The applications.</value>
        public IEnumerable<Sui> Applications { get; private set; }

        /// <summary>Gets or sets the important updates count.</summary>
        /// <value>The important count.</value>
        public int ImportantCount { get; set; }

        /// <summary>Gets or sets the optional updates count.</summary>
        /// <value>The optional count.</value>
        public int OptionalCount { get; set; }

        /// <summary>Gets the recommended updates count.</summary>
        /// <value>The recommended count.</value>
        public int RecommendedCount { get; private set; }

        #endregion
    }
}