//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Specifies a property, event and method that indexed controls need
    ///   to implement.
    /// </summary>
    /// <remarks>
    ///   not sure where else to put this, so leaving here for now.
    /// </remarks>
    internal interface ICommonFileDialogIndexedControls
    {
        int SelectedIndex { get; set; }

        event EventHandler SelectedIndexChanged;

        void RaiseSelectedIndexChangedEvent();
    }
}