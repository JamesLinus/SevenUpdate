#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;

#endregion

namespace Microsoft.Windows.ApplicationServices
{
    /// <summary>
    ///   Specifies the conditions when Windows Error Reporting
    ///   should not restart an application that has registered
    ///   for automatic restart.
    /// </summary>
    [Flags]
    public enum RestartRestrictions
    {
        /// <summary>
        ///   Always restart the application.
        /// </summary>
        None = 0,
        /// <summary>
        ///   Do not restart when the application has crashed.
        /// </summary>
        NotOnCrash = 1,
        /// <summary>
        ///   Do not restart when the application is hung.
        /// </summary>
        NotOnHang = 2,
        /// <summary>
        ///   Do not restart when the application is terminated
        ///   due to a system update.
        /// </summary>
        NotOnPatch = 4,
        /// <summary>
        ///   Do not restart when the application is terminated 
        ///   because of a system reboot.
        /// </summary>
        NotOnReboot = 8
    }
}