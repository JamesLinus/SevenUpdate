//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace Microsoft.Windows.Internal
{
    internal static class DwmMessages
    {
        internal const int WmDwmCompositionChanged = 0x031E;
        internal const int WmDwmnRenderingChanged = 0x031F;
    }

    struct POINTAPI
    {
        public int x;
        public int y;
    };

    struct DTTOPTS
    {
        public uint dwSize;
        public uint dwFlags;
        public uint crText;
        public uint crBorder;
        public uint crShadow;
        public int iTextShadowType;
        public POINTAPI ptShadowOffset;
        public int iBorderSize;
        public int iFontPropId;
        public int iColorPropId;
        public int iStateId;
        public int fApplyOverlay;
        public int iGlowSize;
        public IntPtr pfnDrawTextCallback;
        public int lParam;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int cxLeftWidth; // width of left border that retains its size
        public int cxRightWidth; // width of right border that retains its size
        public int cyTopHeight; // height of top border that retains its size
        public int cyBottomHeight; // height of bottom border that retains its size

        public Margins(bool fullWindow)
        {
            cxLeftWidth = cxRightWidth = cyTopHeight = cyBottomHeight = (fullWindow ? -1 : 0);
        }

        public Margins(int left, int top, int right, int bottom)
        {
            cxLeftWidth = left;
            cxRightWidth = right;
            cyTopHeight = top;
            cyBottomHeight = bottom;
        }
    } ;

    internal enum DwmBlurBehindDwFlags : uint
    {
        DwmBbEnable = 0x00000001,
        DwmBbBlurregion = 0x00000002,
        DwmBbTransitiononMaximized = 0x00000004
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DwmBlurBehind
    {
        public DwmBlurBehindDwFlags dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    } ;

    internal enum CompositionEnable : uint
    {
        DwmEcDisableComposition = 0,
        DwmEcEnableComposition = 1
    }

    /// <summary>
    ///   Internal class that contains interop declarations for 
    ///   functions that are not benign and are performance critical.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static class DwmNativeMethods
    {
        [DllImport("DwmApi.dll")]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr hwnd, ref DwmBlurBehind bb);

        [DllImport("DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins m);

        [DllImport("DwmApi.dll", PreserveSig = false)]
        internal static extern bool DwmIsCompositionEnabled();

        [DllImport("DwmApi.dll")]
        internal static extern int DwmEnableComposition(CompositionEnable compositionAction);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref CoreNativeMethods.RECT rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hwnd, ref CoreNativeMethods.RECT rect);

        [DllImport("gdi32")]
        public static extern IntPtr CreateEllipticRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("UxTheme.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref CoreNativeMethods.RECT pRect, ref DTTOPTS pOptions);
        
        [DllImport("UxTheme.dll", ExactSpelling = true, SetLastError = true)]
        private static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags1, int dwFlags2, ref CoreNativeMethods.RECT pRect);
    }
}