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
    using Microsoft.Windows.Dialogs.TaskDialogs;

    /// <summary>
    /// The default dialog data
    /// </summary>
    internal static class DialogsDefaults
    {
        #region Constants and Fields

        /// <summary>
        /// The default caption
        /// </summary>
        internal const string Caption = "Application";

        /// <summary>
        /// The default content
        /// </summary>
        internal const string Content = "";

        /// <summary>
        /// The default ideal width
        /// </summary>
        internal const int IdealWidth = 0;

        /// <summary>
        /// The default main instruction
        /// </summary>
        internal const string MainInstruction = "";

        /// <summary>
        /// For generating control ID numbers that won't collide with the standard button return IDs.
        /// </summary>
        internal const int MinimumDialogControlId = (int)TaskDialogNativeMethods.TaskDialogCommonButtonReturnID.Close + 1;

        /// <summary>
        /// The default progress bar maximum value
        /// </summary>
        internal const int ProgressBarMaximumValue = 100;

        /// <summary>
        /// The default progress bar minimum value
        /// </summary>
        internal const int ProgressBarMinimumValue = 0;

        /// <summary>
        /// The default progress bar starting value
        /// </summary>
        internal const int ProgressBarStartingValue = 0;

        #endregion
    }
}