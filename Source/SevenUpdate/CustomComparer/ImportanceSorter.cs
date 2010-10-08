// ***********************************************************************
// <copyright file="ImportanceSorter.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.CustomComparer
{
    /// <summary>
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
            var xRank = 0;

            switch (x)
            {
                case Importance.Important:
                    xRank = 0;
                    break;
                case Importance.Recommended:
                    xRank = 1;
                    break;
                case Importance.Optional:
                    xRank = 2;
                    break;
                case Importance.Locale:
                    xRank = 3;
                    break;
            }

            var yRank = 0;
            switch (y)
            {
                case Importance.Important:
                    yRank = 0;
                    break;
                case Importance.Recommended:
                    yRank = 1;
                    break;
                case Importance.Optional:
                    yRank = 2;
                    break;
                case Importance.Locale:
                    yRank = 3;
                    break;
            }

            return xRank > yRank ? 1 : (xRank == yRank ? 0 : -1);
        }

        #endregion
    }
}