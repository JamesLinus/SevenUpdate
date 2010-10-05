//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides VisualStateManager behavior for ListBoxItem controls.
    /// </summary>
    public class ListBoxItemBehavior : ControlBehavior
    {
        #region Properties

        /// <summary>
        ///   This behavior targets ListBoxItem derived Controls.
        /// </summary>
        protected internal override Type TargetType
        {
            get
            {
                return typeof(ListBoxItem);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches to property changes and events.
        /// </summary>
        /// <param name="control">
        /// An instance of the control.
        /// </param>
        protected override void OnAttach(Control control)
        {
            base.OnAttach(control);

            var listBoxItem = (ListBoxItem)control;
            var targetType = typeof(ListBoxItem);

            AddValueChanged(UIElement.IsMouseOverProperty, targetType, listBoxItem, this.UpdateStateHandler);
            AddValueChanged(ListBoxItem.IsSelectedProperty, targetType, listBoxItem, this.UpdateStateHandler);
        }

        /// <summary>
        /// Detaches to property changes and events.
        /// </summary>
        /// <param name="control">
        /// An instance of the control.
        /// </param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            var listBoxItem = (ListBoxItem)control;
            var targetType = typeof(ListBoxItem);

            RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, listBoxItem, this.UpdateStateHandler);
            RemoveValueChanged(ListBoxItem.IsSelectedProperty, targetType, listBoxItem, this.UpdateStateHandler);
        }

        /// <summary>
        /// Called to update the control's visual state.
        /// </summary>
        /// <param name="control">
        /// The instance of the control being updated.
        /// </param>
        /// <param name="useTransitions">
        /// Whether to use transitions or not.
        /// </param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            var listBoxItem = (ListBoxItem)control;

            VisualStateManager.GoToState(listBoxItem, listBoxItem.IsMouseOver ? "MouseOver" : "Normal", useTransitions);

            VisualStateManager.GoToState(listBoxItem, listBoxItem.IsSelected ? "Selected" : "Unselected", useTransitions);

            base.UpdateState(control, useTransitions);
        }

        #endregion
    }
}