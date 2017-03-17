using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class StringListToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int maxLength = 1000;
            List<string> stringList;
            try {
                stringList = (List<string>) value;
                maxLength = Int32.Parse (parameter.ToString ());
            }
            catch {
                return "nije prosao converter";
            }
            string stringReturn = "";
            bool tooLong = false;
            foreach (String tempString in stringList) {
                if ((stringReturn.Length + tempString.Length) > maxLength) {
                    tooLong = true;
                    stringReturn += "...";
                    break;
                }
                stringReturn += tempString + ", ";
            }
            if (tooLong == false) {
                //makni ", " na kraju ako string nije bio predugacak
                stringReturn = stringReturn.Substring (0, stringReturn.Length - 2);
            }
            return stringReturn;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
