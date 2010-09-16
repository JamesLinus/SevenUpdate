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
using System.Runtime.InteropServices;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Internal class containing most native interop declarations used
    ///   throughout the library.
    ///   Functions that are not performance intensive belong in this class.
    /// </summary>
    internal static class TaskDialogNativeMethods
    {
        #region TaskDialog Definitions

        internal const int TASKDIALOG_IDEALWIDTH = 0; // Value for TASKDIALOGCONFIG.cxWidth
        internal const int TASKDIALOG_BUTTON_SHIELD_ICON = 1;
        internal const int NO_DEFAULT_BUTTON_SPECIFIED = 0;

        [DllImport(CommonDllNames.ComCtl32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern HRESULT TaskDialogIndirect([In] TASKDIALOGCONFIG pTaskConfig, [Out] out int pnButton, [Out] out int pnRadioButton,
                                                          [MarshalAs(UnmanagedType.Bool), Out] out bool pVerificationFlagChecked);

        #region Nested type: PBST

        internal enum PBST
        {
            PBST_NORMAL = 0x0001,
            PBST_ERROR = 0x0002,
            PBST_PAUSED = 0x0003
        }

        #endregion

        #region Nested type: PFTASKDIALOGCALLBACK

        internal delegate int PFTASKDIALOGCALLBACK(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData);

        #endregion

        #region Nested type: TASKDIALOGCONFIG

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal class TASKDIALOGCONFIG
        {
            internal uint cbSize;
            internal IntPtr hwndParent;
            internal IntPtr hInstance;
            internal TASKDIALOG_FLAGS dwFlags;
            internal TASKDIALOG_COMMON_BUTTON_FLAGS dwCommonButtons;

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszWindowTitle;

            internal TASKDIALOGCONFIG_ICON_UNION MainIcon; // NOTE: 32-bit union field, holds pszMainIcon as well

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszMainInstruction;

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszContent;

            internal uint cButtons;
            internal IntPtr pButtons; // Ptr to TASKDIALOG_BUTTON structs
            internal int nDefaultButton;
            internal uint cRadioButtons;
            internal IntPtr pRadioButtons; // Ptr to TASKDIALOG_BUTTON structs
            internal int nDefaultRadioButton;

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszVerificationText;

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszExpandedInformation;

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszExpandedControlText;

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszCollapsedControlText;

            internal TASKDIALOGCONFIG_ICON_UNION FooterIcon; // NOTE: 32-bit union field, holds pszFooterIcon as well

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszFooter;

            internal PFTASKDIALOGCALLBACK pfCallback;
            internal IntPtr lpCallbackData;
            internal uint cxWidth;
        }

        #endregion

        // NOTE: We include a "spacer" so that the struct size varies on 
        // 64-bit architectures.

        #region Nested type: TASKDIALOGCONFIG_ICON_UNION

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        internal struct TASKDIALOGCONFIG_ICON_UNION
        {
            internal TASKDIALOGCONFIG_ICON_UNION(int i)
            {
                spacer = IntPtr.Zero;
                pszIcon = 0;
                hMainIcon = i;
            }

            [FieldOffset(0)] internal int hMainIcon;

            [FieldOffset(0)] internal int pszIcon;

            [FieldOffset(0)] internal IntPtr spacer;
        }

        #endregion

        // NOTE: Packing must be set to 4 to make this work on 64-bit platforms.

        #region Nested type: TASKDIALOG_BUTTON

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct TASKDIALOG_BUTTON
        {
            public TASKDIALOG_BUTTON(int n, string txt)
            {
                nButtonID = n;
                pszButtonText = txt;
            }

            internal int nButtonID;

            [MarshalAs(UnmanagedType.LPWStr)] internal string pszButtonText;
        }

        #endregion

        // Task Dialog - identifies common buttons.

        #region Nested type: TASKDIALOG_COMMON_BUTTON_FLAGS

        [Flags]
        internal enum TASKDIALOG_COMMON_BUTTON_FLAGS
        {
            TDCBF_OK_BUTTON = 0x0001, // selected control return value IDOK
            TDCBF_YES_BUTTON = 0x0002, // selected control return value IDYES
            TDCBF_NO_BUTTON = 0x0004, // selected control return value IDNO
            TDCBF_CANCEL_BUTTON = 0x0008, // selected control return value IDCANCEL
            TDCBF_RETRY_BUTTON = 0x0010, // selected control return value IDRETRY
            TDCBF_CLOSE_BUTTON = 0x0020 // selected control return value IDCLOSE
        }

        #endregion

        // Identify button *return values* - note that, unfortunately, these are different
        // from the inbound button values.

        #region Nested type: TASKDIALOG_COMMON_BUTTON_RETURN_ID

        internal enum TASKDIALOG_COMMON_BUTTON_RETURN_ID
        {
            IDOK = 1,
            IDCANCEL = 2,
            IDABORT = 3,
            IDRETRY = 4,
            IDIGNORE = 5,
            IDYES = 6,
            IDNO = 7,
            IDCLOSE = 8
        }

        #endregion

        #region Nested type: TASKDIALOG_ELEMENTS

        internal enum TASKDIALOG_ELEMENTS
        {
            TDE_CONTENT,
            TDE_EXPANDED_INFORMATION,
            TDE_FOOTER,
            TDE_MAIN_INSTRUCTION
        }

        #endregion

        // Task Dialog - flags

        #region Nested type: TASKDIALOG_FLAGS

        [Flags]
        internal enum TASKDIALOG_FLAGS
        {
            NONE = 0,
            TDF_ENABLE_HYPERLINKS = 0x0001,
            TDF_USE_HICON_MAIN = 0x0002,
            TDF_USE_HICON_FOOTER = 0x0004,
            TDF_ALLOW_DIALOG_CANCELLATION = 0x0008,
            TDF_USE_COMMAND_LINKS = 0x0010,
            TDF_USE_COMMAND_LINKS_NO_ICON = 0x0020,
            TDF_EXPAND_FOOTER_AREA = 0x0040,
            TDF_EXPANDED_BY_DEFAULT = 0x0080,
            TDF_VERIFICATION_FLAG_CHECKED = 0x0100,
            TDF_SHOW_PROGRESS_BAR = 0x0200,
            TDF_SHOW_MARQUEE_PROGRESS_BAR = 0x0400,
            TDF_CALLBACK_TIMER = 0x0800,
            TDF_POSITION_RELATIVE_TO_WINDOW = 0x1000,
            TDF_RTL_LAYOUT = 0x2000,
            TDF_NO_DEFAULT_RADIO_BUTTON = 0x4000
        }

        #endregion

        #region Nested type: TASKDIALOG_ICON_ELEMENT

        internal enum TASKDIALOG_ICON_ELEMENT
        {
            TDIE_ICON_MAIN,
            TDIE_ICON_FOOTER
        }

        #endregion

        #region Nested type: TASKDIALOG_MESSAGES

        internal enum TASKDIALOG_MESSAGES
        {
            TDM_NAVIGATE_PAGE = CoreNativeMethods.WM_USER + 101,
            TDM_CLICK_BUTTON = CoreNativeMethods.WM_USER + 102, // wParam = Button ID
            TDM_SET_MARQUEE_PROGRESS_BAR = CoreNativeMethods.WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
            TDM_SET_PROGRESS_BAR_STATE = CoreNativeMethods.WM_USER + 104, // wParam = new progress state
            TDM_SET_PROGRESS_BAR_RANGE = CoreNativeMethods.WM_USER + 105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
            TDM_SET_PROGRESS_BAR_POS = CoreNativeMethods.WM_USER + 106, // wParam = new position
            TDM_SET_PROGRESS_BAR_MARQUEE = CoreNativeMethods.WM_USER + 107,
            // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
            TDM_SET_ELEMENT_TEXT = CoreNativeMethods.WM_USER + 108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            TDM_CLICK_RADIO_BUTTON = CoreNativeMethods.WM_USER + 110, // wParam = Radio Button ID
            TDM_ENABLE_BUTTON = CoreNativeMethods.WM_USER + 111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
            TDM_ENABLE_RADIO_BUTTON = CoreNativeMethods.WM_USER + 112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
            TDM_CLICK_VERIFICATION = CoreNativeMethods.WM_USER + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
            TDM_UPDATE_ELEMENT_TEXT = CoreNativeMethods.WM_USER + 114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE = CoreNativeMethods.WM_USER + 115, // wParam = Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
            TDM_UPDATE_ICON = CoreNativeMethods.WM_USER + 116
            // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
        }

        #endregion

        #region Nested type: TASKDIALOG_NOTIFICATIONS

        internal enum TASKDIALOG_NOTIFICATIONS
        {
            TDN_CREATED = 0,
            TDN_NAVIGATED = 1,
            TDN_BUTTON_CLICKED = 2, // wParam = Button ID
            TDN_HYPERLINK_CLICKED = 3, // lParam = (LPCWSTR)pszHREF
            TDN_TIMER = 4, // wParam = Milliseconds since dialog created or timer reset
            TDN_DESTROYED = 5,
            TDN_RADIO_BUTTON_CLICKED = 6, // wParam = Radio Button ID
            TDN_DIALOG_CONSTRUCTED = 7,
            TDN_VERIFICATION_CLICKED = 8, // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
            TDN_HELP = 9,
            TDN_EXPANDO_BUTTON_CLICKED = 10 // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
        }

        #endregion

        #region Nested type: TDIDelegate

        internal delegate HRESULT TDIDelegate([In] TASKDIALOGCONFIG pTaskConfig, [Out] out int pnButton, [Out] out int pnRadioButton, [Out] out bool pVerificationFlagChecked);

        #endregion

        // Used in the various SET_DEFAULT* TaskDialog messages

        #region Nested type: TD_ICON

        internal enum TD_ICON
        {
            TD_WARNING_ICON = 65535,
            TD_ERROR_ICON = 65534,
            TD_INFORMATION_ICON = 65533,
            TD_SHIELD_ICON = 65532
        }

        #endregion

        #endregion
    }
}