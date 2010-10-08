// ***********************************************************************
// <copyright file="SerializationErrorEventArgs.cs"
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
    /// Provides event data for the SerializationError event
    /// </summary>
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
}