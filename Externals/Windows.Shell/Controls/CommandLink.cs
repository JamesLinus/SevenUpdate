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
    ///   Implements a CommandLink button that can be used in WPF user interfaces.
    /// </summary>
    public sealed class CommandLink : Button, INotifyPropertyChanged
    {
        #region Fields

        public static readonly DependencyProperty UseShieldProperty = DependencyProperty.Register("UseShield", typeof (bool), typeof (CommandLink),
                                                                                                  new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                                                                OnUseShieldChanged));

        public static readonly DependencyProperty NoteProperty = DependencyProperty.Register("Note", typeof (string), typeof (CommandLink),
                                                                                             new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, OnNoteChanged));
        #endregion

        #region Constructors

        static CommandLink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandLink), new FrameworkPropertyMetadata(typeof(CommandLink)));
        }

        public CommandLink()
        {
            if (Resources.Count != 0)
                return;
            var resourceDictionary = new ResourceDictionary { Source = new Uri("/Windows.Shell;component/Resources/Dictionary.xaml", UriKind.Relative) };
            Resources.MergedDictionaries.Add(resourceDictionary);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the supporting text to display on the commandlink below the instruction text
        /// </summary>
        public string Note { get { return ((string) GetValue(NoteProperty)); } set { SetValue(NoteProperty, value); } }

        public bool UseShield { get { return ((bool) GetValue(UseShieldProperty)); } set { SetValue(UseShieldProperty, value); } }

        #endregion

        #region Methods - Dependency Property Callbacks

        /// <summary>
        ///   Handles a change to the <see cref = "UseShield" /> property
        /// </summary>
        /// <param name = "obj" />
        /// <param name = "e" />
        private static void OnUseShieldChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (CommandLink) obj;
            me.UseShield = (bool) e.NewValue;
            me.OnPropertyChanged("UseShield");
        }

        /// <summary>
        ///   Handles a change to the <see cref = "Note" /> property
        /// </summary>
        /// <param name = "obj" />
        /// <param name = "e" />
        private static void OnNoteChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (CommandLink) obj;
            me.Note = e.NewValue.ToString();
            me.OnPropertyChanged("Note");
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}