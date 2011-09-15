// ***********************************************************************
// <copyright file="RestartSettings.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    using System.Globalization;
    using System.Windows.Properties;

    /// <summary>Specifies the options for an application to be automatically restarted by Windows Error Reporting.</summary>
    /// <remarks>
    ///   Regardless of these settings, the application will not be restarted if it executed for less than 60 seconds
    ///   before terminating.
    /// </remarks>
    public class RestartSettings
    {
        #region Constants and Fields

        /// <summary>Gets the command line arguments used to restart the application.</summary>
        /// <value>A <c>System.String</c> object.</value>
        private string command;

        /// <summary>Gets the set of conditions when the application should not be restarted.</summary>
        /// <value>A set of <c>RestartRestrictions</c> values.</value>
        private RestartRestrictions restrictions;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "RestartSettings" /> class.  Creates a new instance of the RestartSettings class.</summary>
        /// <param name = "command">The command line arguments used to restart the application.</param>
        /// <param name = "restrictions">A bitwise combination of the RestartRestrictions values that specify when the application should not be restarted.</param>
        public RestartSettings(string command, RestartRestrictions restrictions)
        {
            this.command = command;
            this.restrictions = restrictions;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the command line arguments used to restart the application.</summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Command
        {
            get
            {
                return this.command;
            }
        }

        /// <summary>Gets the set of conditions when the application should not be restarted.</summary>
        /// <value>A set of <see cref = "RestartRestrictions" /> values.</value>
        public RestartRestrictions Restrictions
        {
            get
            {
                return this.restrictions;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Returns a string representation of the current state of this object.</summary>
        /// <returns>A <see cref = "System.String" /> that displays the command line arguments and restrictions for restarting the application.</returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, 
                Resources.RestartSettingsFormatString, 
                this.command, 
                this.restrictions.ToString());
        }

        #endregion
    }
}
