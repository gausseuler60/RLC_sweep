using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;

namespace Shell
{
    class InputValidator
    {
        public static void HandleKeyEvent(KeyPressEventArgs e, bool AllowDecimal = true, bool AllowE = false)
        {
            if (!((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == '-') || (e.KeyChar == '.' && AllowDecimal) || (e.KeyChar == 'E' && AllowE) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public static T HandleTextFieldChange<T>(TextBox field) where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            T nValue;
            CultureInfo myCIintl = new CultureInfo("en-US", false);
            try
            {
                nValue = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(null, myCIintl, field.Text);  // convert to numeric type
                field.BackColor = SystemColors.Window;
                return nValue;
            }
            catch
            {
                field.BackColor = Color.Red;

                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertTo(-1, typeof(T)); //(object)(-1);
            }
        }
    }
}
