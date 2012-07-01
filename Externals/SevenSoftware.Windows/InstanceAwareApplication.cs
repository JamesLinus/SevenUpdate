// <copyright file="InstanceAwareApplication.cs" project="SevenSoftware.Windows">BladeWise</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SevenSoftware.Windows
{
    /// <summary>
    ///   Class used to define a WPF application which is aware of subsequent application instances running, either
    ///   locally (per session) or globally (per host).
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class InstanceAwareApplication : Application, IPriorApplicationInstance, IDisposable
    {
        /// <summary>The milliseconds to wait to determine if the current instance is the first one.</summary>
        const double FirstInstanceTimeoutMilliseconds = 500;

        /// <summary>The mutex prefix used if the application instance must be single per machine.</summary>
        const string GlobalPrefix = @"Global\";

        /// <summary>The mutex prefix used if the application instance must be single per user session.</summary>
        const string LocalPrefix = @"Local\";

        /// <summary>The milliseconds to wait for the prior instance to signal that the startup information have been received.</summary>
        const double PriorInstanceSignaledTimeoutMilliseconds = 2500;

        /// <summary>The milliseconds to wait for the service to be ready.</summary>
        const double ServiceReadyTimeoutMilliseconds = 1000;

        /// <summary>The SID value to be used to retrieve the <c>Users</c> group identity.</summary>
        const string UsersSidValue = "S-1-5-32-545";

        /// <summary>Gets or sets the instance awareness of the application.</summary>
        /// <value>The instance awareness of the application.</value>
        readonly ApplicationInstanceAwareness awareness;

        /// <summary>
        ///   Flag used to determine if the synchronization objects (and the inter-process communication service) have
        ///   been disposed.
        /// </summary>
        bool disposed;

        /// <summary>The synchronization object owned by the first instance.</summary>
        Mutex firstInstanceMutex;

        /// <summary>The host used to communicate between multiple application instances.</summary>
        ServiceHost serviceHost;

        /// <summary>The synchronization object used to synchronize the service creation or destruction.</summary>
        Mutex serviceInitializationMutex;

        /// <summary>The synchronization object used to signal that the service is ready.</summary>
        EventWaitHandle serviceReadySemaphore;

        /// <summary>
        ///   The synchronization object used to signal a subsequent application instance that the first one received
        ///   the notification.
        /// </summary>
        EventWaitHandle signaledToFirstInstanceSemaphore;

        /// <summary>Initializes a new instance of the <see cref="InstanceAwareApplication" /> class.</summary>
        /// <exception cref="InvalidOperationException">More than one instance of the <c>System.Windows.Application</c> class is created per <c>System.AppDomain</c>.</exception>
        public InstanceAwareApplication() : this(ApplicationInstanceAwareness.Host)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="InstanceAwareApplication" /> class.</summary>
        /// <param name="awareness">The instance awareness of the application.</param>
        /// <exception cref="InvalidOperationException">More than one instance of the <c>System.Windows.Application</c> class is created per <c>System.AppDomain</c>.</exception>
        public InstanceAwareApplication(ApplicationInstanceAwareness awareness)
        {
            this.awareness = awareness;
        }

        /// <summary>Finalizes an instance of the InstanceAwareApplication class.</summary>
        ~InstanceAwareApplication()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Occurs when the <c>System.Windows.Application.Run()</c> or <c>System.Windows.Application.Run(Window)</c>
        ///   method of the next <c>InstanceAwareApplication</c> having the same <c>Guid</c> is called.
        /// </summary>
        public event EventHandler<StartupNextInstanceEventArgs> StartupNextInstance;

        /// <summary>Gets a value indicating whether the current application instance is the first one.</summary>
        /// <value><c>True</c> if the current application instance is the first one, otherwise <c>false</c>.</value>
        /// <remarks>The first application instance gets notified about subsequent application instances startup.</remarks>
        public bool IsFirstInstance { get; private set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Signals the startup of the next application instance.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
        void IPriorApplicationInstance.SignalStartupNextInstance(string[] args)
        {
            signaledToFirstInstanceSemaphore.Set();

            ParameterizedThreadStart onStartupNextApplication = obj => OnStartupNextApplicationInstance((string[])obj);

            // Since the method is called asynchronously, invoke the function using the dispatcher!
            Dispatcher.BeginInvoke(onStartupNextApplication, DispatcherPriority.Background, (object)args);
        }

        /// <summary>Raises the <c>System.Windows.Application.Exit</c> event.</summary>
        /// <param name="e">An <c>System.Windows.ExitEventArgs</c> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            // On exit, try to dispose everything related to the synchronization context and the inter-process
            // communication service...
            TryDisposeSynchronizationObjects();
            base.OnExit(e);
        }

        /// <summary>Raises the <c>System.Windows.Application.Startup</c> event.</summary>
        /// <param name="e">A <c>System.Windows.StartupEventArgs</c> that contains the event data.</param>
        protected override sealed void OnStartup(StartupEventArgs e)
        {
            IsFirstInstance = InitializeInstance(e);
            OnStartup(e, IsFirstInstance);
        }

        /// <summary>Raises the <c>System.Windows.Application.Startup</c> event.</summary>
        /// <param name="e">The <c>System.Windows.StartupEventArgs</c> instance containing the event data.</param>
        /// <param name="isFirstInstance">If set to <c>true</c> the current instance is the first application instance.</param>
        protected virtual void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            base.OnStartup(e);
        }

        /// <summary>Raises the <c>InstanceAwareApplication.StartupNextInstance</c> event.</summary>
        /// <param name="e">The <c>StartupNextInstanceEventArgs</c> instance containing the event data.</param>
        protected virtual void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            EventHandler<StartupNextInstanceEventArgs> startupNextInstanceEvent = StartupNextInstance;
            if (startupNextInstanceEvent != null)
            {
                startupNextInstanceEvent(this, e);
            }
        }

        /// <summary>
        ///   Called when the startup of the current application was unsuccessfully signaled to the prior application
        ///   instance.
        /// </summary>
        protected virtual void OnStartupSignaledToPriorApplicationFailed()
        {
        }

        /// <summary>
        ///   Called when the startup of the current application was successfully signaled to the prior application
        ///   instance.
        /// </summary>
        protected virtual void OnStartupSignaledToPriorApplicationSucceeded()
        {
        }

        /// <summary>Extracts some parameters from the specified <c>ApplicationInstanceAwareness</c> value.</summary>
        /// <param name="awareness">The <c>ApplicationInstanceAwareness</c> value to extract parameters from.</param>
        /// <param name="prefix">The synchronization object prefix.</param>
        /// <param name="identity">The identity used to handle the synchronization object.</param>
        static void ExtractParameters(
            ApplicationInstanceAwareness awareness, out string prefix, out IdentityReference identity)
        {
            new SecurityPermission(SecurityPermissionFlag.ControlPrincipal).Assert();
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();

            if (currentIdentity == null)
            {
                throw new Exception("Unable to retrieve current identity");
            }

            if (awareness == ApplicationInstanceAwareness.Host && currentIdentity.Groups != null)
            {
                prefix = GlobalPrefix;
                identity =
                    currentIdentity.Groups.FirstOrDefault(
                        reference => reference.Translate(typeof(SecurityIdentifier)).Value.Equals(UsersSidValue));
            }
            else
            {
                prefix = LocalPrefix;
                identity = currentIdentity.User;
            }

            CodeAccessPermission.RevertAssert();

            if (identity == null)
            {
                throw new Exception(
                    "Could not determine a proper identity to create synchronization objects access rules");
            }
        }

        /// <summary>Gets the <c>Uri</c> of the pipe used for inter-process communication.</summary>
        /// <param name="applicationPath">The application unique path, used to define the <c>Uri</c> pipe.</param>
        /// <returns>The <c>Uri</c> of the pipe used for inter-process communication.</returns>
        static Uri GetPipeUri(string applicationPath)
        {
            return new Uri(string.Format("net.pipe://localhost/{0}/", applicationPath));
        }

        /// <summary>Releases unmanaged and - optionally - managed resources</summary>
        /// <param name="disposing"><c>True</c> to release both managed and unmanaged resources, <c>false</c> to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            // Try to dispose the synchronization objects, just in case the application did not exit...
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.Invoke((Action)TryDisposeSynchronizationObjects);
            }
            else
            {
                TryDisposeSynchronizationObjects();
            }
        }

        /// <summary>Gets the application unique identifier.</summary>
        /// <returns>The application unique identifier.</returns>
        string GetApplicationId()
        {
            // By default, the application is marked using the entry assembly Guid!
            Assembly assembly = Assembly.GetEntryAssembly();
            var guidAttribute =
                assembly.GetCustomAttributes(typeof(GuidAttribute), false).FirstOrDefault(obj => (obj is GuidAttribute))
                as GuidAttribute;

            return guidAttribute != null ? guidAttribute.Value : null;
        }

        /// <summary>Initializes the first application instance.</summary>
        /// <param name="uri">The <c>Uri</c> used by the service that allows for inter-process communication.</param>
        void InitializeFirstInstance(Uri uri)
        {
            try
            {
                // Acquire the mutex used to synchornize service initialization...
                serviceInitializationMutex.WaitOne();

                // Create and start the service...
                serviceHost = new ServiceHost(this, uri);
                serviceHost.AddServiceEndpoint(typeof(IPriorApplicationInstance), new NetNamedPipeBinding(), uri);
                serviceHost.Open();

                // Release the mutex used to synchornize service initialization...
                serviceInitializationMutex.ReleaseMutex();

                // Signal that the service is ready, so that subsequent instances can go on...
                serviceReadySemaphore.Set();
            }
            catch (Exception exc)
            {
                throw new Exception(
                    "First instance failed to create service to communicate with other application instances", exc);
            }
        }

        /// <summary>Initializes the application instance.</summary>
        /// <param name="e">The <c>System.Windows.StartupEventArgs</c> instance containing the event data.</param>
        /// <returns><c>True</c> if the current instance is the first application instance, otherwise <c>false</c>.</returns>
        bool InitializeInstance(StartupEventArgs e)
        {
            string id = GetApplicationId();

            string prefix;
            IdentityReference identity;

            // Extract synchronization objects parameters...
            ExtractParameters(awareness, out prefix, out identity);

            // Initialize synchornization objects...
            InitializeSynchronizationObjects(prefix + id, identity);

            // The mutex is acquired by the first instance, and never released until first application shutdown!
            bool isFirstInstance;

            try
            {
                isFirstInstance = firstInstanceMutex.WaitOne(
                    TimeSpan.FromMilliseconds(FirstInstanceTimeoutMilliseconds));
            }
            catch (AbandonedMutexException)
            {
                Debug.WriteLine(
                    "The previous application was probably killed, we can just handle the exception since in this case is not fatal!");
                isFirstInstance = true;
            }

            Uri uri = GetPipeUri(id + "/" + identity);
            if (isFirstInstance)
            {
                InitializeFirstInstance(uri);
            }
            else
            {
                if (InitializeNextInstance(uri, e.Args))
                {
                    OnStartupSignaledToPriorApplicationSucceeded();
                }
                else
                {
                    OnStartupSignaledToPriorApplicationFailed();
                }
            }

            return isFirstInstance;
        }

        /// <summary>Initializes the next application instance.</summary>
        /// <param name="uri">The <c>Uri</c> used by the service that allows for inter-process communication.</param>
        /// <param name="args">The arguments passed to the current instance.</param>
        /// <returns><c>True</c> if the prior instance was notified about curernt instance startup, otherwise <c>false</c>.</returns>
        bool InitializeNextInstance(Uri uri, string[] args)
        {
            // Check if the service is up... wait a bit in case two applications are started simultaneously...
            if (!serviceReadySemaphore.WaitOne(TimeSpan.FromMilliseconds(ServiceReadyTimeoutMilliseconds), false))
            {
                return false;
            }

            try
            {
                IPriorApplicationInstance instance =
                    ChannelFactory<IPriorApplicationInstance>.CreateChannel(
                        new NetNamedPipeBinding(), new EndpointAddress(uri));
                instance.SignalStartupNextInstance(args);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(
                    "Exception while signaling first application instance (signal while first application shutdown?)"
                    + Environment.NewLine + exc, 
                    GetType().ToString());
                return false;
            }

            // If the first application does not notify back that the signal has been received, just return false...
            return
                signaledToFirstInstanceSemaphore.WaitOne(
                    TimeSpan.FromMilliseconds(PriorInstanceSignaledTimeoutMilliseconds), false);
        }

        /// <summary>Initializes the synchronization objects needed to deal with multiple instances of the same application.</summary>
        /// <param name="baseName">The base name of the synchronization objects.</param>
        /// <param name="identity">The identity to be associated to the synchronization objects.</param>
        void InitializeSynchronizationObjects(string baseName, IdentityReference identity)
        {
            string firstInstanceMutexName = baseName + "_FirstInstance";
            string serviceInitializationMutexName = baseName + "_ServiceInitialization";
            string serviceReadySemaphoreName = baseName + "_ServiceReady";
            string signaledToFirstInstanceSemaphoreName = baseName + "_SignaledToFirstInstance";

            bool isNew;
            var eventRule = new EventWaitHandleAccessRule(
                identity, EventWaitHandleRights.FullControl, AccessControlType.Allow);
            var eventSecurity = new EventWaitHandleSecurity();
            eventSecurity.AddAccessRule(eventRule);

            var mutexRule = new MutexAccessRule(identity, MutexRights.FullControl, AccessControlType.Allow);
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(mutexRule);

            firstInstanceMutex = new Mutex(false, firstInstanceMutexName, out isNew, mutexSecurity);
            serviceInitializationMutex = new Mutex(false, serviceInitializationMutexName, out isNew, mutexSecurity);
            serviceReadySemaphore = new EventWaitHandle(
                false, EventResetMode.ManualReset, serviceReadySemaphoreName, out isNew, eventSecurity);
            signaledToFirstInstanceSemaphore = new EventWaitHandle(
                false, EventResetMode.AutoReset, signaledToFirstInstanceSemaphoreName, out isNew, eventSecurity);
        }

        /// <summary>Called on next application instance startup.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
        void OnStartupNextApplicationInstance(string[] args)
        {
            var e = new StartupNextInstanceEventArgs(args);
            OnStartupNextInstance(e);

            if (!e.BringToForeground || (MainWindow == null))
            {
                return;
            }

            (new UIPermission(UIPermissionWindow.AllWindows)).Assert();
            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.WindowState = WindowState.Normal;
            }

            MainWindow.Activate();
            CodeAccessPermission.RevertAssert();
        }

        /// <summary>Tries the dispose synchronization objects (if needed).</summary>
        void TryDisposeSynchronizationObjects()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            if (IsFirstInstance)
            {
                // Signal other applications that the service is not ready anymore (it is, but since the application is
                // going to shutdown, it is the same...)
                serviceReadySemaphore.Reset();

                // Stop the service...
                serviceInitializationMutex.WaitOne();

                if (serviceHost.State == CommunicationState.Opened)
                {
                    serviceHost.Close(TimeSpan.Zero); // Shut down the service without waiting!
                }

                serviceHost = null;

                serviceInitializationMutex.ReleaseMutex();

                // Release the first application mutex!
                firstInstanceMutex.ReleaseMutex();
            }

            firstInstanceMutex.Close();
            firstInstanceMutex = null;

            serviceInitializationMutex.Close();
            serviceInitializationMutex = null;

            serviceReadySemaphore.Close();
            serviceReadySemaphore = null;

            signaledToFirstInstanceSemaphore.Close();
            signaledToFirstInstanceSemaphore = null;
        }
    }
}