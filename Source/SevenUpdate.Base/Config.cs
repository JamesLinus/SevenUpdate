// <copyright file="Config.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Configuration options.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    public sealed class Config : INotifyPropertyChanged
    {
        /// <summary>The automatic update setting.</summary>
        private AutoUpdateOption autoOption;

        /// <summary>
        ///   A value that indicates whether to treat <c>Importance.Recommended</c> updates the same as
        ///   <c>Importance.Important</c> updates.
        /// </summary>
        private bool includeRecommended;

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets which automatic update option Seven Update should use.</summary>
        /// <value>The automatic update option.</value>
        [ProtoMember(1)]
        [DataMember]
        public AutoUpdateOption AutoOption
        {
            get { return this.autoOption; }

            set
            {
                this.autoOption = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AutoOption");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Seven Update is to included recommended updates when automatically
        ///   downloading updates.
        /// </summary>
        /// <value><c>True</c> if recommended updates should be treated as important updates otherwise, <c>false</c>.</value>
        [ProtoMember(2)]
        [DataMember]
        public bool IncludeRecommended
        {
            get { return this.includeRecommended; }

            set
            {
                this.includeRecommended = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("IncludeRecommended");
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