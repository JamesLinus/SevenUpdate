// ***********************************************************************
// <copyright file="DispatcherObjectDelegates.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Windows.Threading;

    /// <summary>Extension Methods for Dispatcher.BeginInvoke</summary>
    public static class DispatcherObjectDelegates
    {
        #region Public Methods

        /// <summary>Invokes a Method to run on the UI thread</summary>
        /// <param name="dispatcher">the dispatcher object</param>
        /// <param name="method">the method to invoke</param>
        public static void BeginInvoke(this Dispatcher dispatcher, Action method)
        {
            if (dispatcher != null)
                dispatcher.BeginInvoke(method, DispatcherPriority.Background, null);
        }

        /// <summary>Invokes a Method to run on the UI thread</summary>
        /// <typeparam name="T">the parameter type</typeparam>
        /// <param name="dispatcher">the dispatcher object</param>
        /// <param name="method">the method to invoke</param>
        /// <param name="parameter">the parameter to pass to the method</param>
        public static void BeginInvoke<T>(this Dispatcher dispatcher, Action<T> method, T parameter)
        {
            if (dispatcher != null)
                dispatcher.BeginInvoke(method, DispatcherPriority.Background, parameter);
        }

        #endregion
    }
}