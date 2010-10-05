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
    /// </summary>
    internal class VisualStateBehaviorFactory : TypeHandlerFactory<VisualStateBehavior>
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        [ThreadStatic]
        private static VisualStateBehaviorFactory _instance;

        /// <summary>
        /// </summary>
        [ThreadStatic]
        private static bool _registeredKnownTypes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        private VisualStateBehaviorFactory()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        internal static VisualStateBehaviorFactory Instance
        {
            get
            {
                return _instance ?? (_instance = new VisualStateBehaviorFactory());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="control">
        /// </param>
        internal static void AttachBehavior(Control control)
        {
            // If the VisualStateBehavior has already been set in some way other than the default value, 
            // then let that value win.
            if (DependencyPropertyHelper.GetValueSource(control, VisualStateBehavior.VisualStateBehaviorProperty).BaseValueSource != BaseValueSource.Default)
            {
                return;
            }

            if (!_registeredKnownTypes)
            {
                // When using the Toolkit version of VSM for WPF, the controls
                // don't know about VSM and don't change states. Thus, these
                // behaviors help bootstrap that behavior.
                // In order to appear compatible with Silverlight, we can
                // pre-register these behaviors. When moved into WPF, these
                // behaviors should be unnecessary and this can go away.
                _registeredKnownTypes = true;

                // These are the known behaviors in the Toolkit.
                RegisterControlBehavior(new ButtonBaseBehavior());
                RegisterControlBehavior(new ToggleButtonBehavior());
                RegisterControlBehavior(new ListBoxItemBehavior());
                RegisterControlBehavior(new TextBoxBaseBehavior());
                RegisterControlBehavior(new ProgressBarBehavior());
            }

            // No VisualStateBehavior has been specified, check the list of registered behaviors.
            var behavior = Instance.GetHandler(control.GetType());
            if (behavior != null)
            {
                VisualStateBehavior.SetVisualStateBehavior(control, behavior);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="behavior">
        /// </param>
        internal static void RegisterControlBehavior(VisualStateBehavior behavior)
        {
            Instance.RegisterHandler(behavior);
        }

        /// <summary>
        /// </summary>
        /// <param name="behavior">
        /// </param>
        /// <returns>
        /// </returns>
        protected override Type GetBaseType(VisualStateBehavior behavior)
        {
            return behavior.TargetType;
        }

        #endregion
    }
}