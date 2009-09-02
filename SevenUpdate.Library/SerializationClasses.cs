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
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Win32;

#endregion

namespace SevenUpdate
{

    #region Application Settings

    #region Struct

    /// <summary>
    /// Configuration options
    /// </summary>
    [DataContract(Name = "Config", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (AutoUpdateOption))]
    public struct Config
    {
        /// <summary>
        /// Specifies which update setting Seven Update should use
        /// </summary>
        [DataMember(Name = "Auto")] public AutoUpdateOption AutoOption { get; set; }

        /// <summary>
        /// Specifes if recommended updates should be included when download/install
        /// </summary>\
        [DataMember(Name = "IncludeRecommended")] public bool IncludeRecommended { get; set; }

        /// <summary>
        /// Specifies the langauge Seven Update uses
        /// </summary>
        [DataMember(Name = "Locale")] public string Locale { get; set; }
    }

    #endregion

    #region Enums

    /// <summary>
    /// Automatic Update option Seven Update can use
    /// </summary>
    [DataContract(Name = "Auto", Namespace = "http://sevenupdate.sourceforge.net")]
    public enum AutoUpdateOption
    {
        /// <summary>
        /// Download and Installs updates automatically
        /// </summary>
        [EnumMember] Install,

        /// <summary>
        /// Downloads Updates automatically
        /// </summary>
        [EnumMember] Download,

        /// <summary>
        /// Only checks and notifies the user of updates
        /// </summary>
        [EnumMember] Notify,

        /// <summary>
        /// No automatic checking
        /// </summary>
        [EnumMember] Never
    }

    #endregion

    #endregion

    #region Locale Classes

    [DataContract(Namespace = "http://sevenupdate.sourceforge.net")]
    public class LocaleString
    {
        /// <summary>
        /// an ISO language code
        /// </summary>
        [DataMember(IsRequired = true)] public string lang { get; set; }

        /// <summary>
        /// The value of the string
        /// </summary>
        [DataMember(IsRequired = true)] public string Value { get; set; }
    }

    #endregion

    #region SUA File

    /// <summary>
    /// Seven Update Application information
    /// </summary>
    [DataContract(Name = "Application", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (LocaleString))]
    public class SUA : INotifyPropertyChanged
    {
        /// <summary>
        /// The application name
        /// </summary>
        [DataMember(Name = "ApplicationName", IsRequired = true)] public ObservableCollection<LocaleString> ApplicationName { get; set; }

        /// <summary>
        /// Gets or Sets release information about the update
        /// </summary>
        [DataMember(Name = "Description")] public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The directory where the application is installed
        /// </summary>
        [DataMember(Name = "Directory", IsRequired = true)] public string Directory { get; set; }

        /// <summary>
        /// Indicates if the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        [DataMember(Name = "IsEnabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [DataMember(Name = "Is64Bit")] public bool Is64Bit { get; set; }

