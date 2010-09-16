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
    ///   Defines methods and properties for recovery settings, and specifies options for an application that attempts
    ///   to perform final actions after a fatal event, such as an
    ///   unhandled exception.
    /// </summary>
    /// <remarks>
    ///   This class is used to register for application recovery.
    ///   See the <see cref = "ApplicationRestartRecoveryManager" /> class.
    /// </remarks>
    public abstract class RecoverySettings
    {
        /// <summary>
        ///   Initializes a new instance of the <b>RecoverySettings</b> class.
        /// </summary>
        /// <param name = "data">A recovery data object that contains the callback method (invoked by the system
        ///   before Windows Error Reporting terminates the application) and an optional state object.</param>
        /// <param name = "interval">The time interval within which the 
        ///   callback method must invoke <see cref = "ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress" /> to 
        ///   prevent WER from terminating the application.</param>
        /// <seealso cref = "ApplicationRestartRecoveryManager" />
        public RecoverySettings(RecoveryData data, uint interval)
        {
            RecoveryData = data;
            PingInterval = interval;
        }

        /// <summary>
        ///   Gets the recovery data object that contains the callback method and an optional
        ///   parameter (usually the state of the application) to be passed to the 
        ///   callback method.
        /// </summary>
        /// <value>A <see cref = "RecoveryData" /> object.</value>
        public RecoveryData RecoveryData { get; private set; }

        /// <summary>
        ///   Gets the time interval for notifying Windows Error Reporting.  
        ///   The <see cref = "RecoveryCallback" /> method must invoke <see cref = "ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress" /> 
        ///   within this interval to prevent WER from terminating the application.
        /// </summary>
        /// <remarks>
        ///   The recovery ping interval is specified in milliseconds. 
        ///   By default, the interval is 5 seconds. 
        ///   If you specify zero, the default interval is used.
        /// </remarks>
        public uint PingInterval { get; private set; }

        /// <summary>
        ///   Returns a string representation of the current state
        ///   of this object.
        /// </summary>
        /// <returns>A <see cref = "System.String" /> object.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return String.Format("delegate: {0}, state: {1}, ping: {2}", RecoveryData.Callback.Method, RecoveryData.State, PingInterval);
        }
    }
}