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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Specifies the redirection capabilities for known folders.
    /// </summary>
    public enum RedirectionCapabilities
    {
        /// <summary>
        ///   Redirection capability is unknown.
        /// </summary>
        None = 0x00,
        /// <summary>
        ///   The known folder can be redirected.
        /// </summary>
        AllowAll = 0xff,
        /// <summary>
        ///   The known folder can be redirected. 
        ///   Currently, redirection exists only for 
        ///   common and user folders; fixed and virtual folders 
        ///   cannot be redirected.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Redirectable", Justification = "This is following the native API")] Redirectable = 0x1,
        /// <summary>
        ///   Redirection is not allowed.
        /// </summary>
        DenyAll = 0xfff00,
        /// <summary>
        ///   The folder cannot be redirected because it is 
        ///   already redirected by group policy.
        /// </summary>
        DenyPolicyRedirected = 0x100,
        /// <summary>
        ///   The folder cannot be redirected because the policy 
        ///   prohibits redirecting this folder.
        /// </summary>
        DenyPolicy = 0x200,
        /// <summary>
        ///   The folder cannot be redirected because the calling 
        ///   application does not have sufficient permissions.
        /// </summary>
        DenyPermissions = 0x400
    }
}