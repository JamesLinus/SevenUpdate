//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Windows.Internal;
using Microsoft.Windows.Shell.PropertySystem;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   The base class for all Shell objects in Shell Namespace.
    /// </summary>
    public abstract class ShellObject : IDisposable, IEquatable<ShellObject>
    {
        #region Public Static Methods

        /// <summary>
        ///   Indicates whether this feature is supported on the current platform.
        /// </summary>
        public static bool IsPlatformSupported
        {
            get
            {
                // We need Windows Vista onwards ...
                return CoreHelpers.RunningOnVista;
            }
        }

        /// <summary>
        ///   Creates a ShellObject subclass given a parsing name.
        ///   For file system items, this method will only accept absolute paths.
        /// </summary>
        /// <param name = "parsingName">The parsing name of the object.</param>
        /// <returns>A newly constructed ShellObject object.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public static ShellObject FromParsingName(string parsingName)
        {
            return ShellObjectFactory.Create(parsingName);
        }

        #endregion

        #region Internal Fields

        /// <summary>
        ///   Internal member to keep track of the native IShellItem2
        /// </summary>
        internal IShellItem2 nativeShellItem;

        #endregion

        #region Constructors

        internal ShellObject()
        {
        }

        internal ShellObject(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }

        #endregion

        #region Protected Fields

        /// <summary>
        ///   A friendly name for this object that' suitable for display
        /// </summary>
        private string internalName;

        /// <summary>
        ///   PID List (PIDL) for this object
        /// </summary>
        private IntPtr internalPIDL = IntPtr.Zero;

        /// <summary>
        ///   Parsing name for this Object e.g. c:\Windows\file.txt,
        ///   or ::{Some Guid}
        /// </summary>
        private string internalParsingName;

        #endregion

        #region Internal Properties

        /// <summary>
        ///   Return the native ShellFolder object as newer IShellItem2
        /// </summary>
        /// <exception cref = "System.Runtime.InteropServices.ExternalException">If the native object cannot be created.
        ///   The ErrorCode member will contain the external error code.</exception>
        internal virtual IShellItem2 NativeShellItem2
        {
            get
            {
                if (nativeShellItem == null && ParsingName != null)
                {
                    var guid = new Guid(ShellIIDGuid.IShellItem2);
                    int retCode = ShellNativeMethods.SHCreateItemFromParsingName(ParsingName, IntPtr.Zero, ref guid, out nativeShellItem);

                    if (nativeShellItem == null || !CoreErrorHelper.Succeeded(retCode))
                        throw new ExternalException("Shell item could not be created.", Marshal.GetExceptionForHR(retCode));
                }
                return nativeShellItem;
            }
        }

        /// <summary>
        ///   Return the native ShellFolder object
        /// </summary>
        internal virtual IShellItem NativeShellItem { get { return NativeShellItem2; } }

        /// <summary>
        ///   Gets access to the native IPropertyStore (if one is already
        ///   created for this item and still valid. This is usually done by the
        ///   ShellPropertyWriter class. The reference will be set to null
        ///   when the writer has been closed/commited).
        /// </summary>
        internal IPropertyStore NativePropertyStore { get; set; }

        #endregion

        #region Public Properties

        private ShellObject parentShellObject;
        private ShellProperties properties;
        private ShellThumbnail thumbnail;

        /// <summary>
        ///   Gets an object that allows the manipulation of ShellProperties for this shell item.
        /// </summary>
        public ShellProperties Properties { get { return properties ?? (properties = new ShellProperties(this)); } }

        /// <summary>
        ///   Gets the parsing name for this ShellItem.
        /// </summary>
        public virtual string ParsingName
        {
            get
            {
                if (internalParsingName == null && nativeShellItem != null)
                    internalParsingName = ShellHelper.GetParsingName(nativeShellItem);
                return internalParsingName;
            }
            protected set { internalParsingName = value; }
        }

        /// <summary>
        ///   Gets the normal display for this ShellItem.
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (internalName == null && NativeShellItem != null)
                {
                    IntPtr pszString;
                    HRESULT hr = NativeShellItem.GetDisplayName(ShellNativeMethods.SIGDN.SIGDN_NORMALDISPLAY, out pszString);
                    if (hr == HRESULT.S_OK && pszString != IntPtr.Zero)
                    {
                        internalName = Marshal.PtrToStringAuto(pszString);

                        // Free the string
                        Marshal.FreeCoTaskMem(pszString);
                    }
                }
                return internalName;
            }

            protected set { internalName = value; }
        }

        /// <summary>
        ///   Gets the PID List (PIDL) for this ShellItem.
        /// </summary>
        internal virtual IntPtr PIDL
        {
            get
            {
                // Get teh PIDL for the ShellItem
                if (internalPIDL == IntPtr.Zero && NativeShellItem != null)
                    internalPIDL = ShellHelper.PidlFromShellItem(NativeShellItem);

                return internalPIDL;
            }
            set { internalPIDL = value; }
        }

        /// <summary>
        ///   Gets a value that determines if this ShellObject is a link or shortcut.
        /// </summary>
        public bool IsLink
        {
            get
            {
                try
                {
                    ShellNativeMethods.SFGAO sfgao;
                    NativeShellItem.GetAttributes(ShellNativeMethods.SFGAO.SFGAO_LINK, out sfgao);
                    return (sfgao & ShellNativeMethods.SFGAO.SFGAO_LINK) != 0;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (NullReferenceException)
                {
                    // NativeShellItem is null
                    return false;
                }
            }
        }

        /// <summary>
        ///   Gets a value that determines if this ShellObject is a file system object.
        /// </summary>
        public bool IsFileSystemObject
        {
            get
            {
                try
                {
                    ShellNativeMethods.SFGAO sfgao;
                    NativeShellItem.GetAttributes(ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM, out sfgao);
                    return (sfgao & ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM) != 0;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (NullReferenceException)
                {
                    // NativeShellItem is null
                    return false;
                }
            }
        }

        /// <summary>
        ///   Gets the thumbnail of the ShellObject.
        /// </summary>
        public ShellThumbnail Thumbnail { get { return thumbnail ?? (thumbnail = new ShellThumbnail(this)); } }

        /// <summary>
        ///   Gets the parent ShellObject.
        ///   Returns null if the object has no parent, i.e. if this object is the Desktop folder.
        /// </summary>
        public ShellObject Parent
        {
            get
            {
                if (parentShellObject == null && NativeShellItem2 != null)
                {
                    IShellItem parentShellItem;

                    HRESULT hr = NativeShellItem2.GetParent(out parentShellItem);

                    if (hr == HRESULT.S_OK && parentShellItem != null)
                        parentShellObject = ShellObjectFactory.Create(parentShellItem);
                    else if (hr == HRESULT.NO_OBJECT)
                    {
                        // Should return null if the parent is desktop
                        return null;
                    }
                    else
                        throw Marshal.GetExceptionForHR((int) hr);
                }

                return parentShellObject;
            }
        }

        /// <summary>
        ///   Overrides object.ToString()
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///   Returns the display name of the ShellFolder object. DisplayNameType represents one of the 
        ///   values that indicates how the name should look. 
        ///   See <see cref = "Microsoft.Windows.Shell.DisplayNameType" />for a list of possible values.
        /// </summary>
        /// <param name = "displayNameType">A disaply name type.</param>
        /// <returns>A string.</returns>
        public virtual string GetDisplayName(DisplayNameType displayNameType)
        {
            string returnValue = null;

            HRESULT hr = HRESULT.S_OK;

            if (NativeShellItem2 != null)
                hr = NativeShellItem2.GetDisplayName((ShellNativeMethods.SIGDN) displayNameType, out returnValue);

            if (hr != HRESULT.S_OK)
                throw new COMException("Can't get the display name", (int) hr);

            return returnValue;
        }

        #endregion

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

        /// <summary>
        ///   Release the native and managed objects
        /// </summary>
        /// <param name = "disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                internalName = null;
                internalParsingName = null;
                properties = null;
                thumbnail = null;
                parentShellObject = null;
            }

            if (internalPIDL != IntPtr.Zero)
            {
                ShellNativeMethods.ILFree(internalPIDL);
                internalPIDL = IntPtr.Zero;
            }

            if (nativeShellItem != null)
            {
                Marshal.ReleaseComObject(nativeShellItem);
                nativeShellItem = null;
            }

            if (NativePropertyStore != null)
            {
                Marshal.ReleaseComObject(NativePropertyStore);
                NativePropertyStore = null;
            }
        }

        /// <summary>
        ///   Implement the finalizer.
        /// </summary>
        ~ShellObject()
        {
            Dispose(false);
        }

        #region equality and hashing

        private static MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
        private int? hashValue;

        /// <summary>
        ///   Determines if two ShellObjects are identical.
        /// </summary>
        /// <param name = "other">The ShellObject to comare this one to.</param>
        /// <returns>True if the ShellObjects are equal, false otherwise.</returns>
        public bool Equals(ShellObject other)
        {
            bool areEqual = false;

            if ((object) other != null)
            {
                IShellItem ifirst = NativeShellItem;
                IShellItem isecond = other.NativeShellItem;
                if (((ifirst != null) && (isecond != null)))
                {
                    int result;
                    HRESULT hr = ifirst.Compare(isecond, SICHINTF.SICHINT_ALLFIELDS, out result);

                    if ((hr == HRESULT.S_OK) && (result == 0))
                        areEqual = true;
                }
            }

            return areEqual;
        }

        /// <summary>
        ///   Returns the hash code of the object.
        /// </summary>
        /// <returns />
        public override int GetHashCode()
        {
            if (!hashValue.HasValue)
            {
                uint size = ShellNativeMethods.ILGetSize(PIDL);
                if (size != 0)
                {
                    var pidlData = new byte[size];
                    Marshal.Copy(PIDL, pidlData, 0, (int) size);
                    byte[] hashData = hashProvider.ComputeHash(pidlData);
                    hashValue = BitConverter.ToInt32(hashData, 0);
                }
                else
                    hashValue = 0;
            }
            return hashValue.Value;
        }

        /// <summary>
        ///   Returns whether this object is equal to another.
        /// </summary>
        /// <param name = "obj">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ShellObject);
        }

        /// <summary>
        ///   Implements the == (equality) operator.
        /// </summary>
        /// <param name = "a">Object a.</param>
        /// <param name = "b">Object b.</param>
        /// <returns>true if object a equals object b; false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        public static bool operator ==(ShellObject a, ShellObject b)
        {
            return (object) a == null ? (object) b == null : a.Equals(b);
        }

        /// <summary>
        ///   Implements the != (inequality) operator.
        /// </summary>
        /// <param name = "a">Object a.</param>
        /// <param name = "b">Object b.</param>
        /// <returns>true if object a does not equal object b; false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        public static bool operator !=(ShellObject a, ShellObject b)
        {
            return (object) a == null ? (object) b != null : !a.Equals(b);
        }

        #endregion
    }
}