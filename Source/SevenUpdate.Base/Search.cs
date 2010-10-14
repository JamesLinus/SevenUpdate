// ***********************************************************************
// <copyright file="Search.cs"
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>Contains methods to search for updates</summary>
    public static class Search
    {
        #region Constants and Fields

        /// <summary>Location of the SUI for Seven Update</summary>
        private const string SevenUpdateSui = @"http://sevenupdate.com/apps/SevenUpdate-v2.sui";

        /// <summary>The number of important updates found</summary>
        private static int importantCount;

        /// <summary>The number of optional updates found</summary>
        private static int optionalCount;

        /// <summary>The number of recommended updates found</summary>
        private static int recommendedCount;

        #endregion

        #region Events

        /// <summary>Occurs if an error occurred</summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>Occurs when the searching of updates has completed.</summary>
        public static event EventHandler<SearchCompletedEventArgs> SearchCompleted;

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether Seven update is currently searching for updates</summary>
        public static bool IsSearching
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>Searches for updates while blocking the calling thread</summary>
        /// <param name="applications">The collection of applications to check for updates</param>
        public static void SearchForUpdates(IEnumerable<Sua> applications)
        {
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
            catch (WebException e)
            {
                if (ErrorOccurred != null)
                {
                    ErrorOccurred(null, new ErrorOccurredEventArgs(e.Message, ErrorType.FatalNetworkError));
                }

                return;
            }

            var publisher = new ObservableCollection<LocaleString>();
            var ls = new LocaleString
                {
                    Value = "Seven Software",
                    Lang = "en"
                };
            publisher.Add(ls);

            var name = new ObservableCollection<LocaleString>();
            ls = new LocaleString
                {
                    Value = "Seven Update",
                    Lang = "en"
                };
            name.Add(ls);

            // Download the Seven Update SUI and load it.
            var app = Utilities.Deserialize<Sui>(Utilities.DownloadFile(SevenUpdateSui), SevenUpdateSui);

            if (app != null)
            {
                app.AppInfo = new Sua
                    {
                        AppUrl = @"http://sevenupdate.com/",
                        Directory = Utilities.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update", true, true),
                        Publisher = publisher,
                        Name = name,
                        HelpUrl = @"http://sevenupdate.com/support/",
                        Is64Bit = true,
                        IsEnabled = true,
                        SuiUrl = SevenUpdateSui
                    };

                // Check if there is a newer version of Seven Update
                if (CheckForUpdates(ref app, null))
                {
                    // If there are updates add it to the collection
                    applicationsFound.Add(app);

                    // Search is complete!
                    IsSearching = false;

                    if (SearchCompleted != null)
                    {
                        SearchCompleted(null, new SearchCompletedEventArgs(applicationsFound, importantCount, recommendedCount, optionalCount));
                    }

                    return;
                }
            }

            if (applications == null)
            {
                // Search is complete!
                IsSearching = false;

                if (SearchCompleted != null)
                {
                    SearchCompleted(null, new SearchCompletedEventArgs(applicationsFound, importantCount, recommendedCount, optionalCount));
                }

                return;
            }

            // Gets the hidden updates from settings
            var hidden = Utilities.Deserialize<Collection<Suh>>(Utilities.HiddenFile);

            // If there are no updates for Seven Update, let's download and load the SUI's from the User config.
            foreach (var t in applications)
            {
                if (!t.IsEnabled)
                {
                }
                else
                {
                    try
                    {
                        // Loads a SUI that was downloaded
                        app = Utilities.Deserialize<Sui>(Utilities.DownloadFile(t.SuiUrl), t.SuiUrl);
                    }
                    catch (WebException)
                    {
                        Utilities.ReportError(@"Error downloading file: " + t.SuiUrl, Utilities.AllUserStore);
                        continue;
                    }

                    if (app != null)
                    {
                        app.AppInfo = t;

                        // Check to see if any updates are available and exclude hidden updates

                        // If there is an update available, add it.
                        if (CheckForUpdates(ref app, hidden))
                        {
                            applicationsFound.Add(app);
                        }
                    }
                    else
                    {
                        Utilities.ReportError(@"Error downloading file: " + t.SuiUrl, Utilities.AllUserStore);
                    }
                }
            }

            // Search is complete!
            IsSearching = false;

            if (SearchCompleted != null)
            {
                SearchCompleted(null, new SearchCompletedEventArgs(applicationsFound, importantCount, recommendedCount, optionalCount));
            }
        }

        /// <summary>Searches for files without blocking the calling thread</summary>
        /// <param name="applications">The collection of applications to check for updates</param>
        public static void SearchForUpdatesAsync(IEnumerable<Sua> applications)
        {
            Task.Factory.StartNew(() => SearchForUpdates(applications));
        }

        /// <summary>Manually sets an <see cref="Sui"/> collection has updates found</summary>
        /// <param name="updates">The updates to set as found</param>
        public static void SetUpdatesFound(IEnumerable<Sui> updates)
        {
            importantCount = 0;
            recommendedCount = 0;
            optionalCount = 0;
            foreach (var update in updates.SelectMany(app => app.Updates))
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
                SearchCompleted(null, new SearchCompletedEventArgs(updates, importantCount, recommendedCount, optionalCount));
            }
        }

        #endregion

        #region Methods

        /// <summary>Checks for updates</summary>
        /// <param name="app">a collection of applications to check for updates</param>
        /// <param name="hidden">a collection of hidden updates</param>
        /// <returns>returns <see langword="true"/> if found updates, otherwise <see langword="false"/></returns>
        private static bool CheckForUpdates(ref Sui app, IEnumerable<Suh> hidden)
        {
            app.AppInfo.Directory = Utilities.IsRegistryKey(app.AppInfo.Directory)
                                        ? Utilities.GetRegistryValue(app.AppInfo.Directory, app.AppInfo.ValueName, app.AppInfo.Is64Bit)
                                        : Utilities.ConvertPath(app.AppInfo.Directory, true, app.AppInfo.Is64Bit);

            if (!Directory.Exists(app.AppInfo.Directory))
            {
                return false;
            }

            var isHidden = false;
            for (var y = 0; y < app.Updates.Count; y++)
            {
                if (hidden != null)
                {
                    foreach (var t in hidden)
                    {
                        if (t.ReleaseDate == app.Updates[y].ReleaseDate && t.Name[0].Value == app.Updates[y].Name[0].Value)
                        {
                            isHidden = true;
                            break;
                        }

                        isHidden = false;
                    }

                    if (isHidden)
                    {
                        app.Updates.Remove(app.Updates[y]);

                        if (app.Updates.Count == 0)
                        {
                            break;
                        }

                        y--;
                        continue;
                    }
                }

                var updates = app.Updates[y];

                var size = IterateUpdate(ref updates, app.AppInfo.Directory, app.AppInfo.ValueName, app.AppInfo.Is64Bit);

                app.Updates[y] = updates;

                var remove = true;

                // Checks to see if the update only contains execute and delete actions
                if (app.Updates[y].Files.Count > 0)
                {
                    // ReSharper disable ForCanBeConvertedToForeach
                    for (var z = 0; z < app.Updates[y].Files.Count; z++)
                    {
                        app.Updates[y].Files[z].Destination = Utilities.ConvertPath(app.Updates[y].Files[z].Destination, app.AppInfo.Directory, app.AppInfo.Is64Bit, app.AppInfo.ValueName);

                        app.Updates[y].Files[z].Source = Utilities.ConvertPath(app.Updates[y].Files[z].Source, app.Updates[y].DownloadUrl, app.AppInfo.Is64Bit);

                        if (app.Updates[y].Files[z].Action != FileAction.ExecuteThenDelete)
                        {
                            remove = false;
                        }
                    }

                    // ReSharper restore ForCanBeConvertedToForeach
                }

                // If the update does not have any files or if the update only contains execute and delete, then let's remove the update.
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

        /// <summary>Iterates through the update and removes un needed values. Returns the download size for the update</summary>
        /// <param name="update">The update to iterate</param>
        /// <param name="directory">The Uri or registry key to the application directory </param>
        /// <param name="valueName">The name of the registry value, can be <see langword="null"/></param>
        /// <param name="is64Bit">if set to <see langword="true"/> the application is 64 bit</param>
        /// <returns>The current download size of the update</returns>
        private static ulong IterateUpdate(ref Update update, string directory, string valueName, bool is64Bit)
        {
            ulong size = 0;
            for (var z = 0; z < update.Files.Count; z++)
            {
                update.Files[z].Destination = Utilities.ConvertPath(update.Files[z].Destination, directory, is64Bit, valueName);
                var downloadFile = Utilities.AllUserStore + @"downloads\" + update.Name[0].Value + @"\" + Path.GetFileName(update.Files[z].Destination);

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

        #endregion
    }
}