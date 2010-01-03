using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SevenUpdate.Helper
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
        const int MoveOnReboot = 5;

        static string appDir = Environment.ExpandEnvironmentVariables("%PROGRAMFILES%") + @"\Seven Software\Seven Update\";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {

                var appStore = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Seven Software\Seven Update\";

                var downloadDir = appStore + @"downloads\" + args[0] + @"\";

                try
                {
                    Process.GetProcessesByName("Seven Update")[0].CloseMainWindow();

                    Process.GetProcessesByName("Seven Update")[0].Kill();
                }
                catch (Exception) { }

                try
                {
                    Process.GetProcessesByName("Seven Update.Admin")[0].Kill();
                }
                catch (Exception) { }

                try
                {
                    if (File.Exists(appStore + "reboot.lock"))
                        File.Delete(appStore + "reboot.lock");
                }
                catch
                {
                }

                System.Threading.Thread.Sleep(1000);

                try
                {
                    FileInfo[] files = new DirectoryInfo(downloadDir).GetFiles();

                    foreach (FileInfo t in files)
                    {
                        try
                        {
                            File.Copy(t.FullName, appDir + t.Name, true);
                        }
                        catch (Exception)
                        {
                            MoveFileEx(t.FullName, appDir + t.Name, MoveOnReboot);

                            if (!File.Exists(appStore + "reboot.lock"))
                                File.Create(appStore + "reboot.lock").WriteByte(0);
                        }
                        try 
                        { 
                            File.Delete(t.FullName); 
                        }
                        catch
                        {
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

                        Directory.Delete(appStore + @"downloads", true);
                    }
                    else
                        MoveFileEx(appStore + "reboot.lock", null, MoveOnReboot);
                }
                catch (Exception) { }
                Process.Start(appDir + "Seven Update.exe", "Auto");
                Environment.Exit(0);
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    var aTimer = new System.Timers.Timer();

                    aTimer.Elapsed += ATimerElapsed;
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

        static void ATimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           Process.Start(appDir + @"Seven Update.Admin.exe", "Auto");
        }
    }
}