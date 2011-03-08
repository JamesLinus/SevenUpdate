// ***********************************************************************
// <copyright file="Shortcut.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;

    using ProtoBuf;

    /// <summary>The action to preform on the shortcut</summary>
    [ProtoContract, DataContract, DefaultValue(Add)]
    public enum ShortcutAction
    {
        /// <summary>Adds a shortcut</summary>
        [ProtoEnum, EnumMember]
        Add = 0, 

        /// <summary>Updates a shortcut only if it exists</summary>
        [ProtoEnum, EnumMember]
        Update = 1, 

        /// <summary>Deletes a shortcut</summary>
        [ProtoEnum, EnumMember]
        Delete = 2
    }

    /// <summary>The Msi Component install state</summary>
    internal enum InstallState
    {
        /// <summary>The component being requested is disabled on the computer.</summary>
        NotUsed = -7, 

        /// <summary>The configuration data is corrupt.</summary>
        BadConfig = -6, 

        /// <summary>The installation is incomplete</summary>
        Incomplete = -5, 

        /// <summary>The component source is inaccessible.</summary>
        SourceAbsent = -4, 

        /// <summary>One of the function parameters is invalid.</summary>
        InvalidArg = -2, 

        /// <summary>The product code or component ID is unknown.</summary>
        Unknown = -1, 

        /// <summary>The shortcut is advertised</summary>
        Advertised = 1, 

        /// <summary>The component has been removed</summary>
        Removed = 1, 

        /// <summary>The component is not installed.</summary>
        Absent = 2, 

        /// <summary>The component is installed locally.</summary>
        Local = 3, 

        /// <summary>The component is installed to run from source.</summary>
        Source = 4, 
    }

    /// <summary>A shortcut to be created within an update</summary>
    [ProtoContract, DataContract(IsReference = true), KnownType(typeof(ShortcutAction))]
    public sealed class Shortcut : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>The max feature length</summary>
        private const int MaxFeatureLength = 38;

        /// <summary>The max Guid length</summary>
        private const int MaxGuidLength = 38;

        /// <summary>The max path</summary>
        private const int MaxPath = 260;

        /// <summary>The path path length</summary>
        private const int MaxPathLength = 1024;

        /// <summary>The read constant</summary>
        private const uint Read = 0;

        /// <summary>The action to perform on the <see cref = "Shortcut" /></summary>
        private ShortcutAction action;

        /// <summary>The command line arguments for the shortcut</summary>
        private string arguments;

        /// <summary>The icon resource for the shortcut</summary>
        private string icon;

        /// <summary>The physical location of the shortcut lnk file</summary>
        private string location;

        /// <summary>The file or folder that is executed by the shortcut</summary>
        private string target;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Shortcut"/> class</summary>
        /// <param name="name">The collection of localized update names</param>
        /// <param name="description">The collection of localized update descriptions</param>
        public Shortcut(ObservableCollection<LocaleString> name, ObservableCollection<LocaleString> description)
        {
            this.Description = description;
            this.Name = name;

            if (this.Description == null)
            {
                this.Description = new ObservableCollection<LocaleString>();
            }

            if (this.Name == null)
            {
                this.Name = new ObservableCollection<LocaleString>();
            }

            this.Name.CollectionChanged -= this.NameCollectionChanged;
            this.Description.CollectionChanged -= this.DescriptionCollectionChanged;
            this.Name.CollectionChanged += this.NameCollectionChanged;
            this.Description.CollectionChanged += this.DescriptionCollectionChanged;
        }

        /// <summary>Initializes a new instance of the <see cref = "Shortcut" /> class</summary>
        public Shortcut()
        {
            this.Name = new ObservableCollection<LocaleString>();
            this.Description = new ObservableCollection<LocaleString>();

            this.Name.CollectionChanged -= this.NameCollectionChanged;
            this.Description.CollectionChanged -= this.DescriptionCollectionChanged;
            this.Name.CollectionChanged += this.NameCollectionChanged;
            this.Description.CollectionChanged += this.DescriptionCollectionChanged;
        }

        #endregion

        #region Events

        /// <summary>Occurs when a property has changed</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Interfaces

        /// <summary>The interface for a Persistent file</summary>
        [ComImport, Guid("0000010c-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersist
        {
            /// <summary>Gets the class ID.</summary>
            /// <param name="classID">The class ID.</param>
            [PreserveSig]
            void GetClassID(out Guid classID);
        }

        /// <summary>Enables an object to be loaded from or saved to a disk file, rather than a storage object or stream.Because the information needed to open a file varies greatly from one application to another, the implementation of IPersistFile::Load on the object must also open its disk file.</summary>
        [ComImport, Guid("0000010b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFile : IPersist
        {
            /// <summary>Gets the class ID.</summary>
            /// <param name="classID">The class ID.</param>
            new void GetClassID(out Guid classID);

            /// <summary>Determines whether an object has changed since it was last saved to its current file.</summary>
            /// <returns>the error result</returns>
            [PreserveSig]
            int IsDirty();

            /// <summary>Opens the specified file and initializes an object from the file contents.</summary>
            /// <param name="fileName">The absolute path of the file to be opened.</param>
            /// <param name="mode">
            /// The access mode to be used when opening the file. Possible values are taken from the Stgm enumeration. 
            ///   The method can treat this value as a suggestion, adding more restrictive permissions if necessary. 
            ///   If mode is 0, the implementation should open the file using whatever default permissions are used when a user opens the file.
            /// </param>
            [PreserveSig]
            void Load([MarshalAs(UnmanagedType.LPWStr)] string fileName, uint mode);

            /// <summary>Saves a copy of the object to the specified file.</summary>
            /// <param name="fileName">
            /// The absolute path of the file to which the object should be saved.
            ///   If fileName is null, the object should save its data to the current file, if there is one.
            /// </param>
            /// <param name="remember">
            /// Indicates whether the fileName parameter is to be used as the current working file.
            ///   If true, fileName becomes the current file and the object should clear its dirty flag after the save.
            ///   If false, this save operation is a Save A Copy As ... operation. In this case, the current file is unchanged and the object should not clear its dirty flag.
            ///   If fileName is null, the implementation should ignore the remember flag.
            /// </param>
            [PreserveSig]
            void Save([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.Bool)] bool remember);

            /// <summary>Notifies the object that it can write to its file. It does this by notifying the object that it can revert from NoScribble mode (in which it must not write to its file), to Normal mode (in which it can).The component enters NoScribble mode when it receives an IPersistFile::Save call.</summary>
            /// <param name="fileName">The absolute path of the file where the object was saved previously.</param>
            [PreserveSig]
            void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string fileName);

            /// <summary>Retrieves the current name of the file associated with the object. If there is no current working file, this method retrieves the default save prompt for the object.</summary>
            /// <param name="fileName">The path for the current file or the default file name prompt (such as *.txt). If an error occurs, fileName is set to null.</param>
            [PreserveSig]
            void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] string fileName);
        }

        /// <summary>The i shell link.</summary>
        [ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellLink
        {
            /// <summary>Retrieves the path and file name of a Shell link object</summary>
            /// <param name="file">The filename of the shortcut</param>
            /// <param name="maxPath">The max path.</param>
            /// <param name="data">The data to get</param>
            /// <param name="flags">The options to specify the path is retrieved.</param>
            [PreserveSig]
            void GetPath([MarshalAs(UnmanagedType.LPWStr)] StringBuilder file, int maxPath, ref Win32FindData data, uint flags);

            /// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
            /// <param name="identifer">The indentifer list.</param>
            [PreserveSig]
            void GetIDList(out IntPtr identifer);

            /// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
            /// <param name="identifer">The indentifer list</param>
            [PreserveSig]
            void SetIDList(IntPtr identifer);

            /// <summary>Retrieves the description string for a Shell link object</summary>
            /// <param name="description">The description of the shortcut.</param>
            /// <param name="maxChars">The maximum number of characters to copy to the buffer pointed to by the description parameter.</param>
            [PreserveSig]
            void GetDescription([MarshalAs(UnmanagedType.LPWStr)] StringBuilder description, int maxChars);

            /// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
            /// <param name="description">The description of the shortcut.</param>
            [PreserveSig]
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string description);

            /// <summary>Retrieves the name of the working directory for a Shell link object</summary>
            /// <param name="dir">The working directory.</param>
            /// <param name="maxPath">The maximum number of characters to copy to the buffer pointed to by the dir parameter.</param>
            [PreserveSig]
            void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] StringBuilder dir, int maxPath);

            /// <summary>Sets the name of the working directory for a Shell link object</summary>
            /// <param name="dir">The working directory.</param>
            [PreserveSig]
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string dir);

            /// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
            /// <param name="args">The arguments for the shortcut</param>
            /// <param name="maxPath">The maximum number of characters that can be copied to the buffer supplied by the args parameter</param>
            [PreserveSig]
            void GetArguments([MarshalAs(UnmanagedType.LPWStr)] StringBuilder args, int maxPath);

            /// <summary>Sets the command-line arguments for a Shell link object</summary>
            /// <param name="args">The arguments for the shortcut</param>
            [PreserveSig]
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string args);

            /// <summary>Retrieves the hot key for a Shell link object</summary>
            /// <param name="hotkey">The hotkey.</param>
            [PreserveSig]
            void GetHotkey(out ushort hotkey);

            /// <summary>Sets a hot key for a Shell link object</summary>
            /// <param name="hotkey">The hotkey.</param>
            [PreserveSig]
            void SetHotkey(ushort hotkey);

            /// <summary>Retrieves the show command for a Shell link object</summary>
            /// <param name="showCmd">The show command.</param>
            [PreserveSig]
            void GetShowCmd(out int showCmd);

            /// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
            /// <param name="showCmd">The show command.</param>
            [PreserveSig]
            void SetShowCmd(int showCmd);

            /// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
            /// <param name="iconPath">The icon path.</param>
            /// <param name="iconPathLength">The maximum number of characters to copy to the buffer pointed to by the iconPath parameter.</param>
            /// <param name="iconIndex">Index of the icon.</param>
            [PreserveSig]
            void GetIconLocation([MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] StringBuilder iconPath, int iconPathLength, out int iconIndex);

            /// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
            /// <param name="iconPath">The icon path.</param>
            /// <param name="iconIndex">Index of the icon.</param>
            [PreserveSig]
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string iconPath, int iconIndex);

            /// <summary>Sets the relative path to the Shell link object</summary>
            /// <param name="relativePath">The relative path.</param>
            /// <param name="reserved">The reserved.</param>
            [PreserveSig]
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string relativePath, uint reserved);

            /// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed.</summary>
            /// <param name="handle">A handle to the window that the Shell will use as the parent for a dialog box. The Shell displays the dialog box if it needs to prompt the user for more information while resolving a Shell link.</param>
            /// <param name="flags">The action options</param>
            [PreserveSig]
            void Resolve(IntPtr handle, uint flags);

            /// <summary>Sets the path and file name of a Shell link object</summary>
            /// <param name="file">The file to set the path</param>
            [PreserveSig]
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string file);
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the action to perform on the <see cref = "Shortcut" /></summary>
        /// <value>The action.</value>
        [ProtoMember(3), DataMember]
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

        /// <summary>Gets or sets the command line arguments for the shortcut</summary>
        /// <value>The arguments of the shortcut</value>
        [ProtoMember(4, IsRequired = false), DataMember]
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

        /// <summary>Gets the collection of localized shortcut descriptions</summary>
        /// <value>The localized descriptions for the shortcut</value>
        [ProtoMember(5, IsRequired = false), DataMember]
        public ObservableCollection<LocaleString> Description { get; private set; }

        /// <summary>Gets or sets the icon resource for the shortcut</summary>
        /// <value>The icon for the shortcut</value>
        [ProtoMember(6, IsRequired = false), DataMember]
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

        /// <summary>Gets or sets the physical location of the shortcut lnk file</summary>
        /// <value>The shortcut location</value>
        [ProtoMember(2), DataMember]
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

        /// <summary>Gets the collection of localized shortcut names</summary>
        /// <value>The localized names for the shortcut</value>
        [ProtoMember(1), DataMember]
        public ObservableCollection<LocaleString> Name { get; private set; }

        /// <summary>Gets or sets the file or folder that is executed by the shortcut</summary>
        /// <value>The target for the shortcut</value>
        [ProtoMember(7, IsRequired = false), DataMember]
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

        #region Public Methods

        /// <summary>Creates a shortcut on the system</summary>
        /// <param name="shortcut">The shortcut data used to create the shortcut</param>
        public static void CreateShortcut(Shortcut shortcut)
        {
            if (shortcut == null)
            {
                throw new ArgumentNullException(@"shortcut");
            }

            var shellLink = new CShellLink();
            var link = (IShellLink)shellLink;

            link.SetArguments(shortcut.arguments);

            if (shortcut.Description.Count > 0)
            {
                link.SetDescription(Utilities.GetLocaleString(shortcut.Description));
            }

            if (!String.IsNullOrWhiteSpace(shortcut.Icon))
            {
                var icon = shortcut.Icon.Split(new[] { ',' });
                link.SetIconLocation(icon[0], Convert.ToInt32(icon[1]));
            }

            link.SetWorkingDirectory(Path.GetDirectoryName(shortcut.Target));
            link.SetPath(shortcut.Target);

            var persistFile = (IPersistFile)link;
            persistFile.Save(Path.Combine(shortcut.Location, Utilities.GetLocaleString(shortcut.Name) + ".lnk"), false);
            Marshal.ReleaseComObject(persistFile);
            Marshal.ReleaseComObject(link);
            Marshal.ReleaseComObject(shellLink);

            // NativeMethods.CoInitializeEx((IntPtr)null, NativeMethods.CoInit.ApartmentThreaded);
        }

        /// <summary>Gets data associated with a shortcut</summary>
        /// <param name="shortcutName">The full path to the shortcut lnk file</param>
        /// <returns>The data for the shortcut</returns>
        public static Shortcut GetShortcutData(string shortcutName)
        {
            var link = new CShellLink();
            ((IPersistFile)link).Load(shortcutName, Read);

            var sb = new StringBuilder(MaxPath);
            var ls = new LocaleString { Lang = Utilities.Locale, Value = Path.GetFileNameWithoutExtension(shortcutName) };
            var shortcut = new Shortcut { Target = GetMsiTargetPath(shortcutName), };

            shortcut.Name.Add(ls);

            var shellLink = link as IShellLink;
            if (shellLink == null)
            {
                return null;
            }

            if (shortcut.Target == null)
            {
                var data = new Win32FindData();
                shellLink.GetPath(sb, MaxPath, data, 0);
                shortcut.Target = sb.ToString();
            }

            shellLink.GetArguments(sb, MaxPath);
            shortcut.Arguments = sb.ToString();

            shellLink.GetDescription(sb, MaxPath);
            ls.Value = sb.ToString();
            shortcut.Description.Add(ls);

            int iconIndex;
            shellLink.GetIconLocation(sb, MaxPath, out iconIndex);
            if (String.IsNullOrWhiteSpace(sb.ToString()))
            {
                shortcut.Icon = shortcut.Target + @"," + iconIndex;
            }
            else
            {
                shortcut.Icon = sb.ToString();
            }

            shortcut.Location = shortcutName;

            return shortcut;
        }

        #endregion

        #region Methods

        /// <summary>Gets the target path from a Msi shortcut</summary>
        /// <param name="shortcutPath">The path to the shortcut lnk file</param>
        /// <returns>The resolved path to the shortcut</returns>
        private static string GetMsiTargetPath(string shortcutPath)
        {
            var product = new StringBuilder(MaxGuidLength + 1);
            var feature = new StringBuilder(MaxFeatureLength + 1);
            var component = new StringBuilder(MaxGuidLength + 1);

            var result = NativeMethods.MsiGetShortcutTarget(shortcutPath, product, feature, component);

            if (result != 0x0000)
            {
                return null;
            }

            var pathLength = MaxPathLength;
            var path = new StringBuilder(pathLength);

            var installState = NativeMethods.MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            return installState == InstallState.Source ? path.ToString() : null;
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void DescriptionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Description");
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void NameCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Name");
        }

        /// <summary>When a property has changed, call the <see cref="OnPropertyChanged"/> Event</summary>
        /// <param name="propertyName">The name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>The file time.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FileTime
        {
            /// <summary>The low-order part of the file time.</summary>
            private uint lowDateTime;

            /// <summary>The high-order part of the file time.</summary>
            private uint highDateTime;
        }

        /// <summary>The win 32 find data.</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Win32FindData
        {
            /// <summary>The file attributes</summary>
            private uint fileAttributes;

            /// <summary>A FileTime structure that specifies when a file or directory was created.</summary>
            private FileTime creationTime;

            /// <summary>For a file, the structure specifies when the file was last read from, written to, or for executable files, run.For a directory, the structure specifies when the directory is created. If the underlying file system does not support last access time, this member is zero.</summary>
            private FileTime lastAccessTime;

            /// <summary>For a file, the structure specifies when the file was last written to, truncated, or overwritten, for example, when WriteFile or SetEndOfFile are used. The date and time are not updated when file attributes or security descriptors are changed.For a directory, the structure specifies when the directory is created. If the underlying file system does not support last write time, this member is zero.</summary>
            private FileTime lastWriteTime;

            /// <summary>The high-order file size</summary>
            private uint fileSizeHigh;

            /// <summary>The low-order file size</summary>
            private uint fileSizeLow;

            /// <summary>Reserved data</summary>
            private uint reserved0;

            /// <summary>Reserved data</summary>
            private uint reserved1;

            /// <summary>The name of the file</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            private string fileName;

            /// <summary>The alternate name of the file</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            private string alternateFileName;
        }

        /// <summary>The c shell link.</summary>
        [Guid("00021401-0000-0000-C000-000000000046"), ClassInterfaceAttribute(ClassInterfaceType.None), ComImportAttribute]
        public class CShellLink
        {
        }
    }
}