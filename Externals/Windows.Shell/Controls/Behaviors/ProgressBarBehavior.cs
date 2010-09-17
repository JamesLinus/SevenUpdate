//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Provides VisualStateManager behavior for ProgressBar controls.
    /// </summary>
    public class ProgressBarBehavior : ControlBehavior
    {
        /// <summary>
        ///   This behavior targets ProgressBar derived Controls.
        /// </summary>
        protected internal override Type TargetType { get { return typeof (ProgressBar); } }

        /// <summary>
        ///   Attaches to property changes and events.
        /// </summary>
        /// <param name = "control">An instance of the control.</param>
        protected override void OnAttach(Control control)
        {
            base.OnAttach(control);

            var progressBar = (ProgressBar) control;
            var targetType = typeof (ProgressBar);

            AddValueChanged(ProgressBar.IsIndeterminateProperty, targetType, progressBar, UpdateStateHandler);
        }

        /// <summary>
        ///   Detaches property changes and events.
        /// </summary>
        /// <param name = "control">The control</param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            var progressBar = (ProgressBar) control;
            var targetType = typeof (ProgressBar);

            RemoveValueChanged(ProgressBar.IsIndeterminateProperty, targetType, progressBar, UpdateStateHandler);
        }

        /// <summary>
        ///   Called to update the control's visual state.
        /// </summary>
        /// <param name = "control">The instance of the control being updated.</param>
        /// <param name = "useTransitions">Whether to use transitions or not.</param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            var progressBar = (ProgressBar) control;

            VisualStateManager.GoToState(progressBar, !progressBar.IsIndeterminate ? "Determinate" : "Indeterminate", useTransitions);

            base.UpdateState(control, useTransitions);
        }
    }
}