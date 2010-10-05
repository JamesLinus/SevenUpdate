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
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Provides VisualStateManager behavior for ButtonBase controls.
    /// </summary>
    public class ButtonBaseBehavior : ControlBehavior
    {
        #region Properties

        /// <summary>
        ///   This behavior targets ButtonBase derived Controls.
        /// </summary>
        protected internal override Type TargetType
        {
            get
            {
                return typeof(ButtonBase);
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

            var button = (ButtonBase)control;
            var targetType = typeof(ButtonBase);

            AddValueChanged(UIElement.IsMouseOverProperty, targetType, button, this.UpdateStateHandler);
            AddValueChanged(UIElement.IsEnabledProperty, targetType, button, this.UpdateStateHandler);
            AddValueChanged(ButtonBase.IsPressedProperty, targetType, button, this.UpdateStateHandler);
        }

        /// <summary>
        /// Detaches property changes and events.
        /// </summary>
        /// <param name="control">
        /// The control
        /// </param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            var button = (ButtonBase)control;
            var targetType = typeof(ButtonBase);

            RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, button, this.UpdateStateHandler);
            RemoveValueChanged(UIElement.IsEnabledProperty, targetType, button, this.UpdateStateHandler);
            RemoveValueChanged(ButtonBase.IsPressedProperty, targetType, button, this.UpdateStateHandler);
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
            var button = (ButtonBase)control;

            if (!button.IsEnabled)
            {
                VisualStateManager.GoToState(button, "Disabled", useTransitions);
            }
            else if (button.IsPressed)
            {
                VisualStateManager.GoToState(button, "Pressed", useTransitions);
            }
            else if (button.IsMouseOver)
            {
                VisualStateManager.GoToState(button, "MouseOver", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(button, "Normal", useTransitions);
            }

            base.UpdateState(control, useTransitions);
        }

        #endregion
    }
}