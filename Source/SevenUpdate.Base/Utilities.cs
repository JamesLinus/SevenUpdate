// ***********************************************************************
// <copyright file="Utilities.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Microsoft.Win32;

    using ProtoBuf;

    /// <summary>Methods that are shared between other classes</summary>
    public static class Utilities
    {
        #region Constants and Fields

        /// <summary>The all users application data location</summary>
        public static readonly string AllUserStore = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Seven Software\Seven Update\";

        /// <summary>The application directory of Seven Update</summary>
        public static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";

        /// <summary>The location of the list of applications Seven Update can update</summary>
        public static readonly string ApplicationsFile = AllUserStore + @"Apps.sul";

        /// <summary>The location of the application settings file</summary>
        public static readonly string ConfigFile = AllUserStore + @"App.config";

        /// <summary>The location of the hidden updates file</summary>
        public static readonly string HiddenFile = AllUserStore + @"Hidden.suh";

        /// <summary>The location of the update history file</summary>
        public static readonly string HistoryFile = AllUserStore + @"History.suh";

        /// <summary>The location of the user application data location</summary>
        public static readonly string UserStore = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Seven Software\Seven Update\";

        #endregion

        #region Events

        /// <summary>Occurs when an error occurs while serializing or deserializing a object/file</summary>
        public static event EventHandler<SerializationErrorEventArgs> SerializationError;

        #endregion

        #region Properties

        /// <summary>Gets or sets the ISO language code</summary>
        /// <value>The locale.</value>
        public static string Locale { get; set; }

        /// <summary>Gets a value indicating whether if a reboot is needed</summary>
        /// <value><see langword = "true" /> if a reboot is needed otherwise, <see langword = "false" />.</value>
        public static bool RebootNeeded
        {
            get
            {
                return File.Exists(AllUserStore + @"reboot.lock");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Converts bytes into the proper increments depending on size</summary>
        /// <param name="bytes">the fileSize in bytes</param>
        /// <returns>the formatted string of converted bytes</returns>
        public static string ConvertFileSize(ulong bytes)
        {
            if (bytes >= 1073741824)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:##.##}", bytes / 1073741824) + " GB";
            }

            if (bytes >= 1048576)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:##.##}", bytes / 1048576) + " MB";
            }

            if (bytes >= 1024)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:##.##}", bytes / 1024) + " KB";
            }

            if (bytes < 1024)
            {
                return bytes + " Bytes";
            }

            return "0 Bytes";
        }

        /// <summary>Expands the file location variables</summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="directory">a string that contains a directory</param>
        /// <param name="valueName">a string that contains a value name of the registry key that contains the directory location, this parameter is optional and can be <see langword="null"/></param>
        /// <param name="is64Bit">if set to <see langword="true"/> the application is 64 bit</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, string directory, string valueName = null, bool is64Bit = false)
        {
            path = path.Replace(
                "%INSTALLDIR%", 
                !IsRegistryKey(directory) ? ConvertPath(directory, true, is64Bit) : ConvertPath(GetRegistryValue(directory, valueName, is64Bit), true, is64Bit), 
                true);
            path = path.Replace("%DOWNLOADURL%", ConvertPath(directory, true, is64Bit), true);
            return ConvertPath(path, true, is64Bit);
        }

        /// <summary>Expands the system variables in a string</summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="expand"><see langword = "true" /> to expand system variable, <see langword = "false" /> to converts paths into system variables</param>
        /// <param name="is64Bit">if set to <see langword="true"/> the application is 64 bit</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, bool expand, bool is64Bit)
        {
            if (path == null)
            {
                return path;
            }

            var stringBuilder = new StringBuilder(path);

            if (expand == false)
            {
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "%ALLUSERSSTARTMENU%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "%STARTMENUPROGRAMS%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "%STARTMENU%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "%DOCUMENTS%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "%MUSIC%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "%PICTURES%", true);

                if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    if (is64Bit)
                    {
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%", true);
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%", true);
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%", true);
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%", true);
                    }
                    else
                    {
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%", true);
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%", true);
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%", true);
                        stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%", true);
                    }
                }
                else
                {
                    stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES%", true);
                    stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES%", true);
                    stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%", true);
                    stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%", true);
                }

                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "%APPDATA%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "%LOCALAPPDATA%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "%STARTUP%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Templates), "%TEMPLATES%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "%DESKTOP%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("TMP"), "%TMP%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("TEMP"), "%TEMP%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("USERPROFILE"), "%USERPROFILE%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.System), "%SYSTEM32%", true);
                stringBuilder = stringBuilder.Replace(Environment.UserName, "%USERNAME%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "%LOCALAPPDATA%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("PROGRAMDATA"), "%PROGRAMDATA%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("PUBLIC"), "%PUBLIC%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("HOMEPATH"), "%HOMEPATH%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("SYSTEMROOT"), "%SYSTEMROOT%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("WINDIR"), "%WINDIR%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("SYSTEMDRIVE"), "%SYSTEMDRIVE%", true);
            }
            else
            {
                stringBuilder = stringBuilder.Replace("%ALLUSERSSTARTMENU%", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), true);
                stringBuilder = stringBuilder.Replace("%STARTMENUPROGRAMS%", Environment.GetFolderPath(Environment.SpecialFolder.Programs), true);
                stringBuilder = stringBuilder.Replace("%STARTMENU%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), true);
                stringBuilder = stringBuilder.Replace("%DOCUMENTS%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), true);
                stringBuilder = stringBuilder.Replace("%MUSIC%", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), true);
                stringBuilder = stringBuilder.Replace("%PICTURES%", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), true);

                if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), true);
                    stringBuilder = stringBuilder.Replace("%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"), true);
                    if (is64Bit)
                    {
                        stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), true);
                        stringBuilder = stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles"), true);
                    }
                    else
                    {
                        stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), true);
                        stringBuilder = stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"), true);
                    }
                }
                else
                {
                    stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), true);
                    stringBuilder = stringBuilder.Replace("%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles"), true);
                    stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), true);
                    stringBuilder = stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles"), true);
                }

                stringBuilder = stringBuilder.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), true);
                stringBuilder = stringBuilder.Replace("%LOCALAPPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), true);
                stringBuilder = stringBuilder.Replace("%STARTUP%", Environment.GetFolderPath(Environment.SpecialFolder.Startup), true);
                stringBuilder = stringBuilder.Replace("%TEMPLATES%", Environment.GetFolderPath(Environment.SpecialFolder.Templates), true);
                stringBuilder = stringBuilder.Replace("%DESKTOP%", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), true);
                stringBuilder = stringBuilder.Replace("%TEMP%", Environment.GetEnvironmentVariable("TEMP"), true);
                stringBuilder = stringBuilder.Replace("%TMP%", Environment.GetEnvironmentVariable("TMP"), true);
                stringBuilder = stringBuilder.Replace("%USERPROFILE%", Environment.GetEnvironmentVariable("USERPROFILE"), true);
                stringBuilder = stringBuilder.Replace("%SYSTEM32%", Environment.GetFolderPath(Environment.SpecialFolder.System), true);
                stringBuilder = stringBuilder.Replace("%USERNAME%", Environment.UserName, true);
                stringBuilder = stringBuilder.Replace("%LOCALAPPDATA%", Environment.GetEnvironmentVariable("LOCALAPPDATA"), true);
                stringBuilder = stringBuilder.Replace("%PROGRAMDATA%", Environment.GetEnvironmentVariable("PROGRAMDATA"), true);
                stringBuilder = stringBuilder.Replace("%PUBLIC%", Environment.GetEnvironmentVariable("PUBLIC"), true);
                stringBuilder = stringBuilder.Replace("%HOMEPATH%", Environment.GetEnvironmentVariable("HOMEPATH"), true);
                stringBuilder = stringBuilder.Replace("%SYSTEMROOT%", Environment.GetEnvironmentVariable("SYSTEMROOT"), true);
                stringBuilder = stringBuilder.Replace("%WINDIR%", Environment.GetEnvironmentVariable("WINDIR"), true);
                stringBuilder = stringBuilder.Replace("%SYSTEMDRIVE%", Environment.GetEnvironmentVariable("SYSTEMDRIVE"), true);
            }

            // ReSharper restore AssignNullToNotNullAttribute
            return stringBuilder.ToString();
        }

        /// <summary>DeSerializes an object</summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="fileName">the file that contains the object to DeSerialize</param>
        /// <returns>returns the object</returns>
        public static T Deserialize<T>(string fileName) where T : class
        {
            var task = Task.Factory.StartNew(() => DeserializeFile<T>(new Uri(fileName)));
            task.Wait();
            return task.Result;
        }

        /// <summary>DeSerializes an object</summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="stream">The Stream to deserialize</param>
        /// <param name="sourceUrl">The Uri to the source stream that is being deserialized</param>
        /// <returns>returns the object</returns>
        public static T Deserialize<T>(Stream stream, Uri sourceUrl) where T : class
        {
            var task = Task.Factory.StartNew(() => DeserializeStream<T>(stream, sourceUrl));
            task.Wait();
            return task.Result;
        }

        /// <summary>Downloads a file</summary>
        /// <param name="url">A Uri pointing to the location of the file to download</param>
        /// <returns>the downloaded file <see cref="Stream"/></returns>
        public static Stream DownloadFile(Uri url)
        {
            // Get a data stream from the url
            MemoryStream memStream;
            using (var wc = new WebClient())
            {
                memStream = new MemoryStream(wc.DownloadData(url));
            }

            return memStream;
        }

        /// <summary>Gets the file size of a file</summary>
        /// <param name="file">The full path to the file</param>
        /// <returns>A UInt64 value indicating the file size</returns>
        public static ulong GetFileSize(string file)
        {
            if (!File.Exists(file))
            {
                return 0;
            }

            return (ulong)new FileInfo(file).Length;
        }

        /// <summary>Gets the SHA-2 Hash of a file</summary>
        /// <param name="file">The full path to the file to calculate the hash</param>
        /// <returns>The SHA-2 Hash of the file</returns>
        public static string GetHash(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            var buff = new StringBuilder(10);
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 8192))
            {
                using (var sha2 = new SHA256Managed())
                {
                    sha2.ComputeHash(stream);
                    foreach (var hashByte in sha2.Hash)
                    {
                        buff.Append(String.Format("{0:X1}", hashByte));
                    }
                }
            }

            return buff.ToString();
        }

        /// <summary>Gets the preferred localized string from a collection of localized strings</summary>
        /// <param name="localeStrings">A collection of <see cref="LocaleString"/>'s</param>
        /// <returns>a localized string</returns>
        public static string GetLocaleString(Collection<LocaleString> localeStrings)
        {
            if (localeStrings == null)
            {
                throw new ArgumentNullException(@"localeStrings");
            }

            foreach (var t in localeStrings.Where(t => t.Lang == Locale))
            {
                return t.Value;
            }

            return localeStrings[0].Value;
        }

        /// <summary>Gets and converts the registry key</summary>
        /// <param name="registryKey">The registry key</param>
        /// <param name="is64Bit">if set to <see langword="true"/> the application is 64 bit</param>
        /// <returns>The parsed registry key</returns>
        public static string GetRegistryKey(string registryKey, bool is64Bit)
        {
            if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                if (!is64Bit)
                {
                    if (!registryKey.Contains(@"SOFTWARE\Wow6432Node", StringComparison.OrdinalIgnoreCase))
                    {
                        registryKey = registryKey.Replace(@"SOFTWARE\", @"SOFTWARE\Wow6432Node\", true);
                    }
                }
            }

            registryKey = registryKey.Replace("HKLM", "HKEY_LOCAL_MACHINE", true);
            registryKey = registryKey.Replace("HKCU", "HKEY_CURRENT_USER", true);
            registryKey = registryKey.Replace("HKCR", "HKEY_CLASSES_ROOT", true);
            registryKey = registryKey.Replace("HKU", "HKEY_USERS", true);
            return registryKey;
        }

        /// <summary>Gets a string from a registry path</summary>
        /// <param name="registryKey">The path to the registry key</param>
        /// <param name="valueName">The value name to get the data from</param>
        /// <param name="is64Bit">if set to <see langword="true"/> the application is 64 bit</param>
        /// <returns>The value retrieved from the registry path</returns>
        public static string GetRegistryValue(string registryKey, string valueName, bool is64Bit)
        {
            try
            {
                if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    if (!is64Bit)
                    {
                        if (!registryKey.Contains(@"SOFTWARE\Wow6432Node", StringComparison.OrdinalIgnoreCase))
                        {
                            registryKey = registryKey.Replace(@"SOFTWARE\", @"SOFTWARE\Wow6432Node\", true);
                        }
                    }
                }

                registryKey = registryKey.Replace("HKLM", "HKEY_LOCAL_MACHINE", true);
                registryKey = registryKey.Replace("HKCU", "HKEY_CURRENT_USER", true);
                registryKey = registryKey.Replace("HKCR", "HKEY_CLASSES_ROOT", true);
                registryKey = registryKey.Replace("HKU", "HKEY_USERS", true);

                if (valueName == "@")
                {
                    valueName = string.Empty;
                }

                registryKey = Registry.GetValue(registryKey, valueName, null).ToString();
            }
            catch (UnauthorizedAccessException)
            {
                registryKey = null;
            }

            return registryKey;
        }

        /// <summary>Checks to see if path is a registry key</summary>
        /// <param name="path">The path to check</param>
        /// <returns><see langword="true"/> if the path is a registry key otherwise, <see langword="false"/></returns>
        public static bool IsRegistryKey(string path)
        {
            return Regex.IsMatch(path, @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\", RegexOptions.IgnoreCase);
        }

        /// <summary>Replaces a string within a string</summary>
        /// <param name="value">the string that will be searched</param>
        /// <param name="find">a string to find in the complete string</param>
        /// <param name="replaceValue">a string to use to replace the find string in the complete string</param>
        /// <param name="ignoreCase">if set to <see langword="true"/> case is ignored</param>
        /// <returns>The replacement string</returns>
        public static string Replace(this string value, string find, string replaceValue, bool ignoreCase)
        {
            if (value == null || find == null)
            {
                return value;
            }

            // Get input string length
            var expressionLength = value.Length;

            var findLength = find.Length;

            // Check inputs
            if (0 == expressionLength || 0 == findLength || findLength > expressionLength)
            {
                return value;
            }

            var sb = new StringBuilder(expressionLength);

            var pos = 0;

            while (pos + findLength <= expressionLength)
            {
                if (0 == string.Compare(value, pos, find, 0, findLength, ignoreCase, CultureInfo.CurrentCulture))
                {
                    // Add the replaced string
                    sb.Append(replaceValue);

                    pos += findLength;

                    continue;
                }

                // Advance one character
                sb.Append(value, pos++, 1);
            }

            // Append remaining characters
            sb.Append(value, pos, expressionLength - pos);

            // Return string
            return sb.ToString();
        }

        /// <summary>Reports the error that occurred to a log file</summary>
        /// <param name="message">The message to write in the log</param>
        /// <param name="directoryStore">The directory to store the log</param>
        public static void ReportError(string message, string directoryStore)
        {
            using (var tw = new StreamWriter(directoryStore + "error.log", true))
            {
                tw.WriteLine(DateTime.Now + ": " + message);
            }
        }

        /// <summary>Reports the error that occurred to a log file</summary>
        /// <param name="exception">The exception to write in the log</param>
        /// <param name="directoryStore">The directory to store the log</param>
        public static void ReportError(Exception exception, string directoryStore)
        {
            if (exception == null)
            {
                return;
            }

            using (var tw = new StreamWriter(directoryStore + "error.log", true))
            {
                tw.WriteLine(DateTime.Now + ": " + exception.Source);
                tw.WriteLine(DateTime.Now + ": " + exception.Message);
                tw.WriteLine(DateTime.Now + ": " + exception.StackTrace);

                if (exception.TargetSite != null)
                {
                    tw.WriteLine(DateTime.Now + ": " + exception.TargetSite.Name);
                }

                if (exception.InnerException != null)
                {
                    tw.WriteLine(DateTime.Now + ": " + exception.InnerException.Message);
                    tw.WriteLine(DateTime.Now + ": " + exception.InnerException.Source);
                    tw.WriteLine(DateTime.Now + ": " + exception.InnerException.StackTrace);

                    if (exception.TargetSite != null)
                    {
                        tw.WriteLine(DateTime.Now + ": " + exception.TargetSite.Name);
                    }

                    if (exception.InnerException.InnerException != null)
                    {
                        tw.WriteLine(DateTime.Now + ": " + exception.InnerException.InnerException.Message);
                        tw.WriteLine(DateTime.Now + ": " + exception.InnerException.InnerException.Source);
                        tw.WriteLine(DateTime.Now + ": " + exception.InnerException.InnerException.StackTrace);

                        if (exception.TargetSite != null)
                        {
                            tw.WriteLine(DateTime.Now + ": " + exception.TargetSite.Name);
                        }
                    }
                }
            }
        }

        /// <summary>Serializes an object into a file</summary>
        /// <typeparam name="T">The object type to serialize</typeparam>
        /// <param name="item">the object to serialize</param>
        /// <param name="fileName">the location of a file that will be serialized</param>
        public static void Serialize<T>(T item, Uri fileName) where T : class
        {
            var task = Task.Factory.StartNew(() => SerializeFile(item, fileName));
            task.Wait();
        }

        /// <summary>Serializes an object into a file</summary>
        /// <typeparam name="T">The object type to serialize</typeparam>
        /// <param name="item">the object to serialize</param>
        /// <param name="fileName">the location of a file that will be serialized</param>
        public static void Serialize<T>(T item, string fileName) where T : class
        {
            Serialize(item, new Uri(fileName));
        }

        /// <summary>Starts a process on the system</summary>
        /// <param name="fileName">The file to execute</param>
        /// <param name="arguments">The arguments to execute with the file</param>
        /// <param name="wait">if set to <see langword="true"/> the calling thread will be blocked until process has exited</param>
        /// <param name="hidden">if set to <see langword="true"/> the process will execute with no UI</param>
        /// <returns><see langword="true"/> if the process has executed successfully</returns>
        public static bool StartProcess(string fileName, string arguments = null, bool wait = false, bool hidden = true)
        {
            bool success;
            var process = new Process
                {
                    StartInfo =
                        {
                            FileName = fileName, 
                            UseShellExecute = true
                        }
                };

            if (arguments != null)
            {
                process.StartInfo.Arguments = arguments;
            }

            if (hidden)
            {
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            try
            {
                process.Start();
                success = true;
            }
            catch (Exception e)
            {
                if (!(e is OperationCanceledException || e is UnauthorizedAccessException || e is InvalidOperationException || e is NotSupportedException))
                {
                    throw;
                }

                success = false;
                ReportError(e, UserStore);
            }
            finally
            {
                process.Dispose();
            }

            if (success)
            {
                if (wait)
                {
                    process.WaitForExit();
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>Determines if a string contains another string</summary>
        /// <param name="original">The original string to check</param>
        /// <param name="value">The value to check the string for</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns><see langword="true"/> if the string contains the specified value; otherwise, <see langword="false"/>.</returns>
        private static bool Contains(this string original, string value, StringComparison comparisonType)
        {
            return original.IndexOf(value, comparisonType) >= 0;
        }

        /// <summary>DeSerializes an object</summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="fileName">the file that contains the object to DeSerialize</param>
        /// <returns>returns the object</returns>
        private static T DeserializeFile<T>(Uri fileName) where T : class
        {
            var sourceFile = fileName.AbsoluteUri;
            if (File.Exists(sourceFile))
            {
                try
                {
                    T obj;
                    using (var file = File.OpenRead(sourceFile))
                    {
                        obj = Serializer.Deserialize<T>(file);
                    }

                    return obj;
                }
                catch (InvalidCastException e)
                {
                    if (SerializationError != null)
                    {
                        SerializationError(null, new SerializationErrorEventArgs(e, fileName));
                    }
                }
            }

            return null;
        }

        /// <summary>DeSerializes an object</summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="stream">The Stream to deserialize</param>
        /// <param name="sourceUrl">The <see cref="Uri"/> to the source stream that is being deserialized</param>
        /// <returns>returns the object</returns>
        private static T DeserializeStream<T>(Stream stream, Uri sourceUrl) where T : class
        {
            try
            {
                return Serializer.Deserialize<T>(stream);
            }
            catch (Exception e)
            {
                if (!(e is IOException || e is ProtoException))
                {
                    throw;
                }

                ReportError(e, UserStore);
                if (SerializationError != null)
                {
                    SerializationError(null, new SerializationErrorEventArgs(e, sourceUrl));
                }
            }

            return null;
        }

        /// <summary>Replaces a string within a string</summary>
        /// <param name="sb">The <see cref="StringBuilder"/> object</param>
        /// <param name="find">a string to find in the complete string</param>
        /// <param name="replaceValue">a string to use to replace the find string in the complete string</param>
        /// <param name="ignoreCase">if set to <see langword="true"/> case is ignored</param>
        /// <returns>The <see cref="StringBuilder"/> with replacements</returns>
        private static StringBuilder Replace(this StringBuilder sb, string find, string replaceValue, bool ignoreCase)
        {
            if (sb == null || find == null)
            {
                return sb;
            }

            var str = sb.ToString();

            // Get input string length
            var expressionLength = str.Length;

            var findLength = find.Length;

            // Check inputs
            if (0 == expressionLength || 0 == findLength || findLength > expressionLength)
            {
                return sb;
            }

            var sb2 = new StringBuilder(expressionLength);

            var pos = 0;

            while (pos + findLength <= expressionLength)
            {
                if (0 == string.Compare(str, pos, find, 0, findLength, ignoreCase, CultureInfo.CurrentCulture))
                {
                    // Add the replaced string
                    sb2.Append(replaceValue);

                    pos += findLength;

                    continue;
                }

                // Advance one character
                sb2.Append(str, pos++, 1);
            }

            // Append remaining characters
            sb2.Append(str, pos, expressionLength - pos);

            sb = sb2;

            // Return string
            return sb;
        }

        /// <summary>Serializes an object into a file</summary>
        /// <typeparam name="T">the object type to serialize</typeparam>
        /// <param name="item">the object to serialize</param>
        /// <param name="fileName">the location of a file that will be serialized</param>
        private static void SerializeFile<T>(T item, Uri fileName) where T : class
        {
            try
            {
                var fileDestination = fileName.PathAndQuery;
                if (File.Exists(fileDestination))
                {
                    using (var file = File.Open(fileDestination, FileMode.Truncate))
                    {
                        Serializer.Serialize(file, item);
                    }
                }
                else
                {
                    using (var file = File.Open(fileDestination, FileMode.CreateNew))
                    {
                        Serializer.Serialize(file, item);
                    }
                }
            }
            catch (Exception e)
            {
                if (!(e is IOException || e is ProtoException || e is UnauthorizedAccessException))
                {
                    throw;
                }

                ReportError(e, UserStore);
                if (SerializationError != null)
                {
                    SerializationError(null, new SerializationErrorEventArgs(e, fileName));
                }
            }
        }

        #endregion
    }
}