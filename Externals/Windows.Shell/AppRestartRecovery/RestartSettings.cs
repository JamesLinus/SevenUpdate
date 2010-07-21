//Copyright (c) Microsoft Corporation.  All rights reserved.

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
    public class RestartSettings
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