// ***********************************************************************
// <copyright file="TaskDialogNotifications.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    /// <summary>The notification ids.</summary>
    internal enum TaskDialogNotifications
    {
        /// <summary>
        ///   Sent by a task dialog after the dialog has been created and before it is displayed. This notification code
        ///   is received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        Created = 0, 

        /// <summary>
        ///   Sent by a task dialog when a navigation has occurred. This notification code is received only through the
        ///   task dialog callback function, which can be registered using the TaskDialogIndirect method.
        /// </summary>
        Navigated = 1, 

        /// <summary>
        ///   Sent by a task dialog when the user selects a button or command link in the task dialog. This notification
        ///   code is received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        ButtonClicked = 2, 

        /// <summary>
        ///   Sent by a task dialog when the user clicks a hyperlink in the task dialog content. This notification code
        ///   is received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        HyperlinkClicked = 3, 

        /// <summary>
        ///   Sent by a task dialog approximately every 200 milliseconds. This notification code is sent when the
        ///   CallbackTimer flag has been set in the flags member of the TaskDialog structure that was passed to the
        ///   TaskDialogIndirect function. This notification code is received only through the task dialog callback
        ///   function, which can be registered using the TaskDialogIndirect method.
        /// </summary>
        Timer = 4, 

        /// <summary>
        ///   Sent by a task dialog when it is destroyed and its window handle is no longer valid. This notification
        ///   code is received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        Destroyed = 5, 

        /// <summary>
        ///   Sent by a task dialog when the user selects a button or command link in the task dialog. This notification
        ///   code is received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        RadioButtonClicked = 6, 

        /// <summary>
        ///   Sent by a task dialog after the dialog has been created and before it is displayed. This notification code
        ///   is received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        DialogConstructed = 7, 

        /// <summary>
        ///   Sent by the task dialog when the user clicks the task dialog verification check box. This notification
        ///   code is received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        VerificationClicked = 8, 

        /// <summary>
        ///   Sent by a task dialog when the user presses F1 on the keyboard while the dialog has focus. This
        ///   notification code is received only through the task dialog callback function, which can be registered
        ///   using the TaskDialogIndirect method.
        /// </summary>
        Help = 9, 

        /// <summary>
        ///   Sent by a task dialog when the user clicks on the dialog's expanded button. This notification code is
        ///   received only through the task dialog callback function, which can be registered using the
        ///   TaskDialogIndirect method.
        /// </summary>
        ExpandButtonClicked = 10
    }
}
