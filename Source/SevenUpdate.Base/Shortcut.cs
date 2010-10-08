// ***********************************************************************
// <copyright file="Shortcut.cs"
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>
    /// The action to preform on the shortcut
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Add)]
    public enum ShortcutAction
    {
        /// <summary>
        ///   Adds a shortcut
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Add = 0, 

        /// <summary>
        ///   Updates a shortcut only if it exists
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Update = 1, 

        /// <summary>
        ///   Deletes a shortcut
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Delete = 2
    }

    /// <summary>
    /// A shortcut to be created within an update
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(ShortcutAction))]
    public sealed class Shortcut : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The action to perform on the <see cref = "Shortcut" />
        /// </summary>
        private ShortcutAction action;

        /// <summary>
        ///   The command line arguments for the shortcut
        /// </summary>
        private string arguments;

        /// <summary>
        ///   The collection of localized shortcut descriptions
        /// </summary>
        private ObservableCollection<LocaleString> description;

        /// <summary>
        ///   The icon resource for the shortcut
        /// </summary>
        private string icon;

        /// <summary>
        ///   The physical location of the shortcut lnk file
        /// </summary>
        private string location;

        /// <summary>
        ///   The collection of localized shortcut names
        /// </summary>
        private ObservableCollection<LocaleString> name;

        /// <summary>
        ///   The file or folder that is executed by the shortcut
        /// </summary>
        private string target;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the action to perform on the <see cref = "Shortcut" />
        /// </summary>
        /// <value>The action.</value>
        [ProtoMember(3)]
        [DataMember]
        public ShortcutAction Action
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
        ///   Gets or sets the command line arguments for the shortcut
        /// </summary>
        /// <value>The arguments of the shortcut</value>
        [ProtoMember(4, IsRequired = false)]
        [DataMember]
        public string Arguments
        {
            get
            {
                return this.arguments;
            }

            set
            {
                this.arguments = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Arguments");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of localized shortcut descriptions
        /// </summary>
        /// <value>The localized descriptions for the shortcut</value>
        [ProtoMember(5, IsRequired = false)]
        [DataMember]
        public ObservableCollection<LocaleString> Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Description");
            }
        }

        /// <summary>
        ///   Gets or sets the icon resource for the shortcut
        /// </summary>
        /// <value>The icon for the shortcut</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string Icon
        {
            get
            {
                return this.icon;
            }

            set
            {
                this.icon = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Icon");
            }
        }

        /// <summary>
        ///   Gets or sets the physical location of the shortcut lnk file
        /// </summary>
        /// <value>The shortcut location</value>
        [ProtoMember(2)]
        [DataMember]
        public string Location
        {
            get
            {
                return this.location;
            }

            set
            {
                this.location = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Location");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of localized shortcut names
        /// </summary>
        /// <value>The localized names for the shortcut</value>
        [ProtoMember(1)]
        [DataMember]
        public ObservableCollection<LocaleString> Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        ///   Gets or sets the file or folder that is executed by the shortcut
        /// </summary>
        /// <value>The target for the shortcut</value>
        [ProtoMember(7, IsRequired = false)]
        [DataMember]
        public string Target
        {
            get
            {
                return this.target;
            }

            set
            {
                this.target = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Target");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed
        /// </param>
        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}