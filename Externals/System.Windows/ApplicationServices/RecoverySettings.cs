// ***********************************************************************
// <copyright file="RecoverySettings.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    using System.Globalization;
    using System.Windows.Properties;

    /// <summary>
    ///   Defines methods and properties for recovery settings, and specifies options for an application that attempts
    ///   to perform final actions after a fatal event, such as an unhandled exception.
    /// </summary>
    /// <remarks>
    ///   This class is used to register for application recovery. See the <see cref =
    ///   "ApplicationRestartRecoveryManager" /> class.
    /// </remarks>
    public class RecoverySettings
    {
        #region Constants and Fields

        /// <summary>Gets the time interval for notifying Windows Error Reporting.  The <c>RecoveryCallback</c> method must invoke <c>ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress</c> within this interval to prevent WER from terminating the application.</summary>
        /// <remarks>
        ///   The recovery ping interval is specified in milliseconds. By default, the interval is 5 seconds. If you
        ///   specify zero, the default interval is used.
        /// </remarks>
        private uint pingInterval;

        /// <summary>
        ///   Gets the recovery data object that contains the callback method and an optional parameter (usually the
        ///   state of the application) to be passed to the callback method.
        /// </summary>
        /// <value>A <c>RecoveryData</c> object.</value>
        private RecoveryData recoveryData;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="RecoverySettings" /> class.</summary>
        /// <param name="data">A recovery data object that contains the callback method (invoked by the system before Windows Error Reporting terminates the application) and an optional state object.</param>
        /// <param name="interval">The time interval within which the callback method must invoke <see
        /// cref="ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress" /> to prevent WER from terminating
        /// the application.</param>
        /// <seealso cref="ApplicationRestartRecoveryManager" />
        public RecoverySettings(RecoveryData data, uint interval)
        {
            this.recoveryData = data;
            this.pingInterval = interval;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the time interval for notifying Windows Error Reporting. The <see cref="RecoveryCallback" /> method
        ///   must invoke <see cref="ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress" /> within this
        ///   interval to
        /// prevent WER from terminating the application.
        /// </summary>
        /// <remarks>
        ///   The recovery ping interval is specified in milliseconds. By default, the interval is 5 seconds. If you
        ///   specify zero, the default interval is used.
        /// </remarks>
        public uint PingInterval
        {
            get
            {
                return this.pingInterval;
            }
        }

        /// <summary>
        ///   Gets the recovery data object that contains the callback method and an optional parameter (usually the
        ///   state of the application) to be passed to the callback method.
        /// </summary>
        /// <value>A <see cref="RecoveryData" /> object.</value>
        public RecoveryData RecoveryData
        {
            get
            {
                return this.recoveryData;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Returns a string representation of the current state of this object.</summary>
        /// <returns>A <see cref="System.String" /> object.</returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                Resources.RecoverySettingsFormatString,
                this.recoveryData.Callback.Method,
                this.recoveryData.State,
                this.PingInterval);
        }

        #endregion
    }
}
