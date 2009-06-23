/*Copyright 2007-09 Robert Baker, aka Seven ALive.
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace SevenUpdate
{
    class Program
    {

        #region Global Vars

        /// <summary>
        /// The tray icon for Seven Update
        /// </summary>
        internal static NotifyIcon trayIcon;

        /// <summary>
        /// The UI Resource Strings
        /// </summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.UIStrings", typeof(Program).Assembly);

        /// <summary>
        /// List of Applications
        /// </summary>
        internal static Collection<Application> Applications { get; set; }


        
        #endregion

        #region Methods
        
        /// <summary>
        /// The main entry point for the application.
        /// </summar6y>
        /// <param name="args">Command line args</param>
        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            bool createdNew = true;
            // Makes sure only 1 copy of Seven Update is allowed to run
            using (Mutex mutex = new Mutex(true, "Seven Update", out createdNew))
            {
                for (int x = 0; x < args.Length; x++)
                {
                    if (args[0].EndsWith(".sua", StringComparison.OrdinalIgnoreCase))
                    {
                        AddSUA(args[x]);
                        return;
                    }
                }
                if (createdNew)
                {
                    trayIcon = new NotifyIcon();
                    if (args.Length < 1)
                    {
                        System.Windows.Forms.Application.Run(new SevenUpdate.WinForms.Main());
                    }
                    else
                    {
                        if (args[0] == "Auto")
                        {
                            SevenUpdate.WinForms.Main.AutoCheck = true;
                            System.Windows.Forms.Application.Run(new SevenUpdate.WinForms.Main());
                        }
                    }
                }

                try
                {
                    trayIcon.Icon = null;
                    trayIcon.Visible = false;
                    Process.GetProcessesByName("Seven Update Admin")[0].Kill();
                }
                catch (Exception) { }
                if (!createdNew)
                {
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Adds an Application for use with Seven Update
        /// </summary>
        /// <param name="suaLoc">The location of the SUA file</param>
        static void AddSUA(string suaLoc)
        {
            WebClient wc = new WebClient();
            wc.DownloadFile(suaLoc, Shared.userStore + "add.sua");
            SUA sua = Shared.Deserialize<SUA>(Shared.userStore + "add.sua");
            Collection<SUA> sul = Shared.DeserializeCollection<SUA>(Shared.appStore + "SUApps.sul");
            File.Delete(Shared.userStore + "add.sua");
            int index = sul.IndexOf(sua);
            if (index < 0)
            {
                if (MessageBox.Show(Program.RM.GetString("AllowUpdates") + " " + sua.ApplicationName[0].Value + "?", Program.RM.GetString("SevenUpdate"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sul.Add(sua);
                   
                    SevenUpdate.WCF.Client.AddSUA(sul);
                }
            }

        }

        #region Recount Methods

        /// <summary>
        /// Gets the download size of a single update
        /// </summary>
        /// <param name="update">The update to get the size for</param>
        /// <param name="appName">The name of the application that owns the update</param>
        /// <param name="appDir">The application directory of the update</param>
        /// <param name="Is64Bit">Specifies if the update/application is 64 bit.</param>
        /// <param name="downloadSizeOnly">Specifies if to only count the fize size that needs to be downloaded</param>
        /// <returns>Returns a ulong value of the size of the download</returns>
        internal static ulong GetUpdateSize(Update update, string appName, string appDir, bool Is64Bit, bool downloadSizeOnly)
        {
            ulong size = 0;

            for (int x = 0; x < update.Files.Count; x++)
            {
                if (downloadSizeOnly)
                {
                    if (Shared.GetHash(Shared.appStore + @"downloads\" + appName + @"\" + update.Title + @"\" + Path.GetFileName(Shared.ConvertPath(update.Files[x].Destination,
                        appDir, Is64Bit))) != update.Files[x].Hash)
                        size += update.Files[x].Size;
                }
                else
                    size += update.Files[x].Size;

            }

            return size;
        }

        #endregion

        #region History Methods

        /// <summary>
        /// Gets Hidden Updates
        /// </summary>
        /// <returns>Returns the list of hidden updates</returns>
        internal static Collection<UpdateInformation> GetHiddenUpdates()
        {
            return Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Hidden Updates.xml");
        }

        /// <summary>
        /// Gets the update history
        /// </summary>
        /// <returns>Returns a list of updates installed</returns>
        internal static Collection<UpdateInformation> GetHistory()
        {
            return Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Update History.xml");
        }

        #endregion

        #endregion

        #region Vista UI methods

        /// <summary>
        /// Specifies if the current user running Seven Update is an administrator
        /// </summary>
        /// <returns></returns>
        static internal bool IsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Adds a shield to a button
        /// </summary>
        /// <param name="b">The button object you want to add a shield to</param>
        static internal void AddShieldToButton(Button b)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    b.FlatStyle = FlatStyle.System;
                    NativeMethods.SendMessage(b.Handle, NativeMethods.BCM_SETSHIELD, 0, 0xFFFFFFFF);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Removes a shield from a button
        /// </summary>
        /// <param name="b">The button object you want to remove the shield from</param>
        static internal void RemoveShieldFromButton(Button b)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    b.FlatStyle = FlatStyle.System;
                    NativeMethods.SendMessage(b.Handle, NativeMethods.BCM_SETSHIELD, 0, 0);
                }
                catch (Exception) { }
            }
        }

        #endregion
    }

    static class NativeMethods
    {
        #region DLL Imports

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        internal static extern UInt32 SendMessage
            (IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        /// <summary>
        /// Normal Button
        /// </summary>
        internal const int BCM_FIRST = 0x1600;

        /// <summary>
        /// Elevated button, shows shield
        /// </summary>
        internal const int BCM_SETSHIELD = (BCM_FIRST + 0x000C);

        #endregion
    }
}
