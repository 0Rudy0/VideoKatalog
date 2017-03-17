using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;

namespace Video_katalog.Converters {
    class NameToImagePathConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            string name = (string) value;
            name = removeIllegalChars (name);
            string category = parameter.ToString ();
            string strUri2;
            switch (category) {
                case "movie":
                    strUri2 = HardcodedStrings.moviePosterFolder + name + ".jpg";
                    break;
                case "serie":
                    strUri2 = HardcodedStrings.seriePosterFolder + name + ".jpg";
                    break;
                case "homeVideo":
                    strUri2 = HardcodedStrings.homeVideoPosterFolder + name + ".jpg";
                    break;
                default:
                    strUri2 = "";
                    break;
            }

            try {
                BitmapImage imageSource = new BitmapImage (new Uri (strUri2));
                imageSource.CacheOption = BitmapCacheOption.OnDemand;
                //imageSource.DecodePixelHeight = 431;
                //imageSource.DecodePixelWidth = 300;                
                return imageSource;
            }
            catch {
                try {
                    strUri2 = HardcodedStrings.posterFolder + "default.png";
                    BitmapImage imageSource = new BitmapImage (new Uri (strUri2));
                    imageSource.CacheOption = BitmapCacheOption.OnDemand;
                    return imageSource;
                }
                catch {
                    return null;
                }
            }
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        string removeIllegalChars (string name) {
            return name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "");
        }

        #endregion
    }
}
