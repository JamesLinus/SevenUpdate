using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SevenUpdate.Sdk
{
    internal class Base
    {
        /// <summary>
        ///   Underlines the text when mouse is over the <see cref = "TextBlock" />
        /// </summary>
        internal static void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = TextDecorations.Underline;
        }

        /// <summary>
        ///   Removes the Underlined text when mouse is leaves the <see cref = "TextBlock" />
        /// </summary>
        internal static void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = null;
        }
    }
}
