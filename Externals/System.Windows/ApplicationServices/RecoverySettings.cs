// ***********************************************************************
// <copyright file="RecoverySettings.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.ApplicationServices
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Defines methods and properties for recovery settings, and specifies options for an application that attempts
    ///   to perform final actions after a fatal event, such as an
    ///   unhandled exception.
    /// </summary>
    /// <remarks>
    /// This class is used to register for application recovery.
    ///   See the <see cref="ApplicationRestartRecoveryManager"/> class.
    /// </remarks>
    public class RecoverySettings
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="RecoverySettings"/> class.</summary>
        /// <param name="data">
        /// A recovery data object that contains the callback method (invoked by the system
        ///   before Windows Error Reporting terminates the application) and an optional state object.
        /// </param>
        /// <param name="interval">
        /// The time interval within which the
        ///   callback method must invoke <see cref="ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress"/> to
        ///   prevent WER from terminating the application.
        /// </param>
        /// <seealso cref="ApplicationRestartRecoveryManager"/>
        protected RecoverySettings(RecoveryData data, uint interval)
        {
            this.RecoveryData = data;
            this.PingInterval = interval;
        }

        #endregion

        #region Properties

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
        public uint PingInterval
        {
            get;
            private set;
        }

        /// <summary>
        ///   Gets the recovery data object that contains the callback method and an optional
        ///   parameter (usually the state of the application) to be passed to the 
        ///   callback method.
        /// </summary>
        /// <value>A <see cref = "RecoveryData" /> object.</value>
        public RecoveryData RecoveryData
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string representation of the current state
        ///   of this object.
        /// </summary>
        /// <returns>A <see cref="System.String"/> object.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "delegate: {0}, state: {1}, ping: {2}", this.RecoveryData.Callback.Method, this.RecoveryData.State, this.PingInterval);
        }

        #endregion
    }
}