// ***********************************************************************
// <copyright file="OddsFormatType.cs"
//            project="WPFLocalizeExtension"
//            assembly="WPFLocalizeExtension"
//            solution="SevenUpdate"
//            company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// ***********************************************************************
namespace WPFLocalizeExtension.Engine
{
    /// <summary>
    /// Defines the Format of the odds output.
    /// </summary>
    public enum OddsFormatType
    {
        /// <summary>
        ///   Format "1.23"
        /// </summary>
        EU = 0, 

        /// <summary>
        ///   Format "1/2"
        /// </summary>
        UK = 1, 

        /// <summary>
        ///   Format "-200"
        /// </summary>
        US = 2
    }
}