//Copyright (c) Microsoft Corporation.  All rights reserved.

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
        ///   Specifies the main instruction text
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        ///   Specifies the supporting note text
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        ///   Indicates if the button is in a checked state
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