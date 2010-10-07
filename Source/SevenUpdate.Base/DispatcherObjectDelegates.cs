// ***********************************************************************
// Assembly         : SevenUpdate.Base
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// Extension Methods for Dispatcher.BeginInvoke
    /// </summary>
    public static class DispatcherObjectDelegates
    {
        #region Public Methods

        /// <summary>
        /// Invokes a Method to run on the UI thread
        /// </summary>
        /// <param name="dispatcher">
        /// the dispatcher object
        /// </param>
        /// <param name="method">
        /// the method to invoke
        /// </param>
        public static void BeginInvoke(this Dispatcher dispatcher, Action method)
        {
            dispatcher.BeginInvoke(method, DispatcherPriority.Background, null);
        }

        /// <summary>
        /// Invokes a Method to run on the UI thread
        /// </summary>
        /// <typeparam name="T">
        /// the parameter type
        /// </typeparam>
        /// <param name="dispatcher">
        /// the dispatcher object
        /// </param>
        /// <param name="method">
        /// the method to invoke
        /// </param>
        /// <param name="parameter">
        /// the parameter to pass to the method
        /// </param>
        public static void BeginInvoke<T>(this Dispatcher dispatcher, Action<T> method, T parameter)
        {
            dispatcher.BeginInvoke(method, DispatcherPriority.Background, parameter);
        }

        #endregion
    }
}