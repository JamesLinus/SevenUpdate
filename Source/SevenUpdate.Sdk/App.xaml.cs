// ***********************************************************************
// <copyright file="App.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
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
namespace SevenUpdate.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Shell;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Interaction logic for App.xaml</summary>
    public sealed partial class App
    {
        #region Properties

        /// <summary>The user application data location</summary>
        public static readonly string UserStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seven Software", "Seven Update SDK");

        /// <summary>Gets the command line arguments passed to this instance</summary>
        internal static IList<string> Args { get; private set; }

        #endregion

        #region Methods

        /// <summary>Process command line args</summary>
        /// <param name="args">The list of arguments</param>
        internal static void ProcessArgs(IList<string> args)
        {
            if (args == null)
            {
                return;
            }

            if (args.Count <= 0)
            {
                return;
            }

            switch (args[0])
            {
                case @"-newproject":
                    Core.NewProject();
                    break;

                case @"-newupdate":
                    Core.AppIndex = Convert.ToInt32(args[1], CultureInfo.CurrentCulture);

                    Core.NewUpdate();
                    break;

                case @"-edit":
                    Core.AppIndex = Convert.ToInt32(args[1], CultureInfo.CurrentCulture);
                    Core.UpdateIndex = Convert.ToInt32(args[2], CultureInfo.CurrentCulture);
                    Core.EditItem();
                    break;
            }
        }

        /// <summary>Raises the <see cref="InstanceAwareApplication.Startup"/> event.</summary>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        /// <param name="isFirstInstance">If set to <see langword="true"/> the current instance is the first application instance.</param>
        protected override void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            Utilities.Locale = Settings.Default.locale;
            base.OnStartup(e, isFirstInstance);

            if (!isFirstInstance)
            {
                this.Shutdown(1);
            }
            else
            {
                Args = e.Args;
                Directory.CreateDirectory(UserStore);
                Core.Projects = Utilities.Deserialize<Collection<Project>>(Core.ProjectsFile);
                SetJumpList();
            }
        }

        /// <summary>Raises the <see cref="InstanceAwareApplication.StartupNextInstance"/> event.</summary>
        /// <param name="e">The <see cref="StartupNextInstanceEventArgs"/> instance containing the event data.</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);
            ProcessArgs(e.GetArgs());
        }

        /// <summary>Sets the Windows 7 <see cref="JumpList"/></summary>
        private static void SetJumpList()
        {
            // Create JumpTask
            var jumpList = new JumpList();
            JumpTask jumpTask;

            if (Core.Projects != null)
            {
                var startIndex = Core.Projects.Count - 2;
                if (startIndex < 0)
                {
                    startIndex = 0;
                }

                for (var x = startIndex; x < Core.Projects.Count; x++)
                {
                    jumpTask = new JumpTask
                        {
                            ApplicationPath = Path.Combine(Utilities.AppDir, "SevenUpdate.Sdk.exe"),
                            IconResourcePath = Path.Combine(Utilities.AppDir, @"SevenUpdate.Base.dll"),
                            IconResourceIndex = 7,
                            Title = Sdk.Properties.Resources.CreateUpdate,
                            CustomCategory = Core.Projects[x].ApplicationName,
                            Arguments = @"-newupdate " + x,
                        };
                    jumpList.JumpItems.Add(jumpTask);
                    for (var y = 0; y < Core.Projects[x].UpdateNames.Count; y++)
                    {
                        jumpTask = new JumpTask
                            {
                                ApplicationPath = Path.Combine(Utilities.AppDir, "SevenUpdate.Sdk.exe"),
                                IconResourcePath = Path.Combine(Utilities.AppDir, @"SevenUpdate.Base.dll"),
                                IconResourceIndex = 8,
                                Title = String.Format(CultureInfo.CurrentCulture, Sdk.Properties.Resources.Edit, Core.Projects[x].UpdateNames[y]),
                                CustomCategory = Core.Projects[x].ApplicationName,
                                Arguments = @"-edit " + x + " " + y
                            };

                        jumpList.JumpItems.Add(jumpTask);
                    }
                }
            }

            // Configure a new JumpTask
            jumpTask = new JumpTask
                {
                    ApplicationPath = Path.Combine(Utilities.AppDir, "SevenUpdate.Sdk.exe"),
                    IconResourcePath = Path.Combine(Utilities.AppDir, @"SevenUpdate.Base.dll"),
                    IconResourceIndex = 6,
                    Title = Sdk.Properties.Resources.CreateProject,
                    CustomCategory = Sdk.Properties.Resources.Tasks,
                    Arguments = @"-newproject"
                };
            jumpList.JumpItems.Add(jumpTask);
            JumpList.SetJumpList(Current, jumpList);
        }

        #endregion
    }
}