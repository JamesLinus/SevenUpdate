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
using System.Net;
namespace SevenUpdate
{
    public static class Search
    {
        #region Global Vars

        /// <summary>
        /// List of Applications
        /// </summary>
        static ObservableCollection<Application> applications { get; set; }

        /// <summary>
        /// Indicates if an erorr occurred
        /// </summary>
        static bool ErrorOccurred { get; set; }

        /// <summary>
        /// If true, Seven Update is currently installing updates.
        /// </summary>
        public static bool InstallInProgress { get; set; }
        
        /// <summary>
        /// Location of the SUI for Seven Update
        /// </summary>
        const string SUSUI = @"http://ittakestime.org/su/Seven Update.sui";



        #endregion

        #region Search Methods

        /// <summary>
        /// Checks for updates
        /// </summary>
        /// <param name="suiFile">The SUI file location</param>
        /// <param name="hidden">List of hidden updates</param>
        /// <returns>Returns true if found updates</returns>
        static bool CheckForUpdates(string suiFile, ObservableCollection<UpdateInformation> hidden)
        {
            Application app = Shared.Deserialize<Application>(suiFile);

            if (!Directory.Exists(Shared.ConvertPath(app.Directory, true, app.Is64Bit)))
                return false;
            bool isHidden = false;
            for (int y = 0; y < app.Updates.Count; y++)
            {
                if (hidden != null)
                {
                    for (int z = 0; z < hidden.Count; z++)
                    {
                        if (hidden[z].ReleaseDate == app.Updates[y].ReleaseDate && hidden[z].ApplicationName[0].Value == app.Name[0].Value && hidden[z].UpdateTitle[0].Value == app.Updates[y].Title[0].Value)
                        {
                            isHidden = true;
                            break;
                        }
                        else
                            isHidden = false;
                    }
                    if (isHidden)
                    {
                        app.Updates.Remove(app.Updates[y]);

                        if (app.Updates.Count == 0)
                            break;
                        else
                            y--;
                        continue;
                    }
                }
                string file;
                for (int z = 0; z < app.Updates[y].Files.Count; z++)
                {
                    file = Shared.ConvertPath(app.Updates[y].Files[z].Destination, app.Directory, app.Is64Bit);

                    if (File.Exists(file))
                    {
                        switch (app.Updates[y].Files[z].Action)
                        {
                            case FileAction.Delete:
                            case FileAction.ExecuteAndDelete: 
                            case FileAction.UnregisterAndDelete: break;

                            case FileAction.Update:
                            case FileAction.UpdateAndExecute:
                            case FileAction.UpdateAndRegister:
                                if (Shared.GetHash(file) == app.Updates[y].Files[z].Hash)
                                {
                                    app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                    if (app.Updates[y].Files.Count == 0)
                                        break;
                                    else
                                        z--;
                                }
                                break;

                        }
                    }
                    else
                    {
                        switch (app.Updates[y].Files[z].Action)
                        {
                            case FileAction.Delete: 
                            case FileAction.UnregisterAndDelete: 
                            app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                            if (app.Updates[y].Files.Count == 0)
                                break;
                            else
                                z--;
                                break;

                            case FileAction.ExecuteAndDelete: break;

                            case FileAction.Update:
                            case FileAction.UpdateAndExecute:
                            case FileAction.UpdateAndRegister: 
                            if (Shared.GetHash(file) == app.Updates[y].Files[z].Hash)
                            {
                                app.Updates[y].Files.Remove(app.Updates[y].Files[z]);
                                if (app.Updates[y].Files.Count == 0)
                                    break;
                                else
                                    z--;
                            }
                            break;

                        }
                    }
                }
                bool remove = true;
                if (app.Updates[y].Files.Count > 0)
                {
                    for (int z = 0; z < app.Updates[y].Files.Count; z++)
                    {
                        if (app.Updates[y].Files[z].Action != FileAction.ExecuteAndDelete)
                        {
                            remove = false;
                        }
                    }

                }
                if (app.Updates[y].Files.Count == 0 || remove)
                {
                    app.Updates.Remove(app.Updates[y]);
                    if (app.Updates.Count == 0)
                        break;
                    else
                        y--;
                    continue;
                }


            }
            if (app.Updates.Count == 0)
            {
                return false;
            }
            else
            {
                applications.Add(app);
                return true;
            }
        }

