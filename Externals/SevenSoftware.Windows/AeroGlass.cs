// <copyright file="AeroGlass.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using SevenSoftware.Windows.Internal;

    using Rect = SevenSoftware.Windows.Internal.Rect;

    /// <summary>Contains methods to allow use of Aero Glass.</summary>
    public static class AeroGlass
    {
        /// <summary>Occurs when DWM becomes enabled or disabled on the system.</summary>
        public static event EventHandler<CompositionChangedEventArgs> CompositionChanged;

        /// <summary>Gets a value indicating whether DWM is enabled on the desktop.</summary>
        /// <value><c>True</c> if this instance is enabled; otherwise, <c>False</c>.</value>
        public static bool IsGlassEnabled
        {
            get { return Environment.OSVersion.Version.Major >= 6 && NativeMethods.DwmIsCompositionEnabled(); }

            /*
            set
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    return;
                }

                NativeMethods.DwmEnableComposition(value);
            }
*/
        }

        /// <summary>Enables Blur on Aero Glass for a WPF window.</summary>
        /// <param name="window">The window object to add blur to.</param>
        /// <param name="region">The area to add the blur to.</param>
        public static void EnableBlur(Window window, IntPtr region)
        {
            EnableBlur(new WindowInteropHelper(window).Handle, region);
        }

        /// <summary>Enables Blur on Aero Glass.</summary>
        /// <param name="windowHandle">The windows handle to add the blur to.</param>
        /// <param name="region">The area to add the blur to.</param>
        public static void EnableBlur(IntPtr windowHandle, IntPtr region)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            var blur = new DwmBlurBehind { RegionBlur = region, Flags = BlurBehindOptions.BlurBehindRegion };

            if (NativeMethods.DwmEnableBlurBehindWindow(windowHandle, ref blur) != 0)
            {
                // throw new InvalidOperationException();
            }
        }

        /// <summary>Enables Aero Glass on a WPF window, no exception thrown if OS does not support DWM.</summary>
        /// <param name="window">The window to enable glass.</param>
        /// <param name="margins">The region to add glass.</param>
        public static void EnableGlass(Window window, Margins margins)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            if (margins.TopHeight == 0 && margins.BottomHeight == 0 && margins.RightWidth == 0 && margins.LeftWidth == 0)
            {
                margins = new Margins(true);
            }

            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            IntPtr windowHandle = new WindowInteropHelper(window).Handle;

            if (windowHandle == IntPtr.Zero)
            {
                return;
            }

            // add Window Proc hook to capture DWM messages
            HwndSource source = HwndSource.FromHwnd(windowHandle);
            if (source == null)
            {
                throw new FormatException();
            }

            source.AddHook(WndProc);

            // Set the Background to transparent from Win32 perspective 
            HwndSource handleSource = HwndSource.FromHwnd(windowHandle);
            if (handleSource != null)
            {
                if (handleSource.CompositionTarget != null)
                {
                    handleSource.CompositionTarget.BackgroundColor = Colors.Transparent;
                }
            }

            // Set the Background to transparent from WPF perspective 
            window.Background = Brushes.Transparent;

            ResetAeroGlass(margins, windowHandle);
        }

        /// <summary>Excludes a UI element from the Aero Glass frame.</summary>
        /// <param name="element">The element to exclude.</param>
        /// <param name="window">The window the element resides in.</param>
        /// <remarks>
        ///   cMany non-WPF rendered controls (i.e., the ExplorerBrowser control) will not render properly on top of an
        ///   Aero Glass frame.
        /// </remarks>
        public static void ExcludeElementFromAeroGlass(FrameworkElement element, Window window)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            IntPtr handle = new WindowInteropHelper(window).Handle;

            if (!IsGlassEnabled)
            {
                return;
            }

            // calculate total size of window non-client area
            var handleSource = PresentationSource.FromVisual(window) as HwndSource;
            var windowRect = new Rect();
            var clientRect = new Rect();
            if (handleSource != null)
            {
                NativeMethods.GetWindowRect(handleSource.Handle, ref windowRect);
                NativeMethods.GetClientRect(handleSource.Handle, ref clientRect);
            }

            var nonClientSize =
                new Size(
                    (windowRect.Right - windowRect.Left) - (double)(clientRect.Right - clientRect.Left), 
                    (windowRect.Bottom - windowRect.Top) - (double)(clientRect.Bottom - clientRect.Top));

            // calculate size of element relative to non-client area
            GeneralTransform transform = element.TransformToAncestor(window);
            Point topLeftFrame = transform.Transform(new Point(0, 0));
            Point bottomRightFrame =
                transform.Transform(
                    new Point(element.ActualWidth + nonClientSize.Width, element.ActualHeight + nonClientSize.Height));

            // Create a margin structure
            var margins = new Margins(
                (int)topLeftFrame.X, 
                (int)topLeftFrame.Y, 
                (int)(window.ActualWidth - bottomRightFrame.X), 
                (int)(window.ActualHeight - bottomRightFrame.Y));

            // Extend the Frame into client area
            if (NativeMethods.DwmExtendFrameIntoClientArea(handle, ref margins) != 0)
            {
                // throw new InvalidOperationException();
            }
        }

        /// <summary>Resets the Aero Glass exclusion area.</summary>
        /// <param name="margins">The margins.</param>
        /// <param name="window">The window.</param>
        public static void ResetAeroGlass(Margins margins, Window window)
        {
            ResetAeroGlass(margins, new WindowInteropHelper(window).Handle);
        }

        /// <summary>Resets the Aero Glass exclusion area.</summary>
        /// <param name="margins">The margins.</param>
        /// <param name="windowHandle">The window handle.</param>
        public static void ResetAeroGlass(Margins margins, IntPtr windowHandle)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new PlatformNotSupportedException();
            }

            if (NativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins) != 0)
            {
                // throw new InvalidOperationException();
            }
        }

        /// <summary>An application-defined function that processes messages sent to a window.</summary>
        /// <param name="handle">A handle to the window.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="parameter">Additional message information. The contents of this parameter depend on the value of the msg parameter.</param>
        /// <param name="parameter2">Another additional message information. The contents of this parameter depend on the value of the msg parameter.</param>
        /// <param name="handled">If set to <c>True</c> the event was handled.</param>
        /// <returns>The return value is the result of the message processing and depends on the message sent.</returns>
        private static IntPtr WndProc(IntPtr handle, int msg, IntPtr parameter, IntPtr parameter2, ref bool handled)
        {
            if (msg == 0x031E)
            {
                if (CompositionChanged != null)
                {
                    CompositionChanged.Invoke(null, new CompositionChangedEventArgs(IsGlassEnabled));
                }

                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}