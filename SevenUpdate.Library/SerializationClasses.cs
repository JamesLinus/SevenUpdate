#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
//  
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//  
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Win32;

#endregion

namespace SevenUpdate
{

    #region Application Settings

    #region Struct

    /// <summary>Configuration options</summary>
    [XmlType(AnonymousType = true), XmlRoot("settings", Namespace = "http://sevenupdate.sourceforge.net")]
    public struct Config
    {
        /// <summary>
        /// Specifies which update setting Seven Update should use
        /// </summary>
        [XmlElement("auto")]
        public AutoUpdateOption AutoOption { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating if Seven Update is to included recommended updates when automatically downloading updates
        /// </summary>
        [XmlElement("includeRecommended")]
        public bool IncludeRecommended { get; set; }

        /// <summary>
        /// Specifies the language Seven Update uses
        /// </summary>
        [XmlElement("locale")]
        public string Locale { get; set; }
    }

    #endregion

    #region Enums

    /// <summary>
    /// Automatic Update option Seven Update can use
    /// </summary>
    public enum AutoUpdateOption
    {
        /// <summary>
        /// Download and Installs updates automatically
        /// </summary>
        Install,

        /// <summary>
        /// Downloads Updates automatically
        /// </summary>
        Download,

        /// <summary>
        /// Only checks and notifies the user of updates
        /// </summary>
        Notify,

        /// <summary>No automatic checking</summary>
        Never
    }

    #endregion

    #endregion

    #region Locale Classes

    /// <summary>
    /// Contains a string indicating the language and a value
    /// </summary>
    public class LocaleString : INotifyPropertyChanged
    {
        /// <summary>an ISO language code</summary>
        [XmlAttribute("lang", Namespace = "")]
        public string Lang { get; set; }

