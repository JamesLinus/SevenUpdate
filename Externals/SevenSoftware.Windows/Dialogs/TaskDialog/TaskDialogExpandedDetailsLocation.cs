// <copyright file="TaskDialogExpandedDetailsLocation.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Specifies the options for expand/collapse sections in dialogs.</summary>
    public enum TaskDialogExpandedDetailsLocation
    {
        /// <summary>Do not show the content.</summary>
        Hide, 

        /// <summary>Show the content.</summary>
        ExpandContent, 

        /// <summary>Expand the footer content.</summary>
        ExpandFooter
    }
}