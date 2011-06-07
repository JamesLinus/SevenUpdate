// ***********************************************************************
// <copyright file="OddsFormatManager.cs" project="WPFLocalizeExtension" assembly="WPFLocalizeExtension" solution="SevenUpdate" company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// <license href="http://wpflocalizeextension.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************

namespace WPFLocalizeExtension.Engine
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows;

    /// <summary>
    ///   Represents the odds format manager.
    /// </summary>
    public sealed class OddsFormatManager : DependencyObject
    {
        #region Constants and Fields

        /// <summary>
        ///   Registers the design odds format property
        /// </summary>
        [DesignOnly(true)]
        private static readonly DependencyProperty DesignOddsFormatProperty =
            DependencyProperty.RegisterAttached(
                "DesignOddsFormat",
                typeof(OddsFormatType),
                typeof(OddsFormatManager),
                new PropertyMetadata(DefaultOddsFormatType, SetOddsFormatFromDependencyProperty));

        /// <summary>
        ///   Holds a SyncRoot to be thread safe.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        ///   Holds the instance of singleton.
        /// </summary>
        private static OddsFormatManager instance;

        /// <summary>
        ///   Holds the current chosen <c>OddsFormatType</c>.
        /// </summary>
        private OddsFormatType oddsFormatType = DefaultOddsFormatType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Prevents a default instance of the OddsFormatManager class from being created. Static Constructor.
        /// </summary>
        private OddsFormatManager()
        {
        }

        #endregion

        #region Events

        /// <summary>
        ///   Action for when the odds format changes
        /// </summary>
        internal event Action OnOddsFormatChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the <c>OddsFormatManager</c> singleton.If the underlying instance is <c>null</c>, a instance will be created.
        /// </summary>
        public static OddsFormatManager Instance
        {
            get
            {
                // check if the underlying instance is null
                if (instance == null)
                {
                    // if it is null, lock the sync root.

                    // if another thread is accessing this too, 
                    // it have to wait until the sync root is released
                    lock (SyncRoot)
                    {
                        // check again, if the underlying instance is null
                        if (instance == null)
                        {
                            // create a new instance
                            instance = new OddsFormatManager();
                        }
                    }
                }

                // return the existing/new instance
                return instance;
            }
        }

        /// <summary>
        ///   Gets or sets the odds format type
        /// </summary>
        public OddsFormatType OddsFormatType
        {
            get
            {
                return this.oddsFormatType;
            }

            set
            {
                // the supplied value has to be defined, otherwise an exception will be raised
                if (!Enum.IsDefined(typeof(OddsFormatType), value))
                {
                    throw new ArgumentNullException("value");
                }

                // Set the OddsFormatType
                this.oddsFormatType = value;

                // Raise the OnOddsFormatChanged event
                if (this.OnOddsFormatChanged != null)
                {
                    this.OnOddsFormatChanged();
                }
            }
        }

        /// <summary>
        ///   Gets the default odds format type
        /// </summary>
        private static OddsFormatType DefaultOddsFormatType
        {
            get
            {
                return OddsFormatType.EU;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the status of the design mode.
        /// </summary>
        /// <returns><c>True</c> if in design mode, else <c>False</c>.</returns>
        private bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(this);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Attach an WeakEventListener to the <c>OddsFormatManager</c>.
        /// </summary>
        /// <param name="listener">
        ///   The listener to attach.
        /// </param>
        public static void AddEventListener(IWeakEventListener listener)
        {
            // calls AddListener from the inline WeakOddsFormatChangedEventManager
            WeakOddsFormatChangedEventManager.AddListener(listener);
        }

        /// <summary>
        ///   Getter of <c>DependencyProperty</c> DesignOddsFormat.Only supported at DesignTime.If its in Runtime, the current <c>OddsFormatType</c> will be returned.
        /// </summary>
        /// <param name="obj">
        ///   The dependency object to get the odds format type from.
        /// </param>
        /// <returns>
        ///   The design odds format at design time or the current odds format at runtime.
        /// </returns>
        [DesignOnly(true)]
        public static OddsFormatType GetDesignOddsFormat(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return Instance.IsInDesignMode
                       ? (OddsFormatType)obj.GetValue(DesignOddsFormatProperty)
                       : Instance.OddsFormatType;
        }

        /// <summary>
        ///   Detach an WeakEventListener to the <c>OddsFormatManager</c>.
        /// </summary>
        /// <param name="listener">
        ///   The listener to detach.
        /// </param>
        public static void RemoveEventListener(IWeakEventListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException("listener");
            }

            // calls RemoveListener from the inline WeakOddsFormatChangedEventManager
            WeakOddsFormatChangedEventManager.RemoveListener(listener);
        }

        /// <summary>
        ///   Setter of <c>DependencyProperty</c> DesignOddsFormat. Only supported at DesignTime.
        /// </summary>
        /// <param name="obj">
        ///   The dependency object to set the odds format to.
        /// </param>
        /// <param name="value">
        ///   The odds format.
        /// </param>
        [DesignOnly(true)]
        public static void SetDesignOddsFormat(DependencyObject obj, OddsFormatType value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (Instance.IsInDesignMode)
            {
                obj.SetValue(DesignOddsFormatProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Sets the odds format from the dependency property
        /// </summary>
        /// <param name="obj">
        ///   The dependency object
        /// </param>
        /// <param name="args">
        ///   The event arguments
        /// </param>
        [DesignOnly(true)]
        private static void SetOddsFormatFromDependencyProperty(
            DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!Instance.IsInDesignMode)
            {
                return;
            }

            if (!Enum.IsDefined(typeof(OddsFormatType), args.NewValue))
            {
                if (Instance.IsInDesignMode)
                {
                    Instance.OddsFormatType = DefaultOddsFormatType;
                }
                else
                {
                    throw new InvalidCastException(
                        string.Format(
                            CultureInfo.CurrentCulture, "\"{0}\" not defined in Enum OddsFormatType", args.NewValue));
                }
            }
            else
            {
                Instance.OddsFormatType =
                    (OddsFormatType)Enum.Parse(typeof(OddsFormatType), args.NewValue.ToString(), true);
            }
        }

        #endregion

        /// <summary>
        ///   This in line class is used to handle weak events to avoid memory leaks.
        /// </summary>
        internal sealed class WeakOddsFormatChangedEventManager : WeakEventManager
        {
            #region Constants and Fields

            /// <summary>
            ///   Holds the inner list of listeners.
            /// </summary>
            private readonly ListenerList listeners;

            /// <summary>
            ///   Indicates, if the current instance is listening on the source event.
            /// </summary>
            private bool isListening;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///   Prevents a default instance of the WeakOddsFormatChangedEventManager class from being created.
            /// </summary>
            private WeakOddsFormatChangedEventManager()
            {
                // creates a new list and assign it to listeners
                this.listeners = new ListenerList();
            }

            #endregion

            #region Properties

            /// <summary>
            ///   Gets the singleton instance of <c>WeakOddsFormatChangedEventManager</c>.
            /// </summary>
            private static WeakOddsFormatChangedEventManager CurrentManager
            {
                get
                {
                    // store the type of this WeakEventManager
                    var managerType = typeof(WeakOddsFormatChangedEventManager);

                    // try to retrieve an existing instance of the stored type
                    var manager = (WeakOddsFormatChangedEventManager)GetCurrentManager(managerType);

                    // if the manager does not exists
                    if (manager == null)
                    {
                        // create a new instance of WeakOddsFormatChangedEventManager
                        manager = new WeakOddsFormatChangedEventManager();

                        // add the new instance to the WeakEventManager manager-store
                        SetCurrentManager(managerType, manager);
                    }

                    // return the new / existing WeakOddsFormatChangedEventManager instance
                    return manager;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            ///   Adds an listener to the inner list of listeners.
            /// </summary>
            /// <param name="listener">
            ///   The listener to add.
            /// </param>
            internal static void AddListener(IWeakEventListener listener)
            {
                // add the listener to the inner list of listeners
                CurrentManager.listeners.Add(listener);

                // start / stop the listening process
                CurrentManager.StartStopListening();
            }

            /// <summary>
            ///   Removes an listener from the inner list of listeners.
            /// </summary>
            /// <param name="listener">
            ///   The listener to remove.
            /// </param>
            internal static void RemoveListener(IWeakEventListener listener)
            {
                // removes the listener from the inner list of listeners
                CurrentManager.listeners.Remove(listener);

                // start / stop the listening process
                CurrentManager.StartStopListening();
            }

            /// <summary>
            ///   This method starts the listening process by attaching on the source event.
            /// </summary>
            /// <param name="source">
            ///   The source.
            /// </param>
            [MethodImpl(MethodImplOptions.Synchronized)]
            protected override void StartListening(object source)
            {
                if (this.isListening)
                {
                    return;
                }

                Instance.OnOddsFormatChanged -= this.Instance_OnOddsFormatChanged;
                Localize.Instance.OnCultureChanged -= this.Instance_OnCultureChanged;

                Instance.OnOddsFormatChanged += this.Instance_OnOddsFormatChanged;
                Localize.Instance.OnCultureChanged += this.Instance_OnCultureChanged;
                this.isListening = true;
            }

            /// <summary>
            ///   This method stops the listening process by detaching on the source event.
            /// </summary>
            /// <param name="source">
            ///   The source to stop listening on.
            /// </param>
            [MethodImpl(MethodImplOptions.Synchronized)]
            protected override void StopListening(object source)
            {
                if (!this.isListening)
                {
                    return;
                }

                Instance.OnOddsFormatChanged -= this.Instance_OnOddsFormatChanged;
                Localize.Instance.OnCultureChanged -= this.Instance_OnCultureChanged;
                this.isListening = false;
            }

            /// <summary>
            ///   This method is called if the LocalizeDictionary.OnCultureChanged is called and the listening process is enabled.
            /// </summary>
            private void Instance_OnCultureChanged()
            {
                // tells every listener in the list that the event is occurred
                this.DeliverEventToList(Instance, EventArgs.Empty, this.listeners);
            }

            /// <summary>
            ///   This method is called if the <c>OddsFormatManager</c>.OnOddsFormatChanged is called and the listening process is enabled.
            /// </summary>
            private void Instance_OnOddsFormatChanged()
            {
                // tells every listener in the list that the event is occurred
                this.DeliverEventToList(Instance, EventArgs.Empty, this.listeners);
            }

            /// <summary>
            ///   This method starts and stops the listening process by attaching/detaching on the source event.
            /// </summary>
            [MethodImpl(MethodImplOptions.Synchronized)]
            private void StartStopListening()
            {
                // check if listeners are available and the listening process is stopped, start it.
                // otherwise if no listeners are available and the listening process is started, stop it
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

            #endregion
        }
    }
}