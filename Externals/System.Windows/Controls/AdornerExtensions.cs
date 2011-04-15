// ***********************************************************************
// <copyright file="AdornerExtensions.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace System.Windows.Controls
{
    using System.Windows.Documents;

    /// <summary>The adorner extensions</summary>
    public static class AdornerExtensions
    {
        #region Public Methods

        /// <summary>Determines whether the adorner layer contains an element</summary>
        /// <param name="adorner">The adorner.</param>
        /// <param name="element">The element</param>
        /// <returns><see langword="true" /> if the adorner layer contains the element otherwise, <see langword="false" />.</returns>
        /// <typeparam name="T">The object to look for</typeparam>
        public static bool Contains<T>(this AdornerLayer adorner, UIElement element)
        {
            if (adorner == null)
            {
                return false;
            }

            var adorners = adorner.GetAdorners(element);

            if (adorners == null)
            {
                return false;
            }

            for (var i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Removes the adorners</summary>
        /// <param name="adorner">The adorner</param>
        /// <param name="element">The element</param>
        /// <typeparameter name="T">The type of element</typeparameter>
        /// <typeparam name="T">The type of element</typeparam>
        public static void RemoveAdorners<T>(this AdornerLayer adorner, UIElement element)
        {
            if (adorner == null)
            {
                throw new ArgumentNullException("adorner");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            var adorners = adorner.GetAdorners(element);

            if (adorners == null)
            {
                return;
            }

            for (var i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                {
                    adorner.Remove(adorners[i]);
                }
            }
        }

        /// <summary>Removes all.</summary>
        /// <param name="adorner">The adorner layer</param>
        /// <param name="element">The elementent</param>
        public static void RemoveAll(this AdornerLayer adorner, UIElement element)
        {
            if (adorner == null)
            {
                throw new ArgumentNullException("adorner");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            var adorners = adorner.GetAdorners(element);
            if (adorners == null)
            {
                return;
            }

            foreach (var toRemove in adorners)
            {
                adorner.Remove(toRemove);
            }
        }

        #endregion
    }
}