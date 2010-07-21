// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

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
            Type targetType = typeof (ListBoxItem);

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
            Type targetType = typeof (ListBoxItem);

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

            if (listBoxItem.IsMouseOver)
                VisualStateManager.GoToState(listBoxItem, "MouseOver", useTransitions);
            else
                VisualStateManager.GoToState(listBoxItem, "Normal", useTransitions);

            if (listBoxItem.IsSelected)
                VisualStateManager.GoToState(listBoxItem, "Selected", useTransitions);
            else
                VisualStateManager.GoToState(listBoxItem, "Unselected", useTransitions);

            base.UpdateState(control, useTransitions);
        }
    }
}