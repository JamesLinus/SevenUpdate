// <copyright file="TaskDialogCommonButtons.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;

    /// <summary>The common button types for the dialog.</summary>
    [Flags]
    internal enum TaskDialogCommonButtons
    {
        /// <summary>An "OK" button.</summary>
        Ok = 0x0001, 

        /// <summary>A "Yes" button.</summary>
        Yes = 0x0002, 

        /// <summary>A "No" button.</summary>
        No = 0x0004, 

        /// <summary>A "Cancel" button.</summary>
        Cancel = 0x0008, 

        /// <summary>A "Retry" button.</summary>
        Retry = 0x0010, 

        /// <summary>A "Close" button.</summary>
        Close = 0x0020
    }
}