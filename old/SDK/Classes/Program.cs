/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Resources;
namespace SevenUpdate.SDK
{
    static class Program
    {
        /// <summary>
        /// The UI Resource Strings
        /// </summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.SDK.UIStrings", typeof(Program).Assembly);


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
                System.Windows.Forms.Application.Run(new Main(args[0]));
            else
                System.Windows.Forms.Application.Run(new Main());
        }
    }
}
