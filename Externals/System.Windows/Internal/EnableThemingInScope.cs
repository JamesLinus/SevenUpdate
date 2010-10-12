// ***********************************************************************
// <copyright file="EnableThemingInScope.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.Internal
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    /// <summary>
    /// Enables the process to access theming controls
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public sealed class EnableThemingInScope : IDisposable
    {
        #region Constants and Fields

        /// <summary>A flag to specify that a valid assembly directory is valid</summary>
        private const int AssemblyDirectoryValid = 0x004;

        /// <summary>Indicates if the context was created</summary>
        private static bool contextCreationSucceeded;

        /// <summary>enable theming</summary>
        private static NativeMethods.ActivationContext enableThemingActivationContext;

        /// <summary>The activation context</summary>
        private static IntPtr activationContext;

        /// <summary>The cookie</summary>
        private IntPtr cookie;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="EnableThemingInScope"/> class.</summary>
        /// <param name="enable">if set to <see langword="true"/> [enable].</param>
        public EnableThemingInScope(bool enable)
        {
            if (!enable)
            {
                return;
            }

            // what does this line look like in WPF land?
            if (!EnsureActivateContextCreated())
            {
                return;
            }

            if (!NativeMethods.ActivateActCtx(activationContext, out this.cookie))
            {
                // Be sure cookie always zero if activation failed
                this.cookie = IntPtr.Zero;
            }
        }

        /// <summary>Finalizes an instance of the <see cref="EnableThemingInScope"/> class.</summary>
        ~EnableThemingInScope()
        {
            this.Dispose(false);
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Ensures the activation context is created
        /// </summary>
        /// <returns>If the function succeeds, it returns <see langword="true"/>. Otherwise, it returns <see langword="false"/>.</returns>
        [SuppressUnmanagedCodeSecurity]
        private static bool EnsureActivateContextCreated()
        {
            lock (typeof(EnableThemingInScope))
            {
                if (!contextCreationSucceeded)
                {
                    // Pull manifest from the .NET Framework install directory
                    string assemblyLoc;

                    var filePermission = new FileIOPermission(PermissionState.None)
                        {
                            AllFiles = FileIOPermissionAccess.PathDiscovery
                        };
                    filePermission.Assert();
                    try
                    {
                        assemblyLoc = typeof(object).Assembly.Location;
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }

                    var installDir = Path.GetDirectoryName(assemblyLoc);

                    if (installDir != null)
                    {
                        var manifestLoc = Path.Combine(installDir, @"XPThemes.manifest");
                        enableThemingActivationContext = new NativeMethods.ActivationContext
                            {
                                Size = Marshal.SizeOf(typeof(NativeMethods.ActivationContext)),
                                Source = manifestLoc,
                                AssemblyDirectory = installDir,
                                Flags = AssemblyDirectoryValid
                            };

                        // Set the lpAssemblyDirectory to the install
                        // directory to prevent Win32 Side by Side from
                        // looking for comctl32 in the application
                        // directory, which could cause a bogus dll to be
                        // placed there and open a security hole.

                        // Note this will fail gracefully if file specified
                        // by manifestLoc doesn't exist.
                        activationContext = NativeMethods.CreateActCtx(ref enableThemingActivationContext);
                        contextCreationSucceeded = activationContext != (IntPtr.Size > 4 ? new IntPtr(-1L) : new IntPtr(-1));
                    }
                }

                // If we return false, we'll try again on the next call into
                // EnsureActivateContextCreated(), which is fine.
                return contextCreationSucceeded;
            }
        }

        /// <summary>Releases unmanaged and - optionally - managed resources</summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!disposing || this.cookie == IntPtr.Zero)
            {
                return;
            }

            if (NativeMethods.DeactivateActCtx(0, this.cookie))
            {
                // deactivation succeeded...
                this.cookie = IntPtr.Zero;
            }
        }

        #endregion
    }
}