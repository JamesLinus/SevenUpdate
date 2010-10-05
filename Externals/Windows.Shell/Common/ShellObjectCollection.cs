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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    /// <summary>
    /// An ennumerable list of ShellObjects
    /// </summary>
    public class ShellObjectCollection : IDisposable, IList<ShellObject>
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private List<ShellObject> content;

        /// <summary>
        /// </summary>
        private bool isDisposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructs an empty ShellObjectCollection
        /// </summary>
        public ShellObjectCollection()
        {
            this.IsReadOnly = false;
            this.content = new List<ShellObject>();
        }

        /// <summary>
        /// Creates a ShellObject collection from an IShellItemArray
        /// </summary>
        /// <param name="iArray">
        /// IShellItemArray pointer
        /// </param>
        /// <param name="readOnly">
        /// Indicates whether the collection shouldbe read-only or not
        /// </param>
        internal ShellObjectCollection(IShellItemArray iArray, bool readOnly)
        {
            this.IsReadOnly = readOnly;

            if (iArray == null)
            {
            }
            else
            {
                try
                {
                    uint itemCount;
                    iArray.GetCount(out itemCount);

                    this.content = new List<ShellObject>((int)itemCount);

                    for (uint index = 0; index < itemCount; index++)
                    {
                        IShellItem iShellItem;
                        iArray.GetItemAt(index, out iShellItem);
                        this.content.Add(ShellObjectFactory.Create(iShellItem));
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(iArray);
                }
            }
        }

        /// <summary>
        /// </summary>
        ~ShellObjectCollection()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Item count
        /// </summary>
        public int Count
        {
            get
            {
                return this.content != null ? this.content.Count : 0;
            }
        }

        /// <summary>
        ///   If true, the contents of the collection are immutable.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        ///   Retrieves the number of ShellObjects in the collection
        /// </summary>
        int ICollection<ShellObject>.Count
        {
            get
            {
                return this.content != null ? this.content.Count : 0;
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        ///   The collection indexer
        /// </summary>
        /// <param name = "index">The index of the item to retrieve.</param>
        /// <returns>The ShellObject at the specified index</returns>
        public ShellObject this[int index]
        {
            get
            {
                return this.content != null ? this.content[index] : null;
            }

            set
            {
                if (this.IsReadOnly)
                {
                    throw new ArgumentException("Can not insert items into a read only list");
                }

                this.content[index] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a ShellObjectCollection from an IDataObject passed during Drop operation.
        /// </summary>
        /// <param name="data">
        /// An object that implements the IDataObject COM interface.
        /// </param>
        /// <returns>
        /// ShellObjectCollection created from the given IDataObject
        /// </returns>
        public static ShellObjectCollection FromDataObject(object data)
        {
            var iDataObject = data as IDataObject;

            IShellItemArray shellItemArray;
            var iid = new Guid(ShellIidGuid.IShellItemArray);
            ShellNativeMethods.SHCreateShellItemArrayFromDataObject(iDataObject, ref iid, out shellItemArray);
            return new ShellObjectCollection(shellItemArray, true);
        }

        /// <summary>
        /// Builds the data for the CFSTR_SHELLIDLIST Drag and Clipboard data format from the 
        ///   ShellObjects in the collection.
        /// </summary>
        /// <returns>
        /// A memory stream containing the drag/drop data.
        /// </returns>
        public MemoryStream BuildShellIDList()
        {
            if (this.content == null || this.content.Count < 1)
            {
                throw new ArgumentException("Must have at least one shell object in the collection.");
            }

            var mstream = new MemoryStream();
            var bwriter = new BinaryWriter(mstream);

            // number of IDLs to be written (shell objects + parent folder)
            var itemCount = (uint)(this.content.Count + 1);

            // grab the object IDLs            
            var idls = new IntPtr[itemCount];

            for (var index = 0; index < itemCount; index++)
            {
                if (index == 0)
                {
                    // Because the ShellObjects passed in may be from anywhere, the 
                    // parent folder reference must be the desktop.
                    idls[index] = ((ShellObject)KnownFolders.Desktop).PIDL;
                }
                else
                {
                    idls[index] = this.content[index - 1].PIDL;
                }
            }

            // calculate offset array (folder IDL + item IDLs)
            var offsets = new uint[itemCount + 1];
            for (var index = 0; index < itemCount; index++)
            {
                if (index == 0)
                {
                    // first offset equals size of CIDA header data
                    offsets[0] = (uint)(sizeof(uint) * (offsets.Length + 1));
                }
                else
                {
                    offsets[index] = offsets[index - 1] + ShellNativeMethods.ILGetSize(idls[index - 1]);
                }
            }

            // Fill out the CIDA header
            // typedef struct _IDA {
            // UINT cidl;          // number of relative IDList
            // UINT aoffset[1];    // [0]: folder IDList, [1]-[cidl]: item IDList
            // } CIDA, * LPIDA;
            bwriter.Write(this.content.Count);
            foreach (var offset in offsets)
            {
                bwriter.Write(offset);
            }

            // copy idls
            foreach (var idl in idls)
            {
                var data = new byte[ShellNativeMethods.ILGetSize(idl)];
                Marshal.Copy(idl, data, 0, data.Length);
                bwriter.Write(data, 0, data.Length);
            }

            // return CIDA stream 
            return mstream;
        }

        #endregion

        #region Implemented Interfaces

        #region ICollection<ShellObject>

        /// <summary>
        /// Adds a ShellObject to the collection,
        /// </summary>
        /// <param name="item">
        /// The ShellObject to add.
        /// </param>
        public void Add(ShellObject item)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Can not add items to a read only list.");
            }

            if (this.content != null)
            {
                this.content.Add(item);
            }
        }

        /// <summary>
        /// Clears the collection of ShellObjects.
        /// </summary>
        public void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Can not clear a read only list.");
            }

            if (this.content != null)
            {
                this.content.Clear();
            }
        }

        /// <summary>
        /// Determines if the collection contains a particular ShellObject.
        /// </summary>
        /// <param name="item">
        /// The ShellObject.
        /// </param>
        /// <returns>
        /// true, if the ShellObject is in the list, false otherwise.
        /// </returns>
        public bool Contains(ShellObject item)
        {
            return this.content != null && this.content.Contains(item);
        }

        /// <summary>
        /// Copies the ShellObjects in the collection to a ShellObject array.
        /// </summary>
        /// <param name="array">
        /// The destination to copy to.
        /// </param>
        /// <param name="arrayIndex">
        /// The index into the array at which copying will commence.
        /// </param>
        public void CopyTo(ShellObject[] array, int arrayIndex)
        {
            if (array.Length < arrayIndex + this.content.Count)
            {
                throw new ArgumentException("Destination array too small, or invalid arrayIndex.");
            }

            for (var index = 0; index < this.content.Count; index++)
            {
                array[index + arrayIndex] = this.content[index];
            }
        }

        /// <summary>
        /// Removes a particular ShellObject from the list.
        /// </summary>
        /// <param name="item">
        /// The ShellObject to remove.
        /// </param>
        /// <returns>
        /// True if the item could be removed, false otherwise.
        /// </returns>
        public bool Remove(ShellObject item)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Can not remove an item from a read only list.");
            }

            return this.content != null && this.content.Remove(item);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Standard Dispose pattern
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Collection enumeration
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            for (var index = 0; index < this.Count; index++)
            {
                yield return this.content[index];
            }
        }

        #endregion

        #region IEnumerable<ShellObject>

        /// <summary>
        /// Allows for enumeration through the list of ShellObjects in the collection.
        /// </summary>
        /// <returns>
        /// The IEnumerator interface to use for enumeration.
        /// </returns>
        IEnumerator<ShellObject> IEnumerable<ShellObject>.GetEnumerator()
        {
            return this.content as IEnumerator<ShellObject>;
        }

        #endregion

        #region IList<ShellObject>

        /// <summary>
        /// Returns the index of a particualr shell object in the collection
        /// </summary>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The index of the item found, or -1 if not found.
        /// </returns>
        public int IndexOf(ShellObject item)
        {
            return this.content != null ? this.content.FindIndex(x => x.Equals(item)) : -1;
        }

        /// <summary>
        /// Inserts a new shell object into the collection.
        /// </summary>
        /// <param name="index">
        /// The index at which to insert.
        /// </param>
        /// <param name="item">
        /// The item to insert.
        /// </param>
        public void Insert(int index, ShellObject item)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Can not insert items into a read only list.");
            }

            this.content.Insert(index, item);
        }

        /// <summary>
        /// Removes the specified ShellObject from the collection
        /// </summary>
        /// <param name="index">
        /// The index to remove at.
        /// </param>
        public void RemoveAt(int index)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Can not remove items from a read only list.");
            }

            this.content.RemoveAt(index);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Standard Dispose patterns
        /// </summary>
        /// <param name="disposing">
        /// Indicates that this is being called from Dispose(), rather than the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing && this.content != null)
            {
                foreach (var obj in this.content)
                {
                    obj.Dispose();
                }

                this.content.Clear();
                this.content = null;
            }

            this.isDisposed = true;
        }

        #endregion
    }
}