// <copyright file="LocaleString.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Contains a string indicating the language and a value.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    public sealed class LocaleString : INotifyPropertyChanged
    {
        /// <summary>The ISO language code.</summary>
        private string lang;

        /// <summary>The value of the string.</summary>
        private string value;

        /// <summary>Initializes a new instance of the <see cref="LocaleString" /> class.</summary>
        public LocaleString()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocaleString" /> class.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="lang">The an ISO language code for the value.</param>
        public LocaleString(string value, string lang)
        {
            this.Lang = lang;
            this.Value = value;
        }

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets an ISO language code.</summary>
        /// <value>The iso code.</value>
        [ProtoMember(1)]
        [DataMember]
        public string Lang
        {
            get { return this.lang; }

            set
            {
                this.lang = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Lang");
            }
        }

        /// <summary>Gets or sets the value of the string.</summary>
        /// <value>The value.</value>
        [ProtoMember(2)]
        [DataMember]
        public string Value
        {
            get { return this.value; }

            set
            {
                this.value = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Value");
            }
        }

        /// <summary>When a property has changed, call the <c>OnPropertyChanged</c> Event.</summary>
        /// <param name="name">The name of the property that changed.</param>
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}