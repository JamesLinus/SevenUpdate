// <copyright file="NativeMethods.cs" project="WPFLocalizeExtension">Bernhard Millauer</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>Native Win32 methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>Frees memory of a pointer.</summary>
        /// <param name="o">Object to remove from memory.</param>
        /// <returns>0 if the removing was success, otherwise another number.</returns>
        [DllImport(@"gdi32.dll")]
        internal static extern int DeleteObject(IntPtr o);
    }
}