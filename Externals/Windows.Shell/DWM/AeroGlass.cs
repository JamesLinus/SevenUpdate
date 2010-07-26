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
    public class AeroGlass
    {
        #region properties

        /// <summary>
        ///   Get determines if AeroGlass is enabled on the desktop. Set enables/disables AreoGlass on the desktop.
        /// </summary>
        public static bool IsEnabled { set { CoreNativeMethods.DwmEnableComposition(value ? CoreNativeMethods.CompositionEnable.DwmEcEnableComposition : CoreNativeMethods.CompositionEnable.DwmEcDisableComposition); } get { return CoreNativeMethods.DwmIsCompositionEnabled(); } }

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
            GeneralTransform transform = element.TransformToAncestor(window);
            Point topLeftFrame = transform.Transform(new Point(0, 0));
            Point bottomRightFrame = transform.Transform(new Point(element.ActualWidth + nonClientSize.Width, element.ActualHeight + nonClientSize.Height));

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
            CoreNativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }

        /// <summary>
        ///   Enables Aero Glass on a WPF window
        /// </summary>
        /// <param name = "window">The window to enable glass</param>
        /// <param name = "margins">The region to add glass</param>
        public static void EnableGlass(Window window, CoreNativeMethods.MARGINS margins = new CoreNativeMethods.MARGINS())
        {
            if (margins.cyTopHeight == 0 && margins.cyBottomHeight == 0 && margins.cxRightWidth == 0 && margins.cxLeftWidth == 0)
                margins = new CoreNativeMethods.MARGINS(true);
            var windowHandle = new WindowInteropHelper(window).Handle;

            // add Window Proc hook to capture DWM messages
            HwndSource source = HwndSource.FromHwnd(windowHandle);
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
            var blur = new CoreNativeMethods.DwmBlurBehind { hRgnBlur = region, dwFlags = CoreNativeMethods.DwmBlurBehindDwFlags.DwmBBBlurRegion };

            CoreNativeMethods.DwmEnableBlurBehindWindow(windowHandle, ref blur);
        }

        #endregion

        #region implementation

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == CoreNativeMethods.DwmMessages.WmDwmCompositionChanged || msg == CoreNativeMethods.DwmMessages.WmDwmnRenderingChanged)
            {
                if (AeroGlassCompositionChanged != null)
                    AeroGlassCompositionChanged.Invoke(null, new AeroGlassCompositionChangedEvenArgs(IsEnabled));

                handled = true;
            }
            return IntPtr.Zero;
        }

        #region events

        /// <summary>
        ///   Fires when the availability of Glass effect changes.
        /// </summary>
        public static event AeroGlassCompositionChangedEvent AeroGlassCompositionChanged;

        #endregion

        #endregion
    }
}