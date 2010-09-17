//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.
namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Specifies the options for expand/collapse sections in dialogs.
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