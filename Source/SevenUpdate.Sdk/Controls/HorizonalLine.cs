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

using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace System.Drawing
{
    /// <summary>
    /// Displays a horizonal line
    /// </summary>
    public sealed partial class LineSeparator : Panel
    {
        public LineSeparator()
        {
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            Paint += LineSeparator_Paint;

            MaximumSize = new Size(2000, 2);

            MinimumSize = new Size(0, 2);

            Width = 350;
        }

        [DefaultValue(typeof (Color), "None"), Category("Appearance"), Description("The color of the line")]
        public override Color BackColor { get; set; }

        private void LineSeparator_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;
            using (var pen = new Pen(BackColor))
            {
                g = e.Graphics;

                g.DrawLine(pen, new Point(0, 0), new Point(Width, 0));
            }

            g.DrawLine(Pens.White, new Point(0, 1), new Point(Width, 1));
        }
    }
}