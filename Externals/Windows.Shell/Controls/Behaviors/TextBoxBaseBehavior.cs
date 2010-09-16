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
    ///   Provides VisualStateManager behavior for TextBox controls.
    /// </summary>
    public class TextBoxBaseBehavior : ControlBehavior
    {
        /// <summary>
        ///   This behavior targets TextBoxBase derived Controls.
        /// </summary>
        protected internal override Type TargetType { get { return typeof (TextBoxBase); } }

        /// <summary>
        ///   Attaches to property changes and events.
        /// </summary>
        /// <param name = "control">An instance of the control.</param>
        protected override void OnAttach(Control control)
        {
            base.OnAttach(control);

            var textBoxBase = (TextBoxBase) control;
            var targetType = typeof (TextBoxBase);

            AddValueChanged(UIElement.IsMouseOverProperty, targetType, textBoxBase, UpdateStateHandler);
            AddValueChanged(UIElement.IsEnabledProperty, targetType, textBoxBase, UpdateStateHandler);
            AddValueChanged(TextBoxBase.IsReadOnlyProperty, targetType, textBoxBase, UpdateStateHandler);
        }

        /// <summary>
        ///   Detaches property changes and events.
        /// </summary>
        /// <param name = "control">The control</param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            var textBoxBase = (TextBoxBase) control;
            var targetType = typeof (TextBoxBase);

            RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, textBoxBase, UpdateStateHandler);
            RemoveValueChanged(UIElement.IsEnabledProperty, targetType, textBoxBase, UpdateStateHandler);
            RemoveValueChanged(TextBoxBase.IsReadOnlyProperty, targetType, textBoxBase, UpdateStateHandler);
        }

        /// <summary>
        ///   Called to update the control's visual state.
        /// </summary>
        /// <param name = "control">The instance of the control being updated.</param>
        /// <param name = "useTransitions">Whether to use transitions or not.</param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            var textBoxBase = (TextBoxBase) control;

            if (!textBoxBase.IsEnabled)
                VisualStateManager.GoToState(textBoxBase, "Disabled", useTransitions);
            else if (textBoxBase.IsReadOnly)
                VisualStateManager.GoToState(textBoxBase, "ReadOnly", useTransitions);
            else if (textBoxBase.IsMouseOver)
                VisualStateManager.GoToState(textBoxBase, "MouseOver", useTransitions);
            else
                VisualStateManager.GoToState(textBoxBase, "Normal", useTransitions);

            base.UpdateState(control, useTransitions);
        }
    }
}