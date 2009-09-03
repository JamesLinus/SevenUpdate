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
using System.ComponentModel;
using System.IO;
using System.Net;

#endregion

namespace SevenUpdate
{
    public static class Search
    {
        #region Global Vars

        /// <summary>
        /// Location of the SUI for Seven Update
        /// </summary>
        private const string SevenUpdateSUI = @"http://ittakestime.org/su/Seven Update.sui";

        #endregion

        #region Search Methods

        /// <summary>
        /// Checks for updates
        /// </summary>
        /// <param name="app">A collection of applications to check for updates</param>
        /// <param name="hidden">A collection of hidden updates</param>
        /// <returns>Returns true if found updates, otherwise false</returns>
        private static bool CheckForUpdates(ref SUI app, Collection<SUH> hidden)
        {
            if (!Directory.Exists(Shared.ConvertPath(app.Directory, true, app.Is64Bit)))
                return false;
            var isHidden = false;
            for (var y = 0; y < app.Updates.Count; y++)
            {
                if (hidden != null)
                {
                    for (var z = 0; z < hidden.Count; z++)
                    {
                        if (hidden[z].ReleaseDate == app.Updates[y].ReleaseDate && hidden[z].Name[0].Value == app.Updates[y].Name[0].Value)
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
                            break;
                        y--;
                        continue;
                    }
                }
                ulong size = 0;
                for (var z = 0; z < app.Updates[y].Files.Count; z++)
                {
                    var file = Shared.ConvertPath(app.Updates[y].Files[z].Destination, app.Directory, app.Is64Bit);

                    /// Checks to see if the file needs updated, if it doesn't it removes it from the list.
                    if (File.Exists(file))
                    {
                        #region File Exists

                        switch (app.Updates[y].Files[z].Action)
                        {
                            case FileAction.Update:
                            case FileAction.UpdateAndExecute:
                            case FileAction.UpdateAndRegister:
                                if (Shared.GetHash(file) == app.Updates[y].Files[z].Hash)
                                {
                                    app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                    if (app.Updates[y].Files.Count == 0)
                                        break;
                                    z--;
                                }
                                else if (Shared.GetHash(Shared.AllUserStore + @"downloads\" + app.Updates[y].Name[0].Value + @"\" + Path.GetFileName(file)) != app.Updates[y].Files[z].Hash)
                                    size += app.Updates[y].Files[z].Size;
                                break;
                        }
                    }
                        #endregion

                    else
                    {
                        #region File does not exist

                        switch (app.Updates[y].Files[z].Action)
                        {
                            case FileAction.Delete:
                            case FileAction.UnregisterAndDelete:
                                app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                if (app.Updates[y].Files.Count == 0)
                                    break;
                                z--;
                                break;

                            case FileAction.Update:
                            case FileAction.UpdateAndExecute:
                            case FileAction.UpdateAndRegister:
                                if (Shared.GetHash(file) == app.Updates[y].Files[z].Hash)
                                {
                                    app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                    if (app.Updates[y].Files.Count == 0)
                                        break;
                                    z--;
                                }
                                else if (Shared.GetHash(Shared.AllUserStore + @"downloads\" + app.Updates[y].Name[0].Value + @"\" + Path.GetFileName(file)) != app.Updates[y].Files[z].Hash)
                                    size += app.Updates[y].Files[z].Size;
                                break;
                        }

                        #endregion
                    }
                }

                var remove = true;

                /// Checks to see if the update only contains execute and delete actions
                /// 
                if (app.Updates[y].Files.Count > 0)
                {
                    for (var z = 0; z < app.Updates[y].Files.Count; z++)
                    {
                        /// If the update has a file that isn't an execute and delete, let's indicate not to remove the update
                        if (app.Updates[y].Files[z].Action != FileAction.ExecuteAndDelete)
                            remove = false;
                    }
                }

                /// If the update does not have any files or if the update only contains execute and delete, then let's remove the update.
                if (app.Updates[y].Files.Count == 0 || remove)
                {
                    app.Updates.Remove(app.Updates[y]);
                    if (app.Updates.Count == 0)
                        break;
                    y--;
                    continue;
                }
                app.Updates[y].Size = size;
            }
            if (app.Updates.Count != 0)
            {
                /// Found updates, return
                return true;
            }
            app = null;
            /// No updates, let's return
            return false;
        }

        /// <summary>
        /// Seaches for updates while blocking the calling thread
        /// </summary>
        /// <param name="apps">The list of applications to check for updates</param>
        public static void SearchForUpdates(Collection<SUA> apps)
        {
            // delete the temp directory housing the sui files
            if (Directory.Exists(Shared.UserStore + "temp"))
                Directory.Delete(Shared.UserStore + "temp", true);

            // create the temp directory for housing the sui files
            Directory.CreateDirectory(Shared.UserStore + "temp");

            var web = new WebClient();

            var applications = new Collection<SUI>();

            try
            {
                /// Downloads the Seven Update SUI
                web.DownloadFile(SevenUpdateSUI, Shared.UserStore + @"temp\Seven Update.sui");
            }
            catch (WebException e)
            {
                /// Server Error! If that happens then i am the only one to blame LOL
                OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs(e.Message, ErrorType.SearchError));
                return;
            }

            if (Directory.GetFiles(Shared.UserStore + "temp").Length == 0)
            {
                // Call the Event with a Network Connection Error if no SUI's can be download, assuming a network error is the cause.
                OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs("Network Connection Error", ErrorType.FatalNetworkError));
                return;
            }

            /// Load the Seven Update SUI
            var app = Shared.Deserialize<SUI>(Shared.UserStore + @"temp\Seven Update.sui");

            /// Checks to see if there are any updates for Seven Update
            if (app != null)
            {
                if (CheckForUpdates(ref app, null))
                {
                    /// If there are updates add it to the collection
                    applications.Add(app);
                }
                else
                {
                    /// If there are no updates for Seven Update, let's download and load the SUI's from the User config.
                    for (var x = 0; x < apps.Count; x++)
                    {
                        try
                        {
                            /// Download the SUI
                            web.DownloadFile(apps[x].Source, Shared.UserStore + @"temp\" + apps[x].Name[0].Value + ".sui");
                        }
                        catch (WebException e)
                        {
                            /// Notify that there was an error that occurred.
                            OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs(e.Message, ErrorType.SearchError));
                        }
                    }

                    if (Directory.GetFiles(Shared.UserStore + "temp").Length == 0)
                    {
                        // Call the Event with a special code, meaning it was a Network Connection Error
                        OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs("Network Connection Error", ErrorType.FatalNetworkError));
                        return;
                    }
                    /// Delete the SUI, it's been loaded, no longer needed on filesystem
                    File.Delete(Shared.UserStore + @"temp\Seven Update.sui");

                    web.Dispose();

                    /// Get the rest of the SUI's in the directory and load them
                    var dir = new DirectoryInfo(Shared.UserStore + @"temp").GetFiles("*.sui", SearchOption.TopDirectoryOnly);

                    /// Gets the hidden updates from settings
                    var hidden = Shared.Deserialize<Collection<SUH>>(Shared.AllUserStore + "Hidden Updates.xml");

                    for (var x = 0; x < dir.Length; x++)
                    {
                        /// Loads a SUI that was downloaded
                        app = Shared.Deserialize<SUI>(dir[x].FullName);

                        /// Check to see if any updates are avalible and exclude hidden updates
                        if (CheckForUpdates(ref app, hidden)) /// If there is an update avaliable, add it.
                            applications.Add(app);
                    }
                }
            }
            /// Delete the temp directory, we are done with it.
            Directory.Delete(Shared.UserStore + "temp", true);

            /// Search is complete!
            OnEvent(SearchDoneEventHandler, new SearchDoneEventArgs(applications));
        }

        /// <summary>
        /// Searches for files without blocking the calling thread
        /// </summary>
        /// <param name="apps">The list of Seven Update Admin.applications to check for updates</param>
        public static void SearchForUpdatesAync(Collection<SUA> apps)
        {
            var worker = new BackgroundWorker();
            worker.DoWork -= WorkerDoWork;
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerAsync(apps);
        }

        /// <summary>
        /// Searches for updates on a new thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="f"></param>
        private static void WorkerDoWork(object sender, DoWorkEventArgs f)
        {
            SearchForUpdates(((Collection<SUA>) f.Argument));
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs if an error occurred
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurredEventHandler;

        /// <summary>
        /// Occurs when the seaching of updates has completed.
        /// </summary>
        public static event EventHandler<SearchDoneEventArgs> SearchDoneEventHandler;

        private static void OnEvent<T>(EventHandler<T> @event, T args) where T : EventArgs
        {
            if (@event == null)
                return;
            foreach (EventHandler<T> singleEvent in @event.GetInvocationList())
            {
                if (singleEvent.Target != null && singleEvent.Target is ISynchronizeInvoke)
                {
                    var target = (ISynchronizeInvoke) singleEvent.Target;
                    if (target.InvokeRequired)
                    {
                        target.BeginInvoke(singleEvent, new object[] {"OnEvent", args});
                        continue;
                    }
                }
                singleEvent("OnEvent", args);
            }
        }

        #region Nested type: ErrorOccurredEventArgs

        public class ErrorOccurredEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="description">The description of the error</param>
            /// <param name="type">The type of error that occurred</param>
            public ErrorOccurredEventArgs(string description, ErrorType type)
            {
                Description = description;
                Type = type;
            }

            /// <summary>
            /// A string describing the error
            /// </summary>
            public string Description { get; set; }

            public ErrorType Type { get; set; }
        }

        #endregion

        #region Nested type: SearchDoneEventArgs

        public class SearchDoneEventArgs : EventArgs
        {
            /// <summary>
            /// Contains event data associated with this event
            /// </summary>
            /// <param name="applications">The collection of applications to update</param>
            public SearchDoneEventArgs(Collection<SUI> applications)
            {
                Applications = applications;
            }

            /// <summary>
            /// A collection of applications that contain updates to install
            /// </summary>
            public Collection<SUI> Applications { get; set; }
        }

        #endregion

        #endregion
    }
}