using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class DateConverter: IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return "";
            DateTime date = (DateTime) value;
            string dateReturnString = date.Day.ToString ("00.") + "." + date.Month.ToString ("00.") + "." + date.Year.ToString () + ".";
            return dateReturnString;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
