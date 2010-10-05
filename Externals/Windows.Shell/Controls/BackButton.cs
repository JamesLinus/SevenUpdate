// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace Microsoft.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
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

            var resourceDictionary = new ResourceDictionary { Source = new Uri("/Windows.Shell;component/Resources/Dictionary.xaml", UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(resourceDictionary);
            this.Command = NavigationCommands.BrowseBack;
        }

        #endregion
    }
}