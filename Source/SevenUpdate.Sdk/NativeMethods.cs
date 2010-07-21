#region

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

#endregion

namespace SevenUpdate.Sdk
{
    //public class NativeMethods
    //{
    //    [DllImport("uxtheme.dll", PreserveSig = false)]
    //    public static extern void SetWindowThemeAttribute([In] IntPtr hwnd, [In] WINDOWTHEMEATTRIBUTETYPE eAttribute, [In] ref WTA_OPTIONS pvAttribute, [In] uint cbAttribute);

    //    [DllImport("DwmApi.dll")]
    //    public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins pMarInset);

    //    [DllImport("DwmApi.dll")]
    //    public static extern void DwmIsCompositionEnabled(ref bool isEnabled);

    //    [DllImport("uxtheme.dll")]
    //    public static extern int SetWindowThemeAttribute(IntPtr hWnd, WindowThemeAttributeType wtype, ref WTA_OPTIONS attributes, uint size);
    //}

    //public class AeroHelpers
    //{
    //    private void SetWindowThemeAttribute(Window window, bool showCaption, bool showIcon)
    //    {
    //        bool isGlassEnabled = NativeMethods.DwmIsCompositionEnabled();

    //        IntPtr hwnd = new WindowInteropHelper(window).Handle;

    //        var options = new NativeMethods.WTA_OPTIONS {dwMask = (NativeMethods.WTNCA.NODRAWCAPTION | NativeMethods.WTNCA.NODRAWICON)};
    //        if (isGlassEnabled)
    //        {
    //            if (!showCaption)
    //                options.dwFlags |= NativeMethods.WTNCA.NODRAWCAPTION;
    //            if (!showIcon)
    //                options.dwFlags |= NativeMethods.WTNCA.NODRAWICON;
    //        }

    //        NativeMethods.SetWindowThemeAttribute(hwnd, NativeMethods.WINDOWTHEMEATTRIBUTETYPE.WTA_NONCLIENT, ref options, NativeMethods.WTA_OPTIONS.Size);
    //    }


    //    private void EnableGlass(Window window)
    //    {
    //        try
    //        {
    //            // Obtain the window handle for WPF application
    //            var mainWindowPtr = new WindowInteropHelper(window).Handle;
    //            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
    //            mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

    //            // Get System Dpi
    //            var desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
    //            float desktopDpiX = desktop.DpiX;

    //            // Set Margins
    //            var margins = new NativeMethods.Margins
    //            {
    //                Left = Convert.ToInt32(1 * (desktopDpiX / 96)),
    //                Right = Convert.ToInt32(1 * (desktopDpiX / 96)),
    //                Top = Convert.ToInt32(45 * (desktopDpiX / 96)),
    //                Bottom = Convert.ToInt32(45 * (desktopDpiX / 96))
    //            };

    //            // Extend glass frame into client area
    //            // Note that the default desktop Dpi is 96dpi. The margins are adjusted for the system Dpi.

    //            int hr = NativeMethods.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
    //            //
    //            if (hr < 0)
    //            {
    //                //DwmExtendFrameIntoClientArea Failed
    //            }
    //        }
    //        // If not Vista, paint background white.
    //        catch (DllNotFoundException)
    //        {
    //            window.Background = Brushes.White;
    //        }
    //    }
    //}
}