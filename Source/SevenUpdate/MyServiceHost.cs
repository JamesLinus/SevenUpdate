namespace SevenUpdate
{
    using System;
    using System.ServiceModel;

    using SevenUpdate.Service;

    internal static class MyServiceHost
    {
        internal static ServiceHost Instance;

        internal static void StartService(string pipeEndPoint)
        {
            var binding = new NetNamedPipeBinding
                {
                    Name = "uacbinding",
                    Security =
                        {
                            Mode = NetNamedPipeSecurityMode.Transport
                        }
                };

            var baseAddress = new Uri("net.pipe://localhost/sevenupdate/" + pipeEndPoint);

            Instance = new ServiceHost(typeof(WcfService), baseAddress);
            Instance.AddServiceEndpoint(typeof(IWaitForElevatedProcess), binding, baseAddress).Behaviors.Add(new ProtoBuf.ServiceModel.ProtoEndpointBehavior());
            Instance.Open();
        }

        internal static void StopService()
        {
            if (Instance == null)
                return;
            // Call StopService from your shutdown logic (i.e. dispose method)
            if (Instance.State != CommunicationState.Closed)
                Instance.Close();
        }
    }
}
