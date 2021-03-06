// <copyright file="NativeMethods.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Runtime.InteropServices;
using SevenSoftware.Windows.Dialogs.TaskDialog;

namespace SevenSoftware.Windows.Internal
{
    /// <summary>Wrappers for Native Methods and Structs. This type is intended for internal use only.</summary>
    public static class NativeMethods
    {
        /// <summary>The <c>TaskDialog</c> callback.</summary>
        /// <param name="handle">The handle for the dialog.</param>
        /// <param name="message">The message id.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterLength">The length of the parameter.</param>
        /// <param name="referenceData">The reference data</param>
        /// <returns>The result of the <c>TaskDialog</c>.</returns>
        internal delegate int TaskDialogCallback(
            IntPtr handle, uint message, IntPtr parameter, IntPtr parameterLength, IntPtr referenceData);

        /// <summary>Gets a value indicating whether if the current logged in user is an admin.</summary>
        public static bool IsUserAdmin
        {
            get { return IsUserAnAdmin(); }
        }

        /// <summary>
        ///   Sends the specified message to a window or windows. The SendMessage function calls the window procedure
        ///   for the specified window and does not return until the windowprocedure has processed the message.
        /// </summary>
        /// <param name="pointer">Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,including disabled or invisible unowned windows, overlapped windows, and pop-up windows;but the message is not sent to child windows.</param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="parameter">Specifies additional message-specific information.</param>
        /// <param name="parameterLength">Specifies the length of the additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [DllImport(@"user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr pointer, uint msg, IntPtr parameter, IntPtr parameterLength);

        /// <summary>Enables the blur effect on a specified window.</summary>
        /// <param name="handle">The handle to the window on which the blur behind data is applied.</param>
        /// <param name="bb">A pointer to a <c>DwmBlurBehind</c> structure that provides blur behind data.</param>
        /// <returns>If function succeeds, it returns S_OK. Otherwise, it returns an <c>Result</c> error code.</returns>
        [DllImport(@"DwmApi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr handle, ref DwmBlurBehind bb);

        /// <summary>Extends glass into the client area.</summary>
        /// <param name="handle">The handle to the window for which the frame is extended into the client area.</param>
        /// <param name="margins">A pointer to a Margins structure that describes the margins to use when extending the frame into the client area.</param>
        /// <returns>If function succeeds, it returns S_OK. Otherwise, it returns an <c>Result</c> error code..</returns>
        [DllImport(@"DwmApi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr handle, ref Margins margins);

        /// <summary>
        ///   Gets a value that indicates whether Desktop Window Manager (DWM) composition is enabled. Applications can
        ///   listen for composition state changes by handling the WM_DWMCOMPOSITIONCHANGED notification.
        /// </summary>
        /// <returns><c>True</c> if composition is enabled; otherwise, <c>False</c>.</returns>
        [DllImport(@"DwmApi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <summary>Enables or disables Desktop Window Manager (DWM) composition.</summary>
        /// <param name="enable">If set to <c>True</c> DWM will be enabled.</param>
        /// <returns>If function succeeds, it returns S_OK. Otherwise, it returns an <c>Result</c> error code.</returns>
        [DllImport(@"DwmApi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int DwmEnableComposition([MarshalAs(UnmanagedType.Bool)] bool enable);

        /// <summary>
        ///   Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in
        ///   screen coordinates that are relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="handle">A handle to the window.</param>
        /// <param name="rect">A pointer to a <c>Rect</c> structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
        /// <returns><c>True</c> if successful.</returns>
        [DllImport(@"user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr handle, ref Rect rect);

        /// <summary>
        ///   Retrieves the coordinates of a window's client area. The client coordinates specify the upper-left and
        ///   lower-right corners of the client area. Because client coordinates are relative to the upper-left corner
        ///   of a window's client area, the coordinates of the upper-left corner are (0,0).
        /// </summary>
        /// <param name="handle">A handle to the window whose client coordinates are to be retrieved.</param>
        /// <param name="rect">A pointer to a <c>Rect</c> structure that receives the client coordinates. The left and top members are zero. The right and bottom members contain the width and height of the window.</param>
        /// <returns><c>True</c> if successful.</returns>
        [DllImport(@"user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr handle, ref Rect rect);

        /// <summary>
        ///   The TaskDialogIndirect function creates, displays, and operates a task dialog. The task dialog contains
        ///   application-defined icons, messages, title, verification check box, command links, push buttons, and radio
        ///   buttons. This function can register a callback function to receive notification messages.
        /// </summary>
        /// <param name="taskConfig">A pointer to a <c>TaskDialogConfig</c> structure that contains information used to display the task dialog.</param>
        /// <param name="button">Address of a variable that receives one of the button IDs specified in the <paramref
        /// name="button" /> member of the <paramref name="taskConfig" /> parameter. If this parameter is
        /// <c>null</c>, no value is returned.</param>
        /// <param name="radioButton">Address of a variable that receives one of the button IDs specified in the
        /// <paramref name="radioButton" /> member of the <paramref name="taskConfig" /> parameter. If this parameter is
        /// <c>null</c>, no value is returned.</param>
        /// <param name="verificationFlagChecked"><c>True</c> if the verification <c>CheckBox</c> was checked when the dialog was dismissed; otherwise, <c>False</c>.</param>
        /// <returns>The result.</returns>
        [DllImport("Comctl32.dll", SetLastError = true)]
        internal static extern Result TaskDialogIndirect(
            [In] TaskDialogConfiguration taskConfig, 
            [Out] out int button, 
            [Out] out int radioButton, 
            [MarshalAs(UnmanagedType.Bool)] [Out] out bool verificationFlagChecked);

        /// <summary>Gets a value indicating whether the current user is a member of the Administrator's group.</summary>
        /// <returns><c>True</c> if the user is a member of the Administrator's group; otherwise, <c>False</c>.</returns>
        [DllImport(@"shell32.dll", EntryPoint = "IsUserAnAdmin", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsUserAnAdmin();
    }
}