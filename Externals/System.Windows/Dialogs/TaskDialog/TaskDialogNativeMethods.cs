// ***********************************************************************
// <copyright file="TaskDialogNativeMethods.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using Controls;

    using Internal;

    using Runtime.InteropServices;

    /// <summary>
    ///   Internal class containing most native interop declarations used throughout the library.Functions that are not
    ///   performance intensive belong in this class.
    /// </summary>
    internal static class TaskDialogNativeMethods
    {
        /// <summary>
        ///   The <c>TaskDialog</c> callback.
        /// </summary>
        /// <param name="handle">
        ///   The handle for the dialog.
        /// </param>
        /// <param name="msg">
        ///   The message id.
        /// </param>
        /// <param name="parameter">
        ///   The parameter.
        /// </param>
        /// <param name="parameterLength">
        ///   The length of the parameter.
        /// </param>
        /// <returns>
        ///   The result of the <c>TaskDialog</c>.
        /// </returns>
        internal delegate int TaskDialogCallBack(IntPtr handle, uint msg, IntPtr parameter, IntPtr parameterLength);

        /// <summary>
        ///   The TaskDialogIndirect function creates, displays, and operates a task dialog. The task dialog contains
        ///   application-defined icons, messages, title, verification check box, command links, push buttons, and radio
        ///   buttons. This function can register a callback function to receive notification messages.
        /// </summary>
        /// <param name="taskConfig">
        ///   A pointer to a <c>TaskDialogConfig</c> structure that contains information used to display the task
        ///   dialog.
        /// </param>
        /// <param name="button">
        ///   Address of a variable that receives one of the button IDs specified in the <paramref name="button" />
        ///   member of the <paramref
        ///    name="taskConfig" /> parameter. If this parameter is <c>null</c>, no value is returned.
        /// </param>
        /// <param name="radioButton">
        ///   Address of a variable that receives one of the button IDs specified in the <paramref name="radioButton" />
        ///   member of the <paramref
        ///    name="taskConfig" /> parameter. If this parameter is <c>null</c>, no value is returned.
        /// </param>
        /// <param name="verificationFlagChecked">
        ///   <c>True</c> if the verification <c>CheckBox</c> was checked when the dialog was dismissed; otherwise,
        ///   <c>False</c>.
        /// </param>
        /// <returns>
        ///   The result.
        /// </returns>
        [DllImport(@"comctl32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern Result TaskDialogIndirect(
            [In] TaskDialogConfig taskConfig,
            [Out] out int button,
            [Out] out int radioButton,
            [MarshalAs(UnmanagedType.Bool)] [Out] out bool verificationFlagChecked);
    }
}