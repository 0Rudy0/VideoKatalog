using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class ResolutionToStringConverter :IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int width = (int) value;
            if (width > 1910) {
                return "1080p";
            }
            else if (width > 959) {
                return "720p";
            }
            else {
                return "SD";
            }

        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion       
    }
}
