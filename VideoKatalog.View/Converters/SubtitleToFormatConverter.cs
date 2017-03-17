using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class SubtitleToFormatConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            //string subtitleLanguage = parameter.ToString();
            ObservableCollection<Language> subLanguageList = (ObservableCollection<Language>)value;
            foreach (Language tempLang in subLanguageList) {
                if (tempLang.Name == "Croatian" && parameter.ToString() == "hrv")
                    return 1;
                else if (tempLang.Name == "English" && parameter.ToString() == "eng")
                    return 1;
            }
            return 0.06;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
