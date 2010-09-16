#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Indicates that the implementing class is a dialog that can host
    ///   customizable dialog controls (subclasses of DialogControl).
    /// </summary>
    public interface IDialogControlHost
    {
        /// <summary>
        ///   Handle notifications of pseudo-controls being added 
        ///   or removed from the collection.
        ///   PreFilter should throw if a control cannot 
        ///   be added/removed in the dialog's current state.
        ///   PostProcess should pass on changes to native control, 
        ///   if appropriate.
        /// </summary>
        /// <returns>true if collection change is allowed.</returns>
        bool IsCollectionChangeAllowed();

        /// <summary>
        ///   Applies changes to the collection.
        /// </summary>
        void ApplyCollectionChanged();

        /// <summary>
        ///   Handle notifications of individual child 
        ///   pseudo-controls' properties changing..
        ///   Prefilter should throw if the property 
        ///   cannot be set in the dialog's current state.
        ///   PostProcess should pass on changes to native control, 
        ///   if appropriate.
        /// </summary>
        /// <param name = "propertyName">The name of the property.</param>
        /// <param name = "control">The control propertyName applies to.</param>
        /// <returns>true if the property change is allowed.</returns>
        bool IsControlPropertyChangeAllowed(string propertyName, DialogControl control);

        /// <summary>
        ///   Called when a control currently in the collection 
        ///   has a property changed.
        /// </summary>
        /// <param name = "propertyName">The name of the property changed.</param>
        /// <param name = "control">The control whose property has changed.</param>
        void ApplyControlPropertyChange(string propertyName, DialogControl control);
    }
}