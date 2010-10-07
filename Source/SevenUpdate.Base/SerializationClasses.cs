// ***********************************************************************
// Assembly         : SevenUpdate.Base
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    using Microsoft.Win32;

    using ProtoBuf;

    /// <summary>
    /// Automatic Update option Seven Update can use
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Install)]
    public enum AutoUpdateOption
    {
        /// <summary>
        ///   Download and Installs updates automatically
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Install = 0, 

        /// <summary>
        ///   Downloads Updates automatically
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Download = 1, 

        /// <summary>
        ///   Only checks and notifies the user of updates
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Notify = 2, 

        /// <summary>
        ///   No automatic checking
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Never = 3
    }

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
    /// The action to perform on the file
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Update)]
    public enum FileAction
    {
        /// <summary>
        ///   Updates a file
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Update = 0, 

        /// <summary>
        ///   Updates a file, only if it exist
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        UpdateIfExist = 1, 

        /// <summary>
        ///   Updates a file, then registers the dll
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        UpdateThenRegister = 2, 

        /// <summary>
        ///   Updates a file, then executes it
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        UpdateThenExecute = 3, 

        /// <summary>
        ///   Compares a file, but does not update it
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        CompareOnly = 4, 

        /// <summary>
        ///   Executes a file, can be on system or be downloaded
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Execute = 5, 

        /// <summary>
        ///   Deletes a file
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Delete = 6, 

        /// <summary>
        ///   Executes a file, then deletes it
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        ExecuteThenDelete = 7, 

        /// <summary>
        ///   Unregisters a dll, then deletes it
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        UnregisterThenDelete = 8, 
    }

    /// <summary>
    /// Contains the UpdateType of the update
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Important)]
    public enum Importance
    {
        /// <summary>
        ///   Important update
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Important = 0, 

        /// <summary>
        ///   Locale or language
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Locale = 1, 

        /// <summary>
        ///   Optional update
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Optional = 2, 

        /// <summary>
        ///   Recommended update
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Recommended = 3
    }

    /// <summary>
    /// The current status of the update
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Successful)]
    public enum UpdateStatus
    {
        /// <summary>
        ///   Indicates that the update installation failed
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Failed = 0, 

        /// <summary>
        ///   Indicates that the update is hidden
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Hidden = 1, 

        /// <summary>
        ///   Indicates that the update is visible
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Visible = 2, 

        /// <summary>
        ///   Indicates that the update installation succeeded
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Successful = 3
    }

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
    /// Configuration options
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    public sealed class Config : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The automatic update setting
        /// </summary>
        private AutoUpdateOption autoOption;

        /// <summary>
        ///   A value that indicates whether to treat <see cref = "Importance.Recommended" /> updates the same as <see cref = "Importance.Important" /> updates
        /// </summary>
        private bool includeRecommended;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets which automatic update option Seven Update should use
        /// </summary>
        /// <value>The automatic update option</value>
        [ProtoMember(1)]
        [DataMember]
        public AutoUpdateOption AutoOption
        {
            get
            {
                return this.autoOption;
            }

            set
            {
                this.autoOption = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AutoOption");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Seven Update is to included recommended updates when automatically downloading updates
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if recommended updates should be treated as important updates otherwise, <see langword = "false" />.
        /// </value>
        [ProtoMember(2)]
        [DataMember]
        public bool IncludeRecommended
        {
            get
            {
                return this.includeRecommended;
            }

            set
            {
                this.includeRecommended = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("IncludeRecommended");
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

    /// <summary>
    /// Contains a string indicating the language and a value
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Seven Update classes")]
    [ProtoContract]
    [DataContract]
    public sealed class LocaleString : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The ISO language code
        /// </summary>
        private string lang;

        /// <summary>
        ///   The value of the string
        /// </summary>
        private string value;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets an ISO language code
        /// </summary>
        /// <value>The iso code</value>
        [ProtoMember(1)]
        [DataMember]
        public string Lang
        {
            get
            {
                return this.lang;
            }

            set
            {
                this.lang = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Lang");
            }
        }

        /// <summary>
        ///   Gets or sets the value of the string
        /// </summary>
        /// <value>The value.</value>
        [ProtoMember(2)]
        [DataMember]
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Value");
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

    /// <summary>
    /// Seven Update Application information
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(ObservableCollection<LocaleString>))]
    public sealed class Sua : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The <see cref = "Uri" /> for the application's website
        /// </summary>
        private string appUrl;

        /// <summary>
        ///   The collection of localized descriptions for the application
        /// </summary>
        private ObservableCollection<LocaleString> description;

        /// <summary>
        ///   The directory where the application is installed
        /// </summary>
        private string directory;

        /// <summary>
        ///   The help website <see cref = "Uri" /> of the application.
        /// </summary>
        private string helpUrl;

        /// <summary>
        ///   Indicates whether if the application is 64 bit
        /// </summary>
        private bool is64Bit;

        /// <summary>
        ///   Indicates whether the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        private bool isEnabled;

        /// <summary>
        ///   A collection of localized application names
        /// </summary>
        private ObservableCollection<LocaleString> name;

        /// <summary>
        ///   A collection of localized publisher names
        /// </summary>
        private ObservableCollection<LocaleString> publisher;

        /// <summary>
        ///   The <see cref = "Uri" /> pointing to the sui file containing the application updates
        /// </summary>
        private string suiUrl;

        /// <summary>
        ///   The name of the value to the registry key that contains the application directory location
        /// </summary>
        private string valueName;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the <see cref = "Uri" /> for the application's website
        /// </summary>
        /// <value>The application website</value>
        [ProtoMember(8, IsRequired = false)]
        [DataMember]
        public string AppUrl
        {
            get
            {
                return this.appUrl;
            }

            set
            {
                this.appUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AppUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of localized descriptions for the application
        /// </summary>
        /// <value>The application description</value>
        [ProtoMember(2)]
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
        ///   Gets or sets the directory where the application is installed
        /// </summary>
        /// <value>The install directory</value>
        [ProtoMember(3)]
        [DataMember]
        public string Directory
        {
            get
            {
                return this.directory;
            }

            set
            {
                this.directory = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Directory");
            }
        }

        /// <summary>
        ///   Gets or sets the help website <see cref = "Uri" /> of the application
        /// </summary>
        /// <value>The help and support website for the application</value>
        [ProtoMember(9, IsRequired = false)]
        [DataMember]
        public string HelpUrl
        {
            get
            {
                return this.helpUrl;
            }

            set
            {
                this.helpUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("HelpUrl");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether if the application is 64 bit
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if the application is 64 bit; otherwise, <see langword = "false" />.
        /// </value>
        [ProtoMember(4)]
        [DataMember]
        public bool Is64Bit
        {
            get
            {
                return this.is64Bit;
            }

            set
            {
                this.is64Bit = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Is64Bit");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if this instance is enabled; otherwise, <see langword = "false" />.
        /// </value>
        [ProtoMember(5)]
        [DataMember]
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.isEnabled = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        ///   Gets or sets a collection of localized application names
        /// </summary>
        /// <value>The name of the application localized</value>
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
        ///   Gets or sets the collection of localized publisher names
        /// </summary>
        /// <value>The publisher.</value>
        [ProtoMember(6)]
        [DataMember]
        public ObservableCollection<LocaleString> Publisher
        {
            get
            {
                return this.publisher;
            }

            set
            {
                this.publisher = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Publisher");
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref = "Uri" /> pointing to the sui file containing the application updates
        /// </summary>
        /// <value>The url pointing to the sui file</value>
        [ProtoMember(7)]
        [DataMember]
        public string SuiUrl
        {
            get
            {
                return this.suiUrl;
            }

            set
            {
                this.suiUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("SuiUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the name of the value to the registry key that contains the application directory location
        /// </summary>
        /// <value>The name of the value.</value>
        [ProtoMember(10, IsRequired = false)]
        [DataMember]
        public string ValueName
        {
            get
            {
                return this.valueName;
            }

            set
            {
                this.valueName = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ValueName");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property.
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

    /// <summary>
    /// The collection of updates and the application info.
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Sua))]
    [KnownType(typeof(ObservableCollection<Update>))]
    public sealed class Sui : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The application information
        /// </summary>
        private Sua appInfo;

        /// <summary>
        ///   A collection of updates for the application
        /// </summary>
        private ObservableCollection<Update> updates;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the  software information for the application updates.
        /// </summary>
        [ProtoMember(2)]
        [DataMember]
        public Sua AppInfo
        {
            get
            {
                return this.appInfo;
            }

            set
            {
                this.appInfo = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AppInfo");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of updates for the application
        /// </summary>
        [ProtoMember(1)]
        [DataMember]
        public ObservableCollection<Update> Updates
        {
            get
            {
                return this.updates;
            }

            set
            {
                this.updates = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Updates");
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

    /// <summary>
    /// Information on how to install a software update
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(ObservableCollection<LocaleString>))]
    [KnownType(typeof(UpdateFile))]
    [KnownType(typeof(RegistryItem))]
    [KnownType(typeof(Shortcut))]
    public sealed class Update : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The collection localized update descriptions
        /// </summary>
        private ObservableCollection<LocaleString> description;

        /// <summary>
        ///   The source main location to download files for the update
        /// </summary>
        private string downloadUrl;

        /// <summary>
        ///   The collection of files to perform actions on in the update
        /// </summary>
        private ObservableCollection<UpdateFile> files;

        /// <summary>
        ///   Indicates if the update is hidden
        /// </summary>
        private bool hidden;

        /// <summary>
        ///   Indicates the importance type of the update
        /// </summary>
        private Importance importance;

        /// <summary>
        ///   The url pointing to a resource to find more information about the update
        /// </summary>
        private string infoUrl;

        /// <summary>
        ///   The url pointing to the software license for the application/update
        /// </summary>
        private string licenseUrl;

        /// <summary>
        ///   The collection of localized update names
        /// </summary>
        private ObservableCollection<LocaleString> name;

        /// <summary>
        ///   The collection of registry keys and values to perform actions on in the update
        /// </summary>
        private ObservableCollection<RegistryItem> registryItems;

        /// <summary>
        ///   The formatted date string depicting the release date of the update
        /// </summary>
        private string releaseDate;

        /// <summary>
        ///   Indicates if the update is selected
        /// </summary>
        private bool selected;

        /// <summary>
        ///   The collection of shortcuts to perform actions on in the update
        /// </summary>
        private ObservableCollection<Shortcut> shortcuts;

        /// <summary>
        ///   The total download size in bytes of the update
        /// </summary>
        private ulong size;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the collection of localized update descriptions
        /// </summary>
        /// <value>The localized description for the update</value>
        [ProtoMember(2)]
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
        ///   Gets or sets the source main location to download files for the update
        /// </summary>
        /// <value>The url to download the update files.</value>
        [ProtoMember(3)]
        [DataMember]
        public string DownloadUrl
        {
            get
            {
                return this.downloadUrl;
            }

            set
            {
                this.downloadUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("DownloadUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of files to perform actions on in the update
        /// </summary>
        /// <value>The files.</value>
        [ProtoMember(8, IsRequired = false)]
        [DataMember]
        public ObservableCollection<UpdateFile> Files
        {
            get
            {
                return this.files;
            }

            set
            {
                this.files = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Files");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the update is hidden
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if hidden; otherwise, <see langword = "false" />.
        /// </value>
        [ProtoIgnore]
        [IgnoreDataMember]
        public bool Hidden
        {
            get
            {
                return this.hidden;
            }

            set
            {
                this.hidden = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Hidden");
            }
        }

        /// <summary>
        ///   Gets or sets the importance of the update
        /// </summary>
        /// <value>The importance</value>
        [ProtoMember(4)]
        [DataMember]
        public Importance Importance
        {
            get
            {
                return this.importance;
            }

            set
            {
                this.importance = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Importance");
            }
        }

        /// <summary>
        ///   Gets or sets the url pointing to a resource to find more information about the update
        /// </summary>
        /// <value>The info URL.</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string InfoUrl
        {
            get
            {
                return this.infoUrl;
            }

            set
            {
                this.infoUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("InfoUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the url pointing to the software license for the application/update
        /// </summary>
        /// <value>The url pointing to the software license</value>
        [ProtoMember(7, IsRequired = false)]
        [DataMember]
        public string LicenseUrl
        {
            get
            {
                return this.licenseUrl;
            }

            set
            {
                this.licenseUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("LicenseUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of localized update names
        /// </summary>
        /// <value>The localized update names</value>
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
        ///   Gets or sets the collection of registry keys and values to perform actions on in the update
        /// </summary>
        /// <value>The registry items</value>
        [ProtoMember(9, IsRequired = false)]
        [DataMember]
        public ObservableCollection<RegistryItem> RegistryItems
        {
            get
            {
                return this.registryItems;
            }

            set
            {
                this.registryItems = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("RegistryItems");
            }
        }

        /// <summary>
        ///   Gets or sets the formatted date string depicting the release date of the update
        /// </summary>
        /// <value>The release date in a formatted string MM/DD/YYYY</value>
        [ProtoMember(5)]
        [DataMember]
        public string ReleaseDate
        {
            get
            {
                return this.releaseDate;
            }

            set
            {
                this.releaseDate = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ReleaseDate");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the update is selected (not used in the SDK)
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if selected; otherwise, <see langword = "false" />.
        /// </value>
        [ProtoIgnore]
        [IgnoreDataMember]
        public bool Selected
        {
            get
            {
                return this.selected;
            }

            set
            {
                this.selected = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Selected");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of shortcuts to perform actions on in the update
        /// </summary>
        /// <value>The shortcuts.</value>
        [ProtoMember(10, IsRequired = false)]
        [DataMember]
        public ObservableCollection<Shortcut> Shortcuts
        {
            get
            {
                return this.shortcuts;
            }

            set
            {
                this.shortcuts = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Shortcuts");
            }
        }

        /// <summary>
        ///   Gets the total download size in bytes of the update
        /// </summary>
        /// <value>The total download size of the update</value>
        [ProtoMember(11, IsRequired = false)]
        [DataMember]
        public ulong Size
        {
            get
            {
                return this.size;
            }

            internal set
            {
                this.size = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Size");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property
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

    /// <summary>
    /// Information about a file within an update
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(FileAction))]
    public sealed class UpdateFile : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The action to perform on the <see cref = "UpdateFile" />
        /// </summary>
        private FileAction action;

        /// <summary>
        ///   The command line arguments to execute with the file
        /// </summary>
        private string args;

        /// <summary>
        ///   The location where the file will be installed
        /// </summary>
        private string destination;

        /// <summary>
        ///   The size of the file in bytes
        /// </summary>
        private ulong fileSize;

        /// <summary>
        ///   The SHA-2 hash of the file
        /// </summary>
        private string hash;

        /// <summary>
        ///   The download location for the file
        /// </summary>
        private string source;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the action to perform on the <see cref = "UpdateFile" />
        /// </summary>
        /// <value>The action.</value>
        [ProtoMember(1)]
        [DataMember]
        public FileAction Action
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
        ///   Gets or sets the command line arguments to execute with the file
        /// </summary>
        /// <value>The arguments</value>
        [ProtoMember(6, IsRequired = false)]
        [DataMember]
        public string Args
        {
            get
            {
                return this.args;
            }

            set
            {
                this.args = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Args");
            }
        }

        /// <summary>
        ///   Gets or sets the location where the file will be installed
        /// </summary>
        /// <value>The destination.</value>
        [ProtoMember(3)]
        [DataMember]
        public string Destination
        {
            get
            {
                return this.destination;
            }

            set
            {
                this.destination = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Destination");
            }
        }

        /// <summary>
        ///   Gets or sets the size of the file in bytes
        /// </summary>
        /// <value>The size of the file.</value>
        [ProtoMember(5)]
        [DataMember]
        public ulong FileSize
        {
            get
            {
                return this.fileSize;
            }

            set
            {
                this.fileSize = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("FileSize");
            }
        }

        /// <summary>
        ///   Gets or sets the SHA-2 hash of the file
        /// </summary>
        /// <value>The SHA-2 hash of the file.</value>
        [ProtoMember(4)]
        [DataMember]
        public string Hash
        {
            get
            {
                return this.hash;
            }

            set
            {
                this.hash = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Hash");
            }
        }

        /// <summary>
        ///   Gets or sets the download location for the file
        /// </summary>
        /// <value>The download location of the file</value>
        [ProtoMember(2)]
        [DataMember]
        public string Source
        {
            get
            {
                return this.source;
            }

            set
            {
                this.source = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Source");
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

    /// <summary>
    /// Information about an update, used by History and Hidden Updates. Not used by the SDK
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(UpdateStatus))]
    [KnownType(typeof(Importance))]
    [KnownType(typeof(ObservableCollection<LocaleString>))]
    public sealed class Suh : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The <see cref = "Uri" /> for the application's website
        /// </summary>
        private string appUrl;

        /// <summary>
        ///   The collection of localized update descriptions
        /// </summary>
        private ObservableCollection<LocaleString> description;

        /// <summary>
        ///   The help website <see cref = "Uri" /> of the application
        /// </summary>
        private string helpUrl;

        /// <summary>
        ///   The importance of the update
        /// </summary>
        private Importance importance;

        /// <summary>
        ///   The url pointing to a resource to find more information about the update
        /// </summary>
        private string infoUrl;

        /// <summary>
        ///   The formatted date string when the update was installed
        /// </summary>
        private string installDate;

        /// <summary>
        ///   The collection of localized update names
        /// </summary>
        private ObservableCollection<LocaleString> name;

        /// <summary>
        ///   The collection of localized publisher names
        /// </summary>
        private ObservableCollection<LocaleString> publisher;

        /// <summary>
        ///   The formatted date string depicting the release date of the update
        /// </summary>
        private string releaseDate;

        /// <summary>
        ///   The current status of the update
        /// </summary>
        private UpdateStatus status;

        /// <summary>
        ///   The total download size in bytes of the update
        /// </summary>
        private ulong updateSize;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the <see cref = "Uri" /> for the application's website
        /// </summary>
        /// <value>The application website</value>
        [ProtoMember(8)]
        [DataMember]
        public string AppUrl
        {
            get
            {
                return this.appUrl;
            }

            set
            {
                this.appUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("PublisherUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the collection localized update descriptions
        /// </summary>
        /// <value>The localized description for the update</value>
        [ProtoMember(2)]
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
        ///   Gets or sets the help website <see cref = "Uri" /> of the application
        /// </summary>
        /// <value>The help and support website for the application</value>
        [ProtoMember(9, IsRequired = false)]
        [DataMember]
        public string HelpUrl
        {
            get
            {
                return this.helpUrl;
            }

            set
            {
                this.helpUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("HelpUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the importance of the update
        /// </summary>
        /// <value>The importance</value>
        [ProtoMember(3)]
        [DataMember]
        public Importance Importance
        {
            get
            {
                return this.importance;
            }

            set
            {
                this.importance = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Importance");
            }
        }

        /// <summary>
        ///   Gets or sets the url pointing to a resource to find more information about the update
        /// </summary>
        /// <value>The info URL.</value>
        [ProtoMember(10, IsRequired = false)]
        [DataMember]
        public string InfoUrl
        {
            get
            {
                return this.infoUrl;
            }

            set
            {
                this.infoUrl = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("InfoUrl");
            }
        }

        /// <summary>
        ///   Gets or sets the formatted date string when the update was installed
        /// </summary>
        /// <value>The formatted install date string (MM/DD/YYYY).</value>
        [ProtoMember(11)]
        [DataMember]
        public string InstallDate
        {
            get
            {
                return this.installDate;
            }

            set
            {
                this.installDate = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("InstallDate");
            }
        }

        /// <summary>
        ///   Gets or sets the collection of localized update names
        /// </summary>
        /// <value>The localized update names</value>
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
        ///   Gets or sets the collection of localized publisher names
        /// </summary>
        /// <value>The publisher.</value>
        [ProtoMember(7)]
        [DataMember]
        public ObservableCollection<LocaleString> Publisher
        {
            get
            {
                return this.publisher;
            }

            set
            {
                this.publisher = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Publisher");
            }
        }

        /// <summary>
        ///   Gets or sets the formatted date string depicting the release date of the update
        /// </summary>
        /// <value>The release date in a formatted string MM/DD/YYYY</value>
        [ProtoMember(5)]
        [DataMember]
        public string ReleaseDate
        {
            get
            {
                return this.releaseDate;
            }

            set
            {
                this.releaseDate = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("ReleaseDate");
            }
        }

        /// <summary>
        ///   Gets or sets the current status of the update
        /// </summary>
        /// <value>The status.</value>
        [ProtoMember(4)]
        [DataMember]
        public UpdateStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Status");
            }
        }

        /// <summary>
        ///   Gets or sets the total download size in bytes of the update
        /// </summary>
        /// <value>The total download size of the update</value>
        [ProtoMember(6)]
        [DataMember]
        public ulong UpdateSize
        {
            get
            {
                return this.updateSize;
            }

            set
            {
                this.updateSize = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("UpdateSize");
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