        /// <summary>
        /// Seaches for updates while blocking the calling thread
        /// </summary>
        /// <param name="apps">The list of applications to check for updates</param>
        public static void SearchForUpdates(ObservableCollection<SUA> apps)
        {
            // delete the temp directory housing the sui files
            if (Directory.Exists(Shared.userStore + "temp"))
                Directory.Delete(Shared.userStore + "temp", true);

            // create the temp directory for housing the sui files
            Directory.CreateDirectory(Shared.userStore + "temp");

            var web = new WebClient();

            applications = new ObservableCollection<Application>();

            try
            {
                web.DownloadFile(SUSUI, Shared.userStore + @"temp\Seven Update.sui");
            }
            catch (WebException)
            {
                OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs("Seven Update Server Error"));
                return;
            }

            if (Directory.GetFiles(Shared.userStore + "temp").Length == 0)
            {
                // Call the Event with a Network Connection Error
                OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs("Network Connection Error"));
                return;
            }

            if (!CheckForUpdates(Shared.userStore + @"temp\Seven Update.sui", null))
            {
                if (apps != null)
                {
                    foreach (SUA sua in apps)
                    {
                        try
                        {
                            web.DownloadFile(sua.Source, Shared.userStore + @"temp\" + sua.ApplicationName[0].Value + ".sui");
                        }
                        catch (WebException e)
                        {
                            OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs(e.Message));
                        }
                    }
                }
                if (Directory.GetFiles(Shared.userStore + "temp").Length == 0)
                {
                    // Call the Event with a special code, meaning it was a Network Connection Error
                    OnEvent(ErrorOccurredEventHandler, new ErrorOccurredEventArgs("Network Connection Error"));
                    return;
                }
                else
                {
                    File.Delete(Shared.userStore + @"temp\Seven Update.sui");
                    web.Dispose();
                    web = null;
                    FileInfo[] dir = new DirectoryInfo(Shared.userStore + @"temp").GetFiles("*.sui", SearchOption.TopDirectoryOnly);
                    ObservableCollection<UpdateInformation> hidden = Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Hidden Updates.xml");
                    for (int x = 0; x < dir.Length; x++)
                    {
                        CheckForUpdates(dir[x].FullName, hidden);
                    }
                }
            }
            Directory.Delete(Shared.userStore + "temp", true);
            OnEvent(SearchDoneEventHandler, new SearchDoneEventArgs(applications));
        }

        /// <summary>
        /// Searches for files without blocking the calling thread
        /// </summary>
        /// <param name="apps">The list of Seven Update Admin.applications to check for updates</param>
        public static void SearchForUpdatesAync(ObservableCollection<SUA> apps)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork -= new DoWorkEventHandler(worker_DoWork);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync(apps);
        }

        /// <summary>
        /// Searches for updates on a new thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="f"></param>
        static void worker_DoWork(object sender, DoWorkEventArgs f)
        {
            SearchForUpdates(((ObservableCollection<SUA>)f.Argument));
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises an event if an error occurred
        /// </summary>
        public static event EventHandler<ErrorOccurredEventArgs> ErrorOccurredEventHandler;

        /// <summary>
        /// Raises an event when the seaching of updates has completed.
        /// </summary>
        public static event EventHandler<SearchDoneEventArgs> SearchDoneEventHandler;

        public class SearchDoneEventArgs : EventArgs
        {
            public SearchDoneEventArgs(ObservableCollection<Application> applications)
            {
                Applications = applications;
            }

            /// <summary>
            /// ObservableCollection of the updates found
            /// </summary>
            public ObservableCollection<Application> Applications { get; set; }
        }

        public class ErrorOccurredEventArgs : EventArgs
        {
            public ErrorOccurredEventArgs(string exception)
            {
                ErrorDescription = exception;
            }

            /// <summary>
            /// A string describing the error
            /// </summary>
            public string ErrorDescription { get; set; }
        }

        static void OnEvent<T>(EventHandler<T> Event, T Args) where T : EventArgs
        {
            if (Event != null)
            {
                foreach (EventHandler<T> singleEvent in Event.GetInvocationList())
                {
                    if (singleEvent.Target != null && singleEvent.Target is ISynchronizeInvoke)
                    {
                        ISynchronizeInvoke target = (ISynchronizeInvoke)singleEvent.Target;
                        if (target.InvokeRequired)
                        {
                            target.BeginInvoke(singleEvent, new object[] { "OnEvent", Args });
                            continue;
                        }
                    }
                    singleEvent("OnEvent", Args);
                }
            }
        }

        #endregion
    }
}
