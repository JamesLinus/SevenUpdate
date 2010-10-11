// ***********************************************************************
// <copyright file="InstanceAwareApplication.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) BladeWise. All rights reserved.
// </copyright>
// <author username="BladeWise">BladeWise</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://wpfinstanceawareapp.codeplex.com/license">Microsoft Public License</license>
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

    /// <summary>Enumerator used to define the awareness of an application, when dealing with subsequent instances of the application itself.</summary>
    public enum ApplicationInstanceAwareness : byte
    {
        /// <summary>The awareness is global, meaning that the first application instance is aware of any other instances running on the host.</summary>
        Host = 0x00, 

        /// <summary>The awareness is local, meaning that the first application instance is aware only of other instances running in the current user session.</summary>
        UserSession = 0x01
    }

    /// <summary>Interface used to signal a prior instance of the application about the startup another instance.</summary>
    [ServiceContract]
    internal interface IPriorApplicationInstance
    {
        #region Public Methods

        /// <summary>Signals the startup of the next application instance.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
        [OperationContract]
        void SignalStartupNextInstance(string[] args);

        #endregion
    }

    /// <summary>Class used to define a WPF application which is aware of subsequent application instances running, either locally (per session) or globally (per host).</summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class InstanceAwareApplication : Application, IPriorApplicationInstance, IDisposable
    {
        #region Constants and Fields

        /// <summary>The milliseconds to wait to determine if the current instance is the first one.</summary>
        private const double FirstInstanceTimeoutMilliseconds = 500;

        /// <summary>The mutex prefix used if the application instance must be single per machine.</summary>
        private const string GlobalPrefix = @"Global\";

        /// <summary>The mutex prefix used if the application instance must be single per user session.</summary>
        private const string LocalPrefix = @"Local\";

        /// <summary>The milliseconds to wait for the prior instance to signal that the startup information have been received.</summary>
        private const double PriorInstanceSignaledTimeoutMilliseconds = 2500;

        /// <summary>The milliseconds to wait for the service to be ready.</summary>
        private const double ServiceReadyTimeoutMilliseconds = 1000;

        /// <summary>The SID value to be used to retrieve the <c>Users</c> group identity.</summary>
        private const string UsersSidValue = "S-1-5-32-545";

        /// <summary>The application instance awareness.</summary>
        private readonly ApplicationInstanceAwareness awareness;

        /// <summary>Flag used to determine if the synchronization objects (and the inter-process communication service) have been disposed.</summary>
        private bool disposed;

        /// <summary>The synchronization object owned by the first instance.</summary>
        private Mutex firstInstanceMutex;

        /// <summary>The host used to communicate between multiple application instances.</summary>
        private ServiceHost serviceHost;

        /// <summary>The synchronization object used to synchronize the service creation or destruction.</summary>
        private Mutex serviceInitializationMutex;

        /// <summary>The synchronization object used to signal that the service is ready.</summary>
        private EventWaitHandle serviceReadySemaphore;

        /// <summary>The synchronization object used to signal a subsequent application instance that the first one received the notification.</summary>
        private EventWaitHandle signaledToFirstInstanceSemaphore;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="InstanceAwareApplication"/> class.</summary>
        /// <param name="awareness">The instance awareness of the application.</param>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public InstanceAwareApplication(ApplicationInstanceAwareness awareness)
        {
            this.awareness = awareness;
        }

        /// <summary>Initializes a new instance of the <see cref = "InstanceAwareApplication" /> class.</summary>
        /// <exception cref = "T:System.InvalidOperationException">More than one instance of the <see cref = "T:System.Windows.Application" /> class is created per <see cref = "T:System.AppDomain" />.</exception>
        protected InstanceAwareApplication() : this(ApplicationInstanceAwareness.Host)
        {
        }

        /// <summary>Finalizes an instance of the <see cref="InstanceAwareApplication"/> class.</summary>
        ~InstanceAwareApplication()
        {
            this.Dispose();
        }

        #endregion

        #region Delegates

        /// <summary>Represents the method that handles the <see cref="InstanceAwareApplication.StartupNextInstance"/> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        public delegate void StartupNextInstanceEventHandler(object sender, StartupNextInstanceEventArgs e);

        #endregion

        #region Events

        /// <summary>Occurs when the Application.Run method of the next <see cref = "InstanceAwareApplication" /> having the same ApplicationKey is called.</summary>
        public event StartupNextInstanceEventHandler StartupNextInstance;

        #endregion

        #region Properties

        /// <summary>Gets the instance awareness of the application.</summary>
        /// <value>The instance awareness of the application.</value>
        public ApplicationInstanceAwareness Awareness
        {
            get
            {
                return this.awareness;
            }
        }

        /// <summary>Gets a value indicating whether the current application instance is the first one.</summary>
        /// <value><see langword = "true" /> if the current application instance is the first one, otherwise <see langword = "false" />.</value>
        /// <remarks>The first application instance gets notified about subsequent application instances startup.</remarks>
        public bool IsFirstInstance { get; private set; }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        void IDisposable.Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IPriorApplicationInstance

        /// <summary>Signals the startup of the next application instance.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
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

        /// <summary>Raises the <see cref="E:System.Windows.Application.Exit"/> event.</summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.</param>
        protected override sealed void OnExit(ExitEventArgs e)
        {
            // On exit, try to dispose everything related to the synchronization context and the inter-process communication service...
            this.TryDisposeSynchronizationObjects();
            this.OnExit(e, this.IsFirstInstance);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Application.Exit"/> event.</summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.</param>
        /// <param name="firstInstance">If set to <see langword = "true" />, the current application instance is the first one.</param>
        protected virtual void OnExit(ExitEventArgs e, bool firstInstance)
        {
            base.OnExit(e);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup"/> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override sealed void OnStartup(StartupEventArgs e)
        {
            this.IsFirstInstance = this.InitializeInstance(e);
            this.OnStartup(e, this.IsFirstInstance);
        }

        /// <summary>Raises the <see cref="Application.Startup"/> event.</summary>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        /// <param name="firstInstance">If set to <see langword = "true" /> the current instance is the first application instance.</param>
        protected virtual void OnStartup(StartupEventArgs e, bool firstInstance)
        {
            base.OnStartup(e);
        }

        /// <summary>Raises the <see cref="StartupNextInstance"/> event.</summary>
        /// <param name="e">The <see cref="StartupNextInstanceEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            var startupNextInstanceEvent = this.StartupNextInstance;
            if (startupNextInstanceEvent != null)
            {
                startupNextInstanceEvent(this, e);
            }
        }

        /// <summary>Called when the startup of the current application was unsuccessfully signaled to the prior application instance.</summary>
        protected virtual void OnStartupSignaledToPriorApplicationFailed()
        {
        }

        /// <summary>Called when the startup of the current application was successfully signaled to the prior application instance.</summary>
        protected virtual void OnStartupSignaledToPriorApplicationSucceeded()
        {
        }

        /// <summary>Signals the startup of the next application instance.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
        protected virtual void SignalStartupNextInstance(string[] args)
        {
            ((IPriorApplicationInstance)this).SignalStartupNextInstance(args);
        }

        /// <summary>Extracts some parameters from the specified <see cref="ApplicationInstanceAwareness"/> value.</summary>
        /// <param name="awareness">The <see cref="ApplicationInstanceAwareness"/> value to extract parameters from.</param>
        /// <param name="prefix">The synchronization object prefix.</param>
        /// <param name="identity">The identity used to handle the synchronization object.</param>
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
            }
        }

        /// <summary>Gets the application unique identifier.</summary>
        /// <returns>The application unique identifier.</returns>
        private static string GetApplicationId()
        {
            // By default, the application is marked using the entry assembly Guid!
            var assembly = Assembly.GetEntryAssembly();
            var guidAttribute = assembly.GetCustomAttributes(typeof(GuidAttribute), false).FirstOrDefault(obj => (obj is GuidAttribute)) as GuidAttribute;
            return guidAttribute.Value;
        }

        /// <summary>Gets the <see cref="Uri"/> of the pipe used for inter-process communication.</summary>
        /// <param name="applicationPath">The application unique path, used to define the <see cref="Uri"/> pipe.</param>
        /// <returns>The <see cref="Uri"/> of the pipe used for inter-process communication.</returns>
        private static Uri GetPipeUri(string applicationPath)
        {
            return new Uri(string.Format(CultureInfo.CurrentCulture, @"net.pipe://localhost/{0}/", applicationPath));
        }

        /// <summary>Releases unmanaged and - optionally - managed resources</summary>
        private void Dispose()
        {
            // Try to dispose the synchronization objects, just in case the application did not exit...
            this.TryDisposeSynchronizationObjects();
        }

        /// <summary>Initializes the first application instance.</summary>
        /// <param name="uri">The <see cref="Uri"/> used by the service that allows for inter-process communication.</param>
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
            catch (Exception)
            {
            }
        }

        /// <summary>Initializes the application instance.</summary>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        /// <returns><see langword = "true" /> if the current instance is the first application instance, otherwise <see langword = "false" />.</returns>
        private bool InitializeInstance(StartupEventArgs e)
        {
            var id = GetApplicationId();

            string prefix;
            IdentityReference identity;

            // Extract synchronization objects parameters...
            ExtractParameters(this.awareness, out prefix, out identity);

            // Initialize synchronization objects...
            this.InitializeSynchronizationObjects(prefix + id, identity);

            // The mutex is acquired by the first instance, and never released until first application shutdown!
            bool firstInstance;

            try
            {
                firstInstance = this.firstInstanceMutex.WaitOne(TimeSpan.FromMilliseconds(FirstInstanceTimeoutMilliseconds));
            }
            catch (AbandonedMutexException)
            {
                firstInstance = true;
            }

            var uri = GetPipeUri(id + "/" + identity);
            if (firstInstance)
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

            return firstInstance;
        }

        /// <summary>Initializes the next application instance.</summary>
        /// <param name="uri">The <see cref="Uri"/> used by the service that allows for inter-process communication.</param>
        /// <param name="args">The arguments passed to the current instance.</param>
        /// <returns><see langword = "true" /> if the prior instance was notified about current instance startup, otherwise <see langword = "false" />.</returns>
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
            return this.signaledToFirstInstanceSemaphore.WaitOne(TimeSpan.FromMilliseconds(PriorInstanceSignaledTimeoutMilliseconds), false);
        }

        /// <summary>Initializes the synchronization objects needed to deal with multiple instances of the same application.</summary>
        /// <param name="baseName">The base name of the synchronization objects.</param>
        /// <param name="identity">The identity to be associated to the synchronization objects.</param>
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

        /// <summary>Called on next application instance startup.</summary>
        /// <param name="args">The parameters used to run the next instance of the application.</param>
        private void OnStartupNextApplicationInstance(string[] args)
        {
            var e = new StartupNextInstanceEventArgs(args, true);
            this.OnStartupNextInstance(e);

            if (!e.BringToForeground || (this.MainWindow == null))
            {
                return;
            }

            (new UIPermission(UIPermissionWindow.AllWindows)).Assert();
            if (this.MainWindow.WindowState == WindowState.Minimized)
            {
                this.MainWindow.WindowState = WindowState.Normal;
            }

            this.MainWindow.Activate();
            CodeAccessPermission.RevertAssert();
        }

        /// <summary>Tries the dispose synchronization objects (if needed).</summary>
        private void TryDisposeSynchronizationObjects()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;

            if (this.IsFirstInstance)
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

        #endregion

        /// <summary>Class used to define the arguments of another application instance startup.</summary>
        public class StartupNextInstanceEventArgs : EventArgs
        {
            #region Constants and Fields

            /// <summary>The application arguments.</summary>
            private readonly string[] args;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="StartupNextInstanceEventArgs"/> class.</summary>
            /// <param name="args">The arguments passed to the program</param>
            public StartupNextInstanceEventArgs(string[] args) : this(args, true)
            {
            }

            /// <summary>Initializes a new instance of the <see cref="StartupNextInstanceEventArgs"/> class.</summary>
            /// <param name="args">The arguments passed to the program</param>
            /// <param name="bringToFront">If set to <see langword = "true" /> the application main window will be brought to front.</param>
            public StartupNextInstanceEventArgs(string[] args, bool bringToFront)
            {
                if (args == null)
                {
                    args = new string[0];
                }

                this.args = args;
                this.BringToForeground = bringToFront;
            }

            #endregion

            #region Properties

            /// <summary>Gets a value indicating whether the application main window has to be brought to foreground.</summary>
            /// <value><see langword = "true" /> if the application window has to be brought to foreground, otherwise <see langword = "false" /></value>
            public bool BringToForeground { get; private set; }

            #endregion

            #region Public Methods

            /// <summary>Gets the arguments passed to the other application.</summary>
            /// <returns>Returns the arguments passed to the application</returns>
            /// <value>The arguments passed to the other application.</value>
            public string[] GetArgs()
            {
                return this.args;
            }

            #endregion
        }
    }
}