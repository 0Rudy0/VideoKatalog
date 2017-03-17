using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;

namespace Video_katalog.Converters {
    class IsViewedConverter: IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            try {
                bool isViewed = (bool) value;
                if (isViewed) {
                    return "Resources/greenCheck50p.png";
                }
                else {
                    return "Resources/redCross50p.png";
                }
            }
            catch {
                return null;
            }
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
