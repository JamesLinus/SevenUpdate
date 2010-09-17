//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

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