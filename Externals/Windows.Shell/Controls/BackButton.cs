#region GNU Public License Version 3

// Copyright 2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Implements BackButton that can be used in WPF user interfaces.
    /// </summary>
    public class BackButton : Button
    {
        #region Constructors

        static BackButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackButton), new FrameworkPropertyMetadata(typeof(BackButton)));
        }

        public BackButton()
        {
            if (Resources.Count != 0)
                return;
            var resourceDictionary = new ResourceDictionary {Source = new Uri("/Windows.Shell;component/Resources/Dictionary.xaml", UriKind.Relative)};
            Resources.MergedDictionaries.Add(resourceDictionary);
            Command = NavigationCommands.BrowseBack;
        }

        #endregion
        }
}