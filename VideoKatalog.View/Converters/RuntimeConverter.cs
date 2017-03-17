using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Video_katalog.Converters {
    class RuntimeConverter: IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            long runtimeMS = long.Parse(value.ToString());
            double runtime = runtimeMS / 1000; //sekunde
            runtime = runtime / 60; //minute
            return runtime.ToString ("0.") + " min";
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
