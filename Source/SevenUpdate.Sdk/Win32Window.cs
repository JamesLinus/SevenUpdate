// ***********************************************************************
// <copyright file="Win32Window.cs"
//            project="SevenUpdate.Sdk"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate.Sdk
{
    using System;
    using System.Windows.Forms;

    /// <summary>A Win32 window</summary>
    public sealed class Win32Window : IWin32Window, IDisposable
    {
        #region Constants and Fields

        /// <summary>The pointer to the window</summary>
        private readonly IntPtr windowHandle;

        /// <summary><see langword="true" /> if the window is disposed</summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Win32Window" /> class.</summary>
        /// <param name="handle">The handle.</param>
        public Win32Window(IntPtr handle)
        {
            this.windowHandle = handle;
        }

        /// <summary>Finalizes an instance of the <see cref="Win32Window" /> class.</summary>
        ~Win32Window()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>Gets the handle to the window represented by the implementer.</summary>
        /// <value></value>
        /// <returns>A handle to the window represented by the implementer.</returns>
        IntPtr IWin32Window.Handle
        {
            get
            {
                return this.windowHandle;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(false);

            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>Releases unmanaged and - optionally - managed resources</summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            lock (this)
            {
                // Do nothing if the object has already been disposed of.
                if (this.disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // Release disposable objects used by this instance here.
                }

                // Release unmanaged resources here. Don't access reference type fields.

                // Remember that the object has been disposed of.
                this.disposed = true;
            }
        }

        #endregion
    }
}