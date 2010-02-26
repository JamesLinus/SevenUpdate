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
using System.Xml.Serialization;
using Microsoft.Win32;
using ProtoBuf;

#endregion

namespace SevenUpdate.Base
{

    #region Application Settings

    /// <summary>Configuration options</summary>
    [ProtoContract, XmlType(AnonymousType = true), XmlRoot("settings", Namespace = "http://sevenupdate.com")]
    public class Config
    {
        /// <summary>
        /// Specifies which update setting Seven Update should use
        /// </summary>
        [ProtoMember(1), XmlElement("auto")]
        public AutoUpdateOption AutoOption { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating if Seven Update is to included recommended updates when automatically downloading updates
        /// </summary>
        [ProtoMember(2), XmlElement("includeRecommended")]
        public bool IncludeRecommended { get; set; }

        /// <summary>
        /// Specifies the language Seven Update uses
        /// </summary>
        [ProtoMember(3), XmlElement("locale")]
        public string Locale { get; set; }
    }

    #region Enums

    /// <summary>
    /// Automatic Update option Seven Update can use
    /// </summary>
    [ProtoContract]
    public enum AutoUpdateOption
    {
        /// <summary>
        /// Download and Installs updates automatically
        /// </summary>
        [ProtoEnum] Install=0,

        /// <summary>
        /// Downloads Updates automatically
        /// </summary>
        [ProtoEnum] Download=1,

        /// <summary>
        /// Only checks and notifies the user of updates
        /// </summary>
        [ProtoEnum] Notify=2,

        /// <summary>No automatic checking</summary>
        [ProtoEnum] Never=3
    }

    #endregion

    #endregion

    #region Locale Classes

    /// <summary>
    /// Contains a string indicating the language and a value
    /// </summary>
    [ProtoContract, XmlType(Namespace = "http://sevenupdate.com", AnonymousType = true)]
    public class LocaleString : INotifyPropertyChanged
    {
        /// <summary>an ISO language code</summary>
        [ProtoMember(1), XmlAttribute("lang", Namespace = "")]
        public string Lang { get; set; }

