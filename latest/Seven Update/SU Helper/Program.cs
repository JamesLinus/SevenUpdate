using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;

namespace SUHelper
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
        const int MoveOnReboot = 5;
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    Process.GetProcessesByName("Seven Update")[0].CloseMainWindow();

                    Process.GetProcessesByName("Seven Update")[0].Kill();
                }
                catch (Exception) { }

                try
                {
                    Process.GetProcessesByName("Seven Update Admin")[0].Kill();
                }
                catch (Exception) { }

                System.Threading.Thread.Sleep(1000);

                string appStore = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Seven Update\";

                string appDir = Environment.ExpandEnvironmentVariables("%PROGRAMFILES%") + @"\Seven Software\Seven Update\";

                string downloadDir = appStore + @"downloads\Seven Update\" + args[0] + @"\";

                try
                {
                    FileInfo[] files = new DirectoryInfo(downloadDir).GetFiles();

                    for (int x = 0; x < files.Length; x++)
                    {
                        try
                        {
                            File.Copy(files[x].FullName, appDir + files[x].Name, true);
                            SetFileSecurity(appDir + files[x].Name);
                            File.Delete(files[x].FullName);
                        }
                        catch (Exception)
                        {
                            MoveFileEx(files[x].FullName, appDir + files[x].Name, MoveOnReboot);

                            if (!File.Exists(appStore + "reboot.lock"))
                                File.Create(appStore + "reboot.lock").WriteByte(0);
                        }
                    }
                }
                catch (Exception e)
                { Console.WriteLine(e.Message); }

                try
                {
                    if (!File.Exists(appStore + "reboot.lock"))
                    {
                        Directory.Delete(appStore + downloadDir, true);

                        Directory.Delete(appStore + @"downloads\Seven Update", true);

                        Directory.Delete(appStore + @"downloads", true);
                    }
                    else
                        MoveFileEx(appStore + "reboot.lock", null, MoveOnReboot);
                }
                catch (Exception) { }
                Environment.Exit(0);
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    System.Timers.Timer aTimer = new System.Timers.Timer();

                    aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);
                    aTimer.Interval = 7200000;
                    aTimer.Enabled = true;
                    System.Windows.Forms.Application.Run();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Sets the ACLS of a file, removes the current user and sets the owner as the Administrators group
        /// </summary>
        /// <param name="file">The complete path to the file</param>
        internal static void SetFileSecurity(string file)
        {
            try
            {
                IdentityReference userAdmin = new NTAccount("Administrators");

                IdentityReference user = new NTAccount(Environment.UserDomainName, Environment.UserName);

                FileSecurity fs = new FileSecurity(file, AccessControlSections.All);

                fs.SetOwner(userAdmin);

                try
                {
                    IdentityReference users = new NTAccount("Users");

                    fs.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.ReadAndExecute, AccessControlType.Allow));
                }
                catch (Exception) { }

                fs.PurgeAccessRules(user);

                try
                {
                    File.SetAccessControl(file, fs);
                }
                catch (Exception) { }
            }
            catch (Exception) { }
        }

        static void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           // Process.Start(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Seven Update Admin.exe", "Auto");
        }
    }
}