// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Internal
{
    // using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    // using System.Text;

    /// <summary>
    /// Wrappers for Native Methods and Structs.
    ///   This type is intended for internal use only
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls
        ///   the window procedure for the specified window and does not return until the window
        ///   procedure has processed the message.
        /// </summary>
        /// <parameter name="pointer">
        /// Handle to the window whose window procedure will receive the message.
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///   but the message is not sent to child windows.
        /// </parameter>
        /// <parameter name="msg">
        /// Specifies the message to be sent.
        /// </parameter>
        /// <parameter name="parameter">
        /// Specifies additional message-specific information.
        /// </parameter>
        /// <parameter name="parameterLength">
        /// Specifies additional message-specific information.
        /// </parameter>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", 
            Justification = "This is used from another assembly, also it's in an internal namespace")]
        [DllImport(@"user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr pointer, uint msg, IntPtr parameter, IntPtr parameterLength);

        [DllImport("shell32.dll", EntryPoint = "IsUserAnAdmin", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsUserAnAdmin();

        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr handle, ref DwmBlurBehind bb);

        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr handle, ref Margins m);

        [DllImport(@"DwmApi.dll", PreserveSig = false)]
        internal static extern bool DwmIsCompositionEnabled();

        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmEnableComposition(CompositionEnable compositionAction);

        [DllImport("useDr32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr handle, ref Rect rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr handle, ref Rect rect);

        /// <summary>
        ///   Various important window messages
        /// </summary>
        internal const int WMUser = 0x0400;

        /// <summary>
        ///   Enable/disable non-client rendering based on window style.
        /// </summary>
        internal const int DwmNcrUseWindowStyle = 0;

        /// <summary>
        ///   Disabled non-client rendering; window style is ignored.
        /// </summary>
        internal const int DwmNcrDisabled = 1;

        /// <summary>
        ///   Enabled non-client rendering; window style is ignored.
        /// </summary>
        internal const int DwmNcrEnabled = 2;

        /// <summary>
        ///   Enable/disable non-client rendering Use DWMNCRP_* values.
        /// </summary>
        internal const int DwmNCRenderingEnabled = 1;

        /// <summary>
        ///   Non-client rendering policy.
        /// </summary>
        internal const int DwmNCRenderingPolicy = 2;

        /// <summary>
        ///   Potentially enable/forcibly disable transitions 0 or 1.
        /// </summary>
        internal const int DwmTransitionsForceDisabled = 3;

        /// <summary>
        ///   fEnable has been specified
        /// </summary>
        internal const int DwmBBEnable = 0x00000001;

        /// <summary>
        /// The blur region has been specified
        /// </summary>
        internal const int DwmBlurRegion = 0x00000002;

        /// <summary>
        ///   fTransitionOnMaximized has been specified
        /// </summary>
        internal const int DwmTransitionOnMaximized = 0x00000004;

        /// <summary>
        /// Dwm composition enable
        /// </summary>
        internal enum CompositionEnable : uint
        {
            /// <summary>
            /// </summary>
            DwmECDisableComposition = 0, 

            /// <summary>
            /// </summary>
            DwmECEnableComposition = 1
        }

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DwmBlurBehind
        {
            /// <summary>
            /// </summary>
            public DwmBlurBehindDWFlag Flags;

            /// <summary>
            /// </summary>
            public bool Enable;

            /// <summary>
            /// </summary>
            public IntPtr RegionBlur;

            /// <summary>
            /// </summary>
            public bool TransitionOnMaximized;
        }

        /// <summary>
        /// </summary>
        internal enum DwmBlurBehindDWFlag : uint
        {
            /// <summary>
            /// </summary>
            DwmBBEnable = 0x00000001, 

            /// <summary>
            /// </summary>
            DwmBBBlurRegion = 0x00000002, 

            /// <summary>
            /// </summary>
            DwmBBTransitionOnMaximized = 0x00000004
        }

        /// <summary>
        /// </summary>
        internal static class DwmMessages
        {
            #region Constants and Fields

            /// <summary>
            /// </summary>
            internal const int DwmCompositionChanged = 0x031E;

            /// <summary>
            /// </summary>
            internal const int DwmRenderingChanged = 0x031F;

            #endregion
        }

        /// <summary>
        /// The margins that specify top, left, right, down
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            /// <summary>
            ///   width of left border that retains its size
            /// </summary>
            public int LeftWidth;

            /// <summary>
            ///   width of right border that retains its size
            /// </summary>
            public int RightWidth;

            /// <summary>
            ///   height of top border that retains its size
            /// </summary>
            public int TopHeight;

            /// <summary>
            ///   height of bottom border that retains its size
            /// </summary>
            public int BottomHeight;

            /// <summary>
            /// Initializes a new instance of the <see cref="Margins"/> struct.
            /// </summary>
            /// <parameter name="fullWindow">
            /// if set to <see langword="true"/> [full window].
            /// </parameter>
            public Margins(bool fullWindow)
            {
                this.LeftWidth = this.RightWidth = this.TopHeight = this.BottomHeight = fullWindow ? -1 : 0;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Margins"/> struct.
            /// </summary>
            /// <parameter name="left">
            /// The left margin
            /// </parameter>
            /// <parameter name="top">
            /// The top margin
            /// </parameter>
            /// <parameter name="right">
            /// The right margin
            /// </parameter>
            /// <parameter name="bottom">
            /// The bottom margin
            /// </parameter>
            public Margins(int left, int top, int right, int bottom)
            {
                this.LeftWidth = left;
                this.RightWidth = right;
                this.TopHeight = top;
                this.BottomHeight = bottom;
            }
        }

        /// <summary>
        /// A Wrapper for a RECT struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RECT")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            /// <summary>
            ///   Position of left edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int Left;

            /// <summary>
            ///   Position of top edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int Top;

            /// <summary>
            ///   Position of right edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int Right;

            /// <summary>
            ///   Position of bottom edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int Bottom;
        }
    }
}