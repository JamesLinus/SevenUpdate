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
    using System.ComponentModel;
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

        /// <summary>The application directory of the current assembly</summary>
        public static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        #endregion

        #region Events

        /// <summary>Occurs when an error occurrs</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        #endregion

        #region Properties

        /// <summary>Gets or sets the ISO language code</summary>
        /// <value>The locale.</value>
        public static string Locale { get; set; }

        /// <summary>Gets a value indicating whether if a reboot is needed</summary>
        /// <value><see langword = "true" /> if a reboot is needed otherwise, <see langword = "false" />.</value>
        public static bool RebootNeeded { get; internal set; }

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

        /// <summary>Expands the file location variables and expands the %INSTALLDIR% variable</summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="directory">a string that contains a directory</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, string directory)
        {
            return ConvertPath(path, directory, Platform.x86, null);
        }

        /// <summary>Expands the file location variables and expands the %INSTALLDIR% variable</summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="directory">a string that contains a directory</param>
        /// <param name="platform">a value that indicates what cpu architecture the application supports</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, string directory, Platform platform)
        {
            return ConvertPath(path, directory, platform, null);
        }

        /// <summary>Expands the file location variables and expands the %INSTALLDIR% variable</summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="directory">a string that contains a directory</param>
        /// <param name="platform">a value that indicates what cpu architecture the application supports</param>
        /// <param name="valueName">a string that contains a value name of the registry key that contains the directory location, this parameter is optional and can be <see langword="null"/></param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, string directory, Platform platform, string valueName)
        {
            path = path.Replace("%INSTALLDIR%", !IsRegistryKey(directory) ? ConvertPath(directory, true, platform) : ConvertPath(GetRegistryValue(directory, valueName, platform), true, platform), true);
            path = path.Replace("%DOWNLOADURL%", ConvertPath(directory, true, platform), true);
            return ConvertPath(path, true, platform);
        }

        /// <summary>Expands the system variables in a string, not for use with InstallDir or DownloadUri variables</summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="expand"><see langword="true"/> to expand system variable, <see langword="false"/> to converts paths into system variables</param>
        /// <param name="platform">a value that indicates what cpu architecture the application supports</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, bool expand, Platform platform)
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
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%", true);
                stringBuilder = stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%", true);
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
                    switch (platform)
                    {
                        case Platform.AnyCPU:
                        case Platform.x64:
                            stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), true);
                            stringBuilder = stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles"), true);
                            stringBuilder = stringBuilder.Replace("%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"), true);
                            stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), true);
                            break;
                        case Platform.x86:
                            stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), true);
                            stringBuilder = stringBuilder.Replace("%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"), true);
                            stringBuilder = stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), true);
                            stringBuilder = stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"), true);
                            break;
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
            try
            {
                var task = Task.Factory.StartNew(() => DeserializeFile<T>(fileName));
                task.Wait();
                return task.Result;
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions.OfType<FileNotFoundException>())
                {
                    ErrorOccurred(null, new ErrorOccurredEventArgs(GetExceptionAsString(e), ErrorType.FatalError));
                    throw new FileNotFoundException(e.FileName, e);
                }

                ErrorOccurred(null, new ErrorOccurredEventArgs(GetExceptionAsString(ae), ErrorType.FatalError));
                throw;
            }
        }

        /// <summary>DeSerializes an object</summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="stream">The Stream to deserialize</param>
        /// <returns>returns the object</returns>
        public static T Deserialize<T>(Stream stream) where T : class
        {
            try
            {
                var task = Task.Factory.StartNew(() => DeserializeStream<T>(stream));
                task.Wait();
                return task.Result;
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions.OfType<FileNotFoundException>())
                {
                    ErrorOccurred(null, new ErrorOccurredEventArgs(GetExceptionAsString(e), ErrorType.FatalError));
                    throw new FileNotFoundException(e.FileName, e);
                }

                ErrorOccurred(null, new ErrorOccurredEventArgs(GetExceptionAsString(ae), ErrorType.FatalError));
                throw;
            }
        }

        /// <summary>Downloads a file</summary>
        /// <param name="url">A Uri pointing to the location of the file to download</param>
        /// <returns>the downloaded file <see cref="Stream"/></returns>
        public static Stream DownloadFile(string url)
        {
            // Get a data stream from the url
            using (var wc = new WebClient())
            {
                return new MemoryStream(wc.DownloadData(url));
            }
        }

        /// <summary>Gets data from the exception as a string</summary>
        /// <param name="exception">The exception to write in the log</param>
        /// <returns>The exception as a string</returns>
        public static string GetExceptionAsString(Exception exception)
        {
            return "<--- " + DateTime.Now + ": " + exception + " --->" + Environment.NewLine;
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

            if (localeStrings.Count < 1)
            {
                throw new ArgumentException(@"localeStrings");
            }

            foreach (var t in localeStrings.Where(t => t.Lang == Locale))
            {
                return t.Value;
            }

            return localeStrings[0].Value;
        }

        /// <summary>Gets whether the specified path is a valid absolute file path.</summary>
        /// <param name="path">Any path. OK if null or empty.</param>
        /// <returns>true if path is valid</returns>
        public static bool IsValidPath(string path)
        {
            var r = new Regex(@"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$");
            return r.IsMatch(path);
        }

        /// <summary>Converts the registry key</summary>
        /// <param name="registryKey">The registry key</param>
        /// <param name="platform">a value that indicates what cpu architecture the application supports</param>
        /// <returns>The parsed registry key</returns>
        public static string ParseRegistryKey(string registryKey, Platform platform)
        {
            if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                if (platform == Platform.x86)
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
        /// <param name="platform">a value that indicates what cpu architecture the application supports</param>
        /// <returns>The value retrieved from the registry path, returns null if the registry path does not exist</returns>
        public static string GetRegistryValue(string registryKey, string valueName, Platform platform)
        {
            if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                if (platform == Platform.x86)
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

            try
            {
                registryKey = Registry.GetValue(registryKey, valueName, null) as string;
            }
            catch (Exception ex)
            {
                if (!(ex is UnauthorizedAccessException || ex is NullReferenceException))
                {
                    ErrorOccurred(null, new ErrorOccurredEventArgs(GetExceptionAsString(ex), ErrorType.FatalError));
                    throw;
                }

                registryKey = null;
            }

            return registryKey;
        }

        /// <summary>Checks if a registry key exists</summary>
        /// <param name="registryKey">The path to the registry key</param>
        /// <param name="platform">a value that indicates what cpu architecture the application supports</param>
        /// <returns><see langword="true"/> if exists; otherwise, <see langword="false"/></returns>
        public static bool CheckRegistryKey(string registryKey, Platform platform)
        {
            return GetRegistryValue(registryKey, null, platform) != null;
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
        /// <param name="exception">The exception to write in the log</param>
        /// <param name="errorType">The type of error that ocurred</param>
        public static void ReportError(Exception exception, ErrorType errorType)
        {
            if (exception == null)
            {
                return;
            }

            if (ErrorOccurred != null)
            {
                ErrorOccurred(null, new ErrorOccurredEventArgs(GetExceptionAsString(exception), errorType));
            }
        }

        /// <summary>Serializes an object into a file</summary>
        /// <typeparam name="T">The object type to serialize</typeparam>
        /// <param name="item">the object to serialize</param>
        /// <param name="fileName">the location of a file that will be serialized</param>
        public static void Serialize<T>(T item, string fileName) where T : class
        {
            try
            {
                var task = Task.Factory.StartNew(() => SerializeFile(item, fileName));
                task.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    throw e;
                }
            }
        }

        /// <summary>Starts a process hidden on the system</summary>
        /// <param name="fileName">The file to execute</param>
        /// <returns><see langword="true"/> if the process has executed successfully</returns>
        public static bool StartProcess(string fileName)
        {
            return StartProcess(fileName, null, false, true);
        }

        /// <summary>Starts a process hidden on the system</summary>
        /// <param name="fileName">The file to execute</param>
        /// <param name="arguments">The arguments to execute with the file</param>
        /// <returns><see langword="true"/> if the process has executed successfully</returns>
        public static bool StartProcess(string fileName, string arguments)
        {
            return StartProcess(fileName, arguments, false, true);
        }

        /// <summary>Starts a process hidden on the system</summary>
        /// <param name="fileName">The file to execute</param>
        /// <param name="arguments">The arguments to execute with the file</param>
        /// <param name="wait">if set to <see langword="true"/> the calling thread will be blocked until process has exited</param>
        /// <returns><see langword="true"/> if the process has executed successfully</returns>
        public static bool StartProcess(string fileName, string arguments, bool wait)
        {
            return StartProcess(fileName, arguments, wait, true);
        }

        /// <summary>Starts a process on the system</summary>
        /// <param name="fileName">The file to execute</param>
        /// <param name="arguments">The arguments to execute with the file</param>
        /// <param name="wait">if set to <see langword="true"/> the calling thread will be blocked until process has exited</param>
        /// <param name="hidden">if set to <see langword="true"/> the process will execute with no UI</param>
        /// <returns><see langword="true"/> if the process has executed successfully</returns>
        public static bool StartProcess(string fileName, string arguments, bool wait, bool hidden)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.UseShellExecute = true;
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
                    if (wait)
                    {
                        process.WaitForExit();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    if (!(e is OperationCanceledException || e is UnauthorizedAccessException || e is InvalidOperationException || e is NotSupportedException || e is Win32Exception))
                    {
                        ErrorOccurred(null, new ErrorOccurredEventArgs(GetExceptionAsString(e), ErrorType.FatalError));
                        throw;
                    }

                    ReportError(e, ErrorType.GeneralError);
                }
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
        private static T DeserializeFile<T>(string fileName) where T : class
        {
            T obj;
            using (var file = File.OpenRead(fileName))
            {
                obj = Serializer.Deserialize<T>(file);
            }

            return obj;
        }

        /// <summary>DeSerializes an object</summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="stream">The Stream to deserialize</param>
        /// <returns>returns the object</returns>
        private static T DeserializeStream<T>(Stream stream) where T : class
        {
            stream.Position = 0;
            return Serializer.Deserialize<T>(stream);
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
        private static void SerializeFile<T>(T item, string fileName) where T : class
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (var file = File.Open(fileName, FileMode.Truncate))
                    {
                        Serializer.Serialize(file, item);
                    }
                }
                else
                {
                    using (var file = File.Open(fileName, FileMode.CreateNew))
                    {
                        Serializer.Serialize(file, item);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        #endregion
    }
}