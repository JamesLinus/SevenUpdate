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

#region

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Provides VisualStateManager base behavior for controls.
    /// </summary>
    /// <remarks>
    ///   Provides focus states.
    ///   Forwards the Loaded event to UpdateState.
    /// </remarks>
    public class ControlBehavior : VisualStateBehavior
    {
        /// <summary>
        ///   This behavior targets Control derived Controls.
        /// </summary>
        protected internal override Type TargetType { get { return typeof (Control); } }

        /// <summary>
        ///   Attaches to property changes and events.
        /// </summary>
        /// <param name = "control">An instance of the control.</param>
        protected override void OnAttach(Control control)
        {
            control.Loaded += delegate { UpdateState(control, false); };
            AddValueChanged(UIElement.IsKeyboardFocusWithinProperty, typeof (Control), control, UpdateStateHandler);
        }

        /// <summary>
        ///   Detaches property changes and events.
        /// </summary>
        /// <param name = "control">The control</param>
        protected override void OnDetach(Control control)
        {
            RemoveValueChanged(UIElement.IsKeyboardFocusWithinProperty, typeof (Control), control, UpdateStateHandler);
        }

        protected override void UpdateStateHandler(Object o, EventArgs e)
        {
            var cont = o as Control;
            if (cont == null)
                throw new InvalidOperationException("This should never be used on anything other than a control.");
            UpdateState(cont, true);
        }

        /// <summary>
        ///   Called to update the control's visual state.
        /// </summary>
        /// <param name = "control">The instance of the control being updated.</param>
        /// <param name = "useTransitions">Whether to use transitions or not.</param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            VisualStateManager.GoToState(control, control.IsKeyboardFocusWithin ? "Focused" : "Unfocused", useTransitions);
        }
    }
}