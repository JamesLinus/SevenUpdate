//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell.PropertySystem
{
    using global::System;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Globalization;
    using global::System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Defines a strongly-typed property object. 
    ///   All writable property objects must be of this type 
    ///   to be able to call the value setter.
    /// </summary>
    /// <typeparam name="T">
    /// The type of this property's value. 
    ///   Because a property value can be empty, only nullable types 
    ///   are allowed.
    /// </typeparam>
    public class ShellProperty<T> : IShellProperty
    {
        // The value was set but truncated in a string value or rounded if a numeric value.
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private const int InPlaceStringTruncated = 0x00401A0;

        /// <summary>
        /// </summary>
        private int? imageReferenceIconIndex;

        /// <summary>
        /// </summary>
        private string imageReferencePath;

        /// <summary>
        /// </summary>
        private PropertyKey propertyKey;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructs a new Property object
        /// </summary>
        /// <param name="propertyKey">
        /// </param>
        /// <param name="description">
        /// </param>
        /// <param name="parent">
        /// </param>
        internal ShellProperty(PropertyKey propertyKey, ShellPropertyDescription description, ShellObject parent)
        {
            this.propertyKey = propertyKey;
            this.Description = description;
            this.ParentShellObject = parent;
            this.AllowSetTruncatedValue = false;
        }

        /// <summary>
        /// Constructs a new Property object
        /// </summary>
        /// <param name="propertyKey">
        /// </param>
        /// <param name="description">
        /// </param>
        /// <param name="propertyStore">
        /// </param>
        internal ShellProperty(PropertyKey propertyKey, ShellPropertyDescription description, IPropertyStore propertyStore)
        {
            this.propertyKey = propertyKey;
            this.Description = description;
            this.NativePropertyStore = propertyStore;
            this.AllowSetTruncatedValue = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value that determines if a value can be truncated. The default for this property is false.
        /// </summary>
        /// <remarks>
        ///   An <see cref = "ArgumentOutOfRangeException" /> will be thrown if
        ///   this property is not set to true, and a property value was set
        ///   but later truncated.
        /// </remarks>
        public bool AllowSetTruncatedValue { get; set; }

        /// <summary>
        ///   Gets the case-sensitive name of a property as it is known to the system,
        ///   regardless of its localized name.
        /// </summary>
        public string CanonicalName
        {
            get
            {
                return this.Description.CanonicalName;
            }
        }

        /// <summary>
        ///   Get the property description object.
        /// </summary>
        public ShellPropertyDescription Description { get; private set; }

        /// <summary>
        ///   Gets the image reference path and icon index associated with a property value (Windows 7 only).
        /// </summary>
        public IconReference IconReference
        {
            get
            {
                if (!CoreHelpers.RunningOnWin7)
                {
                    throw new PlatformNotSupportedException("This Property is available on Windows 7 only.");
                }

                this.GetImageReference();
                var index = this.imageReferenceIconIndex.HasValue ? this.imageReferenceIconIndex.Value : -1;

                return new IconReference(this.imageReferencePath, index);
            }
        }

        /// <summary>
        ///   Gets the property key identifying this property.
        /// </summary>
        public PropertyKey PropertyKey
        {
            get
            {
                return this.propertyKey;
            }
        }

        /// <summary>
        ///   Gets or sets the strongly-typed value of this property.
        ///   The value of the property is cleared if the value is set to null.
        /// </summary>
        /// <exception cref = "System.Runtime.InteropServices.COMException">
        ///   If the property value cannot be retrieved or updated in the Property System</exception>
        /// <exception cref = "NotSupportedException">If the type of this property is not supported; e.g. writing a binary object.</exception>
        /// <exception cref = "ArgumentOutOfRangeException">Thrown if <see cref = "AllowSetTruncatedValue" /> is false, and either 
        ///   a string value was truncated or a numeric value was rounded.</exception>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", 
            Justification = "We are not currently handling globalization or localization")]
        public T Value
        {
            get
            {
                // Make sure we load the correct type
                Debug.Assert(this.ValueType == ShellPropertyDescription.VarEnumToSystemType(this.Description.VarEnumType));

                var propVar = new PropVariant();

                if (this.ParentShellObject.NativePropertyStore != null)
                {
                    // If there is a valid property store for this shell object, then use it.
                    this.ParentShellObject.NativePropertyStore.GetValue(ref this.propertyKey, out propVar);
                }
                else if (this.ParentShellObject != null)
                {
                    // Use IShellItem2.GetProperty instead of creating a new property store
                    // The file might be locked. This is probably quicker, and sufficient for what we need
                    this.ParentShellObject.NativeShellItem2.GetProperty(ref this.propertyKey, out propVar);
                }
                else if (this.NativePropertyStore != null)
                {
                    this.NativePropertyStore.GetValue(ref this.propertyKey, out propVar);
                }

                // Get the value
                T value;
                try
                {
                    value = (T)propVar.Value;
                }
                finally
                {
                    propVar.Clear();
                }

                return value;
            }

            set
            {
                // Make sure we use the correct type
                Debug.Assert(this.ValueType == ShellPropertyDescription.VarEnumToSystemType(this.Description.VarEnumType));

                if (typeof(T) != this.ValueType)
                {
                    throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, "This property only accepts a value of type \"{0}\".", this.ValueType.Name));
                }

                if (value is Nullable)
                {
                    var t = typeof(T);
                    var pi = t.GetProperty("HasValue");
                    if (pi != null)
                    {
                        var hasValue = (bool)pi.GetValue(value, null);
                        if (!hasValue)
                        {
                            this.ClearValue();
                            return;
                        }
                    }
                }
                else if (value == null)
                {
                    this.ClearValue();
                    return;
                }

                if (this.ParentShellObject != null)
                {
                    using (var propertyWriter = this.ParentShellObject.Properties.GetPropertyWriter()) propertyWriter.WriteProperty(this, value, this.AllowSetTruncatedValue);
                }
                else if (this.NativePropertyStore != null)
                {
                    throw new InvalidOperationException(
                        "The value on this property cannot be set. To set the property value, use the ShellObject that is associated with this property.");
                }
            }
        }

        /// <summary>
        ///   Gets the value for this property using the generic Object type.
        ///   To obtain a specific type for this value, use the more type strong
        ///   Property&lt;T&gt; class.
        ///   Also, you can only set a value for this type using Property&lt;T&gt;
        /// </summary>
        public object ValueAsObject
        {
            get
            {
                var propVar = new PropVariant();
                propVar.Clear();

                if (this.ParentShellObject != null)
                {
                    var store = ShellPropertyCollection.CreateDefaultPropertyStore(this.ParentShellObject);

                    store.GetValue(ref this.propertyKey, out propVar);

                    Marshal.ReleaseComObject(store);
                }
                else if (this.NativePropertyStore != null)
                {
                    this.NativePropertyStore.GetValue(ref this.propertyKey, out propVar);
                }

                object value;

                try
                {
                    value = propVar.Value;
                }
                finally
                {
                    propVar.Clear();
                }

                return value;
            }
        }

        /// <summary>
        ///   Gets the associated runtime type.
        /// </summary>
        public Type ValueType
        {
            get
            {
                // The type for this object need to match that of the description
                Debug.Assert(this.Description.ValueType == typeof(T));

                return this.Description.ValueType;
            }
        }

        /// <summary>
        /// </summary>
        private IPropertyStore NativePropertyStore { get; set; }

        /// <summary>
        /// </summary>
        private ShellObject ParentShellObject { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the value of the property.
        /// </summary>
        public void ClearValue()
        {
            var propVar = new PropVariant();
            propVar.SetEmptyValue();

            this.StorePropVariantValue(propVar);
        }

        /// <summary>
        /// Returns a formatted, Unicode string representation of a property value.
        /// </summary>
        /// <param name="format">
        /// One or more of the PropertyDescriptionFormat flags 
        ///   that indicate the desired format.
        /// </param>
        /// <param name="formatted">
        /// The formatted value as a string, or null if this property 
        ///   cannot be formatted for display.
        /// </param>
        /// <returns>
        /// True if the method successfully locates the formatted string; otherwise 
        ///   False.
        /// </returns>
        public bool TryFormatForDisplay(PropertyDescriptionFormat format, out string formatted)
        {
            PropVariant propVar;

            if (this.Description == null || this.Description.NativePropertyDescription == null)
            {
                // We cannot do anything without a property description
                formatted = null;
                return false;
            }

            var store = ShellPropertyCollection.CreateDefaultPropertyStore(this.ParentShellObject);

            store.GetValue(ref this.propertyKey, out propVar);

            // Release the Propertystore
            Marshal.ReleaseComObject(store);

            var hr = this.Description.NativePropertyDescription.FormatForDisplay(ref propVar, ref format, out formatted);

            // Sometimes, the value cannot be displayed properly, such as for blobs
            // or if we get argument exception
            if (!CoreErrorHelper.Succeeded((int)hr))
            {
                formatted = null;
                return false;
            }

            return true;
        }

        #endregion

        #region Implemented Interfaces

        #region IShellProperty

        /// <summary>
        /// Returns a formatted, Unicode string representation of a property value.
        /// </summary>
        /// <param name="format">
        /// One or more of the PropertyDescriptionFormat flags 
        ///   that indicate the desired format.
        /// </param>
        /// <returns>
        /// The formatted value as a string, or null if this property 
        ///   cannot be formatted for display.
        /// </returns>
        public string FormatForDisplay(PropertyDescriptionFormat format)
        {
            string formattedString;
            PropVariant propVar;

            if (this.Description == null || this.Description.NativePropertyDescription == null)
            {
                // We cannot do anything without a property description
                return null;
            }

            var store = ShellPropertyCollection.CreateDefaultPropertyStore(this.ParentShellObject);

            store.GetValue(ref this.propertyKey, out propVar);

            // Release the Propertystore
            Marshal.ReleaseComObject(store);

            var hr = this.Description.NativePropertyDescription.FormatForDisplay(ref propVar, ref format, out formattedString);

            // Sometimes, the value cannot be displayed properly, such as for blobs
            // or if we get argument exception
            if (!CoreErrorHelper.Succeeded((int)hr))
            {
                throw Marshal.GetExceptionForHR((int)hr);
            }

            return formattedString;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        private void GetImageReference()
        {
            PropVariant propVar;

            var store = ShellPropertyCollection.CreateDefaultPropertyStore(this.ParentShellObject);

            store.GetValue(ref this.propertyKey, out propVar);

            Marshal.ReleaseComObject(store);

            string refPath;
            ((IPropertyDescription2)this.Description.NativePropertyDescription).GetImageReferenceForValue(ref propVar, out refPath);

            if (refPath == null)
            {
                return;
            }

            var index = ShellNativeMethods.PathParseIconLocation(ref refPath);
            if (refPath == null)
            {
                return;
            }

            this.imageReferencePath = refPath;
            this.imageReferenceIconIndex = index;
        }

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <exception cref="ExternalException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref="ExternalException">
        /// </exception>
        /// <exception cref="ExternalException">
        /// </exception>
        /// <exception cref="ExternalException">
        /// </exception>
        private void StorePropVariantValue(PropVariant propVar)
        {
            var guid = new Guid(ShellIidGuid.IPropertyStore);
            IPropertyStore writablePropStore = null;
            try
            {
                var hr = this.ParentShellObject.NativeShellItem2.GetPropertyStore(ShellNativeMethods.GETPROPERTYSTOREFLAGSs.GpsReadwrite, ref guid, out writablePropStore);

                if (!CoreErrorHelper.Succeeded(hr))
                {
                    throw new ExternalException("Unable to get writable property store for this property.", Marshal.GetExceptionForHR(hr));
                }

                var result = writablePropStore.SetValue(ref this.propertyKey, ref propVar);

                if (!this.AllowSetTruncatedValue && (result == InPlaceStringTruncated))
                {
                    throw new ArgumentOutOfRangeException(
                        "propVar", "A value had to be truncated in a string or rounded if a numeric value. Set AllowTruncatedValue to true to prevent this exception.");
                }

                if (!CoreErrorHelper.Succeeded(result))
                {
                    throw new ExternalException("Unable to set property.", Marshal.GetExceptionForHR(result));
                }

                writablePropStore.Commit();
            }
            catch (InvalidComObjectException e)
            {
                throw new ExternalException("Unable to get writable property store for this property.", e);
            }
            catch (InvalidCastException)
            {
                throw new ExternalException("Unable to get writable property store for this property.");
            }
            finally
            {
                if (writablePropStore != null)
                {
                    Marshal.ReleaseComObject(writablePropStore);
                }
            }
        }

        #endregion
    }
}