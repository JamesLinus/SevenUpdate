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
    /// Provides VisualStateManager base behavior for controls.
    /// </summary>
    /// <remarks>
    /// Provides focus states.
    ///   Forwards the Loaded event to UpdateState.
    /// </remarks>
    public class ControlBehavior : VisualStateBehavior
    {
        #region Properties

        /// <summary>
        ///   This behavior targets Control derived Controls.
        /// </summary>
        protected internal override Type TargetType
        {
            get
            {
                return typeof(Control);
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
            control.Loaded += delegate { this.UpdateState(control, false); };
            AddValueChanged(UIElement.IsKeyboardFocusWithinProperty, typeof(Control), control, this.UpdateStateHandler);
        }

        /// <summary>
        /// Detaches property changes and events.
        /// </summary>
        /// <param name="control">
        /// The control
        /// </param>
        protected override void OnDetach(Control control)
        {
            RemoveValueChanged(UIElement.IsKeyboardFocusWithinProperty, typeof(Control), control, this.UpdateStateHandler);
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
            VisualStateManager.GoToState(control, control.IsKeyboardFocusWithin ? "Focused" : "Unfocused", useTransitions);
        }

        /// <summary>
        /// </summary>
        /// <param name="o">
        /// </param>
        /// <param name="e">
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        protected override void UpdateStateHandler(object o, EventArgs e)
        {
            var cont = o as Control;
            if (cont == null)
            {
                throw new InvalidOperationException("This should never be used on anything other than a control.");
            }

            this.UpdateState(cont, true);
        }

        #endregion
    }
}