// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Threading;
    using System.Windows.Threading;

    /// <summary>
    /// Interface used to signal a prior instance of the application about the startup another instance.
    /// </summary>
    [ServiceContract]
    internal interface IPriorApplicationInstance
    {
        #region Public Methods

        /// <summary>
        /// Signals the startup of the next application instance.
        /// </summary>
        /// <parameter name="args">
        /// The parameters used to run the next instance of the application.
        /// </parameter>
        [OperationContract]
        void SignalStartupNextInstance(string[] args);

        #endregion
    }

    /// <summary>
    /// Class used to define a WPF application which is aware of subsequent application instances running, either locally (per session) or globally (per host).
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class InstanceAwareApplication : Application, IPriorApplicationInstance, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The milliseconds to wait to determine if the current instance is the first one.
        /// </summary>
        private const double FirstInstanceTimeoutMilliseconds = 500;

        /// <summary>
        ///   The mutex prefix used if the application instance must be single per machine.
        /// </summary>
        private const string GlobalPrefix = @"Global\";

        /// <summary>
        ///   The mutex prefix used if the application instance must be single per user session.
        /// </summary>
        private const string LocalPrefix = @"Local\";

        /// <summary>
        ///   The milliseconds to wait for the prior instance to signal that the startup information have been received.
        /// </summary>
        private const double PriorInstanceSignaledTimeoutMilliseconds = 2500;

        /// <summary>
        ///   The milliseconds to wait for the service to be ready.
        /// </summary>
        private const double ServiceReadyTimeoutMilliseconds = 1000;

        /// <summary>
        ///   The SID value to be used to retrieve the <c>Users</c> group identity.
        /// </summary>
        private const string UsersSidValue = "S-1-5-32-545";

        /// <summary>
        ///   The application instance awareness.
        /// </summary>
        private readonly ApplicationInstanceAwareness awareness;

        /// <summary>
        ///   Flag used to determine if the synchronization objects (and the inter-process communication service) have been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        ///   The synchronization object owned by the first instance.
        /// </summary>
        private Mutex firstInstanceMutex;

        /// <summary>
        ///   Flag used to determine if the current application instance is the first one.
        /// </summary>
        private bool isFirstInstance;

        /// <summary>
        ///   The host used to communicate between multiple application instances.
        /// </summary>
        private ServiceHost serviceHost;

        /// <summary>
        ///   The synchronization object used to synchronize the service creation or destruction.
        /// </summary>
        private Mutex serviceInitializationMutex;

        /// <summary>
        ///   The synchronization object used to signal that the service is ready.
        /// </summary>
        private EventWaitHandle serviceReadySemaphore;

        /// <summary>
        ///   The synchronization object used to signal a subsequent application instance that the first one received the notification.
        /// </summary>
        private EventWaitHandle signaledToFirstInstanceSemaphore;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "InstanceAwareApplication" /> class.
        /// </summary>
        /// <exception cref = "T:System.InvalidOperationException">More than one instance of the <see cref = "T:System.Windows.Application" /> class is created per <see cref = "T:System.AppDomain" />.</exception>
        public InstanceAwareApplication()
            : this(ApplicationInstanceAwareness.Host)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceAwareApplication"/> class.
        /// </summary>
        /// <parameter name="awareness">
        /// The instance awareness of the application.
        /// </parameter>
        /// <exception cref="T:System.InvalidOperationException">
        /// More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.
        /// </exception>
        public InstanceAwareApplication(ApplicationInstanceAwareness awareness)
        {
            this.awareness = awareness;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="InstanceAwareApplication"/> class.
        /// </summary>
        ~InstanceAwareApplication()
        {
            this.Dispose(false);
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the <see cref = "M:System.Windows.Application.Run" /> method of the next <see cref = "InstanceAwareApplication" /> having the same <see cref = "InstanceAwareApplication.ApplicationKey" /> is called.
        /// </summary>
        public event StartupNextInstanceEventHandler StartupNextInstance;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the instance awareness of the application.
        /// </summary>
        /// <value>The instance awareness of the application.</value>
        public ApplicationInstanceAwareness Awareness
        {
            get
            {
                return this.awareness;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the current application instance is the first one.
        /// </summary>
        /// <value>
        ///   <c>True</c> if the current application instance is the first one, otherwise <c>false</c>.
        /// </value>
        /// <remarks>
        ///   The first application instance gets notified about subsequent application instances startup.
        /// </remarks>
        public bool IsFirstInstance
        {
            get
            {
                return this.isFirstInstance;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IPriorApplicationInstance

        /// <summary>
        /// </summary>
        /// <parameter name="args">
        /// </parameter>
        void IPriorApplicationInstance.SignalStartupNextInstance(string[] args)
        {
            this.signaledToFirstInstanceSemaphore.Set();

            ParameterizedThreadStart onStartupNextApplication = obj => this.OnStartupNextApplicationInstance((string[])obj);

            // Since the method is called asynchronously, invoke the function using the dispatcher!
            this.Dispatcher.BeginInvoke(onStartupNextApplication, DispatcherPriority.Background, (object)args);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <parameter name="disposing">
        /// <c>True</c> to release both managed and unmanaged resources, <c>false</c> to release only unmanaged resources.
        /// </parameter>
        protected virtual void Dispose(bool disposing)
        {
            // Try to dispose the synchronization objects, just in case the application did not exit...
            this.TryDisposeSynchronizationObjects();
        }

        /// <summary>
        /// Called when the the application <see cref="Guid"/> has to be generated.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/> used to identify the application.
        /// </returns>
        /// <remarks>
        /// <para>
        /// If the entry assembly is decorated with a <see cref="System.Runtime.InteropServices.GuidAttribute"/>, this function is ignored.
        ///   </para>
        /// <para>
        /// Special care must be taken when overriding this method.
        ///     <para>
        /// First of all, <c>do not call the base implementation</c>, since it just throws an <see cref="UndefinedApplicationGuidException"/> to inform the developer that something is missing.
        ///     </para>
        /// <para>
        /// Moreover, the method must return a <see cref="Guid"/> value which is <c>constant</c>, since it is used to mark the application.
        ///     </para>
        /// <para>
        /// The encouraged approach to mark an application, is marking the entry assembly with a proper <see cref="T:System.Runtime.InteropServices.GuidAttribute"/>; this method should be used only if such method is impractical or not possible.
        ///     </para>
        /// </para>
        /// </remarks>
        /// <exception cref="UndefinedApplicationGuidException">
        /// If the function has not been properly overridden or the base implementation has been invoked in a <see cref="InstanceAwareApplication"/> derived class.
        /// </exception>
        protected virtual Guid GenerateApplicationGuid()
        {
            throw new UndefinedApplicationGuidException(
                "No application Guid has been defined, either specify a Guid attribute on the entry assembly (executable) or override the GenerateApplicationGuid method in the " +
                this.GetType() + " class");
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <parameter name="e">
        /// An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.
        /// </parameter>
        protected override sealed void OnExit(ExitEventArgs e)
        {
            // On exit, try to dispose everything related to the synchronization context and the inter-process communication service...
            this.TryDisposeSynchronizationObjects();
            this.OnExit(e, this.isFirstInstance);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <parameter name="e">
        /// An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.
        /// </parameter>
        /// <parameter name="isFirstInstance">
        /// If set to <c>true</c>, the current application instance is the first one.
        /// </parameter>
        protected virtual void OnExit(ExitEventArgs e, bool isFirstInstance)
        {
            base.OnExit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <parameter name="e">
        /// A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.
        /// </parameter>
        protected override sealed void OnStartup(StartupEventArgs e)
        {
            this.isFirstInstance = this.InitializeInstance(e);
            this.OnStartup(e, this.isFirstInstance);
        }

        /// <summary>
        /// Raises the <see cref="Application.Startup"/> event.
        /// </summary>
        /// <parameter name="e">
        /// The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.
        /// </parameter>
        /// <parameter name="isFirstInstance">
        /// If set to <c>true</c> the current instance is the first application instance.
        /// </parameter>
        protected virtual void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            base.OnStartup(e);
        }

        /// <summary>
        /// Raises the <see cref="StartupNextInstance"/> event.
        /// </summary>
        /// <parameter name="e">
        /// The <see cref="StartupNextInstanceEventArgs"/> instance containing the event data.
        /// </parameter>
        protected virtual void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            var startupNextInstanceEvent = this.StartupNextInstance;
            if (startupNextInstanceEvent != null)
            {
                startupNextInstanceEvent(this, e);
            }
        }

        /// <summary>
        /// Called when the startup of the current application was unsuccessfully signaled to the prior application instance.
        /// </summary>
        protected virtual void OnStartupSignaledToPriorApplicationFailed()
        {
        }

        /// <summary>
        /// Called when the startup of the current application was successfully signaled to the prior application instance.
        /// </summary>
        protected virtual void OnStartupSignaledToPriorApplicationSucceeded()
        {
        }

        /// <summary>
        /// Signals the startup of the next application instance.
        /// </summary>
        /// <parameter name="args">
        /// The parameters used to run the next instance of the application.
        /// </parameter>
        protected virtual void SignalStartupNextInstance(string[] args)
        {
            ((IPriorApplicationInstance)this).SignalStartupNextInstance(args);
        }

        /// <summary>
        /// Extracts some parameters from the specified <see cref="ApplicationInstanceAwareness"/> value.
        /// </summary>
        /// <parameter name="awareness">
        /// The <see cref="ApplicationInstanceAwareness"/> value to extract parameters from.
        /// </parameter>
        /// <parameter name="prefix">
        /// The synchronization object prefix.
        /// </parameter>
        /// <parameter name="identity">
        /// The identity used to handle the synchronization object.
        /// </parameter>
        /// <exception cref="UnexpectedInstanceAwareApplicationException">
        /// A proper identity could not be determined.
        /// </exception>
        private static void ExtractParameters(ApplicationInstanceAwareness awareness, out string prefix, out IdentityReference identity)
        {
            new SecurityPermission(SecurityPermissionFlag.ControlPrincipal).Assert();
            if (awareness == ApplicationInstanceAwareness.Host)
            {
                prefix = GlobalPrefix;

                // ReSharper disable PossibleNullReferenceException
                identity = WindowsIdentity.GetCurrent().Groups.FirstOrDefault(reference => reference.Translate(typeof(SecurityIdentifier)).Value.Equals(UsersSidValue));
            }
            else
            {
                prefix = LocalPrefix;
                identity = WindowsIdentity.GetCurrent().User;

                // ReSharper restore PossibleNullReferenceException
            }

            CodeAccessPermission.RevertAssert();

            if (identity == null)
            {
                throw new UnexpectedInstanceAwareApplicationException("Could not determine a proper identity to create synchronization objects access rules");
            }
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the pipe used for inter-process communication.
        /// </summary>
        /// <parameter name="applicationPath">
        /// The application unique path, used to define the <see cref="Uri"/> pipe.
        /// </parameter>
        /// <returns>
        /// The <see cref="Uri"/> of the pipe used for inter-process communication.
        /// </returns>
        private static Uri GetPipeUri(string applicationPath)
        {
            return new Uri(string.Format(CultureInfo.CurrentCulture, @"net.pipe://localhost/{0}/", applicationPath));
        }

        /// <summary>
        /// Gets the application unique identifier.
        /// </summary>
        /// <returns>
        /// The application unique identifier.
        /// </returns>
        private string GetApplicationId()
        {
            // By default, the application is marked using the entry assembly Guid!
            var assembly = Assembly.GetEntryAssembly();
            var guidAttribute = assembly.GetCustomAttributes(typeof(GuidAttribute), false).FirstOrDefault(obj => (obj is GuidAttribute)) as GuidAttribute;
            return guidAttribute != null ? guidAttribute.Value : this.GenerateApplicationGuid().ToString();
        }

        /// <summary>
        /// Initializes the first application instance.
        /// </summary>
        /// <parameter name="uri">
        /// The <see cref="Uri"/> used by the service that allows for inter-process communication.
        /// </parameter>
        private void InitializeFirstInstance(Uri uri)
        {
            try
            {
                // Acquire the mutex used to synchronize service initialization...
                this.serviceInitializationMutex.WaitOne();

                // Create and start the service...
                this.serviceHost = new ServiceHost(this, uri);
                this.serviceHost.AddServiceEndpoint(typeof(IPriorApplicationInstance), new NetNamedPipeBinding(), uri);
                this.serviceHost.Open();

                // Release the mutex used to synchronize service initialization...
                this.serviceInitializationMutex.ReleaseMutex();

                // Signal that the service is ready, so that subsequent instances can go on...
                this.serviceReadySemaphore.Set();
            }
            catch (Exception exc)
            {
                throw new UnexpectedInstanceAwareApplicationException("First instance failed to create service to communicate with other application instances", exc);
            }
        }

        /// <summary>
        /// Initializes the application instance.
        /// </summary>
        /// <parameter name="e">
        /// The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.
        /// </parameter>
        /// <returns>
        /// <c>True</c> if the current instance is the first application instance, otherwise <c>false</c>.
        /// </returns>
        private bool InitializeInstance(StartupEventArgs e)
        {
            var id = this.GetApplicationId();

            string prefix;
            IdentityReference identity;

            // Extract synchronization objects parameters...
            ExtractParameters(this.awareness, out prefix, out identity);

            // Initialize synchronization objects...
            this.InitializeSynchronizationObjects(prefix + id, identity);

            // The mutex is acquired by the first instance, and never released until first application shutdown!
            bool isFirstInstance;

            try
            {
                isFirstInstance = this.firstInstanceMutex.WaitOne(TimeSpan.FromMilliseconds(FirstInstanceTimeoutMilliseconds));
            }
            catch (AbandonedMutexException)
            {
                isFirstInstance = true;
            }

            var uri = GetPipeUri(id + "/" + identity);
            if (isFirstInstance)
            {
                this.InitializeFirstInstance(uri);
            }
            else
            {
                if (this.InitializeNextInstance(uri, e.Args))
                {
                    this.OnStartupSignaledToPriorApplicationSucceeded();
                }
                else
                {
                    this.OnStartupSignaledToPriorApplicationFailed();
                }
            }

            return isFirstInstance;
        }

        /// <summary>
        /// Initializes the next application instance.
        /// </summary>
        /// <parameter name="uri">
        /// The <see cref="Uri"/> used by the service that allows for inter-process communication.
        /// </parameter>
        /// <parameter name="args">
        /// The arguments passed to the current instance.
        /// </parameter>
        /// <returns>
        /// <c>True</c> if the prior instance was notified about current instance startup, otherwise <c>false</c>.
        /// </returns>
        private bool InitializeNextInstance(Uri uri, string[] args)
        {
            // Check if the service is up... wait a bit in case two applications are started simultaneously...
            if (!this.serviceReadySemaphore.WaitOne(TimeSpan.FromMilliseconds(ServiceReadyTimeoutMilliseconds), false))
            {
                return false;
            }

            try
            {
                var instance = ChannelFactory<IPriorApplicationInstance>.CreateChannel(new NetNamedPipeBinding(), new EndpointAddress(uri));
                instance.SignalStartupNextInstance(args);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(
                    "Exception while signaling first application instance (signal while first application shutdown?)" + Environment.NewLine + exc, 
                    this.GetType().ToString());
                return false;
            }

            // If the first application does not notify back that the signal has been received, just return false...
            if (!this.signaledToFirstInstanceSemaphore.WaitOne(TimeSpan.FromMilliseconds(PriorInstanceSignaledTimeoutMilliseconds), false))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes the synchronization objects needed to deal with multiple instances of the same application.
        /// </summary>
        /// <parameter name="baseName">
        /// The base name of the synchronization objects.
        /// </parameter>
        /// <parameter name="identity">
        /// The identity to be associated to the synchronization objects.
        /// </parameter>
        private void InitializeSynchronizationObjects(string baseName, IdentityReference identity)
        {
            var firstInstanceMutexName = baseName + "_FirstInstance";
            var serviceInitializationMutexName = baseName + "_ServiceInitialization";
            var serviceReadySemaphoreName = baseName + "_ServiceReady";
            var signaledToFirstInstanceSemaphoreName = baseName + "_SignaledToFirstInstance";

            bool isNew;
            var eventRule = new EventWaitHandleAccessRule(identity, EventWaitHandleRights.FullControl, AccessControlType.Allow);
            var eventSecurity = new EventWaitHandleSecurity();
            eventSecurity.AddAccessRule(eventRule);

            var mutexRule = new MutexAccessRule(identity, MutexRights.FullControl, AccessControlType.Allow);
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(mutexRule);

            this.firstInstanceMutex = new Mutex(false, firstInstanceMutexName, out isNew, mutexSecurity);
            this.serviceInitializationMutex = new Mutex(false, serviceInitializationMutexName, out isNew, mutexSecurity);
            this.serviceReadySemaphore = new EventWaitHandle(false, EventResetMode.ManualReset, serviceReadySemaphoreName, out isNew, eventSecurity);
            this.signaledToFirstInstanceSemaphore = new EventWaitHandle(false, EventResetMode.AutoReset, signaledToFirstInstanceSemaphoreName, out isNew, eventSecurity);
        }

        /// <summary>
        /// Called on next application instance startup.
        /// </summary>
        /// <parameter name="args">
        /// The parameters used to run the next instance of the application.
        /// </parameter>
        private void OnStartupNextApplicationInstance(string[] args)
        {
            var e = new StartupNextInstanceEventArgs(args, true);
            this.OnStartupNextInstance(e);

            if (e.BringToForeground && (this.MainWindow != null))
            {
                (new UIPermission(UIPermissionWindow.AllWindows)).Assert();
                if (this.MainWindow.WindowState == WindowState.Minimized)
                {
                    this.MainWindow.WindowState = WindowState.Normal;
                }

                this.MainWindow.Activate();
                CodeAccessPermission.RevertAssert();
            }
        }

        /// <summary>
        /// Tries the dispose synchronization objects (if needed).
        /// </summary>
        private void TryDisposeSynchronizationObjects()
        {
            if (!this.disposed)
            {
                this.disposed = true;

                if (this.isFirstInstance)
                {
                    // Signal other applications that the service is not ready anymore (it is, but since the application is going to shutdown, it is the same...)
                    this.serviceReadySemaphore.Reset();

                    // Stop the service...
                    this.serviceInitializationMutex.WaitOne();

                    try
                    {
                        if (this.serviceHost.State == CommunicationState.Opened)
                        {
                            this.serviceHost.Close(TimeSpan.Zero); // Shut down the service without waiting!
                        }
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Exception raised while closing service" + Environment.NewLine + exc, this.GetType().ToString());
                    }
                    finally
                    {
                        this.serviceHost = null;
                    }

                    this.serviceInitializationMutex.ReleaseMutex();
                    try
                    {
                        // Release the first application mutex!
                        this.firstInstanceMutex.ReleaseMutex();
                    }
                    catch (Exception)
                    {
                    }
                }

                this.firstInstanceMutex.Close();
                this.firstInstanceMutex = null;

                this.serviceInitializationMutex.Close();
                this.serviceInitializationMutex = null;

                this.serviceReadySemaphore.Close();
                this.serviceReadySemaphore = null;

                this.signaledToFirstInstanceSemaphore.Close();
                this.signaledToFirstInstanceSemaphore = null;
            }
        }

        #endregion
    }
}