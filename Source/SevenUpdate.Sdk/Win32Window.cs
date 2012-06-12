// <copyright file="Win32Window.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk
{
    using System;
    using System.Windows.Forms;

    /// <summary>A Win32 window.</summary>
    public sealed class Win32Window : IWin32Window, IDisposable
    {
        /// <summary>The pointer to the window.</summary>
        readonly IntPtr windowHandle;

        /// <summary>Indicates if the window is disposed.</summary>
        bool disposed;

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

        /// <summary>Gets the handle to the window represented by the implementer.</summary>
        /// <returns>A handle to the window represented by the implementer.</returns>
        IntPtr IWin32Window.Handle
        {
            get { return this.windowHandle; }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(false);

            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing">Release both managed and unmanaged resources; <c>False</c> to release only unmanaged resources.</param>
        void Dispose(bool disposing)
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
    }
}