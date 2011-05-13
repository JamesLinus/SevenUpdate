// ***********************************************************************
// <copyright file="RecoveryData.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.ApplicationServices
{
    /// <summary>The <see cref="System.Delegate" /> that represents the callback method invoked by the system when an application has registered for application recovery.</summary>
    /// <param name="state">An application-defined state object that is passed to the callback method.</param>
    /// <returns>The result code.</returns>
    /// <remarks>The callback method will be invoked prior to the application being terminated by Windows Error Reporting (WER). To keep WER from terminating the application before the callback method completes, the callback method must periodically call the <see cref="ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress" /> method.</remarks>
    /// <seealso cref="ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(RecoverySettings)" />
    public delegate int RecoveryCallback(object state);

    /// <summary>Defines a class that contains a callback delegate and properties of the application as defined by the user.</summary>
    public class RecoveryData
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="RecoveryData" /> class.</summary>
        /// <param name="callback">The callback delegate.</param>
        /// <param name="state">The current state of the application.</param>
        public RecoveryData(RecoveryCallback callback, object state)
        {
            this.Callback = callback;
            this.State = state;
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether the recovery callback function.</summary>
        public RecoveryCallback Callback { get; set; }

        /// <summary>Gets or sets a value indicating whether the application state.</summary>
        public object State { get; set; }

        #endregion

        #region Public Methods

        /// <summary>Invokes the recovery callback function.</summary>
        public void Invoke()
        {
            if (this.Callback != null)
            {
                this.Callback(this.State);
            }
        }

        #endregion
    }
}