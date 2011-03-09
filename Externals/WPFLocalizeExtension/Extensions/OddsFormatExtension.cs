// ***********************************************************************
// <copyright file="OddsFormatExtension.cs"
//            project="WPFLocalizeExtension"
//            assembly="WPFLocalizeExtension"
//            solution="SevenUpdate"
//            company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// <license href="http://wpflocalizeextension.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************
namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;

    using WPFLocalizeExtension.Engine;

    /// <summary>Represents a OddsFormatExtension which provides a formated decimal odds value.</summary>
    /// <remarks>If a content between two tags in Xaml is set, this has the higher priority and will overwrite the settled properties</remarks>
    [MarkupExtensionReturnType(typeof(string)), ContentProperty("ResourceIdentifierKey")]
    public sealed class OddsFormatExtension : MarkupExtension, IWeakEventListener
    {
        #region Constants and Fields

        /// <summary>Holds the collection of assigned dependency objects as WeakReferences</summary>
        private readonly Collection<WeakReference> targetObjects;

        /// <summary>Holds the Dictionary of the Lookup Table.</summary>
        private static Dictionary<decimal, string> oddsFormatLookupTableUk;

        /// <summary>Holds the value to display</summary>
        private decimal displayValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="OddsFormatExtension"/> class.Initialize the <c>BaseLocalizeExtension"</c>.</summary>
        /// <param name="displayValue">The display Value.</param>
        /// <remarks>This constructor register the <see cref="EventHandler"/><c>OnCultureChanged</c> on <c>LocalizeDictionary</c>to get an acknowledge of changing the culture.</remarks>
        public OddsFormatExtension(decimal displayValue) : this()
        {
            this.displayValue = displayValue;
        }

        /// <summary>Prevents a default instance of the <see cref = "OddsFormatExtension" /> class from being created. Initialize the <c>BaseLocalizeExtension</c>.</summary>
        private OddsFormatExtension()
        {
            // initialize the collection of the assigned dependency objects
            this.targetObjects = new Collection<WeakReference>();
        }

        #endregion

        #region Properties

        /// <summary>Gets the UK odds format lookup table.</summary>
        /// <value>The UK odds format lookup table.</value>
        public static Dictionary<decimal, string> OddsFormatLookupTableUK
        {
            get
            {
                return oddsFormatLookupTableUk ?? (oddsFormatLookupTableUk = GetUKOddsFormatLookupTable());
            }
        }

        /// <summary>Gets or sets the display value.</summary>
        /// <value>The display value.</value>
        public decimal DisplayValue
        {
            get
            {
                return this.displayValue;
            }

            set
            {
                this.displayValue = value;
                this.HandleNewValue();
            }
        }

        /// <summary>Gets or sets the <see cref = "OddsFormatType" /> to force a fixed output</summary>
        public OddsFormatType? ForceOddsFormatType { get; set; }

        /// <summary>Gets or sets the initialize value.This is ONLY used to support the localize extension in blend!</summary>
        /// <value>The initialize value.</value>
        [EditorBrowsable(EditorBrowsableState.Never), ConstructorArgument("displayValue")]
        public decimal InitializeValue { get; set; }

        /// <summary>Gets the collection of <see cref = "DependencyObject" /> as WeakReferences which contains the <see cref = "TargetProperty" />.</summary>
        public ReadOnlyCollection<WeakReference> TargetObjects
        {
            get
            {
                return new ReadOnlyCollection<WeakReference>(this.targetObjects);
            }
        }

        /// <summary>Gets the <see cref = "DependencyProperty" /> which should get the localized content.</summary>
        public DependencyProperty TargetProperty { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>Converts a decimal odds into a localized odds string in the current <see cref="OddsFormatType"/>.</summary>
        /// <param name="sourceOdds">The source odds.</param>
        /// <returns>The ready to use odds string.</returns>
        public static string GetLocalizedOddsString(decimal sourceOdds)
        {
            return GetLocalizedOddsString(sourceOdds, OddsFormatManager.Instance.OddsFormatType, Localize.Instance.SpecificCulture);
        }

        /// <summary>Converts a decimal odds into a localized odds string in the defined <see cref="OddsFormatType"/>.</summary>
        /// <param name="sourceOdds">The source odds.</param>
        /// <param name="oddsType">Type of the odds.</param>
        /// <returns>The ready to use odds string.</returns>
        public static string GetLocalizedOddsString(decimal sourceOdds, OddsFormatType oddsType)
        {
            return GetLocalizedOddsString(sourceOdds, oddsType, Localize.Instance.SpecificCulture);
        }

        /// <summary>Converts a decimal odds into a localized odds string in the defined <see cref="OddsFormatType"/>.</summary>
        /// <param name="sourceOdds">The source odds.</param>
        /// <param name="oddsType">Type of the odds.</param>
        /// <param name="specificCulture">The specific culture.</param>
        /// <returns>The ready to use odds string.</returns>
        /// <remarks>The specific Culture has to be a "xx-xx" culture to support the value.<see cref="ToString"/> method.</remarks>
        public static string GetLocalizedOddsString(decimal sourceOdds, OddsFormatType oddsType, IFormatProvider specificCulture)
        {
            switch (oddsType)
            {
                case OddsFormatType.UK:
                    if (sourceOdds <= 0)
                    {
                        return "0";
                    }

                    string lookupValue;
                    if (TryGetUKOddsLookupValue(sourceOdds, out lookupValue))
                    {
                        return lookupValue;
                    }

                    var localeString = (sourceOdds % 1.0m) * 100;
                    if (localeString == 0)
                    {
                        return string.Format(specificCulture, "{0:N0}/1", sourceOdds - 1);
                    }

                    var two = 0;
                    var one = 0;
                    decimal dec = 1;
                    while ((localeString > 1 && two < 2 && one < 2) && (localeString % 2 == 0 || localeString % 5 == 0))
                    {
                        if (localeString % 2 == 0)
                        {
                            ++two;
                            localeString = localeString / 2;
                            dec = dec * 2;
                        }

                        if (localeString % 5 != 0)
                        {
                            continue;
                        }

                        ++one;
                        localeString = localeString / 5;
                        dec = dec * 5;
                    }

                    var divisor = 100 / dec;
                    var result = (sourceOdds - 1) * divisor;

                    return string.Format(specificCulture, "{0:N0}/{1:N0}", result, divisor);

                case OddsFormatType.US:

                    if (sourceOdds <= 1)
                    {
                        return "0";
                    }

                    return sourceOdds < 2 ? (-100 / (sourceOdds - 1)).ToString("N0", specificCulture) : string.Format(specificCulture, "+{0:N0}", (sourceOdds - 1) * 100);

                case OddsFormatType.EU:
                    return sourceOdds.ToString("N2", specificCulture);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>Provides the Value for the first Binding</summary>
        /// <param name="serviceProvider">The <see cref="System.Windows.Markup.IProvideValueTarget"/> provided from the <see cref="MarkupExtension"/>.</param>
        /// <returns>The found item from the .resx directory or <see langword="null"/> if not found.</returns>
        /// <remarks>
        /// This method register the <see cref="EventHandler"/><c>OnCultureChanged</c> on <c>LocalizeDictionary</c>
        ///   to get an acknowledge of changing the culture, if the passed <see cref="TargetObjects"/> type of <see cref="DependencyObject"/>.
        ///   !PROOF: On every single <see cref="UserControl"/>, Window, and Page,
        ///   there is a new SparedDP reference, and so there is every time a new <c>BaseLocalizeExtension</c>!
        ///   Because of this, we don't need to notify every single DependencyObjects to update their value (for GC).
        /// </remarks>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // try to cast the passed serviceProvider to a IProvideValueTarget
            var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            // if the cast fails, an exception will be thrown. (should never happen)
            if (service == null)
            {
                throw new InvalidOperationException("IProvideValueTarget service is unavailable");
            }

            // if the service.TargetObject is a Binding, throw an exception
            if (service.TargetObject is Binding)
            {
                throw new InvalidOperationException("Use as binding is not supported!");
            }

            // try to cast the service.TargetProperty as DependencyProperty
            this.TargetProperty = service.TargetProperty as DependencyProperty;

            // if this fails, return this (should never happen)
            if (this.TargetProperty == null)
            {
                return this;
            }

            // indicates, if the target object was found
            var foundInWeakReferences = this.targetObjects.Any(wr => wr.Target == service.TargetObject);

            // search for the target in the target object list

            // if the target is a dependency object and it's not collected already, collect it
            if (service.TargetObject is DependencyObject && !foundInWeakReferences)
            {
                // if it's the first object, add an event handler too
                if (this.targetObjects.Count == 0)
                {
                    // add this localize extension to the WeakEventManager on LocalizeDictionary
                    OddsFormatManager.AddEventListener(this);
                }

                // add the target as an dependency object as weak reference to the dependency object list
                this.targetObjects.Add(new WeakReference(service.TargetObject));

                // adds this localize extension to the ObjectDependencyManager to ensure the lifetime along with the target object
                ObjectDependencyManager.AddObjectDependency(new WeakReference(service.TargetObject), this);
            }

            // if the service.TargetObject is System.Windows.SharedDp (= not a DependencyObject), we return "this".
            // the SharedDp will call this instance later again.
            if (!(service.TargetObject is DependencyObject))
            {
                // by returning "this", the provide value will be called later again.
                return this;
            }

            // return the new value for the DependencyProperty
            return GetLocalizedOddsString(this.displayValue, this.GetForcedOddsFormatOrDefault());
        }

        /// <summary>Sets a binding between a <see cref="DependencyObject"/> with its<see cref="DependencyProperty"/> and this <c>BaseLocalizeExtension</c>.</summary>
        /// <param name="targetObject">The target dependency object</param>
        /// <param name="targetProperty">The target dependency property</param>
        /// <returns><see langword="true"/> if the binding was setup successfully, otherwise <see langword="false"/> (Binding already exists).</returns>
        public bool SetBinding(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            // indicates, if the target object was found
            var foundInWeakReferences = this.targetObjects.Any(wr => wr.Target == targetObject);

            // search for the target in the target object list

            // set the TargetProperty to the passed one
            this.TargetProperty = targetProperty;

            // if the target it's not collected already, collect it
            if (!foundInWeakReferences)
            {
                // if it's the first object, add an event handler too
                if (this.targetObjects.Count == 0)
                {
                    // add this localize extension to the WeakEventManager on LocalizeDictionary
                    OddsFormatManager.AddEventListener(this);
                }

                // add the target as an dependency object as weak reference to the dependency object list
                this.targetObjects.Add(new WeakReference(targetObject));

                // adds this localize extension to the ObjectDependencyManager to ensure the lifetime along with the target object
                ObjectDependencyManager.AddObjectDependency(new WeakReference(targetObject), this);

                // set the initial value of the dependency property
                targetObject.SetValue(this.TargetProperty, GetLocalizedOddsString(this.displayValue, this.GetForcedOddsFormatOrDefault()));

                // return true, the binding was successfully
                return true;
            }

            // return false, the binding already exists
            return false;
        }

        /// <summary>Returns the Key that identifies a resource (Assembly:Dictionary:Key)</summary>
        /// <returns>Format: Assembly:Dictionary:Key</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0} -> {1}", this.DisplayValue, this.GetForcedOddsFormatOrDefault());
        }

        #endregion

        #region Implemented Interfaces

        #region IWeakEventListener

        /// <summary>This method will be called through the interface, passed to the<see cref="Localize.WeakCultureChangedEventManager"/> to get notified on culture changed</summary>
        /// <param name="managerType">The manager Type.</param>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event argument.</param>
        /// <returns><see langword="true"/> if the listener handled the event. It is considered an error by the<see cref="T:System.Windows.WeakEventManager"/> handling in WPF�to register alistener for an event that the listener does not handle. Regardless,the method should return <see langword="false"/> if it receives an event that it does not recognize or handle.</returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            // if the passed handler is type of LocalizeDictionary.WeakCultureChangedEventManager, handle it
            if (managerType == typeof(OddsFormatManager.WeakOddsFormatChangedEventManager))
            {
                // call to handle the new value
                this.HandleNewValue();

                // return true, to notify the event was processed
                return true;
            }

            // return false, to notify the event was not processed
            return false;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>Gets the UK odds format lookup table.</summary>
        /// <returns>Returns a Lookup Table</returns>
        private static Dictionary<decimal, string> GetUKOddsFormatLookupTable()
        {
            var dictionary = new Dictionary<decimal, string> 
            {
                    { 11.00m, "10/1" }, 
                    { 10.00m, "9/1" }, 
                    { 9.50m, "17/2" }, 
                    { 9.00m, "8/1" }, 
                    { 8.50m, "15/2" }, 
                    { 8.00m, "7/1" }, 
                    { 7.50m, "13/2" }, 
                    { 7.00m, "6/1" }, 
                    { 6.50m, "11/2" }, 
                    { 6.00m, "5/1" }, 
                    { 5.50m, "9/2" }, 
                    { 5.00m, "4/1" }, 
                    { 4.60m, "18/5" }, 
                    { 4.50m, "7/2" }, 
                    { 4.333m, "10/3" }, 
                    { 4.20m, "16/5" }, 
                    { 4.00m, "3/1" }, 
                    { 3.80m, "15/5" }, 
                    { 3.75m, "11/4" }, 
                    { 3.60m, "13/5" }, 
                    { 3.50m, "5/2" }, 
                    { 3.40m, "12/5" }, 
                    { 3.375m, "19/8" }, 
                    { 3.30m, "23/10" }, 
                    { 3.25m, "9/4" }, 
                    { 3.20m, "11/5" }, 
                    { 3.125m, "17/8" }, 
                    { 3.10m, "21/10" }, 
                    { 3.00m, "2/1" }, 
                    { 2.90m, "19/10" }, 
                    { 2.875m, "15/8" }, 
                    { 2.80m, "9/5" }, 
                    { 2.75m, "7/4" }, 
                    { 2.70m, "17/10" }, 
                    { 2.625m, "13/8" }, 
                    { 2.60m, "8/5" }, 
                    { 2.50m, "6/4" }, 
                    { 2.40m, "7/5" }, 
                    { 2.375m, "11/8" }, 
                    { 2.30m, "13/10" }, 
                    { 2.25m, "5/4" }, 
                    { 2.20m, "6/5" }, 
                    { 2.10m, "11/10" }, 
                    { 2.05m, "21/20" }, 
                    { 2.00m, "1/1" }, 
                    { 1.952m, "20/21" }, 
                    { 1.909m, "10/11" }, 
                    { 1.90m, "9/10" }, 
                    { 1.833m, "5/6" }, 
                    { 1.80m, "4/5" }, 
                    { 1.727m, "8/11" }, 
                    { 1.70m, "7/10" }, 
                    { 1.667m, "4/6" }, 
                    { 1.625m, "5/8" }, 
                    { 1.615m, "8/13" }, 
                    { 1.60m, "3/5" }, 
                    { 1.571m, "4/7" }, 
                    { 1.533m, "8/15" }, 
                    { 1.50m, "1/2" }, 
                    { 1.471m, "8/17" }, 
                    { 1.45m, "9/20" }, 
                    { 1.444m, "4/9" }, 
                    { 1.40m, "2/5" }, 
                    { 1.364m, "4/11" }, 
                    { 1.35m, "7/20" }, 
                    { 1.333m, "1/3" }, 
                    { 1.30m, "3/10" }, 
                    { 1.286m, "2/7" }, 
                    { 1.25m, "1/4" }, 
                    { 1.222m, "2/9" }, 
                    { 1.2m, "1/5" }
                };

            return dictionary;
        }

        /// <summary>Tries the get UK odds lookup value.</summary>
        /// <param name="valToCheck">The value to check.</param>
        /// <param name="retVal">The return value. <see langword="null"/> if nothing was found.</param>
        /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/>.</returns>
        private static bool TryGetUKOddsLookupValue(decimal valToCheck, out string retVal)
        {
            if (OddsFormatLookupTableUK.ContainsKey(valToCheck))
            {
                retVal = OddsFormatLookupTableUK[valToCheck];

                return true;
            }

            retVal = null;
            return false;
        }

        /// <summary>If Culture property defines a valid <see cref="CultureInfo"/>, a <see cref="CultureInfo"/> instance will getcreated and returned, otherwise LocalizeDictionary.Culture will get returned.</summary>
        /// <returns>The <see cref="CultureInfo"/>.</returns>
        private OddsFormatType GetForcedOddsFormatOrDefault()
        {
            return this.ForceOddsFormatType ?? OddsFormatManager.Instance.OddsFormatType;
        }

        /// <summary>This method gets the new value for the target property and call <see cref="SetNewValue"/>.</summary>
        private void HandleNewValue()
        {
            // gets the new value and set it to the dependency property on the dependency object
            this.SetNewValue(GetLocalizedOddsString(this.displayValue, this.GetForcedOddsFormatOrDefault()));
        }

        /// <summary>Set the Value of the <see cref="DependencyProperty"/> to the passed Value.</summary>
        /// <param name="newValue">The new Value</param>
        private void SetNewValue(object newValue)
        {
            // if the list of dependency objects is empty or the target property is null, return
            if (this.targetObjects.Count == 0 || this.TargetProperty == null)
            {
                return;
            }

            // step through all dependency objects as WeakReference and refresh the value of the dependency property
            foreach (var dpo in this.targetObjects.Where(dpo => dpo.IsAlive))
            {
                ((DependencyObject)dpo.Target).SetValue(this.TargetProperty, newValue);
            }
        }

        #endregion
    }
}