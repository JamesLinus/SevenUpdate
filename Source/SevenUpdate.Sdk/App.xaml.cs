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

using System;
using System.IO;
using System.Windows;
using Microsoft.Windows.Shell;
using SevenUpdate.Sdk.Properties;
using SevenUpdate.Sdk.Windows;

namespace SevenUpdate.Sdk
{
    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        private readonly Guid appGuid = new Guid("{82893A11-B7D9-48D2-87B1-9F6F19B17300}");

        protected override void OnStartup(StartupEventArgs e)
        {
            Directory.CreateDirectory(Core.UserStore);
            Base.SerializationError += Core.Base_SerializationError;
            Base.Locale = Settings.Default.locale;
            Core.SetJumpList();
            var si = new SingleInstance(appGuid);
            si.ArgsRecieved += si_ArgsRecieved;
            si.Run(() =>
            {
                new MainWindow().Show();
                return MainWindow;
            }, e.Args);
        }

        private void si_ArgsRecieved(string[] args)
        {
            if (args.Length > 0)
                switch (args[0])
                {
                    case "-newproject":
                        Core.NewProject();
                        break;

                    case "-newupdate":
                        Core.AppIndex = Convert.ToInt32(args[1]);

                        Core.NewUpdate();
                        break;

                    case "-edit":
                        Core.AppIndex = Convert.ToInt32(args[1]);
                        Core.UpdateIndex = Convert.ToInt32(args[2]);
                        Core.EditItem();
                        break;
                }
        }
    }
}