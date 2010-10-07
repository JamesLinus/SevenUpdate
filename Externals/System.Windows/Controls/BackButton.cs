// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.Windows.Input;

    /// <summary>
    /// Implements BackButton that can be used in WPF user interfaces.
    /// </summary>
    public sealed class BackButton : Button
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "BackButton" /> class.
        /// </summary>
        static BackButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackButton), new FrameworkPropertyMetadata(typeof(BackButton)));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BackButton" /> class.
        /// </summary>
        public BackButton()
        {
            if (this.Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary { Source = new Uri("/System.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(resourceDictionary);
            this.Command = NavigationCommands.BrowseBack;
        }

        #endregion
    }
}