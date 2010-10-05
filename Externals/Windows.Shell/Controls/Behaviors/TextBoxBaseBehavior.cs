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
    /// Provides VisualStateManager behavior for TextBox controls.
    /// </summary>
    public class TextBoxBaseBehavior : ControlBehavior
    {
        #region Properties

        /// <summary>
        ///   This behavior targets TextBoxBase derived Controls.
        /// </summary>
        protected internal override Type TargetType
        {
            get
            {
                return typeof(TextBoxBase);
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

            var textBoxBase = (TextBoxBase)control;
            var targetType = typeof(TextBoxBase);

            AddValueChanged(UIElement.IsMouseOverProperty, targetType, textBoxBase, this.UpdateStateHandler);
            AddValueChanged(UIElement.IsEnabledProperty, targetType, textBoxBase, this.UpdateStateHandler);
            AddValueChanged(TextBoxBase.IsReadOnlyProperty, targetType, textBoxBase, this.UpdateStateHandler);
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

            var textBoxBase = (TextBoxBase)control;
            var targetType = typeof(TextBoxBase);

            RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, textBoxBase, this.UpdateStateHandler);
            RemoveValueChanged(UIElement.IsEnabledProperty, targetType, textBoxBase, this.UpdateStateHandler);
            RemoveValueChanged(TextBoxBase.IsReadOnlyProperty, targetType, textBoxBase, this.UpdateStateHandler);
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
            var textBoxBase = (TextBoxBase)control;

            if (!textBoxBase.IsEnabled)
            {
                VisualStateManager.GoToState(textBoxBase, "Disabled", useTransitions);
            }
            else if (textBoxBase.IsReadOnly)
            {
                VisualStateManager.GoToState(textBoxBase, "ReadOnly", useTransitions);
            }
            else if (textBoxBase.IsMouseOver)
            {
                VisualStateManager.GoToState(textBoxBase, "MouseOver", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(textBoxBase, "Normal", useTransitions);
            }

            base.UpdateState(control, useTransitions);
        }

        #endregion
    }
}