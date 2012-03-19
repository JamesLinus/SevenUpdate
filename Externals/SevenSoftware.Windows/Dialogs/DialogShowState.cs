// <copyright file="DialogShowState.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs
{
    /// <summary>Dialog Show State</summary>
    public enum DialogShowState
    {
        /// <summary>The dialog is about to be shown</summary>
        PreShow, 

        /// <summary>The dialog is shown</summary>
        Showing, 

        /// <summary>The dialog is currently closing</summary>
        Closing, 

        /// <summary>The dialog is closed</summary>
        Closed
    }
}