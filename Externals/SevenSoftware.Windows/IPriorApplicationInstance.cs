// ***********************************************************************
// <copyright file="IPriorApplicationInstance.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) BladeWise. All rights reserved.
// </copyright>
// <author username="BladeWise">BladeWise</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://wpfinstanceawareapp.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************

namespace SevenSoftware.Windows
{
    using System.ServiceModel;

    /// <summary>Interface used to signal a prior instance of the application about the startup another instance.</summary>
    [ServiceContract]
    internal interface IPriorApplicationInstance
    {
        #region Public Methods and Operators

        /// <summary>Signals the startup of the next application instance.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
        [OperationContract]
        void SignalStartupNextInstance(string[] args);

        #endregion
    }
}