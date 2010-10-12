// ***********************************************************************
// <copyright file="TaskDialogNativeMethods.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
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
        /// <summary>The <see cref="TaskDialog"/> callback</summary>
        /// <param name="handle">The handle for the dialog</param>
        /// <param name="msg">The message id</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="parameterLength">The length of the parameter</param>
        /// <param name="data">The data for the callback</param>
        /// <returns>The result of the <see cref="TaskDialog"/></returns>
        internal delegate int TaskDialogCallBack(IntPtr handle, uint msg, IntPtr parameter, IntPtr parameterLength, IntPtr data);

        #region Enums

        /// <summary>Indicates the progress bar status</summary>
        internal enum ProgressBarStatus
        {
            /// <summary>Normal status</summary>
            Normal = 0x0001, 

            /// <summary>Red progress</summary>
            Error = 0x0002, 

            /// <summary>Yellow progress</summary>
            Paused = 0x0003
        }

        /// <summary>Specifies the push buttons displayed in the task dialog. If no common buttons are specified and no custom buttons are specified using the Buttons and Buttons members, the task dialog will contain the OK button by default.</summary>
        [Flags]
        internal enum TaskDialogCommonButtonFlags
        {
            /// <summary>The task dialog contains the push button: OK.</summary>
            OkButton = 0x0001, 

            /// <summary>The task dialog contains the push button: Yes.</summary>
            YesButton = 0x0002, 

            /// <summary>The task dialog contains the push button: No.</summary>
            NoButton = 0x0004, 

            /// <summary>The task dialog contains the push button: Cancel. If this button is specified, the task dialog will respond to typical cancel actions (Alt-F4 and Escape).</summary>
            CancelButton = 0x0008, 

            /// <summary>The task dialog contains the push button: Retry.</summary>
            RetryButton = 0x0010, 

            /// <summary>The task dialog contains the push button: Close.</summary>
            CloseButton = 0x0020
        }

        /// <summary>The button return ids</summary>
        internal enum TaskDialogCommonButtonReturnID
        {
            /// <summary>The button returned OK</summary>
            OK = 1, 

            /// <summary>The button returned cancel</summary>
            Cancel = 2, 

            /// <summary>The button return abort</summary>
            Abort = 3, 

            /// <summary>The button returned retry</summary>
            Retry = 4, 

            /// <summary>The button was ignored</summary>
            Ignore = 5, 

            /// <summary>The button returned yes</summary>
            Yes = 6, 

            /// <summary>The button returned no</summary>
            No = 7, 

            /// <summary>The button returned close</summary>
            Close = 8
        }

        /// <summary>The task dialog elements</summary>
        internal enum TaskDialogElement
        {
            /// <summary>The main portion of the dialog</summary>
            Content, 

            /// <summary>Content in the expander</summary>
            ExpandedInformation, 

            /// <summary>The footer of the dialog</summary>
            Footer, 

            /// <summary>The main instructions for the dialog</summary>
            MainInstruction
        }

        /// <summary>Specifies the behavior of the task dialog.</summary>
        [Flags]
        internal enum TaskDialogFlags
        {
            /// <summary>Empty dialog</summary>
            None = 0, 

            /// <summary>Enables hyperlink processing for the strings specified in the pszContent, pszExpandedInformation and pszFooter members.</summary>
            EnableHyperlinks = 0x0001, 

            /// <summary>Indicates that the dialog should use the icon referenced by the handle in the MainIcon member as the primary icon in the task dialog. If this flag is specified, the MainIcon member is ignored.</summary>
            UseIconMain = 0x0002, 

            /// <summary>Indicates that the dialog should use the icon referenced by the handle in the FooterIcon member as the footer icon in the task dialog. If this flag is specified, the FooterIcon member is ignored.</summary>
            UseIconFooter = 0x0004, 

            /// <summary>Indicates that the dialog should be able to be closed using Alt-F4, Escape, and the title bar's close button even if no cancel button is specified in either the CommonButtons or Buttons members.</summary>
            AllowDialogCancellation = 0x0008, 

            /// <summary>Indicates that the buttons specified in the Buttons member are to be displayed as command links (using a standard task dialog glyph) instead of push buttons. When using command links, all characters up to the first new line character in the pszButtonText member will be treated as the command link's main text, and the remainder will be treated as the command link's note. This flag is ignored if the Buttons member is zero.</summary>
            UseCommandLinks = 0x0010, 

            /// <summary>Indicates that the buttons specified in the Buttons member are to be displayed as command links (without a glyph) instead of push buttons. When using command links, all characters up to the first new line character in the ButtonText member will be treated as the command link's main text, and the remainder will be treated as the command link's note. This flag is ignored if the Buttons member is zero.</summary>
            UseCommandLinksNoIcon = 0x0020, 

            /// <summary>Indicates that the string specified by the ExpandedInformation member is displayed at the bottom of the dialog's footer area instead of immediately after the dialog's content. This flag is ignored if the ExpandedInformation member is null.</summary>
            ExpandFooterArea = 0x0040, 

            /// <summary>Indicates that the string specified by the ExpandedInformation member is displayed when the dialog is initially displayed. This flag is ignored if the ExpandedInformation member is <see langword = "null" />.</summary>
            ExpandedByDefault = 0x0080, 

            /// <summary>Indicates that the verification checkbox in the dialog is checked when the dialog is initially displayed. This flag is ignored if the VerificationText parameter is null.</summary>
            VerificationFlagChecked = 0x0100, 

            /// <summary>Indicates that a Progress Bar is to be displayed.</summary>
            ShowProgressBar = 0x0200, 

            /// <summary>Indicates that an Marquee Progress Bar is to be displayed.</summary>
            ShowMarqueeProgressBar = 0x0400, 

            /// <summary>Indicates that the task dialog's callback is to be called approximately every 200 milliseconds.</summary>
            CallbackTimer = 0x0800, 

            /// <summary>Indicates that the task dialog is positioned (centered) relative to the window specified by Parent. If the flag is not supplied (or no Parent member is specified), the task dialog is positioned (centered) relative to the monitor.</summary>
            PositionRelativeToWindow = 0x1000, 

            /// <summary>Indicates that text is displayed reading right to left.</summary>
            RtlLayout = 0x2000, 

            /// <summary>Indicates that no default item will be selected.</summary>
            NoDefaultRadioButton = 0x4000
        }

        /// <summary>The icon elements for the dialog</summary>
        internal enum TaskDialogIconElement
        {
            /// <summary>The main icon</summary>
            IconMain, 

            /// <summary>The footer icon</summary>
            IconFooter
        }

        /// <summary>Dialog messages</summary>
        internal enum TaskDialogMessage
        {
            /// <summary>Recreates a task dialog with new contents, simulating the functionality of a multi-page wizard.</summary>
            NavigatePage = NativeMethods.WmUser + 101, 

            /// <summary>Simulates the action of a button click in a dialog.</summary>
            ClickButton = NativeMethods.WmUser + 102, 

            /// <summary>parameter = 0 (nonMarque) parameter != 0 (Marquee)</summary>
            SetMarqueeProgressBar = NativeMethods.WmUser + 103, 

            /// <summary>Sets the current state of the progress bar.</summary>
            SetProgressBarState = NativeMethods.WmUser + 104, 

            /// <summary>Sets the minimum and maximum values for the hosted progress bar.</summary>
            SetProgressBarRange = NativeMethods.WmUser + 105, 

            /// <summary>Sets the current position for a progress bar.</summary>
            SetProgressBarPos = NativeMethods.WmUser + 106, 

            /// <summary>Indicates whether the hosted progress bar should be displayed in marquee mode.</summary>
            SetProgressBarMarquee = NativeMethods.WmUser + 107, 

            /// <summary>Updates a text element in a task dialog.</summary>
            SetElementText = NativeMethods.WmUser + 108, 

            /// <summary>Simulates the action of a radio button click in a task dialog.</summary>
            ClickRadioButton = NativeMethods.WmUser + 110, 

            /// <summary>Enables or disables a push button in a task dialog.</summary>
            EnableButton = NativeMethods.WmUser + 111, 

            /// <summary>Enables or disables a radio button in a task dialog.</summary>
            EnableRadioButton = NativeMethods.WmUser + 112, 

            /// <summary>Simulates the action of a verification checkbox click in a task dialog.</summary>
            ClickVerification = NativeMethods.WmUser + 113, 

            /// <summary>Updates a text element in a task dialog.</summary>
            UpdateElementText = NativeMethods.WmUser + 114, 

            /// <summary>Specifies whether a given task dialog button or command link should have a UAC shield icon; that is, whether the action invoked by the button requires elevation.</summary>
            SetButtonElevationRequiredState = NativeMethods.WmUser + 115, 

            /// <summary>Refreshes the icon of a task dialog.</summary>
            UpdateIcon = NativeMethods.WmUser + 116
        }

        /// <summary>The notification ids</summary>
        internal enum TaskDialogNotification
        {
            /// <summary>Sent by a task dialog after the dialog has been created and before it is displayed. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            Created = 0, 

            /// <summary>Sent by a task dialog when a navigation has occurred. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            Navigated = 1, 

            /// <summary>Sent by a task dialog when the user selects a button or command link in the task dialog. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            ButtonClicked = 2, 

            /// <summary>Sent by a task dialog when the user clicks a hyperlink in the task dialog content. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            HyperlinkClicked = 3, 

            /// <summary>Sent by a task dialog approximately every 200 milliseconds. This notification code is sent when the CallbackTimer flag has been set in the flags member of the TaskDialog structure that was passed to the TaskDialogIndirect function. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            Timer = 4, 

            /// <summary>Sent by a task dialog when it is destroyed and its window handle is no longer valid. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            Destroyed = 5, 

            /// <summary>Sent by a task dialog when the user selects a button or command link in the task dialog. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            RadioButtonClicked = 6, 

            /// <summary>Sent by a task dialog after the dialog has been created and before it is displayed. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            DialogConstructed = 7, 

            /// <summary>Sent by the task dialog when the user clicks the task dialog verification check box. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            VerificationClicked = 8, 

            /// <summary>Sent by a task dialog when the user presses F1 on the keyboard while the dialog has focus. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            Help = 9, 

            /// <summary>Sent by a task dialog when the user clicks on the dialog's expanded button. This notification code is received only through the task dialog callback function, which can be registered using the TaskDialogIndirect method.</summary>
            ExpandButtonClicked = 10
        }

        /// <summary>The task dialog main icons</summary>
        internal enum TaskDialogIcon
        {
            /// <summary>An exclamation-point icon appears in the task dialog.</summary>
            WarningIcon = 65535, 

            /// <summary>A stop-sign icon appears in the task dialog.</summary>
            ErrorIcon = 65534, 

            /// <summary>An icon consisting of a lowercase letter i in a circle appears in the task dialog.</summary>
            InformationIcon = 65533, 

            /// <summary>A shield icon appears in the task dialog.</summary>
            ShieldIcon = 65532
        }

        #endregion

        /// <summary>The TaskDialogIndirect function creates, displays, and operates a task dialog. The task dialog contains application-defined icons, messages, title, verification check box, command links, push buttons, and radio buttons. This function can register a callback function to receive notification messages.</summary>
        /// <param name="taskConfig">A pointer to a <see cref="TaskDialogConfig"/> structure that contains information used to display the task dialog.</param>
        /// <param name="button">Address of a variable that receives one of the button IDs specified in the <paramref name="button"/> member of the <paramref name="taskConfig"/> parameter. If this parameter is <see langword="null"/>, no value is returned.</param>
        /// <param name="radioButton">Address of a variable that receives one of the button IDs specified in the <paramref name="radioButton"/> member of the <paramref name="taskConfig"/> parameter. If this parameter is <see langword="null"/>, no value is returned.</param>
        /// <param name="verificationFlagChecked"><see langword="true"/> if the verification <see cref="CheckBox"/> was checked when the dialog was dismissed; otherwise, false</param>
        /// <returns>The result</returns>
        [DllImport(@"comctl32.dll", CharSet = CharSet.Auto, PreserveSig = false)]
        internal static extern Result TaskDialogIndirect([In] TaskDialogConfig taskConfig, [Out] out int button, [Out] out int radioButton, [MarshalAs(UnmanagedType.Bool), Out] out bool verificationFlagChecked);

        /// <summary>Contains the data for a <see cref="TaskDialogIcon"/></summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        internal struct TaskDialogConfigIconUnion
        {
            /// <summary>The main icon Id to display</summary>
            [FieldOffset(0)]
            private readonly int MainIcon;

            /// <summary>The footer icon id to display</summary>
            [FieldOffset(0)]
            private readonly int Icon;

            /// <summary>The pointer to the space</summary>
            [FieldOffset(0)]
            private readonly IntPtr Spacer;

            /// <summary>Initializes a new instance of the <see cref="TaskDialogConfigIconUnion"/> struct.</summary>
            /// <param name="i">The icon identifier</param>
            internal TaskDialogConfigIconUnion(int i)
            {
                this.Spacer = IntPtr.Zero;
                this.Icon = 0;
                this.MainIcon = i;
            }
        }

        /// <summary>Contains the data for a <see cref="TaskDialogButton"/></summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct TaskDialogButtonData
        {
            /// <summary>Indicates the value to be returned when this button is selected.</summary>
            internal int ButtonID;

            /// <summary>The text to display on the button</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string ButtonText;

            /// <summary>Initializes a new instance of the <see cref="TaskDialogButtonData"/> struct.</summary>
            /// <param name="buttonID">The button ID.</param>
            /// <param name="buttonText">The button text.</param>
            public TaskDialogButtonData(int buttonID, string buttonText)
            {
                this.ButtonID = buttonID;
                this.ButtonText = buttonText;
            }
        }

        /// <summary>Contains information used to display a task dialog</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal class TaskDialogConfig
        {
            /// <summary>Specifies the structure size, in bytes.</summary>
            internal uint Size;

            /// <summary>Handle to the parent window. This member can be <see langword = "null" />.</summary>
            internal IntPtr handleParent;

            /// <summary>Handle to the module that contains the icon resource identified by the mainIcon or footerIcon members, and the string resources identified by the windowTitle, mainInstruction, content, verificationText, expandedInformation, expandedControlText, collapsedControlText or footer members.</summary>
            internal IntPtr Instance;

            /// <summary>Specifies the behavior of the task dialog</summary>
            internal TaskDialogFlags flags;

            /// <summary>Specifies the push buttons displayed in the task dialog. If no common buttons are specified and no custom buttons are specified using the buttons and pButtons members, the task dialog will contain the OK button by default.</summary>
            internal TaskDialogCommonButtonFlags CommonButtons;

            /// <summary>Pointer that references the string to be used for the task dialog title. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MakeIntResource macro. If this parameter is <see langword = "null" />, the filename of the executable program is used.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string WindowTitle;

            /// <summary>A handle to an Icon that is to be displayed in the task dialog.</summary>
            internal TaskDialogConfigIconUnion MainIcon; // NOTE: 32-bit union field, holds MainIcon as well

            /// <summary>A pointer that references the string to be used for the main instruction. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string MainInstruction;

            /// <summary>Pointer that references the string to be used for the dialog's primary content. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. If the EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in the form: <A HREF = "executablestring">Hyperlink Text</A>.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string Content;

            /// <summary>The number of entries in the buttons array that is used to create buttons or command links in the dialog. If this member is zero and no common buttons have been specified using the <see cref = "CommonButtons" /> member, then the task dialog will have a single OK button displayed.</summary>
            internal uint ButtonLength;

            /// <summary>A pointer to an array of <see cref = "TaskDialogButton" /> structures containing the definition of the custom buttons that are to be displayed in the dialog. This array must contain at least the number of entries that are specified by the buttons member.</summary>
            internal IntPtr ButtonCollection;

            /// <summary>The default button for the dialog. This may be any of the values specified in ButtonID members of one of the <see cref = "TaskDialogButton" /> structures in the <see cref = "ButtonCollection" />, or one of the IDs corresponding to the buttons specified in the <see cref = "CommonButtons" /> member</summary>
            internal int DefaultButton;

            /// <summary>The number of entries in the <see cref = "RadioButtonCollection" /> that is used to create radio buttons in the dialog.</summary>
            internal uint RadioButtonsLength;

            /// <summary>Pointer to an array of <see cref = "TaskDialogButton" /> structures containing the definition of the radio buttons that are to be displayed in the dialog. This array must contain at least the number of entries that are specified by the cRadioButtons member. This parameter can be <see langword = "null" />.</summary>
            internal IntPtr RadioButtonCollection;

            /// <summary>The button ID of the radio button that is selected by default. If this value does not correspond to a button ID, the first button in the array is selected by default.</summary>
            internal int DefaultRadioButton;

            /// <summary>Pointer that references the string to be used to label the verification <see cref = "CheckBox" />. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. If this parameter is <see langword = "null" />, the verification <see cref = "CheckBox" /> is not displayed in the task dialog. If the VerificationFlagChecked parameter of TaskDialogIndirect is <see langword = "null" />, the <see cref = "CheckBox" /> is not enabled.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string VerificationText;

            /// <summary>Pointer that references the string to be used for displaying additional information. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. The additional information is displayed either immediately below the content or below the footer text depending on whether the ExpanderFooterArea flag is specified. If the EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in the form: <A HREF = "executablestring">Hyperlink Text</A>.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string ExpandedInformation;

            /// <summary>Pointer that references the string to be used to label the button for collapsing the expandable information. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. This member is ignored when the <see cref = "ExpandedInformation" /> member is <see langword = "null" />. If this member is <see langword = "null" /> and the <see cref = "CollapsedControlText" /> is specified, then the <see cref = "CollapsedControlText" /> value will be used for this member as well.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string ExpandedControlText;

            /// <summary>Pointer that references the string to be used to label the button for expanding the expandable information. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. This member is ignored when the <see cref = "ExpandedInformation" /> member is <see langword = "null" />. If this member is <see langword = "null" /> and the CollapsedControlText is specified, then the CollapsedControlText value will be used for this member as well.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string CollapsedControlText;

            /// <summary>A handle to an Icon that is to be displayed in the footer of the task dialog.</summary>
            internal TaskDialogConfigIconUnion FooterIcon; // NOTE: 32-bit union field, holds FooterIcon as well

            /// <summary>Pointer to the string to be used in the footer area of the task dialog. This parameter can be either a <see langword = "null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. If the EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in this form.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string Footer;

            /// <summary>The application-defined callback function.</summary>
            internal TaskDialogCallBack Callback;

            /// <summary>A pointer to application-defined reference data. This value is defined by the caller.</summary>
            internal IntPtr CallbackData;

            /// <summary>The width of the task dialog's client area. If 0, the task dialog manager will calculate the ideal width.</summary>
            internal uint Width;
        }
    }
}