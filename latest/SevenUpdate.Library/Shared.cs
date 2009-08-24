/*Copyright 2007, 2008 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace SevenUpdate
{
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

    #region Property Structs

    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public struct LocaleString
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
        public bool Is64Bit{ get; set; }

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
        public bool Is64Bit{ get; set; }

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

    public class Shared
    {
        #region Global Vars

        /// <summary>
        /// The all users application data location
        /// </summary>
        public static string appStore = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Seven Update\";
        
        /// <summary>
        /// Specifies if a reboot is needed
        /// </summary>
        public static bool RebootNeeded
        {
            get { return File.Exists(appStore + @"reboot.lock"); }
        }

        /// <summary>
        /// The user application data location
        /// </summary>
        public static string userStore = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Seven Update\";

        #endregion

        #region Methods


        /// <summary>
        /// Expands the file location variables
        /// </summary>
        /// <param name="path">A string that contains a file path</param>
        /// <param name="dir">a string that contains a directory</param>
        /// <param name="Is64Bit">Specifies if the application is 64 bit</param>
        /// <returns>Returns the converted string expanded</returns>
        public static string ConvertPath(string path, string dir, bool Is64Bit)
        {
            path = Replace(path, "[AppDir]", ConvertPath(dir, true, Is64Bit));
            path = Replace(path, "[DownloadDir]", ConvertPath(dir, true, Is64Bit));
            return ConvertPath(path, true, Is64Bit);
        }
        
        /// <summary>
        /// Expands the system variables in a string
        /// </summary>
        /// <param name="path">A string that contains a file path</param>
        /// <param name="expand">True to expand system variable, false to converts paths into system variables</param>
        /// <param name="Is64Bit">Specifies if the application is 64 bit</param>
        /// <returns>Returns the converted string expanded</returns>
        public static string ConvertPath(string path, bool expand, bool Is64Bit)
        {
            if (path != null)
            {
                if (path.StartsWith("HKEY", StringComparison.OrdinalIgnoreCase))
                {
                    char[] split = { '|' };
                    string key = path.Split(split)[0];
                    string value = path.Split(split)[1];
                    path = Microsoft.Win32.Registry.GetValue(key, value, null).ToString();
                }
                else
                {

                    if (expand == false)
                    {
                        StringBuilder path2 = new StringBuilder(260);
                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FSLocation.CSIDL_COMMON_PROGRAMS, false);
                        path = Replace(path, path2.ToString(), "%ALLUSERSSTARTMENUPROGRAMS%");

                        path2 = new StringBuilder(260);
                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FSLocation.CSIDL_COMMON_STARTMENU, false);
                        path = Replace(path, path2.ToString(), "%ALLUSERSSTARTMENU%");

                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.Programs), "%STARTMENUPROGRAMS%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "%STARTMENU%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "%DOCUMENTS%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "%MUSIC%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "%PICTURES%");

                        if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                        {
                            path = Replace(path, Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%");
                            path = Replace(path, Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%");

                            if (Is64Bit)
                            {
                                path = Replace(path, Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%");
                                path = Replace(path, Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%");
                                path = Replace(path, Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%");
                                path = Replace(path, Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%");
                            }
                            else
                            {
                                path = Replace(path, Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%");
                                path = Replace(path, Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%");
                                path = Replace(path, Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%");
                                path = Replace(path, Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%");
                            }

                        }
                        else
                        {
                            path = Replace(path, Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%");
                            path = Replace(path, Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%");
                        }

                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "%APPDATA%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "%LOCALAPPDATA%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.Startup), "%STARTUP%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.Templates), "%TEMPLATES%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "%DESKTOP%");
                        path = Replace(path, Environment.GetEnvironmentVariable("TMP"), "%TMP%");
                        path = Replace(path, Environment.GetEnvironmentVariable("TEMP"), "%TEMP%");
                        path = Replace(path, Environment.GetEnvironmentVariable("USERPROFILE"), "%USERPROFILE%");
                        path = Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.System), "%SYSTEM32%");
                        path = Replace(path, Environment.UserName, "%USERNAME%");
                        path = Replace(path, Environment.GetEnvironmentVariable("LOCALAPPDATA"), "%LOCALAPPDATA%");
                        path = Replace(path, Environment.GetEnvironmentVariable("PROGRAMDATA"), "%PROGRAMDATA%");
                        path = Replace(path, Environment.GetEnvironmentVariable("PUBLIC"), "%PUBLIC%");
                        path = Replace(path, Environment.GetEnvironmentVariable("HOMEPATH"), "%HOMEPATH%");
                        path = Replace(path, Environment.GetEnvironmentVariable("SYSTEMROOT"), "%SYSTEMROOT%");
                        path = Replace(path, Environment.GetEnvironmentVariable("WINDIR"), "%WINDIR%");
                        path = Replace(path, Environment.GetEnvironmentVariable("SYSTEMDRIVE"), "%SYSTEMDRIVE%");
                    }
                    else
                    {
                        StringBuilder path2 = new StringBuilder(260);

                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FSLocation.CSIDL_COMMON_PROGRAMS, false);

                        path = Replace(path, "%ALLUSERSSTARTMENUPROGRAMS%", path2.ToString());

                        path2 = new StringBuilder(260);

                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FSLocation.CSIDL_COMMON_STARTMENU, false);

                        path = Replace(path, "%ALLUSERSSTARTMENU%", path2.ToString());
                        path = Replace(path, "%STARTMENUPROGRAMS%", Environment.GetFolderPath(Environment.SpecialFolder.Programs));
                        path = Replace(path, "%STARTMENU%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
                        path = Replace(path, "%DOCUMENTS%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                        path = Replace(path, "%MUSIC%", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                        path = Replace(path, "%PICTURES%", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

                        if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                        {
                            path = Replace(path, "%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"));
                            path = Replace(path, "%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
                            if (Is64Bit)
                            {
                                path = Replace(path, "%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"));
                                path = Replace(path, "%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles"));
                            }
                            else
                            {
                                path = Replace(path, "%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"));
                                path = Replace(path, "%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
                            }
                        }
                        else
                        {
                            path = Replace(path, "%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"));
                            path = Replace(path, "%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles"));
                            path = Replace(path, "%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"));
                            path = Replace(path, "%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles"));

                        }

                        path = Replace(path, "%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                        path = Replace(path, "%LOCALAPPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                        path = Replace(path, "%STARTUP%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                        path = Replace(path, "%TEMPLATES%", Environment.GetFolderPath(Environment.SpecialFolder.Templates));
                        path = Replace(path, "%DESKTOP%", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                        path = Replace(path, "%TEMP%", Environment.GetEnvironmentVariable("TEMP"));
                        path = Replace(path, "%TMP%", Environment.GetEnvironmentVariable("TMP"));
                        path = Replace(path, "%USERPROFILE%", Environment.GetEnvironmentVariable("USERPROFILE"));
                        path = Replace(path, "%SYSTEM32%", Environment.GetFolderPath(Environment.SpecialFolder.System));
                        path = Replace(path, "%USERNAME%", Environment.UserName);
                        path = Replace(path, "%LOCALAPPDATA%", Environment.GetEnvironmentVariable("LOCALAPPDATA"));
                        path = Replace(path, "%PROGRAMDATA%", Environment.GetEnvironmentVariable("PROGRAMDATA"));
                        path = Replace(path, "%PUBLIC%", Environment.GetEnvironmentVariable("PUBLIC"));
                        path = Replace(path, "%HOMEPATH%", Environment.GetEnvironmentVariable("HOMEPATH"));
                        path = Replace(path, "%SYSTEMROOT%", Environment.GetEnvironmentVariable("SYSTEMROOT"));
                        path = Replace(path, "%WINDIR%", Environment.GetEnvironmentVariable("WINDIR"));
                        path = Replace(path, "%SYSTEMDRIVE%", Environment.GetEnvironmentVariable("SYSTEMDRIVE"));
                    }
                }
            }
            return path;

        }

        /// <summary>
        /// Gets the SHA1 Hash of a file
        /// </summary>
        /// <param name="fileLoc">The fullpath to the file to get the hash from</param>
        /// <returns>returns the SHA1 hash value</returns>
        public static string GetHash(string fileLoc)
        {
            if (File.Exists(fileLoc))
            {
                FileStream stream = new FileStream(fileLoc,
                FileMode.Open, FileAccess.Read, FileShare.Read, 8192);

                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

                sha1.ComputeHash(stream);

                stream.Close();

                StringBuilder buff = new StringBuilder();

                foreach (byte hashByte in sha1.Hash)
                {
                    buff.Append(String.Format("{0:X1}", hashByte));
                }
                return buff.ToString();
            }
            else
                return null;
            
        }

        /// <summary>
        /// Replaces a string within a string
        /// </summary>
        /// <param name="complete">The string that will be searched</param>
        /// <param name="find">A string to find in the complete string</param>
        /// <param name="replace">A string to use to replace the find string in the complete string</param>
        /// <returns>Returns the new string</returns>
        public static string Replace(string complete, string find, string replace)
        {
            // Get input string length
            int exprLen = complete.Length;

            int findLen = find.Length;

            // Check inputs
            if (0 == exprLen || 0 == findLen || findLen > exprLen)
                return complete;

            StringBuilder sbRet = new StringBuilder(exprLen);

            int pos = 0;

            while (pos + findLen <= exprLen)
            {
                if (0 == string.Compare(complete, pos, find, 0, findLen, true))
                {
                    // Add the replaced string
                    sbRet.Append(replace);

                    pos += findLen;

                    continue;
                }

                // Advance one character
                sbRet.Append(complete, pos++, 1);
            }

            // Append remaining characters
            sbRet.Append(complete, pos, exprLen - pos);
            // Return string
            return sbRet.ToString();

        }

        /// <summary>
        /// Converts bytes into the proper increments depending on size
        /// </summary>
        /// <param name="fileSize">The fileSize is bytes</param>
        /// <returns>Returns formatted string of converted bytes</returns>
        public static string ConvertFileSize(ulong byteCount)
        {
            if (byteCount >= 1073741824)
                return String.Format("{0:##.##}", byteCount / 1073741824) + " GB";
            else if (byteCount >= 1048576)
                return String.Format("{0:##.##}", byteCount / 1048576) + " MB";
            else if (byteCount >= 1024)
                return String.Format("{0:##.##}", byteCount / 1024) + " KB";
            else if (byteCount < 1024)
                return byteCount.ToString() + " Bytes";
            return "0 Bytes";
        }

        #endregion

        #region DeSerialize Methods


        /// <summary>
        /// DeSerializes a list of objects
        /// </summary>
        /// <typeparam name="T">The object to DeSerialize</typeparam>
        /// <param name="xmlFile">he xml file to DeSerialize</param>
        /// <returns>Returns the list of objects</returns>
        public static ObservableCollection<T> DeserializeCollection<T>(string xmlFile) where T : class
        {
            if (File.Exists(xmlFile))
            {
                XmlSerializer s = new XmlSerializer(typeof(ObservableCollection<T>));

                ObservableCollection<T> temp;

                using (TextReader r = new StreamReader(xmlFile))
                {
                    try
                    {
                        temp = (ObservableCollection<T>)s.Deserialize(r);

                        return temp;
                    }
                    catch (Exception e)
                    {
                        if (SerializationErrorEventHandler != null)
                            SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e.Message, xmlFile));
                    }
                    
                    finally { r.Close(); }

                }  
            }
            return new ObservableCollection<T>();
        }

        /// <summary>
        /// DeSerializes an object
        /// </summary>
        /// <typeparam name="T">The object to deserialize</typeparam>
        /// <param name="xmlFile">The file that contains the object to DeSerialize</param>
        /// <returns>Returns the object</returns>
        public static T Deserialize<T>(string xmlFile) where T : class
        {
            if (File.Exists(xmlFile))
            {

                XmlSerializer s = new XmlSerializer(typeof(T));
                
                T temp;
                using (TextReader r = new StreamReader(xmlFile))
                {
                    try
                    {
                        temp = (T)s.Deserialize(r);
                        
                        return temp;
                    }
                    catch (Exception e)
                    {
                        if (SerializationErrorEventHandler != null)
                            SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e.Message, xmlFile));
                    }
                    finally
                    {
                        r.Close();
                    }
                }
            }
            return null;
        }

        #endregion

        #region Serialize Methods

        /// <summary>
        /// Serializes a list of objects into an xml file
        /// </summary>
        /// <typeparam name="T">The object</typeparam>
        /// <param name="list">The list of an object</param>
        /// <param name="xmlFile">The location of a file that will be serialized</param>
        public static void SerializeCollection<T>(ObservableCollection<T> list, string xmlFile) where T : class
        {
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(ObservableCollection<T>));

                TextWriter w = new StreamWriter(xmlFile);

                s.Serialize(w, list);

                w.Close();
            }
            catch (Exception e)
            {
                if (SerializationErrorEventHandler != null)
                    SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e.Message, xmlFile));
            }
        }

        /// <summary>
        /// Serializes an object into a file
        /// </summary>
        /// <typeparam name="T">The object</typeparam>
        /// <param name="item">The object to serialize</param>
        /// <param name="xmlFile">The location of a file that will be serialized</param>
        public static void Serialize<T>(T item, string xmlFile) where T : class
        {
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(T));

                TextWriter w = new StreamWriter(xmlFile);

                s.Serialize(w, item);

                w.Close();
            }
            catch (Exception e)
            {
                if (SerializationErrorEventHandler != null)
                    SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e.Message, xmlFile));
            }
        }

        #endregion

        #region Event Handlers

        public static event EventHandler<SerializationErrorEventArgs> SerializationErrorEventHandler; 

        public class SerializationErrorEventArgs : EventArgs
        {
            public SerializationErrorEventArgs(string errorMessage, string file)
            {
                ErrorMessage = errorMessage;
                this.File = file;
            }

            /// <summary>
            /// A string describing the error
            /// </summary>
            public string ErrorMessage{ get; set; }

            /// <summary>
            /// The file that caused the error message
            /// </summary>
            public string File { get; set; }
        }
        #endregion
    }

    static class NativeMethods
    {
        [DllImport("shell32.dll")]
        internal static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner,
           [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
    }
}
