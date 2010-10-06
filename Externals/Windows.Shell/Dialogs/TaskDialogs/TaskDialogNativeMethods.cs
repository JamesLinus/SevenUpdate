// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Dialogs.TaskDialogs
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
        /// <summary>
        /// </summary>
        internal const int TaskDialogIdealWidth = 0;

        /// <summary>
        /// </summary>
        internal const int TaskDialogButtonShieldIcon = 1;

        /// <summary>
        /// </summary>
        internal const int NoDefaultButtonSpecified = 0;

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
            [In] TaskDialogConfig pTaskConfig, 
            [Out] out int pnButton, 
            [Out] out int pnRadioButton, 
            [MarshalAs(UnmanagedType.Bool)] [Out] out bool pVerificationFlagChecked);

        /// <summary>
        /// </summary>
        internal enum Pbst
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
        internal delegate int TaskDialogCallBack(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData);

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal class TaskDialogConfig : IDisposable
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
            internal TaskDialogFlags dwFlags;

            /// <summary>
            /// </summary>
            internal TaskDialogCommonButtonFlags dwCommonButtons;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszWindowTitle;

            /// <summary>
            /// </summary>
            internal TaskDialogConfigIconUnion MainIcon; // NOTE: 32-bit union field, holds pszMainIcon as well

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
            internal TaskDialogConfigIconUnion FooterIcon; // NOTE: 32-bit union field, holds pszFooterIcon as well

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszFooter;

            /// <summary>
            /// </summary>
            internal TaskDialogCallBack pfCallback;

            /// <summary>
            /// </summary>
            internal IntPtr lpCallbackData;

            /// <summary>
            /// </summary>
            internal uint cxWidth;

            /// <summary>
            /// </summary>
            protected bool disposed;

            /// <summary>
            /// Finalizes an instance of the <see cref="TaskDialogConfig"/> class.
            /// </summary>
            /// <param name="disposing">
            /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.
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
            /// Finalizes an instance of the <see cref="TaskDialogConfig"/> class.
            /// </summary>
            public virtual void Dispose()
            {
                this.Dispose(false);

                // Unregister object for finalization.
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// </summary>
            ~TaskDialogConfig()
            {
                this.Dispose(false);
            }
        }

        // NOTE: We include a "spacer" so that the struct size varies on 
        // 64-bit architectures.

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        internal struct TaskDialogConfigIconUnion
        {
            /// <summary>
            /// </summary>
            /// <param name="i">
            /// </param>
            internal TaskDialogConfigIconUnion(int i)
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
        }

        // NOTE: Packing must be set to 4 to make this work on 64-bit platforms.

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct TaskDialogButton
        {
            /// <summary>
            /// </summary>
            /// <param name="n">
            /// </param>
            /// <param name="txt">
            /// </param>
            public TaskDialogButton(int n, string txt)
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
        }

        // Task Dialog - identifies common buttons.

        /// <summary>
        /// </summary>
        [Flags]
        internal enum TaskDialogCommonButtonFlags
        {
            /// <summary>
            /// </summary>
            OkButton = 0x0001, // selected control return value IDOK
            /// <summary>
            /// </summary>
            YesButton = 0x0002, // selected control return value IDYES
            /// <summary>
            /// </summary>
            NoButton = 0x0004, // selected control return value IDNO
            /// <summary>
            /// </summary>
            CancelButton = 0x0008, // selected control return value IDCANCEL
            /// <summary>
            /// </summary>
            RetryButton = 0x0010, // selected control return value IDRETRY
            /// <summary>
            /// </summary>
            CloseButton = 0x0020 // selected control return value IDCLOSE
        }

        // Identify button *return values* - note that, unfortunately, these are different
        // from the inbound button values.

        /// <summary>
        /// </summary>
        internal enum TaskDialogCommonButtonReturnID
        {
            /// <summary>
            /// </summary>
            OK = 1, 

            /// <summary>
            /// </summary>
            Cancel = 2, 

            /// <summary>
            /// </summary>
            Abort = 3, 

            /// <summary>
            /// </summary>
            Retry = 4, 

            /// <summary>
            /// </summary>
            Ignore = 5, 

            /// <summary>
            /// </summary>
            Yes = 6, 

            /// <summary>
            /// </summary>
            No = 7, 

            /// <summary>
            /// </summary>
            Close = 8
        }

        /// <summary>
        /// </summary>
        internal enum TaskDialogElement
        {
            /// <summary>
            /// </summary>
            Content, 

            /// <summary>
            /// </summary>
            ExpandedInformation, 

            /// <summary>
            /// </summary>
            Footer, 

            /// <summary>
            /// </summary>
            MainInstruction
        }

        // Task Dialog - flags

        /// <summary>
        /// </summary>
        [Flags]
        internal enum TaskDialogFlags
        {
            /// <summary>
            /// </summary>
            None = 0, 

            /// <summary>
            /// </summary>
            EnableHyperlinks = 0x0001, 

            /// <summary>
            /// </summary>
            UseHiconMain = 0x0002, 

            /// <summary>
            /// </summary>
            UseHiconFooter = 0x0004, 

            /// <summary>
            /// </summary>
            AllowDialogCancellation = 0x0008, 

            /// <summary>
            /// </summary>
            UseCommandLinks = 0x0010, 

            /// <summary>
            /// </summary>
            UseCommandLinksNoIcon = 0x0020, 

            /// <summary>
            /// </summary>
            ExpandFooterArea = 0x0040, 

            /// <summary>
            /// </summary>
            ExpandedByDefault = 0x0080, 

            /// <summary>
            /// </summary>
            VerificationFlagChecked = 0x0100, 

            /// <summary>
            /// </summary>
            ShowProgressBar = 0x0200, 

            /// <summary>
            /// </summary>
            ShowMarqueeProgressBar = 0x0400, 

            /// <summary>
            /// </summary>
            CallbackTimer = 0x0800, 

            /// <summary>
            /// </summary>
            PositionRelativeToWindow = 0x1000, 

            /// <summary>
            /// </summary>
            RtlLayout = 0x2000, 

            /// <summary>
            /// </summary>
            NoDefaultRadioButton = 0x4000
        }

        /// <summary>
        /// </summary>
        internal enum TaskDialogIconElement
        {
            /// <summary>
            /// </summary>
            IconMain, 

            /// <summary>
            /// </summary>
            IconFooter
        }

        /// <summary>
        /// </summary>
        internal enum TaskDialogMessage
        {
            /// <summary>
            /// </summary>
            NavigatePage = CoreNativeMethods.WMUser + 101, 

            /// <summary>
            /// </summary>
            ClickButton = CoreNativeMethods.WMUser + 102, // wParam = Button ID
            /// <summary>
            /// </summary>
            SetMarqueeProgressBar = CoreNativeMethods.WMUser + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
            /// <summary>
            /// </summary>
            SetProgressBarState = CoreNativeMethods.WMUser + 104, // wParam = new progress state
            /// <summary>
            /// </summary>
            SetProgressBarRange = CoreNativeMethods.WMUser + 105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
            /// <summary>
            /// </summary>
            SetProgressBarPos = CoreNativeMethods.WMUser + 106, // wParam = new position
            /// <summary>
            /// </summary>
            SetProgressBarMarquee = CoreNativeMethods.WMUser + 107, 

            // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
            /// <summary>
            /// </summary>
            SetElementText = CoreNativeMethods.WMUser + 108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            /// <summary>
            /// </summary>
            ClickRadioButton = CoreNativeMethods.WMUser + 110, // wParam = Radio Button ID
            /// <summary>
            /// </summary>
            EnableButton = CoreNativeMethods.WMUser + 111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
            /// <summary>
            /// </summary>
            EnableRadioButton = CoreNativeMethods.WMUser + 112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
            /// <summary>
            /// </summary>
            ClickVerification = CoreNativeMethods.WMUser + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
            /// <summary>
            /// </summary>
            UpdateElementText = CoreNativeMethods.WMUser + 114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            /// <summary>
            /// </summary>
            SetButtonElevationRequiredState = CoreNativeMethods.WMUser + 115, 

            /// <summary>
            ///   Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
            /// </summary>
            UpdateIcon = CoreNativeMethods.WMUser + 116

            // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
        }

        /// <summary>
        /// </summary>
        internal enum TaskDialogNotification
        {
            /// <summary>
            /// </summary>
            Created = 0, 

            /// <summary>
            /// </summary>
            Navigated = 1, 

            /// <summary>
            /// </summary>
            ButtonClicked = 2, // wParam = Button ID
            /// <summary>
            /// </summary>
            HyperlinkClicked = 3, // lParam = (LPCWSTR)pszHREF
            /// <summary>
            /// </summary>
            Timer = 4, // wParam = Milliseconds since dialog created or timer reset
            /// <summary>
            /// </summary>
            Destroyed = 5, 

            /// <summary>
            /// </summary>
            RadioButtonClicked = 6, // wParam = Radio Button ID
            /// <summary>
            /// </summary>
            DialogConstructed = 7, 

            /// <summary>
            /// </summary>
            VerificationClicked = 8, // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
            /// <summary>
            /// </summary>
            Help = 9, 

            /// <summary>
            /// </summary>
            ExpandoButtonClicked = 10 // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
        }

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
            [In] TaskDialogConfig pTaskConfig, [Out] out int pnButton, [Out] out int pnRadioButton, [Out] out bool pVerificationFlagChecked);

        // Used in the various SET_DEFAULT* TaskDialog messages

        /// <summary>
        /// </summary>
        internal enum TDIcon
        {
            /// <summary>
            /// </summary>
            WarningIcon = 65535, 

            /// <summary>
            /// </summary>
            ErrorIcon = 65534, 

            /// <summary>
            /// </summary>
            InformationIcon = 65533, 

            /// <summary>
            /// </summary>
            ShieldIcon = 65532
        }
    }
}