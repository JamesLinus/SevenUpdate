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
    /// </summary>
    internal static class TaskDialogDefaults
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal const string Caption = "Application";

        /// <summary>
        /// </summary>
        internal const string Content = "";

        /// <summary>
        /// </summary>
        internal const int IdealWidth = 0;

        /// <summary>
        /// </summary>
        internal const string MainInstruction = "";

        // For generating control ID numbers that won't 
        // collide with the standard button return IDs.
        /// <summary>
        /// </summary>
        internal const int MinimumDialogControlId = (int)TaskDialogNativeMethods.TaskdialogCommonButtonReturnID.IDCLOSE + 1;

        /// <summary>
        /// </summary>
        internal const int ProgressBarMaximumValue = 100;

        /// <summary>
        /// </summary>
        internal const int ProgressBarMinimumValue = 0;

        #endregion
    }
}