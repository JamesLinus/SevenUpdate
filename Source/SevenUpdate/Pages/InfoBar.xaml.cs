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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    ///   Interaction logic for InfoBar.xaml
    /// </summary>
    public partial class InfoBar : UserControl
    {
        #region Global Properties

        /// <summary>
        ///   The shield icon uri
        /// </summary>
        internal ImageSource ShieldIcon { set { shieldIcon.Source = value; } }

        /// <summary>
        ///   The side image uri
        /// </summary>
        internal ImageSource SideImage { set { sideImage.Source = value; } }

        #endregion

        /// <summary>
        ///   A Control that displays update progress and information
        /// </summary>
        public InfoBar()
        {
            InitializeComponent();
            tbViewOptionalUpdates.MouseEnter += App.TextBlock_MouseEnter;
            tbViewImportantUpdates.MouseEnter += App.TextBlock_MouseEnter;
            tbViewOptionalUpdates.MouseLeave += App.TextBlock_MouseLeave;
            tbViewImportantUpdates.MouseLeave += App.TextBlock_MouseLeave;
        }
    }
}