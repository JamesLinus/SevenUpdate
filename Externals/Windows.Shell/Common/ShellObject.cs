//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    using Microsoft.Windows.Internal;
    using Microsoft.Windows.Shell.PropertySystem;

    /// <summary>
    /// The base class for all Shell objects in Shell Namespace.
    /// </summary>
    public abstract class ShellObject : IDisposable, IEquatable<ShellObject>
    {
        #region Constants and Fields

        /// <summary>
        ///   Internal member to keep track of the native IShellItem2
        /// </summary>
        internal IShellItem2 nativeShellItem;

        /// <summary>
        /// </summary>
        private static readonly MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();

        /// <summary>
        /// </summary>
        private int? hashValue;

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

        /// <summary>
        /// </summary>
        private ShellObject parentShellObject;

        /// <summary>
        /// </summary>
        private ShellProperties properties;

        /// <summary>
        /// </summary>
        private ShellThumbnail thumbnail;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        internal ShellObject()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal ShellObject(IShellItem2 shellItem)
        {
            this.nativeShellItem = shellItem;
        }

        /// <summary>
        /// 
        ///   Implement the finalizer.
        /// </summary>
        ~ShellObject()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

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
        ///   Gets a value that determines if this ShellObject is a file system object.
        /// </summary>
        public bool IsFileSystemObject
        {
            get
            {
                try
                {
                    ShellNativeMethods.SFGAOs sfgao;
                    this.NativeShellItem.GetAttributes(ShellNativeMethods.SFGAOs.SfgaoFilesystem, out sfgao);
                    return (sfgao & ShellNativeMethods.SFGAOs.SfgaoFilesystem) != 0;
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
        ///   Gets a value that determines if this ShellObject is a link or shortcut.
        /// </summary>
        public bool IsLink
        {
            get
            {
                try
                {
                    ShellNativeMethods.SFGAOs sfgao;
                    this.NativeShellItem.GetAttributes(ShellNativeMethods.SFGAOs.SfgaoLink, out sfgao);
                    return (sfgao & ShellNativeMethods.SFGAOs.SfgaoLink) != 0;
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
        ///   Gets the normal display for this ShellItem.
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (this.internalName == null && this.NativeShellItem != null)
                {
                    IntPtr pszString;
                    var hr = this.NativeShellItem.GetDisplayName(ShellNativeMethods.SIGDN.Normaldisplay, out pszString);
                    if (hr == HRESULT.S_OK && pszString != IntPtr.Zero)
                    {
                        this.internalName = Marshal.PtrToStringAuto(pszString);

                        // Free the string
                        Marshal.FreeCoTaskMem(pszString);
                    }
                }

                return this.internalName;
            }

            protected set
            {
                this.internalName = value;
            }
        }

        /// <summary>
        ///   Gets the parent ShellObject.
        ///   Returns null if the object has no parent, i.e. if this object is the Desktop folder.
        /// </summary>
        public ShellObject Parent
        {
            get
            {
                if (this.parentShellObject == null && this.NativeShellItem2 != null)
                {
                    IShellItem parentShellItem;

                    var hr = this.NativeShellItem2.GetParent(out parentShellItem);

                    if (hr == HRESULT.S_OK && parentShellItem != null)
                    {
                        this.parentShellObject = ShellObjectFactory.Create(parentShellItem);
                    }
                    else if (hr == HRESULT.NOObject)
                    {
                        // Should return null if the parent is desktop
                        return null;
                    }
                    else
                    {
                        throw Marshal.GetExceptionForHR((int)hr);
                    }
                }

                return this.parentShellObject;
            }
        }

        /// <summary>
        ///   Gets the parsing name for this ShellItem.
        /// </summary>
        public virtual string ParsingName
        {
            get
            {
                if (this.internalParsingName == null && this.nativeShellItem != null)
                {
                    this.internalParsingName = ShellHelper.GetParsingName(this.nativeShellItem);
                }

                return this.internalParsingName;
            }

            protected set
            {
                this.internalParsingName = value;
            }
        }

        /// <summary>
        ///   Gets an object that allows the manipulation of ShellProperties for this shell item.
        /// </summary>
        public ShellProperties Properties
        {
            get
            {
                return this.properties ?? (this.properties = new ShellProperties(this));
            }
        }

        /// <summary>
        ///   Gets the thumbnail of the ShellObject.
        /// </summary>
        public ShellThumbnail Thumbnail
        {
            get
            {
                return this.thumbnail ?? (this.thumbnail = new ShellThumbnail(this));
            }
        }

        /// <summary>
        ///   Gets access to the native IPropertyStore (if one is already
        ///   created for this item and still valid. This is usually done by the
        ///   ShellPropertyWriter class. The reference will be set to null
        ///   when the writer has been closed/commited).
        /// </summary>
        internal IPropertyStore NativePropertyStore { get; set; }

        /// <summary>
        ///   Return the native ShellFolder object
        /// </summary>
        internal virtual IShellItem NativeShellItem
        {
            get
            {
                return this.NativeShellItem2;
            }
        }

        /// <summary>
        ///   Return the native ShellFolder object as newer IShellItem2
        /// </summary>
        /// <exception cref = "System.Runtime.InteropServices.ExternalException">If the native object cannot be created.
        ///   The ErrorCode member will contain the external error code.</exception>
        internal virtual IShellItem2 NativeShellItem2
        {
            get
            {
                if (this.nativeShellItem == null && this.ParsingName != null)
                {
                    var guid = new Guid(ShellIidGuid.IShellItem2);
                    var retCode = ShellNativeMethods.SHCreateItemFromParsingName(this.ParsingName, IntPtr.Zero, ref guid, out this.nativeShellItem);

                    if (this.nativeShellItem == null || !CoreErrorHelper.Succeeded(retCode))
                    {
                        throw new ExternalException("Shell item could not be created.", Marshal.GetExceptionForHR(retCode));
                    }
                }

                return this.nativeShellItem;
            }
        }

        /// <summary>
        ///   Gets the PID List (PIDL) for this ShellItem.
        /// </summary>
        internal virtual IntPtr PIDL
        {
            get
            {
                // Get teh PIDL for the ShellItem
                if (this.internalPIDL == IntPtr.Zero && this.NativeShellItem != null)
                {
                    this.internalPIDL = ShellHelper.PidlFromShellItem(this.NativeShellItem);
                }

                return this.internalPIDL;
            }

            set
            {
                this.internalPIDL = value;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        ///   Implements the == (equality) operator.
        /// </summary>
        /// <param name = "a">Object a.</param>
        /// <param name = "b">Object b.</param>
        /// <returns>true if object a equals object b; false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        public static bool operator ==(ShellObject a, ShellObject b)
        {
            return (object)a == null ? (object)b == null : a.Equals(b);
        }

        /// <summary>
        ///   Implements the != (inequality) operator.
        /// </summary>
        /// <param name = "a">Object a.</param>
        /// <param name = "b">Object b.</param>
        /// <returns>true if object a does not equal object b; false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        public static bool operator !=(ShellObject a, ShellObject b)
        {
            return (object)a == null ? (object)b != null : !a.Equals(b);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a ShellObject subclass given a parsing name.
        ///   For file system items, this method will only accept absolute paths.
        /// </summary>
        /// <param name="parsingName">
        /// The parsing name of the object.
        /// </param>
        /// <returns>
        /// A newly constructed ShellObject object.
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public static ShellObject FromParsingName(string parsingName)
        {
            return ShellObjectFactory.Create(parsingName);
        }

        /// <summary>
        /// Returns whether this object is equal to another.
        /// </summary>
        /// <param name="obj">
        /// The object to compare against.
        /// </param>
        /// <returns>
        /// Equality result.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ShellObject);
        }

        /// <summary>
        /// Returns the display name of the ShellFolder object. DisplayNameType represents one of the 
        ///   values that indicates how the name should look. 
        ///   See <see cref="Microsoft.Windows.Shell.DisplayNameType"/>for a list of possible values.
        /// </summary>
        /// <param name="displayNameType">
        /// A disaply name type.
        /// </param>
        /// <returns>
        /// A string.
        /// </returns>
        public virtual string GetDisplayName(DisplayNameType displayNameType)
        {
            string returnValue = null;

            var hr = HRESULT.S_OK;

            if (this.NativeShellItem2 != null)
            {
                hr = this.NativeShellItem2.GetDisplayName((ShellNativeMethods.SIGDN)displayNameType, out returnValue);
            }

            if (hr != HRESULT.S_OK)
            {
                throw new COMException("Can't get the display name", (int)hr);
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the hash code of the object.
        /// </summary>
        /// <returns>
        /// </returns>
        public override int GetHashCode()
        {
            if (!this.hashValue.HasValue)
            {
                var size = ShellNativeMethods.ILGetSize(this.PIDL);
                if (size != 0)
                {
                    var pidlData = new byte[size];
                    Marshal.Copy(this.PIDL, pidlData, 0, (int)size);
                    var hashData = hashProvider.ComputeHash(pidlData);
                    this.hashValue = BitConverter.ToInt32(hashData, 0);
                }
                else
                {
                    this.hashValue = 0;
                }
            }

            return this.hashValue.Value;
        }

        /// <summary>
        /// Overrides object.ToString()
        /// </summary>
        /// <returns>
        /// A string representation of the object.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

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

        #region IEquatable<ShellObject>

        /// <summary>
        /// Determines if two ShellObjects are identical.
        /// </summary>
        /// <param name="other">
        /// The ShellObject to comare this one to.
        /// </param>
        /// <returns>
        /// True if the ShellObjects are equal, false otherwise.
        /// </returns>
        public bool Equals(ShellObject other)
        {
            var areEqual = false;

            if ((object)other != null)
            {
                var ifirst = this.NativeShellItem;
                var isecond = other.NativeShellItem;
                if ((ifirst != null) && (isecond != null))
                {
                    int result;
                    var hr = ifirst.Compare(isecond, SICHINTF.SichintAllfields, out result);

                    if ((hr == HRESULT.S_OK) && (result == 0))
                    {
                        areEqual = true;
                    }
                }
            }

            return areEqual;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Release the native and managed objects
        /// </summary>
        /// <param name="disposing">
        /// Indicates that this is being called from Dispose(), rather than the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.internalName = null;
                this.internalParsingName = null;
                this.properties = null;
                this.thumbnail = null;
                this.parentShellObject = null;
                if (this.parentShellObject != null)
                {
                    this.parentShellObject.Dispose();
                }
            }

            if (this.internalPIDL != IntPtr.Zero)
            {
                ShellNativeMethods.ILFree(this.internalPIDL);
                this.internalPIDL = IntPtr.Zero;
            }

            if (this.nativeShellItem != null)
            {
                Marshal.ReleaseComObject(this.nativeShellItem);
                this.nativeShellItem = null;
            }

            if (this.NativePropertyStore == null)
            {
                return;
            }

            Marshal.ReleaseComObject(this.NativePropertyStore);
            this.NativePropertyStore = null;
        }

        #endregion
    }
}