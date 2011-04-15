// ***********************************************************************
// <copyright file="DwmMessages.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Internal
{
    /// <summary>The DWM messages</summary>
    public static class DwmMessages
    {
        #region Constants and Fields

        /// <summary>Dwm has been enabled or Disabled</summary>
        internal const int CompositionChanged = 0x031E;

        /// <summary>Dwn rendering has changed</summary>
        internal const int RenderingChanged = 0x031F;

        #endregion
    }
}