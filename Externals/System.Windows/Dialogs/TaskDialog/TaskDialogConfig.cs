// <copyright file="TaskDialogConfig.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using System.Runtime.InteropServices;
    using System.Windows.Controls;

    /// <summary>Contains information used to display a task dialog.</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal class TaskDialogConfig : IDisposable
    {
        /// <summary>Specifies the push buttons displayed in the task dialog. If no common buttons are specified and no custom buttons are specified using the buttons and pButtons members, the task dialog will contain the OK button by default.</summary>
        internal TaskDialogCommonButtonFlags CommonButtons;

        /// <summary>Pointer that references the string to be used for the task dialog title. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MakeIntResource macro. If this parameter is <see langword="null" />, the filename of the executable program is used.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string WindowTitle;

        /// <summary>A handle to an Icon that is to be displayed in the task dialog.</summary>
        internal TaskDialogConfigIconUnion MainIcon; // NOTE: 32-bit union field, holds MainIcon as well

        /// <summary>A pointer that references the string to be used for the main instruction. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string MainInstruction;

        /// <summary>Pointer that references the string to be used for the dialog's primary content. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. If the EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in the form: <a href="executablestring">Hyperlink Text</a>.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Content;

        /// <summary>Pointer that references the string to be used to label the verification <see cref="CheckBox" />. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. If this parameter is <see langword="null" />, the verification <see cref="CheckBox" /> is not displayed in the task dialog. If the VerificationFlagChecked parameter of TaskDialogIndirect is <see langword="null" />, the <see cref="CheckBox" /> is not enabled.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string VerificationText;

        /// <summary>Pointer that references the string to be used for displaying additional information. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. The additional information is displayed either immediately below the content or below the footer text depending on whether the ExpanderFooterArea flag is specified. If the EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in the form: <a href="executablestring">Hyperlink Text</a>.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string ExpandedInformation;

        /// <summary>Pointer that references the string to be used to label the button for collapsing the expandable information. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. This member is ignored when the <see cref="ExpandedInformation" /> member is <see langword="null" />. If this member is <see langword="null" /> and the <see cref="CollapsedControlText" /> is specified, then the <see cref="CollapsedControlText" /> value will be used for this member as well.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string ExpandedControlText;

        /// <summary>Pointer that references the string to be used to label the button for expanding the expandable information. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. This member is ignored when the <see cref="ExpandedInformation" /> member is <see langword="null" />. If this member is <see langword="null" /> and the CollapsedControlText is specified, then the CollapsedControlText value will be used for this member as well.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string CollapsedControlText;

        /// <summary>A handle to an Icon that is to be displayed in the footer of the task dialog.</summary>
        internal TaskDialogConfigIconUnion FooterIcon; // NOTE: 32-bit union field, holds FooterIcon as well

        /// <summary>Pointer to the string to be used in the footer area of the task dialog. This parameter can be either a <see langword="null" />-terminated string or an integer resource identifier passed to the MAKEINTRESOURCE macro. If the EnableHyperlinks flag is specified for the flags member, then this string may contain hyperlinks in this form.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Footer;

        /// <summary>The application-defined callback function.</summary>
        internal TaskDialogNativeMethods.TaskDialogCallBack Callback;

        /// <summary>A pointer to application-defined reference data. This value is defined by the caller.</summary>
        internal IntPtr CallbackData;

        /// <summary>The width of the task dialog's client area. If 0, the task dialog manager will calculate the ideal width.</summary>
        internal uint Width;

        /// <summary>Indicates if the object is disposed</summary>
        protected bool disposed;

        #region Destructor

        /// <summary>Finalizes an instance of the TaskDialogConfig class.</summary>
        ~TaskDialogConfig()
        {
            this.Dispose(false);
        }

        #endregion

        /// <summary>Gets or sets the structure size, in bytes.</summary>
        internal uint Size { get; set; }

        /// <summary>Gets or sets the handle to the parent window. This member can be <see langword="null" />.</summary>
        internal IntPtr HandleParent { get; set; }

        /// <summary>Gets or sets the handle to the module that contains the icon resource identified by the mainIcon or footerIcon members, and the string resources identified by the windowTitle, mainInstruction, content, verificationText, expandedInformation, expandedControlText, collapsedControlText or footer members.</summary>
        internal IntPtr Instance { get; set; }

        /// <summary>Gets or sets the behavior of the task dialog.</summary>
        internal TaskDialogFlags Flags { get; set; }

        /// <summary>Gets or sets the number of entries in the buttons array that is used to create buttons or command links in the dialog. If this member is zero and no common buttons have been specified using the <see cref="CommonButtons" /> member, then the task dialog will have a single OK button displayed.</summary>
        internal uint ButtonLength { get; set; }

        /// <summary>Gets or sets a pointer to an array of <see cref="TaskDialogButton" /> structures containing the definition of the custom buttons that are to be displayed in the dialog. This array must contain at least the number of entries that are specified by the buttons member.</summary>
        internal IntPtr ButtonCollection { get; set; }

        /// <summary>Gets or sets the default button for the dialog. This may be any of the values specified in ButtonID members of one of the <see cref="TaskDialogButton" /> structures in the <see cref="ButtonCollection" />, or one of the IDs corresponding to the buttons specified in the <see cref="CommonButtons" /> member.</summary>
        internal int DefaultButton { get; set; }

        /// <summary>Gets or sets the number of entries in the <see cref="RadioButtonCollection" /> that is used to create radio buttons in the dialog.</summary>
        internal uint RadioButtonsLength { get; set; }

        /// <summary>Gets or sets a pointer to an array of <see cref="TaskDialogButton" /> structures containing the definition of the radio buttons that are to be displayed in the dialog. This array must contain at least the number of entries that are specified by the cRadioButtons member. This parameter can be <see langword="null" />.</summary>
        internal IntPtr RadioButtonCollection { get; set; }

        /// <summary>Gets or sets the button ID of the radio button that is selected by default. If this value does not correspond to a button ID, the first button in the array is selected by default.</summary>
        internal int DefaultRadioButton { get; set; }

        #region IDisposable Implementation

        /// <summary>Disposes the objects</summary>
        public virtual void Dispose()
        {
            this.Dispose(false);

            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes the objects</summary>
        /// <param name="disposing">true if the object is already disposing</param>
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
                    // Release disposable objects used by this instance here.
                }

                // Release unmanaged resources here. Don't access reference type fields.

                // Remember that the object has been disposed of.
                this.disposed = true;
            }
        }

        #endregion
    }
}