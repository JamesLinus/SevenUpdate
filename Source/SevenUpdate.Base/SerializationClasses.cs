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
using Microsoft.Win32;
using ProtoBuf;

#endregion

namespace SevenUpdate.Base
{

    #region Application Settings

    /// <summary>Configuration options</summary>
    [ProtoContract]
    public class Config
    {
        /// <summary>
        /// Specifies which update setting Seven Update should use
        /// </summary>
        [ProtoMember(1)]
        public AutoUpdateOption AutoOption { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating if Seven Update is to included recommended updates when automatically downloading updates
        /// </summary>
        [ProtoMember(2)]
        public bool IncludeRecommended { get; set; }

        /// <summary>
        /// Specifies the language Seven Update uses
        /// </summary>
        [ProtoMember(3)]
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
        [ProtoMember(1)] Install,

        /// <summary>
        /// Downloads Updates automatically
        /// </summary>
        [ProtoMember(2)] Download,

        /// <summary>
        /// Only checks and notifies the user of updates
        /// </summary>
        [ProtoMember(3)] Notify,

        /// <summary>No automatic checking</summary>
        [ProtoMember(4)] Never
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
        [ProtoMember(1)]
        public string Lang { get; set; }

        /// <summary>The value of the string</summary>
        [ProtoMember(2)]
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
    [ProtoContract]
    public class Sua : INotifyPropertyChanged
    {
        /// <summary>The application name</summary>
        [ProtoMember(1)]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// Gets or Sets release information about the update
        /// </summary>
        [ProtoMember(2)]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The directory where the application is installed
        /// </summary>
        [ProtoMember(3)]
        public string Directory { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [ProtoMember(4)]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// Gets or Sets a value Indicating if the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        [ProtoMember(5)]
        public bool IsEnabled { get; set; }

        /// 
        /// <summary>
        /// The publisher of the application
        /// </summary>
        [ProtoMember(6)]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The SUI file of the application
        /// </summary>
        [ProtoMember(7)]
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
        [ProtoMember(1)] Add,

        /// <summary>
        /// Deletes a shortcut
        /// </summary>
        [ProtoMember(2)] Delete,

        /// <summary>
        /// Updates a shortcut only if it exists
        /// </summary>
        [ProtoMember(3)] Update
    }

    /// <summary>
    /// The action to perform on the file
    /// </summary>
    [ProtoContract]
    public enum FileAction
    {
        /// <summary>Updates a file</summary>
        [ProtoMember(1)] Update,

        /// <summary>Updates a file, only if it exist</summary>
        [ProtoMember(2)] UpdateIfExist,

        /// <summary>Updates a file, then registers the dll</summary>
        [ProtoMember(3)] UpdateThenRegister,

        /// <summary>Updates a file, then executes it</summary>
        [ProtoMember(4)] UpdateThenExecute,

        /// <summary>Compares a file, but does not update it</summary>
        [ProtoMember(5)] CompareOnly,

        /// <summary>Executes a file, can be on system or be downloaded</summary>
        [ProtoMember(6)] Execute,

        /// <summary>
        /// Deletes a file
        /// </summary>
        [ProtoMember(7)] Delete,

        /// <summary>
        /// Executes a file, then deletes it
        /// </summary>
        [ProtoMember(8)] ExecuteThenDelete,

        /// <summary>
        /// Unregisteres a dll, then deletes it
        /// </summary>
        [ProtoMember(9)] UnregisterThenDelete,
    }

    /// <summary>
    /// Contains the UpdateType of the update
    /// </summary>\
    [ProtoContract]
    public enum Importance
    {
        /// <summary>Important update</summary>
        [ProtoMember(1)] Important,

        /// <summary>Locale or language</summary>
        [ProtoMember(2)] Locale,

        /// <summary>Optional update</summary>
        [ProtoMember(3)] Optional,

        /// <summary>Recommended update</summary>
        [ProtoMember(4)] Recommended
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
        [ProtoMember(1)] Failed,

        /// <summary>
        /// Indicates that the update is hidden
        /// </summary>
        [ProtoMember(2)] Hidden,

        /// <summary>
        /// Indicates that the update is visible
        /// </summary>
        [ProtoMember(3)] Visible,

        /// <summary>
        /// Indicates that the update installation succeeded
        /// </summary>
        [ProtoMember(4)] Successful
    }

    /// <summary>
    /// Contains the Actions you can perform to the registry
    /// </summary>
    public enum RegistryAction
    {
        /// <summary>
        /// Adds a registry entry to the machine
        /// </summary>
        [ProtoMember(1)] Add,
        /// <summary>
        /// Deletes a registry key on the machine
        /// </summary>
        [ProtoMember(2)] DeleteKey,
        /// <summary>
        /// Deletes a value of a registry key on the machine
        /// </summary>
        [ProtoMember(3)] DeleteValue
    }

    #endregion

    #region Classes

    /// <summary>Application info</summary>
    [ProtoContract]
    public class Sui : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The application main directory, usually in Program Files
        /// </summary>
        [ProtoMember(1)]
        public string Directory { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [ProtoMember(2)]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// The company or developer of the Application
        /// </summary>
        [ProtoMember(3)]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The url of the company or developer
        /// </summary>
        [ProtoMember(4)]
        public string PublisherUrl { get; set; }

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [ProtoMember(5)]
        public string HelpUrl { get; set; }

        #endregion

        #region Required Sub-Properties

        /// <summary>
        /// Collection of updates for the application
        /// </summary>
        [ProtoMember(6)]
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
    [ProtoContract]
    public class Update : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>The name of the update</summary>
        [ProtoMember(1)]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// Release information about the update
        /// </summary>
        [ProtoMember(2)]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The default download directory where the updates files are stored
        /// </summary>
        [ProtoMember(3)]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// The update type of the update: Important, Recommended, Optional, Locale, Installation.
        /// </summary>
        [ProtoMember(4)]
        public Importance Importance { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [ProtoMember(5)]
        public string ReleaseDate { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// The information/change log url of the update: Optional
        /// </summary>
        [ProtoMember(6)]
        public string InfoUrl { get; set; }

        /// <summary>
        /// The Software License Agreement Url
        /// </summary>
        [ProtoMember(7)]
        public string LicenseUrl { get; set; }

        #endregion

        #region Optional SubProperties

        /// <summary>
        /// The files of the current update
        /// </summary>
        [ProtoMember(8)]
        public ObservableCollection<UpdateFile> Files { get; set; }

        /// <summary>
        /// The registry entries of the current update
        /// </summary>
        [ProtoMember(9)]
        public ObservableCollection<RegistryItem> RegistryItems { get; set; }

        /// <summary>
        /// The shortcuts to create for the update
        /// </summary>
        [ProtoMember(10)]
        public ObservableCollection<Shortcut> Shortcuts { get; set; }

        #endregion

        #region UI Properties

        /// <summary>
        /// Gets or Sets a value Indicating if the update is selected (not used in the SDK)
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// The download size of the update in bytes, not used by the SDK
        /// </summary>
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
    [ProtoContract]
    public class UpdateFile : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The action to perform on a file
        /// </summary>
        [ProtoMember(1)]
        public FileAction Action { get; set; }

        /// <summary>
        /// The source location of the current file with the filename
        /// </summary>
        [ProtoMember(2)]
        public string Source { get; set; }

        /// <summary>
        /// The destination location of the current file with the filename
        /// </summary>
        [ProtoMember(3)]
        public string Destination { get; set; }

        /// <summary>
        /// The SHA1 hash of the current file
        /// </summary>
        [ProtoMember(4)]
        public string Hash { get; set; }

        /// <summary>File size in bytes</summary>
        [ProtoMember(5)]
        public ulong Size { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Command line arguments for the file
        /// </summary>
        [ProtoMember(6)]
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
    [ProtoContract]
    public class RegistryItem : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The action to perform to the registry item
        /// </summary>
        [ProtoMember(1)]
        public RegistryAction Action { get; set; }

        /// <summary>
        /// The hive of the current registry item
        /// </summary>
        [ProtoMember(2)]
        public RegistryHive Hive { get; set; }

        /// <summary>
        /// The Key path of the current registry item
        /// </summary>
        [ProtoMember(3)]
        public string Key { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Name of the Value in the specified key
        /// </summary>
        [ProtoMember(4)]
        public string KeyValue { get; set; }

        /// <summary>
        /// The ValueKind of the value in the specified key
        /// </summary>
        [ProtoMember(5)]
        public RegistryValueKind ValueKind { get; set; }

        /// <summary>
        /// The data of the value in the specified key
        /// </summary>
        [ProtoMember(6)]
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
    [ProtoContract]
    public class Shortcut : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>
        /// The location of where the shortcut is to be stored.
        /// </summary>
        [ProtoMember(1)]
        public string Location { get; set; }

        /// <summary>
        /// The action to peform on the shortcut
        /// </summary>
        [ProtoMember(2)]
        public ShortcutAction Action { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// Any arguments to be used with the shortcut
        /// </summary>
        [ProtoMember(3)]
        public string Arguments { get; set; }

        /// <summary>
        /// Description of the shortcut
        /// </summary>
        [ProtoMember(4)]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The full path to the icon or exe containing an icon
        /// </summary>
        [ProtoMember(5)]
        public string Icon { get; set; }

        /// <summary>
        /// The full path of the target to the shortcut.
        /// </summary>
        [ProtoMember(6)]
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
    [ProtoContract]
    public class Suh : INotifyPropertyChanged
    {
        #region Required Properties

        /// <summary>The name of the update</summary>
        [ProtoMember(1)]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// A description of the update, usually list new features or changes the update brings.
        /// </summary>
        [ProtoMember(2)]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The update type of the update: Critical, Recommended, Optional, Locale
        /// </summary>
        [ProtoMember(3)]
        public Importance Importance { get; set; }

        /// <summary>
        /// The current status of the update
        /// </summary>
        [ProtoMember(4)]
        public UpdateStatus Status { get; set; }

        /// <summary>
        /// The date when the update was released
        /// </summary>
        [ProtoMember(5)]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// The full size of the update
        /// </summary>
        [ProtoMember(6)]
        public ulong Size { get; set; }

        /// <summary>
        /// The Publisher of the update/application
        /// </summary>
        [ProtoMember(7)]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The website of the publisher
        /// </summary>
        [ProtoMember(8)]
        public string PublisherUrl { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// The help url of the update: Optional
        /// </summary>
        [ProtoMember(9)]
        public string HelpUrl { get; set; }

        /// <summary>
        /// The information/change log url of the update: Optional
        /// </summary>
        [ProtoMember(10)]
        public string InfoUrl { get; set; }

        /// <summary>
        /// The date when the update was installed
        /// </summary>
        [ProtoMember(11)]
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