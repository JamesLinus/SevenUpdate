// <copyright file="WeakCultureChangedEventManager.cs" project="WPFLocalizeExtension">Bernhard Millauer</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace WPFLocalizeExtension.Engine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    /// <summary>This in line class is used to handle weak events to avoid memory leaks.</summary>
    internal sealed class WeakCultureChangedEventManager : WeakEventManager
    {
        /// <summary>Holds the inner list of listeners.</summary>
        private readonly ListenerList listeners;

        /// <summary>Indicates, if the current instance is listening on the source event.</summary>
        private bool isListening;

        /// <summary>Prevents a default instance of the WeakCultureChangedEventManager class from being created.</summary>
        private WeakCultureChangedEventManager()
        {
            // creates a new list and assign it to listeners
            this.listeners = new ListenerList();
        }

        /// <summary>Gets the singleton instance of <c>WeakCultureChangedEventManager</c>.</summary>
        private static WeakCultureChangedEventManager CurrentManager
        {
            get
            {
                // store the type of this WeakEventManager
                Type managerType = typeof(WeakCultureChangedEventManager);

                // try to retrieve an existing instance of the stored type
                var manager = (WeakCultureChangedEventManager)GetCurrentManager(managerType);

                // if the manager does not exists
                if (manager == null)
                {
                    // create a new instance of WeakCultureChangedEventManager
                    manager = new WeakCultureChangedEventManager();

                    // add the new instance to the WeakEventManager manager-store
                    SetCurrentManager(managerType, manager);
                }

                // return the new / existing WeakCultureChangedEventManager instance
                return manager;
            }
        }

        /// <summary>Adds an listener to the inner list of listeners.</summary>
        /// <param name="listener">The listener to add.</param>
        internal static void AddListener(IWeakEventListener listener)
        {
            // add the listener to the inner list of listeners
            CurrentManager.listeners.Add(listener);

            // start / stop the listening process
            CurrentManager.StartStopListening();
        }

        /// <summary>Removes an listener from the inner list of listeners.</summary>
        /// <param name="listener">The listener to remove.</param>
        internal static void RemoveListener(IWeakEventListener listener)
        {
            // removes the listener from the inner list of listeners
            CurrentManager.listeners.Remove(listener);

            // start / stop the listening process
            CurrentManager.StartStopListening();
        }

        /// <summary>This method starts the listening process by attaching on the source event.</summary>
        /// <param name="source">The source.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void StartListening(object source)
        {
            if (this.isListening)
            {
                return;
            }

            Localize.Instance.OnCultureChanged -= this.InstanceOnCultureChanged;
            Localize.Instance.OnCultureChanged += this.InstanceOnCultureChanged;
            this.isListening = true;
        }

        /// <summary>This method stops the listening process by detaching on the source event.</summary>
        /// <param name="source">The source to stop listening on.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void StopListening(object source)
        {
            if (!this.isListening)
            {
                return;
            }

            Localize.Instance.OnCultureChanged -= this.InstanceOnCultureChanged;
            this.isListening = false;
        }

        /// <summary>
        ///   This method is called if the LocalizeDictionary.OnCultureChanged is called and the listening process is
        ///   enabled.
        /// </summary>
        private void InstanceOnCultureChanged()
        {
            // tells every listener in the list that the event is occurred
            this.DeliverEventToList(Localize.Instance, EventArgs.Empty, this.listeners);
        }

        /// <summary>This method starts and stops the listening process by attaching/detaching on the source event.</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void StartStopListening()
        {
            // check if listeners are available and the listening process is stopped, start it. otherwise if no
            // listeners are available and the listening process is started, stop it
            if (this.listeners.Count != 0)
            {
                if (!this.isListening)
                {
                    this.StartListening(null);
                }
            }
            else
            {
                if (this.isListening)
                {
                    this.StopListening(null);
                }
            }
        }
    }
}