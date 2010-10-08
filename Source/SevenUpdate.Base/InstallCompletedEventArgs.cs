// ***********************************************************************
// <copyright file="InstallCompletedEventArgs.cs"
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
    /// Provides event data for the InstallCompleted event
    /// </summary>
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
}