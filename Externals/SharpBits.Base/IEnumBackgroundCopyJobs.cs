// ***********************************************************************
// <copyright file="IEnumBackgroundCopyJobs.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
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
    ///   Use the IEnumBackgroundCopyJobs interface to enumerate the list of jobs in the transfer queue. To get an <see
    ///   cref="IEnumBackgroundCopyJobs" /> interface pointer, call the <c>IBackgroundCopyManager</c>:: EnumJobs
    /// method.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("1AF4F612-3B71-466F-8F58-7B6F73AC57AD")]
    [ComImportAttribute]
    internal interface IEnumBackgroundCopyJobs
    {
        /// <summary>Retrieves a specified number of items in the enumeration sequence.</summary>
        /// <param name="celt">Number of elements requested.</param>
        /// <param name="copyJob">Array of <c>IBackgroundCopyJob</c> objects. You must release each object in
        /// <paramref name="copyJob" /> when done.</param>
        /// <param name="celtFetched">Number of elements returned in <paramref name="copyJob" />. You can set fetched to
        /// <c>null</c> if <paramref name="copyJob" /> is one. Otherwise, initialize the value of fetched to 0 before
        /// calling this method.</param>
        void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob copyJob, out uint celtFetched);

        /// <summary>Skips a specified number of items in the enumeration sequence.</summary>
        /// <param name="celt">Number of elements to skip. .</param>
        void Skip(uint celt);

        /// <summary>Resets the enumeration sequence to the beginning.</summary>
        void Reset();

        /// <summary>Creates another enumerator that contains the same enumeration state as the current one.</summary>
        /// <param name="enum">Receives the interface pointer to the enumeration object. If the method is unsuccessful, the value of this output variable is undefined. You must release enumJobs when done.</param>
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyJobs @enum);

        /// <summary>Returns the number of items in the enumeration.</summary>
        /// <param name="count">Number of jobs in the enumeration.</param>
        void GetCount(out uint count);
    }
}
