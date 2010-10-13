// ***********************************************************************
// <copyright file="Program.cs"
//            project="SevenUpdate.Helper"
//            assembly="SevenUpdate.Helper"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Helper
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Timers;
    using System.Windows.Forms;

    using Timer = System.Timers.Timer;

    /// <summary>The main class</summary>
    internal static class Program
    {
        #region Constants and Fields

        /// <summary>Moves a file on reboot</summary>
        private const int MoveOnReboot = 5;

        /// <summary>The current directory the application resides in</summary>
        private static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";

        #endregion

        #region Methods

        /// <summary>The main entry point for the application.</summary>
        /// <param name="args">The arguments passed to the program at startup</param>
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
                catch (Exception)
                {
                    try
                    {
                        Process.GetProcessesByName("SevenUpdate")[0].Kill();
                    }
                    catch (Exception)
                    {
                    }
                }

                try
                {
                    Process.GetProcessesByName("SevenUpdate.Admin")[0].Kill();
                }
                catch (Exception)
                {
                }

                Thread.Sleep(1000);

                try
                {
                    File.Delete(appStore + "reboot.lock");
                }
                catch (Exception)
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
                            catch (Exception)
                            {
                            }
                        }
                        catch (Exception)
                        {
                            NativeMethods.MoveFileExW(t.FullName, AppDir + t.Name, MoveOnReboot);

                            if (!File.Exists(appStore + "reboot.lock"))
                            {
                                using (var file = File.Create(appStore + "reboot.lock"))
                                {
                                    file.WriteByte(0);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (!File.Exists(appStore + @"reboot.lock"))
                {
                    try
                    {
                        Directory.Delete(appStore + downloadDir, true);
                        Directory.Delete(appStore + @"downloads", true);
                    }
                    catch (Exception e)
                    {
                        if (!(e is OperationCanceledException || e is UnauthorizedAccessException || e is InvalidOperationException || e is NotSupportedException))
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    NativeMethods.MoveFileExW(appStore + @"reboot.lock", null, MoveOnReboot);
                }

                if (Environment.OSVersion.Version.Major < 6)
                {
                    Process.Start(AppDir + @"SevenUpdate.exe", "Auto");
                }
                else
                {
                    Process.Start(@"schtasks.exe", "/Run /TN \"SevenUpdate\"");
                }

                Environment.Exit(0);
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    using (var timer = new Timer())
                    {
                        timer.Elapsed += RunSevenUpdate;
                        timer.Interval = 7200000;
                        timer.Enabled = true;
                        Application.Run();
                    }
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>Run Seven Update and auto check for updates</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private static void RunSevenUpdate(object sender, ElapsedEventArgs e)
        {
            Process.Start(AppDir + @"SevenUpdate.Admin.exe", "Auto");
        }

        #endregion
    }
}