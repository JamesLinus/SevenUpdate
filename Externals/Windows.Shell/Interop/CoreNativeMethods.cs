//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Microsoft.Windows.Internal
{
    /// <summary>
    ///   Wrappers for Native Methods and Structs.
    ///   This type is intended for internal use only
    /// </summary>
    public static class CoreNativeMethods
    {
        #region Common Defintions

        #endregion

        #region General Definitions

        /// <summary>
        ///   Sends the specified message to a window or windows. The SendMessage function calls 
        ///   the window procedure for the specified window and does not return until the window 
        ///   procedure has processed the message.
        /// </summary>
        /// <param name = "hWnd">Handle to the window whose window procedure will receive the message. 
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, 
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows; 
        ///   but the message is not sent to child windows.
        /// </param>
        /// <param name = "msg">Specifies the message to be sent.</param>
        /// <param name = "wParam">Specifies additional message-specific information.</param>
        /// <param name = "lParam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnd"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param"),
         SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "This is used from another assembly, also it's in an internal namespace"),
         DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        ///   Sends the specified message to a window or windows. The SendMessage function calls 
        ///   the window procedure for the specified window and does not return until the window 
        ///   procedure has processed the message.
        /// </summary>
        /// <param name = "hWnd">Handle to the window whose window procedure will receive the message. 
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, 
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows; 
        ///   but the message is not sent to child windows.
        /// </param>
        /// <param name = "msg">Specifies the message to be sent.</param>
        /// <param name = "wParam">Specifies additional message-specific information.</param>
        /// <param name = "lParam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnd"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param"), SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"),
         DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        ///   Sends the specified message to a window or windows. The SendMessage function calls 
        ///   the window procedure for the specified window and does not return until the window 
        ///   procedure has processed the message.
        /// </summary>
        /// <param name = "hWnd">Handle to the window whose window procedure will receive the message. 
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, 
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows; 
        ///   but the message is not sent to child windows.
        /// </param>
        /// <param name = "msg">Specifies the message to be sent.</param>
        /// <param name = "wParam">Specifies additional message-specific information.</param>
        /// <param name = "lParam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnd"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w"), SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"),
         SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "This is an in/out parameter"),
         DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, ref int wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        // Various helpers for forcing binding to proper 
        // version of Comctl32 (v6).
        [DllImport(CommonDllNames.Kernel32, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport(CommonDllNames.User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr graphicsObjectHandle);

        [DllImport(CommonDllNames.User32, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int LoadString(IntPtr hInstance, int uID, StringBuilder buffer, int nBufferMax);

        /// <summary>
        ///   Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name = "hIcon">Handle to the icon to be destroyed. The icon must not be in use. </param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h"),
         SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "This is used from other assemblies, also it's in an internal namespace"),
         DllImport(CommonDllNames.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        #endregion

        #region Window Handling

        [DllImport(CommonDllNames.User32, SetLastError = true, EntryPoint = "DestroyWindow", CallingConvention = CallingConvention.StdCall)]
        internal static extern int DestroyWindow(IntPtr handle);

        #endregion

        #region General Declarations

        // Various important window messages
        internal const int WM_USER = 0x0400;
        internal const int WM_ENTERIDLE = 0x0121;

        // FormatMessage constants and structs.
        internal const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        // App recovery and restart return codes
        internal const uint ResultFailed = 0x80004005;
        internal const uint ResultInvalidArgument = 0x80070057;
        internal const uint ResultFalse = 1;
        internal const uint ResultNotFound = 0x80070490;

        /// <summary>
        ///   Gets the HiWord
        /// </summary>
        /// <param name = "dword">The value to get the hi word from.</param>
        /// <param name = "size">Size</param>
        /// <returns>The upper half of the dword.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dword"),
         SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HIWORD")]
        public static int HIWORD(long dword, int size)
        {
            return (short) (dword >> size);
        }

        /// <summary>
        ///   Gets the LoWord
        /// </summary>
        /// <param name = "dword">The value to get the low word from.</param>
        /// <returns>The lower half of the dword.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LOWORD"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dword")]
        public static int LOWORD(long dword)
        {
            return (short) (dword & 0xFFFF);
        }

        #endregion

        #region GDI and DWM Declarations

        // Enable/disable non-client rendering based on window style.
        internal const int DWMNCRP_USEWINDOWSTYLE = 0;
        // Disabled non-client rendering; window style is ignored.
        internal const int DWMNCRP_DISABLED = 1;
        // Enabled non-client rendering; window style is ignored.
        internal const int DWMNCRP_ENABLED = 2;
        // Enable/disable non-client rendering Use DWMNCRP_* values.
        internal const int DWMWA_NCRENDERING_ENABLED = 1;
        // Non-client rendering policy.
        internal const int DWMWA_NCRENDERING_POLICY = 2;
        // Potentially enable/forcibly disable transitions 0 or 1.
        internal const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;

        internal const int DWM_BB_ENABLE = 0x00000001; // fEnable has been specified
        internal const int DWM_BB_BLURREGION = 0x00000002; // hRgnBlur has been specified
        internal const int DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004; // fTransitionOnMaximized has been specified

        #region Nested type: DWM_BLURBEHIND

        [StructLayout(LayoutKind.Sequential)]
        internal struct DWM_BLURBEHIND
        {
            public DwmBlurBehindDwFlags dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        } ;

        #endregion

        #region Nested type: DWM_PRESENT_PARAMETERS

        [StructLayout(LayoutKind.Sequential)]
        internal struct DWM_PRESENT_PARAMETERS
        {
            internal int cbSize;
            internal bool fQueue;
            internal UInt64 cRefreshStart;
            internal uint cBuffer;
            internal bool fUseSourceRate;
            internal UNSIGNED_RATIO uiNumerator;
        } ;

        #endregion

        #region Nested type: DWM_THUMBNAIL_PROPERTIES

        [StructLayout(LayoutKind.Sequential)]
        internal struct DWM_THUMBNAIL_PROPERTIES
        {
            internal DwmThumbnailFlags dwFlags;
            internal RECT rcDestination;
            internal RECT rcSource;
            internal byte opacity;
            internal bool fVisible;
            internal bool fSourceClientAreaOnly;
        } ;

        #endregion

        #region Nested type: DwmBlurBehindDwFlags

        internal enum DwmBlurBehindDwFlags : uint
        {
            DWM_BB_ENABLE = 0x00000001,
            DWM_BB_BLURREGION = 0x00000002,
            DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004
        }

        #endregion

        #region Nested type: DwmThumbnailFlags

        internal enum DwmThumbnailFlags : uint
        {
            DWM_TNP_RECTDESTINATION = 0x00000001, //Indicates a value for rcDestination has been specified.
            DWM_TNP_RECTSOURCE = 0x00000002, //Indicates a value for rcSource has been specified.
            DWM_TNP_OPACITY = 0x00000004, //Indicates a value for opacity has been specified.
            DWM_TNP_VISIBLE = 0x00000008, // Indicates a value for fVisible has been specified.
            DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010 //Indicates a value for fSourceClientAreaOnly has been specified.
        }

        #endregion

        #region Nested type: MARGINS

        [StructLayout(LayoutKind.Sequential)]
        internal struct MARGINS
        {
            public int cxLeftWidth; // width of left border that retains its size
            public int cxRightWidth; // width of right border that retains its size
            public int cyTopHeight; // height of top border that retains its size
            public int cyBottomHeight; // height of bottom border that retains its size
        } ;

        #endregion

        #region Nested type: RECT

        /// <summary>
        ///   A Wrapper for a RECT struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RECT"), SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"),
         StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            ///   Position of left edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public int left;

            /// <summary>
            ///   Position of top edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public int top;

            /// <summary>
            ///   Position of right edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public int right;

            /// <summary>
            ///   Position of bottom edge
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public int bottom;
        } ;

        #endregion

        #region Nested type: SIZE

        /// <summary>
        ///   A Wrapper for a SIZE struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SIZE"), SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"),
         StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            /// <summary>
            ///   Width
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cx"), SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public int cx;

            /// <summary>
            ///   Height
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public int cy;
        } ;

        #endregion

        #region Nested type: UNSIGNED_RATIO

        [StructLayout(LayoutKind.Sequential)]
        internal struct UNSIGNED_RATIO
        {
            internal UInt32 uiNumerator;
            internal UInt32 uiDenominator;
        } ;

        #endregion

        #endregion

        #region Elevation COM Object

        #region Nested type: BIND_OPTS3

        [StructLayout(LayoutKind.Sequential)]
        internal struct BIND_OPTS3
        {
            internal uint cbStruct;
            internal uint grfFlags;
            internal uint grfMode;
            internal uint dwTickCountDeadline;
            internal uint dwTrackFlags;
            internal uint dwClassContext;
            internal uint locale;
            // This will be passed as null, so the type doesn't matter.
            private object pServerInfo;
            internal IntPtr hwnd;
        }

        #endregion

        #region Nested type: CLSCTX

        [Flags]
        internal enum CLSCTX
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

        #endregion

        #endregion

        #region Windows OS structs and consts

        // Code for CreateWindowEx, for a windowless message pump.
        internal const int HWND_MESSAGE = -3;

        internal const uint STATUS_ACCESS_DENIED = 0xC0000022;

        #region Nested type: MSG

        [StructLayout(LayoutKind.Sequential)]
        internal struct MSG
        {
            internal IntPtr hwnd;
            internal uint message;
            internal IntPtr wParam;
            internal IntPtr lParam;
            internal uint time;
            internal POINT pt;
        }

        #endregion

        #region Nested type: POINT

        /// <summary>
        ///   A Wrapper for a POINT struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "POINT"), SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"),
         StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            ///   The X coordinate of the point
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X"), SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public
                int X;

            /// <summary>
            ///   The Y coordinate of the point
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y"), SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] public
                int Y;

            /// <summary>
            ///   Initialize the point
            /// </summary>
            /// <param name = "x">The x coordinate of the point.</param>
            /// <param name = "y">The y coordinate of the point.</param>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"),
             SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        #endregion

        #region Nested type: WNDCLASSEX

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WNDCLASSEX
        {
            internal uint cbSize;
            internal uint style;

            [MarshalAs(UnmanagedType.FunctionPtr)] internal WNDPROC lpfnWndProc;

            internal int cbClsExtra;
            internal int cbWndExtra;
            internal IntPtr hInstance;
            internal IntPtr hIcon;
            internal IntPtr hCursor;
            internal IntPtr hbrBackground;

            [MarshalAs(UnmanagedType.LPTStr)] internal string lpszMenuName;

            [MarshalAs(UnmanagedType.LPTStr)] internal string lpszClassName;

            internal IntPtr hIconSm;
        }

        #endregion

        #region Nested type: WNDPROC

        internal delegate int WNDPROC(IntPtr hWnd, uint uMessage, IntPtr wParam, IntPtr lParam);

        #endregion

        #endregion
    }
}