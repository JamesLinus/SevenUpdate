// ***********************************************************************
// <copyright file="NativeMethods.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt">GNU General Public License Version 3</license>
// ***********************************************************************
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
namespace System.Windows.Internal
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Wrappers for Native Methods and Structs.
    ///   This type is intended for internal use only
    /// </summary>
    public static class NativeMethods
    {
        #region Constants

        /// <summary>
        ///   Various important window messages
        /// </summary>
        internal const int WmUser = 0x0400;

        /// <summary>
        ///   Enable/disable non-client rendering based on window style.
        /// </summary>
        internal const int DwmNcrUseWindowStyle = 0;

        /// <summary>
        ///   Disabled non-client rendering; window style is ignored.
        /// </summary>
        internal const int DwmNcrDisabled = 1;

        /// <summary>
        ///   Enabled non-client rendering; window style is ignored.
        /// </summary>
        internal const int DwmNcrEnabled = 2;

        /// <summary>
        ///   Enable/disable non-client rendering Use DWMNCRP_* values.If the function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </summary>
        internal const int DwmNcRenderingEnabled = 1;

        /// <summary>
        ///   Non-client rendering policy.
        /// </summary>
        internal const int DwmNcRenderingPolicy = 2;

        /// <summary>
        ///   Potentially enable/forcibly disable transitions 0 or 1.
        /// </summary>
        internal const int DwmTransitionsForceDisabled = 3;

        /// <summary>
        ///   Enable blur behind
        /// </summary>
        internal const int DwmBlurBehindEnable = 0x00000001;

        /// <summary>
        ///   The blur region has been specified
        /// </summary>
        internal const int DwmBlurRegion = 0x00000002;

        /// <summary>
        ///   TransitionOnMaximized has been specified
        /// </summary>
        internal const int DwmTransitionOnMaximized = 0x00000004;

        #endregion

        #region Enums

        /// <summary>
        /// The blur behind flags/options
        /// </summary>
        internal enum DwmBlurBehindFlag : uint
        {
            /// <summary>
            ///   Enables blur behind
            /// </summary>
            DwmBlurBehindEnable = 0x00000001, 

            /// <summary>
            ///   The blur behind region
            /// </summary>
            DwmBlurBehindRegion = 0x00000002, 

            /// <summary>
            ///   True to show effects with maximizing
            /// </summary>
            DwmTransitionOnMaximized = 0x00000004
        }

        #endregion

        /// <summary>
        ///   Gets a value indicating whether if the current logged in user is an admin
        /// </summary>
        public static bool IsUserAdmin
        {
            get
            {
                return IsUserAnAdmin();
            }
        }

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls
        ///   the window procedure for the specified window and does not return until the window
        ///   procedure has processed the message.
        /// </summary>
        /// <param name="pointer">
        /// Handle to the window whose window procedure will receive the message.
        ///   If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///   including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///   but the message is not sent to child windows.
        /// </param>
        /// <param name="msg">
        /// Specifies the message to be sent.
        /// </param>
        /// <param name="parameter">
        /// Specifies additional message-specific information.
        /// </param>
        /// <param name="parameterLength">
        /// Specifies the length of the additional message-specific information.
        /// </param>
        /// <returns>
        /// A return code specific to the message being sent.
        /// </returns>
        [DllImport(@"user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr pointer, uint msg, IntPtr parameter, IntPtr parameterLength);

        /// <summary>
        /// Enables the blur effect on a specified window.
        /// </summary>
        /// <param name="handle">
        /// The handle to the window on which the blur behind data is applied.
        /// </param>
        /// <param name="bb">
        /// A pointer to a <see cref="DwmBlurBehind"/> structure that provides blur behind data.
        /// </param>
        /// <returns>
        /// If function succeeds, it returns S_OK. Otherwise, it returns an <see cref="Result"/> error code.
        /// </returns>
        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr handle, ref DwmBlurBehind bb);

        /// <summary>
        /// Extends glass into the client area
        /// </summary>
        /// <param name="handle">
        /// The handle to the window for which the frame is extended into the client area.
        /// </param>
        /// <param name="margins">
        /// A pointer to a Margins structure that describes the margins to use when extending the frame into the client area.
        /// </param>
        /// <returns>
        /// If function succeeds, it returns S_OK. Otherwise, it returns an <see cref="Result"/> error code..
        /// </returns>
        [DllImport(@"DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr handle, ref Margins margins);

        /// <summary>
        /// Gets a value that indicates whether Desktop Window Manager (DWM) composition is enabled. Applications can listen for composition state changes by handling the WM_DWMCOMPOSITIONCHANGED notification.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if composition is enabled; otherwise, <see langword="false"/>
        /// </returns>
        [DllImport(@"DwmApi.dll", PreserveSig = false)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// Enables or disables Desktop Window Manager (DWM) composition.
        /// </summary>
        /// <param name="enable">
        /// if set to <see langword="true"/> DWM will be enabled
        /// </param>
        /// <returns>
        /// If function succeeds, it returns S_OK. Otherwise, it returns an <see cref="Result"/> error code.
        /// </returns>
        [DllImport(@"DwmApi.dll", PreserveSig = false)]
        internal static extern int DwmEnableComposition(bool enable);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window.
        /// </param>
        /// <param name="rect">
        /// A pointer to a <see cref="Rect"/> structure that receives the screen coordinates of the upper-left and lower-right corners of the window.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if successful
        /// </returns>
        [DllImport(@"user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr handle, ref Rect rect);

        /// <summary>
        /// Retrieves the coordinates of a window's client area. The client coordinates specify the upper-left and lower-right corners of the client area. Because client coordinates are relative to the upper-left corner of a window's client area, the coordinates of the upper-left corner are (0,0).
        /// </summary>
        /// <param name="handle">
        /// A handle to the window whose client coordinates are to be retrieved.
        /// </param>
        /// <param name="rect">
        /// A pointer to a <see cref="Rect"/> structure that receives the client coordinates. The left and top members are zero. The right and bottom members contain the width and height of the window.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if successful
        /// </returns>
        [DllImport(@"user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr handle, ref Rect rect);

        /// <summary>
        /// Gets a value indicating whether the current user is a member of the Administrator's group.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the user is a member of the Administrator's group; otherwise, <see langword="false"/>.
        /// </returns>
        [DllImport(@"shell32.dll", EntryPoint = "IsUserAnAdmin", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool IsUserAnAdmin();

        /// <summary>
        /// Defines the margins of windows that have visual styles applied.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            /// <summary>
            ///   Width of the left border that retains its size.
            /// </summary>
            public int LeftWidth;

            /// <summary>
            ///   Width of the right border that retains its size.
            /// </summary>
            public int RightWidth;

            /// <summary>
            ///   Height of the top border that retains its size.
            /// </summary>
            public int TopHeight;

            /// <summary>
            ///   Height of the bottom border that retains its size.
            /// </summary>
            public int BottomHeight;

            /// <summary>
            /// Initializes a new instance of the <see cref="Margins"/> struct.
            /// </summary>
            /// <param name="fullWindow">
            /// if set to <see langword="true"/> the margin is set to the full window.
            /// </param>
            public Margins(bool fullWindow)
            {
                this.LeftWidth = this.RightWidth = this.TopHeight = this.BottomHeight = fullWindow ? -1 : 0;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Margins"/> struct.
            /// </summary>
            /// <param name="left">
            /// Width of the left border that retains its size.
            /// </param>
            /// <param name="top">
            /// Height of the top border that retains its size.
            /// </param>
            /// <param name="right">
            /// Width of the right border that retains its size.
            /// </param>
            /// <param name="bottom">
            /// Height of the bottom border that retains its size.
            /// </param>
            public Margins(int left, int top, int right, int bottom)
            {
                this.LeftWidth = left;
                this.RightWidth = right;
                this.TopHeight = top;
                this.BottomHeight = bottom;
            }
        }

        /// <summary>
        /// Defines the coordinates of the upper-left and lower-right corners of a rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            /// <summary>
            ///   The x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Left;

            /// <summary>
            ///   The y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Top;

            /// <summary>
            ///   The x-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Right;

            /// <summary>
            ///   The y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// Specifies Desktop Window Manager (DWM) blur behind properties.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DwmBlurBehind
        {
            /// <summary>
            ///   A bitwise combination of DWM Blur Behind Constants values indicating which members are set.
            /// </summary>
            public DwmBlurBehindFlag Flags;

            /// <summary>
            ///   <see langword = "true" /> to register the window handle to DWM blur behind; <see langword = "false" /> to unregister the window handle from DWM blur behind.
            /// </summary>
            public bool Enable;

            /// <summary>
            ///   The region within the client area to apply the blur behind. A <see langword = "null" /> value will apply the blur behind the entire client area.
            /// </summary>
            public IntPtr RegionBlur;

            /// <summary>
            ///   <see langword = "true" /> if the window's colorization should transition to match the maximized windows; otherwise, <see langword = "false" />.
            /// </summary>
            public bool TransitionOnMaximized;
        }

        /// <summary>
        /// The DWM messages
        /// </summary>
        internal static class DwmMessages
        {
            #region Constants and Fields

            /// <summary>
            ///   Dwm has been enabled or Disabled
            /// </summary>
            internal const int DwmCompositionChanged = 0x031E;

            /// <summary>
            ///   Dwn rendering has changed
            /// </summary>
            internal const int DwmRenderingChanged = 0x031F;

            #endregion
        }
    }
}