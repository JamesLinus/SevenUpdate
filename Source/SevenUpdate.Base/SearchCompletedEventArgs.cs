// ***********************************************************************
// <copyright file="SearchCompletedEventArgs.cs"
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
    using System.Collections.Generic;

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
}