        /// <summary>The value of the string</summary>
        [XmlAttribute("value", Namespace = "")]
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
    [XmlRoot("application", Namespace = "http://sevenupdate.sourceforge.net")]
    public class SUA : INotifyPropertyChanged
    {
        /// <summary>The application name</summary>
        [XmlElement("name")]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// Gets or Sets release information about the update
        /// </summary>
        [XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The directory where the application is installed
        /// </summary>
        [XmlAttribute("directory")]
        public string Directory { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [XmlAttribute("is64Bit")]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// Gets or Sets a value Indicating if the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        [XmlAttribute("isEnabled")]
        public bool IsEnabled { get; set; }

        /// 
        /// <summary>
        /// The publisher of the application
        /// </summary>
        [XmlElement("publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The SUI file of the application
        /// </summary>
        [XmlAttribute("source")]
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
    public enum ShortcutAction
    {
        /// <summary>
        /// Adds or updates a shortcut
        /// </summary>
        Add,

        /// <summary>
        /// Deletes a shortcut
        /// </summary>
        Delete

    }
    /// <summary>
    /// The action to perform on the file
    /// </summary>
    public enum FileAction
    {
        /// <summary>
        /// Compares information only, does not update the file
        /// </summary>
        Compare,

        /// <summary>Update the file</summary>
        Update,

        /// <summary>
        /// Updates and executes the file
        /// </summary>
        UpdateAndExecute,

        /// <summary>
        /// Updates and registers the file
        /// </summary>
        UpdateAndRegister,

        /// <summary>
        /// Unregisters dll and deletes the file
        /// </summary>
        UnregisterAndDelete,

        /// <summary>
        /// Executes then deletes the file
        /// </summary>
        ExecuteAndDelete,

        /// <summary>Deletes the file</summary>
        Delete
    }

    /// <summary>
    /// Contains the UpdateType of the update
    /// </summary>
    public enum Importance
    {
        /// <summary>Important update</summary>
        Important,

        /// <summary>Locale or language</summary>
        Locale,

        /// <summary>Optional update</summary>
        Optional,

        /// <summary>Recommended update</summary>
        Recommended
    }

    /// <summary>
    /// The current status of the update
    /// </summary>
    public enum UpdateStatus
    {
        /// <summary>
        /// Indicates that the update installation failed
        /// </summary>
        Failed,

        /// <summary>
        /// Indicates that the update is hidden
        /// </summary>
        Hidden,

        /// <summary>
        /// Indicates that the update is visible
        /// </summary>
        Visible,

        /// <summary>
        /// Indicates that the update installation succeeded
        /// </summary>
        Successful
    }

    /// <summary>
    /// Contains the Actions you can perform to the registry
    /// </summary>
    public enum RegistryAction
    {
        /// <summary>
        /// Adds a registry entry to the machine
        /// </summary>
        Add,
        /// <summary>
        /// Deletes a registry key on the machine
        /// </summary>
        DeleteKey,
        /// <summary>
        /// Deletes a value of a registry key on the machine
        /// </summary>
        DeleteValue
    }

    #endregion

    #region Classes

    /// <summary>Application info</summary>
    [XmlType(AnonymousType = true), XmlRoot("application", Namespace = "http://sevenupdate.sourceforge.net")]
    public class SUI : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The application main directory, usually in Program Files
        /// </summary>
        [XmlAttribute("directory")]
        public string Directory { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [XmlAttribute("is64Bit")]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// The company or developer of the Application
        /// </summary>
        [XmlElement("publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The url of the company or developer
        /// </summary>
        [XmlAttribute("publisherUrl")]
        public string PublisherUrl { get; set; }

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [XmlAttribute("helpUrl")]
        public string HelpUrl { get; set; }

        #endregion

        #region Required Sub-Properties

        /// <summary>
        /// Collection of updates for the application
        /// </summary>
        [XmlElement("update")]
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
    [XmlType("update", Namespace = "http://sevenupdate.sourceforge.net", AnonymousType = true)]
    public class Update : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>The name of the update</summary>
        [XmlElement("name")]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// Release information about the update
        /// </summary>
        [XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The default download directory where the updates files are stored
        /// </summary>
        [XmlAttribute("downloadUrl")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// The update type of the update: Important, Recommended, Optional, Locale, Installation.
        /// </summary>
        [XmlAttribute("importance")]
        public Importance Importance { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [XmlAttribute("releaseDate")]
        public string ReleaseDate { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// The information/change log url of the update: Optional
        /// </summary>
        [XmlAttribute("infoUrl")]
        public string InfoUrl { get; set; }

        /// <summary>
        /// The Software License Agreement Url
        /// </summary>
        [XmlAttribute("licenseUrl")]
        public string LicenseUrl { get; set; }

        #endregion

        #region Optional SubProperties

        /// <summary>
        /// The files of the current update
        /// </summary>
        [XmlElement("file")]
        public ObservableCollection<UpdateFile> Files { get; set; }

        /// <summary>
        /// The registry entries of the current update
        /// </summary>
        [XmlElement("registryItem")]
        public ObservableCollection<RegistryItem> RegistryItems { get; set; }

        /// <summary>
        /// The shortcuts to create for the update
        /// </summary>
        [XmlElement("shortcut")]
        public ObservableCollection<Shortcut> Shortcuts { get; set; }

        #endregion

        #region UI Properties

        /// <summary>
        /// Gets or Sets a value Indicating if the update is selected (not used in the SDK)
        /// </summary>
        [XmlIgnore]
        public bool Selected { get; set; }

        /// <summary>
        /// The download size of the update in bytes, not used by the SDK
        /// </summary>
        [XmlIgnore]
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
    [XmlType("file", Namespace = "http://sevenupdate.sourceforge.net", AnonymousType = true)]
    public class UpdateFile : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The action to perform on a file
        /// </summary>
        [XmlAttribute("action")]
        public FileAction Action { get; set; }

        /// <summary>
        /// The source location of the current file with the filename
        /// </summary>
        [XmlAttribute("source")]
        public string Source { get; set; }

        /// <summary>
        /// The destination location of the current file with the filename
        /// </summary>
        [XmlAttribute("destination")]
        public string Destination { get; set; }

        /// <summary>
        /// The SHA1 hash of the current file
        /// </summary>
        [XmlAttribute("hash")]
        public string Hash { get; set; }

        /// <summary>File size in bytes</summary>
        [XmlAttribute("size")]
        public ulong Size { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Command line arguments for the file
        /// </summary>
        [XmlAttribute("args")]
        public string Args { get; set; }

        /// <summary>
        /// Specifies if the file is optional, will get updated if installed on system, otherwise it's ignored.
        /// </summary>
        [XmlAttribute("optional")]
        public bool Optional { get; set; }

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
    [XmlType("registryItem", Namespace = "http://sevenupdate.sourceforge.net", AnonymousType = true)]
    public class RegistryItem : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The action to perform to the registry item
        /// </summary>
        [XmlAttribute("action")]
        public RegistryAction Action { get; set; }

        /// <summary>
        /// The hive of the current registry item
        /// </summary>
        [XmlAttribute("hive")]
        public RegistryHive Hive { get; set; }

        /// <summary>
        /// The Key path of the current registry item
        /// </summary>
        [XmlAttribute("key")]
        public string Key { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Name of the Value in the specified key
        /// </summary>
        [XmlAttribute("keyValue")]
        public string KeyValue { get; set; }

        /// <summary>
        /// The ValueKind of the value in the specified key
        /// </summary>
        [XmlAttribute("valueKind")]
        public RegistryValueKind ValueKind { get; set; }

        /// <summary>
        /// The data of the value in the specified key
        /// </summary>
        [XmlAttribute("data")]
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
    [XmlType("shortcut", Namespace = "http://sevenupdate.sourceforge.net", AnonymousType = true)]
    public class Shortcut : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The location of where the shortcut is to be stored.
        /// </summary>
        [XmlAttribute("location")]
        public string Location { get; set; }

        /// <summary>
        /// The action to peform on the shortcut
        /// </summary>
        [XmlAttribute("action")]
        public ShortcutAction Action { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Any arguments to be used with the shortcut
        /// </summary>
        [XmlAttribute("arguments")]
        public string Arguments { get; set; }

        /// <summary>
        /// Description of the shortcut
        /// </summary>
        [XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The full path to the icon or exe containing an icon
        /// </summary>
        [XmlAttribute("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// The full path of the target to the shortcut.
        /// </summary>
        [XmlAttribute("target")]
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
    [XmlType(AnonymousType = true), XmlRoot("update", Namespace = "http://sevenupdate.sourceforge.net")]
    public class SUH : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>The name of the update</summary>
        [XmlElement("name")]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// A description of the update, usually list new features or changes the update brings.
        /// </summary>
        [XmlElement("description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The update type of the update: Critical, Recommended, Optional, Locale
        /// </summary>
        [XmlAttribute("importance")]
        public Importance Importance { get; set; }

        /// <summary>
        /// The current status of the update
        /// </summary>
        [XmlAttribute("status")]
        public UpdateStatus Status { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [XmlAttribute("releaseDate")]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// The full size of the update
        /// </summary>
        [XmlAttribute("size")]
        public ulong Size { get; set; }

        /// <summary>
        /// The Publisher of the update/application
        /// </summary>
        [XmlElement("publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The website of the publisher
        /// </summary>
        [XmlAttribute("publisherUrl")]
        public string PublisherUrl { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [XmlAttribute("helpUrl")]
        public string HelpUrl { get; set; }

        /// <summary>
        /// The information/change log url of the update: Optional
        /// </summary>
        [XmlAttribute("infoUrl")]
        public string InfoUrl { get; set; }

        /// <summary>
        /// The date when the update was installed
        /// </summary>
        [XmlAttribute("installDate")]
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