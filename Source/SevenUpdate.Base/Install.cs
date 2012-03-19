// <copyright file="Install.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Win32;

    /// <summary>Class containing methods to install updates.</summary>
    public static class Install
    {
        /// <summary>Gets an int that indicates to move a file on reboot.</summary>
        private const int MoveOnReboot = 5;

        /// <summary>Indicates if the installation of updates should be canceled.</summary>
        private static bool cancelInstall;

        /// <summary>The localized name of the current update being installed.</summary>
        private static string currentUpdateName;

        /// <summary>Indicates if an error has occurred.</summary>
        private static bool errorOccurred;

        /// <summary>The total number of updates being installed.</summary>
        private static int updateCount;

        /// <summary>The index position of the current update being installed.</summary>
        private static int updateIndex;

        /// <summary>Occurs when the installation completed.</summary>
        public static event EventHandler<InstallCompletedEventArgs> InstallCompleted;

        /// <summary>Occurs when the installation progress changed.</summary>
        public static event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;

        /// <summary>Occurs when the installation progress changed.</summary>
        public static event EventHandler<UpdateInstalledEventArgs> UpdateInstalled;

        /// <summary>Gets a value indicating whether Seven Update is installing updates.</summary>
        public static bool IsInstalling { get; private set; }

        /// <summary>Cancel the installation of updates.</summary>
        public static void CancelInstall()
        {
            cancelInstall = true;
        }

        /// <summary>Installs updates.</summary>
        /// <param name="applications">The collection of applications to install updates.</param>
        /// <param name="downloadDirectory">The directory containing the app update files.</param>
        public static void InstallUpdates(Collection<Sui> applications, string downloadDirectory)
        {
            if (applications == null)
            {
                throw new ArgumentNullException("applications");
            }

            if (applications.Count < 1)
            {
                throw new ArgumentNullException("applications");
            }

            IsInstalling = true;
            updateCount = applications.Sum(t => t.Updates.Count);
            int completedUpdates = 0, failedUpdates = 0;

            ReportProgress(0);

            for (int x = 0; x < applications.Count; x++)
            {
                for (int y = 0; y < applications[x].Updates.Count; y++)
                {
                    errorOccurred = false;
                    if (cancelInstall)
                    {
                        Environment.Exit(0);
                    }

                    currentUpdateName = Utilities.GetLocaleString(applications[x].Updates[y].Name);

                    ReportProgress(5);

                    SetRegistryItems(applications[x].Updates[y].RegistryItems, applications[x].AppInfo.Platform);

                    ReportProgress(25);

                    UpdateFiles(applications[x].Updates[y].Files, Path.Combine(downloadDirectory, currentUpdateName));

                    ReportProgress(75);

                    SetShortcuts(applications[x].Updates[y].Shortcuts, applications[x].AppInfo);

                    ReportProgress(95);

                    if (errorOccurred)
                    {
                        failedUpdates++;
                        AddHistory(applications[x], applications[x].Updates[y]);
                    }
                    else
                    {
                        completedUpdates++;
                        AddHistory(applications[x], applications[x].Updates[y]);
                    }

                    // if (applications[x].AppInfo.Directory == Utilities.ConvertPath(@"%PROGRAMFILES%\Seven Update",
                    // true, Platform.AnyCpu)) { selfUpdate = true; }
                    ReportProgress(100);

                    updateIndex++;
                }
            }

            ReportProgress(100);

            if (Utilities.RebootNeeded)
            {
                string fileName = Path.Combine(
                    Environment.ExpandEnvironmentVariables("%WINDIR%"), "Temp", "reboot.lock");
                if (!File.Exists(fileName))
                {
                    using (FileStream file = File.Create(fileName))
                    {
                        file.WriteByte(0);
                    }
                }

                NativeMethods.MoveFileExW(
                    Path.Combine(Environment.ExpandEnvironmentVariables("%WINDIR%"), "Temp", "reboot.lock"), 
                    null, 
                    MoveOnReboot);

                if (Directory.Exists(downloadDirectory))
                {
                    NativeMethods.MoveFileExW(downloadDirectory, null, MoveOnReboot);
                }
            }
            else
            {
                // Delete the downloads directory if no errors were found and no reboot is needed
                if (!errorOccurred)
                {
                    if (Directory.Exists(downloadDirectory))
                    {
                        try
                        {
                            Directory.Delete(downloadDirectory, true);
                        }
                        catch (IOException)
                        {
                        }
                    }
                }
            }

            // if (selfUpdate) { Utilities.StartProcess(Path.Combine(Utilities.AppDir, @"SevenUpdate.Helper.exe"),
            // "-cleanup"); }
            IsInstalling = false;
            if (InstallCompleted != null)
            {
                InstallCompleted(null, new InstallCompletedEventArgs(completedUpdates, failedUpdates));
            }

            return;
        }

        /// <summary>Adds an update to the update history.</summary>
        /// <param name="appInfo">The application information.</param>
        /// <param name="updateInfo">The update information.</param>
        /// <param name="failed"><c>True</c> if the update failed, otherwise <c>False</c>.</param>
        private static void AddHistory(Sui appInfo, Update updateInfo, bool failed = false)
        {
            var hist = new Suh(updateInfo.Name, appInfo.AppInfo.Publisher, updateInfo.Description)
                {
                    HelpUrl = appInfo.AppInfo.HelpUrl, 
                    AppUrl = appInfo.AppInfo.AppUrl, 
                    Status = failed == false ? UpdateStatus.Successful : UpdateStatus.Failed, 
                    InfoUrl = updateInfo.InfoUrl, 
                    InstallDate = DateTime.Now.ToShortDateString(), 
                    ReleaseDate = updateInfo.ReleaseDate, 
                    Importance = updateInfo.Importance, 
                };

            if (UpdateInstalled != null)
            {
                UpdateInstalled(null, new UpdateInstalledEventArgs(hist));
            }
        }

        /// <summary>Reports the installation progress.</summary>
        /// <param name="installProgress">The current install progress percentage.</param>
        private static void ReportProgress(int installProgress)
        {
            if (InstallProgressChanged != null)
            {
                InstallProgressChanged(
                    null, 
                    new InstallProgressChangedEventArgs(currentUpdateName, installProgress, updateIndex, updateCount));
            }
        }

        /// <summary>Sets the registry items of an update.</summary>
        /// <param name="regItems">The registry changes to install on the system.</param>
        /// <param name="platform">A value that indicates what cpu architecture the application supports.</param>
        private static void SetRegistryItems(IList<RegistryItem> regItems, Platform platform)
        {
            if (regItems == null)
            {
                return;
            }

            for (int x = 0; x < regItems.Count; x++)
            {
                string keyPath = Utilities.ParseRegistryKey(regItems[x].Key, platform);
                RegistryKey key;
                switch (keyPath)
                {
                    case "HKEY_CLASSES_ROOT":
                        key = Registry.ClassesRoot;
                        break;
                    case "HKEY_CURRENT_USER":
                        key = Registry.CurrentUser;
                        break;
                    case "HKEY_LOCAL_MACHINE":
                        key = Registry.LocalMachine;
                        break;
                    case "HKEY_USERS":
                        key = Registry.Users;
                        break;
                    default:
                        key = Registry.CurrentUser;
                        break;
                }

                switch (regItems[x].Action)
                {
                    case RegistryAction.Add:
                        try
                        {
                            Registry.SetValue(keyPath, regItems[x].KeyValue, regItems[x].Data, regItems[x].ValueKind);
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Utilities.ReportError(e, ErrorType.InstallationError);
                            errorOccurred = true;
                        }

                        break;
                    case RegistryAction.DeleteKey:
                        try
                        {
                            if (key != null)
                            {
                                // ReSharper disable PossibleNullReferenceException
                                key.OpenSubKey(regItems[x].Key, true).DeleteSubKeyTree(regItems[x].Key);
                            }

                            // ReSharper restore PossibleNullReferenceException
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Utilities.ReportError(e, ErrorType.InstallationError);
                        }

                        break;
                    case RegistryAction.DeleteValue:
                        try
                        {
                            // ReSharper disable PossibleNullReferenceException
                            key.OpenSubKey(regItems[x].Key, true).DeleteValue(regItems[x].KeyValue, false);

                            // ReSharper restore PossibleNullReferenceException
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Utilities.ReportError(e, ErrorType.InstallationError);
                        }

                        break;
                }

                int installProgress = (x * 100) / regItems.Count;
                if (installProgress > 30)
                {
                    installProgress -= 10;
                }

                ReportProgress(installProgress);
            }
        }

        /// <summary>Installs the shortcuts of an update.</summary>
        /// <param name="shortcuts">The shortcuts to install on the system.</param>
        /// <param name="appInfo">The application information.</param>
        private static void SetShortcuts(IList<Shortcut> shortcuts, Sua appInfo)
        {
            if (shortcuts == null)
            {
                return;
            }

            // Choose the path for the shortcut
            for (int x = 0; x < shortcuts.Count; x++)
            {
                shortcuts[x].Location = Utilities.ExpandInstallLocation(
                    shortcuts[x].Location, appInfo.Directory, appInfo.Platform, appInfo.ValueName);
                string linkName = Utilities.GetLocaleString(shortcuts[x].Name);

                if (shortcuts[x].Action == ShortcutAction.Add
                    ||
                    (shortcuts[x].Action == ShortcutAction.Update
                     && File.Exists(Path.Combine(shortcuts[x].Location, linkName + ".lnk"))))
                {
                    if (!Directory.Exists(shortcuts[x].Location))
                    {
                        Directory.CreateDirectory(shortcuts[x].Location);
                    }

                    File.Delete(Path.Combine(shortcuts[x].Location, linkName + ".lnk"));

                    Shortcut.CreateShortcut(shortcuts[x]);
                }

                if (shortcuts[x].Action == ShortcutAction.Delete)
                {
                    File.Delete(shortcuts[x].Location);
                }

                int installProgress = (x * 100) / shortcuts.Count;
                if (installProgress > 90)
                {
                    installProgress -= 15;
                }

                ReportProgress(installProgress);
            }
        }

        /// <summary>Updates the file on the system.</summary>
        /// <param name="file">The file to install or update.</param>
        private static void UpdateFile(UpdateFile file)
        {
            switch (file.Action)
            {
                case FileAction.ExecuteThenDelete:
                case FileAction.UnregisterThenDelete:
                case FileAction.Delete:
                    if (file.Action == FileAction.ExecuteThenDelete)
                    {
                        if (File.Exists(file.Source))
                        {
                            Utilities.StartProcess(file.Source, file.Args, true);
                        }
                    }

                    if (file.Action == FileAction.UnregisterThenDelete)
                    {
                        Utilities.StartProcess("regsvr32", "/u /s" + file.Destination);
                    }

                    try
                    {
                        File.Delete(file.Destination);
                    }
                    catch (IOException)
                    {
                        NativeMethods.MoveFileExW(file.Destination, null, MoveOnReboot);
                    }

                    break;

                case FileAction.Execute:
                    try
                    {
                        Utilities.StartProcess(file.Destination, file.Args);
                    }
                    catch (FileNotFoundException e)
                    {
                        Utilities.ReportError(e, ErrorType.InstallationError);
                        errorOccurred = true;
                    }

                    break;

                case FileAction.Update:
                case FileAction.UpdateIfExist:
                case FileAction.UpdateThenExecute:
                case FileAction.UpdateThenRegister:
                    if (File.Exists(file.Source))
                    {
                        try
                        {
                            if (File.Exists(file.Destination + @".bak"))
                            {
                                File.Delete(file.Destination + @".bak");
                            }

                            if (File.Exists(file.Destination))
                            {
                                File.Move(file.Destination, file.Destination + @".bak");
                            }

                            File.Move(file.Source, file.Destination);

                            if (File.Exists(file.Destination + @".bak"))
                            {
                                try
                                {
                                    File.Delete(file.Destination + @".bak");
                                }
                                catch (Exception e)
                                {
                                    if (!(e is UnauthorizedAccessException || e is IOException))
                                    {
                                        Utilities.ReportError(e, ErrorType.InstallationError);
                                        throw;
                                    }

                                    Utilities.RebootNeeded = true;
                                    NativeMethods.MoveFileExW(file.Source, file.Destination, MoveOnReboot);
                                    NativeMethods.MoveFileExW(file.Source, file.Destination + ".bak", MoveOnReboot);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (!(e is UnauthorizedAccessException || e is IOException))
                            {
                                Utilities.ReportError(e, ErrorType.InstallationError);
                                throw;
                            }

                            Utilities.RebootNeeded = true;
                            NativeMethods.MoveFileExW(file.Source, file.Destination, MoveOnReboot);
                            NativeMethods.MoveFileExW(file.Source, file.Destination + ".bak", MoveOnReboot);
                        }
                    }
                    else
                    {
                        Utilities.ReportError(new FileNotFoundException(file.Source), ErrorType.InstallationError);
                        errorOccurred = true;
                    }

                    if (file.Action == FileAction.UpdateThenExecute)
                    {
                        try
                        {
                            Utilities.StartProcess(file.Destination, file.Args);
                        }
                        catch (FileNotFoundException e)
                        {
                            Utilities.ReportError(e, ErrorType.InstallationError);
                            errorOccurred = true;
                        }
                    }

                    if (file.Action == FileAction.UpdateThenRegister)
                    {
                        Utilities.StartProcess(@"regsvr32", @"/s" + file.Destination);
                    }

                    break;
            }
        }

        /// <summary>Installs the files in the update.</summary>
        /// <param name="files">The collection of files to update.</param>
        /// <param name="downloadDirectory">The path to the download folder where the update files are located.</param>
        private static void UpdateFiles(IList<UpdateFile> files, string downloadDirectory)
        {
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }

            if (downloadDirectory == null)
            {
                throw new ArgumentNullException("downloadDirectory");
            }

            for (int x = 0; x < files.Count; x++)
            {
                files[x].Source = Path.Combine(downloadDirectory, Path.GetFileName(files[x].Destination));
                try
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    Directory.CreateDirectory(Path.GetDirectoryName(files[x].Destination));

                    // ReSharper restore AssignNullToNotNullAttribute
                }
                catch (IOException e)
                {
                    Utilities.ReportError(e, ErrorType.InstallationError);
                    errorOccurred = true;
                }

                int x1 = x;
                int x2 = x;
                Task task = Task.Factory.StartNew(() => UpdateFile(files[x1])).ContinueWith(
                    delegate
                        {
                            int installProgress = (x2 * 100) / files.Count;
                            if (installProgress > 70)
                            {
                                installProgress -= 15;
                            }

                            ReportProgress(installProgress);
                        });
                task.Wait();
            }
        }
    }
}