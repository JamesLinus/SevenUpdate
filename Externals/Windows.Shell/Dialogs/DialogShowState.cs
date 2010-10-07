// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Dialogs
{
    /// <summary>
    /// Dialog Show State
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