// <copyright file="BackButton.cs" project="SevenSoftware.Windows">Thomas Levesque</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License" />

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SevenSoftware.Windows.Controls
{
    /// <summary>Implements BackButton that can be used in WPF user interfaces.</summary>
    public sealed class BackButton : Button
    {
        /// <summary>Initializes static members of the <see cref="BackButton" /> class.</summary>
        static BackButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BackButton), new FrameworkPropertyMetadata(typeof(BackButton)));
        }

        /// <summary>Initializes a new instance of the <see cref="BackButton" /> class.</summary>
        public BackButton()
        {
            if (Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary
                {
                   Source = new Uri("/SevenSoftware.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) 
                };
            Resources.MergedDictionaries.Add(resourceDictionary);
            Command = NavigationCommands.BrowseBack;
        }
    }
}