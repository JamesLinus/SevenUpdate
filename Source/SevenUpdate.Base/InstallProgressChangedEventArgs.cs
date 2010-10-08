// ***********************************************************************
// <copyright file="InstallProgressChangedEventArgs.cs"
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
    /// Provides event data for the InstallProgressChanged event
    /// </summary>
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
}