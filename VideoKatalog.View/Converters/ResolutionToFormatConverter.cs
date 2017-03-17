using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class ResolutionToFormatConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int height = (int)value;
            //sd
            if (height < 1200) {
                if (parameter.ToString() == "sd") {
                    return 1;
                }
                else if (parameter.ToString() == "720p") {
                    return 0.08;
                }
                else if (parameter.ToString() == "1080p") {
                    return 0.08;
                }
            }
                //720p
            else if (height < 1900) {
                if (parameter.ToString() == "sd") {
                    return 0.08;
                }
                else if (parameter.ToString() == "720p") {
                    return 1;
                }
                else if (parameter.ToString() == "1080p") {
                    return 0.08;
                }
            }
                //1080p
            else {
                if (parameter.ToString() == "sd") {
                    return 0.08;
                }
                else if (parameter.ToString() == "720p") {
                    return 0.08;
                }
                else if (parameter.ToString() == "1080p") {
                    return 1;
                }
            }
            return 0.10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
