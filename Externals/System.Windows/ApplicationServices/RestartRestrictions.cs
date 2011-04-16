// ***********************************************************************
// <copyright file="RestartRestrictions.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.ApplicationServices
{
    /// <summary>Specifies the conditions when Windows Error Reportingshould not restart an application that has registeredfor automatic restart.</summary>
    [Flags]
    public enum RestartRestrictions
    {
        /// <summary>Always restart the application.</summary>
        None = 0,

        /// <summary>Do not restart when the application has crashed.</summary>
        NotOnCrash = 1,

        /// <summary>Do not restart when the application is hung.</summary>
        NotOnHang = 2,

        /// <summary>Do not restart when the application is terminateddue to a system update.</summary>
        NotOnPatch = 4,

        /// <summary>Do not restart when the application is terminated because of a system reboot.</summary>
        NotOnReboot = 8
    }
}