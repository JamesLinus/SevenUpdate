// <copyright file="CommandLink.cs" project="SevenSoftware.Windows">Thomas Levesque</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License" />

namespace SevenSoftware.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>Implements a CommandLink button that can be used in WPF user interfaces.</summary>
    public class CommandLink : Button, INotifyPropertyChanged
    {
        /// <summary>The text to display below the main instruction text.</summary>
        static readonly DependencyProperty NoteProperty = DependencyProperty.Register(
            "Note", 
            typeof(string), 
            typeof(CommandLink), 
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, OnNoteChanged));

        /// <summary>Indicates if the Uac shield is needed.</summary>
        static readonly DependencyProperty UseShieldProperty = DependencyProperty.Register(
            "UseShield", 
            typeof(bool), 
            typeof(CommandLink), 
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnUseShieldChanged));

        /// <summary>Initializes static members of the <see cref="CommandLink" /> class.</summary>
        static CommandLink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CommandLink), new FrameworkPropertyMetadata(typeof(CommandLink)));
        }

        /// <summary>Initializes a new instance of the <see cref="CommandLink" /> class.</summary>
        public CommandLink()
        {
            if (this.Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary
                {
                   Source = new Uri("/SevenSoftware.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) 
                };
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the supporting text to display on the <c>CommandLink</c> below the instruction text.</summary>
        public string Note
        {
            get { return (string)this.GetValue(NoteProperty); }

            set { this.SetValue(NoteProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether a Uac shield is needed.</summary>
        /// <value><c>True</c> if [use shield]; otherwise, <c>False</c>.</value>
        public bool UseShield
        {
            get { return (bool)this.GetValue(UseShieldProperty); }

            set { this.SetValue(UseShieldProperty, value); }
        }

        /// <summary>Handles a change to the <c>Note</c> property.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The <c>System.Windows.DependencyPropertyChangedEventArgs</c> instance containing the event data.</param>
        static void OnNoteChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (CommandLink)obj;
            me.Note = e.NewValue.ToString();
            me.OnPropertyChanged("Note");
        }

        /// <summary>Handles a change to the <c>UseShield</c> property.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The <c>System.Windows.DependencyPropertyChangedEventArgs</c> instance containing the event data.</param>
        static void OnUseShieldChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (CommandLink)obj;
            me.UseShield = (bool)e.NewValue;
            me.OnPropertyChanged("UseShield");
        }

        /// <summary>When a property has changed, call the <c>OnPropertyChanged</c> Event.</summary>
        /// <param name="name">The name of the property that has changed.</param>
        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}