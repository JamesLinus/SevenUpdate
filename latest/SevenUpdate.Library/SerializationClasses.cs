using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SevenUpdate
{

    #region Application Settings

    #region Struct

    /// <summary>
    /// Configuration options
    /// </summary>
    [XmlRoot("Settings")]
    public struct Config
    {
        /// <summary>
        /// Specifies which update setting Seven Update should use
        /// </summary>
        [XmlElementAttribute("Auto")]
        public AutoUpdateOption AutoOption { get; set; }

        /// <summary>
        /// Specifes if recommended updates should be included when download/install
        /// </summary>\
        [XmlElementAttribute("IncludeRecommended")]
        public bool IncludeRecommended { get; set; }

        /// <summary>
        /// Specifies the langauge Seven Update uses
        /// </summary>
        [XmlElementAttribute("Locale")]
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

        /// <summary>
        /// No automatic checking
        /// </summary>
        Never
    }

    #endregion

    #endregion

    #region Locale Classes

    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class LocaleString
    {
        /// <summary>
        /// an ISO language code
        /// </summary>
        [XmlAttribute(DataType = "language")]
        public string lang { get; set; }

        /// <summary>
        /// The value of the string
        /// </summary>
        [XmlAttribute()]
        public string Value { get; set; }

    }

    #endregion

    #region SUA File

    /// <summary>
    /// Seven Update Application information
    /// </summary>
    [XmlRoot("Application")]
    public class SUA : INotifyPropertyChanged
    {
        /// <summary>
        /// The application name
        /// </summary>
        [XmlElementAttribute("ApplicationName")]
        public ObservableCollection<LocaleString> ApplicationName { get; set; }

        /// <summary>
        /// Gets or Sets release information about the update
        /// </summary>
        [XmlElementAttribute("Description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The directory where the application is installed
        /// </summary>
        [XmlAttribute("Directory")]
        public string Directory { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [XmlAttribute("Is64Bit")]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// Indicates if the SUA is enabled with Seven Update (SDK does not use this value)
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// <summary>
        /// The publisher of the application
        /// </summary>
        [XmlElementAttribute("Publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The SUI file of the application
        /// </summary>
        [XmlAttribute("Source")]
        public string Source { get; set; }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {

                handler(this, new PropertyChangedEventArgs(name));

            }

        }

        #endregion
    }

    #endregion

    #region SUI File

    #region Enum

    /// <summary>
    /// The action to perform on the file
    /// </summary>
    public enum FileAction
    {
        /// <summary>
        /// Update the file
        /// </summary>
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

        /// <summary>
        /// Deletes the file
        /// </summary>
        Delete
    }

    /// <summary>
    /// Contains the UpdateType of the update
    /// </summary>
    public enum Importance
    {
        /// <summary>
        /// Important update
        /// </summary>
        Important,

        /// <summary>
        /// Locale or language
        /// </summary>
        Locale,

        /// <summary>
        /// Optional update
        /// </summary>
        Optional,

        /// <summary>
        /// Recommended update
        /// </summary>
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
        Add, DeleteKey, DeleteValue
    }

    #endregion

    #region Classes

    /// <summary>
    /// Application info
    /// </summary>
    [XmlRoot("Application")]
    public class Application : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or Sets the application main directory, usually in Program Files
        /// </summary>
        [XmlAttribute("Directory")]
        public string Directory { get; set; }

        /// <summary>
        /// Gets or Sets the help url of the update: Optional
        /// </summary>
        [XmlAttribute("HelpUrl")]
        public string HelpUrl { get; set; }

        /// <summary>
        /// Gets or Sets the application name in which the update applies too
        /// </summary>
        [XmlElementAttribute("Name")]
        public ObservableCollection<LocaleString> Name { get; set; }

        /// <summary>
        /// Specifies if the application is 64 bit
        /// </summary>
        [XmlAttribute("Is64Bit")]
        public bool Is64Bit { get; set; }

        /// <summary>
        /// The company or developer of the Application
        /// </summary>
        [XmlElementAttribute("Publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The Website of the company or developer
        /// </summary>
        [XmlAttribute("PublisherUrl")]
        public string PublisherUrl { get; set; }

        /// <summary>
        /// The total size of the update in bytes, not used by the SDK
        /// </summary>
        [XmlAttribute("Size")]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or Sets the applications application
        /// </summary>
        [XmlElementAttribute("Update")]
        public ObservableCollection<Update> Updates { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

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
    public class Shortcut : INotifyPropertyChanged
    {
        /// <summary>
        /// Any arguments to be used with the shortcut
        /// </summary>
        [XmlAttribute("Arguments")]
        public string Arguments { get; set; }

        /// <summary>
        /// Description of the shortcut
        /// </summary>
        [XmlElementAttribute("Description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The fullpath to the icon or exe containing an icon
        /// </summary>
        [XmlAttribute("Icon")]
        public string Icon { get; set; }

        /// <summary>
        /// The location of where the shortcut is to be stored.
        /// </summary>
        [XmlAttribute("Location")]
        public string Location { get; set; }

        /// <summary>
        /// The fullpath of the target to the shortcut.
        /// </summary>
        [XmlAttribute("Target")]
        public string Target { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

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
    public class RegistryItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The action to perform to the registry item
        /// </summary>
        [XmlAttribute("Action")]
        public RegistryAction Action { get; set; }

        /// <summary>
        ///  The data of the value in the specified key
        /// </summary>
        [XmlAttribute("Data")]
        public string Data { get; set; }

        /// <summary>
        /// The Hive of the current registry item
        /// </summary>
        [XmlAttribute("Hive")]
        public Microsoft.Win32.RegistryHive Hive { get; set; }

        /// <summary>
        /// The KeyPath of the current registry item
        /// </summary>
        [XmlAttribute("Key")]
        public string Key { get; set; }

        /// <summary>
        /// The ValueKind of the value in the specified key
        /// </summary>
        [XmlAttribute("ValueKind")]
        public Microsoft.Win32.RegistryValueKind ValueKind { get; set; }

        /// <summary>
        /// Name of the Value in the specified key
        /// </summary>
        [XmlAttribute("KeyValue")]
        public string KeyValue { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {

                handler(this, new PropertyChangedEventArgs(name));

            }

        }

        #endregion
    }

    /// <summary>
    /// Information about a file within an update
    /// </summary>
    public class UpdateFile : INotifyPropertyChanged
    {
        /// <summary>
        /// The action to do on a file
        /// </summary>
        [XmlAttribute("Action")]
        public FileAction Action { get; set; }

        /// <summary>
        /// Commandline arguments for the file
        /// </summary>
        [XmlAttribute("Arguments")]
        public string Arguments { get; set; }

        /// <summary>
        /// The destinationpath of the current file with the filename
        /// </summary>
        [XmlAttribute("Destination")]
        public string Destination { get; set; }

        /// <summary>
        /// The SHA1 hash of the current file
        /// </summary>
        [XmlAttribute("Hash")]
        public string Hash { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        [XmlAttribute("Size")]
        public ulong Size { get; set; }

        /// <summary>
        /// The sourcepath of the current file with the filename
        /// </summary>
        [XmlAttribute("Source")]
        public string Source { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {

                handler(this, new PropertyChangedEventArgs(name));

            }

        }

        #endregion
    }

    /// <summary>
    /// Information on how to install a program update
    /// </summary>
    public class Update : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or Sets release information about the update
        /// </summary>
        [XmlElementAttribute("Description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// The default download directory where the updates files are stored
        /// </summary>
        [XmlAttribute("DownloadDirectory")]
        public string DownloadDirectory { get; set; }

        /// <summary>
        /// The files of the current update
        /// </summary>
        [XmlElementAttribute("File")]
        public ObservableCollection<UpdateFile> Files { get; set; }

        /// <summary>
        /// Gets or Sets the update type of the update: Important, Recommended, Optional, Locale, Installation.
        /// </summary>
        [XmlAttribute("Importance")]
        public Importance Importance { get; set; }

        /// <summary>
        /// Gets or Sets the information/changelog url of the update: Optional
        /// </summary>
        [XmlAttribute("InfoUrl")]
        public string InfoUrl { get; set; }

        /// <summary>
        /// Gets or Sets the Software License Agreement Url
        /// </summary>
        [XmlAttribute("LicenseUrl")]
        public string LicenseUrl { get; set; }

        /// <summary>
        /// The registry entries of the current update
        /// </summary>
        [XmlElementAttribute("RegistryItem")]
        public ObservableCollection<RegistryItem> RegistryItems { get; set; }

        /// <summary>
        /// Gets or Sets the date when the update was released
        /// </summary>
        [XmlAttribute("ReleaseDate")]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// Indicates if the update is selected (not used in the SDK)
        /// </summary>
        [XmlAttribute("Selected")]
        public bool Selected { get; set; }

        /// <summary>
        /// The shortcuts to create for the update
        /// </summary>
        [XmlElementAttribute("Shortcut")]
        public ObservableCollection<Shortcut> Shortcuts { get; set; }

        /// <summary>
        /// Gets or Sets the title or name of the update
        /// </summary>
        [XmlElementAttribute("Title")]
        public ObservableCollection<LocaleString> Title { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {

                handler(this, new PropertyChangedEventArgs(name));

            }

        }

        #endregion
    }

    #endregion

    #endregion

    #region History and Hidden Updates Files

    /// <summary>
    /// Information about an update, used by History and Hidden Updates. Not used by the SDK
    /// </summary>
    public class UpdateInformation : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or Sets the application name in which the update applies too
        /// </summary>
        [XmlElementAttribute("ApplicationName")]
        public ObservableCollection<LocaleString> ApplicationName { get; set; }

        /// <summary>
        /// Gets or Sets release information about the update
        /// </summary>
        [XmlElementAttribute("Description")]
        public ObservableCollection<LocaleString> Description { get; set; }

        /// <summary>
        /// Gets or Sets the help url of the update: Optional
        /// </summary>
        [XmlAttribute("HelpUrl")]
        public string HelpUrl { get; set; }

        /// <summary>
        /// Gets or Sets the update type of the update: Critical, Recommended, Optional, Locale
        /// </summary>
        [XmlAttribute("Importance")]
        public Importance Importance { get; set; }

        /// <summary>
        /// Gets or Sets the information/changelog url of the update: Optional
        /// </summary>
        [XmlAttribute("InfoUrl")]
        public string InfoUrl { get; set; }

        /// <summary>
        /// Gets or Sets the date when the update was installed
        /// </summary>
        [XmlAttribute("InstallDate")]
        public string InstallDate { get; set; }

        /// <summary>
        /// The Publisher of the update/application
        /// </summary>
        [XmlElementAttribute("Publisher")]
        public ObservableCollection<LocaleString> Publisher { get; set; }

        /// <summary>
        /// The website of the publisher
        /// </summary>
        [XmlAttribute("PublisherUrl")]
        public string PublisherUrl { get; set; }

        /// <summary>
        /// Gets or Sets the date when the update was released
        /// </summary>
        [XmlAttribute("ReleaseDate")]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// Gets or Sets the size of the update
        /// </summary>
        [XmlAttribute("Size")]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or Sets the status of the update
        /// </summary>
        [XmlAttribute("Status")]
        public UpdateStatus Status { get; set; }

        /// <summary>
        /// Gets or Sets the title or name of the update
        /// </summary>
        [XmlElementAttribute("UpdateTitle")]
        public ObservableCollection<LocaleString> UpdateTitle { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {

                handler(this, new PropertyChangedEventArgs(name));

            }

        }

        #endregion
    }

    #endregion

}
