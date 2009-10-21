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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

#endregion

namespace SevenUpdate
{
    /// <summary>
    /// Indicates a type of error that can occur
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// An error that occurred while trying to download updates
        /// </summary>
        DownloadError,
        /// <summary>
        /// An error that occurred while trying to install updates
        /// </summary>
        InstallationError,
        /// <summary>
        /// A general network connection error
        /// </summary>
        FatalNetworkError,
        /// <summary>
        /// An unspecified error, non fatal
        /// </summary>
        GeneralErrorNonFatal,
        /// <summary>
        /// An unspecified error that prevents Seven Update from continuing
        /// </summary>
        FatalError,
        /// <summary>
        /// An error that occurs while searching for updates
        /// </summary>
        SearchError
    }

    /// <summary>
    /// Methods that are shared between other classes
    /// </summary>
    public static class Shared
    {
        #region Global Vars

        /// <summary>
        /// The all users application data location
        /// </summary>
        public static readonly string AllUserStore = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Seven Update\";

        /// <summary>
        /// The location of the list of applications Seven Update can update
        /// </summary>
        public static readonly string AppsFile = AllUserStore + "Apps.sul";

        /// <summary>
        /// The location of the application settings file
        /// </summary>
        public static readonly string ConfigFile = AllUserStore + "App.config";

        /// <summary>
        /// The location of the hidden updates file
        /// </summary>
        public static readonly string HiddenFile = AllUserStore + "Hidden.suh";

        /// <summary>
        /// The location of the update history file
        /// </summary>
        public static readonly string HistoryFile = AllUserStore + "History.suh";

        /// <summary>
        /// The user application data location
        /// </summary>
        public static readonly string UserStore = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Seven Update\";

        /// <summary>
        /// Specifies if a reboot is needed
        /// </summary>
        public static bool RebootNeeded { get { return File.Exists(AllUserStore + @"reboot.lock"); } }

        /// <summary>
        /// Gets or Sets the ISO language code
        /// </summary>
        public static string Locale { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the preferred localized string from a collection of localized strings
        /// </summary>
        /// <param name="localeStrings">A collection of <see cref="LocaleString"/>'s</param>
        /// <returns>a localized string</returns>
        public static string GetLocaleString(Collection<LocaleString> localeStrings)
        {
            for (var x = 0; x < localeStrings.Count; x++)
            {
                if (localeStrings[x].Lang == Locale)
                    return localeStrings[x].Value;
            }
            return localeStrings[0].Value;
        }

        /// <summary>
        /// Expands the file location variables
        /// </summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="dir">a string that contains a directory</param>
        /// <param name="is64Bit">Specifies if the application is 64 bit</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, string dir, bool is64Bit)
        {
            path = Replace(path, "[AppDir]", ConvertPath(dir, true, is64Bit));
            path = Replace(path, "[DownloadDir]", ConvertPath(dir, true, is64Bit));
            return ConvertPath(path, true, is64Bit);
        }

        /// <summary>
        /// Expands the system variables in a string
        /// </summary>
        /// <param name="path">a string that contains a file path</param>
        /// <param name="expand"><c>true</c> to expand system variable, <c>false</c> to converts paths into system variables</param>
        /// <param name="is64Bit">Specifies if the application is 64 bit</param>
        /// <returns>a string of the path expanded</returns>
        public static string ConvertPath(string path, bool expand, bool is64Bit)
        {
            if (path != null)
            {
                if (path.StartsWith("HKEY", StringComparison.OrdinalIgnoreCase))
                {
                    char[] split = {'|'};
                    var key = path.Split(split)[0];
                    var value = path.Split(split)[1];
                    path = Registry.GetValue(key, value, null).ToString();
                }
                else
                {
                    if (expand == false)
                    {
                        var path2 = new StringBuilder(260);
                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_PROGRAMS, false);
                        path = Replace(path, path2.ToString(), "%ALLUSERSSTARTMENUPROGRAMS%");

                        path2 = new StringBuilder(260);
                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_STARTMENU, false);
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

                            if (is64Bit)
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
                        var path2 = new StringBuilder(260);

                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_PROGRAMS, false);

                        path = Replace(path, "%ALLUSERSSTARTMENUPROGRAMS%", path2.ToString());

                        path2 = new StringBuilder(260);

                        NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, path2, FileSystemLocations.CSIDL_COMMON_STARTMENU, false);

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
                            if (is64Bit)
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
        /// <param name="fileLoc">the full path to the file to get the hash from</param>
        /// <returns>a SHA1 value</returns>
        public static string GetHash(string fileLoc)
        {
            if (!File.Exists(fileLoc))
                return null;
            var stream = new FileStream(fileLoc, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);

            var sha1 = new SHA1CryptoServiceProvider();

            sha1.ComputeHash(stream);

            stream.Close();

            var buff = new StringBuilder();

            foreach (var hashByte in sha1.Hash)
                buff.Append(String.Format("{0:X1}", hashByte));
            return buff.ToString();
        }

        /// <summary>
        /// Replaces a string within a string
        /// </summary>
        /// <param name="complete">the string that will be searched</param>
        /// <param name="find">a string to find in the complete string</param>
        /// <param name="replace">a string to use to replace the find string in the complete string</param>
        /// <returns>a string that has the find value replace by the new value</returns>
        public static string Replace(string complete, string find, string replace)
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

        /// <summary>
        /// Reports the error that occurred to a log file
        /// </summary>
        /// <param name="message">The message to write in the log</param>
        /// <param name="directoryStore">The directory to store the log</param>
        public static void ReportError(string message, string directoryStore)
        {
            TextWriter tw = new StreamWriter(directoryStore + "error.log");

            tw.WriteLine(DateTime.Now + ": " + message);

            tw.Close();
        }

        /// <summary>
        /// Converts bytes into the proper increments depending on size
        /// </summary>
        /// <param name="bytes">the fileSize in bytes</param>
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

        #endregion

        #region DeSerialize Methods

        /// <summary>
        /// DeSerializes an object
        /// </summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="xmlFile">the file that contains the object to DeSerialize</param>
        /// <returns>returns the object</returns>
        public static T DeserializeStruct<T>(string xmlFile) where T : struct
        {
            if (File.Exists(xmlFile))
            {
                XmlSerializer s;
                TextReader r = null;
                try
                {
                    s = new XmlSerializer(typeof(T), "http://sevenupdate.sourceforge.net");
                    r = new StreamReader(xmlFile);
                    var temp = (T) s.Deserialize(r);
                    r.Close();
                    return temp;
                }
                catch (Exception e)
                {
                    if (SerializationErrorEventHandler != null)
                        SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e, xmlFile));
                }
                finally
                {
                    if (r != null)
                        r.Close();
                }
            }

            return new T();
        }

        /// <summary>
        /// DeSerializes an object
        /// </summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="xmlFile">the file that contains the object to DeSerialize</param>
        /// <returns>returns the object</returns>
        public static T Deserialize<T>(string xmlFile) where T : class
        {
            if (File.Exists(xmlFile))
            {
                XmlSerializer s;
                TextReader r = null;
                try
                {
                    s = new XmlSerializer(typeof (T), "http://sevenupdate.sourceforge.net");
                    r = new StreamReader(xmlFile);
                    var temp = (T) s.Deserialize(r);
                    r.Close();
                    return temp;
                }
                catch (Exception e)
                {
                    if (SerializationErrorEventHandler != null)
                        SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e, xmlFile));
                }
                finally
                {
                    if (r != null)
                        r.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// DeSerializes an object
        /// </summary>
        /// <typeparam name="T">the object to deserialize</typeparam>
        /// <param name="stream">The Stream to deserialize</param>
        /// <returns>returns the object</returns>
        public static T Deserialize<T>(Stream stream) where T : class
        {
            XmlSerializer s;
            TextReader r = null;
            try
            {
                s = new XmlSerializer(typeof(T), "http://sevenupdate.sourceforge.net");
                r = new StreamReader(stream);
                var temp = (T)s.Deserialize(r);
                r.Close();
                return temp;
            }
            catch (Exception e)
            {
                if (SerializationErrorEventHandler != null)
                    SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e, "SUI file"));
            }
            finally
            {
                if (r != null)
                    r.Close();
            }

            return null;
        }

        #endregion

        #region Serialize Methods

        /// <summary>
        /// Serializes an object into a file
        /// </summary>
        /// <typeparam name="T">the object</typeparam>
        /// <param name="item">the object to serialize</param>
        /// <param name="xmlFile">the location of a file that will be serialized</param>
        public static void Serialize<T>(T item, string xmlFile) where T : class
        {
            XmlSerializer s;
            XmlWriter w = null;
            try
            {
                var xs = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "http://sevenupdate.sourceforge.net");
                s = new XmlSerializer(typeof (T), "http://sevenupdate.sourceforge.net");
                w = XmlWriter.Create(xmlFile, xs);
                if (w != null)
                {
                    s.Serialize(w, item, ns);
                    w.Close();
                }
            }
            catch (Exception e)
            {
                if (SerializationErrorEventHandler != null)
                    SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e, xmlFile));
            }
            finally
            {
                if (w != null)
                    w.Close();
            }
        }

        /// <summary>
        /// Serializes an object into a file
        /// </summary>
        /// <typeparam name="T">the object</typeparam>
        /// <param name="item">the object to serialize</param>
        /// <param name="xmlFile">the location of a file that will be serialized</param>
        public static void SerializeStruct<T>(T item, string xmlFile) where T : struct
        {
            XmlSerializer s;
            XmlWriter w = null;
            try
            {
                var xs = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "http://sevenupdate.sourceforge.net");
                s = new XmlSerializer(typeof(T), "http://sevenupdate.sourceforge.net");
                w = XmlWriter.Create(xmlFile, xs);
                if (w != null)
                {
                    s.Serialize(w, item, ns);
                    w.Close();
                }
            }
            catch (Exception e)
            {
                if (SerializationErrorEventHandler != null)
                    SerializationErrorEventHandler(null, new SerializationErrorEventArgs(e, xmlFile));
            }
            finally
            {
                if (w != null)
                    w.Close();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs when an error occurs while serializing or deserializing a object/file
        /// </summary>
        public static event EventHandler<SerializationErrorEventArgs> SerializationErrorEventHandler;

        /// <summary>
        /// Provides event data for the SerializationError event
        /// </summary>
        public class SerializationErrorEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="e">The exception data</param>
            /// <param name="file">The full path of the file</param>
            public SerializationErrorEventArgs(Exception e, string file)
            {
                Exception = e;
                File = file;
            }

            /// <summary>
            /// Gets the exception data
            /// </summary>
            public Exception Exception { get; private set; }

            /// <summary>
            /// Gets the full path of the file
            /// </summary>
            public string File { get; private set; }
        }

        /// <summary>
        /// Provides event data for the ErrorOccurred event
        /// </summary>
        public class ErrorOccurredEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="exception">the exception that occurred</param>
            /// <param name="type">the type of error that occurred</param>
            public ErrorOccurredEventArgs(Exception exception, ErrorType type)
            {
                Exception = exception;
                Type = type;
            }

            /// <summary>
            /// Gets the Exception information of the error that occurred
            /// </summary>
            public Exception Exception { get; private set; }

            /// <summary>
            /// Gets the <see cref="ErrorType"/> of the error that occurred
            /// </summary>
            public ErrorType Type { get; private set; }
        }

        #endregion
    }
}