        /// <summary>The value of the string</summary>
        [ProtoMember(2), XmlAttribute("value", Namespace = "")]
        public string Value { get; set; }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
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
    /// Seven Update Application information
    /// </summary>
    [ProtoContract, XmlRoot("application", Namespace = "http://sevenupdate.com")]
    public class Sua : INotifyPropertyChanged
    {
        /// <summary>The application name</summary>
        [ProtoMember(1), XmlElement("name")]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// Gets or Sets release information about the update
        /// </summary>
        [ProtoMember(2), XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The directory where the application is installed
        /// </summary>
        [ProtoMember(3), XmlAttribute("directory")]
        public string Directory { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [ProtoMember(4), XmlAttribute("is64Bit")]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// Gets or Sets a value Indicating if the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        [ProtoMember(5), XmlAttribute("isEnabled")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The publisher of the application
        /// </summary>
        [ProtoMember(6), XmlElement("publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The SUI file of the application
        /// </summary>
        [ProtoMember(7), XmlAttribute("source")]
        public string Source { get; set; }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    #endregion

    #region SUI File

    #region Enum

    /// <summary>
    /// The action to preform on the shortcut
    /// </summary>
    [ProtoContract]
    public enum ShortcutAction
    {
        /// <summary>
        /// Adds a shortcut
        /// </summary>
        [ProtoEnum] Add=0,

        /// <summary>
        /// Deletes a shortcut
        /// </summary>
        [ProtoEnum] Delete=2,

        /// <summary>
        /// Updates a shortcut only if it exists
        /// </summary>
        [ProtoEnum] Update=1
    }

    /// <summary>
    /// The action to perform on the file
    /// </summary>
    [ProtoContract]
    public enum FileAction
    {
        /// <summary>Updates a file</summary>
        [ProtoEnum] Update=0,

        /// <summary>Updates a file, only if it exist</summary>
        [ProtoEnum] UpdateIfExist=1,

        /// <summary>Updates a file, then registers the dll</summary>
        [ProtoEnum] UpdateThenRegister=2,

        /// <summary>Updates a file, then executes it</summary>
        [ProtoEnum] UpdateThenExecute=3,

        /// <summary>Compares a file, but does not update it</summary>
        [ProtoEnum] CompareOnly=4,

        /// <summary>Executes a file, can be on system or be downloaded</summary>
        [ProtoEnum] Execute=5,

        /// <summary>
        /// Deletes a file
        /// </summary>
        [ProtoEnum] Delete=6,

        /// <summary>
        /// Executes a file, then deletes it
        /// </summary>
        [ProtoEnum] ExecuteThenDelete=7,

        /// <summary>
        /// Unregisteres a dll, then deletes it
        /// </summary>
        [ProtoEnum] UnregisterThenDelete=8,
    }

    /// <summary>
    /// Contains the UpdateType of the update
    /// </summary>
    [ProtoContract]
    public enum Importance
    {
        /// <summary>Important update</summary>
        [ProtoEnum] Important=0,

        /// <summary>Locale or language</summary>
        [ProtoEnum] Locale=1,

        /// <summary>Optional update</summary>
        [ProtoEnum] Optional=2,

        /// <summary>Recommended update</summary>
        [ProtoEnum] Recommended=3
    }

    /// <summary>
    /// The current status of the update
    /// </summary>
    [ProtoContract]
    public enum UpdateStatus
    {
        /// <summary>
        /// Indicates that the update installation failed
        /// </summary>
        [ProtoEnum] Failed=0,

        /// <summary>
        /// Indicates that the update is hidden
        /// </summary>
        [ProtoEnum] Hidden=1,

        /// <summary>
        /// Indicates that the update is visible
        /// </summary>
        [ProtoEnum] Visible=2,

        /// <summary>
        /// Indicates that the update installation succeeded
        /// </summary>
        [ProtoEnum] Successful=3
    }

    /// <summary>
    /// Contains the Actions you can perform to the registry
    /// </summary>
    public enum RegistryAction
    {
        /// <summary>
        /// Adds a registry entry to the machine
        /// </summary>
        [ProtoEnum] Add=0,
        /// <summary>
        /// Deletes a registry key on the machine
        /// </summary>
        [ProtoEnum] DeleteKey=1,
        /// <summary>
        /// Deletes a value of a registry key on the machine
        /// </summary>
        [ProtoEnum] DeleteValue=2
    }

    #endregion

    #region Classes

    /// <summary>Application info</summary>
    [XmlType(AnonymousType = true), XmlRoot("application", Namespace = "http://sevenupdate.com"), ProtoContract]
    public class Sui : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The application main directory, usually in Program Files
        /// </summary>
        [XmlAttribute("directory"), ProtoMember(1)]
        public string Directory { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [ProtoMember(2), XmlAttribute("is64Bit")]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// The company or developer of the Application
        /// </summary>
        [ProtoMember(3), XmlElement("publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The url of the company or developer
        /// </summary>
        [ProtoMember(4), XmlAttribute("publisherUrl")]
        public string PublisherUrl { get; set; }

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [ProtoMember(5), XmlAttribute("helpUrl")]
        public string HelpUrl { get; set; }

        #endregion

        #region Required Sub-Properties

        /// <summary>
        /// Collection of updates for the application
        /// </summary>
        [ProtoMember(6), XmlElement("update")]
        public ObservableCollection<Update> Updates { get; set; }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    /// Information on how to install a software update
    /// </summary>
    [ProtoContract, XmlType("update", Namespace = "http://sevenupdate.com", AnonymousType = true)]
    public class Update : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>The name of the update</summary>
        [ProtoMember(1), XmlElement("name")]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// Release information about the update
        /// </summary>
        [ProtoMember(2), XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The default download directory where the updates files are stored
        /// </summary>
        [ProtoMember(3), XmlAttribute("downloadUrl")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// The update type of the update: Important, Recommended, Optional, Locale, Installation.
        /// </summary>
        [ProtoMember(4), XmlAttribute("importance")]
        public Importance Importance { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [ProtoMember(5), XmlAttribute("releaseDate")]
        public string ReleaseDate { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// The information/change log url of the update: Optional
        /// </summary>
        [ProtoMember(6, IsRequired = false), XmlAttribute("infoUrl")]
        public string InfoUrl { get; set; }

        /// <summary>
        /// The Software License Agreement Url
        /// </summary>
        [ProtoMember(7, IsRequired = false), XmlAttribute("licenseUrl")]
        public string LicenseUrl { get; set; }

        #endregion

        #region Optional SubProperties

        /// <summary>
        /// The files of the current update
        /// </summary>
        [ProtoMember(8, IsRequired = false), XmlElement("file")]
        public ObservableCollection<UpdateFile> Files { get; set; }

        /// <summary>
        /// The registry entries of the current update
        /// </summary>
        [ProtoMember(9, IsRequired = false), XmlElement("registryItem")]
        public ObservableCollection<RegistryItem> RegistryItems { get; set; }

        /// <summary>
        /// The shortcuts to create for the update
        /// </summary>
        [ProtoMember(10, IsRequired = false), XmlElement("shortcut")]
        public ObservableCollection<Shortcut> Shortcuts { get; set; }

        #endregion

        #region UI Properties

        /// <summary>
        /// Gets or Sets a value Indicating if the update is selected (not used in the SDK)
        /// </summary>
        [XmlIgnore]
        [ProtoIgnore]
        public bool Selected { get; set; }

        /// <summary>
        /// The download size of the update in bytes, not used by the SDK
        /// </summary>
        [XmlIgnore]
        [ProtoIgnore]
        public ulong Size { get; set; }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    /// Information about a file within an update
    /// </summary>
    [ProtoContract, XmlType("file", Namespace = "http://sevenupdate.com", AnonymousType = true)]
    public class UpdateFile : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The action to perform on a file
        /// </summary>
        [ProtoMember(1), XmlAttribute("action")]
        public FileAction Action { get; set; }

        /// <summary>
        /// The source location of the current file with the filename
        /// </summary>
        [ProtoMember(2), XmlAttribute("source")]
        public string Source { get; set; }

        /// <summary>
        /// The destination location of the current file with the filename
        /// </summary>
        [ProtoMember(3), XmlAttribute("destination")]
        public string Destination { get; set; }

        /// <summary>
        /// The SHA1 hash of the current file
        /// </summary>
        [ProtoMember(4), XmlAttribute("hash")]
        public string Hash { get; set; }

        /// <summary>File size in bytes</summary>
        [ProtoMember(5), XmlAttribute("size")]
        public ulong Size { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Command line arguments for the file
        /// </summary>
        [ProtoMember(6, IsRequired = false), XmlAttribute("args")]
        public string Args { get; set; }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    /// A registry entry within an update
    /// </summary>
    [ProtoContract, XmlType("registryItem", Namespace = "http://sevenupdate.com", AnonymousType = true)]
    public class RegistryItem : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The action to perform to the registry item
        /// </summary>
        [ProtoMember(1), XmlAttribute("action")]
        public RegistryAction Action { get; set; }

        /// <summary>
        /// The hive of the current registry item
        /// </summary>
        [ProtoMember(2, IsRequired = true), XmlAttribute("hive")]
        public RegistryHive Hive { get; set; }

        /// <summary>
        /// The Key path of the current registry item
        /// </summary>
        [ProtoMember(3), XmlAttribute("key")]
        public string Key { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Name of the Value in the specified key
        /// </summary>
        [ProtoMember(4, IsRequired = false), XmlAttribute("keyValue")]
        public string KeyValue { get; set; }

        /// <summary>
        /// The ValueKind of the value in the specified key
        /// </summary>
        [ProtoMember(5, IsRequired = false), XmlAttribute("valueKind")]
        public RegistryValueKind ValueKind { get; set; }

        /// <summary>
        /// The data of the value in the specified key
        /// </summary>
        [ProtoMember(6, IsRequired = false), XmlAttribute("data")]
        public string Data { get; set; }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    /// A shortcut to be created within an update
    /// </summary>
    [ProtoContract, XmlType("shortcut", Namespace = "http://sevenupdate.com", AnonymousType = true)]
    public class Shortcut : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The location of where the shortcut is to be stored.
        /// </summary>
        [ProtoMember(1), XmlAttribute("location")]
        public string Location { get; set; }

        /// <summary>
        /// The action to peform on the shortcut
        /// </summary>
        [ProtoMember(2), XmlAttribute("action")]
        public ShortcutAction Action { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Any arguments to be used with the shortcut
        /// </summary>
        [ProtoMember(3, IsRequired = false), XmlAttribute("arguments")]
        public string Arguments { get; set; }

        /// <summary>
        /// Description of the shortcut
        /// </summary>
        [ProtoMember(4, IsRequired = false), XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The full path to the icon or exe containing an icon
        /// </summary>
        [ProtoMember(5, IsRequired = false), XmlAttribute("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// The full path of the target to the shortcut.
        /// </summary>
        [ProtoMember(6, IsRequired = false), XmlAttribute("target")]
        public string Target { get; set; }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    #endregion

    #endregion

    #region History and Hidden Updates Files

    /// <summary>
    /// Information about an update, used by History and Hidden Updates. Not used by the SDK
    /// </summary>
    [ProtoContract, XmlType(AnonymousType = true), XmlRoot("update", Namespace = "http://sevenupdate.com")]
    public class Suh : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>The name of the update</summary>
        [ProtoMember(1), XmlElement("name")]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// A description of the update, usually list new features or changes the update brings.
        /// </summary>
        [ProtoMember(2), XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The update type of the update: Critical, Recommended, Optional, Locale
        /// </summary>
        [ProtoMember(3), XmlAttribute("importance")]
        public Importance Importance { get; set; }

        /// <summary>
        /// The current status of the update
        /// </summary>
        [ProtoMember(4), XmlAttribute("status")]
        public UpdateStatus Status { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [ProtoMember(5), XmlAttribute("releaseDate")]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// The full size of the update
        /// </summary>
        [ProtoMember(6), XmlAttribute("size")]
        public ulong Size { get; set; }

        /// <summary>
        /// The Publisher of the update/application
        /// </summary>
        [ProtoMember(7), XmlElement("publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The website of the publisher
        /// </summary>
        [ProtoMember(8), XmlAttribute("publisherUrl")]
        public string PublisherUrl { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [ProtoMember(9, IsRequired = false), XmlAttribute("helpUrl")]
        public string HelpUrl { get; set; }

        /// <summary>
        /// The information/change log url of the update: Optional
        /// </summary>
        [ProtoMember(10, IsRequired = false), XmlAttribute("infoUrl")]
        public string InfoUrl { get; set; }

        /// <summary>
        /// The date when the update was installed
        /// </summary>
        [XmlAttribute("installDate"), ProtoMember(11)]
        public string InstallDate { get; set; }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged" /> Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    #endregion
}