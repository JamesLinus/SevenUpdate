// ***********************************************************************
// <copyright file="ImportanceSorter.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt">GNU General Public License Version 3</license>
// ***********************************************************************
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
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
namespace SevenUpdate.CustomComparer
{
    /// <summary>
    /// Sorts Importance
    /// </summary>
    internal static class ImportanceSorter
    {
        #region Methods

        /// <summary>
        /// Compares two <see cref="Importance"/> objects
        /// </summary>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// Value  Condition Less than zero <paramref name="x"/> is less than <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>.
        ///   Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        internal static int CompareImportance(Importance x, Importance y)
        {
            var firstRank = 0;

            switch (x)
            {
                case Importance.Important:
                    firstRank = 0;
                    break;
                case Importance.Recommended:
                    firstRank = 1;
                    break;
                case Importance.Optional:
                    firstRank = 2;
                    break;
                case Importance.Locale:
                    firstRank = 3;
                    break;
            }

            var secondRank = 0;
            switch (y)
            {
                case Importance.Important:
                    secondRank = 0;
                    break;
                case Importance.Recommended:
                    secondRank = 1;
                    break;
                case Importance.Optional:
                    secondRank = 2;
                    break;
                case Importance.Locale:
                    secondRank = 3;
                    break;
            }

            return firstRank > secondRank ? 1 : (firstRank == secondRank ? 0 : -1);
        }

        #endregion
    }
}