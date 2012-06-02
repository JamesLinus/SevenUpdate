// <copyright file="IPriorApplicationInstance.cs" project="SevenSoftware.Windows">BladeWise</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace SevenSoftware.Windows
{
    using System.ServiceModel;

    /// <summary>Interface used to signal a prior instance of the application about the startup another instance.</summary>
    [ServiceContract]
    internal interface IPriorApplicationInstance
    {
        /// <summary>Signals the startup of the next application instance.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
        [OperationContract]
        void SignalStartupNextInstance(string[] args);
    }
}