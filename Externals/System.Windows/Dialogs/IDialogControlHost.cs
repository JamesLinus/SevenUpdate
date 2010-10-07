// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Dialogs
{
    /// <summary>
    /// Indicates that the implementing class is a dialog that can host
    ///   customizable dialog controls (subclasses of <see cref="DialogControl"/>).
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
        /// <parameter name="propertyName">
        /// The name of the property changed.
        /// </parameter>
        /// <parameter name="control">
        /// The control whose property has changed.
        /// </parameter>
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
        /// <see langword="true"/> if collection change is allowed.
        /// </returns>
        bool IsCollectionChangeAllowed();

        /// <summary>
        /// Handle notifications of individual child 
        ///   pseudo-controls' properties changing..
        ///   Pre filter should throw if the property 
        ///   cannot be set in the dialog's current state.
        ///   PostProcess should pass on changes to native control, 
        ///   if appropriate.
        /// </summary>
        /// <parameter name="propertyName">
        /// The name of the property.
        /// </parameter>
        /// <parameter name="control">
        /// The control <parameterref name="propertyName"/> applies to.
        /// </parameter>
        /// <returns>
        /// <see langword="true"/> if the property change is allowed.
        /// </returns>
        bool IsControlPropertyChangeAllowed(string propertyName, DialogControl control);

        #endregion
    }
}