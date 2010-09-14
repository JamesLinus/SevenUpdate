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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

#endregion

namespace SevenUpdate.Helper
{
    internal static class Program
    {
        private const int MoveOnReboot = 5;

        private static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";

        [DllImport("kernel32.dll")]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var appStore = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Seven Software\Seven Update\";

                var downloadDir = appStore + @"downloads\" + args[0] + @"\";

                try
                {
                    Process.GetProcessesByName("SevenUpdate")[0].CloseMainWindow();
                }
                catch
                {
                    try
                    {
                        Process.GetProcessesByName("SevenUpdate")[0].Kill();
                    }
                    catch
                    {
                    }
                }

                try
                {
                    Process.GetProcessesByName("SevenUpdate.Admin")[0].Kill();
                }
                catch
                {
                }

                try
                {
                    if (File.Exists(appStore + "reboot.lock"))
                        File.Delete(appStore + "reboot.lock");
                }
                catch
                {
                }

                Thread.Sleep(1000);

                try
                {
                    var files = new DirectoryInfo(downloadDir).GetFiles();

                    foreach (var t in files)
                    {
                        try
                        {
                            File.Copy(t.FullName, AppDir + t.Name, true);
                            try
                            {
                                File.Delete(t.FullName);
                            }
                            catch
                            {
                            }
                        }
                        catch
                        {
                            MoveFileEx(t.FullName, AppDir + t.Name, MoveOnReboot);

                            if (!File.Exists(appStore + "reboot.lock"))
                                File.Create(appStore + "reboot.lock").WriteByte(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

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
                catch
                {
                }
                if (Environment.OSVersion.Version.Major < 6)
                    Process.Start(AppDir + "SevenUpdate.exe", "Auto");
                else
                {
                    var p = new Process {StartInfo = {FileName = "schtasks.exe", CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden, Arguments = "/Run /TN \"SevenUpdate\""}};
                    p.Start();
                }
                Environment.Exit(0);
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    var aTimer = new Timer();

                    aTimer.Elapsed += ATimerElapsed;
                    aTimer.Interval = 7200000;
                    aTimer.Enabled = true;
                    Application.Run();
                }
                else
                    Environment.Exit(0);
            }
        }

        private static void ATimerElapsed(object sender, ElapsedEventArgs e)
        {
            Process.Start(AppDir + @"SevenUpdate.Admin.exe", "Auto");
        }
    }
}