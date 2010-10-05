#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
#endregion

namespace Microsoft.Windows.Dwm
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// WPF Glass Window
    ///   Inherit from this window class to enable glass on a WPF window
    /// </summary>
    public class AeroGlass
    {
        #region Events

        /// <summary>
        ///   Occurs when DWM becomes enabled or disabled on the system
        /// </summary>
        public static event EventHandler<DwmCompositionChangedEventArgs> DwmCompositionChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Get determines if AeroGlass is enabled on the desktop. Set enables/disables AreoGlass on the desktop.
        /// </summary>
        public static bool IsEnabled
        {
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

            set
            {
                try
                {
                    CoreNativeMethods.DwmEnableComposition(
                        value ? CoreNativeMethods.CompositionEnable.DwmECEnableComposition : CoreNativeMethods.CompositionEnable.DwmECDisableComposition);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables Blur on Aero Glass for a WPF window
        /// </summary>
        /// <param name="window">
        /// The window object to add blur to
        /// </param>
        /// <param name="region">
        /// The area to add the blur to
        /// </param>
        public static void EnableBlur(Window window, IntPtr region)
        {
            EnableBlur(new WindowInteropHelper(window).Handle, region);
        }

        /// <summary>
        /// Enables Blur on Aero Glass
        /// </summary>
        /// <param name="windowHandle">
        /// The windows handle to add the blur to
        /// </param>
        /// <param name="region">
        /// The area to add the blur to
        /// </param>
        public static void EnableBlur(IntPtr windowHandle, IntPtr region)
        {
            var blur = new CoreNativeMethods.DwmBlurBehind { HRgnBlur = region, DWFlags = CoreNativeMethods.DwmBlurBehindDWFlag.DwmBBBlurRegion };

            CoreNativeMethods.DwmEnableBlurBehindWindow(windowHandle, ref blur);
        }

        /// <summary>
        /// Enables Aero Glass on a WPF window
        /// </summary>
        /// <param name="window">
        /// The window to enable glass
        /// </param>
        /// <param name="margins">
        /// The region to add glass
        /// </param>
        public static void EnableGlass(Window window, CoreNativeMethods.MARGINS margins = new CoreNativeMethods.MARGINS())
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            if (margins.CYTopHeight == 0 && margins.CYBottomHeight == 0 && margins.CXRightWidth == 0 && margins.CXLeftWidth == 0)
            {
                margins = new CoreNativeMethods.MARGINS(true);
            }

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
        /// Resets the AeroGlass exclusion area.
        /// </summary>
        /// <param name="margins">
        /// </param>
        /// <param name="window">
        /// </param>
        public static void ResetAeroGlass(CoreNativeMethods.MARGINS margins, Window window)
        {
            ResetAeroGlass(margins, new WindowInteropHelper(window).Handle);
        }

        /// <summary>
        /// Resets the AeroGlass exclusion area.
        /// </summary>
        /// <param name="margins">
        /// </param>
        /// <param name="windowHandle">
        /// </param>
        public static void ResetAeroGlass(CoreNativeMethods.MARGINS margins, IntPtr windowHandle)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            CoreNativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }

        /// <summary>
        /// Excludes a UI element from the AeroGlass frame.
        /// </summary>
        /// <param name="element">
        /// The element to exclude.
        /// </param>
        /// <param name="window">
        /// The window the element resides in
        /// </param>
        /// <remarks>
        /// c
        ///   Many non-WPF rendered controls (i.e., the ExplorerBrowser control) will not 
        ///   render properly on top of an AeroGlass frame.
        /// </remarks>
        public void ExcludeElementFromAeroGlass(FrameworkElement element, Window window)
        {
            var hWnd = new WindowInteropHelper(window).Handle;

            if (!IsEnabled)
            {
                return;
            }

            // calculate total size of window nonclient area
            var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            var windowRect = new CoreNativeMethods.RECT();
            var clientRect = new CoreNativeMethods.RECT();
            CoreNativeMethods.GetWindowRect(hwndSource.Handle, ref windowRect);
            CoreNativeMethods.GetClientRect(hwndSource.Handle, ref clientRect);
            var nonClientSize = new Size(
                (windowRect.Right - windowRect.Left) - (double)(clientRect.Right - clientRect.Left), 
                (windowRect.Bottom - windowRect.Top) - (double)(clientRect.Bottom - clientRect.Top));

            // calculate size of element relative to nonclient area
            var transform = element.TransformToAncestor(window);
            var topLeftFrame = transform.Transform(new Point(0, 0));
            var bottomRightFrame = transform.Transform(new Point(element.ActualWidth + nonClientSize.Width, element.ActualHeight + nonClientSize.Height));

            // Create a margin structure
            var margins = new CoreNativeMethods.MARGINS
                {
                    CXLeftWidth = (int)topLeftFrame.X, 
                    CXRightWidth = (int)(window.ActualWidth - bottomRightFrame.X), 
                    CYTopHeight = (int)topLeftFrame.Y, 
                    CYBottomHeight = (int)(window.ActualHeight - bottomRightFrame.Y)
                };

            // Extend the Frame into client area
            CoreNativeMethods.DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="msg">
        /// </param>
        /// <param name="wParam">
        /// </param>
        /// <param name="lParam">
        /// </param>
        /// <param name="handled">
        /// </param>
        /// <returns>
        /// </returns>
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == CoreNativeMethods.DwmMessages.WMDwmCompositionChanged || msg == CoreNativeMethods.DwmMessages.WMDwmnRenderingChanged)
            {
                if (DwmCompositionChanged != null)
                {
                    DwmCompositionChanged.Invoke(null, new DwmCompositionChangedEventArgs(IsEnabled));
                }

                handled = true;
            }

            return IntPtr.Zero;
        }

        #endregion

        /// <summary>
        /// Event argument for The DwmCompositionChanged event
        /// </summary>
        public class DwmCompositionChangedEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            /// Event argument for DwmCompositionChanged event
            /// </summary>
            /// <param name="isGlassEnabled">
            /// </param>
            internal DwmCompositionChangedEventArgs(bool isGlassEnabled)
            {
                this.IsGlassEnabled = isGlassEnabled;
            }

            #endregion

            #region Properties

            /// <summary>
            ///   Gets a bool specifying if DWM/Glass is currently enabled.
            /// </summary>
            public bool IsGlassEnabled { get; private set; }

            #endregion
        }
    }
}