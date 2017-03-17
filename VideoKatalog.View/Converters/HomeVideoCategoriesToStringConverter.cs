using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class HomeVideoCategoriesToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int maxLength = 1000;
            ObservableCollection<Category> categoryList;
            try {
                categoryList = (ObservableCollection<Category>) value;
                try {
                    maxLength = Int32.Parse (parameter.ToString ());
                }
                catch {
                }
            }
            catch {
                return "nije prosao converter";
            }
            string stringReturn = "";
            if (categoryList.Count == 0)
                return stringReturn;
            bool tooLong = false;
            foreach (Category tempCat in categoryList) {
                if ((stringReturn.Length + tempCat.Name.Length) > maxLength) {
                    tooLong = true;
                    stringReturn += "...";
                    break;
                }
                stringReturn += tempCat.Name + ", ";
            }
            if (tooLong == false) {
                //makni ", " na kraju ako string nije bio predugacak
                stringReturn = stringReturn.Substring (0, stringReturn.Length - 2);
            }
            return stringReturn;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
