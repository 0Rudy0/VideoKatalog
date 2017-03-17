using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class SummaryConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int maxLength = 10000;
            string summary = HttpUtility.HtmlDecode ((string) value);
            try {
                maxLength = Int32.Parse (parameter.ToString ());
                bool tooLong = false;
                while (summary.Length > maxLength) {
                    tooLong = true;
                    summary = summary.Substring (0, summary.Length - 1);
                }
                if (tooLong) {
                    return summary + "...";
                }
                else
                    return summary;
            }
            catch {
                return summary;
            }
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
