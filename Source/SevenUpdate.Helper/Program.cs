﻿// ***********************************************************************
// <copyright file="Program.cs"
//            project="SevenUpdate.Helper"
//            assembly="SevenUpdate.Helper"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
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

    /// <summary>
    /// The main class
    /// </summary>
    internal static class Program
    {
        #region Constants and Fields

        /// <summary>
        ///   Moves a file on reboot
        /// </summary>
        private const int MoveOnReboot = 5;

        /// <summary>
        ///   The current directory the application resides in
        /// </summary>
        private static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";

        #endregion

        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The arguments passed to the program at startup
        /// </param>
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
                            MoveFileExW(t.FullName, AppDir + t.Name, MoveOnReboot);

                            if (!File.Exists(appStore + "reboot.lock"))
                            {
                                File.Create(appStore + "reboot.lock").WriteByte(0);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    if (!File.Exists(appStore + @"reboot.lock"))
                    {
                        Directory.Delete(appStore + downloadDir, true);

                        Directory.Delete(appStore + @"downloads", true);
                    }
                    else
                    {
                        MoveFileExW(appStore + @"reboot.lock", null, MoveOnReboot);
                    }
                }
                catch (Exception)
                {
                }

                if (Environment.OSVersion.Version.Major < 6)
                {
                    Process.Start(AppDir + @"SevenUpdate.exe", "Auto");
                }
                else
                {
                    var p = new Process
                        {
                            StartInfo =
                                {
                                    FileName = @"schtasks.exe", 
                                    CreateNoWindow = true, 
                                    WindowStyle = ProcessWindowStyle.Hidden, 
                                    Arguments = "/Run /TN \"SevenUpdate\""
                                }
                        };
                    p.Start();
                }

                Environment.Exit(0);
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    var timer = new Timer();

                    timer.Elapsed += RunSevenUpdate;
                    timer.Interval = 7200000;
                    timer.Enabled = true;
                    Application.Run();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Moves the file using the windows command
        /// </summary>
        /// <param name="sourceFileName">
        /// The current name of the file or directory on the local computer.
        /// </param>
        /// <param name="newFileName">
        /// The new name of the file or directory on the local computer.
        /// </param>
        /// <param name="flags">
        /// The flags that determine how to move the file
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
        /// </returns>
        [DllImport(@"kernel32.dll")]
        private static extern bool MoveFileExW(string sourceFileName, string newFileName, int flags);

        /// <summary>
        /// Run Seven Update and auto check for updates
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.
        /// </param>
        private static void RunSevenUpdate(object sender, ElapsedEventArgs e)
        {
            Process.Start(AppDir + @"SevenUpdate.Admin.exe", "Auto");
        }

        #endregion
    }
}