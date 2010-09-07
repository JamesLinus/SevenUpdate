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

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

#endregion

namespace SevenUpdate
{

    #region Event Args

    /// <summary>
    ///   Indicates a type of error that can occur
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        ///   An error that occurred while trying to download updates
        /// </summary>
        DownloadError,
        /// <summary>
        ///   An error that occurred while trying to install updates
        /// </summary>
        InstallationError,
        /// <summary>
        ///   A general network connection error
        /// </summary>
        FatalNetworkError,
        /// <summary>
        ///   An unspecified error, non fatal
        /// </summary>
        GeneralErrorNonFatal,
        /// <summary>
        ///   An unspecified error that prevents Seven Update from continuing
        /// </summary>
        FatalError,
        /// <summary>
        ///   An error that occurs while searching for updates
        /// </summary>
        SearchError
    }

    /// <summary>
    ///   Provides event data for the ErrorOccurred event
    /// </summary>
    public sealed class ErrorOccurredEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "exception">the exception that occurred</param>
        /// <param name = "type">the type of error that occurred</param>
        public ErrorOccurredEventArgs(string exception, ErrorType type)
        {
            Exception = exception;
            Type = type;
        }

        /// <summary>
        ///   Gets the Exception information of the error that occurred
        /// </summary>
        public string Exception { get; private set; }

        /// <summary>
        ///   Gets the <see cref = "ErrorType" /> of the error that occurred
        /// </summary>
        public ErrorType Type { get; private set; }
    }

    /// <summary>
    ///   Provides event data for the SerializationError event
    /// </summary>
    public sealed class SerializationErrorEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "e">The exception data</param>
        /// <param name = "file">The full path of the file</param>
        public SerializationErrorEventArgs(Exception e, string file)
        {
            Exception = e;
            File = file;
        }

        /// <summary>
        ///   Gets the exception data
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        ///   Gets the full path of the file
        /// </summary>
        public string File { get; set; }
    }

    /// <summary>
    ///   Provides event data for the SerializationError event
    /// </summary>
    public sealed class ProcessEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        public ProcessEventArgs(Process process)
        {
            this.Process = process;
        }

        /// <summary>
        ///   Gets the exception data
        /// </summary>
        public Process Process {  get; set; }
    }

    #endregion

    /// <summary>
    ///   Methods that are shared between other classes
    /// </summary>
    public static class Base
    {
        #region Fields

        /// <summary>
        ///   The application directory of Seven Update
        /// </summary>
        public static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";

        /// <summary>
        ///   The all users application data location
        /// </summary>
        public static readonly string AllUserStore = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Seven Software\Seven Update\";

        /// <summary>
        ///   The location of the list of applications Seven Update can update
        /// </summary>
        public static readonly string AppsFile = AllUserStore + "Apps.sul";

        /// <summary>
        ///   The location of the application settings file
        /// </summary>
        public static readonly string ConfigFile = AllUserStore + "App.config";

        /// <summary>
        ///   The location of the hidden updates file
        /// </summary>
        public static readonly string HiddenFile = AllUserStore + "Hidden.suh";

        /// <summary>
        ///   The location of the update history file
        /// </summary>
        public static readonly string HistoryFile = AllUserStore + "History.suh";

        /// <summary>
        ///   The user application data location
        /// </summary>
        public static readonly string UserStore = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Seven Software\Seven Update\";

        #endregion

        #region Properties

        /// <summary>
        ///   Specifies if a reboot is needed
        /// </summary>
        public static bool RebootNeeded { get { return File.Exists(AllUserStore + @"reboot.lock"); } }

        /// <summary>
        ///   Gets or Sets the ISO language code
        /// </summary>
        public static string Locale { get; set; }

        #endregion

        /// <summary>
        ///   Checks to see if path is a registry key
        /// </summary>
        /// <param name = "path">The path to check</param>
        /// <returns>True if path is a registry key, otherwise false</returns>
        public static bool IsRegistryKey(string path)
        {
            const string pattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";
            return Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///   Waits for the process to exit then triggers an event
        /// </summary>
        /// <param name = "process"></param>
        private static void WaitForExit(Process process)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          try
                                          {
                                              process.WaitForExit();
                                              if (ProcessExited != null)
                                                  ProcessExited(null, new ProcessEventArgs(process));
                                          }
                                          catch
                                          {
                                              if (ProcessExited != null)
                                                  ProcessExited(null, new ProcessEventArgs(null));
                                          }
                                      });
        }

        /// <summary>
        ///   Starts a process on the system
        /// </summary>
        /// <param name = "fileName">The file to execute</param>
        /// <param name = "arguments">The arguments to execute with the file</param>
        /// <param name = "wait">Specifies if Seven Update should wait until the process has finished executing</param>
        /// <param name = "hidden">Specifes if the process should be executed with no UI visibile</param>
        /// <returns />
        public static bool StartProcess(string fileName, string arguments = null, bool wait = false, bool hidden = true)
        {
            var proc = new Process {StartInfo = {FileName = fileName, UseShellExecute = true}};

            if (arguments != null)
                proc.StartInfo.Arguments = arguments;

            if (hidden)
            {
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            try
            {
                proc.Start();
                if (wait)
                    WaitForExit(proc);
                return true;
            }
            catch (Exception e)
            {
                if (e.Message != @"The operation was canceled by the user")
                    ReportError(e, UserStore);

                proc.Dispose();
                return false;
            }
        }

        public static Stream DownloadFile(string url)
        {
            //Get a data stream from the url
            var wc = new WebClient();
            return new MemoryStream(wc.DownloadData(url));
        }

        #region Serialization Methods

        /// <summary>
        ///   DeSerializes an object
        /// </summary>
        /// <typeparam name = "T">the object to deserialize</typeparam>
        /// <param name = "fileName">the file that contains the object to DeSerialize</param>
        /// <param name = "usePrefix"><c>True</c> to Deserialize with a length prefix, otherwise <c>false</c></param>
        /// <returns>returns the object</returns>
        private static T DeserializeFile<T>(string fileName, bool usePrefix) where T : class
        {
            if (File.Exists(fileName))
            {
                try
                {
                    using (var file = File.OpenRead(fileName))
                    {
                        T obj = usePrefix ? Serializer.DeserializeWithLengthPrefix<T>(file, PrefixStyle.Fixed32) : Serializer.Deserialize<T>(file);

                        file.Close();
                        return obj;
                    }
                }
                catch (Exception e)
                {
                    if (SerializationError != null)
                        SerializationError(null, new SerializationErrorEventArgs(e, fileName));
                }
            }

            return null;
        }

        /// <summary>
        ///   DeSerializes an object
        /// </summary>
        /// <typeparam name = "T">the object to deserialize</typeparam>
        /// <param name = "stream">The Stream to deserialize</param>
        /// <param name = "sourceUrl">The url to the source stream that is being deserialized</param>
        /// <param name = "usePrefix"><c>True</c> to Deserialize with a length prefix, otherwise <c>false</c></param>
        /// <returns>returns the object</returns>
        private static T DeserializeStream<T>(Stream stream, string sourceUrl, bool usePrefix) where T : class
        {
            try
            {
                return usePrefix ? Serializer.DeserializeWithLengthPrefix<T>(stream, PrefixStyle.Fixed32) : Serializer.Deserialize<T>(stream);
            }
            catch (Exception e)
            {
                if (SerializationError != null)
                    SerializationError(null, new SerializationErrorEventArgs(e, sourceUrl));
            }

            return null;
        }

        /// <summary>
        ///   DeSerializes an object
        /// </summary>
        /// <typeparam name = "T">the object to deserialize</typeparam>
        /// <param name = "fileName">the file that contains the object to DeSerialize</param>
        /// <param name = "usePrefix"><c>True</c> to Deserialize with a length prefix, otherwise <c>false</c></param>
        /// <returns>returns the object</returns>
        public static T Deserialize<T>(string fileName, bool usePrefix = false) where T : class
        {
            var task = Task.Factory.StartNew(() => DeserializeFile<T>(fileName, usePrefix));
            task.Wait();
            return task.Result;
        }

        /// <summary>
        ///   DeSerializes an object
        /// </summary>
        /// <typeparam name = "T">the object to deserialize</typeparam>
        /// <param name = "stream">The Stream to deserialize</param>
        /// <param name = "sourceUrl">The url to the source stream that is being deserialized</param>
        /// <param name = "usePrefix"><c>True</c> to Deserialize with a length prefix, otherwise <c>false</c></param>
        /// <returns>returns the object</returns>
        public static T Deserialize<T>(Stream stream, string sourceUrl, bool usePrefix = false) where T : class
        {
            var task = Task.Factory.StartNew(() => DeserializeStream<T>(stream, sourceUrl, usePrefix));
            task.Wait();
            return task.Result;
        }

        /// <summary>
        ///   Serializes an object into a file
        /// </summary>
        /// <typeparam name = "T">the object</typeparam>
        /// <param name = "item">the object to serialize</param>
        /// <param name = "fileName">the location of a file that will be serialized</param>
        /// <param name = "usePrefix"><c>True</c> to Serialize with a length prefix, otherwise <c>false</c></param>
        private static void SerializeFile<T>(T item, string fileName, bool usePrefix) where T : class
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (var file = File.Open(fileName, FileMode.Truncate))
                    {
                        if (usePrefix)
                            Serializer.SerializeWithLengthPrefix(file, item, PrefixStyle.Fixed32);
                        else
                            Serializer.Serialize(file, item);
                        file.Close();
                    }
                }
                else
                {
                    using (var file = File.Open(fileName, FileMode.CreateNew))
                    {
                        if (usePrefix)
                            Serializer.SerializeWithLengthPrefix(file, item, PrefixStyle.Fixed32);
                        else
                            Serializer.Serialize(file, item);
                        file.Close();
                    }
                }
            }
            catch (Exception e)
            {
                if (SerializationError != null)
                    SerializationError(null, new SerializationErrorEventArgs(e, fileName));
            }
        }

        /// <summary>
        ///   Serializes an object into a file
        /// </summary>
        /// <typeparam name = "T">the object</typeparam>
        /// <param name = "item">the object to serialize</param>
        /// <param name = "fileName">the location of a file that will be serialized</param>
        /// <param name = "usePrefix"><c>True</c> to Serialize with a length prefix, otherwise <c>false</c></param>
        public static void Serialize<T>(T item, string fileName, bool usePrefix = false) where T : class
        {
            var task = Task.Factory.StartNew(() => SerializeFile(item, fileName, usePrefix));
            task.Wait();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///   Occurs when an error occurs while serializing or deserializing a object/file
        /// </summary>
        public static event EventHandler<SerializationErrorEventArgs> SerializationError;

        /// <summary>
        ///   Occurs when a process has exited
        /// </summary>
        public static event EventHandler<ProcessEventArgs> ProcessExited;

        #endregion

        #region Conversions

        /// <summary>
        ///   Gets the preferred localized string from a collection of localized strings
        /// </summary>
        /// <param name = "localeStrings">A collection of <see cref = "LocaleString" />'s</param>
        /// <returns>a localized string</returns>
        public static string GetLocaleString(Collection<LocaleString> localeStrings)
        {
            foreach (LocaleString t in localeStrings.Where(t => t.Lang == Locale))
                return t.Value;
            return localeStrings[0].Value;
        }

        /// <summary>
        ///   Gets a string from a registry path
        /// </summary>
        /// <param name = "registryKey">The path to the registry key</param>
        /// <param name = "valueName">The value name to get the data from</param>
        /// <param name = "is64Bit">Specifies if the application is 64 bit</param>
        /// <returns></returns>
        public static string GetRegistryPath(string registryKey, string valueName, bool is64Bit)
        {
            try
            {
                if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    if (!is64Bit && registryKey.ToUpper().Contains(@"SOFTWARE\") && !registryKey.ToUpper().Contains(@"SOFTWARE\WOW6432NODE"))
                        registryKey = registryKey.Replace(@"SOFTWARE\", @"SOFTWARE\Wow6432Node\");
                }
                if (valueName == "@")
                    valueName = "";
                registryKey = Registry.GetValue(registryKey, valueName, null).ToString();
            }
            catch
            {
                registryKey = null;
            }

            return registryKey;
        }

        /// <summary>
        ///   Expands the file location variables
        /// </summary>
        /// <param name = "path">a string that contains a file path</param>
        /// <param name = "dir">a string that contains a directory</param>
        /// <param name = "is64Bit">Specifies if the application is 64 bit</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, string dir, bool is64Bit)
        {
            path = Replace(path, "[AppDir]", ConvertPath(dir, true, is64Bit));
            path = Replace(path, "[DownloadDir]", ConvertPath(dir, true, is64Bit));
            return ConvertPath(path, true, is64Bit);
        }

        /// <summary>
        ///   Expands the system variables in a string
        /// </summary>
        /// <param name = "path">a string that contains a file path</param>
        /// <param name = "expand"><c>true</c> to expand system variable, <c>false</c> to converts paths into system variables</param>
        /// <param name = "is64Bit">Specifies if the application is 64 bit</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, bool expand, bool is64Bit)
        {
            if (path == null)
                return path;

            var stringBuilder = new StringBuilder(path);

            if (expand == false)
            {
                var path2 = new StringBuilder();
                NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_PROGRAMS, false);
                stringBuilder.Replace(path2.ToString(), "%ALLUSERSSTARTMENUPROGRAMS%");
                NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_STARTMENU, false);
                stringBuilder.Replace(path2.ToString(), "%ALLUSERSSTARTMENU%");

                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "%STARTMENUPROGRAMS%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "%STARTMENU%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "%DOCUMENTS%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "%MUSIC%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "%PICTURES%");

                if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%");
                    stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%");

                    if (is64Bit)
                    {
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%");
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%");
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%");
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%");
                    }
                    else
                    {
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "%PROGRAMFILES(x86)%");
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"), "%COMMONPROGRAMFILES(x86)%");
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%");
                        stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%");
                    }
                }
                else
                {
                    stringBuilder.Replace(Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"), "%COMMONPROGRAMFILES%");
                    stringBuilder.Replace(Environment.GetEnvironmentVariable("ProgramFiles"), "%PROGRAMFILES%");
                }

                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "%APPDATA%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "%LOCALAPPDATA%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "%STARTUP%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Templates), "%TEMPLATES%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "%DESKTOP%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("TMP"), "%TMP%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("TEMP"), "%TEMP%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("USERPROFILE"), "%USERPROFILE%");
                stringBuilder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.System), "%SYSTEM32%");
                stringBuilder.Replace(Environment.UserName, "%USERNAME%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "%LOCALAPPDATA%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("PROGRAMDATA"), "%PROGRAMDATA%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("PUBLIC"), "%PUBLIC%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("HOMEPATH"), "%HOMEPATH%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("SYSTEMROOT"), "%SYSTEMROOT%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("WINDIR"), "%WINDIR%");
                stringBuilder.Replace(Environment.GetEnvironmentVariable("SYSTEMDRIVE"), "%SYSTEMDRIVE%");
            }
            else
            {
                var path2 = new StringBuilder();

                NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_PROGRAMS, false);

                stringBuilder.Replace("%ALLUSERSSTARTMENUPROGRAMS%", path2.ToString());

                NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_STARTMENU, false);

                stringBuilder.Replace("%ALLUSERSSTARTMENU%", path2.ToString());
                stringBuilder.Replace("%STARTMENUPROGRAMS%", Environment.GetFolderPath(Environment.SpecialFolder.Programs));
                stringBuilder.Replace("%STARTMENU%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
                stringBuilder.Replace("%DOCUMENTS%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                stringBuilder.Replace("%MUSIC%", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                stringBuilder.Replace("%PICTURES%", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

                if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    stringBuilder.Replace("%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"));
                    stringBuilder.Replace("%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
                    if (is64Bit)
                    {
                        stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"));
                        stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles"));
                    }
                    else
                    {
                        stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES(x86)"));
                        stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
                    }
                }
                else
                {
                    stringBuilder.Replace("%COMMONPROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"));
                    stringBuilder.Replace("%PROGRAMFILES(x86)%", Environment.GetEnvironmentVariable("ProgramFiles"));
                    stringBuilder.Replace("%COMMONPROGRAMFILES%", Environment.GetEnvironmentVariable("COMMONPROGRAMFILES"));
                    stringBuilder.Replace("%PROGRAMFILES%", Environment.GetEnvironmentVariable("ProgramFiles"));
                }

                stringBuilder.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                stringBuilder.Replace("%LOCALAPPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                stringBuilder.Replace("%STARTUP%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                stringBuilder.Replace("%TEMPLATES%", Environment.GetFolderPath(Environment.SpecialFolder.Templates));
                stringBuilder.Replace("%DESKTOP%", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                stringBuilder.Replace("%TEMP%", Environment.GetEnvironmentVariable("TEMP"));
                stringBuilder.Replace("%TMP%", Environment.GetEnvironmentVariable("TMP"));
                stringBuilder.Replace("%USERPROFILE%", Environment.GetEnvironmentVariable("USERPROFILE"));
                stringBuilder.Replace("%SYSTEM32%", Environment.GetFolderPath(Environment.SpecialFolder.System));
                stringBuilder.Replace("%USERNAME%", Environment.UserName);
                stringBuilder.Replace("%LOCALAPPDATA%", Environment.GetEnvironmentVariable("LOCALAPPDATA"));
                stringBuilder.Replace("%PROGRAMDATA%", Environment.GetEnvironmentVariable("PROGRAMDATA"));
                stringBuilder.Replace("%PUBLIC%", Environment.GetEnvironmentVariable("PUBLIC"));
                stringBuilder.Replace("%HOMEPATH%", Environment.GetEnvironmentVariable("HOMEPATH"));
                stringBuilder.Replace("%SYSTEMROOT%", Environment.GetEnvironmentVariable("SYSTEMROOT"));
                stringBuilder.Replace("%WINDIR%", Environment.GetEnvironmentVariable("WINDIR"));
                stringBuilder.Replace("%SYSTEMDRIVE%", Environment.GetEnvironmentVariable("SYSTEMDRIVE"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        ///   Converts bytes into the proper increments depending on size
        /// </summary>
        /// <param name = "bytes">the fileSize in bytes</param>
        /// <returns>returns formatted string of converted bytes</returns>
        public static string ConvertFileSize(ulong bytes)
        {
            if (bytes >= 1073741824)
                return String.Format("{0:##.##}", bytes/1073741824) + " GB";
            if (bytes >= 1048576)
                return String.Format("{0:##.##}", bytes/1048576) + " MB";
            if (bytes >= 1024)
                return String.Format("{0:##.##}", bytes/1024) + " KB";
            if (bytes < 1024)
                return bytes + " Bytes";
            return "0 Bytes";
        }

        /// <summary>
        ///   Replaces a string within a string
        /// </summary>
        /// <param name = "complete">the string that will be searched</param>
        /// <param name = "find">a string to find in the complete string</param>
        /// <param name = "replace">a string to use to replace the find string in the complete string</param>
        /// <returns>a string that has the find value replace by the new value</returns>
        private static string Replace(string complete, string find, string replace)
        {
            // Get input string length
            var expressionLength = complete.Length;

            var findLength = find.Length;

            // Check inputs
            if (0 == expressionLength || 0 == findLength || findLength > expressionLength)
                return complete;

            var sbRet = new StringBuilder(expressionLength);

            var pos = 0;

            while (pos + findLength <= expressionLength)
            {
                if (0 == string.Compare(complete, pos, find, 0, findLength, true))
                {
                    // Add the replaced string
                    sbRet.Append(replace);

                    pos += findLength;

                    continue;
                }

                // Advance one character
                sbRet.Append(complete, pos++, 1);
            }

            // Append remaining characters
            sbRet.Append(complete, pos, expressionLength - pos);
            // Return string
            return sbRet.ToString();
        }

        #endregion

        #region Error Reporting

        /// <summary>
        ///   Reports the error that occurred to a log file
        /// </summary>
        /// <param name = "message">The message to write in the log</param>
        /// <param name = "directoryStore">The directory to store the log</param>
        public static void ReportError(string message, string directoryStore)
        {
            TextWriter tw = new StreamWriter(directoryStore + "error.log", true);

            tw.WriteLine(DateTime.Now + ": " + message);

            tw.Close();
        }

        /// <summary>
        ///   Reports the error that occurred to a log file
        /// </summary>
        /// <param name = "exception">The exception to write in the log</param>
        /// <param name = "directoryStore">The directory to store the log</param>
        public static void ReportError(Exception exception, string directoryStore)
        {
            TextWriter tw = new StreamWriter(directoryStore + "error.log", true);
            tw.WriteLine(DateTime.Now + ": " + exception.Source);
            tw.WriteLine(DateTime.Now + ": " + exception.Message);
            tw.WriteLine(DateTime.Now + ": " + exception.StackTrace);

            if (exception.TargetSite != null)
                tw.WriteLine(DateTime.Now + ": " + exception.TargetSite.Name);

            if (exception.InnerException != null)
            {
                tw.WriteLine(DateTime.Now + ": " + exception.InnerException.Message);
                tw.WriteLine(DateTime.Now + ": " + exception.InnerException.Source);
                tw.WriteLine(DateTime.Now + ": " + exception.InnerException.StackTrace);

                if (exception.TargetSite != null)
                    tw.WriteLine(DateTime.Now + ": " + exception.TargetSite.Name);

                if (exception.InnerException.InnerException != null)
                {
                    tw.WriteLine(DateTime.Now + ": " + exception.InnerException.InnerException.Message);
                    tw.WriteLine(DateTime.Now + ": " + exception.InnerException.InnerException.Source);
                    tw.WriteLine(DateTime.Now + ": " + exception.InnerException.InnerException.StackTrace);

                    if (exception.TargetSite != null)
                        tw.WriteLine(DateTime.Now + ": " + exception.TargetSite.Name);
                }
            }

            tw.Close();
        }

        #endregion

        #region File size Functions

        /// <summary>
        ///   Gets the file size of a file
        /// </summary>
        /// <param name = "file">The fullpath to the file</param>
        /// <returns>A ulong value indicating the file size</returns>
        public static ulong GetFileSize(string file)
        {
            return (ulong) new FileInfo(file).Length;
        }

        #endregion

        #region Hash Functions

        /// <summary>
        ///   Gets the SHA-2 Hash of a file
        /// </summary>
        /// <param name = "file">The full path to the file to calculate the hash</param>
        /// <returns>The SHA-2 Hash of the file</returns>
        public static string GetHash(string file)
        {
            file = ConvertPath(file, true, false);
            if (!File.Exists(file))
                return null;
            var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);

            var sha2 = new SHA256Managed();

            sha2.ComputeHash(stream);

            stream.Close();

            var buff = new StringBuilder(10);

            foreach (var hashByte in sha2.Hash)
                buff.Append(String.Format("{0:X1}", hashByte));

            return buff.ToString();
        }

        #endregion
    }
}