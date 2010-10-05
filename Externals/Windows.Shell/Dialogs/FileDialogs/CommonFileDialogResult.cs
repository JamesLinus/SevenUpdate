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
    /// Specifies identifiers to indicate the return value of a CommonFileDialog dialog.
    /// </summary>
    public enum CommonFileDialogResult
    {
        /// <summary>
        ///   The dialog box return value is OK (usually sent from a button labeled OK or Save).
        /// </summary>
        OK = 1, 

        /// <summary>
        ///   The dialog box return value is Cancel (usually sent from a button labeled Cancel).
        /// </summary>
        Cancel = 2, 
    }
}