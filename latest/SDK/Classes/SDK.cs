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
namespace SevenUpdate.SDK
{
    static class SUSDK
    {
        #region Global Variables

        internal static Application application;

        #endregion

        #region Methods

        internal static void RemoveUpdate(int index)
        {
            application.Updates.RemoveAt(index);
        }

        internal static void LoadSUI(string suiLoc)
        {
            if (application.Updates != null)
                application.Updates.Clear();

            try
            {
                application = Shared.Deserialize<Application>(suiLoc);
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        internal static void SaveSUI(string suiLoc)
        {
            Shared.Serialize<Application>(application, suiLoc);
        }

        #endregion
    }
}
