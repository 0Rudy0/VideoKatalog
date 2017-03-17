using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;

namespace Video_katalog.Converters {
    class SelectedPropertyToBackgroundConverter : IValueConverter{
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            Brush background;
            if ((bool)value) {
                background = Brushes.Green;
                return background;
            }
            else {
                background = Brushes.Transparent;
                return background;
            }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
