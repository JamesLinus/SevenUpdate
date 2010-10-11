// ***********************************************************************
// <copyright file="BackButton.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
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

            var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("/System.Windows;component/Resources/Dictionary.xaml", UriKind.Relative)
                };
            this.Resources.MergedDictionaries.Add(resourceDictionary);
            this.Command = NavigationCommands.BrowseBack;
        }

        #endregion
    }
}