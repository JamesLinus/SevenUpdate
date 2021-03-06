// <copyright file="TaskDialogButtonData.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System.Runtime.InteropServices;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Contains the data for a <c>TaskDialogButton</c>.</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal struct TaskDialogButtonData
    {
        /// <summary>Gets or sets the id for the button.</summary>
        internal int ButtonId;

        /// <summary>Gets or sets the button text.</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string ButtonText;

        /// <summary>Initializes a new instance of the <see cref="TaskDialogButtonData" /> struct.</summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="text">The text.</param>
        public TaskDialogButtonData(int buttonId, string text)
        {
            ButtonId = buttonId;
            ButtonText = text;
        }
    }
}