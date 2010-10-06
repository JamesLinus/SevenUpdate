// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Dialogs
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