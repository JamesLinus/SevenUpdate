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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   An ennumerable list of ShellObjects
    /// </summary>
    public class ShellObjectCollection : IDisposable, IList<ShellObject>
    {
        private List<ShellObject> content;

        private bool isDisposed;

        #region construction/disposal/finialization

        /// <summary>
        ///   Creates a ShellObject collection from an IShellItemArray
        /// </summary>
        /// <param name = "iArray">IShellItemArray pointer</param>
        /// <param name = "readOnly">Indicates whether the collection shouldbe read-only or not</param>
        internal ShellObjectCollection(IShellItemArray iArray, bool readOnly)
        {
            IsReadOnly = readOnly;

            if (iArray == null)
            {
            }
            else
            {
                try
                {
                    uint itemCount;
                    iArray.GetCount(out itemCount);

                    content = new List<ShellObject>((int) itemCount);

                    for (uint index = 0; index < itemCount; index++)
                    {
                        IShellItem iShellItem;
                        iArray.GetItemAt(index, out iShellItem);
                        content.Add(ShellObjectFactory.Create(iShellItem));
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(iArray);
                }
            }
        }

        /// <summary>
        ///   Constructs an empty ShellObjectCollection
        /// </summary>
        public ShellObjectCollection()
        {
            IsReadOnly = false;
            content = new List<ShellObject>();
        }

        /// <summary>
        ///   Standard Dispose pattern
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Creates a ShellObjectCollection from an IDataObject passed during Drop operation.
        /// </summary>
        /// <param name = "dataObject">An object that implements the IDataObject COM interface.</param>
        /// <returns>ShellObjectCollection created from the given IDataObject</returns>
        public static ShellObjectCollection FromDataObject(object dataObject)
        {
            var iDataObject = dataObject as IDataObject;

            IShellItemArray shellItemArray;
            var iid = new Guid(ShellIIDGuid.IShellItemArray);
            ShellNativeMethods.SHCreateShellItemArrayFromDataObject(iDataObject, ref iid, out shellItemArray);
            return new ShellObjectCollection(shellItemArray, true);
        }

        /// <summary>
        /// </summary>
        ~ShellObjectCollection()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Standard Dispose patterns
        /// </summary>
        /// <param name = "disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected void Dispose(bool disposing)
        {
            if (isDisposed)
                return;
            if (disposing && content != null)
            {
                foreach (var obj in content)
                    obj.Dispose();

                content.Clear();
                content = null;
            }

            isDisposed = true;
        }

        #endregion

        #region implementation

        /// <summary>
        ///   Item count
        /// </summary>
        public int Count { get { return content != null ? content.Count : 0; } }

        /// <summary>
        ///   Collection enumeration
        /// </summary>
        /// <returns />
        public IEnumerator GetEnumerator()
        {
            for (var index = 0; index < Count; index++)
                yield return content[index];
        }

        /// <summary>
        ///   Builds the data for the CFSTR_SHELLIDLIST Drag and Clipboard data format from the 
        ///   ShellObjects in the collection.
        /// </summary>
        /// <returns>A memory stream containing the drag/drop data.</returns>
        public MemoryStream BuildShellIDList()
        {
            if (content == null || content.Count < 1)
                throw new ArgumentException("Must have at least one shell object in the collection.");

            var mstream = new MemoryStream();
            var bwriter = new BinaryWriter(mstream);

            // number of IDLs to be written (shell objects + parent folder)
            var itemCount = (uint) (content.Count + 1);

            // grab the object IDLs            
            var idls = new IntPtr[itemCount];

            for (var index = 0; index < itemCount; index++)
            {
                if (index == 0)
                {
                    // Because the ShellObjects passed in may be from anywhere, the 
                    // parent folder reference must be the desktop.
                    idls[index] = ((ShellObject) KnownFolders.Desktop).PIDL;
                }
                else
                    idls[index] = content[index - 1].PIDL;
            }

            // calculate offset array (folder IDL + item IDLs)
            var offsets = new uint[itemCount + 1];
            for (var index = 0; index < itemCount; index++)
            {
                if (index == 0)
                {
                    // first offset equals size of CIDA header data
                    offsets[0] = (uint) (sizeof (uint)*(offsets.Length + 1));
                }
                else
                    offsets[index] = offsets[index - 1] + ShellNativeMethods.ILGetSize(idls[index - 1]);
            }

            // Fill out the CIDA header
            //
            //    typedef struct _IDA {
            //    UINT cidl;          // number of relative IDList
            //    UINT aoffset[1];    // [0]: folder IDList, [1]-[cidl]: item IDList
            //    } CIDA, * LPIDA;
            //
            bwriter.Write(content.Count);
            foreach (var offset in offsets)
                bwriter.Write(offset);

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

        #region IList<ShellObject> Members

        /// <summary>
        ///   Returns the index of a particualr shell object in the collection
        /// </summary>
        /// <param name = "item">The item to search for.</param>
        /// <returns>The index of the item found, or -1 if not found.</returns>
        public int IndexOf(ShellObject item)
        {
            return content != null ? content.FindIndex(x => x.Equals(item)) : -1;
        }

        /// <summary>
        ///   Inserts a new shell object into the collection.
        /// </summary>
        /// <param name = "index">The index at which to insert.</param>
        /// <param name = "item">The item to insert.</param>
        public void Insert(int index, ShellObject item)
        {
            if (IsReadOnly)
                throw new ArgumentException("Can not insert items into a read only list.");

            content.Insert(index, item);
        }

        /// <summary>
        ///   Removes the specified ShellObject from the collection
        /// </summary>
        /// <param name = "index">The index to remove at.</param>
        public void RemoveAt(int index)
        {
            if (IsReadOnly)
                throw new ArgumentException("Can not remove items from a read only list.");

            content.RemoveAt(index);
        }

        /// <summary>
        ///   The collection indexer
        /// </summary>
        /// <param name = "index">The index of the item to retrieve.</param>
        /// <returns>The ShellObject at the specified index</returns>
        public ShellObject this[int index]
        {
            get { return content != null ? content[index] : null; }
            set
            {
                if (IsReadOnly)
                    throw new ArgumentException("Can not insert items into a read only list");

                content[index] = value;
            }
        }

        /// <summary>
        ///   Adds a ShellObject to the collection,
        /// </summary>
        /// <param name = "item">The ShellObject to add.</param>
        public void Add(ShellObject item)
        {
            if (IsReadOnly)
                throw new ArgumentException("Can not add items to a read only list.");

            if (content != null)
                content.Add(item);
        }

        /// <summary>
        ///   Clears the collection of ShellObjects.
        /// </summary>
        public void Clear()
        {
            if (IsReadOnly)
                throw new ArgumentException("Can not clear a read only list.");

            if (content != null)
                content.Clear();
        }

        /// <summary>
        ///   Determines if the collection contains a particular ShellObject.
        /// </summary>
        /// <param name = "item">The ShellObject.</param>
        /// <returns>true, if the ShellObject is in the list, false otherwise.</returns>
        public bool Contains(ShellObject item)
        {
            return content != null && content.Contains(item);
        }

        /// <summary>
        ///   Copies the ShellObjects in the collection to a ShellObject array.
        /// </summary>
        /// <param name = "array">The destination to copy to.</param>
        /// <param name = "arrayIndex">The index into the array at which copying will commence.</param>
        public void CopyTo(ShellObject[] array, int arrayIndex)
        {
            if (array.Length < arrayIndex + content.Count)
                throw new ArgumentException("Destination array too small, or invalid arrayIndex.");

            for (var index = 0; index < content.Count; index++)
                array[index + arrayIndex] = content[index];
        }

        /// <summary>
        ///   Retrieves the number of ShellObjects in the collection
        /// </summary>
        int ICollection<ShellObject>.Count { get { return content != null ? content.Count : 0; } }

        /// <summary>
        ///   If true, the contents of the collection are immutable.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        ///   Removes a particular ShellObject from the list.
        /// </summary>
        /// <param name = "item">The ShellObject to remove.</param>
        /// <returns>True if the item could be removed, false otherwise.</returns>
        public bool Remove(ShellObject item)
        {
            if (IsReadOnly)
                throw new ArgumentException("Can not remove an item from a read only list.");

            return content != null && content.Remove(item);
        }

        /// <summary>
        ///   Allows for enumeration through the list of ShellObjects in the collection.
        /// </summary>
        /// <returns>The IEnumerator interface to use for enumeration.</returns>
        IEnumerator<ShellObject> IEnumerable<ShellObject>.GetEnumerator()
        {
            return content as IEnumerator<ShellObject>;
        }

        #endregion
    }
}