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
using System.Resources;
using System.Windows;
using System.Windows.Shell;
using SevenUpdate.Base;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk
{
    public partial class App
    {
        #region Global Vars

        /// <summary>
        ///   Gets the resources for the application
        /// </summary>
        internal static ResourceManager RM { get; private set; }

        internal static string SuiFile { get; set; }

        #endregion

        #region Methods

        private static void Base_SerializationErrorEventHandler(object sender, SerializationErrorEventArgs e)
        {
            MessageBox.Show(RM.GetString("SuiInvalid") + " - " + e.Exception.Message);
        }

        private static void SetJumpLists()
        {
            // Create JumpTask
            var jumpList = new JumpList();

            //Configure a new JumpTask
            var jumpTask = new JumpTask
                               {
                                   ApplicationPath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                   IconResourcePath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                   Title = "Seven Update SDK",
                                   Description = "Create new project",
                                   CustomCategory = "Tasks",
                                   Arguments = "NewProject"
                               };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                           {
                               ApplicationPath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                               Title = "Seven Update SDK",
                               Description = "Edit an existing project",
                               CustomCategory = "Tasks",
                               Arguments = "EditProject"
                           };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                           {
                               ApplicationPath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                               Title = "Seven Update SDK",
                               Description = "Test project",
                               CustomCategory = "Tasks",
                               Arguments = "TestProject"
                           };
            jumpList.JumpItems.Add(jumpTask);
            jumpTask = new JumpTask
                           {
                               ApplicationPath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                               Title = "Seven Update SDK",
                               Description = "Test project",
                               CustomCategory = "Tasks",
                               Arguments = "TestProject"
                           };
            jumpList.JumpItems.Add(jumpTask);

            JumpList.SetJumpList(Current, jumpList);
        }

        /// <summary>
        ///   Gets the app ready for startup
        /// </summary>
        /// <param name = "args">The command line arguments passed to the app</param>
        internal static void Init(string[] args)
        {
            Directory.CreateDirectory(Base.Base.UserStore);
            RM = new ResourceManager("SevenUpdate.Sdk.Resources.UIStrings", ResourceAssembly);
            Base.Base.SerializationErrorEventHandler += Base_SerializationErrorEventHandler;
            if (args.Length > 0)
                SuiFile = args[0];

            SetJumpLists();
        }

        #endregion
    }

    /// <summary>
    ///   Interaction logic to load the app
    /// </summary>
    public static class StartUp
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