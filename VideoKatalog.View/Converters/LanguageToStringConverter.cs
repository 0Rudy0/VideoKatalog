using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class LanguageToStringConverter: IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            try {
                int maxLength = 10000;
                ObservableCollection<Language> languageList = (ObservableCollection<Language>) value;
                try {
                    //maxLength = Int32.Parse (parameter.ToString ());
                }
                catch {
                }
                if (languageList == null || languageList.Count == 0)
                    return "(0)";

                bool tooLong = false;
                string toReturn = string.Format ("({0}):   ", languageList.Count.ToString ());
                foreach (Language tempLang in languageList) {
                    if ((toReturn.Length + tempLang.Name.Length) > maxLength) {
                        tooLong = true;
                        toReturn += "...";
                        break;
                    }
                    toReturn += tempLang.Name + ", ";
                }
                if (tooLong)
                    return toReturn;
                else {
                    toReturn = toReturn.Substring (0, toReturn.Length - 2);
                    return toReturn;
                }
            }
            catch {
                return "nije prosao converter";
            }
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
