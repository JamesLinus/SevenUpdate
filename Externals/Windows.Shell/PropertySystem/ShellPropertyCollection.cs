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
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using global::System.Runtime.InteropServices.ComTypes;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Creates a readonly collection of IProperty objects.
    /// </summary>
    public class ShellPropertyCollection : ReadOnlyCollection<IShellProperty>, IDisposable
    {
        #region Constructors and Destructors

        /// <summary>
        /// Creates a new <c>ShellPropertyCollection</c> object with the specified file or folder path.
        /// </summary>
        /// <param name="path">
        /// The path to the file or folder.
        /// </param>
        public ShellPropertyCollection(string path)
            : this(ShellObjectFactory.Create(path))
        {
        }

        /// <summary>
        /// Creates a new Property collection given an IPropertyStore object
        /// </summary>
        /// <param name="nativePropertyStore">
        /// IPropertyStore
        /// </param>
        internal ShellPropertyCollection(IPropertyStore nativePropertyStore)
            : base(new List<IShellProperty>())
        {
            this.NativePropertyStore = nativePropertyStore;
            this.AddProperties(nativePropertyStore);
        }

        /// <summary>
        /// Creates a new Property collection given an IShellItem2 native interface
        /// </summary>
        /// <param name="parent">
        /// Parent ShellObject
        /// </param>
        internal ShellPropertyCollection(ShellObject parent)
            : base(new List<IShellProperty>())
        {
            this.ParentShellObject = parent;
            IPropertyStore nativePropertyStore = null;
            try
            {
                nativePropertyStore = CreateDefaultPropertyStore(this.ParentShellObject);
                this.AddProperties(nativePropertyStore);
            }
            finally
            {
                if (nativePropertyStore != null)
                {
                    Marshal.ReleaseComObject(nativePropertyStore);
                }
            }
        }

        /// <summary>
        /// 
        ///   Implement the finalizer.
        /// </summary>
        ~ShellPropertyCollection()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        private IPropertyStore NativePropertyStore { get; set; }

        /// <summary>
        /// </summary>
        private ShellObject ParentShellObject { get; set; }

        #endregion

        #region Indexers

        /// <summary>
        ///   Gets the property associated with the supplied canonical name string.
        ///   The canonical name property is case-sensitive.
        /// </summary>
        /// <param name = "canonicalName">The canonical name.</param>
        /// <returns>The property associated with the canonical name, if found.</returns>
        /// <exception cref = "IndexOutOfRangeException">Throws IndexOutOfRangeException 
        ///   if no matching property is found.</exception>
        public IShellProperty this[string canonicalName]
        {
            get
            {
                if (string.IsNullOrEmpty(canonicalName))
                {
                    throw new ArgumentException("Argument CanonicalName cannot be null or empty.", "canonicalName");
                }

                var props = this.Items.Where(p => p.CanonicalName == canonicalName).ToArray();

                if (props.Length > 0)
                {
                    return props[0];
                }

                throw new IndexOutOfRangeException("This CanonicalName is not a valid index.");
            }
        }

        /// <summary>
        ///   Gets a property associated with the supplied property key.
        /// </summary>
        /// <param name = "key">The property key.</param>
        /// <returns>The property associated with the property key, if found.</returns>
        /// <exception cref = "IndexOutOfRangeException">Throws IndexOutOfRangeException 
        ///   if no matching property is found.</exception>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", 
            Justification = "We need the ability to get item from the collection using a property key")]
        public IShellProperty this[PropertyKey key]
        {
            get
            {
                var props = this.Items.Where(p => p.PropertyKey == key).ToArray();

                if (props.Length > 0)
                {
                    return props[0];
                }

                throw new IndexOutOfRangeException("This PropertyKey is not a valid index.");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if a property with the given canonical name is available.
        /// </summary>
        /// <param name="canonicalName">
        /// The canonical name of the property.
        /// </param>
        /// <returns>
        /// <B>True</B> if available, <B>false</B> otherwise.
        /// </returns>
        public bool Contains(string canonicalName)
        {
            if (string.IsNullOrEmpty(canonicalName))
            {
                throw new ArgumentException("Argument CanonicalName cannot be null or empty.", "canonicalName");
            }

            return this.Items.Where(p => p.CanonicalName == canonicalName).Count() > 0;
        }

        /// <summary>
        /// Checks if a property with the given property key is available.
        /// </summary>
        /// <param name="key">
        /// The property key.
        /// </param>
        /// <returns>
        /// <B>True</B> if available, <B>false</B> otherwise.
        /// </returns>
        public bool Contains(PropertyKey key)
        {
            return this.Items.Where(p => p.PropertyKey == key).Count() > 0;
        }

        #endregion

        // TODO - ShellProperties.cs also has a similar class that is used for creating
        // a ShellObject specific IShellProperty. These 2 methods should be combined or moved to a 
        // common location.
        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Release the native objects.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="shellObj">
        /// </param>
        /// <returns>
        /// </returns>
        internal static IPropertyStore CreateDefaultPropertyStore(ShellObject shellObj)
        {
            IPropertyStore nativePropertyStore;

            var guid = new Guid(ShellIidGuid.IPropertyStore);
            var hr = shellObj.NativeShellItem2.GetPropertyStore(ShellNativeMethods.GETPROPERTYSTOREFLAGSs.GpsBesteffort, ref guid, out nativePropertyStore);

            // throw on failure 
            if (nativePropertyStore == null || !CoreErrorHelper.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return nativePropertyStore;
        }

        /// <summary>
        /// </summary>
        /// <param name="propKey">
        /// </param>
        /// <param name="nativePropertyStore">
        /// </param>
        /// <returns>
        /// </returns>
        internal static IShellProperty CreateTypedProperty(PropertyKey propKey, IPropertyStore nativePropertyStore)
        {
            var desc = ShellPropertyDescriptionsCache.Cache.GetPropertyDescription(propKey);

            switch (desc.VarEnumType)
            {
                case VarEnum.VT_EMPTY:
                case VarEnum.VT_NULL:
                    {
                        return new ShellProperty<object>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_UI1:
                    {
                        return new ShellProperty<byte?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_I2:
                    {
                        return new ShellProperty<short?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_UI2:
                    {
                        return new ShellProperty<ushort?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_I4:
                    {
                        return new ShellProperty<int?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_UI4:
                    {
                        return new ShellProperty<uint?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_I8:
                    {
                        return new ShellProperty<long?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_UI8:
                    {
                        return new ShellProperty<ulong?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_R8:
                    {
                        return new ShellProperty<double?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_BOOL:
                    {
                        return new ShellProperty<bool?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_FILETIME:
                    {
                        return new ShellProperty<DateTime?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_CLSID:
                    {
                        return new ShellProperty<IntPtr?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_CF:
                    {
                        return new ShellProperty<IntPtr?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_BLOB:
                    {
                        return new ShellProperty<byte[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_LPWSTR:
                    {
                        return new ShellProperty<string>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_UNKNOWN:
                    {
                        return new ShellProperty<IntPtr?>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_STREAM:
                    {
                        return new ShellProperty<IStream>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI1:
                    {
                        return new ShellProperty<byte[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_I2:
                    {
                        return new ShellProperty<short[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI2:
                    {
                        return new ShellProperty<ushort[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_I4:
                    {
                        return new ShellProperty<int[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI4:
                    {
                        return new ShellProperty<uint[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_I8:
                    {
                        return new ShellProperty<long[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_UI8:
                    {
                        return new ShellProperty<ulong[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_R8:
                    {
                        return new ShellProperty<double[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_BOOL:
                    {
                        return new ShellProperty<bool[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_FILETIME:
                    {
                        return new ShellProperty<DateTime[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_CLSID:
                    {
                        return new ShellProperty<IntPtr[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_CF:
                    {
                        return new ShellProperty<IntPtr[]>(propKey, desc, nativePropertyStore);
                    }

                case VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR:
                    {
                        return new ShellProperty<string[]>(propKey, desc, nativePropertyStore);
                    }

                default:
                    {
                        return new ShellProperty<object>(propKey, desc, nativePropertyStore);
                    }
            }
        }

        /// <summary>
        /// Release the native and managed objects
        /// </summary>
        /// <param name="disposing">
        /// Indicates that this is being called from Dispose(), rather than the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.NativePropertyStore == null)
            {
                return;
            }

            Marshal.ReleaseComObject(this.NativePropertyStore);
            this.NativePropertyStore = null;
        }

        /// <summary>
        /// </summary>
        /// <param name="nativePropertyStore">
        /// </param>
        private void AddProperties(IPropertyStore nativePropertyStore)
        {
            uint propertyCount;

            // Populate the property collection
            nativePropertyStore.GetCount(out propertyCount);
            for (uint i = 0; i < propertyCount; i++)
            {
                PropertyKey propKey;
                nativePropertyStore.GetAt(i, out propKey);

                this.Items.Add(
                    this.ParentShellObject != null
                        ? this.ParentShellObject.Properties.CreateTypedProperty(propKey)
                        : CreateTypedProperty(propKey, this.NativePropertyStore));
            }
        }

        #endregion
    }
}