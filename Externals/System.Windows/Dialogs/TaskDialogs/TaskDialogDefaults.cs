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
    /// Task Dialog defaults
    /// </summary>
    internal static class TaskDialogDefaults
    {
        #region Constants and Fields

        /// <summary>
        ///   The caption
        /// </summary>
        internal const string Caption = "Application";

        /// <summary>
        ///   The content
        /// </summary>
        internal const string Content = "";

        /// <summary>
        ///   The ideal width
        /// </summary>
        internal const int IdealWidth = 0;

        /// <summary>
        ///   The main instruction
        /// </summary>
        internal const string MainInstruction = "";

        /// <summary>
        ///   For generating control ID numbers that won't 
        ///   collide with the standard button return IDs.
        /// </summary>
        internal const int MinimumDialogControlId = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.Close + 1;

        /// <summary>
        ///   The progress bar max value
        /// </summary>
        internal const int ProgressBarMaximumValue = 100;

        /// <summary>
        ///   The progress bar min value
        /// </summary>
        internal const int ProgressBarMinimumValue = 0;

        #endregion
    }
}