// <copyright file="TaskDialogConfig.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using Controls;

    using Runtime.InteropServices;

    /// <summary>Contains information used to display a task dialog.</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal class TaskDialogConfig : IDisposable
    {
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

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for the dialog's primary content. This
        ///   parameter can be either a <c>null</c>-terminated string or an integer resource identifier passed to the
        ///   MAKEINTRESOURCE macro. If the EnableHyperlinks flag is specified for the flags member, then this string
        ///   may contain hyperlinks in the form: <a
        ///    href = "executablestring">Hyperlink Text</a>.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Content;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for displaying additional information. This
        ///   parameter can be either a <c>null</c>-terminated string or an integer resource identifier passed to the
        ///   MAKEINTRESOURCE macro. The additional information is displayed either immediately below the content or
        ///   below the footer text depending on whether the ExpanderFooterArea flag is specified. If the
        ///   EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in the
        ///   form: <a
        ///    href = "executablestring">Hyperlink Text</a>.
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
        ///   Gets or sets a pointer to the string to be used in the footer area of the task dialog. This parameter can
        ///   be either a <c>null</c>-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE
        ///   macro. If the EnableHyperlinks flag is specified for the flags member, then this string may contain
        ///   hyperlinks in this form.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Footer;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for the main instruction. This parameter can
        ///   be either a <c>null</c>-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE
        ///   macro.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string MainInstruction;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used to label the verification <c>CheckBox</c>.
        ///   This parameter can be either a <c>null</c>-terminated string or an integer resource identifier passed to
        ///   the MAKEINTRESOURCE macro. If this parameter is <c>null</c>, the verification <see
        ///    cref="CheckBox" /> is not displayed in the task dialog. If the VerificationFlagChecked parameter of
        ///    TaskDialogIndirect is <c>null</c>, the <c>CheckBox</c> is not enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string VerificationText;

        /// <summary>
        ///   Gets or sets a pointer that references the string to be used for the task dialog title. This parameter can
        ///   be either a <c>null</c>-terminated string or an integer resource identifier passed to the MakeIntResource
        ///   macro. If this parameter is <c>null</c>, the filename of the executable program is used.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string WindowTitle;

        #region Destructor

        /// <summary>Finalizes an instance of the <c>TaskDialogConfig</c> class.</summary>
        ~TaskDialogConfig()
        {
            this.Dispose(false);
        }

        #endregion

        /// <summary>
        ///   Gets or sets a the push buttons displayed in the task dialog. If no common buttons are specified and no
        ///   custom buttons are specified using the buttons and pButtons members, the task dialog will contain the OK
        ///   button by default.
        /// </summary>
        internal TaskDialogCommonButtonFlags CommonButtons { get; set; }

        /// <summary>Gets or sets a handle to an Icon that is to be displayed in the task dialog.</summary>
        internal TaskDialogConfigIconUnion MainIcon { get; set; }

        /// <summary>Gets or sets a handle to an Icon that is to be displayed in the footer of the task dialog.</summary>
        internal TaskDialogConfigIconUnion FooterIcon { get; set; }

        /// <summary>Gets or sets the structure size, in bytes.</summary>
        internal uint Size { get; set; }

        /// <summary>Gets or sets the handle to the parent window. This member can be <c>null</c>.</summary>
        internal IntPtr HandleParent { get; set; }

        /// <summary>
        ///   Gets or sets the handle to the module that contains the icon resource identified by the mainIcon or
        ///   footerIcon members, and the string resources identified by the windowTitle, mainInstruction, content,
        ///   verificationText, expandedInformation, expandedControlText, collapsedControlText or footer members.
        /// </summary>
        internal IntPtr Instance { get; set; }

        /// <summary>Gets or sets the behavior of the task dialog.</summary>
        internal TaskDialogFlags Flags { get; set; }

        /// <summary>
        ///   Gets or sets the number of entries in the buttons array that is used to create buttons or command links in
        ///   the dialog. If this member is zero and no common buttons have been specified using the
        ///   <c>CommonButtons</c> member, then the task dialog will have a single OK button displayed.
        /// </summary>
        internal uint ButtonLength { get; set; }

        /// <summary>
        ///   Gets or sets a pointer to an array of <c>TaskDialogButton</c> structures containing the definition of the
        ///   custom buttons that are to be displayed in the dialog. This array must contain at least the number of
        ///   entries that are specified by the buttons member.
        /// </summary>
        internal IntPtr ButtonCollection { get; set; }

        /// <summary>
        ///   Gets or sets the default button for the dialog. This may be any of the values specified in ButtonID
        ///   members of one of the <c>TaskDialogButton</c> structures in the <c>ButtonCollection</c>, or one of the IDs
        ///   corresponding to the buttons specified in the <c>CommonButtons</c> member.
        /// </summary>
        internal int DefaultButton { get; set; }

        /// <summary>
        ///   Gets or sets the number of entries in the <c>RadioButtonCollection</c> that is used to create radio
        ///   buttons in the dialog.
        /// </summary>
        internal uint RadioButtonsLength { get; set; }

        /// <summary>
        ///   Gets or sets a pointer to an array of <c>TaskDialogButton</c> structures containing the definition of the
        ///   radio buttons that are to be displayed in the dialog. This array must contain at least the number of
        ///   entries that are specified by the cRadioButtons member. This parameter can be <c>null</c>.
        /// </summary>
        internal IntPtr RadioButtonCollection { get; set; }

        /// <summary>
        ///   Gets or sets the button ID of the radio button that is selected by default. If this value does not
        ///   correspond to a button ID, the first button in the array is selected by default.
        /// </summary>
        internal int DefaultRadioButton { get; set; }

        /// <summary>Gets or sets the application-defined callback function.</summary>
        internal TaskDialogNativeMethods.TaskDialogCallBack Callback { get; set; }

        /// <summary>Gets or sets a pointer to application-defined reference data. This value is defined by the caller.</summary>
        internal IntPtr CallbackData { get; set; }

        /// <summary>
        ///   Gets or sets the width of the task dialog's client area. If 0, the task dialog manager will calculate the
        ///   ideal width.
        /// </summary>
        internal uint Width { get; set; }

        /// <summary>Gets or sets a value indicating whether if the object is disposed</summary>
        protected bool Disposed { get; set; }

        #region IDisposable Implementation

        /// <summary>Disposes the objects</summary>
        public virtual void Dispose()
        {
            this.Dispose(false);

            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes the objects</summary>
        /// <param name="disposing">  true if the object is already disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                // Do nothing if the object has already been disposed of.
                if (this.Disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // Release disposable objects used by this instance here.
                }

                // Release unmanaged resources here. Don't access reference type fields.

                // Remember that the object has been disposed of.
                this.Disposed = true;
            }
        }

        #endregion
    }
}