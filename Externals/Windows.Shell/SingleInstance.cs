#region

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Windows;

#endregion

namespace Microsoft.Windows.Shell
{
    public class SingleInstance : IDisposable, IInstanceProvider, IEndpointBehavior
    {
        #region Delegates

        public delegate void ArgsHandler(string[] args);

        #endregion

        private readonly Guid appGuid;
        private readonly string assemblyName;

        private readonly Mutex mutex;
        private ServiceHost host;
        private bool owned;
        private Window window;

        public SingleInstance(Guid appGuid)
        {
            this.appGuid = appGuid;
            assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            mutex = new Mutex(true, assemblyName + this.appGuid, out owned);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (owned) // always release a mutex if you own it
            {
                owned = false;
                mutex.ReleaseMutex();
            }

            if (host != null)
            {
                CloseCommunicationObject(host);
                host = null; // to be on the safe side
            }
        }

        #endregion

        public event ArgsHandler ArgsRecieved;

        public void Run(Func<Window> showWindow, string[] args)
        {
            if (owned)
            {
                // show the main app window
                window = showWindow();
                // and start the service
                StartServiceHost();
            }
            else
            {
                SendCommandLineArgs(args);
                Application.Current.Shutdown();
            }
        }

        private static void CloseCommunicationObject(ICommunicationObject commObject)
        {
            try
            {
                commObject.Close();
            }
            catch
            {
                commObject.Abort();
            }
        }

        private string GetAddress()
        {
            return string.Format("net.pipe://localhost/{0}{1}", assemblyName, appGuid);
        }

        private static Binding GetBinding()
        {
            return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
        }

        private void StartServiceHost()
        {
            try
            {
                host = new ServiceHost(typeof (SingleInstanceService));
                var endpoint = host.AddServiceEndpoint(typeof (ISingleInstanceService), GetBinding(), GetAddress());
                endpoint.Behaviors.Add(this);
                host.BeginOpen(null, null);
            }
            catch(Exception e)
            {
                host = null;
                // log it
            }
        }

        private void BringToFront(Guid appGuid)
        {
            if (appGuid != this.appGuid)
                return;
            if (window.WindowState == WindowState.Minimized)
                window.WindowState = WindowState.Normal;
            window.Activate();
        }

        private void ProcessArgs(Guid appGuid, string[] args)
        {
            if (appGuid == this.appGuid && ArgsRecieved != null)
                ArgsRecieved(args);
        }

        private void SendCommandLineArgs(string[] args)
        {
            ISingleInstanceService proxy = null;
            try
            {
                proxy = ChannelFactory<ISingleInstanceService>.CreateChannel(GetBinding(), new EndpointAddress(GetAddress()));
                proxy.BringToFront(appGuid);
                proxy.ProcessArguments(appGuid, args);
            }
            catch
            {
                // log it
            }
            finally
            {
                if (proxy != null)
                    CloseCommunicationObject(proxy as ICommunicationObject);
            }
        }

        #region IInstanceProvider

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return new SingleInstanceService(BringToFront, ProcessArgs);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            // no disposing logic for our service
        }

        #endregion

        #region IEndpointBehavior

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.InstanceProvider = this;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}