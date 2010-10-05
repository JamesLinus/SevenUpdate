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
    /// Indicates that the implementing class is a dialog that can host
    ///   customizable dialog controls (subclasses of DialogControl).
    /// </summary>
    public interface IDialogControlHost
    {
        #region Public Methods

        /// <summary>
        /// Applies changes to the collection.
        /// </summary>
        void ApplyCollectionChanged();

        /// <summary>
        /// Called when a control currently in the collection 
        ///   has a property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property changed.
        /// </param>
        /// <param name="control">
        /// The control whose property has changed.
        /// </param>
        void ApplyControlPropertyChange(string propertyName, DialogControl control);

        /// <summary>
        /// Handle notifications of pseudo-controls being added 
        ///   or removed from the collection.
        ///   PreFilter should throw if a control cannot 
        ///   be added/removed in the dialog's current state.
        ///   PostProcess should pass on changes to native control, 
        ///   if appropriate.
        /// </summary>
        /// <returns>
        /// true if collection change is allowed.
        /// </returns>
        bool IsCollectionChangeAllowed();

        /// <summary>
        /// Handle notifications of individual child 
        ///   pseudo-controls' properties changing..
        ///   Prefilter should throw if the property 
        ///   cannot be set in the dialog's current state.
        ///   PostProcess should pass on changes to native control, 
        ///   if appropriate.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <param name="control">
        /// The control propertyName applies to.
        /// </param>
        /// <returns>
        /// true if the property change is allowed.
        /// </returns>
        bool IsControlPropertyChangeAllowed(string propertyName, DialogControl control);

        #endregion
    }
}