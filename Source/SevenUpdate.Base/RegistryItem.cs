// ***********************************************************************
// <copyright file="RegistryItem.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using Microsoft.Win32;

    using ProtoBuf;

    /// <summary>
    /// Contains the Actions you can perform to the registry
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Add)]
    public enum RegistryAction
    {
        /// <summary>
        ///   Adds a registry entry to the machine
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Add = 0, 

        /// <summary>
        ///   Deletes a registry key on the machine
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        DeleteKey = 1, 

        /// <summary>
        ///   Deletes a value of a registry key on the machine
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        DeleteValue = 2
    }

    /// <summary>
    /// A registry entry within an update
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(RegistryAction))]
    [KnownType(typeof(RegistryHive))]
    [KnownType(typeof(RegistryValueKind))]
    public sealed class RegistryItem : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The action to perform on the <see cref = "RegistryItem" />
        /// </summary>
        private RegistryAction action;

        /// <summary>
        ///   The data for the key value
        /// </summary>
        private string data;

        /// <summary>
        ///   The registry key and hive
        /// </summary>
        private string key;

        /// <summary>
        ///   The value for the registry key
        /// </summary>
        private string keyValue;

        /// <summary>
        ///   The type of the value
        /// </summary>
        private RegistryValueKind valueKind;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the action to perform on the <see cref = "RegistryItem" />
        /// </summary>
        /// <value>The action.</value>
        [ProtoMember(1)]
        [DataMember]
        public RegistryAction Action
        {
            get
            {
                return this.action;
            }

            set
            {
                this.action = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Action");
            }
        }

        /// <summary>
        ///   Gets or sets the data for the key value
        /// </summary>
        /// <value>The data for the registry value</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string Data
        {
            get
            {
                return this.data;
            }

            set
            {
                this.data = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Data");
            }
        }

        /// <summary>
        ///   Gets or sets the registry key and hive
        /// </summary>
        /// <value>The registry key path</value>
        [ProtoMember(3)]
        [DataMember]
        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                this.key = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Key");
            }
        }

        /// <summary>
        ///   Gets or sets the value for the registry key
        /// </summary>
        /// <value>The value of the key</value>
        [ProtoMember(4, IsRequired = false)]
        [DataMember]
        public string KeyValue
        {
            get
            {
                return this.keyValue;
            }

            set
            {
                this.keyValue = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("KeyValue");
            }
        }

        /// <summary>
        ///   Gets or sets the type of the value
        /// </summary>
        /// <value>The kind of the value</value>
        [ProtoMember(5, IsRequired = false)]
        [DataMember]
        public RegistryValueKind ValueKind
        {
            get
            {
                return this.valueKind;
            }

            set
            {
                this.valueKind = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ValueKind");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="name">
        /// The name of the property that changed
        /// </param>
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