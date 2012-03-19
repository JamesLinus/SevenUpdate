// <copyright file="TaskDialogOptions.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;

    /// <summary>Specifies the behavior of the task dialog.</summary>
    [Flags]
    internal enum TaskDialogOptions
    {
        /// <summary>Empty dialog.</summary>
        None = 0, 

        /// <summary>
        ///   Enables hyperlink processing for the strings specified in the pszContent, pszExpandedInformation and
        ///   pszFooter members.
        /// </summary>
        EnableHyperlinks = 0x0001, 

        /// <summary>
        ///   Indicates that the dialog should use the icon referenced by the handle in the MainIcon member as the
        ///   primary icon in the task dialog. If this flag is specified, the MainIcon member is ignored.
        /// </summary>
        UseMainIcon = 0x0002, 

        /// <summary>
        ///   Indicates that the dialog should use the icon referenced by the handle in the FooterIcon member as the
        ///   footer icon in the task dialog. If this flag is specified, the FooterIcon member is ignored.
        /// </summary>
        UseFooterIcon = 0x0004, 

        /// <summary>
        ///   Indicates that the dialog should be able to be closed using Alt-F4, Escape, and the title bar's close
        ///   button even if no cancel button is specified in either the CommonButtons or Buttons members.
        /// </summary>
        AllowCancel = 0x0008, 

        /// <summary>
        ///   Indicates that the buttons specified in the Buttons member are to be displayed as command links (using a
        ///   standard task dialog glyph) instead of push buttons. When using command links, all characters up to the
        ///   first new line character in the pszButtonText member will be treated as the command link's main text, and
        ///   the remainder will be treated as the command link's note. This flag is ignored if the Buttons member is
        ///   zero.
        /// </summary>
        UseCommandLinks = 0x0010, 

        /// <summary>
        ///   Indicates that the buttons specified in the Buttons member are to be displayed as command links (without a
        ///   glyph) instead of push buttons. When using command links, all characters up to the first new line
        ///   character in the ButtonText member will be treated as the command link's main text, and the remainder will
        ///   be treated as the command link's note. This flag is ignored if the Buttons member is zero.
        /// </summary>
        UseNoIconCommandLinks = 0x0020, 

        /// <summary>
        ///   Indicates that the string specified by the ExpandedInformation member is displayed at the bottom of the
        ///   dialog's footer area instead of immediately after the dialog's content. This flag is ignored if the
        ///   ExpandedInformation member is null.
        /// </summary>
        ExpandFooterArea = 0x0040, 

        /// <summary>
        ///   Indicates that the string specified by the ExpandedInformation member is displayed when the dialog is
        ///   initially displayed. This flag is ignored if the ExpandedInformation member is <c>null</c>.
        /// </summary>
        ExpandedByDefault = 0x0080, 

        /// <summary>
        ///   Indicates that the verification checkbox in the dialog is checked when the dialog is initially displayed.
        ///   This flag is ignored if the VerificationText parameter is null.
        /// </summary>
        CheckVerificationFlag = 0x0100, 

        /// <summary>Indicates that a Progress Bar is to be displayed.</summary>
        ShowProgressBar = 0x0200, 

        /// <summary>Indicates that an Marquee Progress Bar is to be displayed.</summary>
        ShowMarqueeProgressBar = 0x0400, 

        /// <summary>Indicates that the task dialog's callback is to be called approximately every 200 milliseconds.</summary>
        UseCallbackTimer = 0x0800, 

        /// <summary>
        ///   Indicates that the task dialog is positioned (centered) relative to the window specified by Parent. If the
        ///   flag is not supplied (or no Parent member is specified), the task dialog is positioned (centered) relative
        ///   to the monitor.
        /// </summary>
        PositionRelativeToWindow = 0x1000, 

        /// <summary>Indicates that text is displayed reading right to left.</summary>
        RightToLeftLayout = 0x2000, 

        /// <summary>Indicates that no default item will be selected.</summary>
        NoDefaultRadioButton = 0x4000
    }
}