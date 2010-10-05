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
    using global::System.Runtime.InteropServices;
    using global::System.Runtime.InteropServices.ComTypes;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Defines a partial class that implements helper methods for retrieving Shell properties
    ///   using a canonical name, property key, or a strongly-typed property. Also provides
    ///   access to all the strongly-typed system properties and default properties collections.
    /// </summary>
    public partial class ShellProperties
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private ShellPropertyCollection defaultPropertyCollection;

        /// <summary>
        /// </summary>
        private PropertySystem propertySystem;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="parent">
        /// </param>
        internal ShellProperties(ShellObject parent)
        {
            this.ParentShellObject = parent;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the collection of all the default properties for this item.
        /// </summary>
        public ShellPropertyCollection DefaultPropertyCollection
        {
            get
            {
                return this.defaultPropertyCollection ?? (this.defaultPropertyCollection = new ShellPropertyCollection(this.ParentShellObject));
            }
        }

        /// <summary>
        ///   Gets all the properties for the system through an accessor.
        /// </summary>
        public PropertySystem System
        {
            get
            {
                return this.propertySystem ?? (this.propertySystem = new PropertySystem(this.ParentShellObject));
            }
        }

        /// <summary>
        /// </summary>
        private ShellObject ParentShellObject { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a property available in the default property collection using 
        ///   the given property key.
        /// </summary>
        /// <param name="key">
        /// The property key.
        /// </param>
        /// <returns>
        /// An IShellProperty.
        /// </returns>
        public IShellProperty GetProperty(PropertyKey key)
        {
            return CreateTypedProperty(key);
        }

        /// <summary>
        /// Returns a property available in the default property collection using 
        ///   the given canonical name.
        /// </summary>
        /// <param name="canonicalName">
        /// The canonical name.
        /// </param>
        /// <returns>
        /// An IShellProperty.
        /// </returns>
        public IShellProperty GetProperty(string canonicalName)
        {
            return CreateTypedProperty(canonicalName);
        }

        /// <summary>
        /// Returns a strongly typed property available in the default property collection using 
        ///   the given property key.
        /// </summary>
        /// <typeparam name="T">
        /// The type of property to retrieve.
        /// </typeparam>
        /// <param name="key">
        /// The property key.
        /// </param>
        /// <returns>
        /// A strongly-typed ShellProperty for the given property key.
        /// </returns>
        public ShellProperty<T> GetProperty<T>(PropertyKey key)
        {
            return CreateTypedProperty(key) as ShellProperty<T>;
        }

        /// <summary>
        /// Returns a strongly typed property available in the default property collection using 
        ///   the given canonical name.
        /// </summary>
        /// <typeparam name="T">
        /// The type of property to retrieve.
        /// </typeparam>
        /// <param name="canonicalName">
        /// The canonical name.
        /// </param>
        /// <returns>
        /// A strongly-typed ShellProperty for the given canonical name.
        /// </returns>
        public ShellProperty<T> GetProperty<T>(string canonicalName)
        {
            return CreateTypedProperty(canonicalName) as ShellProperty<T>;
        }

        /// <summary>
        /// Returns the shell property writer used when writing multiple properties.
        /// </summary>
        /// <returns>
        /// A ShellPropertyWriter.
        /// </returns>
        /// <remarks>
        /// Use the Using pattern with the returned ShellPropertyWriter or
        ///   manually call the Close method on the writer to commit the changes 
        ///   and dispose the writer
        /// </remarks>
        public ShellPropertyWriter GetPropertyWriter()
        {
            return new ShellPropertyWriter(this.ParentShellObject);
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="propKey">
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        internal IShellProperty CreateTypedProperty<T>(PropertyKey propKey)
        {
            var desc = ShellPropertyDescriptionsCache.Cache.GetPropertyDescription(propKey);
            return new ShellProperty<T>(propKey, desc, this.ParentShellObject);
        }

        /// <summary>
        /// </summary>
        /// <param name="propKey">
        /// </param>
        /// <returns>
        /// </returns>
        internal IShellProperty CreateTypedProperty(PropertyKey propKey)
        {
            var desc = ShellPropertyDescriptionsCache.Cache.GetPropertyDescription(propKey);

            switch (desc.VarEnumType)
            {
                case VarEnum.VT_EMPTY:
                case VarEnum.VT_NULL:
                    {
                        return new ShellProperty<object>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_UI1:
                    {
                        return new ShellProperty<byte?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_I2:
                    {
                        return new ShellProperty<short?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_UI2:
                    {
                        return new ShellProperty<ushort?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_I4:
                    {
                        return new ShellProperty<int?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_UI4:
                    {
                        return new ShellProperty<uint?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_I8:
                    {
                        return new ShellProperty<long?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_UI8:
                    {
                        return new ShellProperty<ulong?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_R8:
                    {
                        return new ShellProperty<double?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_BOOL:
                    {
                        return new ShellProperty<bool?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_FILETIME:
                    {
                        return new ShellProperty<DateTime?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_CLSID:
                    {
                        return new ShellProperty<IntPtr?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_CF:
                    {
                        return new ShellProperty<IntPtr?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_BLOB:
                    {
                        return new ShellProperty<byte[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_LPWSTR:
                    {
                        return new ShellProperty<string>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_UNKNOWN:
                    {
                        return new ShellProperty<IntPtr?>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_STREAM:
                    {
                        return new ShellProperty<IStream>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI1:
                    {
                        return new ShellProperty<byte[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_I2:
                    {
                        return new ShellProperty<short[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI2:
                    {
                        return new ShellProperty<ushort[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_I4:
                    {
                        return new ShellProperty<int[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI4:
                    {
                        return new ShellProperty<uint[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_I8:
                    {
                        return new ShellProperty<long[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI8:
                    {
                        return new ShellProperty<ulong[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_R8:
                    {
                        return new ShellProperty<double[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_BOOL:
                    {
                        return new ShellProperty<bool[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_FILETIME:
                    {
                        return new ShellProperty<DateTime[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_CLSID:
                    {
                        return new ShellProperty<IntPtr[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_CF:
                    {
                        return new ShellProperty<IntPtr[]>(propKey, desc, this.ParentShellObject);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR:
                    {
                        return new ShellProperty<string[]>(propKey, desc, this.ParentShellObject);
                    }

                default:
                    {
                        return new ShellProperty<object>(propKey, desc, this.ParentShellObject);
                    }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="canonicalName">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal IShellProperty CreateTypedProperty(string canonicalName)
        {
            // Otherwise, call the native PropertyStore method
            PropertyKey propKey;

            var result = PropertySystemNativeMethods.PSGetPropertyKeyFromName(canonicalName, out propKey);

            if (!CoreErrorHelper.Succeeded(result))
            {
                throw new ArgumentException("This CanonicalName is not valid", Marshal.GetExceptionForHR(result));
            }

            return CreateTypedProperty(propKey);
        }

        #endregion
    }
}