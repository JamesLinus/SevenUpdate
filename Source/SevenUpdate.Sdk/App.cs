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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using Microsoft.Windows.Dialogs;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk
{
    public partial class App
    {
        #region Global Vars

        internal static string SuiFile { get; set; }

        internal static Sui SuiProject { get; set; }

        #endregion

        #region Methods

        private static void Base_SerializationError(object sender, SerializationErrorEventArgs e)
        {
            MessageBox.Show(Sdk.Properties.Resources.ProjectLoadError + " - " + e.Exception.Message);
        }

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
                                   ApplicationPath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
                                   IconResourcePath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
                                   Title = "Seven Update SDK",
                                   Description = "Create new project",
                                   CustomCategory = "Tasks",
                                   Arguments = "NewProject"
                               };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                           {
                               ApplicationPath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
                               Title = "Seven Update SDK",
                               Description = "Edit an existing project",
                               CustomCategory = "Tasks",
                               Arguments = "EditProject"
                           };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                           {
                               ApplicationPath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
                               Title = "Seven Update SDK",
                               Description = "Test project",
                               CustomCategory = "Tasks",
                               Arguments = "TestProject"
                           };
            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                           {
                               ApplicationPath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
                               IconResourcePath = SevenUpdate.Base.AppDir + "SevenUpdate.Sdk.exe",
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
            Directory.CreateDirectory(SevenUpdate.Base.UserStore);
            SevenUpdate.Base.SerializationError += Base_SerializationError;
            if (args.Length > 0)
                SuiFile = args[0];

            SetJumpLists();
        }

        /// <summary>
        ///   Checks if a file or UNC is valid
        /// </summary>
        /// <param name = "path">The path we want to check</param>
        /// <param name = "is64Bit">Specifies if the application is 64 bit</param>
        public static bool IsValidFilePath(string path, bool is64Bit)
        {
            path = SevenUpdate.Base.ConvertPath(path, true, is64Bit);
            const string pattern = @"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$";
            var reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(path);
        }

        internal static void ShowInputErrorMessage()
        {
            if (TaskDialog.IsPlatformSupported)
            {
                var td = new TaskDialog
                             {
                                 Caption = Sdk.Properties.Resources.SevenUpdateSDK,
                                 InstructionText = Sdk.Properties.Resources.CorrectErrors,
                                 Icon = TaskDialogStandardIcon.Warning,
                                 FooterText = Sdk.Properties.Resources.ErrorHelp,
                                 FooterIcon = TaskDialogStandardIcon.Information,
                                 Cancelable = false
                             };
                td.ShowDialog(Current.MainWindow);
            }
            else
            {
                MessageBox.Show(Sdk.Properties.Resources.CorrectErrors + Environment.NewLine + Sdk.Properties.Resources.ErrorHelp, Sdk.Properties.Resources.SevenUpdateSDK, MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }

        #endregion

        internal static void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
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

    public static class StringExtensions
    {
        /// <summary>
        ///   Checks if a given string contains any of the strings in the passed array of strings.
        /// </summary>
        /// <param name = "str">The string to check against values</param>
        /// <param name = "values">An array of strings to compare to the given string</param>
        /// <returns><c>True</c> if string contains any of the given strings, otherwise <c>False</c></returns>
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length == 0)
                return values.Any(str.Contains);

            return false;
        }

        /// <summary>
        ///   Checks if a given string contains any of the characters in the passed array of <c>characters</c>.
        /// </summary>
        /// <param name = "str">The string to check against values</param>
        /// <param name = "values">An array of characters to compare to the given string</param>
        /// <returns><c>True</c> if string contains any of the given strings, otherwise <c>False</c></returns>
        public static bool ContainsAny(this string str, params char[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length == 0)
                return values.Any(str.Contains);

            return false;
        }
    }
}