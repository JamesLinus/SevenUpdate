// ***********************************************************************
// <copyright file="UpdateSelectionChangedEventArgs.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
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
    /// Provides event data for the UpdateSelection event
    /// </summary>
    internal sealed class UpdateSelectionChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="importantUpdates">
        /// The number of Important updates selected
        /// </param>
        /// <param name="optionalUpdates">
        /// The number of Optional updates selected
        /// </param>
        /// <param name="importantDownloadSize">
        /// A value indicating the download size of the Important updates
        /// </param>
        /// <param name="optionalDownloadSize">
        /// A value indicating the download size of the Optional updates
        /// </param>
        public UpdateSelectionChangedEventArgs(int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
        {
            this.ImportantUpdates = importantUpdates;

            this.OptionalUpdates = optionalUpdates;

            this.ImportantDownloadSize = importantDownloadSize;

            this.OptionalDownloadSize = optionalDownloadSize;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the total download size in bytes of the important updates
        /// </summary>
        internal ulong ImportantDownloadSize { get; private set; }

        /// <summary>
        ///   Gets the number of Important Updates selected
        /// </summary>
        internal int ImportantUpdates { get; private set; }

        /// <summary>
        ///   Gets the total download size in bytes of the optional updates
        /// </summary>
        internal ulong OptionalDownloadSize { get; private set; }

        /// <summary>
        ///   Gets the number of Optional Updates selected
        /// </summary>
        internal int OptionalUpdates { get; private set; }

        #endregion
    }
}