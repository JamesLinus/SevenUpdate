// ***********************************************************************
// <copyright file="TaskDialogNativeMethods.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    using System.Runtime.InteropServices;
    using System.Windows.Controls;
    using System.Windows.Internal;

    /// <summary>
    /// Internal class containing most native interop declarations used
    ///   throughout the library.
    ///   Functions that are not performance intensive belong in this class.
    /// </summary>
    internal static class TaskDialogNativeMethods
    {
        /// <summary>
        /// Indicates the progress bar status
        /// </summary>
        internal enum ProgressBarStatus
        {
            /// <summary>
            ///   Normal status
            /// </summary>
            Normal = 0x0001, 

            /// <summary>
            ///   Red progress
            /// </summary>
            Error = 0x0002, 

            /// <summary>
            ///   Yellow progress
            /// </summary>
            Paused = 0x0003
        }

        /// <summary>
        /// The Common button flags
        /// </summary>
        [Flags]
        internal enum TaskDialogCommonButtonFlags
        {
            /// <summary>
            ///   The OK button was pressed
            /// </summary>
            OkButton = 0x0001, 

            /// <summary>
            ///   The yes button was pressed
            /// </summary>
            YesButton = 0x0002, 

            /// <summary>
            ///   The no button was pressed
            /// </summary>
            NoButton = 0x0004, 

            /// <summary>
            ///   The cancel button was pressed
            /// </summary>
            CancelButton = 0x0008, 

            /// <summary>
            ///   The retry button was pressed
            /// </summary>
            RetryButton = 0x0010, 

            /// <summary>
            ///   The close button was pressed
            /// </summary>
            CloseButton = 0x0020
        }

        /// <summary>
        /// The button return ids
        /// </summary>
        internal enum TaskDialogCommonButtonReturnID
        {
            /// <summary>
            ///   The button returned OK
            /// </summary>
            OK = 1, 

            /// <summary>
            ///   The button returned cancel
            /// </summary>
            Cancel = 2, 

            /// <summary>
            ///   The button return abort
            /// </summary>
            Abort = 3, 

            /// <summary>
            ///   The button returned retry
            /// </summary>
            Retry = 4, 

            /// <summary>
            ///   The button was ignored
            /// </summary>
            Ignore = 5, 

            /// <summary>
            ///   The button returned yes
            /// </summary>
            Yes = 6, 

            /// <summary>
            ///   The button returned no
            /// </summary>
            No = 7, 

            /// <summary>
            ///   The button returned close
            /// </summary>
            Close = 8
        }

        /// <summary>
        /// The task dialog elements
        /// </summary>
        internal enum TaskDialogElement
        {
            /// <summary>
            ///   The main portion of the dialog
            /// </summary>
            Content, 

            /// <summary>
            ///   Content in the expander
            /// </summary>
            ExpandedInformation, 

            /// <summary>
            ///   The footer of the dialog
            /// </summary>
            Footer, 

            /// <summary>
            ///   The main instructions for the dialog
            /// </summary>
            MainInstruction
        }

        /// <summary>
        /// Task dialog flags
        /// </summary>
        [Flags]
        internal enum TaskDialogFlags
        {
            /// <summary>
            ///   Empty dialog
            /// </summary>
            None = 0, 

            /// <summary>
            ///   Hyperlinks enabled
            /// </summary>
            EnableHyperlinks = 0x0001, 

            /// <summary>
            ///   Use a main icon
            /// </summary>
            UseIconMain = 0x0002, 

            /// <summary>
            ///   Use a footer icon
            /// </summary>
            UseIconFooter = 0x0004, 

            /// <summary>
            ///   Allow the dialog to be canceled
            /// </summary>
            AllowDialogCancellation = 0x0008, 

            /// <summary>
            ///   Use command links
            /// </summary>
            UseCommandLinks = 0x0010, 

            /// <summary>
            ///   Use command links with no icons
            /// </summary>
            UseCommandLinksNoIcon = 0x0020, 

            /// <summary>
            ///   Expands the footer area
            /// </summary>
            ExpandFooterArea = 0x0040, 

            /// <summary>
            ///   Expands the footer by default
            /// </summary>
            ExpandedByDefault = 0x0080, 

            /// <summary>
            ///   Verification enabled
            /// </summary>
            VerificationFlagChecked = 0x0100, 

            /// <summary>
            ///   Shows a progress bar
            /// </summary>
            ShowProgressBar = 0x0200, 

            /// <summary>
            ///   Shows the progress bar as a marquee
            /// </summary>
            ShowMarqueeProgressBar = 0x0400, 

            /// <summary>
            ///   The callback timer
            /// </summary>
            CallbackTimer = 0x0800, 

            /// <summary>
            ///   The dialog position relative to the window
            /// </summary>
            PositionRelativeToWindow = 0x1000, 

            /// <summary>
            ///   Use right to left layout
            /// </summary>
            RtlLayout = 0x2000, 

            /// <summary>
            ///   No default radio button selected
            /// </summary>
            NoDefaultRadioButton = 0x4000
        }

        /// <summary>
        /// The icon elements for the dialog
        /// </summary>
        internal enum TaskDialogIconElement
        {
            /// <summary>
            ///   The main icon
            /// </summary>
            IconMain, 

            /// <summary>
            ///   The footer icon
            /// </summary>
            IconFooter
        }

        /// <summary>
        /// Dialog messages
        /// </summary>
        internal enum TaskDialogMessage
        {
            /// <summary>
            ///   Navigates the page
            /// </summary>
            NavigatePage = NativeMethods.WMUser + 101, 

            /// <summary>
            ///   parameter = Button ID
            /// </summary>
            ClickButton = NativeMethods.WMUser + 102, 

            /// <summary>
            ///   parameter = 0 (nonMarque) parameter != 0 (Marquee)
            /// </summary>
            SetMarqueeProgressBar = NativeMethods.WMUser + 103, 

            /// <summary>
            ///   parameter = new progress state
            /// </summary>
            SetProgressBarState = NativeMethods.WMUser + 104, 

            /// <summary>
            ///   parameterLength = MAKELPARAM(nMinRange, nMaxRange)
            /// </summary>
            SetProgressBarRange = NativeMethods.WMUser + 105, 

            /// <summary>
            ///   parameter = new position
            /// </summary>
            SetProgressBarPos = NativeMethods.WMUser + 106, 

            /// <summary>
            ///   Sets the progress bar state to a marquee
            /// </summary>
            SetProgressBarMarquee = NativeMethods.WMUser + 107, 

            /// <summary>
            ///   parameter = element (TASKDIALOG_ELEMENTS), parameterLength = new element text (LPCWSTR)
            /// </summary>
            SetElementText = NativeMethods.WMUser + 108, 

            /// <summary>
            ///   parameter = Radio Button ID
            /// </summary>
            ClickRadioButton = NativeMethods.WMUser + 110, 

            /// <summary>
            ///   parameterLength = 0 (disable), parameterLength != 0 (enable), parameter = Button ID
            /// </summary>
            EnableButton = NativeMethods.WMUser + 111, 

            /// <summary>
            ///   parameterLength = 0 (disable), parameterLength != 0 (enable), parameter = Radio Button ID
            /// </summary>
            EnableRadioButton = NativeMethods.WMUser + 112, 

            /// <summary>
            ///   parameter = 0 (unchecked), 1 (checked), parameterLength = 1 (set key focus)
            /// </summary>
            ClickVerification = NativeMethods.WMUser + 113, 

            /// <summary>
            ///   parameter = element (TASKDIALOG_ELEMENTS), parameterLength = new element text (LPCWSTR)
            /// </summary>
            UpdateElementText = NativeMethods.WMUser + 114, 

            /// <summary>
            ///   Button ID, parameterLength = 0 (elevation not required), parameterLength != 0 (elevation required)
            /// </summary>
            SetButtonElevationRequiredState = NativeMethods.WMUser + 115, 

            /// <summary>
            ///   Updates the icon
            /// </summary>
            UpdateIcon = NativeMethods.WMUser + 116
        }

        /// <summary>
        /// The notification ids
        /// </summary>
        internal enum TaskDialogNotification
        {
            /// <summary>
            ///   Task Dialog created
            /// </summary>
            Created = 0, 

            /// <summary>
            ///   Hyperlink navigated
            /// </summary>
            Navigated = 1, 

            /// <summary>
            ///   A button was clicked
            /// </summary>
            ButtonClicked = 2, 

            /// <summary>
            ///   Hyperlink clicked
            /// </summary>
            HyperlinkClicked = 3, 

            /// <summary>
            ///   A dialog timer
            /// </summary>
            Timer = 4, 

            /// <summary>
            ///   TaskDialog destroyed
            /// </summary>
            Destroyed = 5, 

            /// <summary>
            ///   Radio button clicked
            /// </summary>
            RadioButtonClicked = 6, 

            /// <summary>
            ///   Dialog created
            /// </summary>
            DialogConstructed = 7, 

            /// <summary>
            ///   Verification was clicked
            /// </summary>
            VerificationClicked = 8, 

            /// <summary>
            ///   Help link clicked
            /// </summary>
            Help = 9, 

            /// <summary>
            ///   Expand button clicked
            /// </summary>
            ExpandButtonClicked = 10
        }

        /// <summary>
        /// The task dialog main icons
        /// </summary>
        internal enum TaskDialogIcon
        {
            /// <summary>
            ///   Displays a warning icon
            /// </summary>
            WarningIcon = 65535, 

            /// <summary>
            ///   Displays a red x icon
            /// </summary>
            ErrorIcon = 65534, 

            /// <summary>
            ///   Displays a blue ! icon
            /// </summary>
            InformationIcon = 65533, 

            /// <summary>
            ///   Displays the UAC shield
            /// </summary>
            ShieldIcon = 65532
        }

        /// <summary>
        /// </summary>
        /// <param name="taskConfig">
        /// </param>
        /// <param name="button">
        /// </param>
        /// <param name="radioButton">
        /// </param>
        /// <param name="verificationFlagChecked">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport(@"comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Result TaskDialogIndirect(
            [In] TaskDialogConfig taskConfig, [Out] out int button, [Out] out int radioButton, [MarshalAs(UnmanagedType.Bool)] [Out] out bool verificationFlagChecked);

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal class TaskDialogConfig : IDisposable
        {
            /// <summary>
            ///   The size of the dialog
            /// </summary>
            internal uint Size;

            /// <summary>
            ///   The parent for the dialog
            /// </summary>
            internal IntPtr handleParent;

            /// <summary>
            ///   The instance of the dialog
            /// </summary>
            internal IntPtr Instance;

            /// <summary>
            ///   The flags for this instance
            /// </summary>
            internal TaskDialogFlags flags;

            /// <summary>
            ///   The task dialog buttons
            /// </summary>
            internal TaskDialogCommonButtonFlags CommonButtons;

            /// <summary>
            ///   The text to display on the window
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string WindowTitle;

            /// <summary>
            ///   The main icon for the dialog
            /// </summary>
            internal TaskDialogConfigIconUnion MainIcon; // NOTE: 32-bit union field, holds MainIcon as well

            /// <summary>
            ///   The main text for the dialog
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string MainInstruction;

            /// <summary>
            ///   The sub text for the dialog
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string Content;

            /// <summary>
            ///   The length for the buttons
            /// </summary>
            internal uint ButtonLength;

            /// <summary>
            ///   The collection of buttons to display
            /// </summary>
            internal IntPtr ButtonCollection; // Ptr to TASKDIALOG_BUTTON structs

            /// <summary>
            ///   The id for the default button
            /// </summary>
            internal int DefaultButton;

            /// <summary>
            ///   The collection of <see cref = "RadioButton" />
            /// </summary>
            internal uint RadioButtonsLength;

            /// <summary>
            ///   The radio button collection
            /// </summary>
            internal IntPtr RadioButtonCollection; // Ptr to TASKDIALOG_BUTTON structs

            /// <summary>
            ///   The id for the default radio button
            /// </summary>
            internal int DefaultRadioButton;

            /// <summary>
            ///   The verification text
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string VerificationText;

            /// <summary>
            ///   The expanded information text
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string ExpandedInformation;

            /// <summary>
            ///   The text to display on the expander
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string ExpandedControlText;

            /// <summary>
            ///   The collapsed control text
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string CollapsedControlText;

            /// <summary>
            ///   The footer icon
            /// </summary>
            internal TaskDialogConfigIconUnion FooterIcon; // NOTE: 32-bit union field, holds FooterIcon as well

            /// <summary>
            ///   The footer text
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string Footer;

            /// <summary>
            ///   The dialog callback
            /// </summary>
            internal TaskDialogCallBack Callback;

            /// <summary>
            ///   The dialog data
            /// </summary>
            internal IntPtr CallbackData;

            /// <summary>
            ///   The dialog width
            /// </summary>
            internal uint Width;

            /// <summary>
            ///   Indicates if the dialog is disposed
            /// </summary>
            protected bool disposed;

            /// <summary>
            /// Finalizes an instance of the <see cref="TaskDialogConfig"/> class.
            /// </summary>
            ~TaskDialogConfig()
            {
                this.Dispose(false);
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
        }

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
                this.Spacer = IntPtr.Zero;
                this.Icon = 0;
                this.hMainIcon = i;
            }

            /// <summary>
            /// </summary>
            [FieldOffset(0)]
            internal int hMainIcon;

            /// <summary>
            /// </summary>
            [FieldOffset(0)]
            internal int Icon;

            /// <summary>
            /// </summary>
            [FieldOffset(0)]
            internal IntPtr Spacer;
        }

        /// <summary>
        /// Contains data for the TaskDialogButton
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct TaskDialogButtonData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TaskDialogButtonData"/> struct.
            /// </summary>
            /// <param name="buttonID">
            /// The button ID.
            /// </param>
            /// <param name="buttonText">
            /// The button text.
            /// </param>
            public TaskDialogButtonData(int buttonID, string buttonText)
            {
                this.ButtonID = buttonID;
                this.ButtonText = buttonText;
            }

            /// <summary>
            ///   The button id
            /// </summary>
            internal int ButtonID;

            /// <summary>
            ///   The text to display on the button
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string ButtonText;
        }

        /// <summary>
        /// </summary>
        internal delegate Result TdiDelegate(
            [In] TaskDialogConfig pTaskConfig, [Out] out int pnButton, [Out] out int pnRadioButton, [Out] out bool pVerificationFlagChecked);

        /// <summary>
        /// </summary>
        internal delegate int TaskDialogCallBack(IntPtr handle, uint msg, IntPtr parameter, IntPtr parameterLength, IntPtr data);
    }
}