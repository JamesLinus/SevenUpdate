//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Wrappers for Native Methods and Structs.
    ///   This type is intended for internal use only
    /// </summary>
    public static class CoreNativeMethods
    {
        #region General Definitions

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls 
        ///   the window procedure for the specified window and does not return until the window 
        ///   procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">
        /// Handle to the window whose window procedure will receive the message. 
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, 
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows; 
        ///   but the message is not sent to child windows.
        /// </param>
        /// <param name="msg">
        /// Specifies the message to be sent.
        /// </param>
        /// <param name="wParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <param name="lParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnd")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", 
            Justification = "This is used from another assembly, also it's in an internal namespace")]
        [DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls 
        ///   the window procedure for the specified window and does not return until the window 
        ///   procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">
        /// Handle to the window whose window procedure will receive the message. 
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, 
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows; 
        ///   but the message is not sent to child windows.
        /// </param>
        /// <param name="msg">
        /// Specifies the message to be sent.
        /// </param>
        /// <param name="wParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <param name="lParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnd")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible")]
        [DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls 
        ///   the window procedure for the specified window and does not return until the window 
        ///   procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">
        /// Handle to the window whose window procedure will receive the message. 
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, 
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows; 
        ///   but the message is not sent to child windows.
        /// </param>
        /// <param name="msg">
        /// Specifies the message to be sent.
        /// </param>
        /// <param name="wParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <param name="lParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnd")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "w")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "This is an in/out parameter")]
        [DllImport(CommonDllNames.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, ref int wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        // Various helpers for forcing binding to proper 
        // version of Comctl32 (v6).
        /// <summary>
        /// </summary>
        /// <param name="lpFileName">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport(CommonDllNames.Kernel32, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        /// <summary>
        /// </summary>
        /// <param name="graphicsObjectHandle">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport(CommonDllNames.User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr graphicsObjectHandle);

        /// <summary>
        /// </summary>
        /// <param name="hInstance">
        /// </param>
        /// <param name="uID">
        /// </param>
        /// <param name="buffer">
        /// </param>
        /// <param name="nBufferMax">
        /// </param>
        /// <returns>
        /// </returns>
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
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h")]
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", 
            Justification = "This is used from other assemblies, also it's in an internal namespace")]
        [DllImport(CommonDllNames.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", EntryPoint = "IsUserAnAdmin", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsUserAnAdmin();

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="bb">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("DwmApi.dll")]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr hwnd, ref DwmBlurBehind bb);

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="m">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS m);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [DllImport("DwmApi.dll", PreserveSig = false)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// </summary>
        /// <param name="compositionAction">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("DwmApi.dll")]
        internal static extern int DwmEnableComposition(CompositionEnable compositionAction);

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="rect">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="rect">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hwnd, ref RECT rect);

        // <summary>
        /// </summary>
        /// <param name="nLeftRect">
        /// </param>
        /// <param name="nTopRect">
        /// </param>
        /// <param name="nRightRect">
        /// </param>
        /// <param name="nBottomRect">
        /// </param>
        /// <returns>
        /// </returns>
        // COMMENTED BY CODEIT.RIGHT
        // [DllImport("gdi32")]
        // private static extern IntPtr CreateEllipticRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        /// <summary>
        /// </summary>
        /// <param name="nLeftRect">
        /// </param>
        /// <param name="nTopRect">
        /// </param>
        /// <param name="nRightRect">
        /// </param>
        /// <param name="nBottomRect">
        /// </param>
        /// <returns>
        /// </returns>
        // COMMENTED BY CODEIT.RIGHT
        // [DllImport("gdi32")]
        // private static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        #endregion

        #region Window Handling

        /// <summary>
        /// </summary>
        /// <param name="handle">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport(CommonDllNames.User32, SetLastError = true, EntryPoint = "DestroyWindow", CallingConvention = CallingConvention.StdCall)]
        internal static extern int DestroyWindow(IntPtr handle);

        #endregion

        #region General Declarations

        // Various important window messages
        /// <summary>
        /// </summary>
        internal const int WMUser = 0x0400;

        /// <summary>
        /// </summary>
        internal const int WMEnteridle = 0x0121;

        // FormatMessage constants and structs.
        /// <summary>
        /// </summary>
        internal const int FormatMessageFromSystem = 0x00001000;

        // App recovery and restart return codes
        /// <summary>
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
        /// <param name="dword">
        /// The value to get the hi word from.
        /// </param>
        /// <param name="size">
        /// Size
        /// </param>
        /// <returns>
        /// The upper half of the dword.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dword")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HIWORD")]
        public static int HIWORD(long dword, int size)
        {
            return (short)(dword >> size);
        }

        /// <summary>
        /// Gets the LoWord
        /// </summary>
        /// <param name="dword">
        /// The value to get the low word from.
        /// </param>
        /// <returns>
        /// The lower half of the dword.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LOWORD")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dword")]
        public static int LOWORD(long dword)
        {
            return (short)(dword & 0xFFFF);
        }

        #endregion

        #region GDI and DWM Declarations

        // Enable/disable non-client rendering based on window style.
        /// <summary>
        /// </summary>
        internal const int DwmncrpUsewindowstyle = 0;

        // Disabled non-client rendering; window style is ignored.
        /// <summary>
        /// </summary>
        internal const int DwmncrpDisabled = 1;

        // Enabled non-client rendering; window style is ignored.
        /// <summary>
        /// </summary>
        internal const int DwmncrpEnabled = 2;

        // Enable/disable non-client rendering Use DWMNCRP_* values.
        /// <summary>
        /// </summary>
        internal const int DwmwaNcrenderingEnabled = 1;

        // Non-client rendering policy.
        /// <summary>
        /// </summary>
        internal const int DwmwaNcrenderingPolicy = 2;

        // Potentially enable/forcibly disable transitions 0 or 1.
        /// <summary>
        /// </summary>
        internal const int DwmwaTransitionsForcedisabled = 3;

        /// <summary>
        /// </summary>
        internal const int DwmBBEnable = 0x00000001; // fEnable has been specified

        /// <summary>
        /// </summary>
        internal const int DwmBBBlurregion = 0x00000002; // hRgnBlur has been specified

        /// <summary>
        /// </summary>
        internal const int DwmBBTransitiononmaximized = 0x00000004; // fTransitionOnMaximized has been specified

        #region Nested type: CompositionEnable

        /// <summary>
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

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(DwmBlurBehind x, DwmBlurBehind y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(DwmBlurBehind x, DwmBlurBehind y)
            {
                throw new NotImplementedException();
            }
        } ;

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
            internal int cbSize;

            /// <summary>
            /// </summary>
            internal bool fQueue;

            /// <summary>
            /// </summary>
            internal ulong cRefreshStart;

            /// <summary>
            /// </summary>
            internal uint cBuffer;

            /// <summary>
            /// </summary>
            internal bool fUseSourceRate;

            /// <summary>
            /// </summary>
            internal UnsignedRatio uiNumerator;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(DwmPresentParameters x, DwmPresentParameters y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(DwmPresentParameters x, DwmPresentParameters y)
            {
                throw new NotImplementedException();
            }
        } ;

        #endregion

        #region Nested type: DwmThumbnailFlags

        /// <summary>
        /// </summary>
        internal enum DwmThumbnailFlag : uint
        {
            /// <summary>
            /// </summary>
            DwmTnpRectdestination = 0x00000001, // Indicates a value for rcDestination has been specified.
            /// <summary>
            /// </summary>
            DwmTnpRectsource = 0x00000002, // Indicates a value for rcSource has been specified.
            /// <summary>
            /// </summary>
            DwmTnpOpacity = 0x00000004, // Indicates a value for opacity has been specified.
            /// <summary>
            /// </summary>
            DwmTnpVisible = 0x00000008, // Indicates a value for fVisible has been specified.
            /// <summary>
            /// </summary>
            DwmTnpSourceclientareaonly = 0x00000010 // Indicates a value for fSourceClientAreaOnly has been specified.
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
            internal RECT rcDestination;

            /// <summary>
            /// </summary>
            internal RECT rcSource;

            /// <summary>
            /// </summary>
            internal byte opacity;

            /// <summary>
            /// </summary>
            internal bool fVisible;

            /// <summary>
            /// </summary>
            internal bool fSourceClientAreaOnly;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(DwmThumbnailProperties x, DwmThumbnailProperties y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(DwmThumbnailProperties x, DwmThumbnailProperties y)
            {
                throw new NotImplementedException();
            }
        } ;

        #endregion

        #region Nested type: MARGINS

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            /// <summary>
            /// </summary>
            public int CXLeftWidth; // width of left border that retains its size

            /// <summary>
            /// </summary>
            public int CXRightWidth; // width of right border that retains its size

            /// <summary>
            /// </summary>
            public int CYTopHeight; // height of top border that retains its size

            /// <summary>
            /// </summary>
            public int CYBottomHeight; // height of bottom border that retains its size

            /// <summary>
            /// </summary>
            /// <param name="fullWindow">
            /// </param>
            public MARGINS(bool fullWindow)
            {
                this.CXLeftWidth = this.CXRightWidth = this.CYTopHeight = this.CYBottomHeight = fullWindow ? -1 : 0;
            }

            /// <summary>
            /// </summary>
            /// <param name="left">
            /// </param>
            /// <param name="top">
            /// </param>
            /// <param name="right">
            /// </param>
            /// <param name="bottom">
            /// </param>
            public MARGINS(int left, int top, int right, int bottom)
            {
                this.CXLeftWidth = left;
                this.CXRightWidth = right;
                this.CYTopHeight = top;
                this.CYBottomHeight = bottom;
            }

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(MARGINS x, MARGINS y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(MARGINS x, MARGINS y)
            {
                throw new NotImplementedException();
            }
        } ;

        #endregion

        #region Nested type: RECT

        /// <summary>
        /// A Wrapper for a RECT struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RECT")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
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

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(RECT x, RECT y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(RECT x, RECT y)
            {
                throw new NotImplementedException();
            }
        } ;

        #endregion

        #region Nested type: SIZE

        /// <summary>
        /// A Wrapper for a SIZE struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SIZE")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            /// <summary>
            ///   Width
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cx")]
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int CX;

            /// <summary>
            ///   Height
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            public int CY;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(SIZE x, SIZE y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(SIZE x, SIZE y)
            {
                throw new NotImplementedException();
            }
        } ;

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

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(UnsignedRatio x, UnsignedRatio y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(UnsignedRatio x, UnsignedRatio y)
            {
                throw new NotImplementedException();
            }
        } ;

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
            internal uint grfFlags;

            /// <summary>
            /// </summary>
            internal uint grfMode;

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

            // This will be passed as null, so the type doesn't matter.
            /// <summary>
            /// </summary>
            private readonly object pServerInfo;

            /// <summary>
            /// </summary>
            internal IntPtr hwnd;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(BindOptS3 x, BindOptS3 y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(BindOptS3 x, BindOptS3 y)
            {
                throw new NotImplementedException();
            }
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
        internal struct MSG
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
            internal POINT pt;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(MSG x, MSG y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(MSG x, MSG y)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Nested type: POINT

        /// <summary>
        /// A Wrapper for a POINT struct
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "POINT")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
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
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(POINT x, POINT y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(POINT x, POINT y)
            {
                throw new NotImplementedException();
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

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(WNDCLASSEX x, WNDCLASSEX y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(WNDCLASSEX x, WNDCLASSEX y)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Nested type: WNDPROC

        /// <summary>
        /// </summary>
        /// <param name="hWnd">
        /// </param>
        /// <param name="uMessage">
        /// </param>
        /// <param name="wParam">
        /// </param>
        /// <param name="lParam">
        /// </param>
        internal delegate int WNDPROC(IntPtr hWnd, uint uMessage, IntPtr wParam, IntPtr lParam);

        #endregion

        #endregion
    }
}