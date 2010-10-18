// ***********************************************************************
// <copyright file="WcfServiceCallback.cs"
//            project="SevenUpdate.Admin"
//            assembly="SevenUpdate.Admin"
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
namespace SevenUpdate.Admin
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Win32;

    using SevenUpdate.Service;

    /// <summary>
    /// Contains methods to execute for the service callback
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class WcfServiceCallback : IElevatedProcess
    {
        /// <summary>Adds an application to Seven Update, so it can manage updates for it.</summary>
        /// <param name="application">The application to add to Seven Update</param>
        public void AddApp(Sua application)
        {
            var sul = Utilities.Deserialize<Collection<Sua>>(Utilities.ApplicationsFile);
            var exists = false;

            foreach (var t in sul.Where(t => t.Directory == application.Directory && t.Is64Bit == application.Is64Bit))
            {
                exists = true;
            }

            if (exists)
            {
                return;
            }

            sul.Add(application);

            Utilities.Serialize(sul, Utilities.ApplicationsFile);
        }

        /// <summary>Changes the program settings</summary>
        /// <param name="applications">The applications Seven Update will check and manage updates for</param>
        /// <param name="options">The Seven Update settings</param>
        /// <param name="autoCheck">if set to <see langword="true"/> automatic updates will be enabled</param>
        public void ChangeSettings(Collection<Sua> applications, Config options, bool autoCheck)
        {
            if (!autoCheck)
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    // ReSharper disable PossibleNullReferenceException
                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Seven Update Automatic Checking", false);

                    // ReSharper restore PossibleNullReferenceException
                }
                else
                {
                    Utilities.StartProcess(@"schtasks.exe", "/Change /Disable /TN \"SevenUpdate.Admin\"");
                }
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", "Seven Update Automatic Checking", Utilities.AppDir + @"SevenUpdate.Helper.exe ");
                }
                else
                {
                    Utilities.StartProcess(@"schtasks.exe", "/Change /Enable /TN \"SevenUpdate.Admin\"");
                }
            }

            Utilities.Serialize(applications, Utilities.ApplicationsFile);
            Utilities.Serialize(options, Utilities.ConfigFile);
        }

        /// <summary>Hides a single update</summary>
        /// <param name="hiddenUpdate">The update to hide</param>
        public void HideUpdate(Suh hiddenUpdate)
        {
            var hidden = Utilities.Deserialize<Collection<Suh>>(Utilities.HiddenFile) ?? new Collection<Suh>();
            hidden.Add(hiddenUpdate);

            Utilities.Serialize(hidden, Utilities.HiddenFile);
        }

        /// <summary>Hides a collection of <see cref="Suh"/> to hide</summary>
        /// <param name="hiddenUpdates">The collection of updates to hide</param>
        public void HideUpdates(Collection<Suh> hiddenUpdates)
        {
            Utilities.Serialize(hiddenUpdates, Utilities.HiddenFile);
        }

        /// <summary>Gets a collection of <see cref="Sui"/></summary>
        /// <param name="applicationUpdates">The collection of applications and updates to install</param>
        public void InstallUpdates(Collection<Sui> applicationUpdates)
        {
            try
            {
                if (File.Exists(Utilities.AllUserStore + "abort.lock"))
                {
                    File.Delete(Utilities.AllUserStore + "abort.lock");
                }
            }
            catch (Exception e)
            {
                if (!(e is UnauthorizedAccessException || e is InvalidOperationException))
                {
                    throw;
                }

                Utilities.ReportError(e, Utilities.AllUserStore);
            }

            App.Applications = applicationUpdates;
            Task.Factory.StartNew(() => Download.DownloadUpdates(applicationUpdates, "SevenUpdate", true));
        }

        /// <summary>The update to show and remove from hidden updates</summary>
        /// <param name="hiddenUpdate">The hidden update to show</param>
        public void ShowUpdate(Suh hiddenUpdate)
        {
            var show = Utilities.Deserialize<Collection<Suh>>(Utilities.HiddenFile) ?? new Collection<Suh>();

            if (show.Count == 0)
            {
                File.Delete(Utilities.HiddenFile);
            }
            else
            {
                show.Remove(hiddenUpdate);
                Utilities.Serialize(show, Utilities.HiddenFile);
            }
        }

        /// <summary>Shutdown the admin process if it's not installing updates. Execute the admin process with Abort</summary>
        public void Shutdown()
        {
            Task.Factory.StartNew(
                () =>
                    {
                        Thread.Sleep(500);
                        if (!App.IsInstalling)
                        {
                            Environment.Exit(0);
                        }
                    });
        }
    }
}