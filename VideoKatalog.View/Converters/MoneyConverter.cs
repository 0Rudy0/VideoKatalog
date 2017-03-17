using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class MoneyConverter: IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            float moneyDecimal = (float) value;
            decimal d = Decimal.Parse (moneyDecimal.ToString (), System.Globalization.NumberStyles.Float);
            long money = long.Parse (d.ToString ());
            if (money == 0) {
                return "$" + " N/A";
            }
            string moneyString = money.ToString ();
            string reverseMoneyString = ReverseString (moneyString);
            try {
                reverseMoneyString = reverseMoneyString.Insert (3, ".");
                reverseMoneyString = reverseMoneyString.Insert (7, ".");
                reverseMoneyString = reverseMoneyString.Insert (11, ".");
            }
            catch {
            }
            finally {                
                moneyString = ReverseString (reverseMoneyString);
                if (moneyString[0] == '.') {
                    moneyString = moneyString.Substring (1);
                }
            }
            return "$" + moneyString;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            string moneyString = "";
            string valueInString = (string) value;
            if (valueInString.Contains ("N/A")) {
                return 0;
            }
            int countDigit = 0;
            foreach (char c in value.ToString ()) {
                if (Char.IsDigit (c)) {
                    countDigit++;
                    moneyString += c.ToString ();
                }
            }
            if (countDigit < 1) {
                return 0;
            }
            else {
                return Int64.Parse (moneyString);
            }
        }
        
        public static string ReverseString (string s) {
            char[] arr = s.ToCharArray ();
            Array.Reverse (arr);
            return new string (arr);
        }

        #endregion
    }
}
