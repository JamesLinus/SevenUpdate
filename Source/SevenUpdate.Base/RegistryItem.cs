// <copyright file="RegistryItem.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using Microsoft.Win32;

    using ProtoBuf;

    /// <summary>A registry entry within an update.</summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(RegistryAction))]
    [KnownType(typeof(RegistryHive))]
    [KnownType(typeof(RegistryValueKind))]
    public sealed class RegistryItem : INotifyPropertyChanged
    {
        /// <summary>The action to perform on the <c>RegistryItem</c>.</summary>
        private RegistryAction action;

        /// <summary>The data for the key value.</summary>
        private string data;

        /// <summary>The registry key and hive.</summary>
        private string key;

        /// <summary>The value for the registry key.</summary>
        private string keyValue;

        /// <summary>The type of the value.</summary>
        private RegistryValueKind valueKind;

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the action to perform on the <c>RegistryItem</c>.</summary>
        /// <value>The action.</value>
        [ProtoMember(1)]
        [DataMember]
        public RegistryAction Action
        {
            get { return this.action; }

            set
            {
                this.action = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Action");
            }
        }

        /// <summary>Gets or sets the data for the key value.</summary>
        /// <value>The data for the registry value.</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string Data
        {
            get { return this.data; }

            set
            {
                this.data = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Data");
            }
        }

        /// <summary>Gets or sets the registry key and hive.</summary>
        /// <value>The registry key path.</value>
        [ProtoMember(3)]
        [DataMember]
        public string Key
        {
            get { return this.key; }

            set
            {
                this.key = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Key");
            }
        }

        /// <summary>Gets or sets the value for the registry key.</summary>
        /// <value>The value of the key.</value>
        [ProtoMember(4, IsRequired = false)]
        [DataMember]
        public string KeyValue
        {
            get { return this.keyValue; }

            set
            {
                this.keyValue = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("KeyValue");
            }
        }

        /// <summary>Gets or sets the type of the value.</summary>
        /// <value>The kind of the value.</value>
        [ProtoMember(5, IsRequired = false)]
        [DataMember]
        public RegistryValueKind ValueKind
        {
            get { return this.valueKind; }

            set
            {
                this.valueKind = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ValueKind");
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