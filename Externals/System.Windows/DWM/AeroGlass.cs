// ***********************************************************************
// <copyright file="AeroGlass.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows.Dwm
{
    using System.Windows.Internal;
    using System.Windows.Interop;
    using System.Windows.Media;

    /// <summary>
    /// WPF Glass Window
    ///   Inherit from this window class to enable glass on a WPF window
    /// </summary>
    public static class AeroGlass
    {
        #region Events

        /// <summary>
        ///   Occurs when DWM becomes enabled or disabled on the system
        /// </summary>
        public static event EventHandler<DwmCompositionChangedEventArgs> DwmCompositionChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether DWM is enabled on the desktop.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if this instance is enabled; otherwise, <see langword = "false" />.
        /// </value>
        public static bool IsEnabled
        {
            get
            {
                try
                {
                    return NativeMethods.DwmIsCompositionEnabled();
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
                    NativeMethods.DwmEnableComposition(value ? NativeMethods.CompositionEnable.EnableComposition : NativeMethods.CompositionEnable.DisableComposition);
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
            var blur = new NativeMethods.DwmBlurBehind { RegionBlur = region, Flags = NativeMethods.DwmBlurBehindFlag.DwmBlurBehindRegion };

            NativeMethods.DwmEnableBlurBehindWindow(windowHandle, ref blur);
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
        public static void EnableGlass(Window window, NativeMethods.Margins margins = new NativeMethods.Margins())
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            if (margins.TopHeight == 0 && margins.BottomHeight == 0 && margins.RightWidth == 0 && margins.LeftWidth == 0)
            {
                margins = new NativeMethods.Margins(true);
            }

            var windowHandle = new WindowInteropHelper(window).Handle;

            // add Window Proc hook to capture DWM messages
            var source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(WndProc);

            // Set the Background to transparent from Win32 perspective 
            HwndSource.FromHwnd(windowHandle).CompositionTarget.BackgroundColor = Colors.Transparent;

            // Set the Background to transparent from WPF perspective 
            window.Background = Brushes.Transparent;

            ResetAeroGlass(margins, windowHandle);
        }

        /// <summary>
        /// Excludes a UI element from the Aero Glass frame.
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
        ///   render properly on top of an Aero Glass frame.
        /// </remarks>
        public static void ExcludeElementFromAeroGlass(FrameworkElement element, Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;

            if (!IsEnabled)
            {
                return;
            }

            // calculate total size of window non-client area
            var handleSource = PresentationSource.FromVisual(window) as HwndSource;
            var windowRect = new NativeMethods.Rect();
            var clientRect = new NativeMethods.Rect();
            NativeMethods.GetWindowRect(handleSource.Handle, ref windowRect);
            NativeMethods.GetClientRect(handleSource.Handle, ref clientRect);
            var nonClientSize = new Size(
                (windowRect.Right - windowRect.Left) - (double)(clientRect.Right - clientRect.Left), 
                (windowRect.Bottom - windowRect.Top) - (double)(clientRect.Bottom - clientRect.Top));

            // calculate size of element relative to non-client area
            var transform = element.TransformToAncestor(window);
            var topLeftFrame = transform.Transform(new Point(0, 0));
            var bottomRightFrame = transform.Transform(new Point(element.ActualWidth + nonClientSize.Width, element.ActualHeight + nonClientSize.Height));

            // Create a margin structure
            var margins = new NativeMethods.Margins
                {
                    LeftWidth = (int)topLeftFrame.X, 
                    RightWidth = (int)(window.ActualWidth - bottomRightFrame.X), 
                    TopHeight = (int)topLeftFrame.Y, 
                    BottomHeight = (int)(window.ActualHeight - bottomRightFrame.Y)
                };

            // Extend the Frame into client area
            NativeMethods.DwmExtendFrameIntoClientArea(handle, ref margins);
        }

        /// <summary>
        /// Resets the Aero Glass exclusion area.
        /// </summary>
        /// <param name="margins">
        /// The margins.
        /// </param>
        /// <param name="window">
        /// The window.
        /// </param>
        public static void ResetAeroGlass(NativeMethods.Margins margins, Window window)
        {
            ResetAeroGlass(margins, new WindowInteropHelper(window).Handle);
        }

        /// <summary>
        /// Resets the Aero Glass exclusion area.
        /// </summary>
        /// <param name="margins">
        /// The margins.
        /// </param>
        /// <param name="windowHandle">
        /// The window handle.
        /// </param>
        public static void ResetAeroGlass(NativeMethods.Margins margins, IntPtr windowHandle)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            NativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends a Win32 process message to a window
        /// </summary>
        /// <param name="handle">
        /// The handle to the window
        /// </param>
        /// <param name="msg">
        /// The message to send
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="parameterLength">
        /// Length of the parameter.
        /// </param>
        /// <param name="handled">
        /// if set to <see langword="true"/> the event was handled
        /// </param>
        /// <returns>
        /// Returns a <see langword="null"/> pointer
        /// </returns>
        private static IntPtr WndProc(IntPtr handle, int msg, IntPtr parameter, IntPtr parameterLength, ref bool handled)
        {
            if (msg == NativeMethods.DwmMessages.DwmCompositionChanged || msg == NativeMethods.DwmMessages.DwmRenderingChanged)
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
            /// <param name="isGlassEnabled">
            /// if set to <see langword="true"/> aero glass is enabled
            /// </param>
            internal DwmCompositionChangedEventArgs(bool isGlassEnabled)
            {
                this.IsGlassEnabled = isGlassEnabled;
            }

            #endregion

            #region Properties

            /// <summary>
            ///   Gets a value indicating whether DWM/Glass is currently enabled.
            /// </summary>
            /// <value>
            ///   <see langword = "true" /> if this instance is glass enabled; otherwise, <see langword = "false" />.
            /// </value>
            public bool IsGlassEnabled { get; private set; }

            #endregion
        }
    }
}