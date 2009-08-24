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
using System.Globalization;
using System.Windows.Forms;

namespace SevenUpdate
{
    public class NumericTextBox : TextBox
    {
        bool allowSpace;

        // Restricts the entry of characters to digits (including hex), the negative sign,
        // the decimal point, and editing keystrokes (backspace).

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;

            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;

            string groupSeparator = numberFormatInfo.NumberGroupSeparator;

            string negativeSign = numberFormatInfo.NegativeSign;

            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }
            else
                if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) || keyInput.Equals(negativeSign))
                {
                    // Decimal separator is OK
                }
                else if (e.KeyChar == '\b')
                {
                    // Backspace key is OK
                }
                else if (this.allowSpace && e.KeyChar == ' ')
                {

                }
                else
                {
                    // Consume this invalid key and beep
                    e.Handled = true;
                    //    MessageBeep();
                }
        }

        public int IntValue
        {
            get
            {
                return Int32.Parse(this.Text);
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return Decimal.Parse(this.Text);
            }
        }

        public bool AllowSpace
        {
            set
            {
                this.allowSpace = value;
            }

            get
            {
                return this.allowSpace;
            }
        }
    }
}
