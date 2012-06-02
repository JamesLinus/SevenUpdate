// <copyright file="MyServiceHost.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    using ProtoBuf.ServiceModel;

    using SevenUpdate.Service;

    /// <summary>Contains methods to start the WCF service host.</summary>
    internal static class MyServiceHost
    {
        /// <summary>Gets or sets the <c>ServiceHost</c> instance.</summary>
        private static ServiceHost Instance { get; set; }

        /// <summary>Starts the service.</summary>
        internal static void StartService()
        {
            if (Instance != null)
            {
                return;
            }

            var binding = new NetNamedPipeBinding
                {
                    Name = "sevenupdatebinding", Security = 
                    { Mode = NetNamedPipeSecurityMode.Transport } 
                };

            var baseAddress = new Uri("net.pipe://localhost/sevenupdate/");

            Instance = new ServiceHost(typeof(WcfService), baseAddress);

#if (DEBUG)
            var debug = Instance.Description.Behaviors.Find<ServiceDebugBehavior>();

            // if not found - add behavior with setting turned on 
            if (debug == null)
            {
                Instance.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                // make sure setting is turned ON
                debug.IncludeExceptionDetailInFaults = true;
            }

#endif
            Instance.AddServiceEndpoint(typeof(IElevatedProcessCallback), binding, baseAddress).Behaviors.Add(
                new ProtoEndpointBehavior());
            try
            {
                Instance.Open();
            }
            catch (InvalidOperationException)
            {
                Instance = null;
            }
        }

        /// <summary>Stops the service.</summary>
        internal static void StopService()
        {
            if (Instance == null)
            {
                return;
            }

            // Call StopService from your shutdown logic (i.e. dispose method)
            if (Instance.State != CommunicationState.Closed)
            {
                try
                {
                    Instance.BeginClose(null, null);
                }
                catch (Exception ex)
                {
                    if (
                        !(ex is CommunicationObjectAbortedException || ex is CommunicationObjectFaultedException
                          || ex is ObjectDisposedException))
                    {
                        Utilities.ReportError(ex, ErrorType.FatalError);
                        throw;
                    }
                }
            }

            Instance = null;
        }
    }
}