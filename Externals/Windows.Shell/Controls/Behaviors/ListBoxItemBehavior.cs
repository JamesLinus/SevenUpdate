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
    ///   Provides VisualStateManager behavior for ListBoxItem controls.
    /// </summary>
    public class ListBoxItemBehavior : ControlBehavior
    {
        /// <summary>
        ///   This behavior targets ListBoxItem derived Controls.
        /// </summary>
        protected internal override Type TargetType { get { return typeof (ListBoxItem); } }

        /// <summary>
        ///   Attaches to property changes and events.
        /// </summary>
        /// <param name = "control">An instance of the control.</param>
        protected override void OnAttach(Control control)
        {
            base.OnAttach(control);

            var listBoxItem = (ListBoxItem) control;
            var targetType = typeof (ListBoxItem);

            AddValueChanged(UIElement.IsMouseOverProperty, targetType, listBoxItem, UpdateStateHandler);
            AddValueChanged(ListBoxItem.IsSelectedProperty, targetType, listBoxItem, UpdateStateHandler);
        }

        /// <summary>
        ///   Detaches to property changes and events.
        /// </summary>
        /// <param name = "control">An instance of the control.</param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            var listBoxItem = (ListBoxItem) control;
            var targetType = typeof (ListBoxItem);

            RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, listBoxItem, UpdateStateHandler);
            RemoveValueChanged(ListBoxItem.IsSelectedProperty, targetType, listBoxItem, UpdateStateHandler);
        }

        /// <summary>
        ///   Called to update the control's visual state.
        /// </summary>
        /// <param name = "control">The instance of the control being updated.</param>
        /// <param name = "useTransitions">Whether to use transitions or not.</param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            var listBoxItem = (ListBoxItem) control;

            VisualStateManager.GoToState(listBoxItem, listBoxItem.IsMouseOver ? "MouseOver" : "Normal", useTransitions);

            VisualStateManager.GoToState(listBoxItem, listBoxItem.IsSelected ? "Selected" : "Unselected", useTransitions);

            base.UpdateState(control, useTransitions);
        }
    }
}