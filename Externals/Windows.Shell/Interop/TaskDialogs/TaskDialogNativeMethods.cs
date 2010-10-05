//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Internal class containing most native interop declarations used
    ///   throughout the library.
    ///   Functions that are not performance intensive belong in this class.
    /// </summary>
    internal static class TaskDialogNativeMethods
    {
        #region TaskDialog Definitions

        /// <summary>
        /// </summary>
        internal const int TaskdialogIdealwidth = 0; // Value for TASKDIALOGCONFIG.cxWidth

        /// <summary>
        /// </summary>
        internal const int TaskdialogButtonShieldIcon = 1;

        /// <summary>
        /// </summary>
        internal const int NODefaultButtonSpecified = 0;

        /// <summary>
        /// </summary>
        /// <param name="pTaskConfig">
        /// </param>
        /// <param name="pnButton">
        /// </param>
        /// <param name="pnRadioButton">
        /// </param>
        /// <param name="pVerificationFlagChecked">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport(CommonDllNames.ComCtl32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern HRESULT TaskDialogIndirect(
            [In] TASKDIALOGCONFIG pTaskConfig, 
            [Out] out int pnButton, 
            [Out] out int pnRadioButton, 
            [MarshalAs(UnmanagedType.Bool)] [Out] out bool pVerificationFlagChecked);

        #region Nested type: PBST

        /// <summary>
        /// </summary>
        internal enum PBST
        {
            /// <summary>
            /// </summary>
            Normal = 0x0001, 

            /// <summary>
            /// </summary>
            Error = 0x0002, 

            /// <summary>
            /// </summary>
            Paused = 0x0003
        }

        #endregion

        #region Nested type: PFTASKDIALOGCALLBACK

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
        /// <param name="lpRefData">
        /// </param>
        internal delegate int PFTASKDIALOGCALLBACK(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData);

        #endregion

        #region Nested type: TASKDIALOGCONFIG

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal class TASKDIALOGCONFIG : IDisposable
        {
            /// <summary>
            /// </summary>
            internal uint cbSize;

            /// <summary>
            /// </summary>
            internal IntPtr hwndParent;

            /// <summary>
            /// </summary>
            internal IntPtr hInstance;

            /// <summary>
            /// </summary>
            internal TaskdialogFlagss dwFlags;

            /// <summary>
            /// </summary>
            internal TaskdialogCommonButtonFlagss dwCommonButtons;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszWindowTitle;

            /// <summary>
            /// </summary>
            internal TaskdialogconfigIconUnion MainIcon; // NOTE: 32-bit union field, holds pszMainIcon as well

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszMainInstruction;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszContent;

            /// <summary>
            /// </summary>
            internal uint cButtons;

            /// <summary>
            /// </summary>
            internal IntPtr pButtons; // Ptr to TASKDIALOG_BUTTON structs

            /// <summary>
            /// </summary>
            internal int nDefaultButton;

            /// <summary>
            /// </summary>
            internal uint cRadioButtons;

            /// <summary>
            /// </summary>
            internal IntPtr pRadioButtons; // Ptr to TASKDIALOG_BUTTON structs

            /// <summary>
            /// </summary>
            internal int nDefaultRadioButton;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszVerificationText;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszExpandedInformation;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszExpandedControlText;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszCollapsedControlText;

            /// <summary>
            /// </summary>
            internal TaskdialogconfigIconUnion FooterIcon; // NOTE: 32-bit union field, holds pszFooterIcon as well

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszFooter;

            /// <summary>
            /// </summary>
            internal PFTASKDIALOGCALLBACK pfCallback;

            /// <summary>
            /// </summary>
            internal IntPtr lpCallbackData;

            /// <summary>
            /// </summary>
            internal uint cxWidth;

            #region IDisposable Implementation

            /// <summary>
            /// </summary>
            protected bool disposed /* = false*/;

            /// <summary>
            /// </summary>
            /// <param name="disposing">
            /// </param>
            protected virtual void Dispose(bool disposing)
            {
                lock (this)
                {
                    // Do nothing if the object has already been disposed of.
                    if (this.disposed)
                    {
                        return;
                    }

                    if (disposing)
                    {
                        // Release diposable objects used by this instance here.
                    }

                    // Release unmanaged resources here. Don't access reference type fields.

                    // Remember that the object has been disposed of.
                    this.disposed = true;
                }
            }

            /// <summary>
            /// </summary>
            public virtual void Dispose()
            {
                this.Dispose(false);

                // Unregister object for finalization.
                GC.SuppressFinalize(this);
            }

            #endregion

            #region Destructor

            /// <summary>
            /// </summary>
            ~TASKDIALOGCONFIG()
            {
                this.Dispose(false);
            }

            #endregion
        }

        #endregion

        // NOTE: We include a "spacer" so that the struct size varies on 
        // 64-bit architectures.
        #region Nested type: TASKDIALOGCONFIG_ICON_UNION

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        internal struct TaskdialogconfigIconUnion
        {
            /// <summary>
            /// </summary>
            /// <param name="i">
            /// </param>
            internal TaskdialogconfigIconUnion(int i)
            {
                this.spacer = IntPtr.Zero;
                this.pszIcon = 0;
                this.hMainIcon = i;
            }

            /// <summary>
            /// </summary>
            [FieldOffset(0)]
            internal int hMainIcon;

            /// <summary>
            /// </summary>
            [FieldOffset(0)]
            internal int pszIcon;

            /// <summary>
            /// </summary>
            [FieldOffset(0)]
            internal IntPtr spacer;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(TaskdialogconfigIconUnion x, TaskdialogconfigIconUnion y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(TaskdialogconfigIconUnion x, TaskdialogconfigIconUnion y)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        // NOTE: Packing must be set to 4 to make this work on 64-bit platforms.
        #region Nested type: TASKDIALOG_BUTTON

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct TaskdialogButton
        {
            /// <summary>
            /// </summary>
            /// <param name="n">
            /// </param>
            /// <param name="txt">
            /// </param>
            public TaskdialogButton(int n, string txt)
            {
                this.nButtonID = n;
                this.pszButtonText = txt;
            }

            /// <summary>
            /// </summary>
            internal int nButtonID;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszButtonText;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(TaskdialogButton x, TaskdialogButton y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(TaskdialogButton x, TaskdialogButton y)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        // Task Dialog - identifies common buttons.
        #region Nested type: TASKDIALOG_COMMON_BUTTON_FLAGS

        /// <summary>
        /// </summary>
        [Flags]
        internal enum TaskdialogCommonButtonFlagss
        {
            /// <summary>
            /// </summary>
            TdcbfokButton = 0x0001, // selected control return value IDOK
            /// <summary>
            /// </summary>
            TdcbfYesButton = 0x0002, // selected control return value IDYES
            /// <summary>
            /// </summary>
            TdcbfnoButton = 0x0004, // selected control return value IDNO
            /// <summary>
            /// </summary>
            TdcbfCancelButton = 0x0008, // selected control return value IDCANCEL
            /// <summary>
            /// </summary>
            TdcbfRetryButton = 0x0010, // selected control return value IDRETRY
            /// <summary>
            /// </summary>
            TdcbfCloseButton = 0x0020 // selected control return value IDCLOSE
        }

        #endregion

        // Identify button *return values* - note that, unfortunately, these are different
        // from the inbound button values.
        #region Nested type: TASKDIALOG_COMMON_BUTTON_RETURN_ID

        /// <summary>
        /// </summary>
        internal enum TaskdialogCommonButtonReturnID
        {
            /// <summary>
            /// </summary>
            IDOK = 1, 

            /// <summary>
            /// </summary>
            IDCANCEL = 2, 

            /// <summary>
            /// </summary>
            IDABORT = 3, 

            /// <summary>
            /// </summary>
            IDRETRY = 4, 

            /// <summary>
            /// </summary>
            IDIGNORE = 5, 

            /// <summary>
            /// </summary>
            IDYES = 6, 

            /// <summary>
            /// </summary>
            IDNO = 7, 

            /// <summary>
            /// </summary>
            IDCLOSE = 8
        }

        #endregion

        #region Nested type: TASKDIALOG_ELEMENTS

        /// <summary>
        /// </summary>
        internal enum TaskdialogElement
        {
            /// <summary>
            /// </summary>
            TdeContent, 

            /// <summary>
            /// </summary>
            TdeExpandedInformation, 

            /// <summary>
            /// </summary>
            TdeFooter, 

            /// <summary>
            /// </summary>
            TdeMainInstruction
        }

        #endregion

        // Task Dialog - flags
        #region Nested type: TASKDIALOG_FLAGS

        /// <summary>
        /// </summary>
        [Flags]
        internal enum TaskdialogFlagss
        {
            /// <summary>
            /// </summary>
            NONE = 0, 

            /// <summary>
            /// </summary>
            TdfEnableHyperlinks = 0x0001, 

            /// <summary>
            /// </summary>
            TdfUseHiconMain = 0x0002, 

            /// <summary>
            /// </summary>
            TdfUseHiconFooter = 0x0004, 

            /// <summary>
            /// </summary>
            TdfAllowDialogCancellation = 0x0008, 

            /// <summary>
            /// </summary>
            TdfUseCommandLinks = 0x0010, 

            /// <summary>
            /// </summary>
            TdfUseCommandLinksNOIcon = 0x0020, 

            /// <summary>
            /// </summary>
            TdfExpandFooterArea = 0x0040, 

            /// <summary>
            /// </summary>
            TdfExpandedBYDefault = 0x0080, 

            /// <summary>
            /// </summary>
            TdfVerificationFlagChecked = 0x0100, 

            /// <summary>
            /// </summary>
            TdfShowProgressBar = 0x0200, 

            /// <summary>
            /// </summary>
            TdfShowMarqueeProgressBar = 0x0400, 

            /// <summary>
            /// </summary>
            TdfCallbackTimer = 0x0800, 

            /// <summary>
            /// </summary>
            TdfPositionRelativeTOWindow = 0x1000, 

            /// <summary>
            /// </summary>
            TdfRtlLayout = 0x2000, 

            /// <summary>
            /// </summary>
            TdfnoDefaultRadioButton = 0x4000
        }

        #endregion

        #region Nested type: TASKDIALOG_ICON_ELEMENT

        /// <summary>
        /// </summary>
        internal enum TaskdialogIconElement
        {
            /// <summary>
            /// </summary>
            TdieIconMain, 

            /// <summary>
            /// </summary>
            TdieIconFooter
        }

        #endregion

        #region Nested type: TASKDIALOG_MESSAGES

        /// <summary>
        /// </summary>
        internal enum TaskdialogMessage
        {
            /// <summary>
            /// </summary>
            TdmNavigatePage = CoreNativeMethods.WMUser + 101, 

            /// <summary>
            /// </summary>
            TdmClickButton = CoreNativeMethods.WMUser + 102, // wParam = Button ID
            /// <summary>
            /// </summary>
            TdmSetMarqueeProgressBar = CoreNativeMethods.WMUser + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
            /// <summary>
            /// </summary>
            TdmSetProgressBarState = CoreNativeMethods.WMUser + 104, // wParam = new progress state
            /// <summary>
            /// </summary>
            TdmSetProgressBarRange = CoreNativeMethods.WMUser + 105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
            /// <summary>
            /// </summary>
            TdmSetProgressBarPos = CoreNativeMethods.WMUser + 106, // wParam = new position
            /// <summary>
            /// </summary>
            TdmSetProgressBarMarquee = CoreNativeMethods.WMUser + 107, 

            // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
            /// <summary>
            /// </summary>
            TdmSetElementText = CoreNativeMethods.WMUser + 108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            /// <summary>
            /// </summary>
            TdmClickRadioButton = CoreNativeMethods.WMUser + 110, // wParam = Radio Button ID
            /// <summary>
            /// </summary>
            TdmEnableButton = CoreNativeMethods.WMUser + 111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
            /// <summary>
            /// </summary>
            TdmEnableRadioButton = CoreNativeMethods.WMUser + 112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
            /// <summary>
            /// </summary>
            TdmClickVerification = CoreNativeMethods.WMUser + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
            /// <summary>
            /// </summary>
            TdmUpdateElementText = CoreNativeMethods.WMUser + 114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            /// <summary>
            /// </summary>
            TdmSetButtonElevationRequiredState = CoreNativeMethods.WMUser + 115, 

            // wParam = Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
            /// <summary>
            /// </summary>
            TdmUpdateIcon = CoreNativeMethods.WMUser + 116

            // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
        }

        #endregion

        #region Nested type: TASKDIALOG_NOTIFICATIONS

        /// <summary>
        /// </summary>
        internal enum TaskdialogNotification
        {
            /// <summary>
            /// </summary>
            TdnCreated = 0, 

            /// <summary>
            /// </summary>
            TdnNavigated = 1, 

            /// <summary>
            /// </summary>
            TdnButtonClicked = 2, // wParam = Button ID
            /// <summary>
            /// </summary>
            TdnHyperlinkClicked = 3, // lParam = (LPCWSTR)pszHREF
            /// <summary>
            /// </summary>
            TdnTimer = 4, // wParam = Milliseconds since dialog created or timer reset
            /// <summary>
            /// </summary>
            TdnDestroyed = 5, 

            /// <summary>
            /// </summary>
            TdnRadioButtonClicked = 6, // wParam = Radio Button ID
            /// <summary>
            /// </summary>
            TdnDialogConstructed = 7, 

            /// <summary>
            /// </summary>
            TdnVerificationClicked = 8, // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
            /// <summary>
            /// </summary>
            TdnHelp = 9, 

            /// <summary>
            /// </summary>
            TdnExpandoButtonClicked = 10 // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
        }

        #endregion

        #region Nested type: TDIDelegate

        /// <summary>
        /// </summary>
        /// <param name="pTaskConfig">
        /// </param>
        /// <param name="pnButton">
        /// </param>
        /// <param name="pnRadioButton">
        /// </param>
        /// <param name="pVerificationFlagChecked">
        /// </param>
        internal delegate HRESULT TdiDelegate(
            [In] TASKDIALOGCONFIG pTaskConfig, [Out] out int pnButton, [Out] out int pnRadioButton, [Out] out bool pVerificationFlagChecked);

        #endregion

        // Used in the various SET_DEFAULT* TaskDialog messages
        #region Nested type: TD_ICON

        /// <summary>
        /// </summary>
        internal enum TDIcon
        {
            /// <summary>
            /// </summary>
            TDWarningIcon = 65535, 

            /// <summary>
            /// </summary>
            TDErrorIcon = 65534, 

            /// <summary>
            /// </summary>
            TDInformationIcon = 65533, 

            /// <summary>
            /// </summary>
            TDShieldIcon = 65532
        }

        #endregion

        #endregion
    }
}