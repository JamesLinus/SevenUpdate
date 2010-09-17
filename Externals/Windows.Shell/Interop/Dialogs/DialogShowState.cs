//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.
namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Dialog Show State
    /// </summary>
    public enum DialogShowState
    {
        /// <summary>
        ///   Pre Show
        /// </summary>
        PreShow,

        /// <summary>
        ///   Currently Showing
        /// </summary>
        Showing,

        /// <summary>
        ///   Currently Closing
        /// </summary>
        Closing,

        /// <summary>
        ///   Closed
        /// </summary>
        Closed
    }
}