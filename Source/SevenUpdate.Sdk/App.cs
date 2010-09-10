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
using System.IO;
using System.Windows;
using System.Windows.Shell;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Properties;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk
{
    public sealed partial class App
    {
        #region Methods

        /// <summary>
        ///   Sets the Windows 7 JumpList
        /// </summary>
        private static void SetJumpLists()
        {
            // Create JumpTask
            var jumpList = new JumpList();

            //Configure a new JumpTask
            var jumpTask = new JumpTask
                               {
                                   ApplicationPath = Base.AppDir + "SevenUpdate.Sdk.exe",
                                   IconResourcePath = Base.AppDir + "SevenUpdate.Sdk.exe",
                                   Title = Sdk.Properties.Resources.SevenUpdateSDK,
                                   Description = "Create new project",
                                   CustomCategory = "Tasks",
                                   Arguments = "-new"
                               };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                           {
                               ApplicationPath = Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = Base.AppDir + "SevenUpdate.Sdk.exe",
                               Title = Sdk.Properties.Resources.SevenUpdateSDK,
                               Description = "Edit an existing project",
                               CustomCategory = "Tasks",
                               Arguments = "-edit"
                           };
            jumpList.JumpItems.Add(jumpTask);

            //jumpTask = new JumpTask
            //               {
            //                   ApplicationPath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
            //                   IconResourcePath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
            //                   Title = Sdk.Properties.Resources.SevenUpdateSDK,
            //                   Description = "Test project",
            //                   CustomCategory = Sdk.Properties.Resources.RecentProjects,
            //                   Arguments = "-open " + Base.
            //               };
            //jumpList.JumpItems.Add(jumpTask);


            JumpList.SetJumpList(Current, jumpList);
        }

        /// <summary>
        ///   Gets the app ready for startup
        /// </summary>
        /// <param name = "args">The command line arguments passed to the app</param>
        internal static void Init(string[] args)
        {
            Directory.CreateDirectory(Core.UserStore);
            Base.SerializationError += Core.Base_SerializationError;
            Base.Locale = Settings.Default.locale;
            SetJumpLists();
            if (args.Length <= 0)
                return;
            switch (args[0])
            {
                case "-edit":
                    //TODO open the "Open project window"
                    break;

                case "-open":
                    //TODO open the project for editing
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    ///   Interaction logic to load the app
    /// </summary>
    internal static class StartUp
    {
        /// <summary>
        ///   Initializes the app resources
        /// </summary>
        private static void InitResources()
        {
            if (Application.Current == null)
                return;
            if (Application.Current.Resources.MergedDictionaries.Count > 0)
                return;
            // merge in your application resources
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("SevenUpdate.Sdk;component/Resources/Dictionary.xaml", UriKind.Relative)) as ResourceDictionary);
        }

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        /// <param name = "args">Command line <c>args</c></param>
        [STAThread]
        private static void Main(string[] args)
        {
            var app = new Application();
            App.Init(args);
            InitResources();
            app.Run(new MainWindow());
        }
    }
}