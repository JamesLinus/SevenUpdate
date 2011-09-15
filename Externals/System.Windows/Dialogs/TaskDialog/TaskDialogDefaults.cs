// ***********************************************************************
// <copyright file="TaskDialogDefaults.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    using System.Windows.Properties;

    /// <summary>The task dialog defaults</summary>
    internal static class TaskDialogDefaults
    {
        #region Constants and Fields

        /// <summary>The ideal width for the dialog.</summary>
        public const int IdealWidth = 0;

        /// <summary>For generating control ID numbers that won't collide with the standard button return IDs.</summary>
        public const int MinimumDialogControlId = (int)TaskDialogCommonButtonReturnIds.Close + 1;

        /// <summary>The progress bar max value.</summary>
        public const int ProgressBarMaximumValue = 100;

        /// <summary>The progress bar min value.</summary>
        public const int ProgressBarMinimumValue = 0;

        #endregion

        #region Public Properties

        /// <summary>Gets the default dialog caption.</summary>
        public static string Caption
        {
            get
            {
                return Resources.TaskDialogDefaultCaption;
            }
        }

        /// <summary>Gets the default dialog content.</summary>
        public static string Content
        {
            get
            {
                return null;
            }
        }

        /// <summary>Gets the default dialog main instruction.</summary>
        public static string MainInstruction
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}
