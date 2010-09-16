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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.ApplicationServices
{
    /// <summary>
    ///   Specifies the options for an application to be automatically
    ///   restarted by Windows Error Reporting.
    /// </summary>
    /// <remarks>
    ///   Regardless of these 
    ///   settings, the application
    ///   will not be restarted if it executed for less than 60 seconds before
    ///   terminating.
    /// </remarks>
    public abstract class RestartSettings
    {
        /// <summary>
        ///   Creates a new instance of the RestartSettings class.
        /// </summary>
        /// <param name = "commandLine">The command line arguments 
        ///   used to restart the application.</param>
        /// <param name = "restrict">A bitwise combination of the RestartRestrictions 
        ///   values that specify  
        ///   when the application should not be restarted.
        /// </param>
        public RestartSettings(string commandLine, RestartRestrictions restrict)
        {
            Command = commandLine;
            Restrictions = restrict;
        }

        /// <summary>
        ///   Gets the command line arguments used to restart the application.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Command { get; private set; }

        /// <summary>
        ///   Gets the set of conditions when the application 
        ///   should not be restarted.
        /// </summary>
        /// <value>A set of <see cref = "RestartRestrictions" /> values.</value>
        public RestartRestrictions Restrictions { get; private set; }

        /// <summary>
        ///   Returns a string representation of the current state
        ///   of this object.
        /// </summary>
        /// <returns>A <see cref = "System.String" /> that displays 
        ///   the command line arguments 
        ///   and restrictions for restarting the application.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return String.Format("command: {0} restrictions: {1}", Command, Restrictions);
        }
    }
}