#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace SevenUpdate.Sdk
{
    public class NativeMethods
    {
        // consts for wndproc
        internal const int WM_NCHITTEST = 0x84;
        internal const int HTCLIENT = 1;
        internal const int HTCAPTION = 2;

        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins pMarInset);

        [DllImport("DwmApi.dll")]
        internal static extern void DwmIsCompositionEnabled(ref bool isEnabled);

        #region Nested type: Margins

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int Left, Right, Top, Bottom;
        }

        #endregion
    }
}