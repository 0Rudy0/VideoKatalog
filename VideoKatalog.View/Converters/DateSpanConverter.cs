using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class DateSpanConverter : IValueConverter{
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            string param = (string) parameter;
            if (param == "serie") {
                try {
                    ObservableCollection<WishSerieSeason> seasonList = (ObservableCollection<WishSerieSeason>) value;
                    DateTime dateStart = seasonList.ElementAt (0).WishEpisodes.ElementAt (0).AirDate;
                    DateTime dateEnd = seasonList.ElementAt (0).WishEpisodes.ElementAt (0).AirDate;
                    foreach (WishSerieSeason tempSeason in seasonList) {
                        foreach (WishSerieEpisode tempEpisode in tempSeason.WishEpisodes) {
                            if (tempEpisode.AirDate < dateStart)
                                dateStart = tempEpisode.AirDate;
                            if (tempEpisode.AirDate > dateEnd)
                                dateEnd = tempEpisode.AirDate;
                        }
                    }
                    if (dateStart == dateEnd)
                        return dateStart.ToShortDateString ();
                    else
                        return (dateStart.ToShortDateString () + " - " + dateEnd.ToShortDateString ());
                }
                catch {
                    return "-";
                }
            }
            else if (param == "season") {
                try {
                    ObservableCollection<WishSerieEpisode> episodeList = (ObservableCollection<WishSerieEpisode>) value;
                    DateTime dateStart = new DateTime (3000, 1, 1);
                    DateTime dateEnd = new DateTime (1000, 1, 1);
                    foreach (WishSerieEpisode tempEpisode in episodeList) {
                        if (tempEpisode.AirDate < dateStart)
                            dateStart = tempEpisode.AirDate;
                        if (tempEpisode.AirDate > dateEnd)
                            dateEnd = tempEpisode.AirDate;
                    }
                    if (dateStart == dateEnd)
                        return dateStart.ToShortDateString ();
                    else
                        return (dateStart.ToShortDateString () + " - " + dateEnd.ToShortDateString ());
                }
                catch {
                    return "-";
                }
            }
            return "-";
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
