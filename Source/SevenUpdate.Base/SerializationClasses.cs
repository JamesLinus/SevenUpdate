#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Win32;
using ProtoBuf;

#endregion

namespace SevenUpdate
{

    #region Application Settings

    /// <summary>
    ///   Configuration options
    /// </summary>
    [ProtoContract, DataContract(IsReference = true)]
    public sealed class Config : INotifyPropertyChanged
    {
        #region Fields

        private AutoUpdateOption autoOption;
        private bool includeRecommended;

        #endregion

        /// <summary>
        ///   Specifies which update setting Seven Update should use
        /// </summary>
        [ProtoMember(1), DataMember]
        public AutoUpdateOption AutoOption
        {
            get { return autoOption; }
            set
            {
                autoOption = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AutoOption");
            }
        }

        /// <summary>
        ///   Gets or Sets a value indicating if Seven Update is to included recommended updates when automatically downloading updates
        /// </summary>
        [ProtoMember(2), DataMember]
        public bool IncludeRecommended
        {
            get { return includeRecommended; }
            set
            {
                includeRecommended = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("IncludeRecommended");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }

    #region Enums

    /// <summary>
    ///   Automatic Update option Seven Update can use
    /// </summary>
    [ProtoContract, DataContract, DefaultValue(Install)]
    public enum AutoUpdateOption
    {
        /// <summary>
        ///   Download and Installs updates automatically
        /// </summary>
        [ProtoEnum, EnumMember] Install = 0,

        /// <summary>
        ///   Downloads Updates automatically
        /// </summary>
        [ProtoEnum, EnumMember] Download = 1,

        /// <summary>
        ///   Only checks and notifies the user of updates
        /// </summary>
        [ProtoEnum, EnumMember] Notify = 2,

        /// <summary>
        ///   No automatic checking
        /// </summary>
        [ProtoEnum, EnumMember] Never = 3
    }

    #endregion

    #endregion

    #region Locale Classes

    /// <summary>
    ///   Contains a string indicating the language and a value
    /// </summary>
    [ProtoContract, DataContract]
    public sealed class LocaleString : INotifyPropertyChanged
    {
        #region Fields

        private string lang, value;

        #endregion

        /// <summary>
        ///   an ISO language code
        /// </summary>
        [ProtoMember(1), DataMember]
        public string Lang
        {
            get { return lang; }
            set
            {
                lang = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Lang");
            }
        }

        /// <summary>
        ///   The value of the string
        /// </summary>
        [ProtoMember(2), DataMember]
        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Value");
            }
        }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    #endregion

    #region SUA File

    /// <summary>
    ///   Seven Update Application information
    /// </summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (ObservableCollection<LocaleString>))]
    public sealed class Sua : INotifyPropertyChanged
    {
        #region Fields

        private string appUrl;
        private ObservableCollection<LocaleString> description;
        private string directory;
        private string helpUrl;
        private bool is64Bit, isEnabled;
        private ObservableCollection<LocaleString> name;
        private ObservableCollection<LocaleString> publisher;
        private string suiUrl;
        private string valueName;

        #endregion

