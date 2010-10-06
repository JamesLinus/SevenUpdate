// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;

    using Microsoft.Windows.Dialogs.TaskDialogs;

    /// <summary>
    /// Wrappers for Native Methods and Structs.
    ///   This type is intended for internal use only
    /// </summary>
    public static class CoreNativeMethods
    {
        #region General Definitions

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls
        /// the window procedure for the specified window and does not return until the window
        /// procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">Handle to the window whose window procedure will receive the message.
        /// If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        /// including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        /// but the message is not sent to child windows.</param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"w", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"l", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"Wnd", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"Param", Justification = "Extern")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", 
            Justification = "This is used from another assembly, also it's in an internal namespace")]
        [DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls
        /// the window procedure for the specified window and does not return until the window
        /// procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">Handle to the window whose window procedure will receive the message.
        /// If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        /// including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        /// but the message is not sent to child windows.</param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"Wnd", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"Param", Justification = "Extern")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Extern")]
        [DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls
        /// the window procedure for the specified window and does not return until the window
        /// procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">Handle to the window whose window procedure will receive the message.
        /// If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        /// including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        /// but the message is not sent to child windows.</param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"Param", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = @"Wnd", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l", Justification = "Extern")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w", Justification = "Extern")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Extern")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "This is an in/out parameter")]
        [DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, ref int wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the lp file.</param>
        /// <returns></returns>
        [DllImport(CommonDllNames.Kernel32, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="graphicsObjectHandle">The graphics object handle.</param>
        /// <returns></returns>
        [DllImport(CommonDllNames.User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr graphicsObjectHandle);

        /// <summary>
        /// Loads the string.
        /// </summary>
        /// <param name="hInstance">The h instance.</param>
        /// <param name="uID">The u ID.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="nBufferMax">The n buffer max.</param>
        /// <returns></returns>
        [DllImport(CommonDllNames.User32, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int LoadString(IntPtr hInstance, int uID, StringBuilder buffer, int nBufferMax);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon">
        /// Handle to the icon to be destroyed. The icon must not be in use.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h", Justification = "OK")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", 
            Justification = "This is used from other assemblies, also it's in an internal namespace")]
        [DllImport(CommonDllNames.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Determines whether the current user is an admin
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if [is user an admin]; otherwise, <see langword="false"/>.
        /// </returns>
        [DllImport("shell32.dll", EntryPoint = "IsUserAnAdmin", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsUserAnAdmin();

        /// <summary>
        /// DWMs the enable blur behind window.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="bb">The bb.</param>
        /// <returns></returns>
        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr hwnd, ref DwmBlurBehind bb);

        /// <summary>
        /// DWMs the extend frame into client area.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins m);

        /// <summary>
        /// Gets a value indicating if DWM is enabled
        /// </summary>
        /// <returns></returns>
        [DllImport(@"DwmApi.dll", PreserveSig = false)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// DWMs the enable composition.
        /// </summary>
        /// <param name="compositionAction">The composition action.</param>
        /// <returns></returns>
        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmEnableComposition(CompositionEnable compositionAction);

        /// <summary>
        /// Gets the window rect.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        [DllImport("useDr32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref Rect rect);

        /// <summary>
        /// Gets the client rect.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hwnd, ref Rect rect);

        #endregion

        #region Window Handling

        /// <summary>
        /// Destroys the window.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns></returns>
        [DllImport(CommonDllNames.User32, SetLastError = true, EntryPoint = "DestroyWindow", CallingConvention = CallingConvention.StdCall)]
        internal static extern int DestroyWindow(IntPtr handle);

        #endregion

        #region General Declarations

        /// <summary>
        ///   Various important window messages
        /// </summary>
        internal const int WMUser = 0x0400;

        /// <summary>
        /// </summary>
        internal const int WMEnteridle = 0x0121;

        /// <summary>
        ///   FormatMessage constants and structs.
        /// </summary>
        internal const int FormatMessageFromSystem = 0x00001000;

        /// <summary>
        ///   Application recovery and restart return codes
        /// </summary>
        internal const uint ResultFailed = 0x80004005;

        /// <summary>
        /// </summary>
        internal const uint ResultInvalidArgument = 0x80070057;

        /// <summary>
        /// </summary>
        internal const uint ResultFalse = 1;

        /// <summary>
        /// </summary>
        internal const uint ResultNotFound = 0x80070490;

        /// <summary>
        /// Gets the HiWord
        /// </summary>
        /// <param name="dword">The value to get the hi word from.</param>
        /// <param name="size">Size</param>
        /// <returns>The upper half of the dword.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dword")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HIWORD")]
        public static int HIWORD(long dword, int size)
        {
            return (short)(dword >> size);
        }

        /// <summary>
        /// Gets the LoWord
        /// </summary>
        /// <param name="dword">The value to get the low word from.</param>
        /// <returns>The lower half of the dword.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LOWORD")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dword")]
        public static int LOWORD(long dword)
        {
            return (short)(dword & 0xFFFF);
        }

        #endregion

        #region GDI and DWM Declarations

        /// <summary>
        ///   Enable/disable non-client rendering based on window style.
        /// </summary>
        internal const int DwmncrpUsewindowstyle = 0;

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
        internal const int DwmNcRenderingEnabled = 1;

        /// <summary>
        ///   Non-client rendering policy.
        /// </summary>
        internal const int DwmNcRenderingPolicy = 2;

        /// <summary>
        ///   Potentially enable/forcibly disable transitions 0 or 1.
        /// </summary>
        internal const int DwmwaTransitionsForcedisabled = 3;

        /// <summary>
        ///   fEnable has been specified
        /// </summary>
        internal const int DwmBBEnable = 0x00000001;

        /// <summary>
        ///   hRgnBlur has been specified
        /// </summary>
        internal const int DwmBlurRegion = 0x00000002;

        /// <summary>
        ///   fTransitionOnMaximized has been specified
        /// </summary>
        internal const int DwmTransitiononMaximized = 0x00000004;

        #region Nested type: CompositionEnable

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

        #endregion

        #region Nested type: DwmBlurBehind

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DwmBlurBehind
        {
            /// <summary>
            /// </summary>
            public DwmBlurBehindDWFlag DWFlags;

            /// <summary>
            /// </summary>
            public bool FEnable;

            /// <summary>
            /// </summary>
            public IntPtr HRgnBlur;

            /// <summary>
            /// </summary>
            public bool FTransitionOnMaximized;
        }

        #endregion

        #region Nested type: DwmBlurBehindDwFlags

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
            DwmBBTransitiononMaximized = 0x00000004
        }

        #endregion

        #region Nested type: DwmMessages

        /// <summary>
        /// </summary>
        internal static class DwmMessages
        {
            #region Constants and Fields

            /// <summary>
            /// </summary>
            internal const int WMDwmCompositionChanged = 0x031E;

            /// <summary>
            /// </summary>
            internal const int WMDwmnRenderingChanged = 0x031F;

            #endregion
        }

        #endregion

        #region Nested type: DwmPresentParameters

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DwmPresentParameters
        {
            /// <summary>
            /// </summary>
            internal int size;

            /// <summary>
            /// </summary>
            internal bool queue;

            /// <summary>
            /// </summary>
            internal ulong refreshStart;

            /// <summary>
            /// </summary>
            internal uint buffer;

            /// <summary>
            /// </summary>
            internal bool useSourceRate;

            /// <summary>
            /// </summary>
            internal UnsignedRatio uiNumerator;
        }

        #endregion

        #region Nested type: DwmThumbnailFlags

        /// <summary>
        /// The Dwm thumbnail flag
        /// </summary>
        internal enum DwmThumbnailFlag : uint
        {
            /// <summary>
            /// Indicates a value for rcDestination has been specified.
            /// </summary>
            DwmTnpRectDestination = 0x00000001,

            /// <summary>
            /// Indicates a value for rc source has been specified.
            /// </summary>
            DwmTnpRectsource = 0x00000002,

            /// <summary>
            /// Indicates a value for opacity
            /// </summary>
            DwmTnpOpacity = 0x00000004,

            /// <summary>
            /// Indicates a value for visible has been specified.
            /// </summary>
            DwmTnpVisible = 0x00000008,

            /// <summary>
            /// Indicates a value for fSourceClientAreaOnly has been specified.
            /// </summary>
            DwmTnpSourceclientareaonly = 0x00000010
        }

        #endregion

        #region Nested type: DwmThumbnailProperties

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DwmThumbnailProperties
        {
            /// <summary>
            /// </summary>
            internal DwmThumbnailFlag dwFlags;

            /// <summary>
            /// </summary>
            internal Rect destination;

            /// <summary>
            /// </summary>
            internal Rect source;

            /// <summary>
            /// </summary>
            internal byte opacity;

            /// <summary>
            /// </summary>
            internal bool visible;

            /// <summary>
            /// </summary>
            internal bool sourceClientAreaOnly;
        }

        #endregion

        #region Nested type: MARGINS

        /// <summary>
        /// The margins that specify top, left, right, down
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            /// <summary>
            /// width of left border that retains its size
            /// </summary>
            public int leftWidth;

            /// <summary>
            /// width of right border that retains its size
            /// </summary>
            public int rightWidth;

            /// <summary>
            /// height of top border that retains its size
            /// </summary>
            public int topHeight;

            /// <summary>
            ///  height of bottom border that retains its size
            /// </summary>
            public int bottomHeight;

            /// <summary>
            /// Initializes a new instance of the <see cref="Margins"/> struct.
            /// </summary>
            /// <param name="fullWindow">if set to <see langword="true"/> [full window].</param>
            public Margins(bool fullWindow)
            {
                this.leftWidth = this.rightWidth = this.topHeight = this.bottomHeight = fullWindow ? -1 : 0;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Margins"/> struct.
            /// </summary>
            /// <param name="left">The left margin</param>
            /// <param name="top">The top margin</param>
            /// <param name="right">The right margin</param>
            /// <param name="bottom">The bottom margin</param>
            public Margins(int left, int top, int right, int bottom)
            {
                this.leftWidth = left;
                this.rightWidth = right;
                this.topHeight = top;
                this.bottomHeight = bottom;
            }
        }

        #endregion

        #region Nested type: RECT

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

        #endregion

        #region Nested type: SIZE

        /// <summary>
        /// A Wrapper for a SIZE struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SIZE")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            /// <summary>
            ///   Width
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cx")]
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int x;

            /// <summary>
            ///   Height
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int y;
        }

        #endregion

        #region Nested type: UNSIGNED_RATIO

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct UnsignedRatio
        {
            /// <summary>
            /// </summary>
            internal uint uiNumerator;

            /// <summary>
            /// </summary>
            internal uint uiDenominator;
        }

        #endregion

        #endregion

        #region Elevation COM Object

        #region Nested type: BIND_OPTS3

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct BindOptS3
        {
            /// <summary>
            /// </summary>
            internal uint cbStruct;

            /// <summary>
            /// </summary>
            internal uint flags;

            /// <summary>
            /// </summary>
            internal uint mode;

            /// <summary>
            /// </summary>
            internal uint dwTickCountDeadline;

            /// <summary>
            /// </summary>
            internal uint dwTrackFlags;

            /// <summary>
            /// </summary>
            internal uint dwClassContext;

            /// <summary>
            /// </summary>
            internal uint locale;

            /// <summary>
            /// This will be passed as <see langword="null"/>, so the type doesn't matter.
            /// </summary>
            private readonly object serverInfo;

            /// <summary>
            /// </summary>
            internal IntPtr hwnd;
        }

        #endregion

        #region Nested type: CLSCTX

        /// <summary>
        /// </summary>
        [Flags]
        internal enum LSCTXs
        {
            /// <summary>
            /// </summary>
            ClsctxInprocServer = 0x1, 

            /// <summary>
            /// </summary>
            ClsctxInprocHandler = 0x2, 

            /// <summary>
            /// </summary>
            ClsctxLocalServer = 0x4, 

            /// <summary>
            /// </summary>
            ClsctxRemoteServer = 0x10, 

            /// <summary>
            /// </summary>
            ClsctxnoCodeDownload = 0x400, 

            /// <summary>
            /// </summary>
            ClsctxnoCustomMarshal = 0x1000, 

            /// <summary>
            /// </summary>
            ClsctxEnableCodeDownload = 0x2000, 

            /// <summary>
            /// </summary>
            ClsctxnoFailureLog = 0x4000, 

            /// <summary>
            /// </summary>
            ClsctxDisableAaa = 0x8000, 

            /// <summary>
            /// </summary>
            ClsctxEnableAaa = 0x10000, 

            /// <summary>
            /// </summary>
            ClsctxFromDefaultContext = 0x20000, 

            /// <summary>
            /// </summary>
            ClsctxInproc = ClsctxInprocServer | ClsctxInprocHandler, 

            /// <summary>
            /// </summary>
            ClsctxServer = ClsctxInprocServer | ClsctxLocalServer | ClsctxRemoteServer, 

            /// <summary>
            /// </summary>
            ClsctxAll = ClsctxServer | ClsctxInprocHandler
        }

        #endregion

        #endregion

        #region Windows OS structs and consts

        // Code for CreateWindowEx, for a windowless message pump.
        /// <summary>
        /// </summary>
        internal const int HwndMessage = -3;

        /// <summary>
        /// </summary>
        internal const uint StatusAccessDenied = 0xC0000022;

        #region Nested type: MSG

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct Msg
        {
            /// <summary>
            /// </summary>
            internal IntPtr hwnd;

            /// <summary>
            /// </summary>
            internal uint message;

            /// <summary>
            /// </summary>
            internal IntPtr wParam;

            /// <summary>
            /// </summary>
            internal IntPtr lParam;

            /// <summary>
            /// </summary>
            internal uint time;

            /// <summary>
            /// </summary>
            internal Point pt;
        }

        #endregion

        #region Nested type: POINT

        /// <summary>
        /// A Wrapper for a POINT struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "POINT")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            /// <summary>
            ///   The X coordinate of the point
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int X;

            /// <summary>
            ///   The Y coordinate of the point
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int Y;

            /// <summary>
            /// Initialize the point
            /// </summary>
            /// <param name="x">
            /// The x coordinate of the point.
            /// </param>
            /// <param name="y">
            /// The y coordinate of the point.
            /// </param>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        #endregion

        #region Nested type: WNDCLASSEX

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WNDCLASSEX
        {
            /// <summary>
            /// </summary>
            internal uint cbSize;

            /// <summary>
            /// </summary>
            internal uint style;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.FunctionPtr)]
            internal WNDPROC lpfnWndProc;

            /// <summary>
            /// </summary>
            internal int cbClsExtra;

            /// <summary>
            /// </summary>
            internal int cbWndExtra;

            /// <summary>
            /// </summary>
            internal IntPtr hInstance;

            /// <summary>
            /// </summary>
            internal IntPtr hIcon;

            /// <summary>
            /// </summary>
            internal IntPtr hCursor;

            /// <summary>
            /// </summary>
            internal IntPtr hbrBackground;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string lpszMenuName;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string lpszClassName;

            /// <summary>
            /// </summary>
            internal IntPtr hIconSm;
        }

        #endregion


        internal delegate int WNDPROC(IntPtr hWnd, uint uMessage, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}