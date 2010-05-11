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
    internal class App
    {
        #region Global Vars

        private static Application app;

        /// <summary>
        ///   Gets the resources for the application
        /// </summary>
        internal static ResourceManager RM { get; private set; }

        internal static string SUIFile { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        /// <param name = "args">Command line <c>args</c></param>
        [STAThread]
        private static void Main(string[] args)
        {
            Directory.CreateDirectory(SevenUpdate.Base.Base.UserStore);
            RM = new ResourceManager("SevenUpdate.Sdk.Resources.UIStrings", typeof (App).Assembly);
            SevenUpdate.Base.Base.SerializationErrorEventHandler += Base_SerializationErrorEventHandler;

            app = new Application();
            SetJumpLists();
            if (args.Length > 0)
                SUIFile = args[0];
            else
                app.Run(new MainWindow());
        }

        private static void Base_SerializationErrorEventHandler(object sender, SerializationErrorEventArgs e)
        {
            MessageBox.Show(RM.GetString("SuiInvalid") + " - " + e.Exception.Message);
        }

        private static void SetJumpLists()
        {
            //Configure a new JumpTask
            var jumpTask1 = new JumpTask
                                {
                                    ApplicationPath = SevenUpdate.Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                    IconResourcePath = SevenUpdate.Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                    Title = "Seven Update SDK",
                                    Description = "Create new project",
                                    CustomCategory = "Tasks",
                                    Arguments = "NewProject"
                                };

            var jumpTask2 = new JumpTask
                                {
                                    ApplicationPath = SevenUpdate.Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                    IconResourcePath = SevenUpdate.Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                    Title = "Seven Update SDK",
                                    Description = "Edit an existing project",
                                    CustomCategory = "Tasks",
                                    Arguments = "EditProject"
                                };

            var jumpTask3 = new JumpTask
                                {
                                    ApplicationPath = SevenUpdate.Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                    IconResourcePath = SevenUpdate.Base.Base.AppDir + "SevenUpdate.Sdk.exe",
                                    Title = "Seven Update SDK",
                                    Description = "Test project",
                                    CustomCategory = "Tasks",
                                    Arguments = "TestProject"
                                };

            var jumpList = new JumpList();
            jumpList.JumpItems.Add(jumpTask1);
            jumpList.JumpItems.Add(jumpTask2);
            jumpList.JumpItems.Add(jumpTask3);
            JumpList.SetJumpList(app, jumpList);
        }

        #endregion
    }
}