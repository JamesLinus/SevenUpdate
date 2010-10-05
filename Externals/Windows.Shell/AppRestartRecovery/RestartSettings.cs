//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.ApplicationServices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Specifies the options for an application to be automatically
    ///   restarted by Windows Error Reporting.
    /// </summary>
    /// <remarks>
    /// Regardless of these 
    ///   settings, the application
    ///   will not be restarted if it executed for less than 60 seconds before
    ///   terminating.
    /// </remarks>
    public class RestartSettings
    {
        #region Constructors and Destructors

        /// <summary>
        /// Creates a new instance of the RestartSettings class.
        /// </summary>
        /// <param name="commandLine">
        /// The command line arguments 
        ///   used to restart the application.
        /// </param>
        /// <param name="restrict">
        /// A bitwise combination of the RestartRestrictions 
        ///   values that specify  
        ///   when the application should not be restarted.
        /// </param>
        protected RestartSettings(string commandLine, RestartRestrictions restrict)
        {
            this.Command = commandLine;
            this.Restrictions = restrict;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string representation of the current state
        ///   of this object.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that displays 
        ///   the command line arguments 
        ///   and restrictions for restarting the application.
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", 
            Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "command: {0} restrictions: {1}", this.Command, this.Restrictions);
        }

        #endregion
    }
}