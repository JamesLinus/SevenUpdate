// ***********************************************************************
// <copyright file="IEnumBackgroundCopyFiles.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///   Use the IEnumBackgroundCopyFiles interface to enumerate the files that a job contains. To get an
    ///   IEnumBackgroundCopyFiles interface pointer, call the <c>IBackgroundCopyJob</c>::EnumFiles method.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("CA51E165-C365-424C-8D41-24AAA4FF3C40")]
    [ComImportAttribute]
    internal interface IEnumBackgroundCopyFiles
    {
        /// <summary>
        ///   Retrieves a specified number of items in the enumeration sequence.
        /// </summary>
        /// <param name="celt">
        ///   Number of elements requested.
        /// </param>
        /// <param name="copyFile">
        ///   Array of <c>IBackgroundCopyFile</c> objects. You must release each object in <paramref name="copyFile" />
        ///   when done.
        /// </param>
        /// <param name="fetched">
        ///   Number of elements returned in <paramref name="copyFile" />. You can set fetched to <c>null</c> if
        ///   <paramref
        ///    name="celt" /> is one. Otherwise, initialize the value of fetched to 0 before calling this method.
        /// </param>
        void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyFile copyFile, out uint fetched);

        /// <summary>
        ///   Skips a specified number of items in the enumeration sequence.
        /// </summary>
        /// <param name="celt">
        ///   Number of elements to skip.
        /// </param>
        void Skip(uint celt);

        /// <summary>
        ///   Resets the enumeration sequence to the beginning.
        /// </summary>
        void Reset();

        /// <summary>
        ///   Creates another enumerator that contains the same enumeration state as the current enumerator.
        /// </summary>
        /// <param name="enum">
        ///   Receives the interface pointer to the enumeration object. If the method is unsuccessful, the value of this
        ///   output variable is undefined. You must release enumFiles when done.
        /// </param>
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles @enum);

        /// <summary>
        ///   Retrieves the number of items in the enumeration.
        /// </summary>
        /// <param name="count">
        ///   Number of files in the enumeration.
        /// </param>
        void GetCount(out uint count);
    }
}