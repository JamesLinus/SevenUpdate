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
    /// Provides VisualStateManager behavior for ProgressBar controls.
    /// </summary>
    public class ProgressBarBehavior : ControlBehavior
    {
        #region Properties

        /// <summary>
        ///   This behavior targets ProgressBar derived Controls.
        /// </summary>
        protected internal override Type TargetType
        {
            get
            {
                return typeof(ProgressBar);
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

            var progressBar = (ProgressBar)control;
            var targetType = typeof(ProgressBar);

            AddValueChanged(ProgressBar.IsIndeterminateProperty, targetType, progressBar, this.UpdateStateHandler);
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

            var progressBar = (ProgressBar)control;
            var targetType = typeof(ProgressBar);

            RemoveValueChanged(ProgressBar.IsIndeterminateProperty, targetType, progressBar, this.UpdateStateHandler);
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
            var progressBar = (ProgressBar)control;

            VisualStateManager.GoToState(progressBar, !progressBar.IsIndeterminate ? "Determinate" : "Indeterminate", useTransitions);

            base.UpdateState(control, useTransitions);
        }

        #endregion
    }
}