//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

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
        private static ResourceManager resourceManager = new ResourceManager("ExceptionStringTable", typeof (SR).Assembly);

        internal static string Get(SRID id)
        {
            return resourceManager.GetString(id.String);
        }

        internal static string Get(SRID id, params object[] args)
        {
            string message = resourceManager.GetString(id.String);
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