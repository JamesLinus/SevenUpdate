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
    using System;

    /// <summary>
    /// Specifies the icon displayed in a task dialog.
    /// </summary>
    public enum TaskDialogStandardIcon
    {
        /// <summary>
        ///   Displays no icons (default).
        /// </summary>
        None = 0, 

        /// <summary>
        ///   Displays the warning icon.
        /// </summary>
        Warning = 65535, 

        /// <summary>
        ///   Displays the error icon.
        /// </summary>
        Error = 65534, 

        /// <summary>
        ///   Displays the Information icon.
        /// </summary>
        Information = 65533, 

        /// <summary>
        ///   Displays the User Account Control shield.
        /// </summary>
        Shield = UInt16.MaxValue - 3, 

        /// <summary>
        ///   Displays the User Account Control shield.
        /// </summary>
        ShieldBlue = UInt16.MaxValue - 4, 

        /// <summary>
        ///   Displays the User Account Control shield with gray background.
        /// </summary>
        ShieldGray = UInt16.MaxValue - 8, 

        /// <summary>
        ///   Displays a warning shield with yellow background.
        /// </summary>
        SecurityWarning = UInt16.MaxValue - 5, 

        /// <summary>
        ///   Displays an erro shield with red background.
        /// </summary>
        SecurityError = UInt16.MaxValue - 6, 

        /// <summary>
        ///   Displays a success shield with green background.
        /// </summary>
        ShieldGreen = UInt16.MaxValue - 7, 
    }
}