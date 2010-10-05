//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs.Controls
{
    using System;

    /// <summary>
    /// Specifies a property, event and method that indexed controls need
    ///   to implement.
    /// </summary>
    /// <remarks>
    /// not sure where else to put this, so leaving here for now.
    /// </remarks>
    internal interface ICommonFileDialogIndexedControls
    {
        #region Events

        /// <summary>
        /// </summary>
        event EventHandler SelectedIndexChanged;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        int SelectedIndex { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        void RaiseSelectedIndexChangedEvent();

        #endregion
    }
}