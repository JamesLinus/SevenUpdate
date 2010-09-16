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
using System.Windows.Controls.Primitives;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Provides VisualStateManager behavior for ToggleButton controls.
    /// </summary>
    public class ToggleButtonBehavior : ButtonBaseBehavior
    {
        /// <summary>
        ///   This behavior targets ToggleButton derived Controls.
        /// </summary>
        protected internal override Type TargetType { get { return typeof (ToggleButton); } }

        /// <summary>
        ///   Attaches to property changes and events.
        /// </summary>
        /// <param name = "control">An instance of the control.</param>
        protected override void OnAttach(Control control)
        {
            base.OnAttach(control);

            var toggle = (ToggleButton) control;
            var targetType = typeof (ToggleButton);

            AddValueChanged(ToggleButton.IsCheckedProperty, targetType, toggle, UpdateStateHandler);
        }

        /// <summary>
        ///   Detaches property changes and events.
        /// </summary>
        /// <param name = "control">The control</param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            var toggle = (ToggleButton) control;
            var targetType = typeof (ToggleButton);

            RemoveValueChanged(ToggleButton.IsCheckedProperty, targetType, toggle, UpdateStateHandler);
        }

        /// <summary>
        ///   Called to update the control's visual state.
        /// </summary>
        /// <param name = "control">The instance of the control being updated.</param>
        /// <param name = "useTransitions">Whether to use transitions or not.</param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            var toggle = (ToggleButton) control;

            if (!toggle.IsChecked.HasValue)
                VisualStateManager.GoToState(toggle, "Indeterminate", useTransitions);
            else if (toggle.IsChecked.Value)
                VisualStateManager.GoToState(toggle, "Checked", useTransitions);
            else
                VisualStateManager.GoToState(toggle, "Unchecked", useTransitions);

            base.UpdateState(control, useTransitions);
        }
    }
}