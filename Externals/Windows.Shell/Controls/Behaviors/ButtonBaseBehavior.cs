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
    ///   Provides VisualStateManager behavior for ButtonBase controls.
    /// </summary>
    public class ButtonBaseBehavior : ControlBehavior
    {
        /// <summary>
        ///   This behavior targets ButtonBase derived Controls.
        /// </summary>
        protected internal override Type TargetType { get { return typeof (ButtonBase); } }

        /// <summary>
        ///   Attaches to property changes and events.
        /// </summary>
        /// <param name = "control">An instance of the control.</param>
        protected override void OnAttach(Control control)
        {
            base.OnAttach(control);

            var button = (ButtonBase) control;
            var targetType = typeof (ButtonBase);

            AddValueChanged(UIElement.IsMouseOverProperty, targetType, button, UpdateStateHandler);
            AddValueChanged(UIElement.IsEnabledProperty, targetType, button, UpdateStateHandler);
            AddValueChanged(ButtonBase.IsPressedProperty, targetType, button, UpdateStateHandler);
        }

        /// <summary>
        ///   Detaches property changes and events.
        /// </summary>
        /// <param name = "control">The control</param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            var button = (ButtonBase) control;
            var targetType = typeof (ButtonBase);

            RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, button, UpdateStateHandler);
            RemoveValueChanged(UIElement.IsEnabledProperty, targetType, button, UpdateStateHandler);
            RemoveValueChanged(ButtonBase.IsPressedProperty, targetType, button, UpdateStateHandler);
        }

        /// <summary>
        ///   Called to update the control's visual state.
        /// </summary>
        /// <param name = "control">The instance of the control being updated.</param>
        /// <param name = "useTransitions">Whether to use transitions or not.</param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            var button = (ButtonBase) control;

            if (!button.IsEnabled)
                VisualStateManager.GoToState(button, "Disabled", useTransitions);
            else if (button.IsPressed)
                VisualStateManager.GoToState(button, "Pressed", useTransitions);
            else if (button.IsMouseOver)
                VisualStateManager.GoToState(button, "MouseOver", useTransitions);
            else
                VisualStateManager.GoToState(button, "Normal", useTransitions);

            base.UpdateState(control, useTransitions);
        }
    }
}