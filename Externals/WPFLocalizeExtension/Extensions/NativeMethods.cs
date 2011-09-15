// ***********************************************************************
// <copyright file="NativeMethods.cs" project="WPFLocalizeExtension" assembly="WPFLocalizeExtension" solution="SevenUpdate" company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// <license href="http://wpflocalizeextension.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************

namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>Native Win32 methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>Frees memory of a pointer.</summary>
        /// <param name = "o">Object to remove from memory.</param>
        /// <returns>0 if the removing was success, otherwise another number.</returns>
        [DllImport(@"gdi32.dll")]
        internal static extern int DeleteObject(IntPtr o);
    }
}
