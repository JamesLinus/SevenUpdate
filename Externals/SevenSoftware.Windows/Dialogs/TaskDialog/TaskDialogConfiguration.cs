// <copyright file="TaskDialogConfiguration.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;
    using System.Runtime.InteropServices;

    using SevenSoftware.Windows.Internal;

    /// <summary>Contains information used to display a task dialog. NOTE: Do not convert to auto properties and do not change layout or it will break!</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal class TaskDialogConfiguration
    {
        /// <summary>Gets or sets the structure size, in bytes.</summary>
        internal uint Size;

        /// <summary>Gets or sets the handle to the parent window. This member can be <c>null</c>.</summary>
        internal IntPtr ParentHandle;

        /// <summary>
        ///   Gets or sets the handle to the module that contains the icon resource identified by the mainIcon or
        ///   footerIcon members, and the string resources identified by the windowTitle, mainInstruction, content,
        ///   verificationText, expandedInformation, expandedControlText, collapsedControlText or footer members.
        /// </summary>
        internal IntPtr Instance;

        /// <summary>Gets or sets the behavior of the task dialog.</summary>
        internal TaskDialogOptions TaskDialogFlags;

        /// <summary>Gets or sets the common buttons.</summary>
        internal TaskDialogCommonButtons CommonButtons;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for the task dialog title. This parameter can
        ///   be either a <c>null</c>-terminated string or an integer resource identifier passed to the MakeIntResource
        ///   macro. If this parameter is <c>null</c>, the filename of the executable program is used.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string WindowTitle;

        /// <summary>Gets or sets the main icon.</summary>
        internal IconUnion MainIcon;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for the main instruction. This parameter can
        ///   be either a <c>null</c>-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE
        ///   macro.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string MainInstruction;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for the dialog's primary content. This
        ///   parameter can be either a <c>null</c>-terminated string or an integer resource identifier passed to the
        ///   MAKEINTRESOURCE macro. If the EnableHyperlinks flag is specified for the flags member, then this string
        ///   may contain hyperlinks in the form: <a href = "executablestring">Hyperlink Text</a>.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Content;

        /// <summary>
        ///   Gets or sets the number of entries in the buttons array that is used to create buttons or command links in
        ///   the dialog. If this member is zero and no common buttons have been specified using the
        ///   <c>CommonButtons</c> member, then the task dialog will have a single OK button displayed.
        /// </summary>
        internal uint ButtonCount;

        /// <summary>
        ///   Gets or sets a pointer to an array of <c>TaskDialogButton</c> structures containing the definition of the
        ///   custom buttons that are to be displayed in the dialog. This array must contain at least the number of
        ///   entries that are specified by the buttons member.
        /// </summary>
        internal IntPtr Buttons;

        /// <summary>Gets or sets the default button index.</summary>
        internal int DefaultButtonIndex;

        /// <summary>
        ///   Gets or sets the number of entries in the <c>RadioButtonCollection</c> that is used to create radio
        ///   buttons in the dialog.
        /// </summary>
        internal uint RadioButtonCount;

        /// <summary>
        ///   Gets or sets a pointer to an array of <c>TaskDialogButton</c> structures containing the definition of the
        ///   radio buttons that are to be displayed in the dialog. This array must contain at least the number of
        ///   entries that are specified by the cRadioButtons member. This parameter can be <c>null</c>.
        /// </summary>
        internal IntPtr RadioButtons;

        /// <summary>
        ///   Gets or sets the button ID of the radio button that is selected by default. If this value does not
        ///   correspond to a button ID, the first button in the array is selected by default.
        /// </summary>
        internal int DefaultRadioButtonIndex;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used to label the verification <c>CheckBox</c>.
        ///   This parameter can be either a <c>null</c>-terminated string or an integer resource identifier passed to
        ///   the MAKEINTRESOURCE macro. If this parameter is <c>null</c>, the verification checkbox is not displayed in
        ///   the task dialog. If the VerificationFlagChecked parameter of TaskDialogIndirect is <c>null</c>, the
        ///   <c>CheckBox</c> is not enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string VerificationText;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for displaying additional information. This
        ///   parameter can be either a <c>null</c>-terminated string or an integer resource identifier passed to the
        ///   MAKEINTRESOURCE macro. The additional information is displayed either immediately below the content or
        ///   below the footer text depending on whether the ExpanderFooterArea flag is specified. If the
        ///   EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in the
        ///   form: <a
        /// href = "executablestring">Hyperlink Text</a>.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string ExpandedInformation;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used to label the button for collapsing the
        ///   expandable information. This parameter can be either a <c>null</c>-terminated string or an integer
        ///   resource identifier passed to the MAKEINTRESOURCE macro. This member is ignored when the
        ///   <c>ExpandedInformation</c> member is <c>null</c>. If this member is <c>null</c> and the
        ///   <c>CollapsedControlText"</c> is specified, then the <c>CollapsedControlText</c> value will be used for
        ///   this member as well.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string ExpandedControlText;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used to label the button for expanding the
        ///   expandable information. This parameter can be either a <c>null</c>-terminated string or an integer
        ///   resource identifier passed to the MAKEINTRESOURCE macro. This member is ignored when the
        ///   <c>ExpandedInformation</c> member is <c>null</c>. If this member is <c>null</c> and the
        ///   CollapsedControlText is specified, then the CollapsedControlText value will be used for this member as
        ///   well.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string CollapsedControlText;

        /// <summary>Gets or sets the footer icon</summary>
        internal IconUnion FooterIcon;

        /// <summary>
        ///   Gets or sets a pointer to the string to be used in the footer area of the task dialog. This parameter can
        ///   be either a <c>null</c>-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE
        ///   macro. If the EnableHyperlinks flag is specified for the flags member, then this string may contain
        ///   hyperlinks in this form.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string FooterText;

        /// <summary>Gets or sets the application-defined callback function.</summary>
        internal NativeMethods.TaskDialogCallback Callback;

        /// <summary>Gets or sets a pointer to application-defined reference data. This value is defined by the caller.</summary>
        internal IntPtr CallbackData;

        /// <summary>
        ///   Gets or sets the width of the task dialog's client area. If 0, the task dialog manager will calculate the
        ///   ideal width.
        /// </summary>
        internal uint Width;
    }
}