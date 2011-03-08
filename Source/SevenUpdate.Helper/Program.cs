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
namespace SevenUpdate.Helper
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
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
        private static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        #endregion

        #region Methods

        /// <summary>Stops a running process</summary>
        /// <param name="name">The name of the process to kill</param>
        private static void KillProcess(string name)
        {
            try
            {
                var processes = Process.GetProcessesByName(name);
                if (processes.Length > 0)
                {
                    foreach (var t in processes)
                    {
                        t.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                if (!(e is OperationCanceledException || e is UnauthorizedAccessException || e is InvalidOperationException || e is NotSupportedException || e is Win32Exception))
                {
                    throw;
                }
            }
        }

        /// <summary>The main entry point for the application.</summary>
        /// <param name="args">The arguments passed to the program at startup</param>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                KillProcess("SevenUpdate");
                KillProcess("SevenUpdate.Admin");

                Thread.Sleep(1000);

                try
                {
                    File.Delete(Path.Combine(Environment.ExpandEnvironmentVariables("%WINDIR%"), "Temp", "reboot.lock"));
                }
                catch (Exception e)
                {
                    if (!(e is UnauthorizedAccessException || e is IOException))
                    {
                        throw;
                    }

                    NativeMethods.MoveFileExW(Path.Combine(Environment.ExpandEnvironmentVariables("%WINDIR%"), "Temp", "reboot.lock"), null, MoveOnReboot);
                }

                var files = Directory.GetFiles(AppDir, "*.bak");

                foreach (var t in files)
                {
                    try
                    {
                        File.Delete(t);
                    }
                    catch (Exception e)
                    {
                        if (!(e is UnauthorizedAccessException || e is IOException))
                        {
                        }

                        NativeMethods.MoveFileExW(t, null, MoveOnReboot);
                    }
                }

                if (Environment.OSVersion.Version.Major < 6)
                {
                    StartProcess(Path.Combine(AppDir, "SevenUpdate.exe"), "Auto");
                }
                else
                {
                    StartProcess(@"schtasks.exe", "/Run /TN \"SevenUpdate\"");
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
            Process.Start(Path.Combine(AppDir, "SevenUpdate.Admin.exe"), "Auto");
        }

        /// <summary>Starts a process on the system</summary>
        /// <param name="fileName">The file to execute</param>
        /// <param name="arguments">The arguments to execute with the file</param>
        /// <param name="wait">if set to <see langword="true"/> the calling thread will be blocked until process has exited</param>
        /// <param name="hidden">if set to <see langword="true"/> the process will execute with no UI</param>
        /// <returns><see langword="true"/> if the process has executed successfully</returns>
        private static bool StartProcess(string fileName, string arguments, bool wait = false, bool hidden = true)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                if (arguments != null)
                {
                    process.StartInfo.Arguments = arguments;
                }

                if (hidden)
                {
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }

                try
                {
                    process.Start();
                    if (wait)
                    {
                        process.WaitForExit();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    if (!(e is OperationCanceledException || e is UnauthorizedAccessException || e is InvalidOperationException || e is NotSupportedException || e is Win32Exception))
                    {
                        throw;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}