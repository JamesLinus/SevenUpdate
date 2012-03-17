// ***********************************************************************
// <copyright file="Search.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using ProtoBuf;

    /// <summary>Contains methods to search for updates.</summary>
    public static class Search
    {
        /// <summary>The directory containing the app update files.</summary>
        private static string downloadDirectory;

        /// <summary>The number of important updates found.</summary>
        private static int importantCount;

        /// <summary>The number of optional updates found.</summary>
        private static int optionalCount;

        /// <summary>The number of recommended updates found.</summary>
        private static int recommendedCount;

        /// <summary>Occurs if an error occurred.</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>Occurs when the searching of updates has completed.</summary>
        public static event EventHandler<SearchCompletedEventArgs> SearchCompleted;

        /// <summary>Gets a value indicating whether Seven update is currently searching for updates.</summary>
        public static bool IsSearching { get; private set; }

        /// <summary>Searches for updates while blocking the calling thread.</summary>
        /// <param name="applications">The collection of applications to check for updates.</param>
        /// <param name="downloadFolder">The directory where update might be downloaded to.</param>
        public static void SearchForUpdates(IEnumerable<Sua> applications, string downloadFolder)
        {
            downloadDirectory = downloadFolder;
            IsSearching = true;
            importantCount = 0;
            optionalCount = 0;
            recommendedCount = 0;
            var applicationsFound = new Collection<Sui>();
            try
            {
                // Check if machine is connected to the internet
                var client = new TcpClient(@"sevenupdate.com", 80);
                client.Close();
            }
            catch (Exception ex)
            {
                if (!(ex is SocketException || ex is WebException))
                {
                    throw;
                }

                if (ErrorOccurred != null)
                {
                    ErrorOccurred(null, new ErrorOccurredEventArgs(ex.Message, ErrorType.FatalNetworkError));
                }

                return;
            }

            if (applications == null)
            {
                throw new ArgumentNullException("applications");
            }

            // let's download and load the SUI's from the User config.
            foreach (var t in applications)
            {
                if (!t.IsEnabled)
                {
                }
                else
                {
                    // Loads a SUI that was downloaded
                    Sui app = null;
                    try
                    {
                        app = Utilities.Deserialize<Sui>(Utilities.DownloadFile(t.SuiUrl));
                    }
                    catch (WebException ex)
                    {
                        Utilities.ReportError(ex, ErrorType.SearchError, t.SuiUrl);
                    }
                    catch (Exception ex)
                    {
                        Utilities.ReportError(ex, ErrorType.SearchError, t.SuiUrl);
                        throw;
                    }

                    if (app != null)
                    {
                        app.AppInfo = t;

                        try
                        {
                            // Check to see if any updates are available
                            if (CheckForUpdates(ref app))
                            {
                                applicationsFound.Add(app);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.ReportError(ex, ErrorType.SearchError);

                            if (
                                    !(ex is FileNotFoundException || ex is FileFormatException || ex is ProtoException
                                      || ex is NullReferenceException))
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            // Search is complete!
            IsSearching = false;

            if (SearchCompleted != null)
            {
                SearchCompleted(null, 
                        new SearchCompletedEventArgs(applicationsFound, importantCount, recommendedCount, optionalCount));
            }
        }

        /// <summary>Searches for files without blocking the calling thread.</summary>
        /// <param name="applications">The collection of applications to check for updates.</param>
        /// <param name="downloadFolder">The directory where update might be downloaded to.</param>
        public static void SearchForUpdatesAsync(IEnumerable<Sua> applications, string downloadFolder)
        {
            Task.Factory.StartNew(() => SearchForUpdates(applications, downloadFolder));
        }

        /// <summary>Manually sets an <c>Sui</c> collection has updates found.</summary>
        /// <param name="updates">The updates to set as found.</param>
        public static void SetUpdatesFound(IEnumerable<Sui> updates)
        {
            if (updates == null)
            {
                throw new ArgumentNullException("updates");
            }

            importantCount = 0;
            recommendedCount = 0;
            optionalCount = 0;
            List<Sui> updateList = updates.ToList();
            foreach (var update in updateList.SelectMany(app => app.Updates))
            {
                switch (update.Importance)
                {
                    case Importance.Important:
                        importantCount++;
                        break;
                    case Importance.Recommended:
                        recommendedCount++;
                        break;
                    case Importance.Optional:
                    case Importance.Locale:
                        optionalCount++;
                        break;
                }
            }

            if (SearchCompleted != null)
            {
                SearchCompleted(null, 
                        new SearchCompletedEventArgs(updateList, importantCount, recommendedCount, optionalCount));
            }
        }

        /// <summary>Checks for updates.</summary>
        /// <param name="app">A collection of applications to check for updates.</param>
        /// <returns>Returns <c>True</c> if found updates, otherwise <c>False</c>.</returns>
        private static bool CheckForUpdates(ref Sui app)
        {
            app.AppInfo.Directory = Utilities.IsRegistryKey(app.AppInfo.Directory)
                                            ? Utilities.GetRegistryValue(app.AppInfo.Directory, 
                                                    app.AppInfo.ValueName, 
                                                    app.AppInfo.Platform)
                                            : Utilities.ConvertPath(app.AppInfo.Directory, true, app.AppInfo.Platform);

            if (!Directory.Exists(app.AppInfo.Directory))
            {
                return false;
            }

            for (int y = 0; y < app.Updates.Count; y++)
            {
                Update updates = app.Updates[y];

                ulong size = IterateUpdate(ref updates, 
                        app.AppInfo.Directory, 
                        app.AppInfo.ValueName, 
                        app.AppInfo.Platform);

                app.Updates[y] = updates;

                bool remove = true;

                // Checks to see if the update only contains execute and delete actions
                if (app.Updates[y].Files.Count > 0)
                {
                    // ReSharper disable ForCanBeConvertedToForeach
                    for (int z = 0; z < app.Updates[y].Files.Count; z++)
                    {
                        app.Updates[y].Files[z].Destination =
                                Utilities.ExpandInstallLocation(app.Updates[y].Files[z].Destination, 
                                        app.AppInfo.Directory, 
                                        app.AppInfo.Platform, 
                                        app.AppInfo.ValueName);

                        app.Updates[y].Files[z].Source = Utilities.ExpandDownloadUrl(app.Updates[y].Files[z].Source, 
                                app.Updates[y].DownloadUrl, 
                                app.AppInfo.Platform);

                        if (app.Updates[y].Files[z].Action != FileAction.ExecuteThenDelete)
                        {
                            remove = false;
                        }
                    }

                    // ReSharper restore ForCanBeConvertedToForeach
                }

                // If the update does not have any files or if the update only contains execute and delete, then let's
                // remove the update.
                if (app.Updates[y].Files.Count == 0 || remove)
                {
                    app.Updates.Remove(app.Updates[y]);
                    if (app.Updates.Count == 0)
                    {
                        break;
                    }

                    y--;
                    continue;
                }

                app.Updates[y].Size = size;
                switch (app.Updates[y].Importance)
                {
                    case Importance.Important:
                        importantCount++;
                        break;
                    case Importance.Recommended:
                        recommendedCount++;
                        break;
                    case Importance.Optional:
                    case Importance.Locale:
                        optionalCount++;
                        break;
                }
            }

            if (app.Updates.Count > 0)
            {
                // Found updates, return
                return true;
            }

            app = null;

            // No updates, let's return
            return false;
        }

        /// <summary>Iterates through the update and removes un needed values. Returns the download size for the update.</summary>
        /// <param name="update">The update to iterate.</param>
        /// <param name="directory">The Uri or registry key to the application directory .</param>
        /// <param name="valueName">The name of the registry value, can be <c>null</c>.</param>
        /// <param name="platform">A value that indicates what cpu architecture the application supports.</param>
        /// <returns>The current download size of the update.</returns>
        private static ulong IterateUpdate(ref Update update, string directory, string valueName, Platform platform)
        {
            ulong size = 0;
            for (int z = 0; z < update.Files.Count; z++)
            {
                update.Files[z].Destination = Utilities.ExpandInstallLocation(update.Files[z].Destination, 
                        directory, 
                        platform, 
                        valueName);

                string downloadFile = Path.Combine(downloadDirectory, 
                        update.Name[0].Value, 
                        Path.GetFileName(update.Files[z].Destination));

                // Checks to see if the file needs updated, if it doesn't it removes it from the list.
                if (File.Exists(update.Files[z].Destination))
                {
                    switch (update.Files[z].Action)
                    {
                        case FileAction.Update:
                        case FileAction.UpdateThenExecute:
                        case FileAction.UpdateThenRegister:
                        case FileAction.UpdateIfExist:
                        case FileAction.CompareOnly:
                            if (Utilities.GetHash(update.Files[z].Destination) == update.Files[z].Hash)
                            {
                                update.Files.Remove(update.Files[z]);
                                if (update.Files.Count == 0)
                                {
                                    break;
                                }

                                z--;
                            }
                            else if (Utilities.GetHash(downloadFile) != update.Files[z].Hash)
                            {
                                if (update.Files[z].Action != FileAction.CompareOnly)
                                {
                                    size += update.Files[z].FileSize;
                                }
                            }

                            break;
                    }
                }
                else
                {
                    switch (update.Files[z].Action)
                    {
                        case FileAction.Delete:
                        case FileAction.UnregisterThenDelete:
                            update.Files.Remove(update.Files[z]);
                            if (update.Files.Count == 0)
                            {
                                break;
                            }

                            z--;
                            break;

                        case FileAction.UpdateIfExist:
                            update.Files.Remove(update.Files[z]);
                            if (update.Files.Count == 0)
                            {
                                break;
                            }

                            z--;
                            break;
                        case FileAction.ExecuteThenDelete:
                            size += update.Files[z].FileSize;
                            break;
                        case FileAction.Update:
                        case FileAction.UpdateThenExecute:
                        case FileAction.UpdateThenRegister:
                            if (Utilities.GetHash(update.Files[z].Destination) == update.Files[z].Hash)
                            {
                                update.Files.Remove(update.Files[z]);
                                if (update.Files.Count == 0)
                                {
                                    break;
                                }

                                z--;
                            }
                            else if (Utilities.GetHash(downloadFile) != update.Files[z].Hash)
                            {
                                size += update.Files[z].FileSize;
                            }

                            break;
                    }
                }
            }

            return size;
        }
    }
}