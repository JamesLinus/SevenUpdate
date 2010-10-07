// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.ApplicationServices
{
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
        /// Initializes a new instance of the <see cref="RestartSettings"/> class.
        /// </summary>
        /// <parameter name="commandLine">
        /// The command line arguments
        ///   used to restart the application.
        /// </parameter>
        /// <parameter name="restrict">
        /// A bitwise combination of the <see cref="RestartRestrictions"/>
        ///   values that specify
        ///   when the application should not be restarted.
        /// </parameter>
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