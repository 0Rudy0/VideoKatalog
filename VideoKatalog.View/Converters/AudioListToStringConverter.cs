using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class AudioListToStringConverter: IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            try {
                ObservableCollection<Audio> audioList = (ObservableCollection<Audio>) value;
                if (audioList == null || audioList.Count == 0)
                    return "(0)";

                string toReturn = string.Format ("({0}):   ", audioList.Count.ToString ());
                foreach (Audio tempAudio in audioList)
                    toReturn += tempAudio.ToString () + " // ";

                //makni " // " na kraju
                toReturn = toReturn.Substring (0, toReturn.Length - 4);
                return toReturn;
            }
            catch {
                return "(0)";
            }
            
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