        /// <summary>
        /// <summary>
        /// The publisher of the application
        /// </summary>
        [DataMember(Name = "Publisher")] public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The SUI file of the application
        /// </summary>
        [DataMember(Name = "Source", IsRequired = true)] public string Source { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        //[OnDeserialized]
        //internal void OnDeserializedMethod(StreamingContext context)
        //{
        //    if (Publisher != null) Publisher = Publisher.ToList();

        //    if (Description != null) Description = Description.ToList();

        //    if (ApplicationName != null) ApplicationName = ApplicationName.ToList();
        //}

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    #endregion

    #region SUI File

    #region Enum

    /// <summary>
    /// The action to perform on the file
    /// </summary>
    [DataContract(Name = "Action", Namespace = "http://sevenupdate.sourceforge.net")]
    public enum FileAction
    {
        /// <summary>
        /// Update the file
        /// </summary>
        [EnumMember] Update,

        /// <summary>
        /// Updates and executes the file
        /// </summary>
        [EnumMember] UpdateAndExecute,

        /// <summary>
        /// Updates and registers the file
        /// </summary>
        [EnumMember] UpdateAndRegister,

        /// <summary>
        /// Unregisters dll and deletes the file
        /// </summary>
        [EnumMember] UnregisterAndDelete,

        /// <summary>
        /// Executes then deletes the file
        /// </summary>
        [EnumMember] ExecuteAndDelete,

        /// <summary>
        /// Deletes the file
        /// </summary>
        [EnumMember] Delete
    }

    /// <summary>
    /// Contains the UpdateType of the update
    /// </summary>
    [DataContract(Name = "Importance", Namespace = "http://sevenupdate.sourceforge.net")]
    public enum Importance
    {
        /// <summary>
        /// Important update
        /// </summary>
        [EnumMember] Important,

        /// <summary>
        /// Locale or language
        /// </summary>
        [EnumMember] Locale,

        /// <summary>
        /// Optional update
        /// </summary>
        [EnumMember] Optional,

        /// <summary>
        /// Recommended update
        /// </summary>
        [EnumMember] Recommended
    }

    /// <summary>
    /// The current status of the update
    /// </summary>
    [DataContract(Name = "Status", Namespace = "http://sevenupdate.sourceforge.net")]
    public enum UpdateStatus
    {
        /// <summary>
        /// Indicates that the update installation failed
        /// </summary>
        [EnumMember] Failed,

        /// <summary>
        /// Indicates that the update is hidden
        /// </summary>
        [EnumMember] Hidden,

        /// <summary>
        /// Indicates that the update is visible
        /// </summary>
        [NonSerialized] Visible,

        /// <summary>
        /// Indicates that the update installation succeeded
        /// </summary>
        [EnumMember] Successful
    }

    /// <summary>
    /// Contains the Actions you can perform to the registry
    /// </summary>
    [DataContract(Name = "Action", Namespace = "http://sevenupdate.sourceforge.net")]
    public enum RegistryAction
    {
        /// <summary>
        /// Adds a registry entry to the machine
        /// </summary>
        [EnumMember] Add,
        /// <summary>
        /// Deletes a registry key on the machine
        /// </summary>
        [EnumMember] DeleteKey,
        /// <summary>
        /// Deletes a value of a registry key on the machine
        /// </summary>
        [EnumMember] DeleteValue
    }

    #endregion

    #region Classes

    /// <summary>
    /// Application info
    /// </summary>
    [DataContract(Name = "Application", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (LocaleString))]
    [KnownType(typeof (Update))]
    public class SUI : INotifyPropertyChanged
    {
        /// <summary>
        /// The application main directory, usually in Program Files
        /// </summary>
        [DataMember(Name = "Directory", IsRequired = true)] public string Directory { get; set; }

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [DataMember(Name = "HelpUrl")] public string HelpUrl { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [DataMember(Name = "Is64Bit")] public bool Is64Bit { get; set; }

        /// <summary>
        /// The company or developer of the Application
        /// </summary>
        [DataMember(Name = "Publisher")] public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The Wwebsite of the company or developer
        /// </summary>
        [DataMember(Name = "PublisherUrl")] public string PublisherUrl { get; set; }

        /// <summary>
        /// ObservableCollection of updates for the application
        /// </summary>
        [DataMember(Name = "Update", IsRequired = true)] public ObservableCollection<Update> Updates { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        //[OnDeserialized] internal void OnDeserializedMethod(StreamingContext context)
        //{
        //    if (Publisher != null) Publisher = Publisher.ToList();

        //    if (Updates != null) Updates = Updates.ToList();
        //}

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// A shortcut to be created within an update
    /// </summary>
    [DataContract(Name = "Shortcut", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (LocaleString))]
    public class Shortcut : INotifyPropertyChanged
    {
        /// <summary>
        /// Any arguments to be used with the shortcut
        /// </summary>
        [DataMember(Name = "Arguments")] public string Arguments { get; set; }

        /// <summary>
        /// Description of the shortcut
        /// </summary>
        [DataMember(Name = "Description")] public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The fullpath to the icon or exe containing an icon
        /// </summary>
        [DataMember(Name = "Icon")] public string Icon { get; set; }

        /// <summary>
        /// The location of where the shortcut is to be stored.
        /// </summary>
        [DataMember(Name = "Location", IsRequired = true)] public string Location { get; set; }

        /// <summary>
        /// The fullpath of the target to the shortcut.
        /// </summary>
        [DataMember(Name = "Target", IsRequired = true)] public string Target { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        //[OnDeserialized] internal void OnDeserializedMethod(StreamingContext context)
        //{
        //    if (Description != null) Description = Description.ToList();
        //}

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// A registry entry within an update
    /// </summary>
    [DataContract(Name = "RegistryItem", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (RegistryAction))]
    public class RegistryItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The action to perform to the registry item
        /// </summary>
        [DataMember(Name = "Action", IsRequired = true)] public RegistryAction Action { get; set; }

        /// <summary>
        ///  The data of the value in the specified key
        /// </summary>
        [DataMember(Name = "Data")] public string Data { get; set; }

        /// <summary>
        /// The hive of the current registry item
        /// </summary>
        [DataMember(Name = "Hive")] public RegistryHive Hive { get; set; }

        /// <summary>
        /// The Keypath of the current registry item
        /// </summary>
        [DataMember(Name = "Key", IsRequired = true)] public string Key { get; set; }

        /// <summary>
        /// The ValueKind of the value in the specified key
        /// </summary>
        [DataMember(Name = "ValueKind")] public RegistryValueKind ValueKind { get; set; }

        /// <summary>
        /// Name of the Value in the specified key
        /// </summary>
        [DataMember(Name = "KeyValue")] public string KeyValue { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Information about a file within an update
    /// </summary>
    [DataContract(Name = "File", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (FileAction))]
    public class UpdateFile : INotifyPropertyChanged
    {
        /// <summary>
        /// The action to perform on a file
        /// </summary>
        [DataMember(Name = "Action", IsRequired = true)] public FileAction Action { get; set; }

        /// <summary>
        /// Commandline arguments for the file
        /// </summary>
        [DataMember(Name = "Arguments")] public string Arguments { get; set; }

        /// <summary>
        /// The destination location of the current file with the filename
        /// </summary>
        [DataMember(Name = "Destination", IsRequired = true)] public string Destination { get; set; }

        /// <summary>
        /// The SHA1 hash of the current file
        /// </summary>
        [DataMember(Name = "Hash", IsRequired = true)] public string Hash { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        [DataMember(Name = "Size")] public ulong Size { get; set; }

        /// <summary>
        /// The source location of the current file with the filename
        /// </summary>
        [DataMember(Name = "Source", IsRequired = true)] public string Source { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Information on how to install a program update
    /// </summary>
    [DataContract(Name = "Update", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (LocaleString))]
    [KnownType(typeof (UpdateFile))]
    [KnownType(typeof (Importance))]
    public class Update : INotifyPropertyChanged
    {
        /// <summary>
        /// Release information about the update
        /// </summary>
        [DataMember(Name = "Description")] public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The default download directory where the updates files are stored
        /// </summary>
        [DataMember(Name = "DownloadDirectory", IsRequired = true)] public string DownloadDirectory { get; set; }

        /// <summary>
        /// The files of the current update
        /// </summary>
        [DataMember(Name = "File")] public ObservableCollection<UpdateFile> Files { get; set; }

        /// <summary>
        /// The update type of the update: Important, Recommended, Optional, Locale, Installation.
        /// </summary>
        [DataMember(Name = "Importance", IsRequired = true)] public Importance Importance { get; set; }

        /// <summary>
        /// The information/changelog url of the update: Optional
        /// </summary>
        [DataMember(Name = "InfoUrl")] public string InfoUrl { get; set; }

        /// <summary>
        /// The Software License Agreement Url
        /// </summary>
        [DataMember(Name = "LicenseUrl")] public string LicenseUrl { get; set; }

        /// <summary>
        /// The registry entries of the current update
        /// </summary>
        [DataMember(Name = "RegistryItem")] public ObservableCollection<RegistryItem> RegistryItems { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [DataMember(Name = "ReleaseDate", IsRequired = true)] public string ReleaseDate { get; set; }

        /// <summary>
        /// Indicates if the update is selected (not used in the SDK)
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// The download size of the update in bytes, not used by the SDK
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// The name of the update
        /// </summary>
        [DataMember(Name = "Name", IsRequired = true)] public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// The shortcuts to create for the update
        /// </summary>
        [DataMember(Name = "Shortcut")] public ObservableCollection<Shortcut> Shortcuts { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        //[OnDeserialized] 
        //internal void OnDeserializedMethod(StreamingContext context)
        //{
        //    if (RegistryItems != null) RegistryItems = RegistryItems.ToList();

        //    if (Shortcuts != null) Shortcuts = Shortcuts.ToList();

        //    if (Files != null) Files = Files.ToList();

        //    if (Description != null) Description = Description.ToList();

        //    if (Name != null) Name = Name.ToList();
        //}

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    #endregion

    #endregion

    #region History and Hidden Updates Files

    /// <summary>
    /// Information about an update, used by History and Hidden Updates. Not used by the SDK
    /// </summary>
    [DataContract(Name = "Update", Namespace = "http://sevenupdate.sourceforge.net")]
    [KnownType(typeof (Importance))]
    [KnownType(typeof (LocaleString))]
    [KnownType(typeof (UpdateStatus))]
    public class SUH : INotifyPropertyChanged
    {
        /// <summary>
        /// A description of the update, usually list new features or changes the update brings.
        /// </summary>
        [DataMember(Name = "Description")] public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [DataMember(Name = "HelpUrl")] public string HelpUrl { get; set; }

        /// <summary>
        /// The update type of the update: Critical, Recommended, Optional, Locale
        /// </summary>
        [DataMember(Name = "Importance", IsRequired = true)] public Importance Importance { get; set; }

        /// <summary>
        /// The information/changelog url of the update: Optional
        /// </summary>
        [DataMember(Name = "InfoUrl")] public string InfoUrl { get; set; }

        /// <summary>
        /// The date when the update was installed
        /// </summary>
        [DataMember(Name = "InstallDate")] public string InstallDate { get; set; }

        /// <summary>
        /// The Publisher of the update/application
        /// </summary>
        [DataMember(Name = "Publisher")] public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The website of the publisher
        /// </summary>
        [DataMember(Name = "PublisherUrl")] public string PublisherUrl { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [DataMember(Name = "ReleaseDate", IsRequired = true)] public string ReleaseDate { get; set; }

        /// <summary>
        /// The full size of the update
        /// </summary>
        [DataMember(Name = "Size")] public ulong Size { get; set; }

        /// <summary>
        /// The current status of the update
        /// </summary>
        [DataMember(Name = "Status", IsRequired = true)] public UpdateStatus Status { get; set; }

        /// <summary>
        /// The name of the update
        /// </summary>
        [DataMember(Name = "Name", IsRequired = true)] public ObservableCollection<LocaleString> Name { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        //[OnDeserialized] internal void OnDeserializedMethod(StreamingContext context)
        //{
        //    if (Publisher != null) Publisher = Publisher.ToList();

        //    if (Description != null) Description = Description.ToList();

        //    if (Name != null) Name = Name.ToList();
        //}

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    #endregion
}