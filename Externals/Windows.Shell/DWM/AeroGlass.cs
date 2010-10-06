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
        /// Gets or sets a value indicating whether DWM is enabled on the desktop.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance is enabled; otherwise, <see langword="false"/>.
        /// </value>
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
        public static void EnableGlass(Window window, CoreNativeMethods.Margins margins = new CoreNativeMethods.Margins())
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            if (margins.topHeight == 0 && margins.bottomHeight == 0 && margins.rightWidth == 0 && margins.leftWidth == 0)
            {
                margins = new CoreNativeMethods.Margins(true);
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
        /// Resets the Aero Glass exclusion area.
        /// </summary>
        /// <param name="margins">The margins.</param>
        /// <param name="window">The window.</param>
        public static void ResetAeroGlass(CoreNativeMethods.Margins margins, Window window)
        {
            ResetAeroGlass(margins, new WindowInteropHelper(window).Handle);
        }

        /// <summary>
        /// Resets the Aero Glass exclusion area.
        /// </summary>
        /// <param name="margins">The margins.</param>
        /// <param name="windowHandle">The window handle.</param>
        public static void ResetAeroGlass(CoreNativeMethods.Margins margins, IntPtr windowHandle)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            CoreNativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }

        /// <summary>
        /// Excludes a UI element from the Aero Glass frame.
        /// </summary>
        /// <param name="element">The element to exclude.</param>
        /// <param name="window">The window the element resides in</param>
        /// <remarks>
        /// c
        /// Many non-WPF rendered controls (i.e., the ExplorerBrowser control) will not
        /// render properly on top of an Aero Glass frame.
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
            var windowRect = new CoreNativeMethods.Rect();
            var clientRect = new CoreNativeMethods.Rect();
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
            var margins = new CoreNativeMethods.Margins
                {
                    leftWidth = (int)topLeftFrame.X, 
                    rightWidth = (int)(window.ActualWidth - bottomRightFrame.X), 
                    topHeight = (int)topLeftFrame.Y, 
                    bottomHeight = (int)(window.ActualHeight - bottomRightFrame.Y)
                };

            // Extend the Frame into client area
            CoreNativeMethods.DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }

        #endregion

        #region Methods

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
        /// Event argument for The <see cref="DwmCompositionChanged"/> event
        /// </summary>
        public class DwmCompositionChangedEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DwmCompositionChangedEventArgs"/> class.
            /// </summary>
            /// <param name="isGlassEnabled">if set to <see langword="true"/> aero glass is enabled</param>
            internal DwmCompositionChangedEventArgs(bool isGlassEnabled)
            {
                this.IsGlassEnabled = isGlassEnabled;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets a value indicating whether DWM/Glass is currently enabled.
            /// </summary>
            /// <value>
            /// <see langword="true"/> if this instance is glass enabled; otherwise, <see langword="false"/>.
            /// </value>
            public bool IsGlassEnabled { get; private set; }

            #endregion
        }
    }
}