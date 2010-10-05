//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    /// Specifies the options for expand/collapse sections in dialogs.
    /// </summary>
    public enum TaskDialogExpandedDetailsLocation
    {
        /// <summary>
        ///   Do not show the content.
        /// </summary>
        Hide, 

        /// <summary>
        ///   Show the content.
        /// </summary>
        ExpandContent, 

        /// <summary>
        ///   Expand the footer content.
        /// </summary>
        ExpandFooter
    }
}