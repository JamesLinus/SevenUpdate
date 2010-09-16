#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Shell.PropertySystem
{
    /// <summary>
    ///   Creates a readonly collection of IProperty objects.
    /// </summary>
    public class ShellPropertyCollection : ReadOnlyCollection<IShellProperty>, IDisposable
    {
        #region Internal Constructor

        /// <summary>
        ///   Creates a new Property collection given an IPropertyStore object
        /// </summary>
        /// <param name = "nativePropertyStore">IPropertyStore</param>
        internal ShellPropertyCollection(IPropertyStore nativePropertyStore) : base(new List<IShellProperty>())
        {
            NativePropertyStore = nativePropertyStore;
            AddProperties(nativePropertyStore);
        }

        /// <summary>
        ///   Creates a new Property collection given an IShellItem2 native interface
        /// </summary>
        /// <param name = "parent">Parent ShellObject</param>
        internal ShellPropertyCollection(ShellObject parent) : base(new List<IShellProperty>())
        {
            ParentShellObject = parent;
            IPropertyStore nativePropertyStore = null;
            try
            {
                nativePropertyStore = CreateDefaultPropertyStore(ParentShellObject);
                AddProperties(nativePropertyStore);
            }
            finally
            {
                if (nativePropertyStore != null)
                    Marshal.ReleaseComObject(nativePropertyStore);
            }
        }

        #endregion

        #region Public Constructor

        /// <summary>
        ///   Creates a new <c>ShellPropertyCollection</c> object with the specified file or folder path.
        /// </summary>
        /// <param name = "path">The path to the file or folder.</param>
        public ShellPropertyCollection(string path) : this(ShellObjectFactory.Create(path))
        {
        }

        #endregion

        #region Private Methods

        private ShellObject ParentShellObject { get; set; }

        private IPropertyStore NativePropertyStore { get; set; }

        private void AddProperties(IPropertyStore nativePropertyStore)
        {
            uint propertyCount;

            // Populate the property collection
            nativePropertyStore.GetCount(out propertyCount);
            for (uint i = 0; i < propertyCount; i++)
            {
                PropertyKey propKey;
                nativePropertyStore.GetAt(i, out propKey);

                Items.Add(ParentShellObject != null ? ParentShellObject.Properties.CreateTypedProperty(propKey) : CreateTypedProperty(propKey, NativePropertyStore));
            }
        }

        internal static IPropertyStore CreateDefaultPropertyStore(ShellObject shellObj)
        {
            IPropertyStore nativePropertyStore;

            var guid = new Guid(ShellIIDGuid.IPropertyStore);
            var hr = shellObj.NativeShellItem2.GetPropertyStore(ShellNativeMethods.GETPROPERTYSTOREFLAGS.GPS_BESTEFFORT, ref guid, out nativePropertyStore);

            // throw on failure 
            if (nativePropertyStore == null || !CoreErrorHelper.Succeeded(hr))
                Marshal.ThrowExceptionForHR(hr);

            return nativePropertyStore;
        }

        #endregion

        #region Collection Public Methods

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
                    throw new ArgumentException("Argument CanonicalName cannot be null or empty.", "canonicalName");

                var props = Items.Where(p => p.CanonicalName == canonicalName).ToArray();

                if (props.Length > 0)
                    return props[0];

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
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "We need the ability to get item from the collection using a property key")]
        public IShellProperty this[PropertyKey key]
        {
            get
            {
                var props = Items.Where(p => p.PropertyKey == key).ToArray();

                if (props.Length > 0)
                    return props[0];

                throw new IndexOutOfRangeException("This PropertyKey is not a valid index.");
            }
        }

        /// <summary>
        ///   Checks if a property with the given canonical name is available.
        /// </summary>
        /// <param name = "canonicalName">The canonical name of the property.</param>
        /// <returns><B>True</B> if available, <B>false</B> otherwise.</returns>
        public bool Contains(string canonicalName)
        {
            if (string.IsNullOrEmpty(canonicalName))
                throw new ArgumentException("Argument CanonicalName cannot be null or empty.", "canonicalName");

            return Items.Where(p => p.CanonicalName == canonicalName).Count() > 0;
        }

        /// <summary>
        ///   Checks if a property with the given property key is available.
        /// </summary>
        /// <param name = "key">The property key.</param>
        /// <returns><B>True</B> if available, <B>false</B> otherwise.</returns>
        public bool Contains(PropertyKey key)
        {
            return Items.Where(p => p.PropertyKey == key).Count() > 0;
        }

        #endregion

        // TODO - ShellProperties.cs also has a similar class that is used for creating
        // a ShellObject specific IShellProperty. These 2 methods should be combined or moved to a 
        // common location.

        #region IDisposable Members

        /// <summary>
        ///   Release the native objects.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal static IShellProperty CreateTypedProperty(PropertyKey propKey, IPropertyStore NativePropertyStore)
        {
            var desc = ShellPropertyDescriptionsCache.Cache.GetPropertyDescription(propKey);

            switch (desc.VarEnumType)
            {
                case (VarEnum.VT_EMPTY):
                case (VarEnum.VT_NULL):
                    {
                        return (new ShellProperty<Object>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_UI1):
                    {
                        return (new ShellProperty<Byte?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_I2):
                    {
                        return (new ShellProperty<Int16?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_UI2):
                    {
                        return (new ShellProperty<UInt16?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_I4):
                    {
                        return (new ShellProperty<Int32?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_UI4):
                    {
                        return (new ShellProperty<UInt32?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_I8):
                    {
                        return (new ShellProperty<Int64?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_UI8):
                    {
                        return (new ShellProperty<UInt64?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_R8):
                    {
                        return (new ShellProperty<Double?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_BOOL):
                    {
                        return (new ShellProperty<Boolean?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_FILETIME):
                    {
                        return (new ShellProperty<DateTime?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_CLSID):
                    {
                        return (new ShellProperty<IntPtr?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_CF):
                    {
                        return (new ShellProperty<IntPtr?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_BLOB):
                    {
                        return (new ShellProperty<Byte[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_LPWSTR):
                    {
                        return (new ShellProperty<String>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_UNKNOWN):
                    {
                        return (new ShellProperty<IntPtr?>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_STREAM):
                    {
                        return (new ShellProperty<IStream>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_UI1):
                    {
                        return (new ShellProperty<Byte[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_I2):
                    {
                        return (new ShellProperty<Int16[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_UI2):
                    {
                        return (new ShellProperty<UInt16[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_I4):
                    {
                        return (new ShellProperty<Int32[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_UI4):
                    {
                        return (new ShellProperty<UInt32[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_I8):
                    {
                        return (new ShellProperty<Int64[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_UI8):
                    {
                        return (new ShellProperty<UInt64[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_R8):
                    {
                        return (new ShellProperty<Double[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_BOOL):
                    {
                        return (new ShellProperty<Boolean[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_FILETIME):
                    {
                        return (new ShellProperty<DateTime[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_CLSID):
                    {
                        return (new ShellProperty<IntPtr[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_CF):
                    {
                        return (new ShellProperty<IntPtr[]>(propKey, desc, NativePropertyStore));
                    }
                case (VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR):
                    {
                        return (new ShellProperty<String[]>(propKey, desc, NativePropertyStore));
                    }
                default:
                    {
                        return (new ShellProperty<Object>(propKey, desc, NativePropertyStore));
                    }
            }
        }

        /// <summary>
        ///   Release the native and managed objects
        /// </summary>
        /// <param name = "disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (NativePropertyStore == null)
                return;
            Marshal.ReleaseComObject(NativePropertyStore);
            NativePropertyStore = null;
        }

        /// <summary>
        ///   Implement the finalizer.
        /// </summary>
        ~ShellPropertyCollection()
        {
            Dispose(false);
        }
    }
}