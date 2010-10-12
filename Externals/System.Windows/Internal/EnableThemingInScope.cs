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
    public class EnableThemingInScope : IDisposable
    {
        #region Constants and Fields

        /// <summary>A flag to specify that a valid assembly directory is valid</summary>
        private const int AssemblyDirectoryValid = 0x004;

        /// <summary>Indicates if the context was created</summary>
        private static bool contextCreationSucceeded;

        /// <summary>enable theming</summary>
        private static ActivationContext enableThemingActivationContext;

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

            if (!ActivateActCtx(activationContext, out this.cookie))
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
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Activates the specified activation context. It does this by pushing the specified activation context to the top of the activation stack.
        /// The specified activation context is thus associated with the current thread and any appropriate side-by-side API functions.
        /// </summary>
        /// <param name="activationContext">Handle to an ACTCTX structure that contains information on the activation context that is to be made active.</param>
        /// <param name="cookie">Pointer to a ULONG_PTR that functions as a cookie, uniquely identifying a specific, activated activation context.</param>
        /// <returns>If the function succeeds, it returns <see langword="true"/>. Otherwise, it returns <see langword="false"/>.</returns>
        [DllImport(@"Kernel32.dll")]
        private static extern bool ActivateActCtx(IntPtr activationContext, out IntPtr cookie);

        /// <summary>Creates an activation context.</summary>
        /// <param name="activationContext">Pointer to an ACTCTX structure that contains information about the activation context to be created.</param>
        /// <returns>If the function succeeds, it returns a handle to the returned activation context. Otherwise, it returns InvalidHandleValue.</returns>
        [DllImport(@"Kernel32.dll")]
        private static extern IntPtr CreateActCtx(ref ActivationContext activationContext);

        /// <summary>Deactivates the activation context corresponding to the specified cookie.</summary>
        /// <param name="flags">Flags that indicate how the deactivation is to occur.</param>
        /// <param name="cookie">The ULONG_PTR that was passed into the call to <see cref="ActivateActCtx"/>. This value is used as a cookie to identify a specific activated activation context.</param>
        /// <returns>If the function succeeds, it returns <see langword="true"/>. Otherwise, it returns <see langword="false"/>.</returns>
        [DllImport(@"Kernel32.dll")]
        private static extern bool DeactivateActCtx(uint flags, IntPtr cookie);

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
                        enableThemingActivationContext = new ActivationContext
                            {
                                Size = Marshal.SizeOf(typeof(ActivationContext)),
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
                        activationContext = CreateActCtx(ref enableThemingActivationContext);
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
            if (disposing && this.cookie != IntPtr.Zero)
            {
                if (DeactivateActCtx(0, this.cookie))
                {
                    // deactivation succeeded...
                    this.cookie = IntPtr.Zero;
                }
            }
        }

        #endregion

        /// <summary>
        /// Data used to create the activation context.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct ActivationContext
        {   
            /// <summary>
            /// Identifies the type of processor used. Specifies the system's processor architecture.
            /// </summary>
            public readonly ushort ProcessorArchitecture;

            /// <summary>
            /// Specifies the language manifest that should be used. The default is the current user's current UI language.
            /// </summary>
            public readonly ushort LangId;

            /// <summary>
            /// Pointer to a <see langword="null"/>-terminated string that contains the resource name to be loaded from the PE specified in hModule or Source. If the resource name is an integer, set this member using MAKEINTRESOURCE. This member is required if Source refers to an EXE or DLL.
            /// </summary>
            public readonly string ResourceName;

            /// <summary>
            /// The name of the current application. If the value of this member is set to null, the name of the executable that launched the current process is used.
            /// </summary>
            public readonly string ApplicationName;

            /// <summary>
            /// The size, in bytes, of this structure. This is used to determine the version of this structure.
            /// </summary>
            public int Size;

            /// <summary>
            /// Flags that indicate how the values included in this structure are to be used. Set any undefined bits in Flags to 0. If any undefined bits are not set to 0, the call to CreateActCtx that creates the activation context fails and returns an invalid parameter error code.
            /// </summary>
            public uint Flags;

            /// <summary>
            /// Null-terminated string specifying the path of the manifest file or PE image to be used to create the activation context. If this path refers to an EXE or DLL file, the <see cref="ResourceName"/> member is required.
            /// </summary>
            public string Source;

            /// <summary>
            /// The base directory in which to perform private assembly probing if assemblies in the activation context are not present in the system-wide store.
            /// </summary>
            public string AssemblyDirectory;
        }
    }
}