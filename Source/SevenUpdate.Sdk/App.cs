// // Copyright 2007-2010 Robert Baker, Seven Software.
// // This file is part of Seven Update.
// //  
// //     Seven Update is free software: you can redistribute it and/or modify
// //     it under the terms of the GNU General Public License as published by
// //     the Free Software Foundation, either version 3 of the License, or
// //     (at your option) any later version.
// // 
// //     Seven Update is distributed in the hope that it will be useful,
// //     but WITHOUT ANY WARRANTY; without even the implied warranty of
// //     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// //     GNU General Public License for more details.
// //  
// //    You should have received a copy of the GNU General Public License
// //    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.IO;
using System.Resources;
using System.Windows;
using SevenUpdate.Base;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk
{
    internal class App
    {
        #region Global Vars

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
            Directory.CreateDirectory(Base.Base.UserStore);
            RM = new ResourceManager("SevenUpdate.Sdk.Resources.UIStrings", typeof (App).Assembly);
            Base.Base.SerializationErrorEventHandler += Base_SerializationErrorEventHandler;

            var app = new Application();
            if (args.Length > 0)
                SUIFile = args[0];
            else
                app.Run(new MainWindow());
        }

        private static void Base_SerializationErrorEventHandler(object sender, SerializationErrorEventArgs e)
        {
            MessageBox.Show(RM.GetString("SuiInvalid") + " - " + e.Exception.Message);
        }

        #endregion
    }
}