        /// <summary>
        ///   The application name
        /// </summary>
        [ProtoMember(1), DataMember]
        public ObservableCollection<LocaleString> Name
        {
            get { return name; }
            set
            {
                name = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        ///   Gets or Sets release information about the update
        /// </summary>
        [ProtoMember(2), DataMember]
        public ObservableCollection<LocaleString> Description
        {
            get { return description; }
            set
            {
                description = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        ///   The directory where the application is installed
        /// </summary>
        [ProtoMember(3), DataMember]
        public string Directory
        {
            get { return directory; }
            set
            {
                directory = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Directory");
            }
        }

        /// <summary>
        ///   The name of the value to the registry key that contains the application directory location
        /// </summary>
        [ProtoMember(10, IsRequired = false), DataMember]
        public string ValueName
        {
            get { return valueName; }
            set
            {
                valueName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ValueName");
            }
        }

        /// <summary>
        ///   Specifies if the application is 64 bit
        /// </summary>
        [ProtoMember(4), DataMember]
        public bool Is64Bit
        {
            get { return is64Bit; }
            set
            {
                is64Bit = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Is64Bit");
            }
        }

        /// <summary>
        ///   Gets or Sets a value Indicating if the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        [ProtoMember(5), DataMember]
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        ///   The publisher of the application
        /// </summary>
        [ProtoMember(6), DataMember]
        public ObservableCollection<LocaleString> Publisher
        {
            get { return publisher; }
            set
            {
                publisher = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Publisher");
            }
        }

        /// <summary>
        ///   The url pointing to the sui file containing the app updates
        /// </summary>
        [ProtoMember(7), DataMember]
        public string SuiUrl
        {
            get { return suiUrl; }
            set
            {
                suiUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("SuiUrl");
            }
        }

        /// <summary>
        ///   The url for the application: Optional
        /// </summary>
        [ProtoMember(8, IsRequired = false), DataMember]
        public string AppUrl
        {
            get { return appUrl; }
            set
            {
                appUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AppUrl");
            }
        }

        /// <summary>
        ///   The help url of the update: Optional
        /// </summary>
        [ProtoMember(9, IsRequired = false), DataMember]
        public string HelpUrl
        {
            get { return helpUrl; }
            set
            {
                helpUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("HelpUrl");
            }
        }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "propertyName" />
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #endregion

    #region SUI File

    #region Enum

    /// <summary>
    ///   The action to preform on the shortcut
    /// </summary>
    [ProtoContract, DataContract, DefaultValue(Add)]
    public enum ShortcutAction
    {
        /// <summary>
        ///   Adds a shortcut
        /// </summary>
        [ProtoEnum, EnumMember] Add = 0,

        /// <summary>
        ///   Updates a shortcut only if it exists
        /// </summary>
        [ProtoEnum, EnumMember] Update = 1,

        /// <summary>
        ///   Deletes a shortcut
        /// </summary>
        [ProtoEnum, EnumMember] Delete = 2
    }

    /// <summary>
    ///   The action to perform on the file
    /// </summary>
    [ProtoContract, DataContract, DefaultValue(Update)]
    public enum FileAction
    {
        /// <summary>
        ///   Updates a file
        /// </summary>
        [ProtoEnum, EnumMember] Update = 0,

        /// <summary>
        ///   Updates a file, only if it exist
        /// </summary>
        [ProtoEnum, EnumMember] UpdateIfExist = 1,

        /// <summary>
        ///   Updates a file, then registers the dll
        /// </summary>
        [ProtoEnum, EnumMember] UpdateThenRegister = 2,

        /// <summary>
        ///   Updates a file, then executes it
        /// </summary>
        [ProtoEnum, EnumMember] UpdateThenExecute = 3,

        /// <summary>
        ///   Compares a file, but does not update it
        /// </summary>
        [ProtoEnum, EnumMember] CompareOnly = 4,

        /// <summary>
        ///   Executes a file, can be on system or be downloaded
        /// </summary>
        [ProtoEnum, EnumMember] Execute = 5,

        /// <summary>
        ///   Deletes a file
        /// </summary>
        [ProtoEnum, EnumMember] Delete = 6,

        /// <summary>
        ///   Executes a file, then deletes it
        /// </summary>
        [ProtoEnum, EnumMember] ExecuteThenDelete = 7,

        /// <summary>
        ///   Unregisteres a dll, then deletes it
        /// </summary>
        [ProtoEnum, EnumMember] UnregisterThenDelete = 8,
    }

    /// <summary>
    ///   Contains the UpdateType of the update
    /// </summary>
    [ProtoContract, DataContract, DefaultValue(Important)]
    public enum Importance
    {
        /// <summary>
        ///   Important update
        /// </summary>
        [ProtoEnum, EnumMember] Important = 0,

        /// <summary>
        ///   Locale or language
        /// </summary>
        [ProtoEnum, EnumMember] Locale = 1,

        /// <summary>
        ///   Optional update
        /// </summary>
        [ProtoEnum, EnumMember] Optional = 2,

        /// <summary>
        ///   Recommended update
        /// </summary>
        [ProtoEnum, EnumMember] Recommended = 3
    }

    /// <summary>
    ///   The current status of the update
    /// </summary>
    [ProtoContract, DataContract, DefaultValue(Successful)]
    public enum UpdateStatus
    {
        /// <summary>
        ///   Indicates that the update installation failed
        /// </summary>
        [ProtoEnum, EnumMember] Failed = 0,

        /// <summary>
        ///   Indicates that the update is hidden
        /// </summary>
        [ProtoEnum, EnumMember] Hidden = 1,

        /// <summary>
        ///   Indicates that the update is visible
        /// </summary>
        [ProtoEnum, EnumMember] Visible = 2,

        /// <summary>
        ///   Indicates that the update installation succeeded
        /// </summary>
        [ProtoEnum, EnumMember] Successful = 3
    }

    /// <summary>
    ///   Contains the Actions you can perform to the registry
    /// </summary>
    [ProtoContract, DataContract, DefaultValue(Add)]
    public enum RegistryAction
    {
        /// <summary>
        ///   Adds a registry entry to the machine
        /// </summary>
        [ProtoEnum, EnumMember] Add = 0,
        /// <summary>
        ///   Deletes a registry key on the machine
        /// </summary>
        [ProtoEnum, EnumMember] DeleteKey = 1,
        /// <summary>
        ///   Deletes a value of a registry key on the machine
        /// </summary>
        [ProtoEnum, EnumMember] DeleteValue = 2
    }

    #endregion

    #region Classes

    /// <summary>
    ///   The collection of updates and the application info.
    /// </summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (Sua)), KnownType(typeof (ObservableCollection<Update>))]
    public class Sui : INotifyPropertyChanged
    {
        #region Fields

        private Sua appInfo;
        private ObservableCollection<Update> updates;

        #endregion

        /// <summary>
        ///   Software information for the app updates.
        /// </summary>
        [ProtoMember(2), DataMember]
        public Sua AppInfo
        {
            get { return appInfo; }
            set
            {
                appInfo = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AppInfo");
            }
        }

        /// <summary>
        ///   Collection of updates for the application
        /// </summary>
        [ProtoMember(1), DataMember]
        public ObservableCollection<Update> Updates
        {
            get { return updates; }
            set
            {
                updates = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Updates");
            }
        }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    ///   Information on how to install a software update
    /// </summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (ObservableCollection<LocaleString>)), KnownType(typeof (UpdateFile)), KnownType(typeof (RegistryItem)),
     KnownType(typeof (Shortcut))]
    public sealed class Update : INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<LocaleString> description;
        private string downloadUrl;
        private ObservableCollection<UpdateFile> files;
        private bool hidden;
        private Importance importance;
        private string infoUrl, licenseUrl;
        private ObservableCollection<LocaleString> name;
        private ObservableCollection<RegistryItem> registryItems;
        private string releaseDate;
        private bool selected;
        private ObservableCollection<Shortcut> shortcuts;
        private ulong size;

        #endregion

        #region Required Properties

        /// <summary>
        ///   The name of the update
        /// </summary>
        [ProtoMember(1), DataMember]
        public ObservableCollection<LocaleString> Name
        {
            get { return name; }
            set
            {
                name = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        ///   Release information about the update
        /// </summary>
        [ProtoMember(2), DataMember]
        public ObservableCollection<LocaleString> Description
        {
            get { return description; }
            set
            {
                description = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        ///   The default download directory where the updates files are stored
        /// </summary>
        [ProtoMember(3), DataMember]
        public string DownloadUrl
        {
            get { return downloadUrl; }
            set
            {
                downloadUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DownloadUrl");
            }
        }

        /// <summary>
        ///   The update type of the update: Important, Recommended, Optional, Locale, Installation.
        /// </summary>
        [ProtoMember(4), DataMember]
        public Importance Importance
        {
            get { return importance; }
            set
            {
                importance = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Importance");
            }
        }

        /// <summary>
        ///   The date when the update was released
        /// </summary>
        [ProtoMember(5), DataMember]
        public string ReleaseDate
        {
            get { return releaseDate; }
            set
            {
                releaseDate = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ReleaseDate");
            }
        }

        #endregion

        #region Optional Properties

        /// <summary>
        ///   The information/change log url of the update: Optional
        /// </summary>
        [ProtoMember(6, IsRequired = false), DataMember]
        public string InfoUrl
        {
            get { return infoUrl; }
            set
            {
                infoUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("InfoUrl");
            }
        }

        /// <summary>
        ///   The Software License Agreement Url
        /// </summary>
        [ProtoMember(7, IsRequired = false), DataMember]
        public string LicenseUrl
        {
            get { return licenseUrl; }
            set
            {
                licenseUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("LicenseUrl");
            }
        }

        #endregion

        /// <summary>
        ///   The files of the current update
        /// </summary>
        [ProtoMember(8, IsRequired = false), DataMember]
        public ObservableCollection<UpdateFile> Files
        {
            get { return files; }
            set
            {
                files = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Files");
            }
        }

        #region Optional SubProperties

        /// <summary>
        ///   The registry entries of the current update
        /// </summary>
        [ProtoMember(9, IsRequired = false), DataMember]
        public ObservableCollection<RegistryItem> RegistryItems
        {
            get { return registryItems; }
            set
            {
                registryItems = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("RegistryItems");
            }
        }

        /// <summary>
        ///   The shortcuts to create for the update
        /// </summary>
        [ProtoMember(10, IsRequired = false), DataMember]
        public ObservableCollection<Shortcut> Shortcuts
        {
            get { return shortcuts; }
            set
            {
                shortcuts = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Shortcuts");
            }
        }

        #endregion

        #region UI Properties

        /// <summary>
        ///   Gets or Sets a value Indicating if the update is visible or hidden (not used in the SDK)
        /// </summary>
        [ProtoIgnore, IgnoreDataMember]
        public bool Hidden
        {
            get { return hidden; }
            set
            {
                hidden = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Hidden");
            }
        }

        /// <summary>
        ///   Gets or Sets a value Indicating if the update is selected (not used in the SDK)
        /// </summary>
        [ProtoIgnore, IgnoreDataMember]
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Selected");
            }
        }

        /// <summary>
        ///   The download size of the update in bytes (not used in the SDK)
        /// </summary>
        [ProtoIgnore, IgnoreDataMember]
        public ulong Size
        {
            get { return size; }
            internal set
            {
                size = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Size");
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "propertyName" />
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    ///   Information about a file within an update
    /// </summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (FileAction))]
    public sealed class UpdateFile : INotifyPropertyChanged
    {
        #region Fields

        private FileAction action;
        private string args;
        private string destination;
        private ulong fileSize;
        private string hash;
        private string source;

        #endregion

        #region Required Properties

        /// <summary>
        ///   The action to perform on a file
        /// </summary>
        [ProtoMember(1), DataMember]
        public FileAction Action
        {
            get { return action; }
            set
            {
                action = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Action");
            }
        }

        /// <summary>
        ///   The source location of the current file with the filename
        /// </summary>
        [ProtoMember(2), DataMember]
        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Source");
            }
        }

        /// <summary>
        ///   The destination location of the current file with the filename
        /// </summary>
        [ProtoMember(3), DataMember]
        public string Destination
        {
            get { return destination; }
            set
            {
                destination = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Destination");
            }
        }

        /// <summary>
        ///   The SHA-2 hash of the current file
        /// </summary>
        [ProtoMember(4), DataMember]
        public string Hash
        {
            get { return hash; }
            set
            {
                hash = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Hash");
            }
        }

        /// <summary>
        ///   File size in bytes
        /// </summary>
        [ProtoMember(5), DataMember]
        public ulong FileSize
        {
            get { return fileSize; }
            set
            {
                fileSize = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("FileSize");
            }
        }

        #endregion

        #region Optional Properties

        /// <summary>
        ///   Command line arguments for the file
        /// </summary>
        [ProtoMember(6, IsRequired = false), DataMember]
        public string Args
        {
            get { return args; }
            set
            {
                args = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Args");
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    ///   A registry entry within an update
    /// </summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (RegistryAction)), KnownType(typeof (RegistryHive)), KnownType(typeof (RegistryValueKind))]
    public sealed class RegistryItem : INotifyPropertyChanged
    {
        #region Fields

        private RegistryAction action;
        private string data;
        private RegistryHive hive;
        private string key, keyValue;
        private RegistryValueKind valueKind;

        #endregion

        #region Required Properties

        /// <summary>
        ///   The action to perform to the registry item
        /// </summary>
        [ProtoMember(1), DataMember]
        public RegistryAction Action
        {
            get { return action; }
            set
            {
                action = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Action");
            }
        }

        /// <summary>
        ///   The hive of the current registry item
        /// </summary>
        [ProtoMember(2), DefaultValue(RegistryHive.LocalMachine), DataMember]
        public RegistryHive Hive
        {
            get { return hive; }
            set
            {
                hive = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Hive");
            }
        }

        /// <summary>
        ///   The Key path of the current registry item
        /// </summary>
        [ProtoMember(3), DataMember]
        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Key");
            }
        }

        #endregion

        #region Optional Properties

        /// <summary>
        ///   Name of the Value in the specified key
        /// </summary>
        [ProtoMember(4, IsRequired = false), DataMember]
        public string KeyValue
        {
            get { return keyValue; }
            set
            {
                keyValue = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("KeyValue");
            }
        }

        /// <summary>
        ///   The ValueKind of the value in the specified key
        /// </summary>
        [ProtoMember(5, IsRequired = false), DataMember]
        public RegistryValueKind ValueKind
        {
            get { return valueKind; }
            set
            {
                valueKind = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ValueKind");
            }
        }

        /// <summary>
        ///   The data of the value in the specified key
        /// </summary>
        [ProtoMember(6, IsRequired = false), DataMember]
        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Data");
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    ///   A shortcut to be created within an update
    /// </summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (ShortcutAction))]
    public sealed class Shortcut : INotifyPropertyChanged
    {
        #region Fields

        private ShortcutAction action;
        private string arguments;
        private ObservableCollection<LocaleString> description;
        private string icon;
        private string location;
        private ObservableCollection<LocaleString> name;
        private string target;

        #endregion

        #region Required Properties

        /// <summary>
        ///   The location of where the shortcut is to be stored.
        /// </summary>
        [ProtoMember(1), DataMember]
        public ObservableCollection<LocaleString> Name
        {
            get { return name; }
            set
            {
                name = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        ///   The location of where the shortcut is to be stored.
        /// </summary>
        [ProtoMember(2), DataMember]
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Location");
            }
        }

        /// <summary>
        ///   The action to peform on the shortcut
        /// </summary>
        [ProtoMember(3), DataMember]
        public ShortcutAction Action
        {
            get { return action; }
            set
            {
                action = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Action");
            }
        }

        #endregion

        #region Optional Properties

        /// <summary>
        ///   Any arguments to be used with the shortcut
        /// </summary>
        [ProtoMember(4, IsRequired = false), DataMember]
        public string Arguments
        {
            get { return arguments; }
            set
            {
                arguments = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Arguments");
            }
        }

        /// <summary>
        ///   Description of the shortcut
        /// </summary>
        [ProtoMember(5, IsRequired = false), DataMember]
        public ObservableCollection<LocaleString> Description
        {
            get { return description; }
            set
            {
                description = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        ///   The full path to the icon or exe containing an icon
        /// </summary>
        [ProtoMember(6, IsRequired = false), DataMember]
        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Icon");
            }
        }

        /// <summary>
        ///   The full path of the target to the shortcut.
        /// </summary>
        [ProtoMember(7, IsRequired = false), DataMember]
        public string Target
        {
            get { return target; }
            set
            {
                target = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Target");
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "propertyName" />
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #endregion

    #endregion

    #region History and Hidden Updates Files

    /// <summary>
    ///   Information about an update, used by History and Hidden Updates. Not used by the SDK
    /// </summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof (UpdateStatus)), KnownType(typeof (Importance)), KnownType(typeof (ObservableCollection<LocaleString>))]
    public sealed class Suh : INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<LocaleString> description;
        private string helpUrl;
        private Importance importance;
        private string infoUrl, installDate;
        private ObservableCollection<LocaleString> name;
        private ObservableCollection<LocaleString> publisher;
        private string publisherUrl;
        private string releaseDate;
        private UpdateStatus status;
        private ulong updateSize;

        #endregion

        #region Required Properties

        /// <summary>
        ///   The name of the update
        /// </summary>
        [ProtoMember(1), DataMember]
        public ObservableCollection<LocaleString> Name
        {
            get { return name; }
            set
            {
                name = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        ///   A description of the update, usually list new features or changes the update brings.
        /// </summary>
        [ProtoMember(2), DataMember]
        public ObservableCollection<LocaleString> Description
        {
            get { return description; }
            set
            {
                description = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        ///   The update type of the update: Critical, Recommended, Optional, Locale
        /// </summary>
        [ProtoMember(3), DataMember]
        public Importance Importance
        {
            get { return importance; }
            set
            {
                importance = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Importance");
            }
        }

        /// <summary>
        ///   The current status of the update
        /// </summary>
        [ProtoMember(4), DataMember]
        public UpdateStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Status");
            }
        }

        /// <summary>
        ///   The date when the update was released
        /// </summary>
        [ProtoMember(5), DataMember]
        public string ReleaseDate
        {
            get { return releaseDate; }
            set
            {
                releaseDate = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ReleaseDate");
            }
        }

        /// <summary>
        ///   The full size of the update
        /// </summary>
        [ProtoMember(6), DataMember]
        public ulong UpdateSize
        {
            get { return updateSize; }
            set
            {
                updateSize = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("UpdateSize");
            }
        }

        /// <summary>
        ///   The Publisher of the update/application
        /// </summary>
        [ProtoMember(7), DataMember]
        public ObservableCollection<LocaleString> Publisher
        {
            get { return publisher; }
            set
            {
                publisher = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Publisher");
            }
        }

        /// <summary>
        ///   The website of the publisher
        /// </summary>
        [ProtoMember(8), DataMember]
        public string PublisherUrl
        {
            get { return publisherUrl; }
            set
            {
                publisherUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("PublisherUrl");
            }
        }

        #endregion

        #region Optional Properties

        /// <summary>
        ///   The help url of the update: Optional
        /// </summary>
        [ProtoMember(9, IsRequired = false), DataMember]
        public string HelpUrl
        {
            get { return helpUrl; }
            set
            {
                helpUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("HelpUrl");
            }
        }

        /// <summary>
        ///   The information/change log url of the update: Optional
        /// </summary>
        [ProtoMember(10, IsRequired = false), DataMember]
        public string InfoUrl
        {
            get { return infoUrl; }
            set
            {
                infoUrl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("InfoUrl");
            }
        }

        /// <summary>
        ///   The date when the update was installed
        /// </summary>
        [ProtoMember(11), DataMember]
        public string InstallDate
        {
            get { return installDate; }
            set
            {
                installDate = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("InstallDate");
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "propertyName" />
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #endregion
}