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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;

#endregion

namespace SevenUpdate.Base
{

    #region Event Args

    /// <summary>
    ///   Provides event data for the SearchCompleted event
    /// </summary>
    public class SearchCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///   Contains event data associated with this event
        /// </summary>
        /// <param name = "applications">The collection of applications to update</param>
        public SearchCompletedEventArgs(Collection<Sui> applications)
        {
            Applications = applications;
        }

        /// <summary>
        ///   Gets a collection of applications that contain updates to install
        /// </summary>
        public Collection<Sui> Applications { get; private set; }
    }

    #endregion

    /// <summary>
    ///   Contains methods to search for updates
    /// </summary>
    public static class Search
    {
        #region Global Vars

        /// <summary>
        ///   Location of the SUI for Seven Update
        /// </summary>
        private const string SevenUpdateSui = @"http://sevenupdate.com/apps/SevenUpdate.sui";

        #endregion

        #region Search Methods

        /// <summary>
        ///   Checks for updates
        /// </summary>
        /// <param name = "app">a collection of applications to check for updates</param>
        /// <param name = "hidden">a collection of hidden updates</param>
        /// <returns>returns
        ///   <c>true</c>
        ///   if found updates, otherwise
        ///   <c>false</c>
        /// </returns>
        private static bool CheckForUpdates(ref Sui app, IEnumerable<Suh> hidden)
        {
            app.Directory = Base.ConvertPath(app.Directory, true, app.Is64Bit);
            if (!Directory.Exists(app.Directory))
                return false;

            var isHidden = false;
            for (var y = 0; y < app.Updates.Count; y++)
            {
                if (hidden != null)
                {
                    foreach (Suh t in hidden)
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
                            break;
                        y--;
                        continue;
                    }
                }
                ulong size = 0;
                for (var z = 0; z < app.Updates[y].Files.Count; z++)
                {
                    app.Updates[y].Files[z].Destination = Base.ConvertPath(app.Updates[y].Files[z].Destination, app.Directory, app.Is64Bit);
                    // Checks to see if the file needs updated, if it doesn't it removes it from the list.
                    if (File.Exists(app.Updates[y].Files[z].Destination))
                    {
                        #region File Exists

                        switch (app.Updates[y].Files[z].Action)
                        {
                            case FileAction.Update:
                            case FileAction.UpdateThenExecute:
                            case FileAction.UpdateThenRegister:
                            case FileAction.UpdateIfExist:
                            case FileAction.CompareOnly:
                                if (Base.GetHash(app.Updates[y].Files[z].Destination) == app.Updates[y].Files[z].Hash)
                                {
                                    app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                    if (app.Updates[y].Files.Count == 0)
                                        break;
                                    z--;
                                }
                                else if (
                                    Base.GetHash(Base.AllUserStore + @"downloads\" + app.Updates[y].Name[0].Value + @"\" + Path.GetFileName(app.Updates[y].Files[z].Destination)) !=
                                    app.Updates[y].Files[z].Hash)
                                {
                                    if (app.Updates[y].Files[z].Action != FileAction.CompareOnly)
                                        size += app.Updates[y].Files[z].Size;
                                }
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
                            case FileAction.UnregisterThenDelete:
                                app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                if (app.Updates[y].Files.Count == 0)
                                    break;
                                z--;
                                break;

                            case FileAction.UpdateIfExist:
                                app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                if (app.Updates[y].Files.Count == 0)
                                    break;
                                z--;
                                break;
                            case FileAction.ExecuteThenDelete:
                                size += app.Updates[y].Files[z].Size;
                                break;
                            case FileAction.Update:
                            case FileAction.UpdateThenExecute:
                            case FileAction.UpdateThenRegister:
                                if (Base.GetHash(app.Updates[y].Files[z].Destination) == app.Updates[y].Files[z].Hash)
                                {
                                    app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                    if (app.Updates[y].Files.Count == 0)
                                        break;
                                    z--;
                                }
                                else if (
                                    Base.GetHash(Base.AllUserStore + @"downloads\" + app.Updates[y].Name[0].Value + @"\" + Path.GetFileName(app.Updates[y].Files[z].Destination)) !=
                                    app.Updates[y].Files[z].Hash)
                                    size += app.Updates[y].Files[z].Size;
                                break;
                        }

                        #endregion
                    }
                }

                var remove = true;

                // Checks to see if the update only contains execute and delete actions
                if (app.Updates[y].Files.Count > 0)
                {
                    foreach (UpdateFile t in app.Updates[y].Files)
                    {
                        // If the update has a file that isn't an execute and delete, let's indicate not to remove the update
                        if (t.Action != FileAction.ExecuteThenDelete)
                            remove = false;
                    }
                }

                // If the update does not have any files or if the update only contains execute and delete, then let's remove the update.
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
            if (app.Updates.Count > 0)
            {
                // Found updates, return
                return true;
            }
            app = null;
            // No updates, let's return
            return false;
        }

        /// <summary>
        ///   Searches for updates while blocking the calling thread
        /// </summary>
        /// <param name = "apps">the list of applications to check for updates</param>
        public static void SearchForUpdates(IEnumerable<Sua> apps)
        {
            var applications = new Collection<Sui>();
            try
            {
                // Check if machine is connected to the internet
                var clnt = new TcpClient("www.google.com", 80);
                clnt.Close();
            }
            catch (Exception e)
            {
                if (ErrorOccurredEventHandler != null)
                    ErrorOccurredEventHandler(null, new ErrorOccurredEventArgs(e, ErrorType.FatalNetworkError));
                return;
            }

            #region Seven Update

            try
            {
                // Download the Seven Update SUI and load it.
                var app = Base.Deserialize<Sui>(Base.DownloadFile(SevenUpdateSui), SevenUpdateSui);

                // Check if there is a newer version of Seven Update
                if (CheckForUpdates(ref app, null))
                {
                    // If there are updates add it to the collection
                    applications.Add(app);

                    // Search is complete!
                    if (SearchDoneEventHandler != null)
                        SearchDoneEventHandler(null, new SearchCompletedEventArgs(applications));

                    return;
                }
            }
            catch (Exception e)
            {
                // If this happens i am the only one to blame lol.
                Base.ReportError(e, Base.AllUserStore);
                // Notify that there was an error that occurred.
                if (ErrorOccurredEventHandler != null)
                    ErrorOccurredEventHandler(null, new ErrorOccurredEventArgs(e, ErrorType.SearchError));
            }

            #endregion

            #region Supported Apps

            if (apps == null)
            {
                // Search is complete!
                if (SearchDoneEventHandler != null)
                    SearchDoneEventHandler(null, new SearchCompletedEventArgs(applications));

                return;
            }

            // Gets the hidden updates from settings
            var hidden = Base.Deserialize<Collection<Suh>>(Base.HiddenFile);

            // If there are no updates for Seven Update, let's download and load the SUI's from the User config.
            foreach (Sua t in apps)
            {
                if (!t.IsEnabled)
                {
                }
                else
                {
                    try
                    {
                        // Loads a SUI that was downloaded
                        var app = Base.Deserialize<Sui>(Base.DownloadFile(t.Source), t.Source);

                        // Check to see if any updates are avalible and exclude hidden updates
                        // If there is an update avaliable, add it.
                        if (CheckForUpdates(ref app, hidden))
                            applications.Add(app);
                    }
                    catch (WebException e)
                    {
                        Base.ReportError("Error downloading file: " + t.Source, Base.AllUserStore);
                        // Notify that there was an error that occurred.
                        if (ErrorOccurredEventHandler != null)
                            ErrorOccurredEventHandler(null, new ErrorOccurredEventArgs(e, ErrorType.SearchError));
                    }
                    catch (Exception e)
                    {
                        Base.ReportError(e, Base.AllUserStore);
                        // Notify that there was an error that occurred.
                        if (ErrorOccurredEventHandler != null)
                            ErrorOccurredEventHandler(null, new ErrorOccurredEventArgs(e, ErrorType.SearchError));
                    }
                }
            }

            #endregion

            // Search is complete!
            if (SearchDoneEventHandler != null)
                SearchDoneEventHandler(null, new SearchCompletedEventArgs(applications));
        }

        /// <summary>
        ///   Searches for files without blocking the calling thread
        /// </summary>
        /// <param name = "apps">the list of Seven Update Admin.applications to check for updates</param>
        public static void SearchForUpdatesAync(IEnumerable<Sua> apps)
        {
            var worker = new BackgroundWorker();
            worker.DoWork -= WorkerDoWork;
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerAsync(apps);
        }

        /// <summary>
        ///   Searches for updates on a new thread
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "f"></param>
        private static void WorkerDoWork(object sender, DoWorkEventArgs f)
        {
            SearchForUpdates(((Collection<Sua>) f.Argument));
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs if an error occurred
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurredEventHandler;

        /// <summary>
        ///   Occurs when the searching of updates has completed.
        /// </summary>
        public static event EventHandler<SearchCompletedEventArgs> SearchDoneEventHandler;

        #endregion
    }
}