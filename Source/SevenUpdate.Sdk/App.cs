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
using SevenUpdate.Sdk.Properties;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk
{
    public sealed partial class App
    {
        #region Fields

        internal static string[] Args;

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the app ready for startup
        /// </summary>
        internal static void Init()
        {
            Directory.CreateDirectory(Core.UserStore);
            Base.SerializationError += Core.Base_SerializationError;
            Base.Locale = Settings.Default.locale;
            Core.SetJumpLists();
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
            App.Args = args;
            App.Init();
            InitResources();
            app.Run(new MainWindow());
        }
    }
}