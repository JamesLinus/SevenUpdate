// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
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