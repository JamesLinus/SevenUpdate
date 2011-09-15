// ***********************************************************************
// <copyright file="IDialogControlHost.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>
    ///   Indicates that the implementing class is a dialog that can host customizable dialog controls (subclasses of
    ///   DialogControl).
    /// </summary>
    public interface IDialogControlHost
    {
        #region Public Methods

        /// <summary>Applies changes to the collection.</summary>
        void ApplyCollectionChanged();

        /// <summary>Called when a control currently in the collection has a property changed.</summary>
        /// <param name="propertyName">The name of the property changed.</param>
        /// <param name="control">The control whose property has changed.</param>
        void ApplyControlPropertyChange(string propertyName, DialogControl control);

        /// <summary>Returns if changes to the collection are allowed.</summary>
        /// <returns>true if collection change is allowed.</returns>
        bool IsCollectionChangeAllowed();

        /// <summary>
        ///   Handle notifications of individual child pseudo-controls' properties changing.. Prefilter should throw if
        ///   the property cannot be set in the dialog's current state. PostProcess should pass on changes to native
        ///   control, if appropriate.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="control">The control propertyName applies to.</param>
        /// <returns>true if the property change is allowed.</returns>
        bool IsControlPropertyChangeAllowed(string propertyName, DialogControl control);

        #endregion
    }
}
