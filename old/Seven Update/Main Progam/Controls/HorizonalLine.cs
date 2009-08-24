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
using System.ComponentModel;
using System.Windows.Forms;

namespace System.Drawing
{
    /// <summary>
    /// Displays a horizonal line
    /// </summary>
    public partial class LineSeparator : Panel
    {
        [DefaultValueAttribute(typeof(Color), "None"), CategoryAttribute("Appearance"), DescriptionAttribute("The color of the line")]
        public override Color BackColor { get; set; }

        public LineSeparator()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont; 
            
            InitializeComponent();

            this.Paint += new PaintEventHandler(LineSeparator_Paint);

            this.MaximumSize = new Size(2000, 2);

            this.MinimumSize = new Size(0, 2);

            this.Width = 350;
        }

        void LineSeparator_Paint(object sender, PaintEventArgs e)
        {
            Pen pen;
            if (BackColor == null)
                pen = new Pen(Color.LightGray);
            else
                pen = new Pen(BackColor);

            Graphics g = e.Graphics;
            g.DrawLine(pen, new Point(0, 0), new Point(this.Width, 0));
            g.DrawLine(Pens.White, new Point(0, 1), new Point(this.Width, 1));
        }
    }
}
