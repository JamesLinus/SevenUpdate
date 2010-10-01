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
using Microsoft.Windows;
using SevenUpdate.Sdk.Properties;

#endregion

namespace SevenUpdate.Sdk
{
    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        /// <summary>
        ///   Raises the <see cref = "InstanceAwareApplication.Startup" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "System.Windows.StartupEventArgs" /> instance containing the event data.</param>
        /// <param name = "isFirstInstance">If set to <c>true</c> the current instance is the first application instance.</param>
        protected override void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            base.OnStartup(e, isFirstInstance);

            Base.SerializationError += Core.Base_SerializationError;
            Base.Locale = Settings.Default.locale;

            if (!isFirstInstance)
                Shutdown(1);

            Directory.CreateDirectory(Core.UserStore);
            Core.SetJumpList();
        }

        /// <summary>
        ///   Raises the <see cref = "InstanceAwareApplication.StartupNextInstance" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "Microsoft.Windows.StartupNextInstanceEventArgs" /> instance containing the event data.</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);

            if (e.Args.Length <= 0)
                return;
            switch (e.Args[0])
            {
                case "-newproject":
                    Core.NewProject();
                    break;

                case "-newupdate":
                    Core.AppIndex = Convert.ToInt32(e.Args[1]);

                    Core.NewUpdate();
                    break;

                case "-edit":
                    Core.AppIndex = Convert.ToInt32(e.Args[1]);
                    Core.UpdateIndex = Convert.ToInt32(e.Args[2]);
                    Core.EditItem();
                    break;
            }
        }
    }
}