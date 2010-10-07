// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// Implements a CommandLink button that can be used in WPF user interfaces.
    /// </summary>
    public sealed class CommandLink : Button, INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The text to display below the main instruction text
        /// </summary>
        public static readonly DependencyProperty NoteProperty = DependencyProperty.Register(
            "Note", typeof(string), typeof(CommandLink), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, OnNoteChanged));

        /// <summary>
        ///   Indicates if the Uac shield is needed
        /// </summary>
        public static readonly DependencyProperty UseShieldProperty = DependencyProperty.Register(
            "UseShield", typeof(bool), typeof(CommandLink), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnUseShieldChanged));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "CommandLink" /> class.
        /// </summary>
        static CommandLink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandLink), new FrameworkPropertyMetadata(typeof(CommandLink)));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "CommandLink" /> class.
        /// </summary>
        public CommandLink()
        {
            if (this.Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary { Source = new Uri("/System.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether the application is running on XP
        /// </summary>
        public static bool RunningOnXP
        {
            get
            {
                return Environment.OSVersion.Version.Major < 6;
            }
        }

        /// <summary>
        ///   Gets or sets the supporting text to display on the <see cref = "CommandLink" /> below the instruction text
        /// </summary>
        public string Note
        {
            get
            {
                return (string)this.GetValue(NoteProperty);
            }

            set
            {
                this.SetValue(NoteProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether a Uac shield is needed.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if [use shield]; otherwise, <see langword = "false" />.
        /// </value>
        public bool UseShield
        {
            get
            {
                return (bool)this.GetValue(UseShieldProperty);
            }

            set
            {
                this.SetValue(UseShieldProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles a change to the <see cref="Note"/> property
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </parameter>
        private static void OnNoteChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (CommandLink)obj;
            me.Note = e.NewValue.ToString();
            me.OnPropertyChanged("Note");
        }

        /// <summary>
        /// Handles a change to the <see cref="UseShield"/> property
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </parameter>
        private static void OnUseShieldChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (CommandLink)obj;
            me.UseShield = (bool)e.NewValue;
            me.OnPropertyChanged("UseShield");
        }

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <parameter name="name">
        /// The name of the property that has changed
        /// </parameter>
        private void OnPropertyChanged(string name)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}