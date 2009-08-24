/*Copyright 2007, 2008 Robert Baker, aka Seven ALive.
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
using System.Collections.ObjectModel;

namespace SevenUpdate
{
    #region Application Settings

    /// <summary>
    /// Configuration options
    /// </summary>
    public class UpdateOptions
    {
        /// <summary>
        /// Specifies which update setting Seven Update should use
        /// </summary>
        public AutoUpdateOption AutoOption { get; set; }
        /// <summary>
        /// Specifes if recommended updates should be included when download/install
        /// </summary>
        public bool Recommended { get; set; }
    }

    /// <summary>
    /// Automatic Update option Seven Update can use
    /// </summary>
    public enum AutoUpdateOption
    {
        /// <summary>
        /// Download and Installs updates automatically
        /// </summary>
        Install,

        /// <summary>
        /// Downloads Updates automatically
        /// </summary>
        Download,

        /// <summary>
        /// Only checks and notifies the user of updates
        /// </summary>
        Notify,

        /// <summary>
        /// No automatic checking
        /// </summary>
        Never
    }

    #endregion

    public class UpdateSettings
    {
        /// <summary>
        /// List of Application Seven Update can check for updates
        /// </summary>
        public static ObservableCollection<SUA> Applications
        {
            get
            {
                return Shared.DeserializeCollection<SUA>(Shared.appStore + "SUApps.sul");
            }
        }

        /// <summary>
        /// The update settings for Seven Update
        /// </summary>
        public static UpdateOptions Settings
        {
            get
            {
                return Shared.Deserialize<UpdateOptions>(Shared.appStore + "Settings.xml");
            }
        }
    }
}
