//Copyright (c) Microsoft Corporation.  All rights reserved.

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
    public partial class CommandLink : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommandLink()
        {
            DataContext = this;
            InitializeComponent();
            button.Click += Button_Click;
        }

        /// <summary>
        ///   Routed UI command to use for this button
        /// </summary>
        public RoutedUICommand Command { get; set; }

        /// <summary>
        ///   Gets or sets the main instruction text
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        ///  Gets or sets the supporting note text
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        ///   Gets or sets if the button is in a checked state
        /// </summary>
        public bool? IsCheck { get { return button.IsChecked; } set { button.IsChecked = value; } }

        public bool UseShield { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            e.Source = this;
            if (Click != null)
                Click(sender, e);
        }

        /// <summary>
        ///   Occurs when the control is clicked.
        /// </summary>
        public event RoutedEventHandler Click;

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}