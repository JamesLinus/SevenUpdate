// ***********************************************************************
// <copyright file="RestartSettings.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    using Diagnostics.CodeAnalysis;

    using Globalization;

    /// <summary>Specifies the options for an application to be automatically restarted by Windows Error Reporting.</summary>
    /// <remarks>
    ///   Regardless of these settings, the application will not be restarted if it executed for less than 60 seconds
    ///   before terminating.
    /// </remarks>
    public class RestartSettings
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <c>RestartSettings</c> class.</summary>
        /// <param name="commandLine">  The command line arguments used to restart the application.</param>
        /// <param name="restrict">
        ///   A bitwise combination of the <c>RestartRestrictions</c>values that specify when the application should not
        ///   be restarted.
        /// </param>
        public RestartSettings(string commandLine, RestartRestrictions restrict)
        {
            this.Command = commandLine;
            this.Restrictions = restrict;
        }

        #endregion

        #region Properties

        /// <summary>Gets the command line arguments used to restart the application.</summary>
        /// <value>A <c>System.String</c> object.</value>
        public string Command { get; private set; }

        /// <summary>Gets the set of conditions when the application should not be restarted.</summary>
        /// <value>A set of <c>RestartRestrictions</c> values.</value>
        public RestartRestrictions Restrictions { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>Returns a string representation of the current state of this object.</summary>
        /// <returns>
        ///   A <c>System.String</c> that displays the command line arguments and restrictions for restarting
        ///   the application.
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.string.Format(System.String,System.Object,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture, "command: {0} restrictions: {1}", this.Command, this.Restrictions);
        }

        #endregion
    }
}