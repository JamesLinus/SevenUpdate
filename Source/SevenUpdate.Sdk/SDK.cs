#region GNU Public License v3

// Copyright 2007-2010 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
//  
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//  
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Windows.Forms;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.Sdk
{
    internal static class SDK
    {
        #region Global Variables

        internal static SUI Application;

        #endregion

        #region Methods

        internal static void RemoveUpdate(int index)
        {
            Application.Updates.RemoveAt(index);
        }

        internal static bool LoadSUI(string suiLoc)
        {
            try
            {
                Application = Base.Base.Deserialize<SUI>(suiLoc);
                return Application != null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }

        internal static void SaveSUI(string suiLoc)
        {
            Base.Base.Serialize(Application, suiLoc);
        }

        #endregion
    }
}