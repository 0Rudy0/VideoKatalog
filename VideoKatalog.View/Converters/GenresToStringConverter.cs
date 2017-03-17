using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class GenresToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            ObservableCollection<Genre> stringList;
            try {
                stringList = (ObservableCollection<Genre>) value;                
            }
            catch {
                return "nije prosao converter";
            }
            string stringReturn = "";
            if (stringList.Count == 0)
                return stringReturn;
            bool tooLong = false;
            foreach (Genre tempGenre in stringList) {                
                stringReturn += tempGenre.Name + ", ";
            }
            //makni ", " na kraju
            stringReturn = stringReturn.Substring (0, stringReturn.Length - 2);       
     
            return stringReturn;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
