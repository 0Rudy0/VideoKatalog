using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class SliderValueToRangeConverter : IMultiValueConverter {      

        #region IMultiValueConverter Members

        public object Convert (object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            double lowerValue = (double) values[0];
            double upperValue = (double) values[1];
            string type = parameter as string;
            if (type == "year")
                return string.Format ("{0} - {1}", lowerValue.ToString ("0."), upperValue.ToString ("0."));
            else if (type == "rating")
                return string.Format ("{0} - {1}", lowerValue.ToString ("0.0"), upperValue.ToString ("0.0"));
            else if (type == "size") {
                lowerValue = lowerValue / 1073741824; //1024^3  je 1073741824 - biteovi u GB
                upperValue = upperValue / 1073741824;
                return string.Format ("{0} GB - {1} GB", lowerValue.ToString ("0.0"), upperValue.ToString ("0.0"));
            }
            else if (type == "runtime") {
                lowerValue = lowerValue / 60000; //milisekunde u minute -> 60 * 1000
                upperValue = upperValue / 60000;
                return string.Format ("{0} min - {1} min", lowerValue.ToString ("0."), upperValue.ToString ("0."));
            }
            else if (type == "budget") {
                lowerValue = lowerValue / 1000000; //u milijun
                upperValue = upperValue / 1000000;
                return string.Format ("{0} mil - {1} mil", lowerValue.ToString ("0.0"), upperValue.ToString ("0.0"));
            }
            else if (type == "earnings") {
                lowerValue = lowerValue / 1000000; //u milijun
                upperValue = upperValue / 1000000;
                return string.Format ("{0} mil - {1} mil", lowerValue.ToString ("0.0"), upperValue.ToString ("0.0"));
            }
            else if (type == "bitrate") {
                lowerValue = lowerValue / 1048576; //1024ˇ2 je 1048576 - B/s u MB/s
                upperValue = upperValue / 1048576;
                return string.Format ("{0} Mbps - {1} Mbps", lowerValue.ToString ("0.0"), upperValue.ToString ("0.0"));
            }
            else
                return string.Format ("{0} - {1}", lowerValue.ToString (), upperValue.ToString ());
        }

        public object[] ConvertBack (object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
