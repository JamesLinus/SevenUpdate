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
using System.Globalization;
using System.Resources;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Retrieves exception strings and other localized strings.
    /// </summary>
    internal static class SR
    {
        private static readonly ResourceManager resourceManager = new ResourceManager("ExceptionStringTable", typeof (SR).Assembly);

        internal static string Get(SRID id)
        {
            return resourceManager.GetString(id.String);
        }

        internal static string Get(SRID id, params object[] args)
        {
            var message = resourceManager.GetString(id.String);
            if (message != null)
            {
                // Apply arguments to formatted string (if applicable)
                if (args != null && args.Length > 0)
                    message = String.Format(CultureInfo.CurrentCulture, message, args);
            }

            return message;
        }

        // Get exception string resources for current locale
    }
}