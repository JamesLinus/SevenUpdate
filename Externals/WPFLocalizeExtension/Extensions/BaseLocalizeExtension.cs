// <copyright file="BaseLocalizeExtension.cs" project="WPFLocalizeExtension">Bernhard Millauer</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using WPFLocalizeExtension.Engine;

    /// <summary>
    ///   Implements the BaseLocalizeExtension.Represents a LocalizationExtension which provides a localized object of a
    ///   .resx dictionary.
    /// </summary>
    /// <typeparam name="TValue">The Markup extension to use.</typeparam>
    /// <remarks>
    ///   If a content between two tags in xaml is set, this has the higher priority and will overwrite the settled
    ///   properties
    /// </remarks>
    [MarkupExtensionReturnType(typeof(object))]
    [ContentProperty("ResourceIdentifierKey")]
    public abstract class BaseLocalizeExtension<TValue> : MarkupExtension, IWeakEventListener, INotifyPropertyChanged
    {
        /// <summary>Holds the collection of assigned dependency objects as WeakReferences.</summary>
        readonly Dictionary<WeakReference, object> targetObjects;

        /// <summary>Holds the name of the Assembly where the .resx is located.</summary>
        string assembly;

        /// <summary>The current value.</summary>
        TValue currentValue;

        /// <summary>Holds the Name of the .resx dictionary.If it's <c>null</c>, "Resources" will get returned.</summary>
        string dict;

        /// <summary>Holds the Key to a .resx object.</summary>
        string key;

        /// <summary>Initializes a new instance of the <see cref="BaseLocalizeExtension{TValue}" /> class.</summary>
        protected BaseLocalizeExtension()
        {
            // initialize the collection of the assigned dependency objects
            this.targetObjects = new Dictionary<WeakReference, object>();
        }

        /// <summary>Initializes a new instance of the <see cref="BaseLocalizeExtension{TValue}" /> class.  of the BaseLocalizeExtension class.</summary>
        /// <param name="key">Three types are supported:Direct: passed key = key;Dictionary/Key pair: this have to be separated like ResXDictionaryName:<c>ResourceKey</c>Assembly/Dictionary/Key pair: this have to be separated like ResXDictionaryName:<c>ResourceKey</c>.</param>
        /// <remarks>
        ///   This constructor register the <c>EventHandler</c><c>OnCultureChanged</c> on <c>LocalizeDictionary</c> to
        ///   get an acknowledge of changing the culture
        /// </remarks>
        protected BaseLocalizeExtension(string key) : this()
        {
            // parse the key value and split it up if necessary
            Localize.ParseKey(key, out this.assembly, out this.dict, out this.key);
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   Gets the current value.This property has only a value, if the <c>BaseLocalizeExtension</c> is binded to a
        ///   target.
        /// </summary>
        /// <value>The current value.</value>
        public TValue CurrentValue
        {
            get { return this.currentValue; }

            private set
            {
                this.currentValue = value;
                this.RaiseNotifyPropertyChanged("CurrentValue");
            }
        }

        /// <summary>Gets or sets the culture to force a fixed localized object.</summary>
        public string ForceCulture { get; set; }

        /// <summary>Gets or sets the initialize value.This is ONLY used to support the localize extension in blend!.</summary>
        /// <value>The initialize value.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ConstructorArgument("key")]
        public string InitializeValue { get; set; }

        /// <summary>Gets or sets the Key that identifies a resource (Assembly:Dictionary:Key).</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ResourceIdentifierKey
        {
            get
            {
                return string.Format(
                    CultureInfo.CurrentCulture, "{0}:{1}:{2}", this.Assembly, this.Dictionary, this.Key ?? "(null)");
            }

            set { Localize.ParseKey(value, out this.assembly, out this.dict, out this.key); }
        }

        /// <summary>Gets the collection of <c>DependencyObject</c> as WeakReferences and the target property.</summary>
        public Dictionary<WeakReference, object> TargetObjects
        {
            get { return this.targetObjects; }
        }

        /// <summary>
        ///   Gets or sets the name of the Assembly where the .resx is located.If it's <c>null</c>, the executing
        ///   assembly (where this LocalizeEngine is located at) will get returned.
        /// </summary>
        protected string Assembly
        {
            get { return this.assembly ?? Localize.GetAssemblyName(System.Reflection.Assembly.GetExecutingAssembly()); }

            set { this.assembly = !string.IsNullOrEmpty(value) ? value : null; }
        }

        /// <summary>Gets the current <c>CultureInfo</c>, otherwise LocalizeDictionary.Culture will get returned.</summary>
        /// <returns>The <c>CultureInfo</c>.</returns>
        /// <exception cref="System.ArgumentException">thrown if the parameter Culture don't defines a valid <c>CultureInfo</c></exception>
        protected CultureInfo Culture
        {
            get
            {
                // define a culture info
                CultureInfo cultureInfo = null;

                // check if the forced culture is not null or empty
                if (!string.IsNullOrEmpty(this.ForceCulture))
                {
                    // try to create a valid culture info, if defined
                    try
                    {
                        // try to create a specific culture from the forced one
                        cultureInfo = CultureInfo.CreateSpecificCulture(this.ForceCulture);
                    }
                    catch (ArgumentException)
                    {
                        // on error, check if design mode is on
                        if (Localize.Instance.IsInDesignMode)
                        {
                            // cultureInfo will be set to the current specific culture
                            cultureInfo = Localize.Instance.SpecificCulture;
                        }
                    }
                }
                else
                {
                    // take the current specific culture
                    cultureInfo = Localize.Instance.SpecificCulture;
                }

                // return the evaluated culture info
                return cultureInfo;
            }
        }

        /// <summary>Gets or sets the design value.</summary>
        /// <value>The design value.</value>
        [DesignOnly(true)]
        protected object DesignValue { get; set; }

        /// <summary>Gets or sets the Name of the .resx dictionary.If it's <c>null</c>, "Resources" will get returned.</summary>
        protected string Dictionary
        {
            get { return this.dict ?? Localize.ResourcesName; }

            set { this.dict = !string.IsNullOrEmpty(value) ? value : null; }
        }

        /// <summary>Gets or sets the Key to a .resx object.</summary>
        protected string Key
        {
            get { return this.key; }

            set { this.key = value; }
        }

        /// <summary>Provides the Value for the first Binding.</summary>
        /// <param name="serviceProvider">The <c>System.Windows.Markup.IProvideValueTarget</c> provided from the <c>MarkupExtension</c>.</param>
        /// <returns>The found item from the .resx directory or <c>null</c> if not found.</returns>
        /// <remarks>
        ///   This method register the <c>EventHandler</c><c>OnCultureChanged</c> on <c>LocalizeDictionary</c> to get an
        ///   acknowledge of changing the culture. If the passed <c>TargetObjects</c> type of <c>DependencyObject</c>.
        /// </remarks>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // try to cast the passed serviceProvider to a IProvideValueTarget
            var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            // if the cast fails, return this
            if (service == null)
            {
                return this;
            }

            // if the service.TargetObject is a Binding, throw an exception
            if (service.TargetObject is Binding)
            {
                throw new InvalidOperationException("Use as binding is not supported!");
            }

            // declare a target property
            object targetProperty = null;

            // check if the service.TargetProperty is a DependencyProperty or a PropertyInfo
            if (service.TargetProperty is DependencyProperty || service.TargetProperty is PropertyInfo)
            {
                // set the target property to the service.TargetProperty
                targetProperty = service.TargetProperty;
            }

            // check if the target property is null
            if (targetProperty == null)
            {
                // return this.
                return this;
            }

            // if the service.TargetObject is System.Windows.SharedDp (= not a DependencyObject), we return "this". the
            // SharedDp will call this instance later again.
            if (!(service.TargetObject is DependencyObject) && !(service.TargetProperty is PropertyInfo))
            {
                // by returning "this", the provide value will be called later again.
                return this;
            }

            // indicates, if the target object was found
            bool foundInWeakReferences =
                this.targetObjects.Any(
                    wr => wr.Key.Target == service.TargetObject && wr.Value == service.TargetProperty);

            // search for the target in the target object list

            // if the target is a dependency object and it's not collected already, collect it
            if (service.TargetObject is DependencyObject && !foundInWeakReferences)
            {
                // if it's the first object, add an event handler too
                if (this.targetObjects.Count == 0)
                {
                    // add this localize extension to the WeakEventManager on LocalizeDictionary
                    Localize.AddEventListener(this);
                }

                // add the target as an dependency object as weak reference to the dependency object list
                this.targetObjects.Add(new WeakReference(service.TargetObject), service.TargetProperty);

                // adds this localize extension to the ObjectDependencyManager to ensure the lifetime along with the
                // target object
                ObjectDependencyManager.AddObjectDependency(new WeakReference(service.TargetObject), this);
            }

            // return the new value for the DependencyProperty
            return Localize.Instance.GetLocalizedObject<object>(this.Assembly, this.Dictionary, this.Key, this.Culture);
        }

        /// <summary>Resolves the localized value of the current Assembly, Dictionary, Key pair.</summary>
        /// <param name="resolvedValue">The resolved value.</param>
        /// <returns>True if the resolve was success, otherwise <c>False</c>.</returns>
        /// <exception>If the Assembly, Dictionary, Key pair was not found.</exception>
        public bool ResolveLocalizedValue(out TValue resolvedValue)
        {
            // return the resolved localized value with the current or forced culture.
            return this.ResolveLocalizedValue(out resolvedValue, this.Culture);
        }

        /// <summary>
        ///   Sets a binding between a <c>DependencyObject</c> with its <c>DependencyProperty</c>or <see cref =
        ///   "PropertyInfo" /> and the <c>BaseLocalizeExtension</c>.
        /// </summary>
        /// <param name="targetObject">The target dependency object.</param>
        /// <param name="targetProperty">The target dependency property.</param>
        /// <returns><c>True</c> if the binding was setup successfully, otherwise <c>False</c> (Binding already exists).</returns>
        /// <exception cref="ArgumentException">If the <paramref name="targetProperty" /> is not a <c>DependencyProperty</c> or <c>PropertyInfo</c>.</exception>
        public bool SetBinding(DependencyObject targetObject, object targetProperty)
        {
            if (!(targetProperty is DependencyProperty || targetProperty is PropertyInfo))
            {
                throw new ArgumentException(
                    "The targetProperty should be a DependencyProperty or PropertyInfo!", "targetProperty");
            }

            // indicates, if the target object was found
            bool foundInWeakReferences =
                this.targetObjects.Any(wr => wr.Key.Target == targetObject && wr.Value == targetProperty);

            // search for the target in the target object list

            // if the target it's not collected already, collect it
            if (!foundInWeakReferences)
            {
                // if it's the first object, add an event handler too
                if (this.targetObjects.Count == 0)
                {
                    // add this localize extension to the WeakEventManager on LocalizeDictionary
                    Localize.AddEventListener(this);
                }

                // add the target as an dependency object as weak reference to the dependency object list
                this.targetObjects.Add(new WeakReference(targetObject), targetProperty);

                // adds this localize extension to the ObjectDependencyManager to ensure the lifetime along with the
                // target object
                ObjectDependencyManager.AddObjectDependency(new WeakReference(targetObject), this);

                // get the initial value of the dependency property
                object output =
                    this.FormatOutput(
                        Localize.Instance.GetLocalizedObject<object>(
                            this.Assembly, this.Dictionary, this.Key, this.Culture));

                // set the value to the dependency object
                SetTargetValue(targetObject, targetProperty, output);

                // return true, the binding was successfully
                return true;
            }

            // return false, the binding already exists
            return false;
        }

        /// <summary>Returns the Key that identifies a resource (Assembly:Dictionary:Key).</summary>
        /// <returns>Format: Assembly:Dictionary:Key.</returns>
        public override sealed string ToString()
        {
            return base.ToString() + " -> " + this.ResourceIdentifierKey;
        }

        /// <summary>Receives events from the centralized event manager.</summary>
        /// <param name="managerType">The type of the <c>T:System.Windows.WeakEventManager</c> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns><c>True</c> if the listener handled the event. It is considered an error by the <see
        /// cref="T:System.Windows.WeakEventManager" /> handling in WPF to register a listener for an event that the
        /// listener does not handle. Regardless, the method should return <c>false</c> if it receives an event that it
        /// does not recognize or handle.</returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            // if the passed handler is type of LocalizeDictionary.WeakCultureChangedEventManager, handle it
            if (managerType == typeof(WeakCultureChangedEventManager))
            {
                // call to handle the new value
                this.HandleNewValue();

                // return true, to notify the event was processed
                return true;
            }

            // return false, to notify the event was not processed
            return false;
        }

        /// <summary>Determines whether if the <paramref name="checkType" /> is the <paramref name="targetType" />.</summary>
        /// <param name="checkType">Type of the check.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><c>True</c> if the <paramref name="checkType" /> is type of the <paramref name="targetType" />; otherwise, <c>false</c>.</returns>
        internal bool IsTypeOf(Type checkType, Type targetType)
        {
            // if the checkType is null (possible base type), return false
            if (checkType == null)
            {
                return false;
            }

            // if the targetType (wrong call) is null, return false
            if (targetType == null)
            {
                return false;
            }

            // if we search a generic type
            if (targetType.IsGenericType)
            {
                // and the checkType is a generic (BaseType)
                if (checkType.IsGenericType)
                {
                    // and the signature is the same
                    if (checkType.GetGenericTypeDefinition() == targetType)
                    {
                        // return true
                        return true;
                    }
                }

                // otherwise call the same method again with the base type
                return this.IsTypeOf(checkType.BaseType, targetType);
            }

            // if we search a non generic type and its equal
            return checkType.Equals(targetType) || this.IsTypeOf(checkType.BaseType, targetType);

            // otherwise call the same method again with the base type
        }

        /// <summary>This method is used to modify the passed object into the target format.</summary>
        /// <param name="input">The object that will be modified.</param>
        /// <returns>Returns the modified object.</returns>
        protected abstract object FormatOutput(object input);

        /// <summary>This method gets the new value for the target property and call <c>SetNewValue</c>.</summary>
        protected virtual void HandleNewValue()
        {
            // gets the new value and set it to the dependency property on the dependency object
            this.SetNewValue(
                Localize.Instance.GetLocalizedObject<object>(this.Assembly, this.Dictionary, this.Key, this.Culture));
        }

        /// <summary>
        ///   This method will be called through the interface, passed to theLocalizeDictionary. LocalizeDictionary.
        ///   <c>Localize.WeakCultureChangedEventManager</c> to get notified on culture changed.
        /// </summary>
        /// <param name="managerType">The manager Type.</param>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event argument.</param>
        /// <returns><c>True</c> if the listener handled the event. It is considered an error by the <see
        /// cref="T:System.Windows.WeakEventManager" /> handling in WPF to register a listener for an event that the
        /// listener does not handle. Regardless, the method should return <c>false</c> if it receives an event that it
        /// does not recognize or handle.</returns>
        protected bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ((IWeakEventListener)this).ReceiveWeakEvent(managerType, sender, e);
        }

        /// <summary>Set the Value of the <c>DependencyProperty</c> to the passed Value.</summary>
        /// <param name="newValue">The new Value.</param>
        protected void SetNewValue(object newValue)
        {
            // set the new value to the current value, if its the type of TValue
            if (newValue is TValue)
            {
                this.CurrentValue = (TValue)newValue;
            }

            // if the list of dependency objects is empty or the target property is null, return
            if (this.targetObjects.Count == 0)
            {
                return;
            }

            // step through all dependency objects as WeakReference and refresh the value of the dependency property
            foreach (var dpo in this.targetObjects.Where(dpo => dpo.Key.IsAlive))
            {
                SetTargetValue((DependencyObject)dpo.Key.Target, dpo.Value, newValue);
            }
        }

        /// <summary>Sets the target value.</summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="value">The value.</param>
        static void SetTargetValue(DependencyObject targetObject, object targetProperty, object value)
        {
            // check if the target property is a DependencyProperty
            if (targetProperty is DependencyProperty)
            {
                SetTargetValue(targetObject, (DependencyProperty)targetProperty, value);
            }

            // check if the target property is a PropertyInfo
            if (targetProperty is PropertyInfo)
            {
                SetTargetValue(targetObject, (PropertyInfo)targetProperty, value);
            }
        }

        /// <summary>Sets the target value.</summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="value">The value.</param>
        static void SetTargetValue(DependencyObject targetObject, DependencyProperty targetProperty, object value)
        {
            targetObject.SetValue(targetProperty, value);
        }

        /// <summary>Sets the target value.</summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="value">The value.</param>
        static void SetTargetValue(DependencyObject targetObject, PropertyInfo targetProperty, object value)
        {
            targetProperty.SetValue(targetObject, value, null);
        }

        /// <summary>Raises the notify property changed.</summary>
        /// <param name="propertyName">Name of the property.</param>
        void RaiseNotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>Resolves the localized value of the current Assembly, Dictionary, Key pair.</summary>
        /// <param name="resolvedValue">The resolved value.</param>
        /// <param name="targetCulture">The target culture.</param>
        /// <returns>True if the resolve was success, otherwise <c>False</c>.</returns>
        /// <exception>If the Assembly, Dictionary, Key pair was not found.</exception>
        bool ResolveLocalizedValue(out TValue resolvedValue, CultureInfo targetCulture)
        {
            // define the default value of the resolved value
            resolvedValue = default(TValue);

            // get the localized object from the dictionary
            var localizedObject = Localize.Instance.GetLocalizedObject<object>(
                this.Assembly, this.Dictionary, this.Key, targetCulture);

            // check if the found localized object is type of TValue
            if (localizedObject is TValue)
            {
                // format the localized object
                object formattedOutput = this.FormatOutput(localizedObject);

                // check if the formatted output is not null
                if (formattedOutput != null)
                {
                    // set the content of the resolved value
                    resolvedValue = (TValue)formattedOutput;
                }

                // return true: resolve was successfully
                return true;
            }

            // return false: resolve was not successfully.
            return false;
        }
    }
}