using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class SizeConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            long sizeBytes = (long) value;
            double size = sizeBytes / 1024.0;
            string roundSizeToString;
            roundSizeToString = ((long)size).ToString();
            if (roundSizeToString.Length < 4) {
                return size.ToString(".0") + " KB";
            }
            size = size / 1024.0;
            roundSizeToString = ((long) size).ToString ();            
            if (roundSizeToString.Length < 4) {
                return size.ToString (".0") + " MB";
            }
            size = size / 1024.0;
            roundSizeToString = ((long) size).ToString ();
            if (roundSizeToString.Length < 4) {
                return size.ToString (".0") + " GB";
            }
            size = size / 1024.0;
            roundSizeToString = ((long) size).ToString ();
            return size.ToString (".0") + " TB";
            
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
