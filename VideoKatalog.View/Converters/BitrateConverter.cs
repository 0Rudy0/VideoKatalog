using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class BitrateConverter: IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            long bitrateBps = long.Parse (value.ToString());
            double bitrate;
            string bitrateRoundToString;

            bitrate = bitrateBps / 1024.0;
            bitrateRoundToString = ((int) bitrate).ToString ();
            if (bitrateRoundToString.Length < 4) {
                return bitrate.ToString (".0") + " Kbps";
            }
            bitrate = bitrate / 1024.0;
            bitrateRoundToString = ((int) bitrate).ToString ();
            return bitrate.ToString (".0") + " Mbps"; 
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
