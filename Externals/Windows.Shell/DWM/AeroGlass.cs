#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Dwm
{
    /// <summary>
    ///   WPF Glass Window
    ///   Inherit from this window class to enable glass on a WPF window
    /// </summary>
    public abstract class AeroGlass
    {
        #region properties

        /// <summary>
        ///   Get determines if AeroGlass is enabled on the desktop. Set enables/disables AreoGlass on the desktop.
        /// </summary>
        public static bool IsEnabled
        {
            set
            {
                try
                {
                    CoreNativeMethods.DwmEnableComposition(value ? CoreNativeMethods.CompositionEnable.DwmEcEnableComposition : CoreNativeMethods.CompositionEnable.DwmEcDisableComposition);
                }
                catch
                {
                }
            }
            get
            {
                try
                {
                    return CoreNativeMethods.DwmIsCompositionEnabled();
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion

        #region operations

        /// <summary>
        ///   Excludes a UI element from the AeroGlass frame.
        /// </summary>
        /// <param name = "element">The element to exclude.</param>
        /// <param name = "window">The window the element resides in</param>
        /// <remarks>
        ///   c
        ///   Many non-WPF rendered controls (i.e., the ExplorerBrowser control) will not 
        ///   render properly on top of an AeroGlass frame.
        /// </remarks>
        public void ExcludeElementFromAeroGlass(FrameworkElement element, Window window)
        {
            var hWnd = new WindowInteropHelper(window).Handle;

            if (!IsEnabled)
                return;
            // calculate total size of window nonclient area
            var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            var windowRect = new CoreNativeMethods.RECT();
            var clientRect = new CoreNativeMethods.RECT();
            CoreNativeMethods.GetWindowRect(hwndSource.Handle, ref windowRect);
            CoreNativeMethods.GetClientRect(hwndSource.Handle, ref clientRect);
            var nonClientSize = new Size((windowRect.right - windowRect.left) - (double) (clientRect.right - clientRect.left),
                                         (windowRect.bottom - windowRect.top) - (double) (clientRect.bottom - clientRect.top));

            // calculate size of element relative to nonclient area
            var transform = element.TransformToAncestor(window);
            var topLeftFrame = transform.Transform(new Point(0, 0));
            var bottomRightFrame = transform.Transform(new Point(element.ActualWidth + nonClientSize.Width, element.ActualHeight + nonClientSize.Height));

            // Create a margin structure
            var margins = new CoreNativeMethods.MARGINS
                              {
                                  cxLeftWidth = (int) topLeftFrame.X,
                                  cxRightWidth = (int) (window.ActualWidth - bottomRightFrame.X),
                                  cyTopHeight = (int) (topLeftFrame.Y),
                                  cyBottomHeight = (int) (window.ActualHeight - bottomRightFrame.Y)
                              };

            // Extend the Frame into client area
            CoreNativeMethods.DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }

        /// <summary>
        ///   Resets the AeroGlass exclusion area.
        /// </summary>
        public static void ResetAeroGlass(CoreNativeMethods.MARGINS margins, Window window)
        {
            ResetAeroGlass(margins, new WindowInteropHelper(window).Handle);
        }

        /// <summary>
        ///   Resets the AeroGlass exclusion area.
        /// </summary>
        public static void ResetAeroGlass(CoreNativeMethods.MARGINS margins, IntPtr windowHandle)
        {
            if (Environment.OSVersion.Version.Major < 6)
                return;
            CoreNativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }

        /// <summary>
        ///   Enables Aero Glass on a WPF window
        /// </summary>
        /// <param name = "window">The window to enable glass</param>
        /// <param name = "margins">The region to add glass</param>
        public static void EnableGlass(Window window, CoreNativeMethods.MARGINS margins = new CoreNativeMethods.MARGINS())
        {
            if (Environment.OSVersion.Version.Major < 6)
                return;
            if (margins.cyTopHeight == 0 && margins.cyBottomHeight == 0 && margins.cxRightWidth == 0 && margins.cxLeftWidth == 0)
                margins = new CoreNativeMethods.MARGINS(true);
            var windowHandle = new WindowInteropHelper(window).Handle;

            // add Window Proc hook to capture DWM messages
            var source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(WndProc);

            // Set the Background to transparent from Win32 perpective 
            HwndSource.FromHwnd(windowHandle).CompositionTarget.BackgroundColor = Colors.Transparent;

            // Set the Background to transparent from WPF perpective 
            window.Background = Brushes.Transparent;

            ResetAeroGlass(margins, windowHandle);
        }

        /// <summary>
        ///   Enables Blur on Aero Glass for a WPF window
        /// </summary>
        /// <param name = "window">The window object to add blur to</param>
        /// <param name = "region">The area to add the blur to</param>
        public static void EnableBlur(Window window, IntPtr region)
        {
            EnableBlur(new WindowInteropHelper(window).Handle, region);
        }

        /// <summary>
        ///   Enables Blur on Aero Glass
        /// </summary>
        /// <param name = "windowHandle">The windows handle to add the blur to</param>
        /// <param name = "region">The area to add the blur to</param>
        public static void EnableBlur(IntPtr windowHandle, IntPtr region)
        {
            var blur = new CoreNativeMethods.DwmBlurBehind {hRgnBlur = region, dwFlags = CoreNativeMethods.DwmBlurBehindDwFlags.DwmBBBlurRegion};

            CoreNativeMethods.DwmEnableBlurBehindWindow(windowHandle, ref blur);
        }

        #endregion

        #region implementation

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == CoreNativeMethods.DwmMessages.WmDwmCompositionChanged || msg == CoreNativeMethods.DwmMessages.WmDwmnRenderingChanged)
            {
                if (DwmCompositionChanged != null)
                    DwmCompositionChanged.Invoke(null, new DwmCompositionChangedEventArgs(IsEnabled));

                handled = true;
            }
            return IntPtr.Zero;
        }

        #region events

        /// <summary>
        ///   Occurs when DWM becomes enabled or disabled on the system
        /// </summary>
        public static event EventHandler<DwmCompositionChangedEventArgs> DwmCompositionChanged;

        #endregion

        #endregion

        #region Events

        /// <summary>
        ///   Event argument for The DwmCompositionChanged event
        /// </summary>
        public class DwmCompositionChangedEventArgs : EventArgs
        {
            /// <summary>
            ///   Event argument for DwmCompositionChanged event
            /// </summary>
            /// <param name = "isGlassEnabled" />
            internal DwmCompositionChangedEventArgs(bool isGlassEnabled)
            {
                IsGlassEnabled = isGlassEnabled;
            }

            /// <summary>
            ///   Gets a bool specifying if DWM/Glass is currently enabled.
            /// </summary>
            public bool IsGlassEnabled { get; private set; }
        }

        #endregion
    }
}