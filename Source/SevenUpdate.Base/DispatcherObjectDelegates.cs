// <copyright file="DispatcherObjectDelegates.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Windows.Threading;

    /// <summary>Extension Methods for Dispatcher.BeginInvoke.</summary>
    public static class DispatcherObjectDelegates
    {
        ///// <summary>Invokes a Method to run on the UI thread.</summary>
        ///// <param name="dispatcher">The dispatcher object.</param>
        ///// <param name="method">The method to invoke.</param>
        ///// <param name="background"> </param>
        // public static void BeginInvoke(this Dispatcher dispatcher, Action method, DispatcherPriority background)
        // {
        // if (method == null)
        // {
        // throw new ArgumentNullException("method");
        // }

        // if (dispatcher != null)
        // {
        // dispatcher.BeginInvoke(method, DispatcherPriority.Background, null);
        // }
        // }

        /// <summary>Invokes a Method to run on the UI thread.</summary>
        /// <param name="dispatcher">The dispatcher object.</param>
        /// <param name="method">The method to invoke.</param>
        /// <param name="parameter">The parameter to pass to the method.</param>
        /// <typeparam name="T">The method to invoke under the UI thread.</typeparam>
        public static void BeginInvoke<T>(this Dispatcher dispatcher, Action<T> method, T parameter)
        {
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(method, DispatcherPriority.Background, parameter);
            }
        }

        /// <summary>Invokes a Method to run on the UI thread.</summary>
        /// <param name="dispatcher">The dispatcher object.</param>
        /// <param name="method">The method to invoke.</param>
        /// <typeparam name="T">The method to invoke under the UI thread.</typeparam>
        public static void BeginInvoke<T>(this Dispatcher dispatcher, Delegate method)
        {
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(method, DispatcherPriority.Background);
            }
        }

        /// <summary>Invokes a Method to run on the UI thread.</summary>
        /// <param name="dispatcher">The dispatcher object.</param>
        /// <param name="method">The method to invoke.</param>
        /// <typeparam name="T">The method to invoke under the UI thread.</typeparam>
        public static void BeginInvoke<T>(this Dispatcher dispatcher, Action<T> method)
        {
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(method, DispatcherPriority.Background);
            }
        }
    }
}